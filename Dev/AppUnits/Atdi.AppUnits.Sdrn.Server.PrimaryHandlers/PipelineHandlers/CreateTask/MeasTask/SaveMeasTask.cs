using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Server;
using System.Xml;
using System.Linq;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;


namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    /// <summary>
    /// Сохранение задач в БД
    /// </summary>
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
                    var listSubTaskSensorIds = new List<long>();
                    var listSubTaskIds = new List<long>();
                    var listMeasTaskIds = new List<long>();
                    var builderSelectMeasTask = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().From();
                    builderSelectMeasTask.Select(c => c.SUBTASK.MEAS_TASK.Id);
                    builderSelectMeasTask.Select(c => c.SUBTASK.Id);
                    builderSelectMeasTask.Select(c => c.Id);
                    builderSelectMeasTask.Where(c => c.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, measTask.Id.Value);
                    scope.Executor.Fetch(builderSelectMeasTask, reader =>
                    {
                        while (reader.Read())
                        {
                            listSubTaskSensorIds.Add(reader.GetValue(c => c.Id));
                            listSubTaskIds.Add(reader.GetValue(c => c.SUBTASK.Id));
                            listMeasTaskIds.Add(reader.GetValue(c => c.SUBTASK.MEAS_TASK.Id));
                        }
                        return true;
                    });

                    if (listSubTaskSensorIds.Count > 0)
                    {
                        for (int i = 0; i < listSubTaskSensorIds.Count; i++)
                        {

                            var builderUpdateMeasSubTaskStaSave = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().Update();
                            builderUpdateMeasSubTaskStaSave.Where(c => c.Id, ConditionOperator.Equal, listSubTaskSensorIds[i]);
                            builderUpdateMeasSubTaskStaSave.SetValue(c => c.Status, status);
                            if (scope.Executor.Execute(builderUpdateMeasSubTaskStaSave) > 0)
                            {
                                isSuccess = true;
                            }
                            else
                            {
                                isSuccess = false;
                                break;
                            }

                            var builderSelectMeasSubTask = this._dataLayer.GetBuilder<MD.ISubTask>().Update();
                            builderSelectMeasSubTask.Where(c => c.Id, ConditionOperator.Equal, listSubTaskIds[i]);
                            builderSelectMeasSubTask.SetValue(c => c.Status, status);
                            if (scope.Executor.Execute(builderSelectMeasSubTask) > 0)
                            {
                                isSuccess = true;
                            }
                            else
                            {
                                isSuccess = false;
                                break;
                            }

                            var builderMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().Update();
                            builderMeasTask.Where(c => c.Id, ConditionOperator.Equal, listMeasTaskIds[i]);
                            builderMeasTask.SetValue(c => c.Status, status);
                            if (scope.Executor.Execute(builderMeasTask) > 0)
                            {
                                isSuccess = true;
                            }
                            else
                            {
                                isSuccess = false;
                                break;
                            }
                        }
                    }
                    if (isSuccess == true)
                    {
                        scope.Commit();
                    }
                    else
                    {
                        throw new Exception("An error occurred while updating the task status");
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
                isSuccess = false;
            }
            return isSuccess;
        }

        public long? SaveSpectrumOccupationParameters(long sensorIdentifier, SpectrumOccupationParameters value, IDataLayerScope dataLayerScope)
        {
            long? spectrumOccupationParametersId = null;
            if (value != null)
            {
                var measOther = value;
                var builderInsertMeasOther = this._dataLayer.GetBuilder<MD.IMeasOther>().Insert();
                builderInsertMeasOther.SetValue(c => c.LevelMinOccup, measOther.LevelMinOccup);
                builderInsertMeasOther.SetValue(c => c.Nchenal, measOther.NChenal);
                builderInsertMeasOther.SetValue(c => c.TypeSpectrumOccupation, measOther.TypeSpectrumOccupation.ToString());
                builderInsertMeasOther.SetValue(c => c.MEAS_TASK.Id, sensorIdentifier);

                var measOtherPK = dataLayerScope.Executor.Execute<MD.IMeasOther_PK>(builderInsertMeasOther);
                spectrumOccupationParametersId = measOtherPK.Id;
            }
            return spectrumOccupationParametersId;
        }

        public long? SaveMeasFreqParam(long sensorIdentifier, MeasFreqParam value, IDataLayerScope dataLayerScope)
        {
            long? measFreqParamId = null;
            if (value != null)
            {
                var freq_param = value;
                if (freq_param != null)
                {
                    var builderInsertMeasFreqParam = this._dataLayer.GetBuilder<MD.IMeasFreqParam>().Insert();
                    builderInsertMeasFreqParam.SetValue(c => c.Mode, freq_param.Mode.ToString());
                    builderInsertMeasFreqParam.SetValue(c => c.Rgl, freq_param.RgL);
                    builderInsertMeasFreqParam.SetValue(c => c.Rgu, freq_param.RgU);
                    builderInsertMeasFreqParam.SetValue(c => c.Step, freq_param.Step);
                    builderInsertMeasFreqParam.SetValue(c => c.MEAS_TASK.Id, sensorIdentifier);

                    var measFreqParamPK = dataLayerScope.Executor.Execute<MD.IMeasFreqParam_PK>(builderInsertMeasFreqParam);
                    measFreqParamId = measFreqParamPK.Id;
                }

                if ((freq_param.MeasFreqs != null) && (measFreqParamId > -1))
                {

                    for (int i = 0; i < freq_param.MeasFreqs.Length; i++)
                    {
                        var builderInsertResMeasFreq = this._dataLayer.GetBuilder<MD.IMeasFreq>().Insert();
                        builderInsertResMeasFreq.SetValue(c => c.Freq, freq_param.MeasFreqs[i].Freq);
                        builderInsertResMeasFreq.SetValue(c => c.MEAS_FREQ_PARAM.Id, measFreqParamId.Value);

                        var measFreqPK = dataLayerScope.Executor.Execute<MD.IMeasFreq_PK>(builderInsertResMeasFreq);
                    }
                }
            }
            return measFreqParamId;
        }

        public long? SaveMeasDtParam(long sensorIdentifier, MeasDtParam value, IDataLayerScope dataLayerScope)
        {
            long? measDtParamId = null;
            if (value != null)
            {
                var builderInsertMeasDtParam = this._dataLayer.GetBuilder<MD.IMeasDtParam>().Insert();
                builderInsertMeasDtParam.SetValue(c => c.Demod, value.Demod);
                builderInsertMeasDtParam.SetValue(c => c.DetectType, value.DetectType.ToString());
                builderInsertMeasDtParam.SetValue(c => c.Ifattenuation, value.IfAttenuation);
                builderInsertMeasDtParam.SetValue(c => c.MeasTime, value.MeasTime);
                builderInsertMeasDtParam.SetValue(c => c.Preamplification, value.Preamplification);
                builderInsertMeasDtParam.SetValue(c => c.Rbw, value.RBW);
                builderInsertMeasDtParam.SetValue(c => c.Rfattenuation, value.RfAttenuation);
                builderInsertMeasDtParam.SetValue(c => c.Vbw, value.VBW);
                builderInsertMeasDtParam.SetValue(c => c.SwNumber, value.SwNumber);
                if (value.ReferenceLevel != null)
                {
                    builderInsertMeasDtParam.SetValue(c => c.ReferenceLevel, value.ReferenceLevel);
                }
                if (value.NumberTotalScan != null)
                {
                    builderInsertMeasDtParam.SetValue(c => c.NumberTotalScan, value.NumberTotalScan);
                }
                builderInsertMeasDtParam.SetValue(c => c.MEAS_TASK.Id, sensorIdentifier);
                var measDtParamPK = dataLayerScope.Executor.Execute<MD.IMeasDtParam_PK>(builderInsertMeasDtParam);
                measDtParamId = measDtParamPK.Id;
            }
            return measDtParamId;
        }


        public void SaveStationsForMeasurements(long sensorIdentifier, StationDataForMeasurements[] value, IDataLayerScope dataLayerScope)
        {
            if (value != null)
            {
                for (int v = 0; v < value.Length; v++)
                {
                    var stationDataParam = value[v];

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

                        var ownerDataPK = dataLayerScope.Executor.Execute<MD.IOwnerData_PK>(builderInsertOwnerData);
                        idOwnerdata = ownerDataPK.Id;
                    }

                    if (stationDataParam.Site != null)
                    {
                        var builderInsertStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().Insert();
                        builderInsertStationSite.SetValue(c => c.Address, stationDataParam.Site.Adress);
                        builderInsertStationSite.SetValue(c => c.Lat, stationDataParam.Site.Lat);
                        builderInsertStationSite.SetValue(c => c.Lon, stationDataParam.Site.Lon);
                        builderInsertStationSite.SetValue(c => c.Region, stationDataParam.Site.Region);

                        var stationSitePK = dataLayerScope.Executor.Execute<MD.IStationSite_PK>(builderInsertStationSite);
                        idSite = stationSitePK.Id;
                    }

                    var builderInsertStation = this._dataLayer.GetBuilder<MD.IStation>().Insert();
                    builderInsertStation.SetValue(c => c.GlobalSID, stationDataParam.GlobalSID);
                    builderInsertStation.SetValue(c => c.Standart, stationDataParam.Standart);
                    builderInsertStation.SetValue(c => c.Status, stationDataParam.Status);
                    builderInsertStation.SetValue(c => c.ClientStationCode, stationDataParam.IdStation);
                    builderInsertStation.SetValue(c => c.MEAS_TASK.Id, sensorIdentifier);
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

                    var stationPK = dataLayerScope.Executor.Execute<MD.IStation_PK>(builderInsertStation);
                    idstationDataParam = stationPK.Id;

                    if (stationDataParam.Sectors != null)
                    {

                        for (int g = 0; g < stationDataParam.Sectors.Length; g++)
                        {
                            var sector = stationDataParam.Sectors[g];

                            long? idSecForMeas = -1;
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

                            var sectorPK = dataLayerScope.Executor.Execute<MD.ISector_PK>(builderInsertSector);
                            idSecForMeas = sectorPK.Id;

                            if (sector.Frequencies != null)
                            {

                                for (int d = 0; d < sector.Frequencies.Length; d++)
                                {
                                    var freq = sector.Frequencies[d];
                                    long? idSectorFreq = null;
                                    var builderInsertSectorFreq = this._dataLayer.GetBuilder<MD.ISectorFreq>().Insert();
                                    builderInsertSectorFreq.SetValue(c => c.ChannelNumber, freq.ChannalNumber);
                                    builderInsertSectorFreq.SetValue(c => c.Frequency, freq.Frequency);
                                    builderInsertSectorFreq.SetValue(c => c.ClientPlanCode, freq.IdPlan);
                                    builderInsertSectorFreq.SetValue(c => c.ClientFreqCode, freq.Id);

                                    var sectorFreq_PKPK = dataLayerScope.Executor.Execute<MD.ISectorFreq_PK>(builderInsertSectorFreq);
                                    idSectorFreq = sectorFreq_PKPK.Id;

                                    if ((idSectorFreq != null) && (idSecForMeas != null))
                                    {
                                        var builderInsertLinkSectorFreq = this._dataLayer.GetBuilder<MD.ILinkSectorFreq>().Insert();
                                        builderInsertLinkSectorFreq.SetValue(c => c.SECTOR_FREQ.Id, idSectorFreq);
                                        builderInsertLinkSectorFreq.SetValue(c => c.SECTOR.Id, idSecForMeas);

                                        var linkSectorFreqPK = dataLayerScope.Executor.Execute<MD.ILinkSectorFreq_PK>(builderInsertLinkSectorFreq);
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


                                    var sectorMaskElementPK = dataLayerScope.Executor.Execute<MD.ISectorMaskElement_PK>(builderInsertSectorMaskElement);
                                    sectorMaskElemId = sectorMaskElementPK.Id;


                                    if ((sectorMaskElemId != null) && (idSecForMeas != null))
                                    {
                                        var builderInsertLinkSectorMaskElement = this._dataLayer.GetBuilder<MD.ILinkSectorMaskElement>().Insert();
                                        builderInsertLinkSectorMaskElement.SetValue(c => c.SECTOR_MASK_ELEM.Id, sectorMaskElemId);
                                        builderInsertLinkSectorMaskElement.SetValue(c => c.SECTOR.Id, idSecForMeas);

                                        var linkSectorMaskElement_PK = dataLayerScope.Executor.Execute<MD.ILinkSectorMaskElement_PK>(builderInsertLinkSectorMaskElement);
                                    }
                                }

                            }

                        }
                    }
                }
            }
        }

        public long? SaveMeasTaskInDB(MeasTask value)
        {
            long? ID = null;
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
                        builderInsertMeasTask.SetValue(c => c.Name, value.Name);
                        builderInsertMeasTask.SetValue(c => c.Prio, value.Prio);
                        builderInsertMeasTask.SetValue(c => c.Status, value.Status);
                        builderInsertMeasTask.SetValue(c => c.Type, value.TypeMeasurements.ToString());
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


                                        if ((measSubTask.MeasSubTaskSensors != null) && (valueIdmeasSubTask > -1))
                                        {
                                            for (int v = 0; v < measSubTask.MeasSubTaskSensors.Length; v++)
                                            {
                                                var subTaskSensor = measSubTask.MeasSubTaskSensors[v];
                                                long valueIdmeasSubTaskSta = -1;
                                                var builderInsertMeasSubTaskSta = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().Insert();
                                                builderInsertMeasSubTaskSta.SetValue(c => c.Count, subTaskSensor.Count);
                                                builderInsertMeasSubTaskSta.SetValue(c => c.Status, subTaskSensor.Status);
                                                builderInsertMeasSubTaskSta.SetValue(c => c.SENSOR.Id, subTaskSensor.SensorId);
                                                builderInsertMeasSubTaskSta.SetValue(c => c.SUBTASK.Id, valueIdmeasSubTask);
                                                builderInsertMeasSubTaskSta.SetValue(c => c.TimeNextTask, subTaskSensor.TimeNextTask);

                                                var insertMeasSubTaskStaPK = scope.Executor.Execute<MD.ISubTaskSensor_PK>(builderInsertMeasSubTaskSta);
                                                valueIdmeasSubTaskSta = insertMeasSubTaskStaPK.Id;
                                                subTaskSensor.Id = valueIdmeasSubTaskSta;
                                                measSubTask.MeasSubTaskSensors[v] = subTaskSensor;
                                                
                                            }
                                        }
                                    }
                                    value.MeasSubTasks[u] = measSubTask;
                                }
                            }
                        }

                        switch (value.TypeMeasurements)
                        {
                            case MeasurementType.MonitoringStations:
                                if (value is MeasTaskMonitoringStations)
                                {
                                    var measTaskMonitoringStations = (value as MeasTaskMonitoringStations);
                                    SaveStationsForMeasurements(ID.Value, measTaskMonitoringStations.StationsForMeasurements, scope);
                                }
                                break;
                            case MeasurementType.Signaling:
                                if (value is MeasTaskSignaling)
                                {
                                    var measTaskSignaling = (value as MeasTaskSignaling);
                                    if (measTaskSignaling.SignalingMeasTaskParameters != null)
                                    {
                                        long valueIdMeasTaskSignaling = -1;
                                        var builderInsertMeasTaskSignaling = this._dataLayer.GetBuilder<MD.IMeasTaskSignaling>().Insert();
                                        if (measTaskSignaling.SignalingMeasTaskParameters.allowableExcess_dB != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.allowableExcess_dB, measTaskSignaling.SignalingMeasTaskParameters.allowableExcess_dB);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.AnalyzeByChannel != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.AnalyzeByChannel, measTaskSignaling.SignalingMeasTaskParameters.AnalyzeByChannel);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.AnalyzeSysInfoEmission != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.AnalyzeSysInfoEmission, measTaskSignaling.SignalingMeasTaskParameters.AnalyzeSysInfoEmission);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.CheckFreqChannel != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.CheckFreqChannel, measTaskSignaling.SignalingMeasTaskParameters.CheckFreqChannel);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.CorrelationAnalize != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.CorrelationAnalize, measTaskSignaling.SignalingMeasTaskParameters.CorrelationAnalize);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.CorrelationFactor != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.CorrelationFactor, measTaskSignaling.SignalingMeasTaskParameters.CorrelationFactor);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.DetailedMeasurementsBWEmission, measTaskSignaling.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.Standard != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.Standard, measTaskSignaling.SignalingMeasTaskParameters.Standard);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.triggerLevel_dBm_Hz != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.TriggerLevel_dBm_Hz, measTaskSignaling.SignalingMeasTaskParameters.triggerLevel_dBm_Hz);
                                        }

                                        if (measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters != null)
                                        {
                                            if (measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.CrossingBWPercentageForBadSignals, measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.CrossingBWPercentageForGoodSignals, measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.TimeBetweenWorkTimes_sec, measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.TypeJoinSpectrum, measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum);
                                            }
                                        }

                                        if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters != null)
                                        {
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.AutoDivisionEmitting, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.DifferenceMaxMax, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax);
                                            }

                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.MaxFreqDeviation, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.CheckLevelChannel, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.MinPointForDetailBW, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW);
                                            }

                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.DiffLevelForCalcBW, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.MinExcessNoseLevel_dB, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.NDbLevel_dB, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.NumberIgnoredPoints, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.NumberPointForChangeExcess, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.windowBW != null)
                                            {
                                                builderInsertMeasTaskSignaling.SetValue(c => c.WindowBW, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.windowBW);
                                            }

                                        }

                                        if (measTaskSignaling.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.CompareTraceJustWithRefLevels, measTaskSignaling.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.FiltrationTrace != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.FiltrationTrace, measTaskSignaling.SignalingMeasTaskParameters.FiltrationTrace);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.SignalizationNChenal != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.SignalizationNChenal, measTaskSignaling.SignalingMeasTaskParameters.SignalizationNChenal);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.SignalizationNCount != null)
                                        {
                                            builderInsertMeasTaskSignaling.SetValue(c => c.SignalizationNCount, measTaskSignaling.SignalingMeasTaskParameters.SignalizationNCount);
                                        }


                                        builderInsertMeasTaskSignaling.SetValue(c => c.MEAS_TASK.Id, ID);
                                        var measTaskSignalingPK = scope.Executor.Execute<MD.IMeasTaskSignaling_PK>(builderInsertMeasTaskSignaling);
                                        valueIdMeasTaskSignaling = measTaskSignalingPK.Id;


                                        if (measTaskSignaling.RefSituation != null)
                                        {
                                            for (int l = 0; l < measTaskSignaling.RefSituation.Length; l++)
                                            {
                                                long valueIdReferenceSituation = -1;
                                                var refSituationReferenceSignal = measTaskSignaling.RefSituation[l];
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

                                    if (measTaskSignaling.MeasDtParam != null)
                                    {
                                        SaveMeasDtParam(ID.Value, measTaskSignaling.MeasDtParam, scope);
                                    }

                                    if (measTaskSignaling.MeasFreqParam != null)
                                    {
                                        SaveMeasFreqParam(ID.Value, measTaskSignaling.MeasFreqParam, scope);
                                    }
                                }
                                break;
                            case MeasurementType.SpectrumOccupation:
                                if (value is MeasTaskSpectrumOccupation)
                                {
                                    var measTaskSpectrumOccupation = (value as MeasTaskSpectrumOccupation);
                                    if (measTaskSpectrumOccupation.MeasDtParam != null)
                                    {
                                        SaveMeasDtParam(ID.Value, measTaskSpectrumOccupation.MeasDtParam, scope);
                                    }

                                    if (measTaskSpectrumOccupation.MeasFreqParam != null)
                                    {
                                        SaveMeasFreqParam(ID.Value, measTaskSpectrumOccupation.MeasFreqParam, scope);
                                    }

                                    if (measTaskSpectrumOccupation.SpectrumOccupationParameters != null)
                                    {
                                        SaveSpectrumOccupationParameters(ID.Value, measTaskSpectrumOccupation.SpectrumOccupationParameters, scope);
                                    }
                                }
                                break;
                            case MeasurementType.Level:
                                if (value is MeasTaskLevel)
                                {
                                    var measTaskLevel = (value as MeasTaskLevel);

                                    if (measTaskLevel.MeasDtParam != null)
                                    {
                                        SaveMeasDtParam(ID.Value, measTaskLevel.MeasDtParam, scope);
                                    }

                                    if (measTaskLevel.MeasFreqParam != null)
                                    {
                                        SaveMeasFreqParam(ID.Value, measTaskLevel.MeasFreqParam, scope);
                                    }
                                }
                                break;
                            case MeasurementType.BandwidthMeas:
                                if (value is MeasTaskBandWidth)
                                {
                                    var measTaskBandWidth = (value as MeasTaskBandWidth);
                                    if (measTaskBandWidth.MeasDtParam != null)
                                    {
                                        SaveMeasDtParam(ID.Value, measTaskBandWidth.MeasDtParam, scope);
                                    }

                                    if (measTaskBandWidth.MeasFreqParam != null)
                                    {
                                        SaveMeasFreqParam(ID.Value, measTaskBandWidth.MeasFreqParam, scope);
                                    }
                                }
                                break;
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
                                throw new NotImplementedException($"Type '{value.TypeMeasurements}' not supported");
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

        private DEV.MeasuredFrequencies CreateMeasuredFrequencies(MeasFreqParam measFreqParam)
        {
            DEV.MeasuredFrequencies Frequencies = new DEV.MeasuredFrequencies();
            Frequencies = new DEV.MeasuredFrequencies();
            if (measFreqParam != null)
            {
                var freqs = measFreqParam.MeasFreqs;
                if (freqs != null)
                {
                    Double[] listFreqs = new double[freqs.Length];
                    for (int j = 0; j < freqs.Length; j++)
                    {
                        listFreqs[j] = freqs[j].Freq;
                    }
                    Frequencies.Values_MHz = listFreqs;
                }

                if (measFreqParam.Mode == FrequencyMode.FrequencyList)
                {
                    Frequencies.Mode = DataModels.Sdrns.FrequencyMode.FrequencyList;
                }
                else if (measFreqParam.Mode == FrequencyMode.FrequencyRange)
                {
                    Frequencies.Mode = DataModels.Sdrns.FrequencyMode.FrequencyRange;
                }
                else if (measFreqParam.Mode == FrequencyMode.SingleFrequency)
                {
                    Frequencies.Mode = DataModels.Sdrns.FrequencyMode.SingleFrequency;
                }
                else
                {
                    throw new NotImplementedException($"Type '{Frequencies.Mode}' not supported");
                }
                Frequencies.RgL_MHz = measFreqParam.RgL;
                Frequencies.RgU_MHz = measFreqParam.RgU;
                Frequencies.Step_kHz = measFreqParam.Step;
            }
            return Frequencies;
        }

        private DEV.DeviceMeasParam CreateDeviceMeasParam(MeasDtParam measDtParam, MeasurementType measurementType)
        {
            DEV.DeviceMeasParam deviceParam = new DEV.DeviceMeasParam();
            if (measDtParam.MeasTime != null) { deviceParam.MeasTime_sec = measDtParam.MeasTime.GetValueOrDefault(); } else { deviceParam.MeasTime_sec = 0.001; }
            switch (measDtParam.DetectType)
            {
                case DetectingType.Average:
                    deviceParam.DetectType = DataModels.Sdrns.DetectingType.Average;
                    break;
                case DetectingType.MaxPeak:
                    deviceParam.DetectType = DataModels.Sdrns.DetectingType.MaxPeak;
                    break;
                case DetectingType.MinPeak:
                    deviceParam.DetectType = DataModels.Sdrns.DetectingType.MinPeak;
                    break;
                case DetectingType.Peak:
                    deviceParam.DetectType = DataModels.Sdrns.DetectingType.Peak;
                    break;
                case DetectingType.RMS:
                    deviceParam.DetectType = DataModels.Sdrns.DetectingType.RMS;
                    break;
                case DetectingType.Auto:
                    deviceParam.DetectType = DataModels.Sdrns.DetectingType.Auto;
                    break;
                default:
                    throw new NotImplementedException($"Type '{measDtParam.DetectType}' not supported");
            }

            if (measDtParam.Preamplification != null)
            {
                deviceParam.Preamplification_dB = measDtParam.Preamplification;
            }
            else
            {
                deviceParam.Preamplification_dB = -1;
            }

            if (measDtParam.RBW != null)
            {
                deviceParam.RBW_kHz = measDtParam.RBW.GetValueOrDefault();
            }
            else
            {
                deviceParam.RBW_kHz = -1;
            }
            if (measDtParam.RfAttenuation != null)
            {
                deviceParam.RfAttenuation_dB = (int)measDtParam.RfAttenuation;
            }
            else
            {
                deviceParam.RfAttenuation_dB = -1;
            }
            if (measDtParam.NumberTotalScan != null) { deviceParam.NumberTotalScan = measDtParam.NumberTotalScan.Value; }
            if (measDtParam.VBW != null) { deviceParam.VBW_kHz = measDtParam.VBW.Value; } else { deviceParam.VBW_kHz = -1; }
            if (measDtParam.ReferenceLevel != null)
            {
                deviceParam.RefLevel_dBm = measDtParam.ReferenceLevel.Value;
            }
            else
            {
                if ((measurementType == MeasurementType.Signaling)
                || (measurementType == MeasurementType.BandwidthMeas)
                || (measurementType == MeasurementType.Level))
                {
                    deviceParam.RefLevel_dBm = 1000000000;
                }
                else if (measurementType == MeasurementType.SpectrumOccupation)
                {
                    deviceParam.RefLevel_dBm = 1000000000;
                }
            }
            return deviceParam;
        }
        

        public  Atdi.DataModels.Sdrns.Device.MeasTask[] CreateeasTaskSDRsApi(MeasTask task, string SensorName, string SdrnServer, string EquipmentTechId, long? MeasTaskId, long? SensorId, string Type = "New")
        {
            List<Atdi.DataModels.Sdrns.Device.MeasTask> ListMTSDR = new List<Atdi.DataModels.Sdrns.Device.MeasTask>();
            if (task.MeasSubTasks == null) return null;

            for (int f = 0; f < task.MeasSubTasks.Length; f++)
            {
                var SubTask = task.MeasSubTasks[f];

                if (SubTask.MeasSubTaskSensors != null)
                {
                    for (int g = 0; g < SubTask.MeasSubTaskSensors.Length; g++)
                    {
                        var SubTaskSensor = SubTask.MeasSubTaskSensors[g];

                        if ((Type == "New") || ((Type == "Stop") && ((SubTaskSensor.Status == "F") || (SubTaskSensor.Status == "P"))) || ((Type == "Run") && ((SubTaskSensor.Status == "O") || (SubTaskSensor.Status == "A"))) ||
                            ((Type == "Del") && (SubTaskSensor.Status == "Z")))
                        {
                            if (SensorId!= SubTaskSensor.SensorId)
                            {
                                continue;
                            }

                            var MTSDR = new Atdi.DataModels.Sdrns.Device.MeasTask();
                            MTSDR.TaskId = string.Format("SDRN.SubTaskSensorId.{0}", SubTaskSensor.Id);
                            if (task.Id == null) task.Id = new MeasTaskIdentifier();

                            MTSDR.SensorId = (int)SensorId;


                            if (task.Prio != null) { MTSDR.Priority = task.Prio.GetValueOrDefault(); } else { MTSDR.Priority = 10; }
                            MTSDR.SensorName = SensorName;
                            MTSDR.SdrnServer = SdrnServer;
                            MTSDR.EquipmentTechId = EquipmentTechId;
                            if (Type == "New")
                            {
                                MTSDR.SOParam = new DEV.SpectrumOccupationMeasParam();

                                if (SubTask.Interval != null) { MTSDR.Interval_sec = SubTask.Interval.GetValueOrDefault(); }

                                switch (task.TypeMeasurements)
                                {
                                    case MeasurementType.MonitoringStations:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.MonitoringStations;
                                        break;
                                    case MeasurementType.Level:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.Level;
                                        break;
                                    case MeasurementType.SpectrumOccupation:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.SpectrumOccupation;
                                        break;
                                    case MeasurementType.BandwidthMeas:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.BandwidthMeas;
                                        break;
                                    case MeasurementType.Signaling:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.Signaling;
                                        break;

                                    default:
                                        throw new NotImplementedException($"Type '{task.TypeMeasurements}' not supported");
                                }



                                if (task is MeasTaskBandWidth)
                                {
                                    var taskBandWidth = task as MeasTaskBandWidth;
                                    if (taskBandWidth.MeasDtParam == null) { taskBandWidth.MeasDtParam = new MeasDtParam(); }

                                    if (taskBandWidth.MeasDtParam.SwNumber != null) { MTSDR.ScanPerTaskNumber = taskBandWidth.MeasDtParam.SwNumber.GetValueOrDefault(); }
                                    MTSDR.DeviceParam = CreateDeviceMeasParam(taskBandWidth.MeasDtParam, task.TypeMeasurements);
                                    MTSDR.Frequencies = CreateMeasuredFrequencies(taskBandWidth.MeasFreqParam);
                                    task = taskBandWidth;
                                }
                                else if (task is MeasTaskSignaling)
                                {
                                    var taskSignaling = task as MeasTaskSignaling;
                                    if (taskSignaling.MeasDtParam == null) { taskSignaling.MeasDtParam = new MeasDtParam(); }

                                    if (taskSignaling.MeasDtParam.SwNumber != null) { MTSDR.ScanPerTaskNumber = taskSignaling.MeasDtParam.SwNumber.GetValueOrDefault(); }
                                    MTSDR.DeviceParam = CreateDeviceMeasParam(taskSignaling.MeasDtParam, task.TypeMeasurements);
                                    MTSDR.Frequencies = CreateMeasuredFrequencies(taskSignaling.MeasFreqParam);


                                    if (taskSignaling.SignalingMeasTaskParameters != null)
                                    {
                                        MTSDR.SignalingMeasTaskParameters = new DEV.SignalingMeasTask();
                                        MTSDR.SignalingMeasTaskParameters.InterruptionParameters = new DEV.SignalingInterruptionParameters();
                                        MTSDR.SignalingMeasTaskParameters.GroupingParameters = new DEV.SignalingGroupingParameters();


                                        if (taskSignaling.SignalingMeasTaskParameters.GroupingParameters != null)
                                        {
                                            MTSDR.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals = taskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals;
                                            MTSDR.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals = taskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals;
                                            MTSDR.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec = taskSignaling.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec;
                                            MTSDR.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum = taskSignaling.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum;
                                        }

                                        if (taskSignaling.SignalingMeasTaskParameters.InterruptionParameters != null)
                                        {
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting;
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax;
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW;
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB;
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB;
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints;
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess;
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.windowBW = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.windowBW;
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation;
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel;
                                            MTSDR.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW = taskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW;
                                            MTSDR.SignalingMeasTaskParameters.allowableExcess_dB = taskSignaling.SignalingMeasTaskParameters.allowableExcess_dB;
                                        }

                                        MTSDR.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels = taskSignaling.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels;
                                        MTSDR.SignalingMeasTaskParameters.FiltrationTrace = taskSignaling.SignalingMeasTaskParameters.FiltrationTrace;
                                        MTSDR.SignalingMeasTaskParameters.SignalizationNChenal = taskSignaling.SignalingMeasTaskParameters.SignalizationNChenal;
                                        MTSDR.SignalingMeasTaskParameters.SignalizationNCount = taskSignaling.SignalingMeasTaskParameters.SignalizationNCount;
                                        MTSDR.SignalingMeasTaskParameters.AnalyzeByChannel = taskSignaling.SignalingMeasTaskParameters.AnalyzeByChannel;
                                        MTSDR.SignalingMeasTaskParameters.AnalyzeSysInfoEmission = taskSignaling.SignalingMeasTaskParameters.AnalyzeSysInfoEmission;
                                        MTSDR.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission = taskSignaling.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission;
                                        MTSDR.SignalingMeasTaskParameters.Standard = taskSignaling.SignalingMeasTaskParameters.Standard;
                                        MTSDR.SignalingMeasTaskParameters.triggerLevel_dBm_Hz = taskSignaling.SignalingMeasTaskParameters.triggerLevel_dBm_Hz;

                                        MTSDR.SignalingMeasTaskParameters.CorrelationAnalize = taskSignaling.SignalingMeasTaskParameters.CorrelationAnalize;
                                        MTSDR.SignalingMeasTaskParameters.CorrelationFactor = taskSignaling.SignalingMeasTaskParameters.CorrelationFactor;
                                        MTSDR.SignalingMeasTaskParameters.CheckFreqChannel = taskSignaling.SignalingMeasTaskParameters.CheckFreqChannel;
                                    }

                                    if (taskSignaling.RefSituation != null)
                                    {
                                        var listReferenceSituation = new List<DEV.ReferenceSituation>();
                                        for (int k = 0; k < taskSignaling.RefSituation.Length; k++)
                                        {
                                            var refSituation = new DEV.ReferenceSituation();
                                            var refSituationTemp = taskSignaling.RefSituation[k];
                                            refSituation.SensorId = (int)refSituationTemp.SensorId;

                                            var referenceSignal = refSituationTemp.ReferenceSignal;
                                            if (referenceSignal.Length > 0)
                                            {
                                                refSituation.ReferenceSignal = new DEV.ReferenceSignal[referenceSignal.Length];
                                                for (int l = 0; l < referenceSignal.Length; l++)
                                                {
                                                    var refSituationReferenceSignal = refSituation.ReferenceSignal[l];
                                                    refSituationReferenceSignal = new DEV.ReferenceSignal();
                                                    refSituationReferenceSignal.Bandwidth_kHz = referenceSignal[l].Bandwidth_kHz;
                                                    refSituationReferenceSignal.Frequency_MHz = referenceSignal[l].Frequency_MHz;
                                                    refSituationReferenceSignal.LevelSignal_dBm = referenceSignal[l].LevelSignal_dBm;
                                                    refSituationReferenceSignal.IcsmId = referenceSignal[l].IcsmId;
                                                    refSituationReferenceSignal.SignalMask = new DEV.SignalMask();
                                                    if (referenceSignal[l].SignalMask != null)
                                                    {
                                                        refSituationReferenceSignal.SignalMask.Freq_kHz = referenceSignal[l].SignalMask.Freq_kHz;
                                                        refSituationReferenceSignal.SignalMask.Loss_dB = referenceSignal[l].SignalMask.Loss_dB;
                                                    }
                                                    refSituation.ReferenceSignal[l] = refSituationReferenceSignal;
                                                }
                                            }
                                            listReferenceSituation.Add(refSituation);
                                        }
                                        if (listReferenceSituation.Count > 0)
                                        {
                                            //MTSDR.RefSituation = listReferenceSituation.ToArray();
                                            MTSDR.RefSituation = listReferenceSituation[0];
                                        }
                                    }
                                    task = taskSignaling;
                                }
                                else if (task is MeasTaskMonitoringStations)
                                {
                                    var taskMonitoringStations = task as MeasTaskMonitoringStations;
                                    if (taskMonitoringStations.StationsForMeasurements != null)
                                    {
                                        MTSDR.Stations = new DataModels.Sdrns.Device.MeasuredStation[taskMonitoringStations.StationsForMeasurements.Count()];
                                        if (task.TypeMeasurements == MeasurementType.MonitoringStations)
                                        { // 21_02_2018 в данном случае мы передаем станции  исключительно для системы мониторинга станций т.е. один таск на месяц Надо проверить.
                                            if (taskMonitoringStations.StationsForMeasurements != null)
                                            {
                                                ///MTSDR.StationsForMeasurements = task.StationsForMeasurements;
                                                // далее сформируем переменную GlobalSID 
                                                for (int i = 0; i < taskMonitoringStations.StationsForMeasurements.Count(); i++)
                                                {
                                                    MTSDR.Stations[i] = new DataModels.Sdrns.Device.MeasuredStation();
                                                    var stationI = MTSDR.Stations[i];
                                                    string CodeOwener = "0";
                                                    stationI.Owner = new DataModels.Sdrns.Device.StationOwner();
                                                    var station = taskMonitoringStations.StationsForMeasurements[i];
                                                    if (station.Owner != null)
                                                    {
                                                        var owner = stationI.Owner;
                                                        owner.Address = station.Owner.Addres;
                                                        owner.Code = station.Owner.Code;
                                                        owner.Id = (int)station.Owner.Id;
                                                        owner.OKPO = station.Owner.OKPO;
                                                        owner.OwnerName = station.Owner.OwnerName;
                                                        owner.Zip = station.Owner.Zip;


                                                        if (owner.OKPO == "14333937") { CodeOwener = "1"; };
                                                        if (owner.OKPO == "22859846") { CodeOwener = "6"; };
                                                        if (owner.OKPO == "21673832") { CodeOwener = "3"; };
                                                        if (owner.OKPO == "37815221") { CodeOwener = "7"; };
                                                    }
                                                    stationI.GlobalSid = "255 " + CodeOwener + " 00000 " + string.Format("{0:00000}", station.IdStation);
                                                    station.GlobalSID = stationI.GlobalSid;

                                                    stationI.OwnerGlobalSid = taskMonitoringStations.StationsForMeasurements[i].GlobalSID;//работать с таблицей (доп. создасть в БД по GlobalSID и Standard)
                                                                                                                        //
                                                    stationI.License = new DataModels.Sdrns.Device.StationLicenseInfo();
                                                    if (station.LicenseParameter != null)
                                                    {
                                                        stationI.License.CloseDate = station.LicenseParameter.CloseDate;
                                                        stationI.License.EndDate = station.LicenseParameter.EndDate;
                                                        stationI.License.IcsmId = station.LicenseParameter.Id;
                                                        stationI.License.Name = station.LicenseParameter.DozvilName;
                                                        stationI.License.StartDate = station.LicenseParameter.StartDate;
                                                    }

                                                    stationI.Site = new DataModels.Sdrns.Device.StationSite();
                                                    if (station.Site != null)
                                                    {
                                                        stationI.Site.Adress = station.Site.Adress;
                                                        stationI.Site.Lat = station.Site.Lat;
                                                        stationI.Site.Lon = station.Site.Lon;
                                                        stationI.Site.Region = station.Site.Region;
                                                    }
                                                    stationI.Standard = station.Standart;
                                                    stationI.StationId = station.IdStation.ToString();
                                                    stationI.Status = station.Status;


                                                    if (station.Sectors != null)
                                                    {
                                                        stationI.Sectors = new DataModels.Sdrns.Device.StationSector[station.Sectors.Length];
                                                        for (int j = 0; j < station.Sectors.Length; j++)
                                                        {
                                                            var sector = station.Sectors[j];
                                                            stationI.Sectors[j] = new DataModels.Sdrns.Device.StationSector();
                                                            var statSector = stationI.Sectors[j];
                                                            statSector.AGL = sector.AGL;
                                                            statSector.Azimuth = sector.Azimut;

                                                            if (sector.MaskBW != null)
                                                            {
                                                                statSector.BWMask = new DataModels.Sdrns.Device.ElementsMask[sector.MaskBW.Length];
                                                                for (int k = 0; k < sector.MaskBW.Length; k++)
                                                                {
                                                                    statSector.BWMask[k] = new DataModels.Sdrns.Device.ElementsMask();
                                                                    statSector.BWMask[k].BW_kHz = sector.MaskBW[k].BW;
                                                                    statSector.BWMask[k].Level_dB = sector.MaskBW[k].level;
                                                                }
                                                            }
                                                            statSector.BW_kHz = sector.BW;
                                                            statSector.ClassEmission = sector.ClassEmission;
                                                            statSector.EIRP_dBm = sector.EIRP;

                                                            if (sector.Frequencies != null)
                                                            {
                                                                statSector.Frequencies = new DataModels.Sdrns.Device.SectorFrequency[sector.Frequencies.Length];
                                                                for (int k = 0; k < sector.Frequencies.Length; k++)
                                                                {
                                                                    statSector.Frequencies[k] = new DataModels.Sdrns.Device.SectorFrequency();
                                                                    statSector.Frequencies[k].ChannelNumber = (int?)sector.Frequencies[k].ChannalNumber;
                                                                    statSector.Frequencies[k].Frequency_MHz = sector.Frequencies[k].Frequency;
                                                                    statSector.Frequencies[k].Id = (int?)sector.Frequencies[k].Id;
                                                                    statSector.Frequencies[k].PlanId = (int?)sector.Frequencies[k].IdPlan;
                                                                }
                                                            }
                                                            statSector.SectorId = sector.IdSector.ToString();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (task is MeasTaskSpectrumOccupation)
                                {
                                    var taskSO = task as MeasTaskSpectrumOccupation;
                                    if (taskSO.SpectrumOccupationParameters == null) taskSO.SpectrumOccupationParameters = new SpectrumOccupationParameters();
                                    if (taskSO.SpectrumOccupationParameters.LevelMinOccup != null) { MTSDR.SOParam.LevelMinOccup_dBm = taskSO.SpectrumOccupationParameters.LevelMinOccup.GetValueOrDefault(); } else { MTSDR.SOParam.LevelMinOccup_dBm = -70; }
                                    if (taskSO.SpectrumOccupationParameters.NChenal != null) { MTSDR.SOParam.MeasurmentNumber = taskSO.SpectrumOccupationParameters.NChenal.GetValueOrDefault(); } else { MTSDR.SOParam.MeasurmentNumber = 10; }
                                    switch (taskSO.SpectrumOccupationParameters.TypeSpectrumOccupation)
                                    {
                                        case SpectrumOccupationType.FreqBandwidthOccupation:
                                            MTSDR.SOParam.Type = DataModels.Sdrns.SpectrumOccupationType.FreqBandOccupancy;
                                            break;
                                        case SpectrumOccupationType.FreqChannelOccupation:
                                            MTSDR.SOParam.Type = DataModels.Sdrns.SpectrumOccupationType.FreqChannelOccupancy;
                                            break;
                                        default:
                                            throw new NotImplementedException($"Type '{taskSO.SpectrumOccupationParameters.TypeSpectrumOccupation}' not supported");
                                    }


                                    if (taskSO.MeasDtParam == null) { taskSO.MeasDtParam = new MeasDtParam(); }

                                    if (taskSO.MeasDtParam.SwNumber != null) { MTSDR.ScanPerTaskNumber = taskSO.MeasDtParam.SwNumber.GetValueOrDefault(); }
                                    MTSDR.DeviceParam = CreateDeviceMeasParam(taskSO.MeasDtParam, task.TypeMeasurements);
                                    MTSDR.Frequencies = CreateMeasuredFrequencies(taskSO.MeasFreqParam);

                                    task = taskSO;
                                }
                                else if (task is MeasTaskLevel)
                                {
                                    var taskLevel = task as MeasTaskLevel;
                                    if (taskLevel.MeasDtParam == null) { taskLevel.MeasDtParam = new MeasDtParam(); }

                                    if (taskLevel.MeasDtParam.SwNumber != null) { MTSDR.ScanPerTaskNumber = taskLevel.MeasDtParam.SwNumber.GetValueOrDefault(); }
                                    MTSDR.DeviceParam = CreateDeviceMeasParam(taskLevel.MeasDtParam, task.TypeMeasurements);
                                    MTSDR.Frequencies = CreateMeasuredFrequencies(taskLevel.MeasFreqParam);

                                    task = taskLevel;
                                }

                                double subFreqMaxMin = 0;
                                if ((MTSDR.Frequencies != null) && (MTSDR.Frequencies.Values_MHz != null) && (MTSDR.Frequencies.Values_MHz.Length > 0))
                                {
                                    var minFreq = MTSDR.Frequencies.Values_MHz.Min();
                                    var maxFreq = MTSDR.Frequencies.Values_MHz.Max();
                                    subFreqMaxMin = maxFreq - minFreq;
                                }
                                if ((subFreqMaxMin >= 0) && (MTSDR.Frequencies.Step_kHz>0))
                                {
                                    MTSDR.DeviceParam.ScanBW_kHz = subFreqMaxMin * 1000 + MTSDR.Frequencies.Step_kHz;
                                }

                                MTSDR.ScanParameters = new DataModels.Sdrns.Device.StandardScanParameter[] { };
                                MTSDR.StartTime = SubTask.TimeStart;
                                MTSDR.StopTime = SubTask.TimeStop;
                                MTSDR.Status = SubTask.Status;
                                MTSDR.MobEqipmentMeasurements = new DataModels.Sdrns.MeasurementType[3];
                                MTSDR.MobEqipmentMeasurements[0] = DataModels.Sdrns.MeasurementType.MonitoringStations;
                                MTSDR.MobEqipmentMeasurements[1] = DataModels.Sdrns.MeasurementType.BandwidthMeas;
                                MTSDR.MobEqipmentMeasurements[2] = DataModels.Sdrns.MeasurementType.Level;
                                

                            }
                            ListMTSDR.Add(MTSDR);
                        }
                    }
                }
            }
            return ListMTSDR.ToArray();
        }

    }
}


