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
    public class SendCommandForRegistrationTaskWorker
    {
        private readonly AppServerComponentConfig _config;
        private readonly IController _controller;
        private readonly ILogger _logger;

        public SendCommandForRegistrationTaskWorker(AppServerComponentConfig config, IController controller, ILogger logger)
        {
            this._config = config;
            this._controller = controller;
            this._logger = logger;
        }

        public bool Handle(ITaskContext<ClientTaskRegistrationTask, OnlineMeasurementProcess> context, MesureTraceCommand deviceCommand, out DeviceServerParametersDataLevel deviceServerParametersDataLevel)
        {
            deviceServerParametersDataLevel = null;
            bool isSuccessOperation = false;

            this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
            (
                ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
            ) =>
            {
                taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(failureReason, ex));
            });

      
            bool isDown = context.WaitEvent<DeviceServerParametersDataLevel>(out deviceServerParametersDataLevel, (int)(deviceCommand.Timeout));
            if (isDown == false) // таймут - результатов нет
            {
                var error = new ExceptionProcessLevel();
                if (context.WaitEvent<ExceptionProcessLevel>(out error, 1) == true)
                {
                    var checkErrorAdapter = new CheckErrorAdapterForRegistrationTaskWorker(this._config, this._controller, this._logger);
                    var deviceServerCancellationData = checkErrorAdapter.Handle(context, error, out deviceServerParametersDataLevel);
                    if (deviceServerCancellationData != null)
                    {
                        var message = new OnlineMeasMessage
                        {
                            Kind = OnlineMeasMessageKind.DeviceServerCancellation,
                            Container = deviceServerCancellationData
                        };

                        // и  отправляем DeviceServerCancellationData
                        context.Process.Publisher.Send(message);
                        _logger.StartTrace(Contexts.ThisComponent, Categories.SendCommandForRegistrationTaskWorker, Events.ErrorReceivingResult);
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

            if ((deviceServerParametersDataLevel != null) && (isSuccessOperation==true))
            {
                var serverParams = new DeviceServerParametersDataLevel
                {
                    SensorToken = context.Process.SensorToken,
                    Att_dB = deviceServerParametersDataLevel.Att_dB,
                    PreAmp_dB = deviceServerParametersDataLevel.PreAmp_dB,
                    RefLevel_dBm = deviceServerParametersDataLevel.RefLevel_dBm,
                    RBW_kHz = deviceServerParametersDataLevel.RBW_kHz,
                    Freq_Hz = deviceServerParametersDataLevel.Freq_Hz,
                    isChanged_Att_dB = context.Process.MeasTask.Att_dB == deviceServerParametersDataLevel.Att_dB ? true : false,
                    isChanged_PreAmp_dB = context.Process.MeasTask.PreAmp_dB == deviceServerParametersDataLevel.PreAmp_dB ? true : false,
                    isChanged_RBW_kHz = context.Process.MeasTask.RBW_kHz == deviceServerParametersDataLevel.RBW_kHz ? true : false,
                    isChanged_RefLevel_dBm = context.Process.MeasTask.RefLevel_dBm == deviceServerParametersDataLevel.RefLevel_dBm ? true : false
                };
                context.Process.Parameters = serverParams;
            }
            return isSuccessOperation;
        }
    }
}
