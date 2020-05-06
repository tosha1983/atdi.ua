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
    public class UpdateMeasTask
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;

        public UpdateMeasTask(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        

        public bool SaveSpectrumOccupationParameters(long taskId, SpectrumOccupationParameters value, IDataLayerScope dataLayerScope)
        {
            bool isSuccess = false;
            try
            {
                if (value != null)
                {
                    long idMeasOther = -1;
                    var queryForMeasOther = this._dataLayer.GetBuilder<MD.IMeasOther>()
                    .From()
                    .Select(c => c.Id, c => c.LevelMinOccup, c => c.SupportMultyLevel, c => c.Nchenal, c => c.TypeSpectrumOccupation)
                    .Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                    dataLayerScope.Executor.Fetch(queryForMeasOther, readerMeasOther =>
                    {
                        while (readerMeasOther.Read())
                        {
                            idMeasOther = readerMeasOther.GetValue(c => c.Id);
                            break;
                        }
                        return true;
                    });

                    if (idMeasOther > -1)
                    {
                        bool isUpdate = false;
                        var measOther = value;
                        var builderUpdateMeasOther = this._dataLayer.GetBuilder<MD.IMeasOther>().Update();
                        if (measOther.LevelMinOccup != null)
                        {
                            isUpdate = true;
                            builderUpdateMeasOther.SetValue(c => c.LevelMinOccup, measOther.LevelMinOccup);
                        }
                        if (measOther.SupportMultyLevel != null)
                        {
                            isUpdate = true;
                            builderUpdateMeasOther.SetValue(c => c.SupportMultyLevel, measOther.SupportMultyLevel);
                        }
                        if (measOther.NChenal != null)
                        {
                            isUpdate = true;
                            builderUpdateMeasOther.SetValue(c => c.Nchenal, measOther.NChenal);
                        }
                        builderUpdateMeasOther.SetValue(c => c.TypeSpectrumOccupation, measOther.TypeSpectrumOccupation.ToString());
                        builderUpdateMeasOther.Where(c => c.Id, ConditionOperator.Equal, idMeasOther);
                        if (isUpdate)
                        {
                            dataLayerScope.Executor.Execute(builderUpdateMeasOther);
                        }
                    }
                    isSuccess = true;
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        public bool SaveMeasFreqParam(long taskId, MeasFreqParam value, IDataLayerScope dataLayerScope)
        {
            bool isSuccess = false;
            try
            {
                if (value != null)
                {
                    var freq_param = value;
                    if (freq_param != null)
                    {
                        var builderDeleteIMeasFreqAll = this._dataLayer.GetBuilder<MD.IMeasFreq>().Delete();
                        builderDeleteIMeasFreqAll.Where(c => c.MEAS_FREQ_PARAM.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                        dataLayerScope.Executor.Execute(builderDeleteIMeasFreqAll);

                        var builderDeleteIMeasFreqParamAll = this._dataLayer.GetBuilder<MD.IMeasFreqParam>().Delete();
                        builderDeleteIMeasFreqParamAll.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                        dataLayerScope.Executor.Execute(builderDeleteIMeasFreqParamAll);


                        var builderInsertMeasFreqParam = this._dataLayer.GetBuilder<MD.IMeasFreqParam>().Insert();
                        builderInsertMeasFreqParam.SetValue(c => c.Mode, freq_param.Mode.ToString());
                        builderInsertMeasFreqParam.SetValue(c => c.Rgl, freq_param.RgL);
                        builderInsertMeasFreqParam.SetValue(c => c.Rgu, freq_param.RgU);
                        builderInsertMeasFreqParam.SetValue(c => c.Step, freq_param.Step);
                        builderInsertMeasFreqParam.SetValue(c => c.MEAS_TASK.Id, taskId);

                        var measFreqParamPK = dataLayerScope.Executor.Execute<MD.IMeasFreqParam_PK>(builderInsertMeasFreqParam);
                        var measFreqParamId = measFreqParamPK.Id;

                        if ((freq_param.MeasFreqs != null) && (measFreqParamId > -1))
                        {

                            for (int i = 0; i < freq_param.MeasFreqs.Length; i++)
                            {
                                var builderInsertResMeasFreq = this._dataLayer.GetBuilder<MD.IMeasFreq>().Insert();
                                builderInsertResMeasFreq.SetValue(c => c.Freq, freq_param.MeasFreqs[i].Freq);
                                builderInsertResMeasFreq.SetValue(c => c.MEAS_FREQ_PARAM.Id, measFreqParamId);

                                var measFreqPK = dataLayerScope.Executor.Execute<MD.IMeasFreq_PK>(builderInsertResMeasFreq);
                            }
                        }
                    }
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        public bool SaveMeasDtParam(long taskid, MeasDtParam value, IDataLayerScope dataLayerScope)
        {
            bool isSuccess = false;
            try
            {
                if (value != null)
                {
                    var builderUpdateMeasDtParam = this._dataLayer.GetBuilder<MD.IMeasDtParam>().Update();
                    if (value.Demod != null)
                    {
                        builderUpdateMeasDtParam.SetValue(c => c.Demod, value.Demod);
                    }
                    
                    builderUpdateMeasDtParam.SetValue(c => c.DetectType, value.DetectType.ToString());
                    if (value.IfAttenuation != null)
                    {
                        builderUpdateMeasDtParam.SetValue(c => c.Ifattenuation, value.IfAttenuation);
                    }
                    if (value.MeasTime != null)
                    {
                        builderUpdateMeasDtParam.SetValue(c => c.MeasTime, value.MeasTime);
                    }
                    if (value.Preamplification != null)
                    {
                        builderUpdateMeasDtParam.SetValue(c => c.Preamplification, value.Preamplification);
                    }
                    if (value.RBW != null)
                    {
                        builderUpdateMeasDtParam.SetValue(c => c.Rbw, value.RBW);
                    }
                    if (value.RfAttenuation != null)
                    {
                        builderUpdateMeasDtParam.SetValue(c => c.Rfattenuation, value.RfAttenuation);
                    }
                    if (value.VBW != null)
                    {
                        builderUpdateMeasDtParam.SetValue(c => c.Vbw, value.VBW);
                    }
                    if (value.SwNumber != null)
                    {
                        builderUpdateMeasDtParam.SetValue(c => c.SwNumber, value.SwNumber);
                    }
                    if (value.ReferenceLevel != null)
                    {
                        builderUpdateMeasDtParam.SetValue(c => c.ReferenceLevel, value.ReferenceLevel);
                    }
                    if (value.NumberTotalScan != null)
                    {
                        builderUpdateMeasDtParam.SetValue(c => c.NumberTotalScan, value.NumberTotalScan);
                    }
                    builderUpdateMeasDtParam.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal,  taskid);
                    dataLayerScope.Executor.Execute(builderUpdateMeasDtParam);
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }


        public bool SaveStationsForMeasurements(long taskId, StationDataForMeasurements[] value, IDataLayerScope dataLayerScope)
        {
            bool isSuccess = false;
            try
            {
                if (value != null)
                {
                    for (int v = 0; v < value.Length; v++)
                    {

                        var queryForLinkSectorMaskElement = this._dataLayer.GetBuilder<MD.ILinkSectorMaskElement>()
                        .From()
                        .Select(c => c.Id, c => c.SECTOR_MASK_ELEM.Id, c => c.SECTOR.Id)
                        .Where(c => c.SECTOR.STATION.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                        dataLayerScope.Executor.Fetch(queryForLinkSectorMaskElement, readerLinkSectorMaskElement =>
                        {
                            while (readerLinkSectorMaskElement.Read())
                            {
                                var builderDeleteSectorMaskElementAll = this._dataLayer.GetBuilder<MD.ISectorMaskElement>().Delete();
                                builderDeleteSectorMaskElementAll.Where(c => c.Id, ConditionOperator.Equal, readerLinkSectorMaskElement.GetValue(c => c.SECTOR_MASK_ELEM.Id));
                                dataLayerScope.Executor.Execute(builderDeleteSectorMaskElementAll);
                            }
                            return true;
                        });


                        var builderDeleteLinkSectorMaskElementAll = this._dataLayer.GetBuilder<MD.ILinkSectorMaskElement>().Delete();
                        builderDeleteLinkSectorMaskElementAll.Where(c => c.SECTOR.STATION.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                        dataLayerScope.Executor.Execute(builderDeleteLinkSectorMaskElementAll);



                        var queryForLinkSectorFreq = this._dataLayer.GetBuilder<MD.ILinkSectorFreq>()
                        .From()
                        .Select(c => c.Id, c => c.SECTOR_FREQ.Id, c => c.SECTOR.Id)
                        .Where(c => c.SECTOR.STATION.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                        dataLayerScope.Executor.Fetch(queryForLinkSectorFreq, readerLinkSectorFreq =>
                        {
                            while (readerLinkSectorFreq.Read())
                            {
                                var builderDeleteSectorFreqrAll = this._dataLayer.GetBuilder<MD.ISectorFreq>().Delete();
                                builderDeleteSectorFreqrAll.Where(c => c.Id, ConditionOperator.Equal, readerLinkSectorFreq.GetValue(c => c.SECTOR_FREQ.Id));
                                dataLayerScope.Executor.Execute(builderDeleteSectorFreqrAll);
                            }
                            return true;
                        });


                        var builderDeleteLinkSectorFreqAll = this._dataLayer.GetBuilder<MD.ILinkSectorFreq>().Delete();
                        builderDeleteLinkSectorFreqAll.Where(c => c.SECTOR.STATION.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                        dataLayerScope.Executor.Execute(builderDeleteLinkSectorFreqAll);


                        var builderDeleteSectorAll = this._dataLayer.GetBuilder<MD.ISector>().Delete();
                        builderDeleteSectorAll.Where(c => c.STATION.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                        dataLayerScope.Executor.Execute(builderDeleteSectorAll);





                        var queryForLinkStation = this._dataLayer.GetBuilder<MD.IStation>()
                        .From()
                        .Select(c => c.Id, c => c.OWNER_DATA.Id, c => c.STATION_SITE.Id)
                        .Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                        dataLayerScope.Executor.Fetch(queryForLinkStation, readerStation =>
                        {
                            while (readerStation.Read())
                            {
                                var builderDeleteStationSiteAll = this._dataLayer.GetBuilder<MD.IStationSite>().Delete();
                                builderDeleteStationSiteAll.Where(c => c.Id, ConditionOperator.Equal, readerStation.GetValue(c => c.STATION_SITE.Id));
                                dataLayerScope.Executor.Execute(builderDeleteStationSiteAll);

                                var builderDeleteOwnerDataAll = this._dataLayer.GetBuilder<MD.IOwnerData>().Delete();
                                builderDeleteOwnerDataAll.Where(c => c.Id, ConditionOperator.Equal, readerStation.GetValue(c => c.OWNER_DATA.Id));
                                dataLayerScope.Executor.Execute(builderDeleteOwnerDataAll);
                            }
                            return true;
                        });

                        var builderDeleteStationAll = this._dataLayer.GetBuilder<MD.IStation>().Delete();
                        builderDeleteStationAll.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                        dataLayerScope.Executor.Execute(builderDeleteStationAll);



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
                        builderInsertStation.SetValue(c => c.MEAS_TASK.Id, taskId);
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
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        public bool UpdateMeasTaskInDB(MeasTask value)
        {
            bool isSuccess = false;
            if (value.Id != null)
            {
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        scope.BeginTran();

                        var builderUpdateMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().Update();
                        if (value.CreatedBy != null)
                        {
                            builderUpdateMeasTask.SetValue(c => c.CreatedBy, value.CreatedBy);
                        }
                        if (value.DateCreated != null)
                        {
                            builderUpdateMeasTask.SetValue(c => c.DateCreated, value.DateCreated);
                        }
                       
                        builderUpdateMeasTask.SetValue(c => c.ExecutionMode, value.ExecutionMode.ToString());
                        if (value.Name != null)
                        {
                            builderUpdateMeasTask.SetValue(c => c.Name, value.Name);
                        }
                        if (value.Prio != null)
                        {
                            builderUpdateMeasTask.SetValue(c => c.Prio, value.Prio);
                        }
                        if (value.Status != null)
                        {
                            builderUpdateMeasTask.SetValue(c => c.Status, value.Status);
                        }
                        
                        builderUpdateMeasTask.SetValue(c => c.Type, value.TypeMeasurements.ToString());
                        
                        if (value.MeasTimeParamList != null)
                        {
                            if (value.MeasTimeParamList.PerStart != null)
                            {
                                builderUpdateMeasTask.SetValue(c => c.PerStart, value.MeasTimeParamList.PerStart);
                            }
                            if (value.MeasTimeParamList.PerStop != null)
                            {
                                builderUpdateMeasTask.SetValue(c => c.PerStop, value.MeasTimeParamList.PerStop);
                            }
                            if (value.MeasTimeParamList.TimeStart != null)
                            {
                                builderUpdateMeasTask.SetValue(c => c.TimeStart, value.MeasTimeParamList.TimeStart);
                            }
                            if (value.MeasTimeParamList.TimeStop != null)
                            {
                                builderUpdateMeasTask.SetValue(c => c.TimeStop, value.MeasTimeParamList.TimeStop);
                            }
                            if (value.MeasTimeParamList.PerInterval != null)
                            {
                                builderUpdateMeasTask.SetValue(c => c.PerInterval, value.MeasTimeParamList.PerInterval);
                            }
                        }
                        builderUpdateMeasTask.Where(c => c.Id, ConditionOperator.Equal, value.Id.Value);
                        scope.Executor.Execute(builderUpdateMeasTask);


                        if (value.MeasSubTasks != null)
                        {
                            var builderDeleteSubTaskSensorAll = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().Delete();
                            builderDeleteSubTaskSensorAll.Where(c => c.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, value.Id.Value);
                            scope.Executor.Execute(builderDeleteSubTaskSensorAll);

                            var builderDeleteSubTaskAll = this._dataLayer.GetBuilder<MD.ISubTask>().Delete();
                            builderDeleteSubTaskAll.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, value.Id.Value);
                            scope.Executor.Execute(builderDeleteSubTaskAll);


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
                                    builderInsertMeasSubTask.SetValue(c => c.MEAS_TASK.Id, value.Id.Value);



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


                        switch (value.TypeMeasurements)
                        {
                            case MeasurementType.MonitoringStations:
                                if (value is MeasTaskMonitoringStations)
                                {
                                    var measTaskMonitoringStations = (value as MeasTaskMonitoringStations);
                                    SaveStationsForMeasurements(value.Id.Value, measTaskMonitoringStations.StationsForMeasurements, scope);
                                }
                                break;
                            case MeasurementType.Signaling:
                                if (value is MeasTaskSignaling)
                                {
                                    var measTaskSignaling = (value as MeasTaskSignaling);
                                    if (measTaskSignaling.SignalingMeasTaskParameters != null)
                                    {

                                        var builderUpdateMeasTaskSignaling = this._dataLayer.GetBuilder<MD.IMeasTaskSignaling>().Update();
                                        if (measTaskSignaling.SignalingMeasTaskParameters.allowableExcess_dB != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.allowableExcess_dB, measTaskSignaling.SignalingMeasTaskParameters.allowableExcess_dB);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.AnalyzeByChannel != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.AnalyzeByChannel, measTaskSignaling.SignalingMeasTaskParameters.AnalyzeByChannel);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.CollectEmissionInstrumentalEstimation != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.CollectEmissionInstrumentalEstimation, measTaskSignaling.SignalingMeasTaskParameters.CollectEmissionInstrumentalEstimation);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.AnalyzeSysInfoEmission != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.AnalyzeSysInfoEmission, measTaskSignaling.SignalingMeasTaskParameters.AnalyzeSysInfoEmission);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.CheckFreqChannel != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.CheckFreqChannel, measTaskSignaling.SignalingMeasTaskParameters.CheckFreqChannel);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.CorrelationAnalize != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.CorrelationAnalize, measTaskSignaling.SignalingMeasTaskParameters.CorrelationAnalize);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.CorrelationFactor != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.CorrelationFactor, measTaskSignaling.SignalingMeasTaskParameters.CorrelationFactor);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.DetailedMeasurementsBWEmission, measTaskSignaling.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.Standard != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.Standard, measTaskSignaling.SignalingMeasTaskParameters.Standard);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.triggerLevel_dBm_Hz != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.TriggerLevel_dBm_Hz, measTaskSignaling.SignalingMeasTaskParameters.triggerLevel_dBm_Hz);
                                        }

                                        if (measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters != null)
                                        {
                                            if (measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.CrossingBWPercentageForBadSignals, measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.CrossingBWPercentageForGoodSignals, measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.TimeBetweenWorkTimes_sec, measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.TypeJoinSpectrum, measTaskSignaling.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum);
                                            }
                                        }

                                        if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters != null)
                                        {
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.AutoDivisionEmitting, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.DifferenceMaxMax, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax);
                                            }

                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.MaxFreqDeviation, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.CheckLevelChannel, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.MinPointForDetailBW, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW);
                                            }

                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.DiffLevelForCalcBW, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.MinExcessNoseLevel_dB, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.NDbLevel_dB, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.NumberIgnoredPoints, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.NumberPointForChangeExcess, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess);
                                            }
                                            if (measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.windowBW != null)
                                            {
                                                builderUpdateMeasTaskSignaling.SetValue(c => c.WindowBW, measTaskSignaling.SignalingMeasTaskParameters.InterruptionParameters.windowBW);
                                            }

                                        }

                                        if (measTaskSignaling.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.CompareTraceJustWithRefLevels, measTaskSignaling.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.FiltrationTrace != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.FiltrationTrace, measTaskSignaling.SignalingMeasTaskParameters.FiltrationTrace);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.SignalizationNChenal != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.SignalizationNChenal, measTaskSignaling.SignalingMeasTaskParameters.SignalizationNChenal);
                                        }
                                        if (measTaskSignaling.SignalingMeasTaskParameters.SignalizationNCount != null)
                                        {
                                            builderUpdateMeasTaskSignaling.SetValue(c => c.SignalizationNCount, measTaskSignaling.SignalingMeasTaskParameters.SignalizationNCount);
                                        }

                                        builderUpdateMeasTaskSignaling.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, value.Id.Value);
                                        scope.Executor.Execute(builderUpdateMeasTaskSignaling);



                                        if (measTaskSignaling.RefSituation != null)
                                        {

                                            var builderDeleteReferenceSituationqAll = this._dataLayer.GetBuilder<MD.IReferenceSituation>().Delete();
                                            builderDeleteReferenceSituationqAll.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, value.Id.Value);
                                            scope.Executor.Execute(builderDeleteReferenceSituationqAll);

                                            var builderDeleteReferenceSignalAll = this._dataLayer.GetBuilder<MD.IReferenceSignal>().Delete();
                                            builderDeleteReferenceSignalAll.Where(c => c.REFERENCE_SITUATION.MEAS_TASK.Id, ConditionOperator.Equal, value.Id.Value);
                                            scope.Executor.Execute(builderDeleteReferenceSignalAll);


                                            for (int l = 0; l < measTaskSignaling.RefSituation.Length; l++)
                                            {
                                                long valueIdReferenceSituation = -1;
                                                var refSituationReferenceSignal = measTaskSignaling.RefSituation[l];
                                                var builderInsertReferenceSituation = this._dataLayer.GetBuilder<MD.IReferenceSituation>().Insert();
                                                builderInsertReferenceSituation.SetValue(c => c.MEAS_TASK.Id, value.Id.Value);
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
                                        SaveMeasDtParam(value.Id.Value, measTaskSignaling.MeasDtParam, scope);
                                    }

                                    if (measTaskSignaling.MeasFreqParam != null)
                                    {
                                        SaveMeasFreqParam(value.Id.Value, measTaskSignaling.MeasFreqParam, scope);
                                    }
                                }
                                break;
                            case MeasurementType.SpectrumOccupation:
                                if (value is MeasTaskSpectrumOccupation)
                                {
                                    var measTaskSpectrumOccupation = (value as MeasTaskSpectrumOccupation);
                                    if (measTaskSpectrumOccupation.MeasDtParam != null)
                                    {
                                        SaveMeasDtParam(value.Id.Value, measTaskSpectrumOccupation.MeasDtParam, scope);
                                    }

                                    if (measTaskSpectrumOccupation.MeasFreqParam != null)
                                    {
                                        SaveMeasFreqParam(value.Id.Value, measTaskSpectrumOccupation.MeasFreqParam, scope);
                                    }

                                    if (measTaskSpectrumOccupation.SpectrumOccupationParameters != null)
                                    {
                                        SaveSpectrumOccupationParameters(value.Id.Value, measTaskSpectrumOccupation.SpectrumOccupationParameters, scope);
                                    }
                                }
                                break;
                            case MeasurementType.Level:
                                if (value is MeasTaskLevel)
                                {
                                    var measTaskLevel = (value as MeasTaskLevel);

                                    if (measTaskLevel.MeasDtParam != null)
                                    {
                                        SaveMeasDtParam(value.Id.Value, measTaskLevel.MeasDtParam, scope);
                                    }

                                    if (measTaskLevel.MeasFreqParam != null)
                                    {
                                        SaveMeasFreqParam(value.Id.Value, measTaskLevel.MeasFreqParam, scope);
                                    }
                                }
                                break;
                            case MeasurementType.BandwidthMeas:
                                if (value is MeasTaskBandWidth)
                                {
                                    var measTaskBandWidth = (value as MeasTaskBandWidth);
                                    if (measTaskBandWidth.MeasDtParam != null)
                                    {
                                        SaveMeasDtParam(value.Id.Value, measTaskBandWidth.MeasDtParam, scope);
                                    }

                                    if (measTaskBandWidth.MeasFreqParam != null)
                                    {
                                        SaveMeasFreqParam(value.Id.Value, measTaskBandWidth.MeasFreqParam, scope);
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
                    isSuccess = true;
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return isSuccess;
        }
    }
}


