using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.EntityOrm;
using Atdi.DataModels.Sdrns.Device;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class SignalizationTaskWorker : ITaskWorker<SignalizationTask, SignalizationProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IController _controller;
        private readonly IBusGate _busGate;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;


        public SignalizationTaskWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IBusGate busGate,
            IController controller)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._busGate = busGate;
            this._controller = controller;
        }


        public void Run(ITaskContext<SignalizationTask, SignalizationProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.SOTaskWorker, Categories.Measurements, Events.StartSOTaskWorker.With(context.Task.Id));

                DateTime currTime = DateTime.Now;

                var deviceCommand = new MesureTraceCommand(context.Task.mesureTraceParameter);

                this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                (
                    ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                ) =>
                {
                    //taskContext.SetEvent<ExceptionProcessSignalization>(new ExceptionProcessSignalization(failureReason, ex));
                });
                //////////////////////////////////////////////
                // 
                // Получение очередного  результат от Result Handler
                //
                //////////////////////////////////////////////
                MeasResults outResultData = null;
                bool isDown = context.WaitEvent<MeasResults>(out outResultData, 1000);
                if (isDown == false) // таймут - результатов нет
                {

                }
                else
                {
                    // есть результат
                }


            }
            catch (Exception e)
            {
                _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Exceptions.UnknownErrorSOTaskWorker, e.Message);
                context.Abort(e);
            }
        }

    }
}
