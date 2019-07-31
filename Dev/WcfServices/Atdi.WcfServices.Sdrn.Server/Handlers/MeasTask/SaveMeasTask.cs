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
    public class SaveMeasTask 
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;

        public SaveMeasTask(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }



        /// <summary>
        /// Update status MeasTask
        /// </summary>
        /// <param name="measTask"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool SetStatusTasksInDB(MeasTask measTask, string status)
        {
            bool isSuccess = true;
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var builderSelectMeasTask = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().From();
                    builderSelectMeasTask.Select(c => c.SUBTASK.MEAS_TASK.Id);
                    builderSelectMeasTask.Select(c => c.SUBTASK.Id);
                    builderSelectMeasTask.Select(c => c.Id);
                    builderSelectMeasTask.Where(c => c.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, measTask.Id.Value);
                    scope.Executor.Fetch(builderSelectMeasTask, reader =>
                    {
                        while (reader.Read())
                        {

                            var builderUpdateMeasSubTaskStaSave = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().Update();
                            builderUpdateMeasSubTaskStaSave.Where(c => c.Id, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                            builderUpdateMeasSubTaskStaSave.SetValue(c => c.Status, status);
                            if (scope.Executor.Execute(builderUpdateMeasSubTaskStaSave) > 0)
                            {
                                isSuccess = true;
                            }
                            else
                            {
                                isSuccess = false;
                            }

                            var builderSelectMeasSubTask = this._dataLayer.GetBuilder<MD.ISubTask>().Update();
                            builderSelectMeasSubTask.Where(c => c.Id, ConditionOperator.Equal, reader.GetValue(c => c.SUBTASK.Id));
                            builderSelectMeasSubTask.SetValue(c => c.Status, status);
                            if (scope.Executor.Execute(builderSelectMeasSubTask) > 0)
                            {
                                isSuccess = true;
                            }
                            else
                            {
                                isSuccess = false;
                            }

                            var builderMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().Update();
                            builderMeasTask.Where(c => c.Id, ConditionOperator.Equal, reader.GetValue(c => c.SUBTASK.MEAS_TASK.Id));
                            builderMeasTask.SetValue(c => c.Status, status);
                            if (scope.Executor.Execute(builderMeasTask) > 0)
                            {
                                isSuccess = true;
                            }
                            else
                            {
                                isSuccess = false;
                            }
                            
                        }
                        return true;
                    });
                    scope.Commit();
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
                isSuccess = false;
            }
            return isSuccess;
        }



        public long? SaveMeasTaskInDB(MeasTask value)
        {
            long? ID = null;
            //var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            if (value.Id != null)
            {
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        scope.BeginTran();

                        var builderInsertMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().Insert();
                        builderInsertMeasTask.SetValue(c => c.CreatedBy, value.CreatedBy);
                        builderInsertMeasTask.SetValue(c => c.DateCreated, value.DateCreated);
                        builderInsertMeasTask.SetValue(c => c.ExecutionMode, value.ExecutionMode.ToString());
                        builderInsertMeasTask.SetValue(c => c.MaxTimeBs, value.MaxTimeBs);
                        builderInsertMeasTask.SetValue(c => c.Name, value.Name);
                        builderInsertMeasTask.SetValue(c => c.OrderId, value.OrderId);
                        builderInsertMeasTask.SetValue(c => c.Prio, value.Prio);
                        builderInsertMeasTask.SetValue(c => c.ResultType, value.ResultType.ToString());
                        builderInsertMeasTask.SetValue(c => c.Status, value.Status);
                        builderInsertMeasTask.SetValue(c => c.Task, value.Task.ToString());
                        builderInsertMeasTask.SetValue(c => c.Type, value.Type);
                        if (value.MeasTimeParamList != null)
                        {
                            builderInsertMeasTask.SetValue(c => c.PerStart, value.MeasTimeParamList.PerStart);
                            builderInsertMeasTask.SetValue(c => c.PerStop, value.MeasTimeParamList.PerStop);
                            builderInsertMeasTask.SetValue(c => c.TimeStart, value.MeasTimeParamList.TimeStart);
                            builderInsertMeasTask.SetValue(c => c.TimeStop, value.MeasTimeParamList.TimeStop);
                            builderInsertMeasTask.SetValue(c => c.PerInterval, value.MeasTimeParamList.PerInterval);
                        }
                        
                        var measTaskPK = scope.Executor.Execute<MD.IMeasTask_PK>(builderInsertMeasTask);
                        ID = measTaskPK.Id;
                        value.Id.Value = ID.Value;


                        if (ID != null)
                        {
                            if (value.SignalingMeasTaskParameters != null)
                            {
                                long valueIdMeasTaskSignaling = -1;
                                var builderInsertMeasTaskSignaling = this._dataLayer.GetBuilder<MD.IMeasTaskSignaling>().Insert();
                                if (value.SignalingMeasTaskParameters.allowableExcess_dB != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.allowableExcess_dB, value.SignalingMeasTaskParameters.allowableExcess_dB);
                                }
                                if (value.SignalingMeasTaskParameters.AnalyzeByChannel != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.AnalyzeByChannel, value.SignalingMeasTaskParameters.AnalyzeByChannel);
                                }
                                if (value.SignalingMeasTaskParameters.AnalyzeSysInfoEmission != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.AnalyzeSysInfoEmission, value.SignalingMeasTaskParameters.AnalyzeSysInfoEmission);
                                }
                                if (value.SignalingMeasTaskParameters.CheckFreqChannel != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.CheckFreqChannel, value.SignalingMeasTaskParameters.CheckFreqChannel);
                                }
                                if (value.SignalingMeasTaskParameters.CorrelationAnalize != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.CorrelationAnalize, value.SignalingMeasTaskParameters.CorrelationAnalize);
                                }
                                if (value.SignalingMeasTaskParameters.CorrelationFactor != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.CorrelationFactor, value.SignalingMeasTaskParameters.CorrelationFactor);
                                }
                                if (value.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.DetailedMeasurementsBWEmission, value.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission);
                                }
                                if (value.SignalingMeasTaskParameters.Standard != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.Standard, value.SignalingMeasTaskParameters.Standard);
                                }
                                if (value.SignalingMeasTaskParameters.triggerLevel_dBm_Hz != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.TriggerLevel_dBm_Hz, value.SignalingMeasTaskParameters.triggerLevel_dBm_Hz);
                                }

                                if (value.SignalingMeasTaskParameters.GroupingParameters != null)
                                {
                                    if (value.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.CrossingBWPercentageForBadSignals, value.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals);
                                    }
                                    if (value.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.CrossingBWPercentageForGoodSignals, value.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals);
                                    }
                                    if (value.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.TimeBetweenWorkTimes_sec, value.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec);
                                    }
                                    if (value.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.TypeJoinSpectrum, value.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum);
                                    }
                                }

                                if (value.SignalingMeasTaskParameters.InterruptionParameters != null)
                                {
                                    if (value.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.AutoDivisionEmitting, value.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting);
                                    }
                                    if (value.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.DifferenceMaxMax, value.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax);
                                    }

                                    if (value.SignalingMeasTaskParameters.InterruptionParameters.allowableExcess_dB != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.InterruptAllowableExcess_dB, value.SignalingMeasTaskParameters.InterruptionParameters.allowableExcess_dB);
                                    }
                                    if (value.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.DiffLevelForCalcBW, value.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW);
                                    }
                                    if (value.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.MinExcessNoseLevel_dB, value.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB);
                                    }
                                    if (value.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.NDbLevel_dB, value.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB);
                                    }
                                    if (value.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.NumberIgnoredPoints, value.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints);
                                    }
                                    if (value.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.NumberPointForChangeExcess, value.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess);
                                    }
                                    if (value.SignalingMeasTaskParameters.InterruptionParameters.windowBW != null)
                                    {
                                        builderInsertMeasTaskSignaling.SetValue(c => c.WindowBW, value.SignalingMeasTaskParameters.InterruptionParameters.windowBW);
                                    }

                                }
                                
                                if (value.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.CompareTraceJustWithRefLevels, value.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels);
                                }
                                if (value.SignalingMeasTaskParameters.FiltrationTrace != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.FiltrationTrace, value.SignalingMeasTaskParameters.FiltrationTrace);
                                }
                                if (value.SignalingMeasTaskParameters.SignalizationNChenal != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.SignalizationNChenal, value.SignalingMeasTaskParameters.SignalizationNChenal);
                                }
                                if (value.SignalingMeasTaskParameters.SignalizationNCount != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.SignalizationNCount, value.SignalingMeasTaskParameters.SignalizationNCount);
                                }


                                builderInsertMeasTaskSignaling.SetValue(c => c.MEAS_TASK.Id, ID);
                                


                                var measTaskSignalingPK = scope.Executor.Execute<MD.IMeasTaskSignaling_PK>(builderInsertMeasTaskSignaling);
                                valueIdMeasTaskSignaling = measTaskSignalingPK.Id;


                                if (value.RefSituation != null)
                                {
                                    for (int l = 0; l < value.RefSituation.Length; l++)
                                    {
                                        long valueIdReferenceSituation = -1;
                                        var refSituationReferenceSignal = value.RefSituation[l];
                                        var builderInsertReferenceSituation = this._dataLayer.GetBuilder<MD.IReferenceSituation>().Insert();
                                        builderInsertReferenceSituation.SetValue(c => c.MEAS_TASK.Id, ID);
                                        builderInsertReferenceSituation.SetValue(c => c.SENSOR.Id, refSituationReferenceSignal.SensorId);
                                        

                                        var referenceSituationRawPK = scope.Executor.Execute<MD.IReferenceSituation_PK>(builderInsertReferenceSituation);
                                        valueIdReferenceSituation = referenceSituationRawPK.Id;

                                        if (valueIdReferenceSituation > 0)
                                        {
                                            for (int j = 0; j < refSituationReferenceSignal.ReferenceSignal.Length; j++)
                                            {
                                                long valueIdReferenceSignal = -1;
                                                var situationReferenceSignal = refSituationReferenceSignal.ReferenceSignal[j];


                                                var builderInsertReferenceSignal = this._dataLayer.GetBuilder<MD.IReferenceSignal>().Insert();
                                                builderInsertReferenceSignal.SetValue(c => c.Bandwidth_kHz, situationReferenceSignal.Bandwidth_kHz);
                                                builderInsertReferenceSignal.SetValue(c => c.Frequency_MHz, situationReferenceSignal.Frequency_MHz);
                                                builderInsertReferenceSignal.SetValue(c => c.LevelSignal_dBm, situationReferenceSignal.LevelSignal_dBm);
                                                builderInsertReferenceSignal.SetValue(c => c.REFERENCE_SITUATION.Id, valueIdReferenceSituation);
                                                builderInsertReferenceSignal.SetValue(c => c.IcsmId, situationReferenceSignal.IcsmId);
                                                builderInsertReferenceSignal.SetValue(c => c.IcsmTable, situationReferenceSignal.IcsmTable);
                                                var signalMask = situationReferenceSignal.SignalMask;
                                                if (signalMask != null)
                                                {
                                                    if (signalMask.Loss_dB != null)
                                                    {
                                                        builderInsertReferenceSignal.SetValue(c => c.Loss_dB, signalMask.Loss_dB);
                                                    }
                                                    if (signalMask.Freq_kHz != null)
                                                    {
                                                        builderInsertReferenceSignal.SetValue(c => c.Freq_kHz, signalMask.Freq_kHz);
                                                    }
                                                }
                                                
                                                var referenceSignalRawPK = scope.Executor.Execute<MD.IReferenceSignal_PK>(builderInsertReferenceSignal);
                                                valueIdReferenceSignal = referenceSignalRawPK.Id;
                                            }
                                        }
                                    }
                                }
                            }


                            if (value.MeasDtParam != null)
                            {
                                long valueIdMeasDtParam = -1;
                                var builderInsertMeasDtParam = this._dataLayer.GetBuilder<MD.IMeasDtParam>().Insert();
                                builderInsertMeasDtParam.SetValue(c => c.Demod, value.MeasDtParam.Demod);
                                builderInsertMeasDtParam.SetValue(c => c.DetectType, value.MeasDtParam.DetectType.ToString());
                                builderInsertMeasDtParam.SetValue(c => c.Ifattenuation, value.MeasDtParam.IfAttenuation);
                                builderInsertMeasDtParam.SetValue(c => c.MeasTime, value.MeasDtParam.MeasTime);
                                builderInsertMeasDtParam.SetValue(c => c.Preamplification, value.MeasDtParam.Preamplification);
                                builderInsertMeasDtParam.SetValue(c => c.Rbw, value.MeasDtParam.RBW);
                                builderInsertMeasDtParam.SetValue(c => c.Rfattenuation, value.MeasDtParam.RfAttenuation);
                                builderInsertMeasDtParam.SetValue(c => c.TypeMeasurements, value.MeasDtParam.TypeMeasurements.ToString());
                                builderInsertMeasDtParam.SetValue(c => c.Vbw, value.MeasDtParam.VBW);
                                builderInsertMeasDtParam.SetValue(c => c.MEAS_TASK.Id, ID.Value);
                                
                                var measDtParamPK = scope.Executor.Execute<MD.IMeasDtParam_PK>(builderInsertMeasDtParam);
                                valueIdMeasDtParam = measDtParamPK.Id;
                            }



                            if (value.MeasLocParams != null)
                            {
                                for (int u = 0; u < value.MeasLocParams.Length; u++)
                                {
                                    var locParam = value.MeasLocParams[u];
                                    if (locParam.Id != null)
                                    {
                                        long valueIdMeasLocationParam = -1;
                                        var builderInsertMeasLocationParam = this._dataLayer.GetBuilder<MD.IMeasLocationParam>().Insert();
                                        builderInsertMeasLocationParam.SetValue(c => c.Asl, locParam.ASL);
                                        builderInsertMeasLocationParam.SetValue(c => c.Lat, locParam.Lat);
                                        builderInsertMeasLocationParam.SetValue(c => c.Lon, locParam.Lon);
                                        builderInsertMeasLocationParam.SetValue(c => c.MaxDist, locParam.MaxDist);
                                        builderInsertMeasLocationParam.SetValue(c => c.MEAS_TASK.Id, ID.Value);
                                        
                                        var measLocationParam_PK = scope.Executor.Execute<MD.IMeasLocationParam_PK>(builderInsertMeasLocationParam);
                                        valueIdMeasLocationParam = measLocationParam_PK.Id;
                                        locParam.Id.Value = valueIdMeasLocationParam;
                                    }
                                }
                            }
                            if (value.MeasOther != null)
                            {
                                var measOther = value.MeasOther;
                                long valueIdMeasOther = -1;
                                var builderInsertMeasOther = this._dataLayer.GetBuilder<MD.IMeasOther>().Insert();
                                builderInsertMeasOther.SetValue(c => c.LevelMinOccup, measOther.LevelMinOccup);
                                builderInsertMeasOther.SetValue(c => c.Nchenal, measOther.NChenal);
                                builderInsertMeasOther.SetValue(c => c.SwNumber, measOther.SwNumber);
                                builderInsertMeasOther.SetValue(c => c.TypeSpectrumOccupation, measOther.TypeSpectrumOccupation.ToString());
                                builderInsertMeasOther.SetValue(c => c.TypeSpectrumscan, measOther.TypeSpectrumScan.ToString());
                                builderInsertMeasOther.SetValue(c => c.MEAS_TASK.Id, ID.Value);
                                

                                var measOtherPK = scope.Executor.Execute<MD.IMeasOther_PK>(builderInsertMeasOther);
                                valueIdMeasOther = measOtherPK.Id;
                            }


                            if (value.MeasSubTasks != null)
                            {
                                for (int u = 0; u < value.MeasSubTasks.Length; u++)
                                {
                                    var measSubTask = value.MeasSubTasks[u];
                                    if (measSubTask.Id != null)
                                    {
                                        long valueIdmeasSubTask = -1;
                                        var builderInsertMeasSubTask = this._dataLayer.GetBuilder<MD.ISubTask>().Insert();
                                        builderInsertMeasSubTask.SetValue(c => c.Interval, measSubTask.Interval);
                                        builderInsertMeasSubTask.SetValue(c => c.Status, measSubTask.Status);
                                        builderInsertMeasSubTask.SetValue(c => c.TimeStart, measSubTask.TimeStart);
                                        builderInsertMeasSubTask.SetValue(c => c.TimeStop, measSubTask.TimeStop);
                                        builderInsertMeasSubTask.SetValue(c => c.MEAS_TASK.Id, ID.Value);
                                        


                                        var measSubTaskPK = scope.Executor.Execute<MD.ISubTask_PK>(builderInsertMeasSubTask);
                                        valueIdmeasSubTask = measSubTaskPK.Id;
                                        measSubTask.Id.Value = valueIdmeasSubTask;


                                        if ((measSubTask.MeasSubTaskStations != null) && (valueIdmeasSubTask > -1))
                                        {
                                            for (int v = 0; v < measSubTask.MeasSubTaskStations.Length; v++)
                                            {
                                                var subTaskStation = measSubTask.MeasSubTaskStations[v];
                                                long valueIdmeasSubTaskSta = -1;
                                                var builderInsertMeasSubTaskSta = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().Insert();
                                                builderInsertMeasSubTaskSta.SetValue(c => c.Count, subTaskStation.Count);
                                                builderInsertMeasSubTaskSta.SetValue(c => c.Status, subTaskStation.Status);
                                                if (subTaskStation.StationId != null)
                                                {
                                                    builderInsertMeasSubTaskSta.SetValue(c => c.SENSOR.Id, subTaskStation.StationId.Value);
                                                }
                                                builderInsertMeasSubTaskSta.SetValue(c => c.SUBTASK.Id, valueIdmeasSubTask);
                                                builderInsertMeasSubTaskSta.SetValue(c => c.TimeNextTask, subTaskStation.TimeNextTask);
                                                
                                                var insertMeasSubTaskStaPK = scope.Executor.Execute<MD.ISubTaskSensor_PK>(builderInsertMeasSubTaskSta);
                                                valueIdmeasSubTaskSta = insertMeasSubTaskStaPK.Id;
                                                subTaskStation.Id = valueIdmeasSubTaskSta;
                                            }
                                        }
                                    }
                                }
                            }


                            if (value.MeasFreqParam != null)
                            {
                                var freq_param = value.MeasFreqParam;
                                {
                                    long? idMeasFreqParam = -1;
                                    if (freq_param != null)
                                    {
                                        var builderInsertMeasFreqParam = this._dataLayer.GetBuilder<MD.IMeasFreqParam>().Insert();
                                        builderInsertMeasFreqParam.SetValue(c => c.Mode, freq_param.Mode.ToString());
                                        builderInsertMeasFreqParam.SetValue(c => c.Rgl, freq_param.RgL);
                                        builderInsertMeasFreqParam.SetValue(c => c.Rgu, freq_param.RgU);
                                        builderInsertMeasFreqParam.SetValue(c => c.Step, freq_param.Step);
                                        builderInsertMeasFreqParam.SetValue(c => c.MEAS_TASK.Id, ID.Value);
                                        
                                        var measFreqParamPK = scope.Executor.Execute<MD.IMeasFreqParam_PK>(builderInsertMeasFreqParam);
                                        idMeasFreqParam = measFreqParamPK.Id;
                                    }


                                    if ((freq_param.MeasFreqs != null) && (idMeasFreqParam > -1))
                                    {

                                        for (int i = 0; i < freq_param.MeasFreqs.Length; i++)
                                        {
                                            var builderInsertResMeasFreq = this._dataLayer.GetBuilder<MD.IMeasFreq>().Insert();
                                            builderInsertResMeasFreq.SetValue(c => c.Freq, freq_param.MeasFreqs[i].Freq);
                                            builderInsertResMeasFreq.SetValue(c => c.MEAS_FREQ_PARAM.Id, idMeasFreqParam);
                                            
                                            var measFreqPK = scope.Executor.Execute<MD.IMeasFreq_PK>(builderInsertResMeasFreq);
                                        }
                                    }
                                }
                            }


                            if (value.Stations != null)
                            {

                                for (int i = 0; i < value.Stations.Length; i++)
                                {
                                    if (value.Stations[i].StationId != null)
                                    {
                                        var builderInsertMeasStation = this._dataLayer.GetBuilder<MD.IMeasStation>().Insert();
                                        builderInsertMeasStation.SetValue(c => c.StationType, value.Stations[i].StationType);
                                        builderInsertMeasStation.SetValue(c => c.ClientStationCode, value.Stations[i].StationId.Value);
                                        builderInsertMeasStation.SetValue(c => c.MEAS_TASK.Id, ID.Value);
                                        


                                        var measStation_PK = scope.Executor.Execute<MD.IMeasStation_PK>(builderInsertMeasStation);

                                    }
                                }
                            }

                            if (value.StationsForMeasurements != null)
                            {
                                for (int v = 0; v < value.StationsForMeasurements.Length; v++)
                                {
                                    var stationDataParam = value.StationsForMeasurements[v];

                                    long? idstationDataParam = -1;
                                    long? idOwnerdata = -1;
                                    long? idSite = -1;

                                    if (stationDataParam.Owner != null)
                                    {
                                        var builderInsertOwnerData = this._dataLayer.GetBuilder<MD.IOwnerData>().Insert();
                                        builderInsertOwnerData.SetValue(c => c.Address, stationDataParam.Owner.Addres);
                                        builderInsertOwnerData.SetValue(c => c.CODE, stationDataParam.Owner.Code);
                                        builderInsertOwnerData.SetValue(c => c.OKPO, stationDataParam.Owner.OKPO);
                                        builderInsertOwnerData.SetValue(c => c.OwnerName, stationDataParam.Owner.OwnerName);
                                        builderInsertOwnerData.SetValue(c => c.ZIP, stationDataParam.Owner.Zip);
                                        
                                        var ownerDataPK = scope.Executor.Execute<MD.IOwnerData_PK>(builderInsertOwnerData);
                                        idOwnerdata = ownerDataPK.Id;
                                    }

                                    if (stationDataParam.Site != null)
                                    {
                                        var builderInsertStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().Insert();
                                        builderInsertStationSite.SetValue(c => c.Address, stationDataParam.Site.Adress);
                                        builderInsertStationSite.SetValue(c => c.Lat, stationDataParam.Site.Lat);
                                        builderInsertStationSite.SetValue(c => c.Lon, stationDataParam.Site.Lon);
                                        builderInsertStationSite.SetValue(c => c.Region, stationDataParam.Site.Region);
                                        
                                        var stationSitePK = scope.Executor.Execute<MD.IStationSite_PK>(builderInsertStationSite);
                                        idSite = stationSitePK.Id;
                                    }

                                    var builderInsertStation = this._dataLayer.GetBuilder<MD.IStation>().Insert();
                                    builderInsertStation.SetValue(c => c.GlobalSID, stationDataParam.GlobalSID);
                                    builderInsertStation.SetValue(c => c.Standart, stationDataParam.Standart);
                                    builderInsertStation.SetValue(c => c.Status, stationDataParam.Status);
                                    builderInsertStation.SetValue(c => c.ClientStationCode, stationDataParam.IdStation);
                                    builderInsertStation.SetValue(c => c.MEAS_TASK.Id, ID.Value);
                                    if (stationDataParam.LicenseParameter != null)
                                    {
                                        builderInsertStation.SetValue(c => c.CloseDate, stationDataParam.LicenseParameter.CloseDate);
                                        builderInsertStation.SetValue(c => c.DozvilName, stationDataParam.LicenseParameter.DozvilName);
                                        builderInsertStation.SetValue(c => c.EndDate, stationDataParam.LicenseParameter.EndDate);
                                        builderInsertStation.SetValue(c => c.StartDate, stationDataParam.LicenseParameter.StartDate);
                                        builderInsertStation.SetValue(c => c.ClientPermissionCode, stationDataParam.LicenseParameter.Id);
                                    }
                                    if (idSite > -1)
                                    {
                                        builderInsertStation.SetValue(c => c.STATION_SITE.Id, idSite);
                                    }
                                    if (idOwnerdata > -1)
                                    {
                                        builderInsertStation.SetValue(c => c.OWNER_DATA.Id, idOwnerdata);
                                    }
                                    
                                    var stationPK = scope.Executor.Execute<MD.IStation_PK>(builderInsertStation);
                                    idstationDataParam = stationPK.Id;

                                    if (idstationDataParam > -1)
                                    {
                                        long? idLinkMeasStation = -1;
                                        var builderInsertLinkMeasStation = this._dataLayer.GetBuilder<MD.ILinkMeasStation>().Insert();
                                        builderInsertLinkMeasStation.SetValue(c => c.STATION.Id, idstationDataParam);
                                        builderInsertLinkMeasStation.SetValue(c => c.MEAS_TASK.Id, ID.Value);
                                        
                                        var linkMeasStationPK = scope.Executor.Execute<MD.ILinkMeasStation_PK>(builderInsertLinkMeasStation);
                                        idLinkMeasStation = linkMeasStationPK.Id;
                                    }

                                    if (stationDataParam.Sectors != null)
                                    {

                                        for (int g = 0; g < stationDataParam.Sectors.Length; g++)
                                        {
                                            var sector = stationDataParam.Sectors[g];

                                            long? idSecForMeas = -1;
                                            //this._logger.Info(Contexts.ThisComponent, $"AGL-{sector.AGL};Azimut-{sector.Azimut};BW-{sector.BW};ClassEmission-{sector.ClassEmission};EIRP-{sector.EIRP};IdSector-{sector.IdSector};idstationDataParam-{idstationDataParam}");
                                            var builderInsertSector = this._dataLayer.GetBuilder<MD.ISector>().Insert();
                                            builderInsertSector.SetValue(c => c.Agl, sector.AGL);
                                            if (sector.Azimut != 1E-99)
                                            {
                                                builderInsertSector.SetValue(c => c.Azimut, sector.Azimut);
                                            }
                                            builderInsertSector.SetValue(c => c.Bw, sector.BW);
                                            builderInsertSector.SetValue(c => c.ClassEmission, sector.ClassEmission);
                                            builderInsertSector.SetValue(c => c.Eirp, sector.EIRP);
                                            builderInsertSector.SetValue(c => c.ClientSectorCode, sector.IdSector);
                                            builderInsertSector.SetValue(c => c.STATION.Id, idstationDataParam);
                                            
                                            var sectorPK = scope.Executor.Execute<MD.ISector_PK>(builderInsertSector);
                                            idSecForMeas = sectorPK.Id;

                                            if (sector.Frequencies != null)
                                            {

                                                for (int d = 0; d < sector.Frequencies.Length; d++)
                                                {
                                                    var freq = sector.Frequencies[d];
                                                    //this._logger.Info(Contexts.ThisComponent, $"ChannalNumber-{freq.ChannalNumber};Frequency-{freq.Frequency};IdPlan-{freq.IdPlan};Id-{freq.Id}");
                                                    long? idSectorFreq = null;
                                                    var builderInsertSectorFreq = this._dataLayer.GetBuilder<MD.ISectorFreq>().Insert();
                                                    builderInsertSectorFreq.SetValue(c => c.ChannelNumber, freq.ChannalNumber);
                                                    builderInsertSectorFreq.SetValue(c => c.Frequency, freq.Frequency);
                                                    builderInsertSectorFreq.SetValue(c => c.ClientPlanCode, freq.IdPlan);
                                                    builderInsertSectorFreq.SetValue(c => c.ClientFreqCode, freq.Id);
                                                    
                                                    var sectorFreq_PKPK = scope.Executor.Execute<MD.ISectorFreq_PK>(builderInsertSectorFreq);
                                                    idSectorFreq = sectorFreq_PKPK.Id;

                                                    if ((idSectorFreq != null) && (idSecForMeas != null))
                                                    {
                                                        var builderInsertLinkSectorFreq = this._dataLayer.GetBuilder<MD.ILinkSectorFreq>().Insert();
                                                        builderInsertLinkSectorFreq.SetValue(c => c.SECTOR_FREQ.Id, idSectorFreq);
                                                        builderInsertLinkSectorFreq.SetValue(c => c.SECTOR.Id, idSecForMeas);
                                                        
                                                        var linkSectorFreqPK = scope.Executor.Execute<MD.ILinkSectorFreq_PK>(builderInsertLinkSectorFreq);
                                                    }
                                                }
                                            }


                                            if (sector.MaskBW != null)
                                            {

                                                for (int d = 0; d < sector.MaskBW.Length; d++)
                                                {
                                                    var maskBw = sector.MaskBW[d];

                                                    long? sectorMaskElemId = -1;
                                                    var builderInsertSectorMaskElement = this._dataLayer.GetBuilder<MD.ISectorMaskElement>().Insert();
                                                    builderInsertSectorMaskElement.SetValue(c => c.Level, maskBw.level);
                                                    builderInsertSectorMaskElement.SetValue(c => c.Bw, maskBw.BW);
                                                    

                                                    var sectorMaskElementPK = scope.Executor.Execute<MD.ISectorMaskElement_PK>(builderInsertSectorMaskElement);
                                                    sectorMaskElemId = sectorMaskElementPK.Id;


                                                    if ((sectorMaskElemId != null) && (idSecForMeas != null))
                                                    {
                                                        var builderInsertLinkSectorMaskElement = this._dataLayer.GetBuilder<MD.ILinkSectorMaskElement>().Insert();
                                                        builderInsertLinkSectorMaskElement.SetValue(c => c.SECTOR_MASK_ELEM.Id, sectorMaskElemId);
                                                        builderInsertLinkSectorMaskElement.SetValue(c => c.SECTOR.Id, idSecForMeas);
                                                        
                                                        var linkSectorMaskElement_PK = scope.Executor.Execute<MD.ILinkSectorMaskElement_PK>(builderInsertLinkSectorMaskElement);
                                                    }
                                                }

                                            }

                                        }
                                    }
                                }
                            }
                        }
                        scope.Commit();
                    }
                }
                catch (Exception e)
                {
                    ID = null;
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return ID;
        }

    }
}


