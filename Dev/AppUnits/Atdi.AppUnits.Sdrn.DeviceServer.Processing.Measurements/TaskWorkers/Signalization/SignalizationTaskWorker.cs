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
using Atdi.DataModels.EntityOrm;
using Atdi.DataModels.Sdrns.Device;
using System.Linq;
using System.Collections.Generic;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class SignalizationTaskWorker : ITaskWorker<SignalizationTask, SignalizationProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IController _controller;
        private readonly IBusGate _busGate;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParametersByInt;


        public SignalizationTaskWorker(ITimeService timeService,
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


        public void Run(ITaskContext<SignalizationTask, SignalizationProcess> context)
        {
            const int maximumDurationMeasSignaling_ms = 1000;
            try
            {

                _logger.Verbouse(Contexts.SignalizationTaskWorker, Categories.Measurements, Events.StartSignalizationTaskWorker.With(context.Task.Id));
                if (context.Process.Parent != null)
                {
                    if (context.Process.Parent is DispatchProcess)
                    {
                        (context.Process.Parent as DispatchProcess).contextSignalizationTasks.Add(context);
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
                        _logger.Info(Contexts.SignalizationTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                        break;
                    }
                    else if (context.Task.taskParameters.status == StatusTask.F.ToString())
                    {
                        Thread.Sleep((int)context.Task.SleepTimePeriodForWaitingStartingMeas); // засыпание потока на время SleepTimePeriodForWaitingStartingMeas_ms
                        continue;
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////
                    // 
                    //  Определяеим актуальный период времени между единичными измерениями в общей задаче Signalization если оно отрицательное значит время измерения вышло пора закругляться
                    // 
                    ////////////////////////////////////////////////////////////////////////////////////////////////////
                    var maximumDurationMeas = CommonConvertors.CalculateTimeSleep(context.Task.taskParameters, context.Task.CountMeasurementDone);
                    if (maximumDurationMeas < 0)
                    {
                        // обновление TaskParameters в БД
                        context.Task.taskParameters.status = StatusTask.C.ToString();
                        this._repositoryTaskParametersByInt.Update(context.Task.taskParameters);
                        DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                        deviceCommandResult.CommandId = "UpdateStatusMeasTask";
                        deviceCommandResult.CustDate1 = DateTime.Now;
                        deviceCommandResult.Status = StatusTask.C.ToString();
                        deviceCommandResult.CustTxt1 = context.Task.taskParameters.SDRTaskId;
                        var publisher = this._busGate.CreatePublisher("main");
                        publisher.Send<DM.DeviceCommandResult>("SendCommandResult", deviceCommandResult);
                        publisher.Dispose();
                        _logger.Info(Contexts.SOTaskWorker, Categories.Measurements, Events.MaximumDurationMeas);
                        context.Cancel();
                        break;
                    }


                    //////////////////////////////////////////////
                    // 
                    // Отправка команды в контроллер 
                    //
                    //////////////////////////////////////////////
                    //context.Task.CountCallSignaling++;
                    var deviceCommand = new MesureTraceCommand(context.Task.mesureTraceParameter);
                    deviceCommand.Delay = 0;
                    //deviceCommand.Options = CommandOption.StartImmediately;
                    DateTime currTime = DateTime.Now;
                    _logger.Info(Contexts.SignalizationTaskWorker, Categories.Measurements, Events.SendMeasureTraceCommandToController.With(deviceCommand.Id));
                    this._controller.SendCommand<MesureTraceResult>(context, deviceCommand,
                    (
                        ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception ex
                    ) =>
                    {
                        taskContext.SetEvent<ExceptionProcessSignalization>(new ExceptionProcessSignalization(failureReason, ex));
                    });
                    //////////////////////////////////////////////
                    // 
                    // Получение очередного  результат от Result Handler
                    //
                    //////////////////////////////////////////////
                    MeasResults outResultData = null;
                    bool isDown = context.WaitEvent<MeasResults>(out outResultData,  (int)context.Task.maximumTimeForWaitingResultSignalization);
                    if (isDown == false) // таймут - результатов нет
                    {
                        // проверка - не отменили ли задачу
                        if (context.Task.taskParameters.status == StatusTask.Z.ToString())
                        {
                            context.Cancel();
                            _logger.Info(Contexts.SignalizationTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id));
                            break;
                        }
                        else if (context.Task.taskParameters.status == StatusTask.F.ToString())
                        {
                            Thread.Sleep((int)context.Task.SleepTimePeriodForWaitingStartingMeas); // засыпание потока на время SleepTimePeriodForWaitingStartingMeas_ms
                            continue;
                        }

                        var error = new ExceptionProcessSignalization();
                        if (context.WaitEvent<ExceptionProcessSignalization>(out error, 1) == true)
                        {
                            if (error._ex != null)
                            {
                                /// реакция на ошибку выполнения команды
                                _logger.Error(Contexts.SignalizationTaskWorker, Categories.Measurements, Events.HandlingErrorSendCommandController.With(deviceCommand.Id), error._ex.StackTrace);
                                switch (error._failureReason)
                                {
                                    case CommandFailureReason.DeviceIsBusy:
                                    case CommandFailureReason.CanceledExecution:
                                    case CommandFailureReason.TimeoutExpired:
                                    case CommandFailureReason.CanceledBeforeExecution:
                                        _logger.Error(Contexts.SignalizationTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, (int)maximumDurationMeas), error._ex.StackTrace);
                                        Thread.Sleep(maximumDurationMeasSignaling_ms); // вынести в константу (по умолчанию 1 сек)
                                        return;
                                    case CommandFailureReason.NotFoundConvertor:
                                    case CommandFailureReason.NotFoundDevice:
                                        var durationToRepietMeas = (int)maximumDurationMeasSignaling_ms * (int)context.Task.KoeffWaitingDevice;
                                        TimeSpan durationToFinishTask = context.Task.taskParameters.StopTime.Value - DateTime.Now;
                                        if (durationToRepietMeas < durationToFinishTask.TotalMilliseconds)
                                        {
                                            _logger.Error(Contexts.SignalizationTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id), error._ex.StackTrace);
                                            context.Cancel();
                                            return;
                                        }
                                        else
                                        {
                                            _logger.Error(Contexts.SignalizationTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, durationToRepietMeas, error._ex.StackTrace));
                                            Thread.Sleep(durationToRepietMeas);
                                        }
                                        break;
                                    case CommandFailureReason.Exception:
                                        _logger.Error(Contexts.SignalizationTaskWorker, Categories.Measurements, Events.TaskIsCancled.With(context.Task.Id), error._ex.StackTrace);
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
                        // есть результат
                    }


                    var action = new Action(() =>
                    {
                        //реакция на принятые результаты измерения
                        if (outResultData != null)
                        {
                            //////////////////////////////////////////////
                            // 
                            //  Здесь получаем данные с GPS приемника
                            //  
                            //////////////////////////////////////////////
                            //outResultData.ResultId = Guid.NewGuid().ToString();
                            context.Task.CountSendResults++;
                            outResultData.ResultId = string.Format("{0}|{1}",context.Task.taskParameters.SDRTaskId, context.Task.CountSendResults);
                            outResultData.Status = "N";
                            outResultData.ScansNumber = context.Task.CountMeasurementDone;
                            outResultData.Measurement = DataModels.Sdrns.MeasurementType.Signaling;
                            outResultData.StartTime = context.Task.LastTimeSend.Value;
                            outResultData.StopTime = currTime;
                            outResultData.Measured = currTime;
                            outResultData.Location = new DataModels.Sdrns.GeoLocation();
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
                                            outResultData.Location.ASL = dispatchProcessParent.Asl;
                                            outResultData.Location.Lon = dispatchProcessParent.Lon;
                                            outResultData.Location.Lat = dispatchProcessParent.Lat;
                                        }
                                        else
                                        {
                                            _logger.Error(Contexts.SignalizationTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.AfterConvertParentProcessIsNull);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Error(Contexts.SignalizationTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, ex.Message);
                                    }
                                }
                                else
                                {
                                    _logger.Error(Contexts.SignalizationTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.ParentProcessIsNotTypeDispatchProcess);
                                }
                            }
                            else
                            {
                                _logger.Error(Contexts.SignalizationTaskWorker, Categories.Measurements, Exceptions.ErrorConvertToDispatchProcess, Exceptions.ParentProcessIsNull);
                            }

                            outResultData.TaskId = CommonConvertors.GetTaskId(outResultData.ResultId);
                            var arrEmit = new Emitting[outResultData.Emittings.Length];
                            for (int i=0; i< outResultData.Emittings.Length; i++)
                            {
                                var val = outResultData.Emittings[i];
                                var emit = CopyHelper.CreateDeepCopy(val);
                                var levelDistributionCorrCount = new List<int>();
                                var levelDistributionCorrLevel = new List<int>();
                                if (emit!=null)
                                {
                                    if (emit.LevelsDistribution!=null)
                                    {
                                        var newEmittingLevelsDistributionCount = emit.LevelsDistribution.Count;
                                        var newEmittingLevelsDistributionLevel = emit.LevelsDistribution.Levels;
                                        int startIndex = -1;
                                        int endIndex = -1;
                                        for (int j=0; j< newEmittingLevelsDistributionCount.Length; j++)
                                        {
                                            var valCount = newEmittingLevelsDistributionCount[j];
                                            var valLevel = newEmittingLevelsDistributionLevel[j];
                                            if (valCount==0)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                startIndex = j;
                                                break;
                                            }
                                        }

                                        for (int j = newEmittingLevelsDistributionCount.Length - 1; j >=0; j--)
                                        {
                                            var valCount = newEmittingLevelsDistributionCount[j];
                                            var valLevel = newEmittingLevelsDistributionLevel[j];
                                            if (valCount == 0)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                endIndex = j;
                                                break;
                                            }
                                        }

                                        if ((startIndex >= 0) && (endIndex >= 0))
                                        {
                                            for (int k = startIndex; k <= endIndex; k++)
                                            {
                                                var valCount = newEmittingLevelsDistributionCount[k];
                                                var valLevel = newEmittingLevelsDistributionLevel[k];

                                                levelDistributionCorrCount.Add(valCount);
                                                levelDistributionCorrLevel.Add(valLevel);
                                            }
                                            emit.LevelsDistribution.Levels = levelDistributionCorrLevel.ToArray();
                                            emit.LevelsDistribution.Count = levelDistributionCorrCount.ToArray();
                                        }
                                        else
                                        {
                                            emit.LevelsDistribution.Levels = newEmittingLevelsDistributionLevel.ToArray();
                                            emit.LevelsDistribution.Count = newEmittingLevelsDistributionCount.ToArray();
                                        }
                                    }
                                }
                                arrEmit[i] = emit;
                            }

                            CheckPercentAvailability.CheckPercent(ref arrEmit);

                            DM.MeasResults measResultsNew = new MeasResults()
                            {
                                BandwidthResult = outResultData.BandwidthResult,
                                Emittings = arrEmit,
                                Frequencies = outResultData.Frequencies,
                                FrequencySamples = outResultData.FrequencySamples,
                                Levels_dBm = outResultData.Levels_dBm,
                                Location = outResultData.Location,
                                Measured = outResultData.Measured,
                                Measurement = outResultData.Measurement,
                                RefLevels = outResultData.RefLevels,
                                ResultId = outResultData.ResultId,
                                Routes = outResultData.Routes,
                                ScansNumber = outResultData.ScansNumber,
                                StartTime = outResultData.StartTime,
                                StationResults = outResultData.StationResults,
                                Status = outResultData.Status,
                                StopTime = outResultData.StopTime,
                                SwNumber = outResultData.SwNumber,
                                TaskId = outResultData.TaskId
                            };

                            

                            //Отправка результатов в шину 
                            var publisher = this._busGate.CreatePublisher("main");
                            publisher.Send<DM.MeasResults>("SendMeasResults", measResultsNew);
                            publisher.Dispose();
                            context.Task.MeasResults = null;
                            context.Task.LastTimeSend = currTime;
                        }
                    });

                    

                    //////////////////////////////////////////////
                    // 
                    //  Принять решение о полноте результатов
                    //  
                    //////////////////////////////////////////////
                    TimeSpan timeSpan = currTime - context.Task.LastTimeSend.Value;
                    if (timeSpan.TotalMilliseconds > context.Task.durationForSendResultSignaling)
                    {
                        //реакция на принятые результаты измерения
                        action.Invoke();
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
                        if (outResultData != null)
                        {
                            timeSpan = currTime - context.Task.LastTimeSend.Value;
                            if (timeSpan.TotalMilliseconds > (int)(context.Task.durationForSendResultSignaling / 2.0))
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
                    var sleepTime = maximumDurationMeas - (DateTime.Now - currTime).TotalMilliseconds;
                    if (sleepTime >= 0)
                    {
                        _logger.Info(Contexts.SignalizationTaskWorker, Categories.Measurements, Events.SleepThread.With(deviceCommand.Id, (int)sleepTime));
                        Thread.Sleep((int)sleepTime);
                    }
                    //if (isDown) context.Task.CountMeasurementDone++;
                }
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.SignalizationTaskWorker, Categories.Measurements, Exceptions.UnknownErrorSignalizationTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
