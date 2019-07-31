using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;




namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{
    public class LoadMeasTask
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public LoadMeasTask(IDataLayer<EntityDataOrm> dataLayer,  ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }


        public MeasTask ReadTask(long id)
        {
            var measTask = new MeasTask();
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
                        builderMeasTaskSignaling.Select(c => c.InterruptAllowableExcess_dB);
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
                        


                        builderMeasTaskSignaling.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasTaskSignaling, readerMeasTaskSignaling =>
                        {
                            var resultMeasTaskSignaling = true;
                            while (readerMeasTaskSignaling.Read())
                            {
                                measTask.SignalingMeasTaskParameters.InterruptionParameters = new SignalingInterruptionParameters();
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting = readerMeasTaskSignaling.GetValue(c => c.AutoDivisionEmitting);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax = readerMeasTaskSignaling.GetValue(c => c.DifferenceMaxMax);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.allowableExcess_dB = readerMeasTaskSignaling.GetValue(c => c.allowableExcess_dB);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW = readerMeasTaskSignaling.GetValue(c => c.DiffLevelForCalcBW);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB = readerMeasTaskSignaling.GetValue(c => c.MinExcessNoseLevel_dB);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB = readerMeasTaskSignaling.GetValue(c => c.NDbLevel_dB);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints = readerMeasTaskSignaling.GetValue(c => c.NumberIgnoredPoints);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess = readerMeasTaskSignaling.GetValue(c => c.NumberPointForChangeExcess);
                                measTask.SignalingMeasTaskParameters.InterruptionParameters.windowBW = readerMeasTaskSignaling.GetValue(c => c.WindowBW);
                                measTask.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels = readerMeasTaskSignaling.GetValue(c => c.CompareTraceJustWithRefLevels);
                                measTask.SignalingMeasTaskParameters.FiltrationTrace = readerMeasTaskSignaling.GetValue(c => c.FiltrationTrace);
                                measTask.SignalingMeasTaskParameters.SignalizationNChenal = readerMeasTaskSignaling.GetValue(c => c.SignalizationNChenal);
                                measTask.SignalingMeasTaskParameters.SignalizationNCount = readerMeasTaskSignaling.GetValue(c => c.SignalizationNCount);
                                measTask.SignalingMeasTaskParameters.AnalyzeByChannel = readerMeasTaskSignaling.GetValue(c => c.AnalyzeByChannel);
                                measTask.SignalingMeasTaskParameters.AnalyzeSysInfoEmission = readerMeasTaskSignaling.GetValue(c => c.AnalyzeSysInfoEmission);
                                measTask.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission = readerMeasTaskSignaling.GetValue(c => c.DetailedMeasurementsBWEmission);
                                measTask.SignalingMeasTaskParameters.Standard = readerMeasTaskSignaling.GetValue(c => c.Standard);
                                measTask.SignalingMeasTaskParameters.CorrelationAnalize= readerMeasTaskSignaling.GetValue(c => c.CorrelationAnalize);
                                measTask.SignalingMeasTaskParameters.CorrelationFactor = readerMeasTaskSignaling.GetValue(c => c.CorrelationFactor);
                                measTask.SignalingMeasTaskParameters.CheckFreqChannel = readerMeasTaskSignaling.GetValue(c => c.CheckFreqChannel);
                                measTask.SignalingMeasTaskParameters.triggerLevel_dBm_Hz = readerMeasTaskSignaling.GetValue(c => c.TriggerLevel_dBm_Hz);
                                measTask.SignalingMeasTaskParameters.GroupingParameters = new SignalingGroupingParameters(); 
                                measTask.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals = readerMeasTaskSignaling.GetValue(c => c.CrossingBWPercentageForBadSignals);
                                measTask.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals = readerMeasTaskSignaling.GetValue(c => c.CrossingBWPercentageForGoodSignals);
                                measTask.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec = readerMeasTaskSignaling.GetValue(c => c.TimeBetweenWorkTimes_sec);
                                measTask.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum = readerMeasTaskSignaling.GetValue(c => c.TypeJoinSpectrum);
                                measTask.SignalingMeasTaskParameters.allowableExcess_dB = readerMeasTaskSignaling.GetValue(c => c.InterruptAllowableExcess_dB);
                            }
                            return resultMeasTaskSignaling;
                        });

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

                        // IMeasStation

                        var measStations = new List<MeasStation>();
                        var builderMeasstation = this._dataLayer.GetBuilder<MD.IMeasStation>().From();
                        builderMeasstation.Select(c => c.Id);
                        builderMeasstation.Select(c => c.MEAS_TASK.Id);
                        builderMeasstation.Select(c => c.ClientStationCode);
                        builderMeasstation.Select(c => c.StationType);
                        builderMeasstation.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasstation, readerMeasStation =>
                        {
                            while (readerMeasStation.Read())
                            {
                                var measStation = new MeasStation();
                                measStation.StationId = new MeasStationIdentifier();
                                measStation.StationId.Value = readerMeasStation.GetValue(c => c.ClientStationCode).Value;
                                measStation.StationType = readerMeasStation.GetValue(c => c.StationType);
                                measStations.Add(measStation);
                            }
                            return true;
                        });
                        measTask.Stations = measStations.ToArray();

                        // IMeasDtParam

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
                        builderMeasDtParam.Select(c => c.TypeMeasurements);
                        builderMeasDtParam.Select(c => c.Vbw);
                        builderMeasDtParam.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasDtParam, readerMeasDtParam =>
                        {
                            while (readerMeasDtParam.Read())
                            {
                                var dtx = new MeasDtParam();
                                dtx.Demod = readerMeasDtParam.GetValue(c => c.Demod);
                                DetectingType detectType;
                                if (Enum.TryParse<DetectingType>(readerMeasDtParam.GetValue(c => c.DetectType), out detectType))
                                    dtx.DetectType = detectType;

                                dtx.IfAttenuation = readerMeasDtParam.GetValue(c => c.Ifattenuation).HasValue ? readerMeasDtParam.GetValue(c => c.Ifattenuation).Value : 0;
                                dtx.MeasTime = readerMeasDtParam.GetValue(c => c.MeasTime);
                                MeasurementMode mode;
                                if (Enum.TryParse<MeasurementMode>(readerMeasDtParam.GetValue(c => c.Mode), out mode))
                                    dtx.Mode = mode;

                                dtx.Preamplification = readerMeasDtParam.GetValue(c => c.Preamplification).HasValue ? readerMeasDtParam.GetValue(c => c.Preamplification).Value : -1;
                                dtx.RBW = readerMeasDtParam.GetValue(c => c.Rbw);
                                dtx.RfAttenuation = readerMeasDtParam.GetValue(c => c.Rfattenuation).HasValue ? readerMeasDtParam.GetValue(c => c.Rfattenuation).Value : 0;
                                MeasurementType typeMeasurements;
                                if (Enum.TryParse<MeasurementType>(readerMeasDtParam.GetValue(c => c.TypeMeasurements), out typeMeasurements))
                                    dtx.TypeMeasurements = typeMeasurements;
                                dtx.VBW = readerMeasDtParam.GetValue(c => c.Vbw);
                                measTask.MeasDtParam = dtx;

                            }
                            return true;
                        });
                        measTask.Stations = measStations.ToArray();


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
                        builderMeasOther.Select(c => c.MEAS_TASK.Id);
                        builderMeasOther.Select(c => c.Nchenal);
                        builderMeasOther.Select(c => c.SwNumber);
                        builderMeasOther.Select(c => c.TypeSpectrumOccupation);
                        builderMeasOther.Select(c => c.TypeSpectrumscan);
                        builderMeasOther.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasOther, readerMeasOther =>
                        {
                            while (readerMeasOther.Read())
                            {
                                measOther.LevelMinOccup = readerMeasOther.GetValue(c => c.LevelMinOccup);
                                measOther.NChenal = readerMeasOther.GetValue(c => c.Nchenal);
                                measOther.SwNumber = readerMeasOther.GetValue(c => c.SwNumber);

                                SpectrumOccupationType typeSpectrumOccupation;
                                if (Enum.TryParse<SpectrumOccupationType>(readerMeasOther.GetValue(c => c.TypeSpectrumOccupation), out typeSpectrumOccupation))
                                {
                                    measOther.TypeSpectrumOccupation = typeSpectrumOccupation;
                                }

                                SpectrumScanType typeSpectrumscan;
                                if (Enum.TryParse<SpectrumScanType>(readerMeasOther.GetValue(c => c.TypeSpectrumscan), out typeSpectrumscan))
                                {
                                    measOther.TypeSpectrumScan = typeSpectrumscan;
                                }

                            }
                            return true;
                        });
                        measTask.MeasOther = measOther;


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
                                var listMeasSubTaskStation = new List<MeasSubTaskStation>();
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
                                        var measSubTaskStation = new MeasSubTaskStation();
                                        measSubTaskStation.Count = readerMeasSubTaskSta.GetValue(c => c.Count);
                                        measSubTaskStation.Id = readerMeasSubTaskSta.GetValue(c => c.Id);
                                        measSubTaskStation.StationId = new SensorIdentifier();
                                        measSubTaskStation.StationId.Value = readerMeasSubTaskSta.GetValue(c => c.SENSOR.Id);
                                        measSubTaskStation.Status = readerMeasSubTaskSta.GetValue(c => c.Status);
                                        measSubTaskStation.TimeNextTask = readerMeasSubTaskSta.GetValue(c => c.TimeNextTask);
                                        listMeasSubTaskStation.Add(measSubTaskStation);
                                    }
                                    return true;
                                });
                                measSubTask.MeasSubTaskStations = listMeasSubTaskStation.ToArray();
                                listmeasSubTask.Add(measSubTask);
                            }
                            return true;
                        });
                        measTask.MeasSubTasks = listmeasSubTask.ToArray();
                        measTask.StationsForMeasurements = GetStationDataForMeasurementsByTaskId(id);
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
                        measStation.IdSite =  readerStation.GetValue(c => c.STATION_SITE.Id);
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

