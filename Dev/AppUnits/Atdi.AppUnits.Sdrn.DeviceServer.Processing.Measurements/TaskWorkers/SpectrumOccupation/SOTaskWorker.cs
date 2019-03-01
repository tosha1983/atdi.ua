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


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class SOTaskWorker : ITaskWorker<SOTask, SpectrumOccupationProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IController _controller;
        private readonly IBusGate _busGate;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;


        public SOTaskWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IBusGate busGate,
            IServicesResolver resolver,
            IServicesContainer servicesContainer,
            IController controller)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._busGate = busGate;
            this._controller = controller;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
        }

    
        public void Run(ITaskContext<SOTask, SpectrumOccupationProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.SOTaskWorker, Categories.Measurements, Events.StartSOTaskWorker.With(context.Task.Id));
                ////////////////////////////////////////////////////////////////////////
                // 
                //
                // получение с DI - контейнера экземпляра глобального процесса MainProcess
                //
                ////////////////////////////////////////////////////////////////////////
                this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;

                while (true)
                {
                    // проверка - не отменили ли задачу
                    if (context.Token.IsCancellationRequested)
                    {
                        context.Cancel();
                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                        return;
                    }
                    //////////////////////////////////////////////
                    // 
                    //  Послать команду DeviceControler MeaseTrace
                    // 
                    //
                    //////////////////////////////////////////////
                    // Формирование команды (инициализация начальными параметрами) перед отправкой в контроллер

                    var maximumDurationMeas = CalculateTimeSleep(context.Task.taskParameters, context.Task.CountMeasurementDone);
                    if (maximumDurationMeas==-1)
                    {
                        context.Cancel();
                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.MaximumDurationMeas);
                        return;
                    }

                    var timeStamp = this._timeService.TimeStamp.Milliseconds;
                    var deviceCommand = new MesureTraceCommand(context.Task.mesureTraceParameter)
                    {
                        Options = CommandOption.PutInQueue,
                        StartTimeStamp = timeStamp,
                        Timeout = timeStamp + maximumDurationMeas
                    };

                    //////////////////////////////////////////////
                    // 
                    // Отправка команды в контроллер (причем context уже содержит информацию о сообщение с шины RabbitMq)
                    //
                    //////////////////////////////////////////////
                    DateTime currTime = DateTime.Now;
                    _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.SendMeasureTraceCommandToController.With(deviceCommand.Id));
                    this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                    (
                        ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                    ) => 
                    {
                        taskContext.SetEvent<ExceptionProcessSO>(new ExceptionProcessSO(failureReason, ex));
                    });
                    //////////////////////////////////////////////
                    // 
                    // Получение очередного  результат от Result Handler
                    //
                    //
                    //////////////////////////////////////////////
                    SpectrumOcupationResult outSpectrumOcupation = null;
                    bool isDown = context.WaitEvent<SpectrumOcupationResult>(out outSpectrumOcupation, 1000 /*(int)maximumDurationMeas*/);
                    if (isDown == false) // таймут - результатов нет
                    {
                        // проверка - не отменили ли задачу
                        if (context.Token.IsCancellationRequested)
                        {
                            // явно нужна логика отмены
                            _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                            context.Cancel();
                            return;
                        }
                        var error = new ExceptionProcessSO();
                        if (context.WaitEvent<ExceptionProcessSO>(out error, 1) == true)
                        {
                            /// реакция на ошибку выполнения команды
                             _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.HandlingErrorSendCommandController.With(deviceCommand.Id));

                            switch (error._failureReason)
                            {
                                case CommandFailureReason.DeviceIsBusy:
                                case CommandFailureReason.CanceledExecution:
                                case CommandFailureReason.CanceledBeforeExecution:
                                    _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, (int)maximumDurationMeas));
                                    Thread.Sleep((int)maximumDurationMeas);
                               break;

                                case CommandFailureReason.NotFoundConvertor:
                                case CommandFailureReason.NotFoundDevice:
                                    var durationToRepietMeas = (int)maximumDurationMeas * 1000;
                                    TimeSpan durationToFinishTask = context.Task.taskParameters.StopTime.Value - DateTime.Now;
                                    if (durationToRepietMeas < durationToFinishTask.Milliseconds)
                                    {
                                        // здесь необходимо отправить уведомление об ошибке
                                        // отправка уведомления в шину (что ошибка)
                                        // запись в лог
                                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                                        context.Cancel();
                                        return;
                                    }
                                    else
                                    {
                                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, durationToRepietMeas));
                                        Thread.Sleep(durationToRepietMeas);
                                    }
                                break;
                                case CommandFailureReason.TimeoutExpired:

                                break;

                                case CommandFailureReason.Exception:
                                    var publisher = this._busGate.CreatePublisher("main");
                                    DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult()
                                    {
                                         CommandId = "SendCommandResult",
                                         CustTxt1 = $"Error get result 'SpectrumOcupationResult' for TaskId = {context.Task.taskParameters.SDRTaskId}",
                                         Status = "Failure"
                                    };
                                    publisher.Send<DM.DeviceCommandResult>("SendCommandResult", deviceCommandResult);
                                    publisher.Dispose();
                                    // отправка уведомления в шину (что ошибка)
                                    // запись в лог
                                    _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                                    context.Cancel();
                                    return;
                                break;
                                default:
                                    throw new NotImplementedException($"Type {error._failureReason} not supported");
                            }
                        }
                    }
                    else
                    {
                       // здесь пока не определена логика
                    }

                    var action = new Action(() =>
                    {
                        //реакция на принятые результаты измерения
                        if (outSpectrumOcupation.fSemplesResult != null)
                        {
                            DM.MeasResults measResult = new DM.MeasResults();
                            measResult.FrequencySamples = outSpectrumOcupation.fSemplesResult.Convert();
                            measResult.ScansNumber = outSpectrumOcupation.NN;
                            measResult.StartTime = context.Task.LastTimeSend.Value;
                            measResult.StopTime = currTime;
                            measResult.Location = new DataModels.Sdrns.GeoLocation();
                            //////////////////////////////////////////////
                            // 
                            //  Здесь получаем данные с GPS приемника
                            //  
                            //////////////////////////////////////////////
                            measResult.Location.ASL = baseContext.Asl;
                            measResult.Location.Lon = baseContext.Lon;
                            measResult.Location.Lat = baseContext.Lat;
                            //Отправка результатов в шину 
                            var publisher = this._busGate.CreatePublisher("main");
                            publisher.Send<DM.MeasResults>("SendMeasResults", measResult);
                            publisher.Dispose();
                            context.Task.lastResultParameters = null;
                            context.Task.LastTimeSend = currTime;
                        }
                    });


                    //////////////////////////////////////////////
                    // 
                    //  Принять решение о полноте результатов
                    //  
                    //////////////////////////////////////////////
                    if (outSpectrumOcupation != null)
                    {
                        TimeSpan timeSpan = currTime - context.Task.LastTimeSend.Value;
                        if (timeSpan.Milliseconds > context.Task.durationForSendResult)
                        {
                            //реакция на принятые результаты измерения
                            if (outSpectrumOcupation.fSemplesResult != null)
                            {
                                action.Invoke();
                            }
                        }
                    }

                    //////////////////////////////////////////////
                    // 
                    // Принятие решение о завершении таска
                    // 
                    //
                    //////////////////////////////////////////////
                    if (currTime > context.Task.taskParameters.StopTime)
                    {
                        // Здесь отправка последнего таска 
                        //(С проверкой - чтобы не отправляллся дубликат)
                        if (outSpectrumOcupation != null)
                        {
                            TimeSpan timeSpan = currTime - context.Task.LastTimeSend.Value;
                            if (timeSpan.Milliseconds > (int)(context.Task.durationForSendResult/2.0))
                            {
                                action.Invoke();
                            }
                        }
                        context.Finish();
                        return;
                    }
                    //////////////////////////////////////////////
                    // 
                    // Приостановка потока на рассчитаное время 
                    //
                    //////////////////////////////////////////////
                    var sleepTime = CalculateTimeSleep(context.Task.taskParameters, context.Task.CountMeasurementDone);
                    if (sleepTime > 0)
                    {
                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, (int)sleepTime));
                        Thread.Sleep((int)sleepTime);
                    }
                    else
                    {
                        context.Finish();
                        return;
                    }
                    context.Task.CountMeasurementDone++;
                }
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Exceptions.UnknownErrorSOTaskWorker, e.Message);
                context.Abort(e);
            }
        }

        /// <summary>
        ///Вычисление задержки выполнения потока результатом является количество vмилисекунд на которое необходимо приостановить поток
        /// </summary>
        /// <param name="taskParameters">Параметры таска</param> 
        /// <param name="doneCount">Количество измерений которое было проведено</param>
        /// <returns></returns>
        private long CalculateTimeSleep(TaskParameters taskParameters, int DoneCount)
        {
            DateTime dateTimeNow = DateTime.Now;
            if (dateTimeNow > taskParameters.StopTime.Value) { return -1; }
            TimeSpan interval = taskParameters.StopTime.Value - dateTimeNow;
            long interval_ms = interval.Milliseconds;
            if (taskParameters.NCount <= DoneCount) { return -1; }
            long duration = (interval_ms / (taskParameters.NCount - DoneCount));
            return duration;
        }

    }
}
