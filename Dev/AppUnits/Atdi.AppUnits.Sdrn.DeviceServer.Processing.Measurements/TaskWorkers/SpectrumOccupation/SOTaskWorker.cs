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
        private readonly IRepository<TaskParameters, string> _repositoryTaskParametersByString;
        private readonly IRepository<DM.MeasResults, string> _measResultsByStringRepository;
        private readonly IRepository<DM.DeviceCommandResult, string> _repositoryDeviceCommandResult;


        public SOTaskWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IBusGate busGate,
            IRepository<TaskParameters, string> repositoryTaskParametersByString,
            IRepository<DM.MeasResults, string> measResultsByStringRepository,
            IRepository<DM.DeviceCommandResult, string> repositoryDeviceCommandResult,
            IController controller)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._busGate = busGate;
            this._controller = controller;
            this._measResultsByStringRepository = measResultsByStringRepository;
            this._repositoryTaskParametersByString = repositoryTaskParametersByString;
            this._repositoryDeviceCommandResult = repositoryDeviceCommandResult;
        }


        public void Run(ITaskContext<SOTask, SpectrumOccupationProcess> context)
        {
            try
            {
                _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.StartSOTaskWorker.With(context.Task.taskParameters.SDRTaskId));
                if (context.Process.Parent != null)
                {
                    if (context.Process.Parent is DispatchProcess)
                    {
                        (context.Process.Parent as DispatchProcess).contextSOTasks.Add(context);
                    }
                }


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
                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId));
                        break;
                    }
                    else if (context.Task.taskParameters.status == StatusTask.F.ToString())
                    {
                        if (DateTime.Now > context.Task.taskParameters.StopTime)
                        {
                            context.Cancel();
                            _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId));
                            break;
                        }
                        Thread.Sleep((int)context.Task.SleepTimePeriodForWaitingStartingMeas); // засыпание потока на время SleepTimePeriodForWaitingStartingMeas_ms
                        continue;
                    }

                    _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.StartSOTaskId.With(context.Task.taskParameters.SDRTaskId));

                    //////////////////////////////////////////////
                    // 
                    //  Послать команду DeviceControler MeaseTrace
                    // 
                    //////////////////////////////////////////////
                    // Формирование команды (инициализация начальными параметрами) перед отправкой в контроллер

                    var maximumDurationMeas = CommonConvertors.CalculateTimeSleep(context.Task.taskParameters, context.Task.CountMeasurementDone);
                    if (maximumDurationMeas < 0)
                    {
                        // обновление TaskParameters в БД
                        context.Task.taskParameters.status = StatusTask.C.ToString();
                        this._repositoryTaskParametersByString.Update(context.Task.taskParameters);


                        DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                        deviceCommandResult.CommandId = "UpdateStatusMeasTask";
                        deviceCommandResult.CustDate1 = DateTime.Now;
                        deviceCommandResult.Status = StatusTask.C.ToString();
                        deviceCommandResult.CustTxt1 = context.Task.taskParameters.SDRTaskId;


                        this._repositoryDeviceCommandResult.Create(deviceCommandResult);

                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.MaximumDurationMeas);
                        //context.Cancel();
                        //break;
                    }

                    //////////////////////////////////////////////
                    // 
                    // Получение очередного  результат от Result Handler
                    //
                    //////////////////////////////////////////////
                    //bool isDown = context.WaitEvent<SpectrumOcupationResult>(out outSpectrumOcupation, (int)context.Task.maximumTimeForWaitingResultSO);
                    var error = new ExceptionProcessSO();
                    SpectrumOcupationResult outSpectrumOcupation = null;
                    bool isError = context.WaitEvent<ExceptionProcessSO>(out error, 1);
                    bool isResultNotNull = false;
                    if (isError == true) // есть ошибка
                    {
                        // проверка - не отменили ли задачу
                        if (context.Task.taskParameters.status == StatusTask.Z.ToString())
                        {
                            context.Cancel();
                            _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId));
                            break;
                        }
                        else if (context.Task.taskParameters.status == StatusTask.F.ToString())
                        {
                            if (DateTime.Now > context.Task.taskParameters.StopTime)
                            {
                                context.Cancel();
                                _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId));
                                break;
                            }
                            Thread.Sleep((int)context.Task.SleepTimePeriodForWaitingStartingMeas); // засыпание потока на время SleepTimePeriodForWaitingStartingMeas_ms
                            continue;
                        }


                        if (error._ex != null)
                        {
                            /// реакция на ошибку выполнения команды
                            _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Events.HandlingErrorSendCommandController.With(context.Task.taskParameters.SDRTaskId), error._ex.StackTrace);
                            switch (error._failureReason)
                            {
                                case CommandFailureReason.DeviceIsBusy:
                                case CommandFailureReason.CanceledExecution:
                                case CommandFailureReason.TimeoutExpired:
                                case CommandFailureReason.CanceledBeforeExecution:
                                    _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Events.SleepThread.With(context.Task.taskParameters.SDRTaskId, (int)maximumDurationMeas), error._ex.StackTrace);
                                    Thread.Sleep((int)maximumDurationMeas);
                                    return;
                                case CommandFailureReason.NotFoundConvertor:
                                case CommandFailureReason.NotFoundDevice:
                                    var durationToRepietMeas = (int)maximumDurationMeas * (int)context.Task.KoeffWaitingDevice;
                                    TimeSpan durationToFinishTask = context.Task.taskParameters.StopTime.Value - DateTime.Now;
                                    if (durationToRepietMeas < durationToFinishTask.TotalMilliseconds)
                                    {
                                        _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId), error._ex.StackTrace);
                                        context.Cancel();
                                        return;
                                    }
                                    else
                                    {
                                        _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Events.SleepThread.With(context.Task.taskParameters.SDRTaskId, durationToRepietMeas), error._ex.StackTrace);
                                        Thread.Sleep(durationToRepietMeas);
                                    }
                                    break;
                                case CommandFailureReason.Exception:
                                    _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId), error._ex.StackTrace);
                                    context.Cancel();
                                    return;
                                default:
                                    throw new NotImplementedException($"Type {error._failureReason} not supported");
                            }
                        }
                    }
                    else
                    {

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

                        // есть ошибок нет
                        isResultNotNull = context.WaitEvent<SpectrumOcupationResult>(out outSpectrumOcupation);


                        var action = new Action(() =>
                        {
                            //реакция на принятые результаты измерения
                            if (outSpectrumOcupation.fSemplesResult != null)
                            {
                                DM.MeasResults measResult = new DM.MeasResults();
                                measResult.Status = "N";
                                context.Task.CountSendResults++;
                                //outResultData.ScansNumber = context.Task.CountMeasurementDone;
                                //measResult.ResultId = string.Format("{0}|{1}",context.Task.taskParameters.SDRTaskId, context.Task.CountSendResults);
                                measResult.ResultId = Guid.NewGuid().ToString();
                                measResult.TaskId = context.Task.taskParameters.SDRTaskId;
                                measResult.Measurement = DataModels.Sdrns.MeasurementType.SpectrumOccupation;
                                measResult.FrequencySamples = outSpectrumOcupation.fSemplesResult.Convert();
                                measResult.ScansNumber = outSpectrumOcupation.NN;
                                measResult.StartTime = context.Task.LastTimeSend.Value;
                                measResult.StopTime = currTime;
                                measResult.Location = new DataModels.Sdrns.GeoLocation();
                                measResult.Measured = currTime;
                                //////////////////////////////////////////////
                                // 
                                //  Здесь получаем данные с GPS приемника
                                //  
                                //////////////////////////////////////////////
                                var parentProcess = context.Process.Parent;
                                if (parentProcess != null)
                                {
                                    if (parentProcess is DispatchProcess)
                                    {
                                        DispatchProcess dispatchProcessParent = null;
                                        try
                                        {
                                            dispatchProcessParent = (parentProcess as DispatchProcess);
                                            if (dispatchProcessParent != null)
                                            {
                                                measResult.Location.ASL = dispatchProcessParent.Asl;
                                                measResult.Location.Lon = dispatchProcessParent.Lon;
                                                measResult.Location.Lat = dispatchProcessParent.Lat;
                                            }
                                            else
                                            {
                                                _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.AfterConvertParentProcessIsNull);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.ParentProcessIsNotTypeDispatchProcess);
                                    }
                                }
                                else
                                {
                                    _logger.Error(Contexts.SOTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.ParentProcessIsNull);
                                }

                                if (maximumDurationMeas < 0)
                                {
                                    context.Task.taskParameters.status = StatusTask.C.ToString();
                                    measResult.Status = StatusTask.C.ToString();
                                }

                                this._measResultsByStringRepository.Create(measResult);

                                context.Task.lastResultParameters = null;
                                context.Task.LastTimeSend = currTime;
                            }
                        });

                        if ((maximumDurationMeas < 0) || (currTime > context.Task.taskParameters.StopTime))
                        {
                            //реакция на принятые результаты измерения
                            action.Invoke();
                            context.Finish();
                            break;
                        }

                        //////////////////////////////////////////////
                        // 
                        //  Принять решение о полноте результатов
                        //  
                        //////////////////////////////////////////////
                        if (outSpectrumOcupation != null)
                        {
                            TimeSpan timeSpan = currTime - context.Task.LastTimeSend.Value;
                            if (timeSpan.TotalMilliseconds > context.Task.durationForSendResultSO)
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
                                if (timeSpan.TotalMilliseconds > (int)(context.Task.durationForSendResultSO / 2.0))
                                {
                                    action.Invoke();
                                }
                            }

                            // обновление TaskParameters в БД
                            context.Task.taskParameters.status = StatusTask.C.ToString();
                            this._repositoryTaskParametersByString.Update(context.Task.taskParameters);

                            DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                            deviceCommandResult.CommandId = "UpdateStatusMeasTask";
                            deviceCommandResult.CustDate1 = DateTime.Now;
                            deviceCommandResult.CustTxt1 = "";
                            deviceCommandResult.Status = StatusTask.C.ToString();
                            deviceCommandResult.CustNbr1 = int.Parse(context.Task.taskParameters.SDRTaskId);


                            this._repositoryDeviceCommandResult.Create(deviceCommandResult);

                            context.Finish();
                            break;
                        }
                        //////////////////////////////////////////////
                        // 
                        // Приостановка потока на рассчитаное время 
                        //
                        //////////////////////////////////////////////
                        var sleepTime = maximumDurationMeas - (DateTime.Now - currTime).TotalMilliseconds;
                        if (sleepTime > 0)
                        {
                            _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, (int)sleepTime));
                            Thread.Sleep((int)sleepTime);
                        }
                        if (isResultNotNull) context.Task.CountMeasurementDone++;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.SOTaskWorker, Categories.Measurements, Exceptions.UnknownErrorSOTaskWorker, e);
                context.Abort(e);
            }
        }
    }
}
