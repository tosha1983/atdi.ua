using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Xml;
using System.Linq;


namespace Atdi.WcfServices.Sdrn.Server
{
    public class LoadMeasTask 
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public LoadMeasTask(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }


        public ShortMeasTask[] GetShortMeasTasks()
        {
            var listMeasTask = new List<ShortMeasTask>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasTasksMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().From();
                builderMeasTask.Select(c => c.CreatedBy);
                builderMeasTask.Select(c => c.DateCreated);
                builderMeasTask.Select(c => c.ExecutionMode);
                builderMeasTask.Select(c => c.Id);
                builderMeasTask.Select(c => c.IdentStart);
                builderMeasTask.Select(c => c.MaxTimeBs);
                builderMeasTask.Select(c => c.Name);
                builderMeasTask.Select(c => c.OrderId);
                builderMeasTask.Select(c => c.PerInterval);
                builderMeasTask.Select(c => c.PerStart);
                builderMeasTask.Select(c => c.PerStop);
                builderMeasTask.Select(c => c.Prio);
                builderMeasTask.Select(c => c.ResultType);
                builderMeasTask.Select(c => c.Status);
                builderMeasTask.Select(c => c.Task);
                builderMeasTask.Select(c => c.TimeStart);
                builderMeasTask.Select(c => c.TimeStop);
                builderMeasTask.Select(c => c.Type);
                builderMeasTask.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderMeasTask, readerMeasTask =>
                {

                    while (readerMeasTask.Read())
                    {
                        var measTask = new ShortMeasTask();
                        measTask.CreatedBy = readerMeasTask.GetValue(c => c.CreatedBy);
                        measTask.DateCreated = readerMeasTask.GetValue(c => c.DateCreated);
                        MeasTaskExecutionMode ExecutionMode;
                        if (Enum.TryParse<MeasTaskExecutionMode>(readerMeasTask.GetValue(c => c.ExecutionMode), out ExecutionMode))
                        {
                            measTask.ExecutionMode = ExecutionMode;
                        }
                        measTask.Id = new MeasTaskIdentifier();
                        measTask.Id.Value = readerMeasTask.GetValue(c => c.Id);
                        measTask.MaxTimeBs = readerMeasTask.GetValue(c => c.MaxTimeBs);
                        measTask.Name = readerMeasTask.GetValue(c => c.Name);
                        measTask.OrderId = readerMeasTask.GetValue(c => c.OrderId).HasValue ? readerMeasTask.GetValue(c => c.OrderId).Value : -1;
                        measTask.Prio = readerMeasTask.GetValue(c => c.Prio);
                        MeasTaskResultType ResultType;
                        if (Enum.TryParse<MeasTaskResultType>(readerMeasTask.GetValue(c => c.ResultType), out ResultType))
                        {
                            measTask.ResultType = ResultType;
                        }
                        measTask.Status = readerMeasTask.GetValue(c => c.Status);
                        MeasTaskType Task;
                        if (Enum.TryParse<MeasTaskType>(readerMeasTask.GetValue(c => c.Task), out Task))
                        {
                            measTask.Task = Task;
                        }
                        measTask.Type = readerMeasTask.GetValue(c => c.Type);

                        
                        if (readerMeasTask.GetValue(c => c.Type) != null)
                        {
                            MeasurementType typeMeasurements;
                            if (Enum.TryParse<MeasurementType>(readerMeasTask.GetValue(c => c.Type), out typeMeasurements))
                            {
                                measTask.TypeMeasurements = typeMeasurements;
                            }
                            else
                            {
                                throw new InvalidOperationException("NOT set 'MeasurementType' for task Task!");
                            }
                        }

                        listMeasTask.Add(measTask);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listMeasTask.ToArray();
        }

        public ShortMeasTask GetShortMeasTask(long taskId)
        {
            var measTask = new ShortMeasTask();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasTaskMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().From();
                builderMeasTask.Select(c => c.CreatedBy);
                builderMeasTask.Select(c => c.DateCreated);
                builderMeasTask.Select(c => c.ExecutionMode);
                builderMeasTask.Select(c => c.Id);
                builderMeasTask.Select(c => c.IdentStart);
                builderMeasTask.Select(c => c.MaxTimeBs);
                builderMeasTask.Select(c => c.Name);
                builderMeasTask.Select(c => c.OrderId);
                builderMeasTask.Select(c => c.PerInterval);
                builderMeasTask.Select(c => c.PerStart);
                builderMeasTask.Select(c => c.PerStop);
                builderMeasTask.Select(c => c.Prio);
                builderMeasTask.Select(c => c.ResultType);
                builderMeasTask.Select(c => c.Status);
                builderMeasTask.Select(c => c.Task);
                builderMeasTask.Select(c => c.TimeStart);
                builderMeasTask.Select(c => c.TimeStop);
                builderMeasTask.Select(c => c.Type);
                builderMeasTask.Where(c => c.Id, ConditionOperator.Equal, taskId);
                builderMeasTask.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                builderMeasTask.Where(c => c.Status, ConditionOperator.IsNotNull);
                queryExecuter.Fetch(builderMeasTask, readerMeasTask =>
                {
                    while (readerMeasTask.Read())
                    {
                        measTask.CreatedBy = readerMeasTask.GetValue(c => c.CreatedBy);
                        measTask.DateCreated = readerMeasTask.GetValue(c => c.DateCreated);
                        MeasTaskExecutionMode ExecutionMode;
                        if (Enum.TryParse<MeasTaskExecutionMode>(readerMeasTask.GetValue(c => c.ExecutionMode), out ExecutionMode))
                        {
                            measTask.ExecutionMode = ExecutionMode;
                        }
                        measTask.Id = new MeasTaskIdentifier();
                        measTask.Id.Value = readerMeasTask.GetValue(c => c.Id);
                        measTask.MaxTimeBs = readerMeasTask.GetValue(c => c.MaxTimeBs);
                        measTask.Name = readerMeasTask.GetValue(c => c.Name);
                        measTask.OrderId = readerMeasTask.GetValue(c => c.OrderId).HasValue ? readerMeasTask.GetValue(c => c.OrderId).Value : -1;
                        measTask.Prio = readerMeasTask.GetValue(c => c.Prio);
                        MeasTaskResultType ResultType;
                        if (Enum.TryParse<MeasTaskResultType>(readerMeasTask.GetValue(c => c.ResultType), out ResultType))
                        {
                            measTask.ResultType = ResultType;
                        }
                        measTask.Status = readerMeasTask.GetValue(c => c.Status);
                        MeasTaskType Task;
                        if (Enum.TryParse<MeasTaskType>(readerMeasTask.GetValue(c => c.Task), out Task))
                        {
                            measTask.Task = Task;
                        }
                        measTask.Type = readerMeasTask.GetValue(c => c.Type);

                        if (readerMeasTask.GetValue(c => c.Type) != null)
                        {
                            MeasurementType typeMeasurements;
                            if (Enum.TryParse<MeasurementType>(readerMeasTask.GetValue(c => c.Type), out typeMeasurements))
                            {
                                measTask.TypeMeasurements = typeMeasurements;
                            }
                            else
                            {
                                throw new InvalidOperationException("NOT set 'MeasurementType' for task Task!");
                            }
                        }
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return measTask;
        }


        public MeasTask GetMeasTaskById(long id)
        {
            MeasTask result = null;
            if (id>0)
            {
                var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
                var Res = loadMeasTask.ShortReadTask(id);
                if (Res != null)
                {
                    if (Res.Count > 0)
                    {
                        result = Res[0];
                    }
                    else
                    {
                        result = null;
                    }
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                return null;
            }
            return result;
        }

        public MeasTask GetMeasTaskHeader(MeasTaskIdentifier taskId)
        {
            MeasTask result = null;
            if (taskId != null)
            {
                var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
                var Res = loadMeasTask.ShortReadTaskWithoutRefSituation(taskId.Value);
                if (Res != null)
                {
                    if (Res.Count > 0)
                    {
                        result = Res[0];
                    }
                    else
                    {
                        result = null;
                    }
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                return null;
            }
            return result;
        }


        public List<MeasTask> ShortReadTask(long id)
        {
            var listMeasTask = new List<MeasTask>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallShortReadTaskMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().From();
                builderMeasTask.Select(c => c.CreatedBy);
                builderMeasTask.Select(c => c.DateCreated);
                builderMeasTask.Select(c => c.ExecutionMode);
                builderMeasTask.Select(c => c.Id);
                builderMeasTask.Select(c => c.IdentStart);
                builderMeasTask.Select(c => c.MaxTimeBs);
                builderMeasTask.Select(c => c.Name);
                builderMeasTask.Select(c => c.OrderId);
                builderMeasTask.Select(c => c.PerInterval);
                builderMeasTask.Select(c => c.PerStart);
                builderMeasTask.Select(c => c.PerStop);
                builderMeasTask.Select(c => c.Prio);
                builderMeasTask.Select(c => c.ResultType);
                builderMeasTask.Select(c => c.Status);
                builderMeasTask.Select(c => c.Task);
                builderMeasTask.Select(c => c.TimeStart);
                builderMeasTask.Select(c => c.TimeStop);
                builderMeasTask.Select(c => c.Type);
                builderMeasTask.Where(c => c.Id, ConditionOperator.Equal, id);
                builderMeasTask.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                builderMeasTask.Where(c => c.Status, ConditionOperator.IsNotNull);
                queryExecuter.Fetch(builderMeasTask, readerMeasTask =>
                {
                    var measTask = new MeasTask();
                    while (readerMeasTask.Read())
                    {
                        measTask.CreatedBy = readerMeasTask.GetValue(c => c.CreatedBy);
                        measTask.DateCreated = readerMeasTask.GetValue(c => c.DateCreated);
                        MeasTaskExecutionMode ExecutionMode;
                        if (Enum.TryParse<MeasTaskExecutionMode>(readerMeasTask.GetValue(c => c.ExecutionMode), out ExecutionMode))
                        {
                            measTask.ExecutionMode = ExecutionMode;
                        }
                        measTask.Id = new MeasTaskIdentifier();
                        measTask.Id.Value = readerMeasTask.GetValue(c => c.Id);
                        measTask.MaxTimeBs = readerMeasTask.GetValue(c => c.MaxTimeBs);
                        measTask.Name = readerMeasTask.GetValue(c => c.Name);
                        measTask.OrderId = readerMeasTask.GetValue(c => c.OrderId);
                        measTask.Prio = readerMeasTask.GetValue(c => c.Prio);
                        MeasTaskResultType ResultType;
                        if (Enum.TryParse<MeasTaskResultType>(readerMeasTask.GetValue(c => c.ResultType), out ResultType))
                        {
                            measTask.ResultType = ResultType;
                        }
                        measTask.Status = readerMeasTask.GetValue(c => c.Status);
                        MeasTaskType Task;
                        if (Enum.TryParse<MeasTaskType>(readerMeasTask.GetValue(c => c.Task), out Task))
                        {
                            measTask.Task = Task;
                        }
                        measTask.Type = readerMeasTask.GetValue(c => c.Type);


                        measTask.SignalingMeasTaskParameters = new SignalingMeasTask();
                        var builderMeasTaskSignaling = this._dataLayer.GetBuilder<MD.IMeasTaskSignaling>().From();
                        builderMeasTaskSignaling.Select(c => c.Id);
                        builderMeasTaskSignaling.Select(c => c.MEAS_TASK.Id);
                        builderMeasTaskSignaling.Select(c => c.allowableExcess_dB);
                        builderMeasTaskSignaling.Select(c => c.AutoDivisionEmitting);
                        builderMeasTaskSignaling.Select(c => c.CompareTraceJustWithRefLevels);
                        builderMeasTaskSignaling.Select(c => c.DifferenceMaxMax);
                        builderMeasTaskSignaling.Select(c => c.FiltrationTrace);
                        builderMeasTaskSignaling.Select(c => c.SignalizationNChenal);
                        builderMeasTaskSignaling.Select(c => c.SignalizationNCount);
                        builderMeasTaskSignaling.Select(c => c.CorrelationAnalize);
                        builderMeasTaskSignaling.Select(c => c.CheckFreqChannel);
                        builderMeasTaskSignaling.Select(c => c.AnalyzeByChannel);
                        builderMeasTaskSignaling.Select(c => c.AnalyzeSysInfoEmission);
                        builderMeasTaskSignaling.Select(c => c.DetailedMeasurementsBWEmission);
                        builderMeasTaskSignaling.Select(c => c.CorrelationFactor);
                        builderMeasTaskSignaling.Select(c => c.Standard);
                        builderMeasTaskSignaling.Select(c => c.TriggerLevel_dBm_Hz);
                        builderMeasTaskSignaling.Select(c => c.NumberPointForChangeExcess);
                        builderMeasTaskSignaling.Select(c => c.WindowBW);
                        builderMeasTaskSignaling.Select(c => c.DiffLevelForCalcBW);
                        builderMeasTaskSignaling.Select(c => c.NDbLevel_dB);
                        builderMeasTaskSignaling.Select(c => c.NumberIgnoredPoints);
                        builderMeasTaskSignaling.Select(c => c.MinExcessNoseLevel_dB);
                        builderMeasTaskSignaling.Select(c => c.TimeBetweenWorkTimes_sec);
                        builderMeasTaskSignaling.Select(c => c.TypeJoinSpectrum);
                        builderMeasTaskSignaling.Select(c => c.CrossingBWPercentageForGoodSignals);
                        builderMeasTaskSignaling.Select(c => c.CrossingBWPercentageForBadSignals);
                        builderMeasTaskSignaling.Select(c => c.MaxFreqDeviation);
                        builderMeasTaskSignaling.Select(c => c.CheckLevelChannel);
                        builderMeasTaskSignaling.Select(c => c.MinPointForDetailBW);
                        builderMeasTaskSignaling.Select(c => c.CollectEmissionInstrumentalEstimation);
                        


                        builderMeasTaskSignaling.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasTaskSignaling, readerMeasTaskSignaling =>
                        {
                            var resultMeasTaskSignaling = true;
                            while (readerMeasTaskSignaling.Read())
                            {
                                measTask.SignalingMeasTaskParameters.InterruptionParameters = new SignalingInterruptionParameters();
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting = readerMeasTaskSignaling.GetValue(c => c.AutoDivisionEmitting);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax = readerMeasTaskSignaling.GetValue(c => c.DifferenceMaxMax);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW = readerMeasTaskSignaling.GetValue(c => c.DiffLevelForCalcBW);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB = readerMeasTaskSignaling.GetValue(c => c.MinExcessNoseLevel_dB);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB = readerMeasTaskSignaling.GetValue(c => c.NDbLevel_dB);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints = readerMeasTaskSignaling.GetValue(c => c.NumberIgnoredPoints);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess = readerMeasTaskSignaling.GetValue(c => c.NumberPointForChangeExcess);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.windowBW = readerMeasTaskSignaling.GetValue(c => c.WindowBW);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation = readerMeasTaskSignaling.GetValue(c => c.MaxFreqDeviation);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel = readerMeasTaskSignaling.GetValue(c => c.CheckLevelChannel);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW = readerMeasTaskSignaling.GetValue(c => c.MinPointForDetailBW);
                                measTask.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels = readerMeasTaskSignaling.GetValue(c => c.CompareTraceJustWithRefLevels);
                                measTask.SignalingMeasTaskParameters.FiltrationTrace = readerMeasTaskSignaling.GetValue(c => c.FiltrationTrace);
                                measTask.SignalingMeasTaskParameters.SignalizationNChenal = readerMeasTaskSignaling.GetValue(c => c.SignalizationNChenal);
                                measTask.SignalingMeasTaskParameters.SignalizationNCount = readerMeasTaskSignaling.GetValue(c => c.SignalizationNCount);
                                measTask.SignalingMeasTaskParameters.AnalyzeByChannel = readerMeasTaskSignaling.GetValue(c => c.AnalyzeByChannel);
                                measTask.SignalingMeasTaskParameters.CollectEmissionInstrumentalEstimation = readerMeasTaskSignaling.GetValue(c => c.CollectEmissionInstrumentalEstimation);
                                measTask.SignalingMeasTaskParameters.AnalyzeSysInfoEmission = readerMeasTaskSignaling.GetValue(c => c.AnalyzeSysInfoEmission);
                                measTask.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission = readerMeasTaskSignaling.GetValue(c => c.DetailedMeasurementsBWEmission);
                                measTask.SignalingMeasTaskParameters.Standard = readerMeasTaskSignaling.GetValue(c => c.Standard);
                                measTask.SignalingMeasTaskParameters.CorrelationAnalize = readerMeasTaskSignaling.GetValue(c => c.CorrelationAnalize);
                                measTask.SignalingMeasTaskParameters.CorrelationFactor = readerMeasTaskSignaling.GetValue(c => c.CorrelationFactor);
                                measTask.SignalingMeasTaskParameters.CheckFreqChannel = readerMeasTaskSignaling.GetValue(c => c.CheckFreqChannel);
                                measTask.SignalingMeasTaskParameters.triggerLevel_dBm_Hz = readerMeasTaskSignaling.GetValue(c => c.TriggerLevel_dBm_Hz);



                                measTask.SignalingMeasTaskParameters.GroupingParameters = new SignalingGroupingParameters();
                                measTask.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals = readerMeasTaskSignaling.GetValue(c => c.CrossingBWPercentageForBadSignals);
                                measTask.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals = readerMeasTaskSignaling.GetValue(c => c.CrossingBWPercentageForGoodSignals);
                                measTask.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec = readerMeasTaskSignaling.GetValue(c => c.TimeBetweenWorkTimes_sec);
                                measTask.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum = readerMeasTaskSignaling.GetValue(c => c.TypeJoinSpectrum);
                                measTask.SignalingMeasTaskParameters.allowableExcess_dB = readerMeasTaskSignaling.GetValue(c => c.allowableExcess_dB);


                            }
                            return resultMeasTaskSignaling;
                        });



                        //Ref situation
                        var listReferenceSituation = new List<ReferenceSituation>();
                        var builderReferenceSituationRaw = this._dataLayer.GetBuilder<MD.IReferenceSituation>().From();
                        builderReferenceSituationRaw.Select(c => c.Id);
                        builderReferenceSituationRaw.Select(c => c.SENSOR.Id);
                        builderReferenceSituationRaw.Select(c => c.MEAS_TASK.Id);
                        builderReferenceSituationRaw.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderReferenceSituationRaw, readerReferenceSituationRaw =>
                        {
                            while (readerReferenceSituationRaw.Read())
                            {
                                var refSituation = new ReferenceSituation();

                                refSituation.SensorId = readerReferenceSituationRaw.GetValue(c => c.SENSOR.Id);

                                var referenceSignals = new List<ReferenceSignal>();
                                var builderReferenceSignalRaw = this._dataLayer.GetBuilder<MD.IReferenceSignal>().From();
                                builderReferenceSignalRaw.Select(c => c.Id);
                                builderReferenceSignalRaw.Select(c => c.Bandwidth_kHz);
                                builderReferenceSignalRaw.Select(c => c.Frequency_MHz);
                                builderReferenceSignalRaw.Select(c => c.LevelSignal_dBm);
                                builderReferenceSignalRaw.Select(c => c.REFERENCE_SITUATION.Id);
                                builderReferenceSignalRaw.Select(c => c.IcsmId);
                                builderReferenceSignalRaw.Select(c => c.IcsmTable);
                                builderReferenceSignalRaw.Select(c => c.Loss_dB);
                                builderReferenceSignalRaw.Select(c => c.Freq_kHz);
                                builderReferenceSignalRaw.Where(c => c.REFERENCE_SITUATION.Id, ConditionOperator.Equal, readerReferenceSituationRaw.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderReferenceSignalRaw, readerReferenceSignalRaw =>
                                {
                                    while (readerReferenceSignalRaw.Read())
                                    {

                                        var referenceSignal = new ReferenceSignal();
                                        if (readerReferenceSignalRaw.GetValue(c => c.Bandwidth_kHz) != null)
                                        {
                                            referenceSignal.Bandwidth_kHz = readerReferenceSignalRaw.GetValue(c => c.Bandwidth_kHz).Value;
                                        }
                                        if (readerReferenceSignalRaw.GetValue(c => c.Frequency_MHz) != null)
                                        {
                                            referenceSignal.Frequency_MHz = readerReferenceSignalRaw.GetValue(c => c.Frequency_MHz).Value;
                                        }
                                        if (readerReferenceSignalRaw.GetValue(c => c.LevelSignal_dBm) != null)
                                        {
                                            referenceSignal.LevelSignal_dBm = readerReferenceSignalRaw.GetValue(c => c.LevelSignal_dBm).Value;
                                        }
                                        if (readerReferenceSignalRaw.GetValue(c => c.IcsmId) != null)
                                        {
                                            referenceSignal.IcsmId = readerReferenceSignalRaw.GetValue(c => c.IcsmId).Value;
                                        }

                                        referenceSignal.IcsmTable = readerReferenceSignalRaw.GetValue(c => c.IcsmTable);
                                        referenceSignal.SignalMask = new SignalMask();
                                        if (readerReferenceSignalRaw.GetValue(c => c.Loss_dB) != null)
                                        {
                                            referenceSignal.SignalMask.Loss_dB = readerReferenceSignalRaw.GetValue(c => c.Loss_dB);
                                        }
                                        if (readerReferenceSignalRaw.GetValue(c => c.Freq_kHz) != null)
                                        {
                                            referenceSignal.SignalMask.Freq_kHz = readerReferenceSignalRaw.GetValue(c => c.Freq_kHz);
                                        }

                                        referenceSignals.Add(referenceSignal);
                                    }
                                    return true;
                                });

                                refSituation.ReferenceSignal = referenceSignals.ToArray();

                                listReferenceSituation.Add(refSituation);
                            }
                            return true;
                        });

                        if (listReferenceSituation.Count > 0)
                        {
                            measTask.RefSituation = listReferenceSituation.ToArray();
                        }


                        // MeasTimeParamList

                        var timeParamList = new MeasTimeParamList();
                        timeParamList.PerInterval = readerMeasTask.GetValue(c => c.PerInterval);
                        if (readerMeasTask.GetValue(c => c.PerStart) != null)
                        {
                            timeParamList.PerStart = readerMeasTask.GetValue(c => c.PerStart).Value;
                        }
                        if (readerMeasTask.GetValue(c => c.PerStop) != null)
                        {
                            timeParamList.PerStop = readerMeasTask.GetValue(c => c.PerStop).Value;
                        }
                        timeParamList.TimeStart = readerMeasTask.GetValue(c => c.TimeStart);
                        timeParamList.TimeStop = readerMeasTask.GetValue(c => c.TimeStop);
                        measTask.MeasTimeParamList = timeParamList;


                        int? swNumber = null;

                        //measTask.MeasDtParam = new MeasDtParam();
                        MeasurementType typeMeasurements;
                        if (Enum.TryParse<MeasurementType>(readerMeasTask.GetValue(c => c.Type), out typeMeasurements))
                        {
                            measTask.MeasDtParam = new MeasDtParam();
                            measTask.MeasDtParam.TypeMeasurements = typeMeasurements;
                        }
                        else
                        {
                            throw new InvalidOperationException("NOT set 'MeasurementType' for task Task!");
                        }


                        if (measTask.MeasDtParam.TypeMeasurements!= MeasurementType.MonitoringStations)
                        {
                            var builderMeasDtParam = this._dataLayer.GetBuilder<MD.IMeasDtParam>().From();
                            builderMeasDtParam.Select(c => c.Id);
                            builderMeasDtParam.Select(c => c.Demod);
                            builderMeasDtParam.Select(c => c.DetectType);
                            builderMeasDtParam.Select(c => c.Ifattenuation);
                            builderMeasDtParam.Select(c => c.MEAS_TASK.Id);
                            builderMeasDtParam.Select(c => c.MeasTime);
                            builderMeasDtParam.Select(c => c.Mode);
                            builderMeasDtParam.Select(c => c.Preamplification);
                            builderMeasDtParam.Select(c => c.Rbw);
                            builderMeasDtParam.Select(c => c.Rfattenuation);
                            builderMeasDtParam.Select(c => c.ReferenceLevel);
                            builderMeasDtParam.Select(c => c.NumberTotalScan);
                            builderMeasDtParam.Select(c => c.Vbw);
                            builderMeasDtParam.Select(c => c.SwNumber);
                            builderMeasDtParam.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                            queryExecuter.Fetch(builderMeasDtParam, readerMeasDtParam =>
                            {
                                while (readerMeasDtParam.Read())
                                {
                                    var dtx = new MeasDtParam();
                                    dtx.TypeMeasurements = measTask.MeasDtParam.TypeMeasurements;
                                    dtx.Demod = readerMeasDtParam.GetValue(c => c.Demod);
                                    DetectingType detectType;
                                    if (Enum.TryParse<DetectingType>(readerMeasDtParam.GetValue(c => c.DetectType), out detectType))
                                        dtx.DetectType = detectType;

                                    dtx.ReferenceLevel = readerMeasDtParam.GetValue(c => c.ReferenceLevel);
                                    dtx.NumberTotalScan = readerMeasDtParam.GetValue(c => c.NumberTotalScan);
                                    dtx.IfAttenuation = readerMeasDtParam.GetValue(c => c.Ifattenuation).HasValue ? readerMeasDtParam.GetValue(c => c.Ifattenuation).Value : 0;
                                    dtx.MeasTime = readerMeasDtParam.GetValue(c => c.MeasTime);
                                    MeasurementMode mode;
                                    if (Enum.TryParse<MeasurementMode>(readerMeasDtParam.GetValue(c => c.Mode), out mode))
                                        dtx.Mode = mode;

                                    dtx.Preamplification = readerMeasDtParam.GetValue(c => c.Preamplification).HasValue ? readerMeasDtParam.GetValue(c => c.Preamplification).Value : -1;
                                    dtx.RBW = readerMeasDtParam.GetValue(c => c.Rbw);
                                    swNumber = readerMeasDtParam.GetValue(c => c.SwNumber);

                                    dtx.RfAttenuation = readerMeasDtParam.GetValue(c => c.Rfattenuation).HasValue ? readerMeasDtParam.GetValue(c => c.Rfattenuation).Value : 0;

                                    dtx.VBW = readerMeasDtParam.GetValue(c => c.Vbw);
                                    measTask.MeasDtParam = dtx;

                                }
                                return true;
                            });
                        }


                        // IMeasFreqParam


                        var builderMeasFreqParam = this._dataLayer.GetBuilder<MD.IMeasFreqParam>().From();
                        builderMeasFreqParam.Select(c => c.Id);
                        builderMeasFreqParam.Select(c => c.MEAS_TASK.Id);
                        builderMeasFreqParam.Select(c => c.Mode);
                        builderMeasFreqParam.Select(c => c.Rgl);
                        builderMeasFreqParam.Select(c => c.Rgu);
                        builderMeasFreqParam.Select(c => c.Step);
                        builderMeasFreqParam.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasFreqParam, readerMeasFreqParam =>
                        {
                            while (readerMeasFreqParam.Read())
                            {
                                var freqParam = new MeasFreqParam();
                                FrequencyMode Mode;
                                if (Enum.TryParse<FrequencyMode>(readerMeasFreqParam.GetValue(x => x.Mode), out Mode))
                                    freqParam.Mode = Mode;
                                freqParam.RgL = readerMeasFreqParam.GetValue(x => x.Rgl);
                                freqParam.RgU = readerMeasFreqParam.GetValue(x => x.Rgu);
                                freqParam.Step = readerMeasFreqParam.GetValue(x => x.Step);


                                var listmeasFreq = new List<MeasFreq>();
                                var builderMeasFreq = this._dataLayer.GetBuilder<MD.IMeasFreq>().From();
                                builderMeasFreq.Select(c => c.Id);
                                builderMeasFreq.Select(c => c.Freq);
                                builderMeasFreq.Select(c => c.MEAS_FREQ_PARAM.Id);
                                builderMeasFreq.Where(c => c.MEAS_FREQ_PARAM.Id, ConditionOperator.Equal, readerMeasFreqParam.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderMeasFreq, readerMeasFreq =>
                                {
                                    while (readerMeasFreq.Read())
                                    {
                                        var measFreq = new MeasFreq();
                                        if (readerMeasFreq.GetValue(c => c.Freq) != null)
                                        {
                                            measFreq.Freq = readerMeasFreq.GetValue(c => c.Freq).Value;
                                            listmeasFreq.Add(measFreq);
                                        }
                                    }
                                    return true;
                                });
                                freqParam.MeasFreqs = listmeasFreq.ToArray();
                                measTask.MeasFreqParam = freqParam;

                            }
                            return true;
                        });


                        // IMeasLocationParam

                        var measLocParams = new List<MeasLocParam>();
                        var builderMeasLocationParam = this._dataLayer.GetBuilder<MD.IMeasLocationParam>().From();
                        builderMeasLocationParam.Select(c => c.Id);
                        builderMeasLocationParam.Select(c => c.Asl);
                        builderMeasLocationParam.Select(c => c.Lat);
                        builderMeasLocationParam.Select(c => c.Lon);
                        builderMeasLocationParam.Select(c => c.MaxDist);
                        builderMeasLocationParam.Select(c => c.MEAS_TASK.Id);
                        builderMeasLocationParam.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasLocationParam, readermeasLocParam =>
                        {
                            while (readermeasLocParam.Read())
                            {
                                var measLocParam = new MeasLocParam();
                                measLocParam.ASL = readermeasLocParam.GetValue(c => c.Asl);
                                measLocParam.Lat = readermeasLocParam.GetValue(c => c.Lat);
                                measLocParam.Lon = readermeasLocParam.GetValue(c => c.Lon);
                                measLocParam.MaxDist = readermeasLocParam.GetValue(c => c.MaxDist);
                                measLocParams.Add(measLocParam);
                            }
                            return true;
                        });
                        measTask.MeasLocParams = measLocParams.ToArray();



                        var measOther = new MeasOther();
                        var builderMeasOther = this._dataLayer.GetBuilder<MD.IMeasOther>().From();
                        builderMeasOther.Select(c => c.Id);
                        builderMeasOther.Select(c => c.LevelMinOccup);
                        builderMeasOther.Select(c => c.SupportMultyLevel);
                        builderMeasOther.Select(c => c.MEAS_TASK.Id);
                        builderMeasOther.Select(c => c.Nchenal);
                        builderMeasOther.Select(c => c.TypeSpectrumOccupation);
                        builderMeasOther.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasOther, readerMeasOther =>
                        {
                            while (readerMeasOther.Read())
                            {
                                measOther.LevelMinOccup = readerMeasOther.GetValue(c => c.LevelMinOccup);
                                measOther.SupportMultyLevel = readerMeasOther.GetValue(c => c.SupportMultyLevel);
                                measOther.NChenal = readerMeasOther.GetValue(c => c.Nchenal);

                                SpectrumOccupationType typeSpectrumOccupation;
                                if (Enum.TryParse<SpectrumOccupationType>(readerMeasOther.GetValue(c => c.TypeSpectrumOccupation), out typeSpectrumOccupation))
                                {
                                    measOther.TypeSpectrumOccupation = typeSpectrumOccupation;
                                }

                            }
                            return true;
                        });
                        measTask.MeasOther = measOther;
                        measTask.MeasOther.SwNumber = swNumber;

                        var measSensors = new List<MeasSensor>();
                        var listmeasSubTask = new List<MeasSubTask>();
                        var builderMeasSubTask = this._dataLayer.GetBuilder<MD.ISubTask>().From();
                        builderMeasSubTask.Select(c => c.Id);
                        builderMeasSubTask.Select(c => c.Interval);
                        builderMeasSubTask.Select(c => c.MEAS_TASK.Id);
                        builderMeasSubTask.Select(c => c.Status);
                        builderMeasSubTask.Select(c => c.TimeStart);
                        builderMeasSubTask.Select(c => c.TimeStop);
                        builderMeasSubTask.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasSubTask, readerMeasSubTask =>
                        {
                            while (readerMeasSubTask.Read())
                            {
                                var measSubTask = new MeasSubTask();
                                measSubTask.Id = new MeasTaskIdentifier();
                                measSubTask.Id.Value = readerMeasSubTask.GetValue(c => c.Id);
                                measSubTask.Interval = readerMeasSubTask.GetValue(c => c.Interval);
                                measSubTask.Status = readerMeasSubTask.GetValue(c => c.Status);
                                if (readerMeasSubTask.GetValue(c => c.TimeStart) != null) measSubTask.TimeStart = readerMeasSubTask.GetValue(c => c.TimeStart).Value;
                                if (readerMeasSubTask.GetValue(c => c.TimeStop) != null) measSubTask.TimeStop = readerMeasSubTask.GetValue(c => c.TimeStop).Value;
                                var listMeasSubTaskStation = new List<MeasSubTaskSensor>();
                                var builderMeasSubTaskSta = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().From();
                                builderMeasSubTaskSta.Select(c => c.Id);
                                builderMeasSubTaskSta.Select(c => c.Count);
                                builderMeasSubTaskSta.Select(c => c.SUBTASK.Id);
                                builderMeasSubTaskSta.Select(c => c.SENSOR.Id);
                                builderMeasSubTaskSta.Select(c => c.Status);
                                builderMeasSubTaskSta.Select(c => c.TimeNextTask);
                                builderMeasSubTaskSta.Where(c => c.SUBTASK.Id, ConditionOperator.Equal, readerMeasSubTask.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderMeasSubTaskSta, readerMeasSubTaskSta =>
                                {
                                    while (readerMeasSubTaskSta.Read())
                                    {
                                        var measSubTaskSensor = new MeasSubTaskSensor();
                                        measSubTaskSensor.Count = readerMeasSubTaskSta.GetValue(c => c.Count);
                                        measSubTaskSensor.Id = readerMeasSubTaskSta.GetValue(c => c.Id);
                                        measSubTaskSensor.SensorId = new SensorIdentifier();
                                        measSubTaskSensor.SensorId.Value = readerMeasSubTaskSta.GetValue(c => c.SENSOR.Id);
                                        measSubTaskSensor.Status = readerMeasSubTaskSta.GetValue(c => c.Status);
                                        measSubTaskSensor.TimeNextTask = readerMeasSubTaskSta.GetValue(c => c.TimeNextTask);
                                        listMeasSubTaskStation.Add(measSubTaskSensor);
                                        measSensors.Add(new MeasSensor()
                                        {
                                            SensorId = new MeasSensorIdentifier()
                                            {
                                                Value = measSubTaskSensor.SensorId.Value
                                            }
                                        });
                                    }
                                    return true;
                                });
                                measSubTask.MeasSubTaskSensors = listMeasSubTaskStation.ToArray();
                                listmeasSubTask.Add(measSubTask);
                            }
                            return true;
                        });

                        measTask.MeasSubTasks = listmeasSubTask.ToArray();
                        measTask.Sensors = measSensors.ToArray();
                        listMeasTask.Add(measTask);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listMeasTask;
        }

        public List<MeasTask> ShortReadTaskWithoutRefSituation(long id)
        {
            var listMeasTask = new List<MeasTask>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallShortReadTaskMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().From();
                builderMeasTask.Select(c => c.CreatedBy);
                builderMeasTask.Select(c => c.DateCreated);
                builderMeasTask.Select(c => c.ExecutionMode);
                builderMeasTask.Select(c => c.Id);
                builderMeasTask.Select(c => c.IdentStart);
                builderMeasTask.Select(c => c.MaxTimeBs);
                builderMeasTask.Select(c => c.Name);
                builderMeasTask.Select(c => c.OrderId);
                builderMeasTask.Select(c => c.PerInterval);
                builderMeasTask.Select(c => c.PerStart);
                builderMeasTask.Select(c => c.PerStop);
                builderMeasTask.Select(c => c.Prio);
                builderMeasTask.Select(c => c.ResultType);
                builderMeasTask.Select(c => c.Status);
                builderMeasTask.Select(c => c.Task);
                builderMeasTask.Select(c => c.TimeStart);
                builderMeasTask.Select(c => c.TimeStop);
                builderMeasTask.Select(c => c.Type);
                builderMeasTask.Where(c => c.Id, ConditionOperator.Equal, id);
                builderMeasTask.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                builderMeasTask.Where(c => c.Status, ConditionOperator.IsNotNull);
                queryExecuter.Fetch(builderMeasTask, readerMeasTask =>
                {
                    var measTask = new MeasTask();
                    while (readerMeasTask.Read())
                    {
                        measTask.CreatedBy = readerMeasTask.GetValue(c => c.CreatedBy);
                        measTask.DateCreated = readerMeasTask.GetValue(c => c.DateCreated);
                        MeasTaskExecutionMode ExecutionMode;
                        if (Enum.TryParse<MeasTaskExecutionMode>(readerMeasTask.GetValue(c => c.ExecutionMode), out ExecutionMode))
                        {
                            measTask.ExecutionMode = ExecutionMode;
                        }
                        measTask.Id = new MeasTaskIdentifier();
                        measTask.Id.Value = readerMeasTask.GetValue(c => c.Id);
                        measTask.MaxTimeBs = readerMeasTask.GetValue(c => c.MaxTimeBs);
                        measTask.Name = readerMeasTask.GetValue(c => c.Name);
                        measTask.OrderId = readerMeasTask.GetValue(c => c.OrderId);
                        measTask.Prio = readerMeasTask.GetValue(c => c.Prio);
                        MeasTaskResultType ResultType;
                        if (Enum.TryParse<MeasTaskResultType>(readerMeasTask.GetValue(c => c.ResultType), out ResultType))
                        {
                            measTask.ResultType = ResultType;
                        }
                        measTask.Status = readerMeasTask.GetValue(c => c.Status);
                        MeasTaskType Task;
                        if (Enum.TryParse<MeasTaskType>(readerMeasTask.GetValue(c => c.Task), out Task))
                        {
                            measTask.Task = Task;
                        }
                        measTask.Type = readerMeasTask.GetValue(c => c.Type);


                        measTask.SignalingMeasTaskParameters = new SignalingMeasTask();
                        var builderMeasTaskSignaling = this._dataLayer.GetBuilder<MD.IMeasTaskSignaling>().From();
                        builderMeasTaskSignaling.Select(c => c.Id);
                        builderMeasTaskSignaling.Select(c => c.MEAS_TASK.Id);


                        builderMeasTaskSignaling.Select(c => c.allowableExcess_dB);
                        builderMeasTaskSignaling.Select(c => c.AutoDivisionEmitting);
                        builderMeasTaskSignaling.Select(c => c.CompareTraceJustWithRefLevels);
                        builderMeasTaskSignaling.Select(c => c.DifferenceMaxMax);
                        builderMeasTaskSignaling.Select(c => c.FiltrationTrace);
                        builderMeasTaskSignaling.Select(c => c.SignalizationNChenal);
                        builderMeasTaskSignaling.Select(c => c.SignalizationNCount);
                        builderMeasTaskSignaling.Select(c => c.CorrelationAnalize);
                        builderMeasTaskSignaling.Select(c => c.CheckFreqChannel);
                        builderMeasTaskSignaling.Select(c => c.AnalyzeByChannel);
                        builderMeasTaskSignaling.Select(c => c.AnalyzeSysInfoEmission);
                        builderMeasTaskSignaling.Select(c => c.DetailedMeasurementsBWEmission);
                        builderMeasTaskSignaling.Select(c => c.CorrelationFactor);
                        builderMeasTaskSignaling.Select(c => c.Standard);
                        builderMeasTaskSignaling.Select(c => c.TriggerLevel_dBm_Hz);
                        builderMeasTaskSignaling.Select(c => c.NumberPointForChangeExcess);
                        builderMeasTaskSignaling.Select(c => c.WindowBW);
                        builderMeasTaskSignaling.Select(c => c.DiffLevelForCalcBW);
                        builderMeasTaskSignaling.Select(c => c.NDbLevel_dB);
                        builderMeasTaskSignaling.Select(c => c.NumberIgnoredPoints);
                        builderMeasTaskSignaling.Select(c => c.MinExcessNoseLevel_dB);
                        builderMeasTaskSignaling.Select(c => c.TimeBetweenWorkTimes_sec);
                        builderMeasTaskSignaling.Select(c => c.TypeJoinSpectrum);
                        builderMeasTaskSignaling.Select(c => c.CrossingBWPercentageForGoodSignals);
                        builderMeasTaskSignaling.Select(c => c.CrossingBWPercentageForBadSignals);
                        builderMeasTaskSignaling.Select(c => c.MaxFreqDeviation);
                        builderMeasTaskSignaling.Select(c => c.CheckLevelChannel);
                        builderMeasTaskSignaling.Select(c => c.MinPointForDetailBW);
                        builderMeasTaskSignaling.Select(c => c.CollectEmissionInstrumentalEstimation);
                        builderMeasTaskSignaling.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasTaskSignaling, readerMeasTaskSignaling =>
                        {
                            var resultMeasTaskSignaling = true;
                            while (readerMeasTaskSignaling.Read())
                            {
                                measTask.SignalingMeasTaskParameters.InterruptionParameters = new SignalingInterruptionParameters();
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting = readerMeasTaskSignaling.GetValue(c => c.AutoDivisionEmitting);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax = readerMeasTaskSignaling.GetValue(c => c.DifferenceMaxMax);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW = readerMeasTaskSignaling.GetValue(c => c.DiffLevelForCalcBW);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB = readerMeasTaskSignaling.GetValue(c => c.MinExcessNoseLevel_dB);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB = readerMeasTaskSignaling.GetValue(c => c.NDbLevel_dB);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints = readerMeasTaskSignaling.GetValue(c => c.NumberIgnoredPoints);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess = readerMeasTaskSignaling.GetValue(c => c.NumberPointForChangeExcess);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.windowBW = readerMeasTaskSignaling.GetValue(c => c.WindowBW);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation = readerMeasTaskSignaling.GetValue(c => c.MaxFreqDeviation);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel = readerMeasTaskSignaling.GetValue(c => c.CheckLevelChannel);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW = readerMeasTaskSignaling.GetValue(c => c.MinPointForDetailBW);
                                measTask.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels = readerMeasTaskSignaling.GetValue(c => c.CompareTraceJustWithRefLevels);
                                measTask.SignalingMeasTaskParameters.FiltrationTrace = readerMeasTaskSignaling.GetValue(c => c.FiltrationTrace);
                                measTask.SignalingMeasTaskParameters.SignalizationNChenal = readerMeasTaskSignaling.GetValue(c => c.SignalizationNChenal);
                                measTask.SignalingMeasTaskParameters.SignalizationNCount = readerMeasTaskSignaling.GetValue(c => c.SignalizationNCount);
                                measTask.SignalingMeasTaskParameters.AnalyzeByChannel = readerMeasTaskSignaling.GetValue(c => c.AnalyzeByChannel);
                                measTask.SignalingMeasTaskParameters.CollectEmissionInstrumentalEstimation = readerMeasTaskSignaling.GetValue(c => c.CollectEmissionInstrumentalEstimation);
                                measTask.SignalingMeasTaskParameters.AnalyzeSysInfoEmission = readerMeasTaskSignaling.GetValue(c => c.AnalyzeSysInfoEmission);
                                measTask.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission = readerMeasTaskSignaling.GetValue(c => c.DetailedMeasurementsBWEmission);
                                measTask.SignalingMeasTaskParameters.Standard = readerMeasTaskSignaling.GetValue(c => c.Standard);
                                measTask.SignalingMeasTaskParameters.CorrelationAnalize = readerMeasTaskSignaling.GetValue(c => c.CorrelationAnalize);
                                measTask.SignalingMeasTaskParameters.CorrelationFactor = readerMeasTaskSignaling.GetValue(c => c.CorrelationFactor);
                                measTask.SignalingMeasTaskParameters.CheckFreqChannel = readerMeasTaskSignaling.GetValue(c => c.CheckFreqChannel);
                                measTask.SignalingMeasTaskParameters.triggerLevel_dBm_Hz = readerMeasTaskSignaling.GetValue(c => c.TriggerLevel_dBm_Hz);
                                measTask.SignalingMeasTaskParameters.GroupingParameters = new SignalingGroupingParameters();
                                measTask.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals = readerMeasTaskSignaling.GetValue(c => c.CrossingBWPercentageForBadSignals);
                                measTask.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals = readerMeasTaskSignaling.GetValue(c => c.CrossingBWPercentageForGoodSignals);
                                measTask.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec = readerMeasTaskSignaling.GetValue(c => c.TimeBetweenWorkTimes_sec);
                                measTask.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum = readerMeasTaskSignaling.GetValue(c => c.TypeJoinSpectrum);
                                measTask.SignalingMeasTaskParameters.allowableExcess_dB = readerMeasTaskSignaling.GetValue(c => c.allowableExcess_dB);
                            }
                            return resultMeasTaskSignaling;
                        });


                        // MeasTimeParamList

                        var timeParamList = new MeasTimeParamList();
                        timeParamList.PerInterval = readerMeasTask.GetValue(c => c.PerInterval);
                        if (readerMeasTask.GetValue(c => c.PerStart) != null)
                        {
                            timeParamList.PerStart = readerMeasTask.GetValue(c => c.PerStart).Value;
                        }
                        if (readerMeasTask.GetValue(c => c.PerStop) != null)
                        {
                            timeParamList.PerStop = readerMeasTask.GetValue(c => c.PerStop).Value;
                        }
                        timeParamList.TimeStart = readerMeasTask.GetValue(c => c.TimeStart);
                        timeParamList.TimeStop = readerMeasTask.GetValue(c => c.TimeStop);
                        measTask.MeasTimeParamList = timeParamList;

      
                        // IMeasDtParam
                        int? swNumber = null;
                    
                        
                         //measTask.MeasDtParam = new MeasDtParam();
                         MeasurementType typeMeasurements;
                         if (Enum.TryParse<MeasurementType>(readerMeasTask.GetValue(c => c.Type), out typeMeasurements))
                         {
                               measTask.MeasDtParam = new MeasDtParam();
                               measTask.MeasDtParam.TypeMeasurements = typeMeasurements;
                         }
                         else
                         {
                               throw new InvalidOperationException("NOT set 'MeasurementType' for task Task!");
                         }

                        if (measTask.MeasDtParam.TypeMeasurements != MeasurementType.MonitoringStations)
                        {
                            var builderMeasDtParam = this._dataLayer.GetBuilder<MD.IMeasDtParam>().From();
                            builderMeasDtParam.Select(c => c.Id);
                            builderMeasDtParam.Select(c => c.Demod);
                            builderMeasDtParam.Select(c => c.DetectType);
                            builderMeasDtParam.Select(c => c.Ifattenuation);
                            builderMeasDtParam.Select(c => c.MEAS_TASK.Id);
                            builderMeasDtParam.Select(c => c.MeasTime);
                            builderMeasDtParam.Select(c => c.Mode);
                            builderMeasDtParam.Select(c => c.Preamplification);
                            builderMeasDtParam.Select(c => c.Rbw);
                            builderMeasDtParam.Select(c => c.Rfattenuation);
                            builderMeasDtParam.Select(c => c.Vbw);
                            builderMeasDtParam.Select(c => c.SwNumber);
                            builderMeasDtParam.Select(c => c.ReferenceLevel);
                            builderMeasDtParam.Select(c => c.NumberTotalScan);
                            builderMeasDtParam.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                            queryExecuter.Fetch(builderMeasDtParam, readerMeasDtParam =>
                            {
                                while (readerMeasDtParam.Read())
                                {
                                    var dtx = new MeasDtParam();
                                    dtx.TypeMeasurements = measTask.MeasDtParam.TypeMeasurements;
                                    dtx.Demod = readerMeasDtParam.GetValue(c => c.Demod);
                                    DetectingType detectType;
                                    if (Enum.TryParse<DetectingType>(readerMeasDtParam.GetValue(c => c.DetectType), out detectType))
                                        dtx.DetectType = detectType;

                                    dtx.ReferenceLevel = readerMeasDtParam.GetValue(c => c.ReferenceLevel);
                                    dtx.NumberTotalScan = readerMeasDtParam.GetValue(c => c.NumberTotalScan);
                                    dtx.IfAttenuation = readerMeasDtParam.GetValue(c => c.Ifattenuation).HasValue ? readerMeasDtParam.GetValue(c => c.Ifattenuation).Value : 0;
                                    dtx.MeasTime = readerMeasDtParam.GetValue(c => c.MeasTime);
                                    MeasurementMode mode;
                                    if (Enum.TryParse<MeasurementMode>(readerMeasDtParam.GetValue(c => c.Mode), out mode))
                                        dtx.Mode = mode;

                                    dtx.Preamplification = readerMeasDtParam.GetValue(c => c.Preamplification).HasValue ? readerMeasDtParam.GetValue(c => c.Preamplification).Value : -1;
                                    dtx.RBW = readerMeasDtParam.GetValue(c => c.Rbw);
                                    swNumber = readerMeasDtParam.GetValue(c => c.SwNumber);
                                    dtx.RfAttenuation = readerMeasDtParam.GetValue(c => c.Rfattenuation).HasValue ? readerMeasDtParam.GetValue(c => c.Rfattenuation).Value : 0;

                                    dtx.VBW = readerMeasDtParam.GetValue(c => c.Vbw);
                                    measTask.MeasDtParam = dtx;

                                }
                                return true;
                            });
                        }

                        // IMeasFreqParam


                        var builderMeasFreqParam = this._dataLayer.GetBuilder<MD.IMeasFreqParam>().From();
                        builderMeasFreqParam.Select(c => c.Id);
                        builderMeasFreqParam.Select(c => c.MEAS_TASK.Id);
                        builderMeasFreqParam.Select(c => c.Mode);
                        builderMeasFreqParam.Select(c => c.Rgl);
                        builderMeasFreqParam.Select(c => c.Rgu);
                        builderMeasFreqParam.Select(c => c.Step);
                        builderMeasFreqParam.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasFreqParam, readerMeasFreqParam =>
                        {
                            while (readerMeasFreqParam.Read())
                            {
                                var freqParam = new MeasFreqParam();
                                FrequencyMode Mode;
                                if (Enum.TryParse<FrequencyMode>(readerMeasFreqParam.GetValue(x => x.Mode), out Mode))
                                    freqParam.Mode = Mode;
                                freqParam.RgL = readerMeasFreqParam.GetValue(x => x.Rgl);
                                freqParam.RgU = readerMeasFreqParam.GetValue(x => x.Rgu);
                                freqParam.Step = readerMeasFreqParam.GetValue(x => x.Step);


                                var listmeasFreq = new List<MeasFreq>();
                                var builderMeasFreq = this._dataLayer.GetBuilder<MD.IMeasFreq>().From();
                                builderMeasFreq.Select(c => c.Id);
                                builderMeasFreq.Select(c => c.Freq);
                                builderMeasFreq.Select(c => c.MEAS_FREQ_PARAM.Id);
                                builderMeasFreq.Where(c => c.MEAS_FREQ_PARAM.Id, ConditionOperator.Equal, readerMeasFreqParam.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderMeasFreq, readerMeasFreq =>
                                {
                                    while (readerMeasFreq.Read())
                                    {
                                        var measFreq = new MeasFreq();
                                        if (readerMeasFreq.GetValue(c => c.Freq) != null)
                                        {
                                            measFreq.Freq = readerMeasFreq.GetValue(c => c.Freq).Value;
                                            listmeasFreq.Add(measFreq);
                                        }
                                    }
                                    return true;
                                });
                                freqParam.MeasFreqs = listmeasFreq.ToArray();
                                measTask.MeasFreqParam = freqParam;

                            }
                            return true;
                        });


                        // IMeasLocationParam

                        var measLocParams = new List<MeasLocParam>();
                        var builderMeasLocationParam = this._dataLayer.GetBuilder<MD.IMeasLocationParam>().From();
                        builderMeasLocationParam.Select(c => c.Id);
                        builderMeasLocationParam.Select(c => c.Asl);
                        builderMeasLocationParam.Select(c => c.Lat);
                        builderMeasLocationParam.Select(c => c.Lon);
                        builderMeasLocationParam.Select(c => c.MaxDist);
                        builderMeasLocationParam.Select(c => c.MEAS_TASK.Id);
                        builderMeasLocationParam.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasLocationParam, readermeasLocParam =>
                        {
                            while (readermeasLocParam.Read())
                            {
                                var measLocParam = new MeasLocParam();
                                measLocParam.ASL = readermeasLocParam.GetValue(c => c.Asl);
                                measLocParam.Lat = readermeasLocParam.GetValue(c => c.Lat);
                                measLocParam.Lon = readermeasLocParam.GetValue(c => c.Lon);
                                measLocParam.MaxDist = readermeasLocParam.GetValue(c => c.MaxDist);
                                measLocParams.Add(measLocParam);
                            }
                            return true;
                        });
                        measTask.MeasLocParams = measLocParams.ToArray();



                        var measOther = new MeasOther();
                        var builderMeasOther = this._dataLayer.GetBuilder<MD.IMeasOther>().From();
                        builderMeasOther.Select(c => c.Id);
                        builderMeasOther.Select(c => c.LevelMinOccup);
                        builderMeasOther.Select(c => c.SupportMultyLevel);
                        builderMeasOther.Select(c => c.MEAS_TASK.Id);
                        builderMeasOther.Select(c => c.Nchenal);
                        builderMeasOther.Select(c => c.TypeSpectrumOccupation);
                        builderMeasOther.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasOther, readerMeasOther =>
                        {
                            while (readerMeasOther.Read())
                            {
                                measOther.LevelMinOccup = readerMeasOther.GetValue(c => c.LevelMinOccup);
                                measOther.SupportMultyLevel = readerMeasOther.GetValue(c => c.SupportMultyLevel);
                                measOther.NChenal = readerMeasOther.GetValue(c => c.Nchenal);

                                SpectrumOccupationType typeSpectrumOccupation;
                                if (Enum.TryParse<SpectrumOccupationType>(readerMeasOther.GetValue(c => c.TypeSpectrumOccupation), out typeSpectrumOccupation))
                                {
                                    measOther.TypeSpectrumOccupation = typeSpectrumOccupation;
                                }

                            }
                            return true;
                        });
                        measTask.MeasOther = measOther;
                        measTask.MeasOther.SwNumber = swNumber;

                        var measSensors = new List<MeasSensor>();
                        var listmeasSubTask = new List<MeasSubTask>();
                        var builderMeasSubTask = this._dataLayer.GetBuilder<MD.ISubTask>().From();
                        builderMeasSubTask.Select(c => c.Id);
                        builderMeasSubTask.Select(c => c.Interval);
                        builderMeasSubTask.Select(c => c.MEAS_TASK.Id);
                        builderMeasSubTask.Select(c => c.Status);
                        builderMeasSubTask.Select(c => c.TimeStart);
                        builderMeasSubTask.Select(c => c.TimeStop);
                        builderMeasSubTask.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasSubTask, readerMeasSubTask =>
                        {
                            while (readerMeasSubTask.Read())
                            {
                                var measSubTask = new MeasSubTask();
                                measSubTask.Id = new MeasTaskIdentifier();
                                measSubTask.Id.Value = readerMeasSubTask.GetValue(c => c.Id);
                                measSubTask.Interval = readerMeasSubTask.GetValue(c => c.Interval);
                                measSubTask.Status = readerMeasSubTask.GetValue(c => c.Status);
                                if (readerMeasSubTask.GetValue(c => c.TimeStart) != null) measSubTask.TimeStart = readerMeasSubTask.GetValue(c => c.TimeStart).Value;
                                if (readerMeasSubTask.GetValue(c => c.TimeStop) != null) measSubTask.TimeStop = readerMeasSubTask.GetValue(c => c.TimeStop).Value;
                                var listMeasSubTaskStation = new List<MeasSubTaskSensor>();
                                var builderMeasSubTaskSta = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().From();
                                builderMeasSubTaskSta.Select(c => c.Id);
                                builderMeasSubTaskSta.Select(c => c.Count);
                                builderMeasSubTaskSta.Select(c => c.SUBTASK.Id);
                                builderMeasSubTaskSta.Select(c => c.SENSOR.Id);
                                builderMeasSubTaskSta.Select(c => c.Status);
                                builderMeasSubTaskSta.Select(c => c.TimeNextTask);
                                builderMeasSubTaskSta.Where(c => c.SUBTASK.Id, ConditionOperator.Equal, readerMeasSubTask.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderMeasSubTaskSta, readerMeasSubTaskSta =>
                                {
                                    while (readerMeasSubTaskSta.Read())
                                    {
                                        var measSubTaskSensor = new MeasSubTaskSensor();
                                        measSubTaskSensor.Count = readerMeasSubTaskSta.GetValue(c => c.Count);
                                        measSubTaskSensor.Id = readerMeasSubTaskSta.GetValue(c => c.Id);
                                        measSubTaskSensor.SensorId = new SensorIdentifier();
                                        measSubTaskSensor.SensorId.Value = readerMeasSubTaskSta.GetValue(c => c.SENSOR.Id);
                                        measSubTaskSensor.Status = readerMeasSubTaskSta.GetValue(c => c.Status);
                                        measSubTaskSensor.TimeNextTask = readerMeasSubTaskSta.GetValue(c => c.TimeNextTask);
                                        listMeasSubTaskStation.Add(measSubTaskSensor);
                                        measSensors.Add(new MeasSensor()
                                        {
                                            SensorId = new MeasSensorIdentifier()
                                            {
                                                Value = measSubTaskSensor.SensorId.Value
                                            }
                                        });
                                    }
                                    return true;
                                });
                                measSubTask.MeasSubTaskSensors = listMeasSubTaskStation.ToArray();
                                listmeasSubTask.Add(measSubTask);
                            }
                            return true;
                        });

                        measTask.MeasSubTasks = listmeasSubTask.ToArray();
                        measTask.Sensors = measSensors.ToArray();
                        listMeasTask.Add(measTask);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listMeasTask;
        }

        public StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(long taskId)
        {
            var listStationData = new List<StationDataForMeasurements>();
            try
            {
                List<KeyValuePair<long, StationDataForMeasurements>> idStations = new List<KeyValuePair<long, StationDataForMeasurements>>();
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetStationDataForMeasurementsByTaskIdMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderStation = this._dataLayer.GetBuilder<MD.IStation>().From();
                builderStation.Select(c => c.MEAS_TASK.Id);
                builderStation.Select(c => c.Id);
                builderStation.Select(c => c.STATION_SITE.Id);
                builderStation.Select(c => c.CloseDate);
                builderStation.Select(c => c.DozvilName);
                builderStation.Select(c => c.EndDate);
                builderStation.Select(c => c.GlobalSID);
                builderStation.Select(c => c.OWNER_DATA.Id);
                builderStation.Select(c => c.Standart);
                builderStation.Select(c => c.StartDate);
                builderStation.Select(c => c.ClientPermissionCode);
                builderStation.Select(c => c.ClientStationCode);
                builderStation.Select(c => c.Status);
                builderStation.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                builderStation.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderStation, readerStation =>
                {
                    while (readerStation.Read())
                    {
                        var measStation = new StationDataForMeasurements();
                        measStation.IdStation = readerStation.GetValue(c => c.ClientStationCode).HasValue ? readerStation.GetValue(c => c.ClientStationCode).Value : -1;
                        measStation.GlobalSID = readerStation.GetValue(c => c.GlobalSID);
                        measStation.Standart = readerStation.GetValue(c => c.Standart);
                        measStation.Status = readerStation.GetValue(c => c.Status);
                        var perm = new PermissionForAssignment();
                        perm.CloseDate = readerStation.GetValue(c => c.CloseDate);
                        perm.DozvilName = readerStation.GetValue(c => c.DozvilName);
                        perm.EndDate = readerStation.GetValue(c => c.EndDate);
                        perm.Id = readerStation.GetValue(c => c.ClientPermissionCode);
                        perm.StartDate = readerStation.GetValue(c => c.StartDate);
                        measStation.LicenseParameter = perm;
                        measStation.IdSite =  readerStation.GetValue(c => c.STATION_SITE.Id);
                        measStation.IdOwner = readerStation.GetValue(c => c.OWNER_DATA.Id);
                        idStations.Add(new KeyValuePair<long, StationDataForMeasurements>(readerStation.GetValue(c => c.Id), measStation));
                    }
                    return true;
                });



                var listOwnerData = new List<OwnerData>();
                var idOwners = idStations.ToList().Select(c => c.Value.IdOwner);
                var listOwnerDataTemp = new List<long>();
                if ((idOwners != null))
                {
                    var arrIds = idOwners.ToArray();
                    if (arrIds.Length > 0)
                    {
                        for (int i = 0; i < arrIds.Length; i++)
                        {
                            listOwnerDataTemp.Add(arrIds[i]);
                            if (listOwnerDataTemp.Count == 1000)
                            {
                                var builderOwnerData = this._dataLayer.GetBuilder<MD.IOwnerData>().From();
                                builderOwnerData.Select(c => c.Address);
                                builderOwnerData.Select(c => c.CODE);
                                builderOwnerData.Select(c => c.Id);
                                builderOwnerData.Select(c => c.OKPO);
                                builderOwnerData.Select(c => c.OwnerName);
                                builderOwnerData.Select(c => c.ZIP);
                                builderOwnerData.Where(c => c.Id, ConditionOperator.In, listOwnerDataTemp.ToArray());
                                builderOwnerData.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderOwnerData, readerOwnerData =>
                                {
                                    while (readerOwnerData.Read())
                                    {
                                        var ownerData = new OwnerData();
                                        ownerData.Addres = readerOwnerData.GetValue(c => c.Address);
                                        ownerData.Code = readerOwnerData.GetValue(c => c.CODE);
                                        ownerData.OKPO = readerOwnerData.GetValue(c => c.OKPO);
                                        ownerData.OwnerName = readerOwnerData.GetValue(c => c.OwnerName);
                                        ownerData.Zip = readerOwnerData.GetValue(c => c.ZIP);
                                        ownerData.Id = readerOwnerData.GetValue(c => c.Id);
                                        listOwnerData.Add(ownerData);
                                    }
                                    return true;
                                });
                                listOwnerDataTemp.Clear();
                            }
                        }

                        if (listOwnerDataTemp.Count > 0)
                        {
                            var builderOwnerDataAnother = this._dataLayer.GetBuilder<MD.IOwnerData>().From();
                            builderOwnerDataAnother.Select(c => c.Address);
                            builderOwnerDataAnother.Select(c => c.CODE);
                            builderOwnerDataAnother.Select(c => c.Id);
                            builderOwnerDataAnother.Select(c => c.OKPO);
                            builderOwnerDataAnother.Select(c => c.OwnerName);
                            builderOwnerDataAnother.Select(c => c.ZIP);
                            builderOwnerDataAnother.Where(c => c.Id, ConditionOperator.In, listOwnerDataTemp.ToArray());
                            builderOwnerDataAnother.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderOwnerDataAnother, readerOwnerData =>
                            {
                                while (readerOwnerData.Read())
                                {
                                    var ownerData = new OwnerData();
                                    ownerData.Addres = readerOwnerData.GetValue(c => c.Address);
                                    ownerData.Code = readerOwnerData.GetValue(c => c.CODE);
                                    ownerData.OKPO = readerOwnerData.GetValue(c => c.OKPO);
                                    ownerData.OwnerName = readerOwnerData.GetValue(c => c.OwnerName);
                                    ownerData.Zip = readerOwnerData.GetValue(c => c.ZIP);
                                    ownerData.Id = readerOwnerData.GetValue(c => c.Id);
                                    listOwnerData.Add(ownerData);
                                }
                                return true;
                            });
                        }
                    }
                }


                var listSitesDataTemp = new List<long>();
                var idSites = idStations.ToList().Select(c => c.Value.IdSite);
                var listSiteStationForMeas = new List<SiteStationForMeas>();
                if ((idSites != null))
                {
                    var arrIds = idSites.ToArray();
                    if (arrIds.Length > 0)
                    {
                        for (int i = 0; i < arrIds.Length; i++)
                        {
                            listSitesDataTemp.Add(arrIds[i]);
                            if (listSitesDataTemp.Count == 1000)
                            {

                                var builderStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().From();
                                builderStationSite.Select(c => c.Address);
                                builderStationSite.Select(c => c.Id);
                                builderStationSite.Select(c => c.Lat);
                                builderStationSite.Select(c => c.Lon);
                                builderStationSite.Select(c => c.Region);
                                builderStationSite.Where(c => c.Id, ConditionOperator.In, listSitesDataTemp.ToArray());
                                builderStationSite.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderStationSite, readerStationSite =>
                                {
                                    while (readerStationSite.Read())
                                    {
                                        var siteStationForMeas = new SiteStationForMeas();
                                        siteStationForMeas.Adress = readerStationSite.GetValue(c => c.Address);
                                        siteStationForMeas.Lat = readerStationSite.GetValue(c => c.Lat);
                                        siteStationForMeas.Lon = readerStationSite.GetValue(c => c.Lon);
                                        siteStationForMeas.Region = readerStationSite.GetValue(c => c.Region);
                                        siteStationForMeas.Id = readerStationSite.GetValue(c => c.Id);
                                        listSiteStationForMeas.Add(siteStationForMeas);
                                    }
                                    return true;
                                });
                                listSitesDataTemp.Clear();
                            }
                        }
                    }
                }

