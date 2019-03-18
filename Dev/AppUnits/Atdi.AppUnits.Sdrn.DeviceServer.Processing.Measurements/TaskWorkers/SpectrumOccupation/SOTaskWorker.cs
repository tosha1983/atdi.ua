using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using DM = Atdi.DataModels.Sdrns.Device;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.EntityOrm;



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
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParametersByInt;


        public SOTaskWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IBusGate busGate,
            IRepository<TaskParameters, int?> repositoryTaskParametersByInt,
            IController controller)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._busGate = busGate;
            this._controller = controller;
            this._repositoryTaskParametersByInt = repositoryTaskParametersByInt;
        }


        public void Run(ITaskContext<SOTask, SpectrumOccupationProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.SOTaskWorker, Categories.Measurements, Events.StartSOTaskWorker.With(context.Task.Id));
                (context.Process.Parent as DispatchProcess).contextSOTasks.Add(context);

                DateTime dateTimeNow = DateTime.Now;
                TimeSpan waitStartTask = context.Task.taskParameters.StartTime.Value - dateTimeNow;
                if (waitStartTask.TotalMilliseconds > 0)
                {
                    //Засыпаем до начала выполнения задачи
                    Thread.Sleep((int)waitStartTask.TotalMilliseconds);
                }

                while (true)
                {
                    // проверка - не отменили ли задачу
                    if (context.Task.taskParameters.status == StatusTask.Z.ToString())
                    {
                        context.Cancel();
                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                        break;
                    }
                    else if (context.Task.taskParameters.status == StatusTask.F.ToString())
                    {
                        Thread.Sleep((int)context.Task.SleepTimePeriodForWaitingStartingMeas); // засыпание потока на время SleepTimePeriodForWaitingStartingMeas_ms
                        continue;
                    }

                    //////////////////////////////////////////////
                    // 
                    //  Послать команду DeviceControler MeaseTrace
                    // 
                    //////////////////////////////////////////////
                    // Формирование команды (инициализация начальными параметрами) перед отправкой в контроллер

                    var maximumDurationMeas = CalculateTimeSleep(context.Task.taskParameters, context.Task.CountMeasurementDone);
                    if (maximumDurationMeas < 0)
                    {
                        // обновление TaskParameters в БД
                        context.Task.taskParameters.status = StatusTask.C.ToString();
                        this._repositoryTaskParametersByInt.Update(context.Task.taskParameters);
                        

                        DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                        deviceCommandResult.CommandId = "UpdateStatusMeasTask";
                        deviceCommandResult.CustDate1 = DateTime.Now;
                        deviceCommandResult.CustTxt1 = "";
                        deviceCommandResult.Status = StatusTask.C.ToString();
                        deviceCommandResult.CustNbr1 = int.Parse(context.Task.taskParameters.SDRTaskId);

                        var publisher = this._busGate.CreatePublisher("main");
                        publisher.Send<DM.DeviceCommandResult>("SendCommandResult", deviceCommandResult);
                        publisher.Dispose();

                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.MaximumDurationMeas);
                        context.Cancel();
                        
                        break;
                    }

                    var timeStamp = this._timeService.TimeStamp.Milliseconds;
                    var deviceCommand = new MesureTraceCommand(context.Task.mesureTraceParameter);
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
                    //////////////////////////////////////////////
                    SpectrumOcupationResult outSpectrumOcupation = null;
                    bool isDown = context.WaitEvent<SpectrumOcupationResult>(out outSpectrumOcupation, (int)context.Task.maximumTimeForWaitingResultSO);
                    if (isDown == false) // таймут - результатов нет
                    {
                        // проверка - не отменили ли задачу
                        if (context.Task.taskParameters.status == StatusTask.Z.ToString())
                        {
                            context.Cancel();
                            _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                            break;
                        }
                        else if (context.Task.taskParameters.status == StatusTask.F.ToString())
                        {
                            Thread.Sleep((int)context.Task.SleepTimePeriodForWaitingStartingMeas); // засыпание потока на время SleepTimePeriodForWaitingStartingMeas_ms
                            continue;
                        }

                        var error = new ExceptionProcessSO();
                        if (context.WaitEvent<ExceptionProcessSO>(out error, 1) == true)
                        {
                            if (error._ex != null)
                            {
                                /// реакция на ошибку выполнения команды
                                _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Events.HandlingErrorSendCommandController.With(deviceCommand.Id));
                                switch (error._failureReason)
                                {
                                    case CommandFailureReason.DeviceIsBusy:
                                    case CommandFailureReason.CanceledExecution:
                                    case CommandFailureReason.TimeoutExpired:
                                    case CommandFailureReason.CanceledBeforeExecution:
                                        _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, (int)maximumDurationMeas));
                                        Thread.Sleep((int)maximumDurationMeas);
                                        return;
                                    case CommandFailureReason.NotFoundConvertor:
                                    case CommandFailureReason.NotFoundDevice:
                                        var durationToRepietMeas = (int)maximumDurationMeas * (int)context.Task.KoeffWaitingDevice;
                                        TimeSpan durationToFinishTask = context.Task.taskParameters.StopTime.Value - DateTime.Now;
                                        if (durationToRepietMeas < durationToFinishTask.TotalMilliseconds)
                                        {
                                            _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                                            context.Cancel();
                                            return;
                                        }
                                        else
                                        {
                                            _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, durationToRepietMeas));
                                            Thread.Sleep(durationToRepietMeas);
                                        }
                                        break;
                                    case CommandFailureReason.Exception:
                                        _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
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
                            measResult.Location.ASL = (context.Process.Parent as DispatchProcess).Asl;
                            measResult.Location.Lon = (context.Process.Parent as DispatchProcess).Lon;
                            measResult.Location.Lat = (context.Process.Parent as DispatchProcess).Lat;

                            measResult.TaskId = context.Task.taskParameters.SDRTaskId;
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
                        if (timeSpan.TotalMilliseconds > context.Task.durationForSendResult)
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
                            if (timeSpan.TotalMilliseconds > (int)(context.Task.durationForSendResult / 2.0))
                            {
                                action.Invoke();
                            }
                        }

                        // обновление TaskParameters в БД
                        context.Task.taskParameters.status = StatusTask.C.ToString();
                        this._repositoryTaskParametersByInt.Update(context.Task.taskParameters);

                        DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                        deviceCommandResult.CommandId = "UpdateStatusMeasTask";
                        deviceCommandResult.CustDate1 = DateTime.Now;
                        deviceCommandResult.CustTxt1 = "";
                        deviceCommandResult.Status = StatusTask.C.ToString();
                        deviceCommandResult.CustNbr1 = int.Parse(context.Task.taskParameters.SDRTaskId);

                        var publisher = this._busGate.CreatePublisher("main");
                        publisher.Send<DM.DeviceCommandResult>("SendCommandResult", deviceCommandResult);
                        publisher.Dispose();

                        context.Finish();
                        break;
                    }
                    //////////////////////////////////////////////
                    // 
                    // Приостановка потока на рассчитаное время 
                    //
                    //////////////////////////////////////////////
                    var sleepTime = maximumDurationMeas - (DateTime.Now - currTime).TotalMilliseconds ;
                    if (sleepTime >= 0)
                    {
                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, (int)sleepTime));
                        Thread.Sleep((int)sleepTime);
                    }
                    if (isDown) context.Task.CountMeasurementDone++;
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
            double interval_ms = interval.TotalMilliseconds;
            if (taskParameters.NCount <= DoneCount) { return -1; }
            long duration = (long)(interval_ms / (taskParameters.NCount - DoneCount));
            return duration;
        }

    }
}
