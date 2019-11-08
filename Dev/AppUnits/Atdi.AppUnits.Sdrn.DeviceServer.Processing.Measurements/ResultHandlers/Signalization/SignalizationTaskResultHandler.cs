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
        private readonly ConfigMeasurements _configMeasurements;
        

        public SignalizationTaskResultHandler(ILogger logger,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            IWorkScheduler workScheduler,
            ConfigMeasurements configMeasurement,
            ITimeService timeService)
        {
            this._logger = logger;
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._timeService = timeService;
            this._workScheduler = workScheduler;
            this._configMeasurements = configMeasurement;
        }

        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess> taskContext)
        {
            var measResults = new MeasResults();
            try
            {
                if ((NeedSearchEmitting(taskContext.Task.CountMeasurementDone)) == true)
                {
                    taskContext.Task.EmittingsRaw = CalcSearchEmitting.CalcSearch(taskContext.Task.ReferenceLevels, result, taskContext.Task.NoiseLevel_dBm);
                }
                else
                {
                    if (taskContext.Task.CountMeasurementDone == 0)
                    {
                        taskContext.Task.ReferenceLevels = CalcReferenceLevels.CalcRefLevels(taskContext.Task.taskParameters, taskContext.Task.taskParameters.RefSituation, result, taskContext.Task.mesureTraceDeviceProperties, ref taskContext.Task.NoiseLevel_dBm, taskContext.Task.taskParameters.SignalingMeasTaskParameters.triggerLevel_dBm_Hz==null ? -999 : taskContext.Task.taskParameters.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.Value);
                    }
                    taskContext.Task.EmittingsRaw = CalcSearchInterruption.Calc(taskContext.Task.taskParameters, taskContext.Task.ReferenceLevels, result, taskContext.Task.NoiseLevel_dBm, taskContext.Task.taskParameters.ChCentrFreqs_Mhz, taskContext.Task.taskParameters.BWChalnel_kHz);
                }
                // Результат содержится в taskContext.Task.EmittingsRaw
                //получаем результаты BW
                
                var listMeasBandwidthResult = new List<BWResult>();
                if (taskContext.Task.taskParameters.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission == true)
                {
                    while (true)
                    {
                        BWResult outMeasBandwidthResultData = null;
                        bool isDown = taskContext.WaitEvent<BWResult>(out outMeasBandwidthResultData, 1);
                        if (isDown == true)
                        {
                            if (outMeasBandwidthResultData != null)
                            {
                                listMeasBandwidthResult.Add(outMeasBandwidthResultData);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                var listMeasSysInfoResult = new List<SysInfoResult>();
                if (taskContext.Task.taskParameters.SignalingMeasTaskParameters.AnalyzeSysInfoEmission == true)
                {
                    while (true)
                    {
                        SysInfoResult outMeasSysInfoResultData = null;
                        bool isDown = taskContext.WaitEvent<SysInfoResult>(out outMeasSysInfoResultData, 1);
                        if (isDown == true)
                        {
                            if (outMeasSysInfoResultData != null)
                            {
                                listMeasSysInfoResult.Add(outMeasSysInfoResultData);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                //Обработка результатов SysInfo если они есть
                if ((listMeasSysInfoResult != null) && (listMeasSysInfoResult.Count > 0))
                {
                    //здесь нужно дописать функцию GetEmittingDetailedForSysInfo
                    bool isSuccess = CalcEmittingSummuryByEmittingDetailed.GetEmittingDetailedForSysInfo(ref taskContext.Task.EmittingsSummary, listMeasSysInfoResult, taskContext.Task.ReferenceLevels, this._logger);
                    if (isSuccess == false)
                    {
                        //обработка  ошибка
                        _logger.Warning(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Events.GetEmittingDetailedNull);
                    }
                }

                //Обработка результатов BW если они есть
                if ((listMeasBandwidthResult != null) && (listMeasBandwidthResult.Count > 0))
                {
                    //taskContext.Task.EmittingsDetailed = CalcEmittingDetailed.GetEmittingDetailed(listMeasBandwidthResult);
                    bool isSuccess = CalcEmittingSummuryByEmittingDetailed.GetEmittingDetailed(ref taskContext.Task.EmittingsSummary, listMeasBandwidthResult, taskContext.Task.ReferenceLevels, this._logger);
                    if (isSuccess == false)
                    {
                        //обработка  ошибка
                        _logger.Warning(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Events.GetEmittingDetailedNull);
                    }
                }

                //Групируем сырые данные измерений к существующим
                bool isSuccessCalcGrouping = CalcGroupingEmitting.CalcGrouping(taskContext.Task.taskParameters, ref taskContext.Task.EmittingsRaw, ref taskContext.Task.EmittingsTemp, ref taskContext.Task.EmittingsSummary, _logger, taskContext.Task.NoiseLevel_dBm, this._configMeasurements.CountMaxEmission);
                if (isSuccessCalcGrouping == false)
                {
                    //обработка  ошибка
                    _logger.Warning(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Events.CalcGroupingNull);
                }

                //Нужно ли исследование существующих сигналов?
                if (taskContext.Task.taskParameters.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission == true)
                {
                    if (CalcNeedResearchExistSignals.NeedResearchExistSignals(taskContext.Task.EmittingsSummary, out taskContext.Task.taskParametersForBW))
                    {
                        // вызов функции по отправке BandWidthTask в контроллер
                        SendCommandBW(taskContext);
                    }
                }
                //Нужно ли исследование по SysInfo?
                if (taskContext.Task.taskParameters.SignalingMeasTaskParameters.AnalyzeSysInfoEmission == true)
                {
                    if (CalcNeedResearchSysInfo.NeedGetSysInfo(taskContext.Task.CounterCallSignaling))
                    {
                        // вызов функции по отправке BandWidthTask в контроллер
                        SendCommandSysInfo(taskContext);
                    }
                }
                // Отправка результата в Task Handler
                //if ((taskContext.Task.EmittingsSummary != null) && (taskContext.Task.EmittingsSummary.Length>0))
                {
                    var allEmitting = new List<Emitting>();
                    allEmitting.AddRange(taskContext.Task.EmittingsSummary);
                    allEmitting.AddRange(taskContext.Task.EmittingsTemp);
                    for (int p=0; p< allEmitting.Count; p++)
                    {
                        allEmitting[p].SensorId = taskContext.Task.taskParameters.SensorId;
                    }
                    var sortedByFreqAsc = from z in allEmitting orderby z.StartFrequency_MHz ascending select z;
                    measResults.Emittings = sortedByFreqAsc.ToArray();
                    measResults.RefLevels = taskContext.Task.ReferenceLevels;
                    taskContext.SetEvent(measResults);
                }

                taskContext.Task.CountMeasurementDone++;
                taskContext.Task.CounterCallSignaling++;
            }
            catch (Exception ex)
            {
                taskContext.SetEvent((MeasResults)(null));
                taskContext.SetEvent<ExceptionProcessSignalization>(new ExceptionProcessSignalization(CommandFailureReason.Exception, ex));
            }
        }



        private bool NeedSearchEmitting(int CountDone)
        {
            //заглушка
            return false;
        }


        /// <summary>
        /// Запуск задач типа BandWidthTask
        /// </summary>
        /// <param name="taskContext"></param>
        private void SendCommandBW(DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess> taskContext)
        {
            if (taskContext.Task.taskParametersForBW != null)
            {
                for (int i = 0; i < taskContext.Task.taskParametersForBW.Length; i++)
                {
                    var taskParametersForBW = taskContext.Task.taskParametersForBW[i];
                    var bandWidthProcess = _processingDispatcher.Start<BandWidthProcess>(taskContext.Process);
                    var bandWidtTask = new BandWidthTask();
                    bandWidtTask.durationForMeasBW_ms = taskContext.Task.durationForMeasBW_ms;
                    bandWidtTask.durationForSendResultBandWidth = taskContext.Task.durationForSendResultBandWidth; // файл конфигурации (с него надо брать)
                    bandWidtTask.maximumTimeForWaitingResultBandWidth = taskContext.Task.maximumTimeForWaitingResultSignalization;
                    bandWidtTask.SleepTimePeriodForWaitingStartingMeas = taskContext.Task.SleepTimePeriodForWaitingStartingMeas;
                    bandWidtTask.KoeffWaitingDevice = taskContext.Task.KoeffWaitingDevice;
                    bandWidtTask.LastTimeSend = DateTime.Now;
                    bandWidtTask.taskParameters = taskParametersForBW;
                    bandWidtTask.Smooth = taskParametersForBW.Smooth;
                    bandWidtTask.mesureTraceParameter = taskContext.Task.actionConvertBW.Invoke(bandWidtTask.taskParameters);
                    _taskStarter.Run(bandWidtTask, bandWidthProcess, taskContext);
                }
            }
        }

        /// <summary>
        /// Запуск задач типа SysInfo
        /// </summary>
        /// <param name="taskContext"></param>
        private void SendCommandSysInfo(DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess> taskContext)
        {
            if (taskContext.Task.taskParameters!=null)
            {
                var sysInfoProcess = _processingDispatcher.Start<SysInfoProcess>(taskContext.Process);
                var sysInfoTask = new SysInfoTask();
                sysInfoTask.durationForMeasSysInfo_ms = taskContext.Task.durationForMeasSysInfo_ms;
                sysInfoTask.durationForSendResultSysInfo = taskContext.Task.durationForSendResultSysInfo; // файл конфигурации (с него надо брать)
                sysInfoTask.maximumTimeForWaitingResultBandWidth = taskContext.Task.maximumTimeForWaitingResultSignalization;
                sysInfoTask.SleepTimePeriodForWaitingStartingMeas = taskContext.Task.SleepTimePeriodForWaitingStartingMeas;
                sysInfoTask.KoeffWaitingDevice = taskContext.Task.KoeffWaitingDevice;
                sysInfoTask.LastTimeSend = DateTime.Now;
                sysInfoTask.taskParameters = taskContext.Task.taskParameters;
                sysInfoTask.mesureSystemInfoParameters = taskContext.Task.actionConvertSysInfo.Invoke(sysInfoTask.taskParameters);
                _taskStarter.Run(sysInfoTask, sysInfoProcess, taskContext);
            }
        }
    }


}




