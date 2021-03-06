﻿using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using Atdi.Platform;
using Atdi.Platform.Data;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal class AdapterWorker : IDevice, IAdapterHost, IDisposable
    {
        public const int CommandContextPoolObjectsDefaultValue = 10;

        private readonly IDeviceServerConfig _config;
        private readonly Type _adapterType;
        private readonly IAdapterFactory _adapterFactory;
        private readonly ICommandsHost _commandsHost;
        private readonly IResultsHost _resultsHost;
        private readonly IResultConvertorsHost _convertorsHost;
        private readonly IResultHandlersHost _handlersHost;
        private readonly ITimeService _timeService;
        private readonly IEventWaiter _eventWaiter;
        private readonly IStatistics _statistics;
        private readonly IObjectPoolSite _poolSite;
        private readonly ILogger _logger;
        private readonly int _abortingTimeout = 1000;
        private Thread _adapterThread;
        private IAdapter _adapterObject;
        private volatile bool _isDisposing;

        private readonly Dictionary<CommandType, IObjectPool<CommandContext>> _pools;
        private readonly Dictionary<ValueTuple<Type, CommandType, string>, IObjectPool> _resultsPool;

        //private readonly ConcurrentDictionary<Guid, ExecutionContext> _executingCommands;
        private readonly CommandsBuffer _buffer;
        private readonly Dictionary<Type, CommandHandler> _commandHandlers;

        private volatile DeviceState _state;

        private readonly Dictionary<CommandType, CommandLock> _enumLocks;
        private readonly Dictionary<Type, CommandLock> _typeLocks;

        private readonly Dictionary<CommandType, IDeviceProperties> _properties;
        private readonly IStatisticCounter _adaptersCommandsHitsCounter;
        private readonly IStatisticCounter _adaptersCommandsCanceledCounter;
        private readonly IStatisticCounter _adaptersCommandsShotsCounter;
        private long _counter;

        public DeviceState State => _state;

        public Type AdapterType => this._adapterType;

        public AdapterWorker(
            IDeviceServerConfig config,
            Type adapterType, 
            IAdapterFactory adapterFactory, 
            ICommandsHost commandsHost, 
            IResultsHost resultsHost,
            IResultConvertorsHost convertorsHost, 
            IResultHandlersHost handlersHost,
            ITimeService timeService,
            IEventWaiter eventWaiter,
            IStatistics statistics,
            IObjectPoolSite poolSite,
            ILogger logger)
        {
            this._config = config;
            this._adapterType = adapterType ?? throw new ArgumentNullException(nameof(adapterType));
            this._adapterFactory = adapterFactory ?? throw new ArgumentNullException(nameof(adapterFactory));
            this._commandsHost = commandsHost;
            this._resultsHost = resultsHost;
            this._convertorsHost = convertorsHost;
            this._handlersHost = handlersHost;
            this._timeService = timeService;
            this._eventWaiter = eventWaiter;
            this._statistics = statistics;
            this._poolSite = poolSite;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.DeviceId = Guid.NewGuid();
            this._pools = new Dictionary<CommandType, IObjectPool<CommandContext>>();
            this._resultsPool = new Dictionary<(Type, CommandType, string), IObjectPool>();
            //this._executingCommands = new ConcurrentDictionary<Guid, ExecutionContext>();
            this._buffer = new CommandsBuffer();
            this._commandHandlers = new Dictionary<Type, CommandHandler>();
            this._enumLocks = new Dictionary<CommandType, CommandLock>();
            this._typeLocks = new Dictionary<Type, CommandLock>();
            this._properties = new Dictionary<CommandType, IDeviceProperties>();
            this._counter = 0;
            this._state = DeviceState.Created;

            if (this._statistics != null)
            {
                _statistics.Counter(Monitoring.AdaptersCountKey)?.Increment();
                this._adaptersCommandsShotsCounter = _statistics.Counter(Monitoring.AdaptersCommandsShotsKey);
                this._adaptersCommandsHitsCounter = _statistics.Counter(Monitoring.AdaptersCommandsHitsKey);
                this._adaptersCommandsCanceledCounter = _statistics.Counter(Monitoring.AdaptersCommandsCanceledKey);
            }
        }

        public Guid DeviceId { get; private set; }

        public long Counter => this._counter;

        public void Run()
        {
            this._adapterThread = new Thread(this.Process)
            {
                Name = $"ATDI.DeviceServer.Adapter.[{this._adapterType.Namespace}].{this._adapterType.Name}",
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
                    //var start = _timeService.TimeStamp.Milliseconds;
                    //var timer = System.Diagnostics.Stopwatch.StartNew();
                    var commandDescriptor = _buffer.Take();
                    
                    //var waitTime = _timeService.TimeStamp.Milliseconds - start;
                    //timer.Stop();

                    this._state = DeviceState.Basy;
                    var command = commandDescriptor.Command;

                    this._adaptersCommandsShotsCounter?.Increment();

                    //using (var scope = this._logger.StartTrace(Contexts.AdapterWorker, Categories.Executing, TraceScopeNames.ExecutingAdapterCommand.With(commandDescriptor.CommandType.Name, command.Type, command.Id, _adapterType, waitTime, timer.Elapsed.TotalMilliseconds)))
                    {
                        //_logger.Verbouse(Contexts.AdapterWorker, Categories.Processing, Events.TookCommand.With(_adapterType, command.Type, command.ParameterType));

                        if (command.StartTimeStamp > 0 && command.Timeout > 0)
                        {
                            if (!_timeService.TimeStamp.HitMilliseconds(command.StartTimeStamp, command.Timeout, out long lateness))
                            {
                                this._adaptersCommandsCanceledCounter?.Increment();

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
                            this._adaptersCommandsCanceledCounter?.Increment();

                            this._state = DeviceState.Available;
                            commandDescriptor.Reject(CommandFailureReason.CanceledBeforeExecution);
                            continue;
                        }

                        // нужно убедится что именно эту комманду поддерживает адаптер 
                        // и именно ее он в стостяни в текущий момент времени обработать
                        // так допсуается ситуаци язапуска в отдельном потоке измерения по отдельнйо комманде
                        if (!this.CheckLockState(commandDescriptor.CommandType, command.Type))
                        {
                            this._adaptersCommandsCanceledCounter?.Increment();

                            this._state = DeviceState.Available;
                            commandDescriptor.Reject(CommandFailureReason.DeviceIsBusy);
                            _logger.Warning(Contexts.AdapterWorker, Categories.Processing, Events.RejectedCommand.With(_adapterType, command.Type, command.ParameterType, CommandFailureReason.DeviceIsBusy));
                            // ignore this command so to wait next command
                            continue;
                        }

                        // выполням комманду
                        this._adaptersCommandsHitsCounter?.Increment();
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

                    _statistics.Counter(Monitoring.AdaptersCountKey)?.Decrement();
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.AdapterWorker, Categories.Disposing, e);
            }
            
        }

        public void RegisterHandler<TCommand, TResult>(Action<TCommand, IExecutionContext> commandHandler, IResultPoolDescriptor<TResult>[] poolDescriptors,  IDeviceProperties deviceProperties) 
            where TCommand : new()
        {
            var commandType = typeof(TCommand);
            if (this._commandHandlers.ContainsKey(commandType))
            {
                throw new InvalidOperationException("The command handler was previously registered");
            }

            var handler = new CommandHandler(commandHandler.Method, commandType, typeof(TResult), _statistics);
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

            // регестрируем пул контекстов комманд
            if (!_pools.ContainsKey(dummyCommand.Type))
            {
                var type = dummyCommand.Type;
                var pool = _poolSite.Register(new ObjectPoolDescriptor<CommandContext>()
                {
                    Key = $"AdapterWork[{this.DeviceId.ToString()}].CommandContexts.#" + ((int)dummyCommand.Type).ToString(),
                    MinSize = 0,
                    MaxSize = _config.CommandContextPoolObjects.GetValueOrDefault(CommandContextPoolObjectsDefaultValue),
                    Factory = () =>
                    {
                        var buffer = this._resultsHost.TakeBuffer();
                        var worker = new ResultWorker(buffer, this._convertorsHost, this._handlersHost, this._poolSite, this._logger);
                        return new CommandContext(this._resultsPool)
                        {
                            Type = type,
                            Handler = handler,
                            Buffer = buffer,
                            Worker = worker,
                            ExecutionContext = new ExecutionContext(this, buffer, _poolSite, _statistics)
                        };
                    }
                });
                _pools[dummyCommand.Type] = pool;
            }

            // регистрация пулов результатов
            if (poolDescriptors != null && poolDescriptors.Length > 0)
            {
                foreach (var poolDescriptor in poolDescriptors)
                {
                    this.RegisterPoolResult(dummyCommand.Type, poolDescriptor);
                }
            }

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

        private string BuildPoolKey(CommandType commandType, string key)
        {
            return string.Concat($"AdapterWorker[{this.DeviceId.ToString()}].CommandResults.#", ((int)commandType).ToString(), "|", key);
        }
        
        private void RegisterPoolResult<TResult>(CommandType commandType, IResultPoolDescriptor<TResult> descriptor)
        {
            var poolDescriptor = new ObjectPoolDescriptor<TResult>()
            {
                Factory = descriptor.Factory,
                Key = this.BuildPoolKey(commandType, descriptor.Key),
                MinSize = descriptor.MinSize,
                MaxSize = descriptor.MaxSize,
            };

            var pool = _poolSite.Register(poolDescriptor);
            _resultsPool[new ValueTuple<Type, CommandType, string>(typeof(TResult), commandType, descriptor.Key)] = pool;
            _logger.Verbouse(Contexts.AdapterWorker, Categories.Initializing, $"Result object pool was registered: {pool.ToString()}");
        }

        private void StartExecutingCommand(CommandDescriptor commandDescriptor)
        {
            

            var command = commandDescriptor.Command;

            //var handler = this._commandHandlers[commandDescriptor.CommandType];
            //var resultBuffer = this._resultsHost.TakeBuffer(commandDescriptor);
            //var executionContext = new ExecutionContext(commandDescriptor, this, resultBuffer, this._poolSite, this._statistics);

            var pool = _pools[command.Type];

            if (!pool.TryTake(out var commandContext))
            {
                do
                {
                    Thread.SpinWait(100);
                } while (!pool.TryTake(out commandContext));
            }
            
            //var commandContext = pool.Take();

            commandContext.Use(commandDescriptor);
            //commandContext.ExecutionContext.Use(commandDescriptor, commandContext);

            //this._executingCommands.TryAdd(command.Id, commandContext.ExecutionContext);

            commandDescriptor.Process();

            try
            {
                //using (var scope = this._logger.StartTrace(Contexts.AdapterWorker, Categories.Executing, TraceScopeNames.InvokingAdapterCommand.With(command.Id)))
                {
                    commandContext.Handler.StartedCounter?.Increment();
                    commandContext.Handler.RunningCounter?.Increment();
                    commandContext.Handler.Invoker(this._adapterObject, command, commandContext.ExecutionContext);
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.AdapterWorker, Categories.Processing, e);
                commandDescriptor.Abort(CommandFailureReason.Exception, e);
                this.FinalizeCommand(commandDescriptor, commandContext);
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

        public void FinalizeCommand(CommandDescriptor descriptor, CommandContext context)
        {
            
            var command = descriptor.Command;
            var pool = _pools[command.Type];

            //this._resultsHost.ReleaseBuffer(resultBuffer);

            //this._executingCommands.TryRemove(command.Id, out _);

            var handler = context.Handler; // this._commandHandlers[descriptor.CommandType];
            handler.RunningCounter?.Decrement();
            if (command.State == CommandState.Done)
            {
                handler.CompletedCounter?.Increment();
            }
            else if (command.State == CommandState.Cancelled)
            {
                handler.CanceledCounter?.Increment();
            }
            else if (command.State == CommandState.Aborted)
            {
                handler.AbortedCounter?.Increment();
            }

            context.Free(pool);
            
            //_logger.Verbouse(Contexts.AdapterWorker, Categories.Processing, Events.FinalizedCommand.With(_adapterType, command.Type, command.ParameterType));
        }

        public IDeviceProperties EnsureProperties(CommandType commandType)
        {
            this._properties.TryGetValue(commandType, out IDeviceProperties deviceProperties);
            return deviceProperties;
        }
    }
}
