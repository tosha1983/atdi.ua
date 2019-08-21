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
using Atdi.DataModels.Sdrns.Device;
using System.Linq;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class LevelTaskWorker : ITaskWorker<LevelTask, LevelProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IController _controller;
        private readonly IBusGate _busGate;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private readonly IRepository<MeasResults, string> _measResultsByStringRepository;
        private readonly IRepository<DM.DeviceCommandResult, string> _repositoryDeviceCommandResult;
        private readonly IRepository<TaskParameters, string> _repositoryTaskParametersByString;


        public LevelTaskWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IBusGate busGate,
            IRepository<MeasResults, string> measResultsByStringRepository,
            IRepository<DM.DeviceCommandResult, string> repositoryDeviceCommandResult,
            IRepository<TaskParameters, string> repositoryTaskParametersByString,
            IController controller)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._busGate = busGate;
            this._measResultsByStringRepository = measResultsByStringRepository;
            this._controller = controller;
            this._repositoryDeviceCommandResult = repositoryDeviceCommandResult;
            this._repositoryTaskParametersByString = repositoryTaskParametersByString;
        }


        public void Run(ITaskContext<LevelTask, LevelProcess> context)
        {
            try
            {
                const int maximumDurationMeasLevel_ms = 1000;

                _logger.Verbouse(Contexts.LevelTaskWorker, Categories.Measurements, Events.StartLevelTaskWorker.With(context.Task.taskParameters.SDRTaskId));
                if (context.Process.Parent != null)
                {
                    if (context.Process.Parent is DispatchProcess)
                    {
                        (context.Process.Parent as DispatchProcess).contextLevelTasks.Add(context);
                    }
                }


                while (true)
                {
                    // проверка - не отменили ли задачу
                    if (context.Task.taskParameters.status == StatusTask.Z.ToString())
                    {
                        context.Cancel();
                        _logger.Info(Contexts.LevelTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId));
                        break;
                    }
                    else if (context.Task.taskParameters.status == StatusTask.F.ToString())
                    {
                        if (DateTime.Now > context.Task.taskParameters.StopTime)
                        {
                            context.Cancel();
                            _logger.Info(Contexts.LevelTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId));
                            break;
                        }
                        Thread.Sleep((int)context.Task.SleepTimePeriodForWaitingStartingMeas); // засыпание потока на время SleepTimePeriodForWaitingStartingMeas_ms
                        continue;
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////
                    // 
                    //  Определяеим актуальный период времени между единичными измерениями в общей задаче Level если оно отрицательное значит время измерения вышло пора закругляться
                    // 
                    ////////////////////////////////////////////////////////////////////////////////////////////////////
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

                        _logger.Info(Contexts.LevelTaskWorker, Categories.Measurements, Events.MaximumDurationMeas);
                        context.Cancel();
                        break;
                    }


                    //////////////////////////////////////////////
                    // 
                    // Отправка команды в контроллер 
                    //
                    //////////////////////////////////////////////
                    var deviceCommand = new MesureTraceCommand(context.Task.mesureTraceParameter);
                    //durationForMeasLevel_ms = 30 sec
                    if (context.Task.durationForMeasLevel_ms > 0)
                    {
                        deviceCommand.Timeout = context.Task.durationForMeasLevel_ms;
                    }
                    else
                    {
                        deviceCommand.Timeout = 30000;
                    }
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


                    LevelResult outResultData = null;
                    bool isDown = context.WaitEvent<LevelResult>(out outResultData, (int)(deviceCommand.Timeout));
                    if (isDown == false) // таймут - результатов нет
                    {

                        // проверка - не отменили ли задачу
                        if (context.Task.taskParameters.status == StatusTask.Z.ToString())
                        {
                            context.Cancel();
                            _logger.Info(Contexts.LevelTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId));
                            break;
                        }
                        else if (context.Task.taskParameters.status == StatusTask.F.ToString())
                        {
                            if (DateTime.Now > context.Task.taskParameters.StopTime)
                            {
                                context.Cancel();
                                _logger.Info(Contexts.LevelTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId));
                                break;
                            }
                            Thread.Sleep((int)context.Task.SleepTimePeriodForWaitingStartingMeas); // засыпание потока на время SleepTimePeriodForWaitingStartingMeas_ms
                            continue;
                        }
                        var error = new ExceptionProcessLevel();
                        if (context.WaitEvent<ExceptionProcessLevel>(out error, 1) == true)
                        {
                            if (error._ex != null)
                            {
                                /// реакция на ошибку выполнения команды
                                _logger.Error(Contexts.LevelTaskWorker, Categories.Measurements, Events.HandlingErrorSendCommandController.With(deviceCommand.Id), error._ex.StackTrace);
                                switch (error._failureReason)
                                {
                                    case CommandFailureReason.DeviceIsBusy:
                                    case CommandFailureReason.CanceledExecution:
                                    case CommandFailureReason.TimeoutExpired:
                                    case CommandFailureReason.CanceledBeforeExecution:
                                        _logger.Error(Contexts.LevelTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, (int)maximumDurationMeas), error._ex.StackTrace);
                                        Thread.Sleep(maximumDurationMeasLevel_ms); // вынести в константу (по умолчанию 1 сек)
                                        return;
                                    case CommandFailureReason.NotFoundConvertor:
                                    case CommandFailureReason.NotFoundDevice:
                                    case CommandFailureReason.Exception:
                                        _logger.Error(Contexts.LevelTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.taskParameters.SDRTaskId), error._ex.StackTrace);
                                         context.Cancel();
                                        return;
                                    default:
                                        throw new NotImplementedException($"Type {error._failureReason} not supported");
                                }
                            }
                        }
                    }
                    

                    DateTime currTime = DateTime.Now;
                    var action = new Action(() =>
                    {
                        //реакция на принятые результаты измерения
                        if (outResultData != null)
                        {
                            DM.MeasResults measResult = new DM.MeasResults();
                            context.Task.CountSendResults++;

                            //measResult.ResultId = string.Format("{0}|{1}", context.Task.taskParameters.SDRTaskId, context.Task.CountSendResults);
                            measResult.TaskId = context.Task.taskParameters.SDRTaskId;
                            measResult.ResultId = Guid.NewGuid().ToString();
                            measResult.ScansNumber = context.Task.CountMeasurementDone;

                            if (currTime > context.Task.taskParameters.StopTime)
                            {
                                measResult.Status = "C";
                            }
                            else
                            {
                                measResult.Status = "N";
                            }

                            measResult.Measurement = DataModels.Sdrns.MeasurementType.Level;
                            measResult.Levels_dBm = outResultData.Level;

                            var floatArray = outResultData.Freq_Hz.Select(x => (float)x).ToList();
                            measResult.Frequencies = floatArray.ToArray();


                            measResult.StartTime = currTime;
                            measResult.StopTime = currTime;
                            measResult.Location = new DataModels.Sdrns.GeoLocation();
                            measResult.Measured = currTime;

                            this._measResultsByStringRepository.Create(measResult);

                            context.Task.LevelResult = null;
                        }
                    });


                    //////////////////////////////////////////////
                    // 
                    //  Принять решение о полноте результатов
                    //  
                    //////////////////////////////////////////////
                    
                    action.Invoke();
                    

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

                        // обновление TaskParameters в БД
                        context.Task.taskParameters.status = StatusTask.C.ToString();
                        this._repositoryTaskParametersByString.Update(context.Task.taskParameters);

                        DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                        deviceCommandResult.CommandId = "UpdateStatusMeasTask";
                        deviceCommandResult.CustDate1 = DateTime.Now;
                        deviceCommandResult.Status = StatusTask.C.ToString();
                        deviceCommandResult.CustTxt1 = context.Task.taskParameters.SDRTaskId;

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
                    if (sleepTime >= 0)
                    {
                        _logger.Info(Contexts.LevelTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, (int)sleepTime));
                        Thread.Sleep((int)sleepTime);
                    }
                    //if (isDown) context.Task.CountMeasurementDone++;
                  
                }
                _logger.Info(Contexts.LevelTaskWorker, Categories.Measurements, Events.FinishedLevelTaskWorker);

            }
            catch (Exception e)
            {
                _logger.Error(Contexts.LevelTaskWorker, Categories.Measurements, Exceptions.UnknownErrorLevelTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
