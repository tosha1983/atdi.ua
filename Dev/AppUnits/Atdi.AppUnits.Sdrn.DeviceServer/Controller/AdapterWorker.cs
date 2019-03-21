using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using Atdi.Common;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class AdapterWorker : IDevice, IAdapterHost, IDisposable
    {
        private readonly Type _adapterType;
        private readonly IAdapterFactory _adapterFactory;
        private readonly ICommandsHost _commandsHost;
        private readonly IResultsHost _resultsHost;
        private readonly ITimeService _timeService;
        private readonly IEventWaiter _eventWaiter;
        private readonly ILogger _logger;
        private readonly int _abortingTimeout = 1000;
        private Thread _adapterThread;
        private IAdapter _adapterObject;
        private volatile bool _isDisposing;

        private readonly ConcurrentDictionary<Guid, ExecutionContext> _executingCommands;
        private readonly CommandsBuffer _buffer;
        private readonly Dictionary<Type, CommandHandler> _commandHandlers;

        private volatile DeviceState _state;

        private readonly Dictionary<CommandType, CommandLock> _enumLocks;
        private readonly Dictionary<Type, CommandLock> _typeLocks;

        private readonly Dictionary<CommandType, IDeviceProperties> _properties;
        private long _counter;

        public DeviceState State => _state;

        public Type AdapterType => this._adapterType;

        public AdapterWorker(
            Type adapterType, 
            IAdapterFactory adapterFactory, 
            ICommandsHost commandsHost, 
            IResultsHost resultsHost, 
            ITimeService timeService,
            IEventWaiter eventWaiter,
            ILogger logger)
        {
            this._adapterType = adapterType ?? throw new ArgumentNullException(nameof(adapterType));
            this._adapterFactory = adapterFactory ?? throw new ArgumentNullException(nameof(adapterFactory));
            this._commandsHost = commandsHost;
            this._resultsHost = resultsHost;
            this._timeService = timeService;
            this._eventWaiter = eventWaiter;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.DeviceId = Guid.NewGuid();
            this._executingCommands = new ConcurrentDictionary<Guid, ExecutionContext>();
            this._buffer = new CommandsBuffer();
            this._commandHandlers = new Dictionary<Type, CommandHandler>();
            this._enumLocks = new Dictionary<CommandType, CommandLock>();
            this._typeLocks = new Dictionary<Type, CommandLock>();
            this._properties = new Dictionary<CommandType, IDeviceProperties>();
            this._counter = 0;
            this._state = DeviceState.Created;
        }

        public Guid DeviceId { get; private set; }

        public long Counter => this._counter;

        public void Run()
        {
            this._adapterThread = new Thread(this.Process)
            {
                Name = $"Adapter.[{this._adapterType.Namespace}].{this._adapterType.Name}",
                Priority = ThreadPriority.Highest
            };

            this._adapterThread.Start();

            _logger.Verbouse(Contexts.AdapterWorker, Categories.Running, Events.RanAdapterThread.With(_adapterType));
        }

        private void Process()
        {
            try
            {
                try
                {
                    this._adapterObject = this._adapterFactory.Create(this._adapterType);
                    _logger.Verbouse(Contexts.AdapterWorker, Categories.Processing, Events.CreatedAdapter.With(_adapterType));

                    this._adapterObject.Connect(this);
                    this._state = DeviceState.Available;
                    _logger.Verbouse(Contexts.AdapterWorker, Categories.Processing, Events.ConnectedAdapter.With(_adapterType));
                }
                catch(Exception)
                {
                    this._state = DeviceState.Failure;
                    throw;
                }
                finally
                {
                    this._eventWaiter.Emit(this);
                }

                while (true)
                {
                    var start = _timeService.TimeStamp.Milliseconds;
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    var commandDescriptor = _buffer.Take();
                    
                    var waitTime = _timeService.TimeStamp.Milliseconds - start;
                    timer.Stop();

                    this._state = DeviceState.Basy;
                    var command = commandDescriptor.Command;

                    using (var scope = this._logger.StartTrace(Contexts.AdapterWorker, Categories.Executing, TraceScopeNames.ExecutingAdapterCommand.With(commandDescriptor.CommandType.Name, command.Type, command.Id, _adapterType, waitTime, timer.Elapsed.TotalMilliseconds)))
                    {
                        //_logger.Verbouse(Contexts.AdapterWorker, Categories.Processing, Events.TookCommand.With(_adapterType, command.Type, command.ParameterType));

                        if (command.StartTimeStamp > 0 && command.Timeout > 0)
                        {
                            if (!_timeService.TimeStamp.HitMilliseconds(command.StartTimeStamp, command.Timeout, out long lateness))
                            {
                                this._state = DeviceState.Available;
                                commandDescriptor.Reject(CommandFailureReason.TimeoutExpired);
                                _logger.Warning(Contexts.AdapterWorker, Categories.Processing, Events.RejectedCommand.With(_adapterType, command.Type, command.ParameterType, CommandFailureReason.TimeoutExpired));
                                _logger.Debug(Contexts.AdapterWorker, Categories.Processing, Events.RejectedCommandByTimeout.With(command.Delay, command.Timeout, lateness));
                                // ignore this command so to wait next command
                                continue;
                            }
                        }

                        // пока команда доходжила ее могли отменить
                        if (commandDescriptor.CancellationToken.IsCancellationRequested)
                        {
                            this._state = DeviceState.Available;
                            commandDescriptor.Reject(CommandFailureReason.CanceledBeforeExecution);
                            continue;
                        }

                        // нужно убедится что именно эту комманду поддерживает адаптер 
                        // и именно ее он в стостяни в текущий момент времени обработать
                        // так допсуается ситуаци язапуска в отдельном потоке измерения по отдельнйо комманде
                        if (!this.CheckLockState(commandDescriptor.CommandType, command.Type))
                        {
                            this._state = DeviceState.Available;
                            commandDescriptor.Reject(CommandFailureReason.DeviceIsBusy);
                            _logger.Warning(Contexts.AdapterWorker, Categories.Processing, Events.RejectedCommand.With(_adapterType, command.Type, command.ParameterType, CommandFailureReason.DeviceIsBusy));
                            // ignore this command so to wait next command
                            continue;
                        }

                        // выполням комманду
                        this.StartExecutingCommand(commandDescriptor);
                        this._state = DeviceState.Available;
                        //_logger.Verbouse(Contexts.AdapterWorker, Categories.Processing, Events.TransferCommand.With(_adapterType, command.Type, command.ParameterType));
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                
                if (_adapterObject != null && _isDisposing)
                {
                    try
                    {
                        _adapterObject.Disconnect();
                        this._state = DeviceState.Aborted;
                    }
                    catch (Exception e)
                    {
                        _logger.Exception(Contexts.AdapterWorker, Categories.Processing, Events.DisconnectingAdapterError.With(_adapterType.FullName), e);
                    }
                    this._adapterObject = null;
                    return;
                }
                this._state = DeviceState.Aborted;
                _logger.Critical(Contexts.AdapterWorker, Categories.Processing, Events.AbortAdapterThread.With(_adapterType.FullName));
            }
            catch(Exception e)
            {
                this._state = DeviceState.Failure;
                _logger.Critical(Contexts.AdapterWorker, Categories.Processing, Events.ProcessingAdapterError.With(_adapterType.FullName), e);
            }
        }

        public void Dispose()
        {
            try
            {
                _state = DeviceState.Basy;

                if (this._adapterThread != null)
                {
                    this._isDisposing = true;
                    this._adapterThread.Abort();
                    this._adapterThread.Join(_abortingTimeout);
                    
                    this._adapterThread = null;
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.AdapterWorker, Categories.Disposing, e);
            }
            
        }

        public void RegisterHandler<TCommand, TResult>(Action<TCommand, IExecutionContext> commandHandler, IDeviceProperties deviceProperties = null) 
            where TCommand : new()
        {
            var commandType = typeof(TCommand);
            if (this._commandHandlers.ContainsKey(commandType))
            {
                throw new InvalidOperationException("The command handler was previously registered");
            }

            var handler = new CommandHandler(commandHandler.Method, commandType, typeof(TResult));
            this._commandHandlers.Add(commandType, handler);

            var typeLock = new CommandLock();
            this._typeLocks.Add(commandType, typeLock);

            var dummyCommand = (ICommand)new TCommand();
            if (!this._enumLocks.ContainsKey(dummyCommand.Type))
            {
                var enumLock = new CommandLock();
                this._enumLocks.Add(dummyCommand.Type, enumLock);
            }

            _commandsHost.Register(this, dummyCommand.Type, commandType);

            if (deviceProperties != null)
            {
                if (_properties.ContainsKey(dummyCommand.Type))
                {
                    throw new InvalidOperationException("The command handler was previously registered");
                }
                deviceProperties.DeviceId = this.DeviceId;
                _properties.Add(dummyCommand.Type, deviceProperties);
            }
        }


        private void StartExecutingCommand(CommandDescriptor commandDescriptor)
        {
            var handler = this._commandHandlers[commandDescriptor.CommandType];

            var command = commandDescriptor.Command;

            var resultBuffer = this._resultsHost.TakeBuffer(commandDescriptor);

            var executionContext = new ExecutionContext(commandDescriptor, this, resultBuffer);
            this._executingCommands.TryAdd(command.Id, executionContext);

            commandDescriptor.Process();

            try
            {
                using (var scope = this._logger.StartTrace(Contexts.AdapterWorker, Categories.Executing, TraceScopeNames.InvokingAdapterCommand.With(command.Id)))
                {
                    handler.Invoker(this._adapterObject, command, executionContext);
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.AdapterWorker, Categories.Processing, e);
                commandDescriptor.Abort(CommandFailureReason.Exception, e);
                this.FinalizeCommand(commandDescriptor, resultBuffer);
            }
        }

        public bool TryPushCommand(ICommandDescriptor descriptor)
        {
            Interlocked.Increment(ref _counter);
            return _buffer.TryPush(descriptor as CommandDescriptor);
        }

        public bool CheckAbilityToExecute(ICommandDescriptor descriptor)
        {
            var descriptorObject = descriptor as CommandDescriptor;

            if (_state != DeviceState.Available)
            {
                return false;
            }
            if (!_commandHandlers.ContainsKey(descriptorObject.CommandType))
            {
                return false;
            }

            return this.CheckLockState(descriptorObject.CommandType, descriptor.Command.Type);
        }

        private bool CheckLockState(Type instanceType, CommandType commandType)
        {
            if (!_enumLocks.TryGetValue(commandType, out CommandLock enumLock))
            {
                return false;
            }
            if (enumLock.State != CommandLockState.Unlocked)
            {
                return false;
            }

            if (!_typeLocks.TryGetValue(instanceType, out CommandLock typeLock))
            {
                return false;
            }
            if (typeLock.State != CommandLockState.Unlocked)
            {
                return false;
            }
            return true;
        }

        public void Lock(CommandType commandType)
        {
            var commandLock = this._enumLocks[commandType];
            commandLock.Lock();
        }
        public void Lock(Type commandType)
        {
            var commandLock = this._typeLocks[commandType];
            commandLock.Lock();
        }

        public void Unlock(CommandType commandType)
        {
            var commandLock = this._enumLocks[commandType];
            commandLock.Unlock();
        }
        public void Unlock(Type commandType)
        {
            var commandLock = this._typeLocks[commandType];
            commandLock.Unlock();
        }

        public void FinalizeCommand(CommandDescriptor descriptor, IResultBuffer resultBuffer)
        {
            var command = descriptor.Command;
            this._resultsHost.ReleaseBuffer(resultBuffer);
            this._executingCommands.TryRemove(command.Id, out ExecutionContext context);
            _logger.Verbouse(Contexts.AdapterWorker, Categories.Processing, Events.FinalizedCommand.With(_adapterType, command.Type, command.ParameterType));
        }

        public IDeviceProperties EnsureProperties(CommandType commandType)
        {
            this._properties.TryGetValue(commandType, out IDeviceProperties deviceProperties);
            return deviceProperties;
        }
    }
}
