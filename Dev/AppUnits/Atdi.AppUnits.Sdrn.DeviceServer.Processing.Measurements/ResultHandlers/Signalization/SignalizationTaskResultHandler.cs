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
using Atdi.Common;
using Atdi.Platform.Logging;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.DataModels.EntityOrm;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class SignalizationTaskResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, SignalizationTask, SignalizationProcess>
    {

        private readonly ILogger _logger;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITaskStarter _taskStarter;
        private readonly ITimeService _timeService;
        private readonly IWorkScheduler _workScheduler;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParametersByInt;

        public SignalizationTaskResultHandler(ILogger logger,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            IWorkScheduler workScheduler,
            IRepository<TaskParameters, int?> repositoryTaskParametersByInt,
            ITimeService timeService)
        {
            this._logger = logger;
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._timeService = timeService;
            this._workScheduler = workScheduler;
            this._repositoryTaskParametersByInt = repositoryTaskParametersByInt;
        }

        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess> taskContext)
        {
            if (result != null)
            {
                try
                {
                    MeasResults measResults = new MeasResults();
                    var maximumDurationMeas = CommonConvertors.CalculateTimeSleep(taskContext.Task.taskParameters, taskContext.Task.CountMeasurementDone);
                    if (maximumDurationMeas < 0)
                    {
                        // обновление TaskParameters в БД
                        taskContext.Task.taskParameters.status = StatusTask.C.ToString();
                        this._repositoryTaskParametersByInt.Update(taskContext.Task.taskParameters);
                        taskContext.Cancel();
                    }
                    else
                    {

                        // 1) Нужно ли исследование существующих сигналов?
                        if (NeedResearchExistSignals(result))
                        {
                            var emittingsDetailed = CalcSearchEmitting.Convert(taskContext.Task.taskParameters.RefSituation);
                            taskContext.Task.EmittingsDetailed = emittingsDetailed;
                        }
                        else
                        {
                            // Первый расчет ?
                            if (result.PartIndex == 0)
                            {
                                taskContext.Task.ReferenceLevels = CalcReferenceLevels.CalcRefLevels(taskContext.Task.taskParameters.RefSituation, result, taskContext.Task.mesureTraceDeviceProperties);
                            }
                            else
                            {
                                taskContext.Task.EmittingsRaw = CalcSearchInterruption.Calc(taskContext.Task.ReferenceLevels, result, -1);
                            }
                        }

                   
                        CalcGroupingEmitting.Convert(taskContext.Task.EmittingsRaw, ref taskContext.Task.EmittingsDetailed, ref taskContext.Task.EmittingsSummary);

                        //////////////////////////////////////////
                        //
                        // Необходим доп иследование спектра ?
                        // 
                        //////////////////////////////////////////
                        if (IsAdditionalSpectrumStudyRrequired(taskContext.Task.EmittingsSummary))
                        {

                            //Проверка и запуск задачи BandWidth на одном из этапов 
                            /////////////////////////////////////////////////////////
                            //
                            //  Формирование нового таска BW
                            //
                            /////////////////////////////////////////////////////////

                            var bandWidthProcess = _processingDispatcher.Start<BandWidthProcess>(taskContext.Process);

                            var bandWidtTask = new BandWidthTask();
                            bandWidtTask.taskParameters = taskContext.Task.taskParameters;
                            bandWidtTask.mesureTraceParameter = taskContext.Task.mesureTraceParameter;

                            _taskStarter.RunParallel(bandWidtTask, bandWidthProcess, taskContext);

                            MeasBandwidthResult outMeasBandwidthResultData = null;
                            //получение результата с BandWidthTaskResultHandler
                            bool isDown = taskContext.WaitEvent<MeasBandwidthResult>(out outMeasBandwidthResultData, 1000);
                            if (isDown == false) // таймут - результатов нет
                            {
                                var error = new ExceptionProcessBandWidth();
                                if (taskContext.WaitEvent<ExceptionProcessBandWidth>(out error, 1) == true)
                                {
                                    if (error._ex != null)
                                    {
                                        /// реакция на ошибку выполнения команды
                                        _logger.Error(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Events.HandlingErrorSendCommandController.With(bandWidthProcess.Id));
                                        switch (error._failureReason)
                                        {
                                            case CommandFailureReason.DeviceIsBusy:
                                            case CommandFailureReason.CanceledExecution:
                                            case CommandFailureReason.TimeoutExpired:
                                            case CommandFailureReason.CanceledBeforeExecution:
                                                _logger.Error(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Events.SleepThread.With(bandWidthProcess.Id, (int)maximumDurationMeas));
                                                Thread.Sleep((int)maximumDurationMeas);
                                                taskContext.Cancel();
                                                return;
                                            case CommandFailureReason.NotFoundConvertor:
                                            case CommandFailureReason.NotFoundDevice:
                                                var durationToRepietMeas = (int)maximumDurationMeas * (int)taskContext.Task.KoeffWaitingDevice;
                                                TimeSpan durationToFinishTask = taskContext.Task.taskParameters.StopTime.Value - DateTime.Now;
                                                if (durationToRepietMeas < durationToFinishTask.TotalMilliseconds)
                                                {
                                                    _logger.Error(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Events.TaskIsCancled.With(taskContext.Task.Id));
                                                    taskContext.Cancel();
                                                    return;
                                                }
                                                else
                                                {
                                                    _logger.Error(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Events.SleepThread.With(bandWidthProcess.Id, durationToRepietMeas));
                                                    Thread.Sleep(durationToRepietMeas);
                                                }
                                                break;
                                            case CommandFailureReason.Exception:
                                                _logger.Error(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Events.TaskIsCancled.With(taskContext.Task.Id));
                                                taskContext.Cancel();
                                                return;
                                            default:
                                                throw new NotImplementedException($"Type {error._failureReason} not supported");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                measResults.BandwidthResult = new BandwidthMeasResult();
                                measResults.BandwidthResult.Bandwidth_kHz = outMeasBandwidthResultData.BandwidthkHz;
                                measResults.BandwidthResult.MarkerIndex = outMeasBandwidthResultData.MarkerIndex;
                                measResults.BandwidthResult.T1 = outMeasBandwidthResultData.T1;
                                measResults.BandwidthResult.T2 = outMeasBandwidthResultData.T2;
                                measResults.BandwidthResult.СorrectnessEstimations = outMeasBandwidthResultData.СorrectnessEstimations;
                            }

                            measResults.Emittings = taskContext.Task.EmittingsSummary.ToArray();
                            // Отправка результата в Task Handler
                            taskContext.SetEvent(measResults);
                        }
                        else
                        {
                            measResults.Emittings = taskContext.Task.EmittingsSummary.ToArray();
                            // Отправка результата в Task Handler
                            taskContext.SetEvent(measResults);
                        }
                    }
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent<ExceptionProcessSignalization>(new ExceptionProcessSignalization(CommandFailureReason.Exception, ex));
                }
            }
        }
        //////////////////////////////////////////////
        //
        // Нужно ли исследование существующих сигналов
        //
        //////////////////////////////////////////////
        private bool NeedResearchExistSignals(MesureTraceResult result)
        {
            //заглушка
            return true;
        }


        //////////////////////////////////////////////
        //
        // Необходим доп иследование спектра
        //
        //////////////////////////////////////////////
        private bool IsAdditionalSpectrumStudyRrequired(Emitting[] EmittingsSummary)
        {
            //заглушка
            return true;
        }


    }



}
