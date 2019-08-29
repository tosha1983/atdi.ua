using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using System.Xml;
using System.Linq;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    /// <summary>
    /// Загрузка сведений по задачам
    /// </summary>
    public class LoadMeasTask 
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public LoadMeasTask(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public  MeasTaskBandWidth ReadTaskBandWidth(long id, MeasTask task)
        {
            var baseObjectMeasTask = task;
            var measTask = new MeasTaskBandWidth()
            {
                CreatedBy = baseObjectMeasTask.CreatedBy,
                DateCreated = baseObjectMeasTask.DateCreated,
                ExecutionMode = baseObjectMeasTask.ExecutionMode,
                Id = baseObjectMeasTask.Id,
                MeasSubTasks = baseObjectMeasTask.MeasSubTasks,
                MeasTimeParamList = baseObjectMeasTask.MeasTimeParamList,
                Name = baseObjectMeasTask.Name,
                Prio = baseObjectMeasTask.Prio,
                Sensors = baseObjectMeasTask.Sensors,
                Status = baseObjectMeasTask.Status,
                TypeMeasurements = baseObjectMeasTask.TypeMeasurements
            };
            measTask.MeasDtParam = GetMeasDtParam(id);
            measTask.MeasFreqParam = GetMeasFreqParam(id);
            return measTask;
        }


        public  MeasTaskSpectrumOccupation ReadTaskSpectrumOccupation(long id, MeasTask task)
        {
            var baseObjectMeasTask = task;
            var measTask = new MeasTaskSpectrumOccupation()
            {
                CreatedBy = baseObjectMeasTask.CreatedBy,
                DateCreated = baseObjectMeasTask.DateCreated,
                ExecutionMode = baseObjectMeasTask.ExecutionMode,
                Id = baseObjectMeasTask.Id,
                MeasSubTasks = baseObjectMeasTask.MeasSubTasks,
                MeasTimeParamList = baseObjectMeasTask.MeasTimeParamList,
                Name = baseObjectMeasTask.Name,
                Prio = baseObjectMeasTask.Prio,
                Sensors = baseObjectMeasTask.Sensors,
                Status = baseObjectMeasTask.Status,
                TypeMeasurements = baseObjectMeasTask.TypeMeasurements
            };
            measTask.MeasDtParam = GetMeasDtParam(id);
            measTask.MeasFreqParam = GetMeasFreqParam(id);
            measTask.SpectrumOccupationParameters = GetSpectrumOccupationParameters(id);
            return measTask;
        }

        public  MeasTaskLevel ReadTaskLevel(long id, MeasTask task)
        {
            var baseObjectMeasTask = task;
            var measTask = new MeasTaskLevel()
            {
                CreatedBy = baseObjectMeasTask.CreatedBy,
                DateCreated = baseObjectMeasTask.DateCreated,
                ExecutionMode = baseObjectMeasTask.ExecutionMode,
                Id = baseObjectMeasTask.Id,
                MeasSubTasks = baseObjectMeasTask.MeasSubTasks,
                MeasTimeParamList = baseObjectMeasTask.MeasTimeParamList,
                Name = baseObjectMeasTask.Name,
                Prio = baseObjectMeasTask.Prio,
                Sensors = baseObjectMeasTask.Sensors,
                Status = baseObjectMeasTask.Status,
                TypeMeasurements = baseObjectMeasTask.TypeMeasurements
            };

            measTask.MeasDtParam = GetMeasDtParam(id);
            measTask.MeasFreqParam = GetMeasFreqParam(id);
            return measTask;
        }

        public MeasTaskMonitoringStations ReadTaskMonitoringStations(long id, MeasTask task)
        {
            var baseObjectMeasTask = task;
            var measTask = new MeasTaskMonitoringStations()
            {
                CreatedBy = baseObjectMeasTask.CreatedBy,
                DateCreated = baseObjectMeasTask.DateCreated,
                ExecutionMode = baseObjectMeasTask.ExecutionMode,
                Id = baseObjectMeasTask.Id,
                MeasSubTasks = baseObjectMeasTask.MeasSubTasks,
                MeasTimeParamList = baseObjectMeasTask.MeasTimeParamList,
                Name = baseObjectMeasTask.Name,
                Prio = baseObjectMeasTask.Prio,
                Sensors = baseObjectMeasTask.Sensors,
                Status = baseObjectMeasTask.Status,
                TypeMeasurements = baseObjectMeasTask.TypeMeasurements
            };
            measTask.StationsForMeasurements = GetStationDataForMeasurementsByTaskId(id);

            return measTask;
        }

        private MeasTaskSignaling ReadTaskSignaling(long id, MeasTask task)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var baseObjectMeasTask = task;
            var measTaskSignaling = new MeasTaskSignaling()
            {
                CreatedBy = baseObjectMeasTask.CreatedBy,
                DateCreated = baseObjectMeasTask.DateCreated,
                ExecutionMode = baseObjectMeasTask.ExecutionMode,
                Id = baseObjectMeasTask.Id,
                MeasSubTasks = baseObjectMeasTask.MeasSubTasks,
                MeasTimeParamList = baseObjectMeasTask.MeasTimeParamList,
                Name = baseObjectMeasTask.Name,
                Prio = baseObjectMeasTask.Prio,
                Sensors = baseObjectMeasTask.Sensors,
                Status = baseObjectMeasTask.Status,
                TypeMeasurements = baseObjectMeasTask.TypeMeasurements
            };
                     
            measTaskSignaling.SignalingMeasTaskParameters = new SignalingMeasTaskParameters();
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


            builderMeasTaskSignaling.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, id);
            queryExecuter.Fetch(builderMeasTaskSignaling, readerMeasTaskSignaling =>
            {
                var resultMeasTaskSignaling = true;
                while (readerMeasTaskSignaling.Read())
                {
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters = new SignalingInterruptionParameters();
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting = readerMeasTaskSignaling.GetValue(c => c.AutoDivisionEmitting);
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax = readerMeasTaskSignaling.GetValue(c => c.DifferenceMaxMax);
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW = readerMeasTaskSignaling.GetValue(c => c.DiffLevelForCalcBW);
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB = readerMeasTaskSignaling.GetValue(c => c.MinExcessNoseLevel_dB);
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB = readerMeasTaskSignaling.GetValue(c => c.NDbLevel_dB);
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints = readerMeasTaskSignaling.GetValue(c => c.NumberIgnoredPoints);
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess = readerMeasTaskSignaling.GetValue(c => c.NumberPointForChangeExcess);
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.windowBW = readerMeasTaskSignaling.GetValue(c => c.WindowBW);
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation = readerMeasTaskSignaling.GetValue(c => c.MaxFreqDeviation);
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel = readerMeasTaskSignaling.GetValue(c => c.CheckLevelChannel);
                    measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW = readerMeasTaskSignaling.GetValue(c => c.MinPointForDetailBW);
                    measTaskSignaling.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels = readerMeasTaskSignaling.GetValue(c => c.CompareTraceJustWithRefLevels);
                    measTaskSignaling.SignalingMeasTaskParameters.FiltrationTrace = readerMeasTaskSignaling.GetValue(c => c.FiltrationTrace);
                    measTaskSignaling.SignalingMeasTaskParameters.SignalizationNChenal = readerMeasTaskSignaling.GetValue(c => c.SignalizationNChenal);
                    measTaskSignaling.SignalingMeasTaskParameters.SignalizationNCount = readerMeasTaskSignaling.GetValue(c => c.SignalizationNCount);
                    measTaskSignaling.SignalingMeasTaskParameters.AnalyzeByChannel = readerMeasTaskSignaling.GetValue(c => c.AnalyzeByChannel);
                    measTaskSignaling.SignalingMeasTaskParameters.AnalyzeSysInfoEmission = readerMeasTaskSignaling.GetValue(c => c.AnalyzeSysInfoEmission);
                    measTaskSignaling.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission = readerMeasTaskSignaling.GetValue(c => c.DetailedMeasurementsBWEmission);
                    measTaskSignaling.SignalingMeasTaskParameters.Standard = readerMeasTaskSignaling.GetValue(c => c.Standard);
                    measTaskSignaling.SignalingMeasTaskParameters.CorrelationAnalize = readerMeasTaskSignaling.GetValue(c => c.CorrelationAnalize);
                    measTaskSignaling.SignalingMeasTaskParameters.CorrelationFactor = readerMeasTaskSignaling.GetValue(c => c.CorrelationFactor);
                    measTaskSignaling.SignalingMeasTaskParameters.CheckFreqChannel = readerMeasTaskSignaling.GetValue(c => c.CheckFreqChannel);
                    measTaskSignaling.SignalingMeasTaskParameters.triggerLevel_dBm_Hz = readerMeasTaskSignaling.GetValue(c => c.TriggerLevel_dBm_Hz);
                    measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters = new SignalingGroupingParameters();
                    measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals = readerMeasTaskSignaling.GetValue(c => c.CrossingBWPercentageForBadSignals);
                    measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals = readerMeasTaskSignaling.GetValue(c => c.CrossingBWPercentageForGoodSignals);
                    measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec = readerMeasTaskSignaling.GetValue(c => c.TimeBetweenWorkTimes_sec);
                    measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum = readerMeasTaskSignaling.GetValue(c => c.TypeJoinSpectrum);
                }
                return resultMeasTaskSignaling;
            });

            var listReferenceSituation = new List<ReferenceSituation>();
            var builderReferenceSituationRaw = this._dataLayer.GetBuilder<MD.IReferenceSituation>().From();
            builderReferenceSituationRaw.Select(c => c.Id);
            builderReferenceSituationRaw.Select(c => c.SENSOR.Id);
            builderReferenceSituationRaw.Select(c => c.MEAS_TASK.Id);
            builderReferenceSituationRaw.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, id);
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
                measTaskSignaling.RefSituation = listReferenceSituation.ToArray();
            }
            return measTaskSignaling;
        }


        private MeasFreqParam GetMeasFreqParam(long Id)
        {
            // IMeasFreqParam
            var freqParam = new MeasFreqParam();
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

            var builderMeasFreqParam = this._dataLayer.GetBuilder<MD.IMeasFreqParam>().From();
            builderMeasFreqParam.Select(c => c.Id);
            builderMeasFreqParam.Select(c => c.MEAS_TASK.Id);
            builderMeasFreqParam.Select(c => c.Mode);
            builderMeasFreqParam.Select(c => c.Rgl);
            builderMeasFreqParam.Select(c => c.Rgu);
            builderMeasFreqParam.Select(c => c.Step);
            builderMeasFreqParam.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, Id);
            queryExecuter.Fetch(builderMeasFreqParam, readerMeasFreqParam =>
            {
                while (readerMeasFreqParam.Read())
                {
                  
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
                }
                return true;
            });

            return freqParam;
        }

        private MeasDtParam GetMeasDtParam(long Id)
        {
            // IMeasDtParam
            var measDtParam = new MeasDtParam();
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

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
            builderMeasDtParam.Select(c => c.SwNumber);
            builderMeasDtParam.Select(c => c.Rfattenuation);
            builderMeasDtParam.Select(c => c.Vbw);
            builderMeasDtParam.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, Id);
            queryExecuter.Fetch(builderMeasDtParam, readerMeasDtParam =>
            {
                while (readerMeasDtParam.Read())
                {

                    measDtParam.Demod = readerMeasDtParam.GetValue(c => c.Demod);
                    DetectingType detectType;
                    if (Enum.TryParse<DetectingType>(readerMeasDtParam.GetValue(c => c.DetectType), out detectType))
                        measDtParam.DetectType = detectType;

                    measDtParam.IfAttenuation = readerMeasDtParam.GetValue(c => c.Ifattenuation);
                    measDtParam.MeasTime = readerMeasDtParam.GetValue(c => c.MeasTime);
                    MeasurementMode mode;
                    if (Enum.TryParse<MeasurementMode>(readerMeasDtParam.GetValue(c => c.Mode), out mode))
                        measDtParam.Mode = mode;

                    measDtParam.Preamplification = readerMeasDtParam.GetValue(c => c.Preamplification);
                    measDtParam.RBW = readerMeasDtParam.GetValue(c => c.Rbw);
                    measDtParam.RfAttenuation = readerMeasDtParam.GetValue(c => c.Rfattenuation);

                    measDtParam.VBW = readerMeasDtParam.GetValue(c => c.Vbw);
                    measDtParam.SwNumber = readerMeasDtParam.GetValue(c => c.SwNumber);

                }
                return true;
            });

            return measDtParam;
        }

        private SpectrumOccupationParameters GetSpectrumOccupationParameters(long Id)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

            var spectrumOccupationParameters = new SpectrumOccupationParameters();
            var builderMeasOther = this._dataLayer.GetBuilder<MD.IMeasOther>().From();
            builderMeasOther.Select(c => c.Id);
            builderMeasOther.Select(c => c.LevelMinOccup);
            builderMeasOther.Select(c => c.MEAS_TASK.Id);
            builderMeasOther.Select(c => c.Nchenal);
            builderMeasOther.Select(c => c.TypeSpectrumOccupation);
            builderMeasOther.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, Id);
            queryExecuter.Fetch(builderMeasOther, readerMeasOther =>
            {
                while (readerMeasOther.Read())
                {
                    spectrumOccupationParameters.LevelMinOccup = readerMeasOther.GetValue(c => c.LevelMinOccup);
                    spectrumOccupationParameters.NChenal = readerMeasOther.GetValue(c => c.Nchenal);

                    SpectrumOccupationType typeSpectrumOccupation;
                    if (Enum.TryParse<SpectrumOccupationType>(readerMeasOther.GetValue(c => c.TypeSpectrumOccupation), out typeSpectrumOccupation))
                    {
                        spectrumOccupationParameters.TypeSpectrumOccupation = typeSpectrumOccupation;
                    }
                }
                return true;
            });
            return spectrumOccupationParameters;
        }

        public MeasTask ReadTask(long Id)
        {
            var task = ReadBaseTask(Id);
            if (task != null)
            {
                switch (task.TypeMeasurements)
                {
                    case MeasurementType.AmplModulation:
                    case MeasurementType.Bearing:
                    case MeasurementType.FreqModulation:
                    case MeasurementType.Frequency:
                    case MeasurementType.Location:
                    case MeasurementType.Offset:
                    case MeasurementType.PICode:
                    case MeasurementType.Program:
                    case MeasurementType.SoundID:
                    case MeasurementType.SubAudioTone:
                        return task;
                    case MeasurementType.BandwidthMeas:
                        return ReadTaskBandWidth(Id, task);
                    case MeasurementType.Level:
                        return ReadTaskLevel(Id, task);
                    case MeasurementType.MonitoringStations:
                        return ReadTaskMonitoringStations(Id, task);
                    case MeasurementType.Signaling:
                        return ReadTaskSignaling(Id, task);
                    case MeasurementType.SpectrumOccupation:
                        return ReadTaskSpectrumOccupation(Id, task);
                }
            }
            return task;
        }


        public MeasTask ReadBaseTask(long id)
        {
            MeasTask measTask = null;
            try
            {
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
                builderMeasTask.Where(c => c.Status, ConditionOperator.NotEqual, "Z");
                builderMeasTask.Where(c => c.Status, ConditionOperator.IsNotNull);
                queryExecuter.Fetch(builderMeasTask, readerMeasTask =>
                {
                    while (readerMeasTask.Read())
                    {
                        measTask = new MeasTask();

                        MeasurementType typeMeasurements;
                        if (Enum.TryParse<MeasurementType>(readerMeasTask.GetValue(c => c.Type), out typeMeasurements))
                            measTask.TypeMeasurements = typeMeasurements;

                    
                        measTask.CreatedBy = readerMeasTask.GetValue(c => c.CreatedBy);
                        measTask.DateCreated = readerMeasTask.GetValue(c => c.DateCreated);
                        MeasTaskExecutionMode ExecutionMode;
                        if (Enum.TryParse<MeasTaskExecutionMode>(readerMeasTask.GetValue(c => c.ExecutionMode), out ExecutionMode))
                        {
                            measTask.ExecutionMode = ExecutionMode;
                        }
                        measTask.Id = new MeasTaskIdentifier();
                        measTask.Id.Value = readerMeasTask.GetValue(c => c.Id);
                        measTask.Name = readerMeasTask.GetValue(c => c.Name);
                        measTask.Prio = readerMeasTask.GetValue(c => c.Prio);
                        measTask.Status = readerMeasTask.GetValue(c => c.Status);





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
                                        measSubTaskSensor.SensorId = readerMeasSubTaskSta.GetValue(c => c.SENSOR.Id);
                                        measSubTaskSensor.Status = readerMeasSubTaskSta.GetValue(c => c.Status);
                                        measSubTaskSensor.TimeNextTask = readerMeasSubTaskSta.GetValue(c => c.TimeNextTask);
                                        listMeasSubTaskStation.Add(measSubTaskSensor);
                                        measSensors.Add(new MeasSensor()
                                        {
                                            SensorId = new MeasSensorIdentifier()
                                            {
                                                Value = measSubTaskSensor.SensorId
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

        public StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(long taskId)
        {
            var listStationData = new List<StationDataForMeasurements>();
            try
            {
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
                builderStation.Select(c => c.ClientStationCode);
                builderStation.Select(c => c.ClientPermissionCode);
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
                        measStation.IdSite = readerStation.GetValue(c => c.STATION_SITE.Id);
                        measStation.IdOwner = readerStation.GetValue(c => c.OWNER_DATA.Id);

                        var ownerData = new OwnerData();
                        var builderOwnerData = this._dataLayer.GetBuilder<MD.IOwnerData>().From();
                        builderOwnerData.Select(c => c.Address);
                        builderOwnerData.Select(c => c.CODE);
                        builderOwnerData.Select(c => c.Id);
                        builderOwnerData.Select(c => c.OKPO);
                        builderOwnerData.Select(c => c.OwnerName);
                        builderOwnerData.Select(c => c.ZIP);
                        builderOwnerData.Where(c => c.Id, ConditionOperator.Equal, measStation.IdOwner);
                        builderOwnerData.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderOwnerData, readerOwnerData =>
                        {
                            while (readerOwnerData.Read())
                            {
                                ownerData.Addres = readerOwnerData.GetValue(c => c.Address);
                                ownerData.Code = readerOwnerData.GetValue(c => c.CODE);
                                ownerData.OKPO = readerOwnerData.GetValue(c => c.OKPO);
                                ownerData.OwnerName = readerOwnerData.GetValue(c => c.OwnerName);
                                ownerData.Zip = readerOwnerData.GetValue(c => c.ZIP);
                                ownerData.Id = readerOwnerData.GetValue(c => c.Id);
                            }
                            return true;
                        });

                        measStation.Owner = ownerData;


                        var siteStationForMeas = new SiteStationForMeas();
                        var builderStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().From();
                        builderStationSite.Select(c => c.Address);
                        builderStationSite.Select(c => c.Id);
                        builderStationSite.Select(c => c.Lat);
                        builderStationSite.Select(c => c.Lon);
                        builderStationSite.Select(c => c.Region);
                        builderStationSite.Where(c => c.Id, ConditionOperator.Equal, measStation.IdSite);
                        builderStationSite.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderStationSite, readerStationSite =>
                        {
                            while (readerStationSite.Read())
                            {
                                siteStationForMeas.Adress = readerStationSite.GetValue(c => c.Address);
                                siteStationForMeas.Lat = readerStationSite.GetValue(c => c.Lat);
                                siteStationForMeas.Lon = readerStationSite.GetValue(c => c.Lon);
                                siteStationForMeas.Region = readerStationSite.GetValue(c => c.Region);
                            }
                            return true;
                        });

                        measStation.Site = siteStationForMeas;


                        List<SectorStationForMeas> listSector = new List<SectorStationForMeas>();
                        var builderISector = this._dataLayer.GetBuilder<MD.ISector>().From();
                        builderISector.Select(c => c.Agl);
                        builderISector.Select(c => c.Azimut);
                        builderISector.Select(c => c.Bw);
                        builderISector.Select(c => c.ClassEmission);
                        builderISector.Select(c => c.Eirp);
                        builderISector.Select(c => c.Id);
                        builderISector.Select(c => c.ClientSectorCode);
                        builderISector.Select(c => c.STATION.Id);
                        builderISector.Where(c => c.STATION.Id, ConditionOperator.Equal, readerStation.GetValue(c => c.Id));
                        builderISector.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderISector, readerSector =>
                        {
                            while (readerSector.Read())
                            {
                                var sectM = new SectorStationForMeas();
                                sectM.AGL = readerSector.GetValue(c => c.Agl);
                                sectM.Azimut = readerSector.GetValue(c => c.Azimut);
                                sectM.BW = readerSector.GetValue(c => c.Bw);
                                sectM.ClassEmission = readerSector.GetValue(c => c.ClassEmission);
                                sectM.EIRP = readerSector.GetValue(c => c.Eirp);
                                sectM.IdSector = readerSector.GetValue(c => c.ClientSectorCode).HasValue ? readerSector.GetValue(c => c.ClientSectorCode).Value : -1;



                                var lFreqICSM = new List<FrequencyForSectorFormICSM>();
                                var builderLinkSectorFreq = this._dataLayer.GetBuilder<MD.ILinkSectorFreq>().From();
                                builderLinkSectorFreq.Select(c => c.Id);
                                builderLinkSectorFreq.Select(c => c.SECTOR_FREQ.ChannelNumber);
                                builderLinkSectorFreq.Select(c => c.SECTOR_FREQ.Frequency);
                                builderLinkSectorFreq.Select(c => c.SECTOR_FREQ.Id);
                                builderLinkSectorFreq.Select(c => c.SECTOR_FREQ.ClientPlanCode);
                                builderLinkSectorFreq.Select(c => c.SECTOR_FREQ.ClientFreqCode);
                                builderLinkSectorFreq.Where(c => c.SECTOR.Id, ConditionOperator.Equal, readerSector.GetValue(c => c.Id));
                                builderLinkSectorFreq.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderLinkSectorFreq, readerLinkSectorFreq =>
                                {
                                    while (readerLinkSectorFreq.Read())
                                    {
                                        var freqM = new FrequencyForSectorFormICSM();
                                        freqM.ChannalNumber = readerLinkSectorFreq.GetValue(x => x.SECTOR_FREQ.ChannelNumber);
                                        freqM.Frequency = readerLinkSectorFreq.GetValue(x => x.SECTOR_FREQ.Frequency);
                                        freqM.Id = readerLinkSectorFreq.GetValue(x => x.SECTOR_FREQ.ClientFreqCode);
                                        freqM.IdPlan = readerLinkSectorFreq.GetValue(x => x.SECTOR_FREQ.ClientPlanCode);
                                        lFreqICSM.Add(freqM);
                                    }
                                    return true;
                                });

                                sectM.Frequencies = lFreqICSM.ToArray();

                                var lMask = new List<MaskElements>();
                                var builderLinkSectorMaskElement = this._dataLayer.GetBuilder<MD.ILinkSectorMaskElement>().From();
                                builderLinkSectorMaskElement.Select(c => c.Id);
                                builderLinkSectorMaskElement.Select(c => c.SECTOR_MASK_ELEM.Bw);
                                builderLinkSectorMaskElement.Select(c => c.SECTOR_MASK_ELEM.Id);
                                builderLinkSectorMaskElement.Select(c => c.SECTOR_MASK_ELEM.Level);
                                builderLinkSectorMaskElement.Where(c => c.SECTOR.Id, ConditionOperator.Equal, readerSector.GetValue(c => c.Id));
                                builderLinkSectorMaskElement.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderLinkSectorMaskElement, readerLinkSectorMaskElement =>
                                {
                                    while (readerLinkSectorMaskElement.Read())
                                    {
                                        MaskElements maskElementsM = new MaskElements();
                                        maskElementsM.BW = readerLinkSectorMaskElement.GetValue(c => c.SECTOR_MASK_ELEM.Bw);
                                        maskElementsM.level = readerLinkSectorMaskElement.GetValue(c => c.SECTOR_MASK_ELEM.Level);
                                        lMask.Add(maskElementsM);
                                    }
                                    return true;
                                });
                                sectM.MaskBW = lMask.ToArray();
                                listSector.Add(sectM);
                            }
                            return true;
                        });

                        measStation.Sectors = listSector.ToArray();
                        listStationData.Add(measStation);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listStationData.ToArray();
        }

    }
}




