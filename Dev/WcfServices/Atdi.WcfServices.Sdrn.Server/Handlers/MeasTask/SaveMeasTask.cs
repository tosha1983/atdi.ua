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
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            try
            {
                queryExecuter.BeginTransaction();
                var builderSelectMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().From();
                builderSelectMeasTask.Select(c => c.Id);
                builderSelectMeasTask.Where(c => c.Id, ConditionOperator.Equal, measTask.Id.Value);
                builderSelectMeasTask.Where(c => c.Status, ConditionOperator.IsNotNull);
                queryExecuter.Fetch(builderSelectMeasTask, reader =>
                {
                    while (reader.Read())
                    {
                        var builderSelectMeasSubTask = this._dataLayer.GetBuilder<MD.IMeasSubTask>().From();
                        builderSelectMeasSubTask.Select(c => c.Id);
                        builderSelectMeasSubTask.Where(c => c.MeasTaskId, ConditionOperator.Equal, measTask.Id.Value);
                        queryExecuter.Fetch(builderSelectMeasSubTask, readereasSubTask =>
                        {
                            while (readereasSubTask.Read())
                            {
                                var id = readereasSubTask.GetValue(c => c.Id);
                                if (measTask.MeasSubTasks != null)
                                {
                                    var msSubTask = measTask.MeasSubTasks.ToList().Find(t => t.Id.Value == id);
                                    if (msSubTask != null)
                                    {
                                        var builderSelectMeasSubTaskSta = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>().From();
                                        builderSelectMeasSubTaskSta.Select(c => c.Id);
                                        builderSelectMeasSubTaskSta.Where(c => c.MeasSubTaskId, ConditionOperator.Equal, readereasSubTask.GetValue(c => c.Id));
                                        builderSelectMeasSubTaskSta.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                                        queryExecuter.Fetch(builderSelectMeasSubTaskSta, readereasSubTaskSta =>
                                        {
                                            while (readereasSubTaskSta.Read())
                                            {
                                                var msSubTaskSta = msSubTask.MeasSubTaskStations.ToList().Find(t => t.Id == readereasSubTaskSta.GetValue(c => c.Id));
                                                if (msSubTaskSta != null)
                                                {
                                                    if (msSubTaskSta.TimeNextTask.GetValueOrDefault().Subtract(DateTime.Now).TotalSeconds < 0)
                                                    {
                                                        var builderUpdateMeasSubTaskStaSave = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>().Update();
                                                        builderUpdateMeasSubTaskStaSave.Where(c => c.Id, ConditionOperator.Equal, readereasSubTaskSta.GetValue(c => c.Id));
                                                        builderUpdateMeasSubTaskStaSave.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                                                        builderUpdateMeasSubTaskStaSave.SetValue(c => c.Status, status);
                                                        if (queryExecuter.Execute(builderUpdateMeasSubTaskStaSave) > 0)
                                                        {
                                                            isSuccess = true;
                                                        }
                                                        else
                                                        {
                                                            isSuccess = false;
                                                        }

                                                        var builderUpdateSubTask = this._dataLayer.GetBuilder<MD.IMeasSubTask>().Update();
                                                        builderUpdateSubTask.Where(c => c.Id, ConditionOperator.Equal, readereasSubTask.GetValue(c => c.Id));
                                                        builderUpdateSubTask.SetValue(c => c.Status, status);
                                                        if (queryExecuter.Execute(builderUpdateSubTask) > 0)
                                                        {
                                                            isSuccess = true;
                                                        }
                                                        else
                                                        {
                                                            isSuccess = false;
                                                        }

                                                        var builderUpdateTask = this._dataLayer.GetBuilder<MD.IMeasTask>().Update();
                                                        builderUpdateTask.Where(c => c.Id, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                                                        builderUpdateTask.SetValue(c => c.Status, status);
                                                        if (queryExecuter.Execute(builderUpdateTask) > 0)
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
                                    }
                                }

                            }
                            return true;
                        });
                       
                    }
                    return true;
                });
                queryExecuter.CommitTransaction();
            }
            catch (Exception e)
            {
                queryExecuter.RollbackTransaction();
                this._logger.Exception(Contexts.ThisComponent, e);
                isSuccess = false;
            }
            return isSuccess;
        }



        public int? SaveMeasTaskInDB(MeasTask value)
        {
            int? ID = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            if (value.Id != null)
            {
                try
                {
                    queryExecuter.BeginTransaction();
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
                        //builderInsertMeasTask.SetValue(c => c., value.MeasTimeParamList.Days);
                    }
                    builderInsertMeasTask.Select(c => c.Id);
                    queryExecuter.ExecuteAndFetch(builderInsertMeasTask, readerMeasTask =>
                    {
                        while (readerMeasTask.Read())
                        {
                            ID = readerMeasTask.GetValue(c => c.Id);
                            value.Id.Value = ID.Value;

                        }
                        return true;
                    });
                    if (ID != null)
                    {
                        if (value.MeasDtParam != null)
                        {
                            int valueIdMeasDtParam = -1;
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
                            builderInsertMeasDtParam.SetValue(c => c.MeasTaskId, ID.Value);
                            builderInsertMeasDtParam.Select(c => c.Id);
                            queryExecuter.ExecuteAndFetch(builderInsertMeasDtParam, readerMeasDtParam =>
                            {
                                while (readerMeasDtParam.Read())
                                {
                                    valueIdMeasDtParam = readerMeasDtParam.GetValue(c => c.Id);
                                }
                                return true;
                            });
                        }



                        if (value.MeasLocParams != null)
                        {
                            foreach (var locParam in value.MeasLocParams)
                            {
                                if (locParam.Id != null)
                                {
                                    int valueIdMeasLocationParam = -1;
                                    var builderInsertMeasLocationParam = this._dataLayer.GetBuilder<MD.IMeasLocationParam>().Insert();
                                    builderInsertMeasLocationParam.SetValue(c => c.Asl, locParam.ASL);
                                    builderInsertMeasLocationParam.SetValue(c => c.Lat, locParam.Lat);
                                    builderInsertMeasLocationParam.SetValue(c => c.Lon, locParam.Lon);
                                    builderInsertMeasLocationParam.SetValue(c => c.MaxDist, locParam.MaxDist);
                                    builderInsertMeasLocationParam.SetValue(c => c.MeasTaskId, ID.Value);
                                    builderInsertMeasLocationParam.Select(c => c.Id);
                                    queryExecuter.ExecuteAndFetch(builderInsertMeasLocationParam, readerMeasLocationParam =>
                                    {
                                        while (readerMeasLocationParam.Read())
                                        {
                                            valueIdMeasLocationParam = readerMeasLocationParam.GetValue(c => c.Id);
                                            locParam.Id.Value = valueIdMeasLocationParam;
                                        }
                                        return true;
                                    });
                                }
                            }
                        }
                        if (value.MeasOther != null)
                        {
                            var measOther = value.MeasOther;
                            int valueIdMeasOther = -1;
                            var builderInsertMeasOther = this._dataLayer.GetBuilder<MD.IMeasOther>().Insert();
                            builderInsertMeasOther.SetValue(c => c.LevelMinOccup, measOther.LevelMinOccup);
                            builderInsertMeasOther.SetValue(c => c.Nchenal, measOther.NChenal);
                            builderInsertMeasOther.SetValue(c => c.SwNumber, measOther.SwNumber);
                            builderInsertMeasOther.SetValue(c => c.TypeSpectrumOccupation, measOther.TypeSpectrumOccupation.ToString());
                            builderInsertMeasOther.SetValue(c => c.TypeSpectrumscan, measOther.TypeSpectrumScan.ToString());
                            builderInsertMeasOther.SetValue(c => c.MeasTaskId, ID.Value);
                            builderInsertMeasOther.Select(c => c.Id);
                            queryExecuter.ExecuteAndFetch(builderInsertMeasOther, readerMeasOther =>
                            {
                                while (readerMeasOther.Read())
                                {
                                    valueIdMeasOther = readerMeasOther.GetValue(c => c.Id);
                                }
                                return true;
                            });
                        }


                        if (value.MeasSubTasks != null)
                        {
                            foreach (var measSubTask in value.MeasSubTasks)
                            {
                                if (measSubTask.Id != null)
                                {
                                    int valueIdmeasSubTask = -1;
                                    var builderInsertMeasSubTask = this._dataLayer.GetBuilder<MD.IMeasSubTask>().Insert();
                                    builderInsertMeasSubTask.SetValue(c => c.Interval, measSubTask.Interval);
                                    builderInsertMeasSubTask.SetValue(c => c.Status, measSubTask.Status);
                                    builderInsertMeasSubTask.SetValue(c => c.TimeStart, measSubTask.TimeStart);
                                    builderInsertMeasSubTask.SetValue(c => c.TimeStop, measSubTask.TimeStop);
                                    builderInsertMeasSubTask.SetValue(c => c.MeasTaskId, ID.Value);
                                    builderInsertMeasSubTask.Select(c => c.Id);
                                    queryExecuter.ExecuteAndFetch(builderInsertMeasSubTask, readerMeasSubTask =>
                                    {
                                        while (readerMeasSubTask.Read())
                                        {
                                            valueIdmeasSubTask = readerMeasSubTask.GetValue(c => c.Id);
                                            measSubTask.Id.Value = valueIdmeasSubTask;
                                        }
                                        return true;
                                    });


                                    if ((measSubTask.MeasSubTaskStations != null) && (valueIdmeasSubTask > -1))
                                    {
                                        foreach (var subTaskStation in measSubTask.MeasSubTaskStations)
                                        {
                                            int valueIdmeasSubTaskSta = -1;
                                            var builderInsertMeasSubTaskSta = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>().Insert();
                                            builderInsertMeasSubTaskSta.SetValue(c => c.Count, subTaskStation.Count);
                                            builderInsertMeasSubTaskSta.SetValue(c => c.Status, subTaskStation.Status);
                                            if (subTaskStation.StationId != null)
                                            {
                                                builderInsertMeasSubTaskSta.SetValue(c => c.SensorId, subTaskStation.StationId.Value);
                                            }
                                            builderInsertMeasSubTaskSta.SetValue(c => c.MeasSubTaskId, valueIdmeasSubTask);
                                            builderInsertMeasSubTaskSta.SetValue(c => c.TimeNextTask, subTaskStation.TimeNextTask);
                                            builderInsertMeasSubTaskSta.Select(c => c.Id);
                                            queryExecuter.ExecuteAndFetch(builderInsertMeasSubTaskSta, readerMeasSubTaskSta =>
                                            {
                                                while (readerMeasSubTaskSta.Read())
                                                {
                                                    valueIdmeasSubTaskSta = readerMeasSubTaskSta.GetValue(c => c.Id);
                                                    subTaskStation.Id = valueIdmeasSubTaskSta;
                                                }
                                                return true;
                                            });
                                        }
                                    }
                                }
                            }
                        }


                        if (value.MeasFreqParam != null)
                        {
                            var freq_param = value.MeasFreqParam;
                            {
                                int? idMeasFreqParam = -1;
                                if (freq_param != null)
                                {
                                    var builderInsertMeasFreqParam = this._dataLayer.GetBuilder<MD.IMeasFreqParam>().Insert();
                                    builderInsertMeasFreqParam.SetValue(c => c.Mode, freq_param.Mode.ToString());
                                    builderInsertMeasFreqParam.SetValue(c => c.Rgl, freq_param.RgL);
                                    builderInsertMeasFreqParam.SetValue(c => c.Rgu, freq_param.RgU);
                                    builderInsertMeasFreqParam.SetValue(c => c.Step, freq_param.Step);
                                    builderInsertMeasFreqParam.SetValue(c => c.MeasTaskId, ID.Value);
                                    builderInsertMeasFreqParam.Select(c => c.Id);
                                    queryExecuter.ExecuteAndFetch(builderInsertMeasFreqParam, readerMeasFreqParam =>
                                    {
                                        while (readerMeasFreqParam.Read())
                                        {
                                            idMeasFreqParam = readerMeasFreqParam.GetValue(c => c.Id);
                                        }
                                        return true;
                                    });
                                }

                                if ((freq_param.MeasFreqs != null) && (idMeasFreqParam > -1))
                                {
                                    var lstIns = new IQueryInsertStatement<MD.IMeasFreq>[freq_param.MeasFreqs.Length];
                                    for (int i = 0; i < freq_param.MeasFreqs.Length; i++)
                                    {
                                        var builderInsertResMeasFreq = this._dataLayer.GetBuilder<MD.IMeasFreq>().Insert();
                                        builderInsertResMeasFreq.SetValue(c => c.Freq, freq_param.MeasFreqs[i].Freq);
                                        builderInsertResMeasFreq.SetValue(c => c.MeasFreqParamId, idMeasFreqParam);
                                        builderInsertResMeasFreq.Select(c => c.Id);
                                        lstIns[i] = builderInsertResMeasFreq;

                                    }
                                    queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                    {
                                        return true;
                                    });
                                }
                            }
                        }


                        if (value.Stations != null)
                        {
                            var lstIns = new IQueryInsertStatement<MD.IMeasStation>[value.Stations.Length];
                            for (int i = 0; i < value.Stations.Length; i++)
                            {
                                if (value.Stations[i].StationId != null)
                                {
                                    var builderInsertMeasStation = this._dataLayer.GetBuilder<MD.IMeasStation>().Insert();
                                    builderInsertMeasStation.SetValue(c => c.StationType, value.Stations[i].StationType);
                                    builderInsertMeasStation.SetValue(c => c.StationId, value.Stations[i].StationId.Value);
                                    builderInsertMeasStation.SetValue(c => c.MeasTaskId, ID.Value);
                                    builderInsertMeasStation.Select(c => c.Id);
                                    lstIns[i] = builderInsertMeasStation;
                                }

                            }
                            queryExecuter.ExecuteAndFetch(lstIns, reader =>
                            {
                                return true;
                            });
                        }

                        if (value.StationsForMeasurements != null)
                        {
                            foreach (var stationDataParam in value.StationsForMeasurements)
                            {

                                int? idstationDataParam = -1;
                                int? idOwnerdata = -1;
                                int? idSite = -1;

                                if (stationDataParam.Owner != null)
                                {
                                    var builderInsertOwnerData = this._dataLayer.GetBuilder<MD.IOwnerData>().Insert();
                                    builderInsertOwnerData.SetValue(c => c.Address, stationDataParam.Owner.Addres);
                                    builderInsertOwnerData.SetValue(c => c.CODE, stationDataParam.Owner.Code);
                                    builderInsertOwnerData.SetValue(c => c.OKPO, stationDataParam.Owner.OKPO);
                                    builderInsertOwnerData.SetValue(c => c.OwnerName, stationDataParam.Owner.OwnerName);
                                    builderInsertOwnerData.SetValue(c => c.ZIP, stationDataParam.Owner.Zip);
                                    builderInsertOwnerData.Select(c => c.Id);
                                    queryExecuter.ExecuteAndFetch(builderInsertOwnerData, readerOwnerData =>
                                    {
                                        while (readerOwnerData.Read())
                                        {
                                            idOwnerdata = readerOwnerData.GetValue(c => c.Id);
                                        }
                                        return true;
                                    });
                                }

                                if (stationDataParam.Site != null)
                                {
                                    var builderInsertStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().Insert();
                                    builderInsertStationSite.SetValue(c => c.Address, stationDataParam.Site.Adress);
                                    builderInsertStationSite.SetValue(c => c.Lat, stationDataParam.Site.Lat);
                                    builderInsertStationSite.SetValue(c => c.Lon, stationDataParam.Site.Lon);
                                    builderInsertStationSite.SetValue(c => c.Region, stationDataParam.Site.Region);
                                    builderInsertStationSite.Select(c => c.Id);
                                    queryExecuter.ExecuteAndFetch(builderInsertStationSite, readerStationSite =>
                                    {
                                        while (readerStationSite.Read())
                                        {
                                            idSite = readerStationSite.GetValue(c => c.Id);
                                        }
                                        return true;
                                    });
                                }

                                var builderInsertStation = this._dataLayer.GetBuilder<MD.IStation>().Insert();
                                builderInsertStation.SetValue(c => c.GlobalSID, stationDataParam.GlobalSID);
                                builderInsertStation.SetValue(c => c.Standart, stationDataParam.Standart);
                                builderInsertStation.SetValue(c => c.Status, stationDataParam.Status);
                                builderInsertStation.SetValue(c => c.StationId, stationDataParam.IdStation);
                                builderInsertStation.SetValue(c => c.MeasTaskId, ID.Value);
                                if (stationDataParam.LicenseParameter != null)
                                {
                                    builderInsertStation.SetValue(c => c.CloseDate, stationDataParam.LicenseParameter.CloseDate);
                                    builderInsertStation.SetValue(c => c.DozvilName, stationDataParam.LicenseParameter.DozvilName);
                                    builderInsertStation.SetValue(c => c.EndDate, stationDataParam.LicenseParameter.EndDate);
                                    builderInsertStation.SetValue(c => c.StartDate, stationDataParam.LicenseParameter.StartDate);
                                }
                                if (idSite > -1)
                                {
                                    builderInsertStation.SetValue(c => c.StationSiteId, idSite);
                                }
                                if (idOwnerdata > -1)
                                {
                                    builderInsertStation.SetValue(c => c.OwnerDataId, idOwnerdata);
                                }
                                builderInsertStation.Select(c => c.Id);
                                queryExecuter.ExecuteAndFetch(builderInsertStation, readerStation =>
                                {
                                    while (readerStation.Read())
                                    {
                                        idstationDataParam = readerStation.GetValue(c => c.Id);
                                    }
                                    return true;
                                });

                                if (idstationDataParam > -1)
                                {
                                    int? idLinkMeasStation = -1;
                                    var builderInsertLinkMeasStation = this._dataLayer.GetBuilder<MD.ILinkMeasStation>().Insert();
                                    builderInsertLinkMeasStation.SetValue(c => c.StationId, idstationDataParam);
                                    builderInsertLinkMeasStation.SetValue(c => c.MeasTaskId, ID.Value);
                                    builderInsertLinkMeasStation.Select(c => c.Id);
                                    queryExecuter.ExecuteAndFetch(builderInsertLinkMeasStation, readerLinkMeasStation =>
                                    {
                                        while (readerLinkMeasStation.Read())
                                        {
                                            idLinkMeasStation = readerLinkMeasStation.GetValue(c => c.Id);
                                        }
                                        return true;
                                    });

                                }

                                if (stationDataParam.Sectors != null)
                                {
                                    foreach (var sector in stationDataParam.Sectors)
                                    {
                                        int? idSecForMeas = -1;
                                        var builderInsertSector = this._dataLayer.GetBuilder<MD.ISector>().Insert();
                                        builderInsertSector.SetValue(c => c.Agl, sector.AGL);
                                        builderInsertSector.SetValue(c => c.Azimut, sector.Azimut);
                                        builderInsertSector.SetValue(c => c.Bw, sector.BW);
                                        builderInsertSector.SetValue(c => c.ClassEmission, sector.ClassEmission);
                                        builderInsertSector.SetValue(c => c.Eirp, sector.EIRP);
                                        builderInsertSector.SetValue(c => c.SectorId, sector.IdSector);
                                        builderInsertSector.SetValue(c => c.StationId, idstationDataParam);
                                        builderInsertSector.Select(c => c.Id);
                                        queryExecuter.ExecuteAndFetch(builderInsertSector, readerSector =>
                                        {
                                            while (readerSector.Read())
                                            {
                                                idSecForMeas = readerSector.GetValue(c => c.Id);
                                            }
                                            return true;
                                        });


                                        if (sector.Frequencies != null)
                                        {
                                            var lstInsLinkSectorFreq = new List<IQueryInsertStatement<MD.ILinkSectorFreq>>();
                                            foreach (var freq in sector.Frequencies)
                                            {
                                                int? idSectorFreq = null;
                                                var builderInsertSectorFreq = this._dataLayer.GetBuilder<MD.ISectorFreq>().Insert();
                                                builderInsertSectorFreq.SetValue(c => c.ChannelNumber, freq.ChannalNumber);
                                                builderInsertSectorFreq.SetValue(c => c.Frequency, (double?)freq.Frequency);
                                                builderInsertSectorFreq.SetValue(c => c.PlanId, freq.IdPlan);
                                                builderInsertSectorFreq.Select(c => c.Id);
                                                queryExecuter.ExecuteAndFetch(builderInsertSectorFreq, readerSectorFreq =>
                                                {
                                                    while (readerSectorFreq.Read())
                                                    {
                                                        idSectorFreq = readerSectorFreq.GetValue(c => c.Id);
                                                    }
                                                    return true;
                                                });


                                                if ((idSectorFreq != null) && (idSecForMeas != null))
                                                {
                                                    var builderInsertLinkSectorFreq = this._dataLayer.GetBuilder<MD.ILinkSectorFreq>().Insert();
                                                    builderInsertLinkSectorFreq.SetValue(c => c.SectorFreqId, idSectorFreq);
                                                    builderInsertLinkSectorFreq.SetValue(c => c.SectorId, idSecForMeas);
                                                    builderInsertLinkSectorFreq.Select(c => c.Id);
                                                    lstInsLinkSectorFreq.Add(builderInsertLinkSectorFreq);
                                                }

                                            }
                                            if (lstInsLinkSectorFreq.Count > 0)
                                            {
                                                queryExecuter.ExecuteAndFetch(lstInsLinkSectorFreq.ToArray(), reader =>
                                                {
                                                    return true;
                                                });
                                            }
                                        }


                                        if (sector.MaskBW != null)
                                        {
                                            var lstInsLinkSectorMaskElement = new List<IQueryInsertStatement<MD.ILinkSectorMaskElement>>();
                                            foreach (var maskBw in sector.MaskBW)
                                            {
                                                int? sectorMaskElemId = -1;
                                                var builderInsertSectorMaskElement = this._dataLayer.GetBuilder<MD.ISectorMaskElement>().Insert();
                                                builderInsertSectorMaskElement.SetValue(c => c.Level, maskBw.level);
                                                builderInsertSectorMaskElement.SetValue(c => c.Bw, maskBw.BW);
                                                builderInsertSectorMaskElement.Select(c => c.Id);
                                                queryExecuter.ExecuteAndFetch(builderInsertSectorMaskElement, readerSectorMaskElement =>
                                                {
                                                    while (readerSectorMaskElement.Read())
                                                    {
                                                        sectorMaskElemId = readerSectorMaskElement.GetValue(c => c.Id);
                                                    }
                                                    return true;
                                                });

                                                if ((sectorMaskElemId != null) && (idSecForMeas != null))
                                                {
                                                    var builderInsertLinkSectorMaskElement = this._dataLayer.GetBuilder<MD.ILinkSectorMaskElement>().Insert();
                                                    builderInsertLinkSectorMaskElement.SetValue(c => c.SectorMaskElementId, sectorMaskElemId);
                                                    builderInsertLinkSectorMaskElement.SetValue(c => c.SectorId, idSecForMeas);
                                                    builderInsertLinkSectorMaskElement.Select(c => c.Id);
                                                    lstInsLinkSectorMaskElement.Add(builderInsertLinkSectorMaskElement);
                                                }
                                            }
                                            if (lstInsLinkSectorMaskElement.Count > 0)
                                            {
                                                queryExecuter.ExecuteAndFetch(lstInsLinkSectorMaskElement.ToArray(), reader =>
                                                {
                                                    return true;
                                                });
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    queryExecuter.CommitTransaction();
                }
                catch (Exception e)
                {
                    queryExecuter.RollbackTransaction();
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return ID;
        }

    }
}

