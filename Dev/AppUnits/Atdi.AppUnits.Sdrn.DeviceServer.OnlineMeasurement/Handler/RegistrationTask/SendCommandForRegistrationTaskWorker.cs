﻿using Atdi.Contracts.Sdrn.DeviceServer;
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
            const int CountLoopTimeoutExpired = 10;
            const int CountLoopDeviceIsBusy = 50;
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
                    DeviceServerCancellationData deviceServerCancellationDataValue = new DeviceServerCancellationData();
                    switch (error._failureReason)
                    {
                        // повторяем 50 раз иначе ошибка (повтор через 1/25 сек)
                        case CommandFailureReason.DeviceIsBusy:
                            while (context.Process.CountLoopForRegistrationTaskWorkerDeviceIsBusy <= CountLoopDeviceIsBusy)
                            {
                                this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                                (
                                ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                                ) =>
                                {
                                    taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(failureReason, ex));
                                });


                                isDown = context.WaitEvent<DeviceServerParametersDataLevel>(out deviceServerParametersDataLevel, (int)(deviceCommand.Timeout));
                                if (isDown == true) // таймут - результатов нет
                                {
                                    isSuccessOperation = true;
                                    break;
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(this._config.minimumTimeDurationLevel_ms);
                                    isSuccessOperation = false;
                                }
                                context.Process.CountLoopForRegistrationTaskWorkerDeviceIsBusy++;
                            }

                            if (isSuccessOperation == false)
                            {
                                deviceServerCancellationDataValue.FailureCode = FailureReason.DeviceIsBusy;
                            }

                            break;
                        case CommandFailureReason.TimeoutExpired:
                            while (context.Process.CountLoopForRegistrationTaskWorkerTimeoutExpired <= CountLoopTimeoutExpired)
                            {
                                this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                                (
                                ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                                ) =>
                                {
                                    taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(failureReason, ex));
                                });


                                isDown = context.WaitEvent<DeviceServerParametersDataLevel>(out deviceServerParametersDataLevel, (int)(deviceCommand.Timeout));
                                if (isDown == true) // таймут - результатов нет
                                {
                                    isSuccessOperation = true;
                                    break;
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(this._config.minimumTimeDurationLevel_ms);
                                    isSuccessOperation = false;
                                }
                                context.Process.CountLoopForRegistrationTaskWorkerTimeoutExpired++;
                            }

                            if (isSuccessOperation == false)
                            {
                                deviceServerCancellationDataValue.FailureCode = FailureReason.TimeoutExpired;
                            }

                           
                            break;

                        case CommandFailureReason.CanceledBeforeExecution:
                            deviceServerCancellationDataValue.FailureCode = FailureReason.CanceledBeforeExecution;
                            break;
                        case CommandFailureReason.CanceledExecution:
                            deviceServerCancellationDataValue.FailureCode = FailureReason.CanceledExecution;
                            break;
                        case CommandFailureReason.Exception:
                            deviceServerCancellationDataValue.FailureCode = FailureReason.Exception;
                            break;
                        case CommandFailureReason.ExecutionCompleted:
                            deviceServerCancellationDataValue.FailureCode = FailureReason.ExecutionCompleted;
                            break;
                        case CommandFailureReason.NotFoundConvertor:
                            deviceServerCancellationDataValue.FailureCode = FailureReason.NotFoundConvertor;
                            break;
                        case CommandFailureReason.NotFoundDevice:
                            deviceServerCancellationDataValue.FailureCode = FailureReason.NotFoundDevice;
                            break;
                    }

                    if (isSuccessOperation == false)
                    {
                        var message = new OnlineMeasMessage
                        {
                            Kind = OnlineMeasMessageKind.DeviceServerCancellation,
                            Container = deviceServerCancellationDataValue
                        };

                        // и  отправляем DeviceServerCancellationData
                        context.Process.Publisher.Send(message);
                        _logger.StartTrace(Contexts.ThisComponent, Categories.SendCommandForRegistrationTaskWorker, Events.ErrorReceivingResult);
                    }
                }
            }
            else
            {
                context.Process.CountLoopForRegistrationTaskWorkerDeviceIsBusy = 0;
                context.Process.CountLoopForRegistrationTaskWorkerTimeoutExpired = 0;
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
