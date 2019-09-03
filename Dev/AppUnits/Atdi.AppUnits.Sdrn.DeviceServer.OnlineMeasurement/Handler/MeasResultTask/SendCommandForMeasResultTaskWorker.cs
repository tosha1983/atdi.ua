using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Linq;
using System.Collections.Generic;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using System.Collections;
using System.ComponentModel;
using Atdi.Platform.Logging;



namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Results
{
    public class SendCommandForMeasResultTaskWorker
    {
        private readonly AppServerComponentConfig _config;
        private readonly IController _controller;
        private readonly ILogger _logger;

        public SendCommandForMeasResultTaskWorker(AppServerComponentConfig config, IController controller, ILogger logger)
        {
            this._config = config;
            this._controller = controller;
            this._logger = logger;
        }

        public bool Handle(ITaskContext<ClientReadyTakeMeasResultTask, OnlineMeasurementProcess> context, MesureTraceCommand deviceCommand, out DeviceServerResultLevel deviceServerResultLevel)
        {
            deviceServerResultLevel = null;
            bool isSuccessOperation = false;

            this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
            (
                ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
            ) =>
            {
                taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(failureReason, ex));
            });

      
            bool isDown = context.WaitEvent<DeviceServerResultLevel>(out deviceServerResultLevel, (int)(deviceCommand.Timeout));
            if (isDown == false) // таймут - результатов нет
            {
                var error = new ExceptionProcessLevel();
                if (context.WaitEvent<ExceptionProcessLevel>(out error, 1) == true)
                {
                    var checkErrorAdapter = new CheckErrorAdapterForMeasResultTaskWorker(this._config, this._controller, this._logger);
                    var deviceServerCancellationData = checkErrorAdapter.Handle(context, error, out deviceServerResultLevel);
                    if (deviceServerCancellationData != null)
                    {

                        var message = new OnlineMeasMessage
                        {
                            Kind = OnlineMeasMessageKind.DeviceServerCancellation,
                            Container = deviceServerCancellationData
                        };

                        // и  отправляем DeviceServerCancellationData
                        context.Process.Publisher.Send(message);

                        _logger.StartTrace(Contexts.ThisComponent, Categories.SendCommandForMeasResultTaskWorker, Events.ErrorReceivingResult);
                        isSuccessOperation = false;
                    }
                    else
                    {
                        isSuccessOperation = true;
                    }
                }
            }
            else
            {
                isSuccessOperation = true;
            }

            if ((deviceServerResultLevel != null) && (isSuccessOperation==true))
            {
                // так оборачиваем результат
                var message = new OnlineMeasMessage
                {
                    Kind = OnlineMeasMessageKind.DeviceServerMeasResult,
                    Container = deviceServerResultLevel
                };
                // и  отправляем его
                context.Process.Publisher.Send(message);
            }

            return isSuccessOperation;
        }
    }
}
