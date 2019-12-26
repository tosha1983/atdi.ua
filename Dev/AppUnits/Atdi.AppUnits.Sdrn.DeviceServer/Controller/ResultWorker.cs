using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Data;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal sealed class ResultWorker : IDisposable
    {
        public readonly object Locker = new object();

        private CommandContext _context;
        private CommandDescriptor _descriptor;
        private readonly IResultConvertorsHost _convertorsHost;
        private readonly IResultHandlersHost _handlersHost;
        private readonly IObjectPoolSite _poolSite;
        private readonly ILogger _logger;
        private ResultHandler _resultHandler;
        private readonly IResultBuffer _resultBuffer;
        //private readonly Task _task;
        private Thread _workerThread;
        private readonly CancellationTokenSource _source;

        public ResultWorker(IResultBuffer resultBuffer, IResultConvertorsHost convertorsHost, IResultHandlersHost handlersHost, IObjectPoolSite poolSite, ILogger logger)
        {
            this._source = new CancellationTokenSource();
            this._resultBuffer = resultBuffer;
            this._convertorsHost = convertorsHost;
            this._handlersHost = handlersHost;
            this._poolSite = poolSite;
            this._logger = logger;
            this._workerThread = new Thread(this.Process)
            {
                Name = $"ATDI.DeviceServer.ResultWorker.[{resultBuffer.Id.ToString()}]"
            };

            this._workerThread.Start();
        }

        public bool Processing { get; private set; }

        public void Use(CommandDescriptor descriptor, CommandContext context)
        {
            //_logger.Info(Contexts.ResultWorker, Categories.Creating,
            //    $"Use new descriptor: old=#{_descriptor?.Command.Id.ToString()}, new={descriptor.Command.Id.ToString()}, Processing={this.Processing.ToString()}");

            this._descriptor = descriptor;
            this._context = context;

            try
            {
                this._resultHandler = (ResultHandler)_handlersHost.GetHandler(descriptor.CommandType, descriptor.ResultType, descriptor.TaskType, descriptor.ProcessType);
                _resultHandler?.StartedWorkerCounter?.Increment();
                _resultHandler?.RunningWorkerCounter?.Increment();

                this.Processing = true;
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ResultWorker, Categories.Initializing,
                    Events.DefiningResultHandlerError.With(_descriptor.Device.AdapterType, _descriptor.CommandType), e);
            }

        }


        private void Process()
        {
            try
            {
                var convertor = default(IResultConvertor);
                var prevResultPartType = default(Type);
                var token = _source.Token;
                
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        if (this.Processing)
                        {
                            _resultHandler?.RunningWorkerCounter?.Decrement();
                            _resultHandler?.FinishedWorkerCounter?.Increment();
                            lock (Locker)
                            {
                                this.Processing = false;
                                this._context.FinishResultProcessing();
                            }
                        }
                        return;
                    }

                    if (!this._resultBuffer.TryTake(out var resultPart, token))
                    {
                        if (this.Processing)
                        {
                            _resultHandler?.RunningWorkerCounter?.Decrement();
                            _resultHandler?.FinishedWorkerCounter?.Increment();
                            
                            lock (Locker)
                            {
                                this.Processing = false;
                                this._context.FinishResultProcessing();
                            }
                        }
                        return;
                    }

                    if (resultPart == null)
                    {
                        // null - признак окончания процесса - cancel
                        _resultHandler?.RunningWorkerCounter?.Decrement();
                        _resultHandler?.FinishedWorkerCounter?.Increment();
                        lock (Locker)
                        {
                            this.Processing = false;
                            this._context.FinishResultProcessing();
                        }
                        
                        continue;
                    }

                    var resultPartType = resultPart.GetType();

                    try
                    {
                        /// тип адаптера равен типу обработчика, нет смысла в конвертации
                        if (resultPartType == _descriptor.ResultType)
                        {
                            var poolId = resultPart.PoolId;
                            try
                            {
                                this._resultHandler?.Handle(_descriptor.Command, resultPart,
                                    this._descriptor.TaskContext);
                            }
                            finally
                            {
                                // сброс идетификатора пула означет призанк освобожения объектиа
                                if (poolId != Guid.Empty && poolId == resultPart.PoolId)
                                {
                                    this.ReleaseResult(resultPart);
                                }
                            }
                            
                        }
                        else
                        {
                            // микро оптимизация - как правило в 90% результаты будут приходит в однмо типе
                            if (convertor == null || prevResultPartType == null && prevResultPartType != resultPartType)
                            {
                                convertor = this._convertorsHost.GetConvertor(resultPartType,
                                    this._descriptor.ResultType);
                            }

                            prevResultPartType = resultPartType;

                            /// конвеер обработки результатов
                            var poolId = resultPart.PoolId;
                            try
                            {
                                // тут возможен трафик - желательно вконверторах также использовать пул объектов
                                var convertedResultPartResult = convertor.Convert(resultPart, _descriptor.Command);
                                this._resultHandler?.Handle(_descriptor.Command, convertedResultPartResult,
                                    this._descriptor.TaskContext);
                            }
                            finally
                            {
                                // сброс идетификатора пула означет призанк освобожения объектиа
                                if (poolId != Guid.Empty && poolId == resultPart.PoolId)
                                {
                                    this.ReleaseResult(resultPart);
                                }
                            }
                            
                        }
                    }
                    catch (Exception e)
                    {

                        this._logger.Exception(Contexts.ResultWorker, Categories.Processing,
                            Events.ProcessingResultError.With(_descriptor.Device.AdapterType, _descriptor.CommandType),
                            e);
                    }

                    // возможно стоит прекратить процесс
                    if (resultPart.Status == CommandResultStatus.Final ||
                        resultPart.Status == CommandResultStatus.Ragged)
                    {
                        _resultHandler?.RunningWorkerCounter?.Decrement();
                        _resultHandler?.FinishedWorkerCounter?.Increment();

                        lock (Locker)
                        {
                            this.Processing = false;
                            this._context.FinishResultProcessing();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _resultHandler?.RunningWorkerCounter?.Decrement();
                _resultHandler?.AbortedWorkerCounter?.Increment();
                this._logger.Critical(Contexts.ResultWorker, Categories.Processing,
                    Events.ProcessingResultError.With(_descriptor.Device.AdapterType, _descriptor.CommandType), e);
            }

        }

        private void ReleaseResult(ICommandResultPart result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            var pool = this.GetResultPool(result.PoolKey, result);
            result.PoolId = Guid.Empty;
            pool.PutObject(result);
        }

        private IObjectPool GetResultPool(string key, ICommandResultPart resultPart)
        {
            //var poolKey = AdapterWorker.BuildPoolKey(_descriptor.Command.Type, key);
            var resultPoolKey = new ValueTuple<Type, CommandType, string>(resultPart.GetType(), _descriptor.Command.Type, key);
            var pool = _context.ResultsPool[resultPoolKey];
            return pool;
        }


        public void Dispose()
        {
            _source.Cancel();
            _resultBuffer.Cancel();
        }
    }
}
