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
            try
            {
                var measResults = new MeasResults();
                if ((NeedSearchEmitting(taskContext.Task.CountMeasurementDone)) == true)
                {
                    taskContext.Task.EmittingsRaw = CalcSearchEmitting.CalcSearch(taskContext.Task.ReferenceLevels, result, taskContext.Task.NoiseLevel_dBm);
                }
                else
                {
                    if (taskContext.Task.CountMeasurementDone == 0)
                    {
                        taskContext.Task.ReferenceLevels = CalcReferenceLevels.CalcRefLevels(taskContext.Task.taskParameters.RefSituation, result, taskContext.Task.mesureTraceDeviceProperties, ref taskContext.Task.NoiseLevel_dBm);
                    }
                    taskContext.Task.EmittingsRaw = CalcSearchInterruption.Calc(taskContext.Task.ReferenceLevels, result, taskContext.Task.NoiseLevel_dBm);
                }
                // Результат содержится в taskContext.Task.EmittingsRaw


                //получаем результаты BW
                var listMeasBandwidthResult = new List<BWResult>();
                while (true)
                {
                    var error = new ExceptionProcessBandWidth();
                    if (taskContext.WaitEvent<ExceptionProcessBandWidth>(out error,1) == true)
                    {
                        //if (error._ex!=null)
                        //{
                            //if (error._ex.Message!= "MeasBWResults is Null")
                            //{
                                //taskContext.Task.CountGetResultBWN++;
                            //}
                        //}
                        //taskContext.Task.CountGetResultBWNegative++;
                    }

                    BWResult outMeasBandwidthResultData = null;
                    bool isDown = taskContext.WaitEvent<BWResult>(out outMeasBandwidthResultData,1);
                    if (isDown == true)
                    {
                        if (outMeasBandwidthResultData != null)
                        {
                            //taskContext.Task.CountGetResultBWPositive++;
                            listMeasBandwidthResult.Add(outMeasBandwidthResultData);
                        }
                    }
                    else
                    {
                        break;
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
                bool isSuccessCalcGrouping = CalcGroupingEmitting.CalcGrouping(taskContext.Task.EmittingsRaw, ref taskContext.Task.EmittingsTemp, ref taskContext.Task.EmittingsSummary, _logger, taskContext.Task.NoiseLevel_dBm);
                if (isSuccessCalcGrouping == false)
                {
                    //обработка  ошибка
                    _logger.Warning(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Events.CalcGroupingNull);
                }

                //Нужно ли исследование существующих сигналов?
                if (CalcNeedResearchExistSignals.NeedResearchExistSignals(taskContext.Task.EmittingsSummary, out taskContext.Task.taskParametersForBW))
                {
                    //if ((listMeasBandwidthResult != null) && (listMeasBandwidthResult.Count > 0))
                    //{
                    //    taskContext.Task.EmittingsDetailed = CalcEmittingDetailed.GetEmittingDetailed(listMeasBandwidthResult);
                    //}
                    // вызов функции по отправке BandWidthTask в контроллер
                    SendCommandBW(taskContext);
                }
                // Отправка результата в Task Handler
                if (taskContext.Task.EmittingsSummary != null)
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
                taskContext.Finish();
            }
            catch (Exception ex)
            {
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
                    bandWidtTask.mesureTraceParameter = bandWidtTask.taskParameters.ConvertForBW();
                    _taskStarter.RunParallel(bandWidtTask, bandWidthProcess, taskContext);
                }
            }
        }
    }


}




