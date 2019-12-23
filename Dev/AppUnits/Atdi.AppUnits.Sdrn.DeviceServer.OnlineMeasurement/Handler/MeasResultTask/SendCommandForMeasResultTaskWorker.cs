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

        public bool Handle(ITaskContext<ClientReadyTakeMeasResultTask, OnlineMeasurementProcess> context, MesureTraceCommand deviceCommand, out DeviceServerResultLevel deviceServerResultLevel, out bool isCriticalError)
        {
            int countLoopForResultTaskWorkerDeviceIsBusy = 0;
            int countLoopForResultTaskWorkerTimeoutExpired = 0;
            isCriticalError = false;
            deviceServerResultLevel = null;
            bool isSuccessOperation = false;

            this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
            (
                ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
            ) =>
            {
                taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(failureReason, ex));
            });


            bool isDown = context.WaitEvent<DeviceServerResultLevel>(out deviceServerResultLevel /*, (int)(deviceCommand.Timeout)*/);
            if (isDown == false) // таймут - результатов нет
            {
                var error = new ExceptionProcessLevel();
                DeviceServerCancellationData deviceServerCancellationDataValue = new DeviceServerCancellationData();
                if (context.WaitEvent<ExceptionProcessLevel>(out error, 1) == true)
                {
                    switch (error._failureReason)
                    {
                        // повторяем 50 раз иначе ошибка (повтор через 1/25 сек)
                        case CommandFailureReason.DeviceIsBusy:
                            while (countLoopForResultTaskWorkerDeviceIsBusy <= this._config.CountLoopDeviceIsBusy)
                            {
                                this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                                (
                                ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                                ) =>
                                {
                                    taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(failureReason, ex));
                                });


                                isDown = context.WaitEvent<DeviceServerResultLevel>(out deviceServerResultLevel/*, (int)(deviceCommand.Timeout)*/);
                                if (isDown == true)
                                {
                                    isSuccessOperation = true;
                                    countLoopForResultTaskWorkerDeviceIsBusy = 0;
                                    break;
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(this._config.minimumTimeDurationLevel_ms);
                                    isSuccessOperation = false;
                                }
                                countLoopForResultTaskWorkerDeviceIsBusy++;
                            }

                            if (isSuccessOperation == false)
                            {
                                deviceServerCancellationDataValue.FailureCode = FailureReason.DeviceIsBusy;
                            }

                            break;
                        case CommandFailureReason.TimeoutExpired:
                            while (countLoopForResultTaskWorkerTimeoutExpired <= this._config.CountLoopTimeoutExpired)
                            {
                                this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                                (
                                ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                                ) =>
                                {
                                    taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(failureReason, ex));
                                });


                                isDown = context.WaitEvent<DeviceServerResultLevel>(out deviceServerResultLevel/*, (int)(deviceCommand.Timeout)*/);
                                if (isDown == true)
                                {
                                    isSuccessOperation = true;
                                    countLoopForResultTaskWorkerTimeoutExpired = 0;
                                    break;
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(this._config.minimumTimeDurationLevel_ms);
                                    isSuccessOperation = false;
                                }
                                countLoopForResultTaskWorkerTimeoutExpired++;
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
                        default:
                            throw new Exception($"Not supported type {error._failureReason}");
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
                        isCriticalError = true;
                        _logger.StartTrace(Contexts.ThisComponent, Categories.SendCommandForRegistrationTaskWorker, Events.ErrorReceivingResult);
                    }
                }
                else
                {
                    while (countLoopForResultTaskWorkerTimeoutExpired <= this._config.CountLoopTimeoutExpired)
                    {
                        this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                        (
                        ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                        ) =>
                        {
                            taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(failureReason, ex));
                        });


                        isDown = context.WaitEvent<DeviceServerResultLevel>(out deviceServerResultLevel/*, (int)(deviceCommand.Timeout)*/);
                        if (isDown == true)
                        {
                            isSuccessOperation = true;
                            countLoopForResultTaskWorkerTimeoutExpired = 0;
                            break;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(this._config.minimumTimeDurationLevel_ms);
                            isSuccessOperation = false;
                        }
                        countLoopForResultTaskWorkerTimeoutExpired++;
                    }

                    if (isSuccessOperation == false)
                    {
                        deviceServerCancellationDataValue.FailureCode = FailureReason.TimeoutExpired;
                    }
                }
            }
            else
            {
                countLoopForResultTaskWorkerDeviceIsBusy = 0;
                countLoopForResultTaskWorkerTimeoutExpired = 0;
                isSuccessOperation = true;
            }

            if ((deviceServerResultLevel != null) && (isSuccessOperation == true))
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
