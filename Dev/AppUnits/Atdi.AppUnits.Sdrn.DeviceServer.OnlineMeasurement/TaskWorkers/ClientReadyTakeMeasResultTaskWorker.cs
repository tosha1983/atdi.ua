using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using DM = Atdi.DataModels.Sdrns.Device;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.EntityOrm;
using Atdi.DataModels.Sdrns.Device;



namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.TaskWorkers
{
    public class ClientReadyTakeMeasResultTaskWorker : ITaskWorker<ClientReadyTakeMeasResultTask, OnlineMeasurementProcess, PerThreadTaskWorkerLifetime>
    {
        private readonly AppServerComponentConfig _config;
        private readonly IController _controller;
        private readonly IBusGate _busGate;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;

        public ClientReadyTakeMeasResultTaskWorker(ITimeService timeService,
           IProcessingDispatcher processingDispatcher,
           ITaskStarter taskStarter,
           ILogger logger,
           IBusGate busGate,
           IController controller,
           AppServerComponentConfig config)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._busGate = busGate;
            this._controller = controller;
            this._config = config;
        }


        public void Run(ITaskContext<ClientReadyTakeMeasResultTask, OnlineMeasurementProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.ThisComponent, Categories.ClientReadyTakeMeasResultTaskWorker, "Started TaskWorker...");
                while (true) 
                {
                    
                    if (context.Token.IsCancellationRequested)
                    {
                        context.Cancel();
                        _logger.Info(Contexts.ThisComponent, Categories.ClientReadyTakeMeasResultTaskWorker, Events.OnlineTaskIsCancled);
                        return;
                    }

                    var measTraceParameter = ConvertToMesureTraceParameterForLevel.ConvertForLevel(context.Process.MeasTask);

                    //////////////////////////////////////////////
                    // 
                    // Отправка команды в контроллер 
                    //
                    //////////////////////////////////////////////
                    DateTime currTime = DateTime.Now;
                    var deviceCommand = new MesureTraceCommand(measTraceParameter);
                    deviceCommand.Timeout = this._config.maximumDurationMeasLevel_ms;
                    deviceCommand.Delay = 0;
                    deviceCommand.Options = CommandOption.StartImmediately;

                    this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                    (
                        ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                    ) =>
                    {
                        taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(failureReason, ex));
                    });


                    //////////////////////////////////////////////
                    // 
                    // Получение очередного  результат от Result Handler
                    //
                    //////////////////////////////////////////////
                    ///
                    DeviceServerResultLevel outResultData = null;
                    bool isDown = context.WaitEvent<DeviceServerResultLevel>(out outResultData, (int)(deviceCommand.Timeout));
                    if (isDown == false) // таймут - результатов нет
                    {
                     
                        if (context.Token.IsCancellationRequested)
                        {
                            context.Cancel();
                            return;
                        }

                        var error = new ExceptionProcessLevel();
                        if (context.WaitEvent<ExceptionProcessLevel>(out error, 1) == true)
                        {
                            if (error._ex != null)
                            {
                                /// реакция на ошибку выполнения команды
                                _logger.Error(Contexts.ThisComponent, Categories.ClientReadyTakeMeasResultTaskWorker, Events.HandlingErrorSendCommandController.With(deviceCommand.Id), error._ex.StackTrace);
                                switch (error._failureReason)
                                {
                                    case CommandFailureReason.DeviceIsBusy:
                                    case CommandFailureReason.CanceledExecution:
                                    case CommandFailureReason.TimeoutExpired:
                                    case CommandFailureReason.CanceledBeforeExecution:
                                        _logger.Error(Contexts.ThisComponent, Categories.ClientReadyTakeMeasResultTaskWorker, Events.SleepThread.With(deviceCommand.Id, (int)this._config.maximumDurationMeasLevel_ms), error._ex.StackTrace);
                                        Thread.Sleep(this._config.maximumDurationMeasLevel_ms); // вынести в константу (по умолчанию 1 сек)
                                        return;
                                    case CommandFailureReason.NotFoundConvertor:
                                    case CommandFailureReason.NotFoundDevice:
                                    case CommandFailureReason.Exception:
                                        _logger.Error(Contexts.ThisComponent, Categories.ClientReadyTakeMeasResultTaskWorker, Events.OnlineTaskIsCancled);
                                        _logger.Error(Contexts.ThisComponent, Categories.ClientReadyTakeMeasResultTaskWorker, error._ex.StackTrace);
                                        context.Cancel();
                                        return;
                                    default:
                                        throw new NotImplementedException($"Type {error._failureReason} not supported");
                                }
                            }
                        }
                    }
                    else
                    {
                        // так оборачиваем результат
                        var message = new OnlineMeasMessage
                        {
                            Kind = OnlineMeasMessageKind.DeviceServerMeasResult,
                            Container = outResultData
                        };

                        // и  отправляем его
                        context.Process.Publisher.Send(message);
                    }
                    //////////////////////////////////////////////
                    // 
                    // Приостановка потока на рассчитаное время 
                    //
                    //////////////////////////////////////////////
                    var sleepTime = this._config.minimumTimeDurationLevel_ms - (DateTime.Now - currTime).TotalMilliseconds;
                    if (sleepTime >= 0)
                    {
                        Thread.Sleep((int)sleepTime);
                        _logger.Info(Contexts.ThisComponent, Categories.ClientReadyTakeMeasResultTaskWorker, Events.SleepThread.With(deviceCommand.Id, (int)sleepTime));
                    }
                }
                //context.Finish();
            }
            catch (Exception e)
            {
                context.Abort(e);
            }
        }
    }
}
