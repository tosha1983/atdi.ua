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
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.Platform.Logging;


namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Results
{
    public class CheckErrorAdapterForRegistrationTaskWorker
    {
        private readonly AppServerComponentConfig _config;
        private readonly IController _controller;
        private readonly ILogger _logger;


        public CheckErrorAdapterForRegistrationTaskWorker(AppServerComponentConfig config, IController controller, ILogger logger)
        {
            this._config = config;
            this._controller = controller;
            this._logger = logger;
        }

     
        public DeviceServerCancellationData Handle(ITaskContext<ClientTaskRegistrationTask, OnlineMeasurementProcess> context, ExceptionProcessLevel exceptionProcessLevel, out DeviceServerParametersDataLevel deviceServerParametersDataLevel)
        {
            DeviceServerCancellationData deviceServerCancellationData = null;
            deviceServerParametersDataLevel = null;
            var sendCommandForRegistrationTaskWorker = new SendCommandForRegistrationTaskWorker(this._config, this._controller, this._logger);
            if (exceptionProcessLevel._ex != null)
            {
                deviceServerCancellationData = new DeviceServerCancellationData();
                deviceServerCancellationData.SensorToken = context.Process.SensorToken;
                deviceServerCancellationData.Message = exceptionProcessLevel._ex.Message;
                /// реакция на ошибку выполнения команды
                switch (exceptionProcessLevel._failureReason)
                {
                    // повторяем 50 раз иначе ошибка (повтор через 1/25 сек)
                    case CommandFailureReason.DeviceIsBusy:
                        for (int i=0; i<50; i++)
                        {
                            var measTraceParameter = ConvertToMesureTraceParameterForLevel.ConvertForLevel(context.Process.MeasTask);
                            var deviceCommand = new MesureTraceCommand(measTraceParameter);
                            deviceCommand.Timeout = this._config.maximumDurationMeasLevel_ms;
                            deviceCommand.Delay = 0;
                            deviceCommand.Options = CommandOption.StartImmediately;

                            if (sendCommandForRegistrationTaskWorker.Handle(context, deviceCommand, out deviceServerParametersDataLevel) == false)
                            {
                                System.Threading.Thread.Sleep(this._config.minimumTimeDurationLevel_ms);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        deviceServerCancellationData.FailureCode = FailureReason.DeviceIsBusy;
                        break;
                    //  (возвращение ошибки)
                    case CommandFailureReason.CanceledExecution:
                        deviceServerCancellationData.FailureCode = FailureReason.CanceledExecution;
                        break;
                    // повторяем 10 раз иначе ошибка (повтор через 1/25 сек)
                    case CommandFailureReason.TimeoutExpired:
                        for (int i = 0; i < 10; i++)
                        {
                            var measTraceParameter = ConvertToMesureTraceParameterForLevel.ConvertForLevel(context.Process.MeasTask);
                            var deviceCommand = new MesureTraceCommand(measTraceParameter);
                            deviceCommand.Timeout = this._config.maximumDurationMeasLevel_ms;
                            deviceCommand.Delay = 0;
                            deviceCommand.Options = CommandOption.StartImmediately;

                            if (sendCommandForRegistrationTaskWorker.Handle(context, deviceCommand, out deviceServerParametersDataLevel) == false)
                            {
                                System.Threading.Thread.Sleep(this._config.minimumTimeDurationLevel_ms);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        deviceServerCancellationData.FailureCode = FailureReason.TimeoutExpired;
                        break;
                    //  (возвращение ошибки)
                    case CommandFailureReason.CanceledBeforeExecution:
                        deviceServerCancellationData.FailureCode = FailureReason.CanceledBeforeExecution;
                        break;
                    //  (возвращение ошибки)
                    case CommandFailureReason.NotFoundConvertor:
                        deviceServerCancellationData.FailureCode = FailureReason.NotFoundConvertor;
                        break;
                    //  (возвращение ошибки)
                    case CommandFailureReason.NotFoundDevice:
                        deviceServerCancellationData.FailureCode = FailureReason.NotFoundDevice;
                        break;
                    //  (возвращение ошибки)
                    case CommandFailureReason.Exception:
                        deviceServerCancellationData.FailureCode = FailureReason.Exception;
                        break;
                    //  (возвращение ошибки)
                    case CommandFailureReason.ExecutionCompleted:
                        deviceServerCancellationData.FailureCode = FailureReason.ExecutionCompleted;
                        break;
                }
            }
            return deviceServerCancellationData;
        }
    }
}
