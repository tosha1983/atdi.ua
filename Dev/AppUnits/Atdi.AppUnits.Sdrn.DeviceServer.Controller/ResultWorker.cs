using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class ResultWorker
    {
        private readonly CommandDescriptor _descriptor;
        private readonly IResultConvertorsHost _convertorsHost;
        private readonly ILogger _logger;
        private readonly IResultHandler _resultHandler;
        private readonly ResultBuffer _resultBuffer;
        private readonly Task _task;

        public ResultWorker(CommandDescriptor descriptor, IResultConvertorsHost convertorsHost, IResultHandlersHost handlersHost, ILogger logger)
        {
            this._descriptor = descriptor;
            this._convertorsHost = convertorsHost;
            this._resultHandler = handlersHost.GetHandler(descriptor.CommandType, descriptor.ResultType, descriptor.ContextType);
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
            var convertor = default(IResultConvertor);
            var prevResultPartType = default(Type);

            while (true)
            {
                var resultPart = this._resultBuffer.Take();
                if (resultPart == null)
                {
                    /// null - признак окончания процесса
                    return;
                }

                var resultPartType = resultPart.GetType();

                try
                {
                    /// тип адаптера равен типу обработчика, нет смысла в конвертации
                    if (resultPartType == _descriptor.ResultType)
                    {
                        this._resultHandler.Handle(_descriptor.Command, resultPart, this._descriptor.Context);
                    }
                    else
                    {
                        // микро оптимизация - как правило в 90% результаты будут приходит в однмо типе
                        if (convertor == null || prevResultPartType == null && prevResultPartType != resultPartType)
                        {
                            convertor = this._convertorsHost.GetConvertor(resultPartType, this._descriptor.ResultType);
                        }
                        prevResultPartType = resultPartType;

                        /// конвеер обработки результатов
                        var handlerResult = convertor.Convert(resultPart, _descriptor.Command, this._descriptor.Context);
                        this._resultHandler.Handle(_descriptor.Command, handlerResult, this._descriptor.Context);
                    }
                }
                catch (Exception e)
                {

                    this._logger.Exception(Contexts.ResultWorker, Categories.Processing, Events.ProcessingResultError.With(_descriptor.Device.AdapterType, _descriptor.CommandType), e);
                }
            }
        }

        
        public void Stop()
        {
            this._resultBuffer.Cancel();
        }
    }
}
