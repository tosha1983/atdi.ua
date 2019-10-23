using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal class ResultWorker
    {
        private readonly CommandDescriptor _descriptor;
        private readonly IResultConvertorsHost _convertorsHost;
        private readonly ILogger _logger;
        private readonly ResultHandler _resultHandler;
        private readonly ResultBuffer _resultBuffer;
        private readonly Task _task;

        public ResultWorker(CommandDescriptor descriptor, IResultConvertorsHost convertorsHost, IResultHandlersHost handlersHost, ILogger logger)
        {
            this._descriptor = descriptor;
            this._convertorsHost = convertorsHost;
            this._resultHandler = (ResultHandler)handlersHost.GetHandler(descriptor.CommandType, descriptor.ResultType, descriptor.TaskType, descriptor.ProcessType);
            this._logger = logger;
            this._task = new Task(this.Process);
            this._resultBuffer = new ResultBuffer(descriptor);
        }

        public ResultBuffer Run()
        {
            this._task.Start();
            return _resultBuffer;
        }

        private void Process()
        {
            try
            {
                _resultHandler.StartedWorkerCounter?.Increment();
                _resultHandler.RunningWorkerCounter?.Increment();

                var convertor = default(IResultConvertor);
                var prevResultPartType = default(Type);

                while (true)
                {
                    var resultPart = this._resultBuffer.Take();
                    if (resultPart == null)
                    {
                        /// null - признак окончания процесса - cancel
                        break;
                    }

                    var resultPartType = resultPart.GetType();

                    try
                    {
                        /// тип адаптера равен типу обработчика, нет смысла в конвертации
                        if (resultPartType == _descriptor.ResultType)
                        {
                            this._resultHandler.Handle(_descriptor.Command, resultPart, this._descriptor.TaskContext);
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
                            var handlerResult = convertor.Convert(resultPart, _descriptor.Command);
                            this._resultHandler.Handle(_descriptor.Command, handlerResult,
                                this._descriptor.TaskContext);
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
                        break;
                    }
                }

                _resultHandler.RunningWorkerCounter?.Decrement();
                _resultHandler.FinishedWorkerCounter?.Increment();
            }
            catch (Exception e)
            {
                _resultHandler.RunningWorkerCounter?.Decrement();
                _resultHandler.AbortedWorkerCounter?.Increment();
                this._logger.Exception(Contexts.ResultWorker, Categories.Processing,
                    Events.ProcessingResultError.With(_descriptor.Device.AdapterType, _descriptor.CommandType), e);
            }
            finally
            {
                this._resultBuffer.Dispose();
            }
        }

        
        public void Stop()
        {
            // ничего делать не нужно - юуфер сам освободитьс якогда поток результатов перестанет поступать, 
            // так как его нужно обрботать если данные бегут
            //this._resultBuffer.Cancel();
        }
    }
}