                if (listSitesDataTemp.Count > 0)
                {
                    var builderStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().From();
                    builderStationSite.Select(c => c.Address);
                    builderStationSite.Select(c => c.Id);
                    builderStationSite.Select(c => c.Lat);
                    builderStationSite.Select(c => c.Lon);
                    builderStationSite.Select(c => c.Region);
                    builderStationSite.Where(c => c.Id, ConditionOperator.In, listSitesDataTemp.ToArray());
                    builderStationSite.OrderByAsc(c => c.Id);
                    queryExecuter.Fetch(builderStationSite, readerStationSite =>
                    {
                        while (readerStationSite.Read())
                        {
                            var siteStationForMeas = new SiteStationForMeas();
                            siteStationForMeas.Adress = readerStationSite.GetValue(c => c.Address);
                            siteStationForMeas.Lat = readerStationSite.GetValue(c => c.Lat);
                            siteStationForMeas.Lon = readerStationSite.GetValue(c => c.Lon);
                            siteStationForMeas.Region = readerStationSite.GetValue(c => c.Region);
                            siteStationForMeas.Id = readerStationSite.GetValue(c => c.Id);
                            listSiteStationForMeas.Add(siteStationForMeas);
                        }
                        return true;
                    });
                }

                foreach (var item in idStations)
                {

                    var findIdOwner = listOwnerData.Find(c => c.Id == item.Value.IdOwner);
                    if (findIdOwner != null)
                    {
                        item.Value.Owner = findIdOwner;
                    }

                    var findIdSite = listSiteStationForMeas.Find(c => c.Id == item.Value.IdSite);
                    if (findIdSite != null)
                    {
                        item.Value.Site = findIdSite;
                    }

                    listStationData.Add(item.Value);
                }

            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listStationData.ToArray();
        }

    }
}




