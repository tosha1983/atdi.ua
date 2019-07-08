﻿using System.Collections.Generic;
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
            //var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var builderSelectMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().From();
                    builderSelectMeasTask.Select(c => c.Id);
                    builderSelectMeasTask.Where(c => c.Id, ConditionOperator.Equal, measTask.Id.Value);
                    builderSelectMeasTask.Where(c => c.Status, ConditionOperator.IsNotNull);
                    scope.Executor.Fetch(builderSelectMeasTask, reader =>
                    {
                        while (reader.Read())
                        {
                            var builderSelectMeasSubTask = this._dataLayer.GetBuilder<MD.IMeasSubTask>().From();
                            builderSelectMeasSubTask.Select(c => c.Id);
                            builderSelectMeasSubTask.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, measTask.Id.Value);
                            scope.Executor.Fetch(builderSelectMeasSubTask, readereasSubTask =>
                            {
                                while (readereasSubTask.Read())
                                {
                                    var id = readereasSubTask.GetValue(c => c.Id);
                                    if (measTask.MeasSubTasks != null)
                                    {
                                        var msSubTask = measTask.MeasSubTasks.ToList().Find(t => t.Id.Value == id);
                                        if (msSubTask != null)
                                        {
                                            var builderSelectMeasSubTaskSta = this._dataLayer.GetBuilder<MD.IMeasSubTaskStation>().From();
                                            builderSelectMeasSubTaskSta.Select(c => c.Id);
                                            builderSelectMeasSubTaskSta.Where(c => c.MEAS_SUBTASK.Id, ConditionOperator.Equal, readereasSubTask.GetValue(c => c.Id));
                                            builderSelectMeasSubTaskSta.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                                            scope.Executor.Fetch(builderSelectMeasSubTaskSta, readereasSubTaskSta =>
                                            {
                                                while (readereasSubTaskSta.Read())
                                                {
                                                    var msSubTaskSta = msSubTask.MeasSubTaskStations.ToList().Find(t => t.Id == readereasSubTaskSta.GetValue(c => c.Id));
                                                    if (msSubTaskSta != null)
                                                    {
                                                        if (msSubTaskSta.TimeNextTask.GetValueOrDefault().Subtract(DateTime.Now).TotalSeconds < 0)
                                                        {
                                                            var builderUpdateMeasSubTaskStaSave = this._dataLayer.GetBuilder<MD.IMeasSubTaskStation>().Update();
                                                            builderUpdateMeasSubTaskStaSave.Where(c => c.Id, ConditionOperator.Equal, readereasSubTaskSta.GetValue(c => c.Id));
                                                            builderUpdateMeasSubTaskStaSave.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                                                            builderUpdateMeasSubTaskStaSave.SetValue(c => c.Status, status);
                                                            if (scope.Executor.Execute(builderUpdateMeasSubTaskStaSave) > 0)
                                                            {
                                                                isSuccess = true;
                                                            }
                                                            else
                                                            {
                                                                isSuccess = false;
                                                            }
                                                           
                                                        }
                                                    }
                                                }
                                                return true;
                                            });


                                            var builderUpdateSubTask = this._dataLayer.GetBuilder<MD.IMeasSubTask>().Update();
                                            builderUpdateSubTask.Where(c => c.Id, ConditionOperator.Equal, readereasSubTask.GetValue(c => c.Id));
                                            builderUpdateSubTask.SetValue(c => c.Status, status);
                                            if (scope.Executor.Execute(builderUpdateSubTask) > 0)
                                            {
                                                isSuccess = true;
                                            }
                                            else
                                            {
                                                isSuccess = false;
                                            }

                                        }
                                    }

                                }
                                return true;
                            });


                            var builderUpdateTask = this._dataLayer.GetBuilder<MD.IMeasTask>().Update();
                            builderUpdateTask.Where(c => c.Id, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                            builderUpdateTask.SetValue(c => c.Status, status);
                            if (scope.Executor.Execute(builderUpdateTask) > 0)
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
                        builderInsertMeasTask.Select(c => c.Id);
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
                                if (value.SignalingMeasTaskParameters.AutoDivisionEmitting != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.AutoDivisionEmitting, value.SignalingMeasTaskParameters.AutoDivisionEmitting == true ? 1 : 0);
                                }
                                if (value.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.CompareTraceJustWithRefLevels, value.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels == true ? 1 : 0);
                                }
                                if (value.SignalingMeasTaskParameters.DifferenceMaxMax != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.DifferenceMaxMax, value.SignalingMeasTaskParameters.DifferenceMaxMax);
                                }
                                if (value.SignalingMeasTaskParameters.FiltrationTrace != null)
                                {
                                    builderInsertMeasTaskSignaling.SetValue(c => c.FiltrationTrace, value.SignalingMeasTaskParameters.FiltrationTrace == true ? 1 : 0);
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
                                builderInsertMeasTaskSignaling.Select(c => c.Id);


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
                                        builderInsertReferenceSituation.Select(c => c.Id);

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
                                                builderInsertReferenceSignal.Select(c => c.Id);
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
                                builderInsertMeasDtParam.Select(c => c.Id);
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
                                        builderInsertMeasLocationParam.Select(c => c.Id);
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
                                builderInsertMeasOther.Select(c => c.Id);

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
                                        var builderInsertMeasSubTask = this._dataLayer.GetBuilder<MD.IMeasSubTask>().Insert();
                                        builderInsertMeasSubTask.SetValue(c => c.Interval, measSubTask.Interval);
                                        builderInsertMeasSubTask.SetValue(c => c.Status, measSubTask.Status);
                                        builderInsertMeasSubTask.SetValue(c => c.TimeStart, measSubTask.TimeStart);
                                        builderInsertMeasSubTask.SetValue(c => c.TimeStop, measSubTask.TimeStop);
                                        builderInsertMeasSubTask.SetValue(c => c.MEAS_TASK.Id, ID.Value);
                                        builderInsertMeasSubTask.Select(c => c.Id);


                                        var measSubTaskPK = scope.Executor.Execute<MD.IMeasSubTask_PK>(builderInsertMeasSubTask);
                                        valueIdmeasSubTask = measSubTaskPK.Id;
                                        measSubTask.Id.Value = valueIdmeasSubTask;


                                        if ((measSubTask.MeasSubTaskStations != null) && (valueIdmeasSubTask > -1))
                                        {
                                            for (int v = 0; v < measSubTask.MeasSubTaskStations.Length; v++)
                                            {
                                                var subTaskStation = measSubTask.MeasSubTaskStations[v];
                                                long valueIdmeasSubTaskSta = -1;
                                                var builderInsertMeasSubTaskSta = this._dataLayer.GetBuilder<MD.IMeasSubTaskStation>().Insert();
                                                builderInsertMeasSubTaskSta.SetValue(c => c.Count, subTaskStation.Count);
                                                builderInsertMeasSubTaskSta.SetValue(c => c.Status, subTaskStation.Status);
                                                if (subTaskStation.StationId != null)
                                                {
                                                    builderInsertMeasSubTaskSta.SetValue(c => c.SENSOR.Id, subTaskStation.StationId.Value);
                                                }
                                                builderInsertMeasSubTaskSta.SetValue(c => c.MEAS_SUBTASK.Id, valueIdmeasSubTask);
                                                builderInsertMeasSubTaskSta.SetValue(c => c.TimeNextTask, subTaskStation.TimeNextTask);
                                                builderInsertMeasSubTaskSta.Select(c => c.Id);
                                                var insertMeasSubTaskStaPK = scope.Executor.Execute<MD.IMeasSubTaskStation_PK>(builderInsertMeasSubTaskSta);
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
                                        builderInsertMeasFreqParam.Select(c => c.Id);
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
                                            builderInsertResMeasFreq.Select(c => c.Id);
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
                                        builderInsertMeasStation.Select(c => c.Id);


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
                                        builderInsertOwnerData.Select(c => c.Id);
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
                                        builderInsertStationSite.Select(c => c.Id);
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
                                    builderInsertStation.Select(c => c.Id);
                                    var stationPK = scope.Executor.Execute<MD.IStation_PK>(builderInsertStation);
                                    idstationDataParam = stationPK.Id;

                                    if (idstationDataParam > -1)
                                    {
                                        long? idLinkMeasStation = -1;
                                        var builderInsertLinkMeasStation = this._dataLayer.GetBuilder<MD.ILinkMeasStation>().Insert();
                                        builderInsertLinkMeasStation.SetValue(c => c.STATION.Id, idstationDataParam);
                                        builderInsertLinkMeasStation.SetValue(c => c.MEAS_TASK.Id, ID.Value);
                                        builderInsertLinkMeasStation.Select(c => c.Id);
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
                                            builderInsertSector.Select(c => c.Id);
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
                                                    builderInsertSectorFreq.Select(c => c.Id);
                                                    var sectorFreq_PKPK = scope.Executor.Execute<MD.ISectorFreq_PK>(builderInsertSectorFreq);
                                                    idSectorFreq = sectorFreq_PKPK.Id;

                                                    if ((idSectorFreq != null) && (idSecForMeas != null))
                                                    {
                                                        var builderInsertLinkSectorFreq = this._dataLayer.GetBuilder<MD.ILinkSectorFreq>().Insert();
                                                        builderInsertLinkSectorFreq.SetValue(c => c.SECTOR_FREQ.Id, idSectorFreq);
                                                        builderInsertLinkSectorFreq.SetValue(c => c.SECTOR.Id, idSecForMeas);
                                                        builderInsertLinkSectorFreq.Select(c => c.Id);
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
                                                    builderInsertSectorMaskElement.Select(c => c.Id);

                                                    var sectorMaskElementPK = scope.Executor.Execute<MD.ISectorMaskElement_PK>(builderInsertSectorMaskElement);
                                                    sectorMaskElemId = sectorMaskElementPK.Id;


                                                    if ((sectorMaskElemId != null) && (idSecForMeas != null))
                                                    {
                                                        var builderInsertLinkSectorMaskElement = this._dataLayer.GetBuilder<MD.ILinkSectorMaskElement>().Insert();
                                                        builderInsertLinkSectorMaskElement.SetValue(c => c.SECTOR_MASK_ELEM.Id, sectorMaskElemId);
                                                        builderInsertLinkSectorMaskElement.SetValue(c => c.SECTOR.Id, idSecForMeas);
                                                        builderInsertLinkSectorMaskElement.Select(c => c.Id);
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


