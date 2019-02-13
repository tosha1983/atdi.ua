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
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;



        public SaveMeasTask(ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._environment = environment;
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
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
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderSelectMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().From();
                builderSelectMeasTask.Select(c => c.Id);
                builderSelectMeasTask.Where(c => c.Id, ConditionOperator.Equal, measTask.Id.Value);
                builderSelectMeasTask.Where(c => c.Status, ConditionOperator.IsNotNull);
                queryExecuter.Fetch(builderSelectMeasTask, reader =>
                {
                    var result = false;
                    while (reader.Read())
                    {
                        var builderSelectMeasSubTask = this._dataLayer.GetBuilder<MD.IMeasSubTask>().From();
                        builderSelectMeasSubTask.Select(c => c.Id);
                        builderSelectMeasSubTask.Where(c => c.MeasTaskId, ConditionOperator.Equal, measTask.Id.Value);
                        queryExecuter.Fetch(builderSelectMeasSubTask, readereasSubTask =>
                        {
                            var resultSubTask = false;
                            while (readereasSubTask.Read())
                            {
                                var id = readereasSubTask.GetValue(c => c.Id);
                                MeasSubTask msSubTask = measTask.MeasSubTasks.ToList().Find(t => t.Id.Value == id);
                                if (msSubTask != null)
                                {
                                    var builderSelectMeasSubTaskSta = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>().From();
                                    builderSelectMeasSubTaskSta.Select(c => c.Id);
                                    builderSelectMeasSubTaskSta.Where(c => c.MeasSubTaskId, ConditionOperator.Equal, readereasSubTask.GetValue(c => c.Id));
                                    builderSelectMeasSubTaskSta.Where(c => c.Status, ConditionOperator.NotEqual, "Z");
                                    queryExecuter.Fetch(builderSelectMeasSubTaskSta, readereasSubTaskSta =>
                                    {
                                        var resultSubTaskSta = false;
                                        while (readereasSubTaskSta.Read())
                                        {
                                            MeasSubTaskStation msSubTaskSta = msSubTask.MeasSubTaskStations.ToList().Find(t => t.Id == readereasSubTaskSta.GetValue(c => c.Id));
                                            if (msSubTaskSta != null)
                                            {
                                                if (msSubTaskSta.TimeNextTask.GetValueOrDefault().Subtract(DateTime.Now).TotalSeconds < 0)
                                                {
                                                    var builderUpdateMeasSubTaskStaSave = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>().Update();
                                                    builderUpdateMeasSubTaskStaSave.Where(c => c.Id, ConditionOperator.Equal, readereasSubTaskSta.GetValue(c => c.Id));
                                                    builderUpdateMeasSubTaskStaSave.Where(c => c.Status, ConditionOperator.NotEqual, "Z");
                                                    builderUpdateMeasSubTaskStaSave.SetValue(c => c.Status, status);
                                                    if (queryExecuter.Execute(builderUpdateMeasSubTaskStaSave) >0)
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
                                        return resultSubTaskSta;
                                    });
                                }

                            }
                            return resultSubTask;
                        });
                       
                    }
                    return result;
                });
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            return isSuccess;
        }


        public int? SaveMeasTaskInDB(MeasTask value)
        {
            int? ID = null;
            if (value.Id != null)
            {

                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderSelectMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().From();
                builderSelectMeasTask.Select(c => c.Id);
                builderSelectMeasTask.Where(c => c.Id, ConditionOperator.Equal, value.Id.Value);
                queryExecuter.Fetch(builderSelectMeasTask, readerMeasTask =>
                {
                    var result = false;
                    while (readerMeasTask.Read())
                    {
                        ID = readerMeasTask.GetValue(c => c.Id);
                        value.Id.Value = ID.Value;
                    }
                    return result;

                });

                if (ID == null)
                {

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
                        var result = false;
                        while (readerMeasTask.Read())
                        {
                            ID = readerMeasTask.GetValue(c => c.Id);
                            value.Id.Value = ID.Value;

                        }
                        return result;

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
                                var resultMeasDtParam = false;
                                while (readerMeasDtParam.Read())
                                {
                                    valueIdMeasDtParam = readerMeasDtParam.GetValue(c => c.Id);
                                }
                                return resultMeasDtParam;
                            });
                        }



                        if (value.MeasLocParams != null)
                        {
                            foreach (MeasLocParam locParam in value.MeasLocParams)
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
                                        var resultMeasLocationParam = false;
                                        while (readerMeasLocationParam.Read())
                                        {
                                            valueIdMeasLocationParam = readerMeasLocationParam.GetValue(c => c.Id);
                                            locParam.Id.Value = valueIdMeasLocationParam;
                                        }
                                        return resultMeasLocationParam;
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
                                var resultMeasOther = false;
                                while (readerMeasOther.Read())
                                {
                                    valueIdMeasOther = readerMeasOther.GetValue(c => c.Id);
                                }
                                return resultMeasOther;
                            });
                        }


                        if (value.MeasSubTasks != null)
                        {
                            foreach (MeasSubTask measSubTask in value.MeasSubTasks)
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
                                        var resultMeasSubTask = false;
                                        while (readerMeasSubTask.Read())
                                        {
                                            valueIdmeasSubTask = readerMeasSubTask.GetValue(c => c.Id);
                                            measSubTask.Id.Value = valueIdmeasSubTask;
                                        }
                                        return resultMeasSubTask;
                                    });


                                    if ((measSubTask.MeasSubTaskStations != null) && (valueIdmeasSubTask > -1))
                                    {
                                        foreach (MeasSubTaskStation subTaskStation in measSubTask.MeasSubTaskStations)
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
                                                var resultMeasSubTaskSta = false;
                                                while (readerMeasSubTaskSta.Read())
                                                {
                                                    valueIdmeasSubTaskSta = readerMeasSubTaskSta.GetValue(c => c.Id);
                                                    subTaskStation.Id = valueIdmeasSubTaskSta;
                                                }
                                                return resultMeasSubTaskSta;
                                            });
                                        }
                                    }
                                }
                            }
                        }



                        /*



                            if (obj.MeasFreqParam != null)
                            {
                                foreach (MeasFreqParam freq_param in new List<MeasFreqParam> { obj.MeasFreqParam })
                                {
                                    int? ID_MeasFreqParam = Constants.NullI;
                                    if (freq_param != null)
                                    {
                                        YXbsMeasfreqparam prm_MeasFreqParam = new YXbsMeasfreqparam();
                                        prm_MeasFreqParam.Format("*");
                                        if (!prm_MeasFreqParam.Fetch(string.Format("ID_XBS_MEASTASK={0}", ID)))
                                        {
                                            prm_MeasFreqParam.New();
                                            prm_MeasFreqParam.m_id_xbs_meastask = ID;
                                            prm_MeasFreqParam.m_mode = freq_param.Mode.ToString();
                                            if (freq_param.RgL != null) prm_MeasFreqParam.m_rgl = freq_param.RgL.GetValueOrDefault();
                                            if (freq_param.RgU != null) prm_MeasFreqParam.m_rgu = freq_param.RgU.GetValueOrDefault();
                                            if (freq_param.Step != null) prm_MeasFreqParam.m_step = freq_param.Step.GetValueOrDefault();
                                            ID_MeasFreqParam = prm_MeasFreqParam.Save(dbConnect, transaction);
                                        }
                                        else
                                        {
                                            ID_MeasFreqParam = prm_MeasFreqParam.m_id;
                                        }
                                        prm_MeasFreqParam.Close();
                                        prm_MeasFreqParam.Dispose();
                                    }

                                    if (freq_param.MeasFreqs != null)
                                    {
                                        List<Yyy> BlockInsert_MeasFreq = new List<Yyy>();
                                        foreach (MeasFreq sub_freq_st in freq_param.MeasFreqs.ToArray())
                                        {
                                            YXbsMeasfreq prm_sub_freq_st = new YXbsMeasfreq();
                                            prm_sub_freq_st.Format("*");
                                            if (sub_freq_st != null)
                                            {
                                                if (!prm_sub_freq_st.Fetch(string.Format("(ID_XBS_MEASFREQPARAM={0}) AND (FREQ={1})", ID_MeasFreqParam, sub_freq_st.Freq.ToString().Replace(",", "."))))
                                                    prm_sub_freq_st.New();

                                                prm_sub_freq_st.m_id_xbs_measfreqparam = ID_MeasFreqParam;
                                                prm_sub_freq_st.m_freq = sub_freq_st.Freq;
                                            }
                                            else
                                            {
                                                prm_sub_freq_st.m_id_xbs_measfreqparam = ID_MeasFreqParam;
                                            }

                                            for (int i = 0; i < prm_sub_freq_st.getAllFields.Count; i++)
                                                prm_sub_freq_st.getAllFields[i].Value = prm_sub_freq_st.valc[i];

                                            BlockInsert_MeasFreq.Add(prm_sub_freq_st);
                                            prm_sub_freq_st.Close();
                                            prm_sub_freq_st.Dispose();
                                        }
                                        if (BlockInsert_MeasFreq.Count > 0)
                                        {
                                            YXbsMeasfreq YXbsMeasfreq11 = new YXbsMeasfreq();
                                            YXbsMeasfreq11.Format("*");
                                            YXbsMeasfreq11.New();
                                            YXbsMeasfreq11.SaveBath(BlockInsert_MeasFreq, dbConnect, transaction);
                                            YXbsMeasfreq11.Close();
                                            YXbsMeasfreq11.Dispose();
                                        }
                                    }
                                }
                            }

                            if (obj.Stations != null)
                            {
                                List<Yyy> BlockInsert_YXbsMeasstation = new List<Yyy>();
                                foreach (MeasStation s_st in obj.Stations.ToArray())
                                {
                                    YXbsMeasstation prm_XbsMeasstation = new YXbsMeasstation();
                                    prm_XbsMeasstation.Format("*");
                                    if (prm_XbsMeasstation != null)
                                    {
                                        if (!prm_XbsMeasstation.Fetch(string.Format("(STATIONID={0}) AND (ID_XBS_MEASTASK={1})", s_st.StationId.Value, ID)))
                                            prm_XbsMeasstation.New();
                                        prm_XbsMeasstation.m_id_xbs_meastask = ID;
                                        prm_XbsMeasstation.m_stationid = s_st.StationId.Value;
                                        prm_XbsMeasstation.m_stationtype = s_st.StationType;
                                    }
                                    else
                                    {
                                        prm_XbsMeasstation.m_id_xbs_meastask = ID;
                                    }

                                    for (int i = 0; i < prm_XbsMeasstation.getAllFields.Count; i++)
                                        prm_XbsMeasstation.getAllFields[i].Value = prm_XbsMeasstation.valc[i];

                                    BlockInsert_YXbsMeasstation.Add(prm_XbsMeasstation);
                                    prm_XbsMeasstation.Close();
                                    prm_XbsMeasstation.Dispose();
                                }
                                if (BlockInsert_YXbsMeasstation.Count > 0)
                                {
                                    YXbsMeasstation YXbsMeasstation11 = new YXbsMeasstation();
                                    YXbsMeasstation11.Format("*");
                                    YXbsMeasstation11.New();
                                    YXbsMeasstation11.SaveBath(BlockInsert_YXbsMeasstation, dbConnect, transaction);
                                    YXbsMeasstation11.Close();
                                    YXbsMeasstation11.Dispose();
                                }
                            }

                            if (obj.StationsForMeasurements != null)
                            {
                                foreach (StationDataForMeasurements StationData_param in obj.StationsForMeasurements.ToArray())
                                {
                                    int? ID_loc_params = -1;
                                    YXbsStation prm_loc = new YXbsStation();
                                    prm_loc.Format("*");

                                    prm_loc.New();
                                    if (StationData_param.GlobalSID != null) prm_loc.m_globalsid = StationData_param.GlobalSID;
                                    if (StationData_param.Standart != null) prm_loc.m_standart = StationData_param.Standart;
                                    if (StationData_param.Status != null) prm_loc.m_status = StationData_param.Status;
                                    if (StationData_param.IdStation != Constants.NullI) prm_loc.m_id_station = StationData_param.IdStation;
                                    prm_loc.m_id_xbs_meastask = ID;
                                    if (StationData_param.LicenseParameter != null)
                                    {
                                        if (StationData_param.LicenseParameter.CloseDate != null) prm_loc.m_closedate = StationData_param.LicenseParameter.CloseDate.GetValueOrDefault();
                                        if (StationData_param.LicenseParameter.DozvilName != null) prm_loc.m_dozvilname = StationData_param.LicenseParameter.DozvilName;
                                        if (StationData_param.LicenseParameter.EndDate != null) prm_loc.m_enddate = StationData_param.LicenseParameter.EndDate.GetValueOrDefault();
                                        if (StationData_param.LicenseParameter.StartDate != null) prm_loc.m_startdate = StationData_param.LicenseParameter.StartDate.GetValueOrDefault();
                                    }
                                    int? ID_Ownerdata = null;
                                    int? ID_site_formeas = null;
                                    if (StationData_param.Owner != null)
                                    {
                                        YXbsOwnerdata owner_formeas = new YXbsOwnerdata();
                                        owner_formeas.Format("*");
                                        //if (!owner_formeas.Fetch(string.Format("(ADDRES='{0}') AND (CODE='{1}') AND (OKPO='{2}') AND (OWNERNAME='{3}' AND (ZIP='{4}'))", StationData_param.Owner.Addres, StationData_param.Owner.Code, StationData_param.Owner.OKPO, StationData_param.Owner.OwnerName, StationData_param.Owner.Zip)))
                                        //{
                                        owner_formeas.New();
                                        if (StationData_param.Owner.Addres != null) owner_formeas.m_addres = StationData_param.Owner.Addres;
                                        if (StationData_param.Owner.Code != null) owner_formeas.m_code = StationData_param.Owner.Code;
                                        if (StationData_param.Owner.OKPO != null) owner_formeas.m_okpo = StationData_param.Owner.OKPO;
                                        if (StationData_param.Owner.OwnerName != null) owner_formeas.m_ownername = StationData_param.Owner.OwnerName;
                                        if (StationData_param.Owner.Zip != null) owner_formeas.m_zip = StationData_param.Owner.Zip;
                                        ID_Ownerdata = owner_formeas.Save(dbConnect, transaction);
                                        //}
                                        owner_formeas.Close();
                                        owner_formeas.Dispose();
                                    }
                                    if (StationData_param.Site != null)
                                    {
                                        YXbsStationSite site_formeas = new YXbsStationSite();
                                        site_formeas.Format("*");
                                        //if (!site_formeas.Fetch(string.Format("(REGION='{0}') AND (ADDRES='{1}') AND (LAT={2}) AND (LON={3})", StationData_param.Site.Region, StationData_param.Site.Adress, ID, StationData_param.Site.Lat.GetValueOrDefault().ToString().Replace(",", "."), StationData_param.Site.Lon.GetValueOrDefault().ToString().Replace(",", "."))))
                                        //{
                                        site_formeas.New();
                                        if (StationData_param.Site.Lat != null) site_formeas.m_lat = StationData_param.Site.Lat.GetValueOrDefault();
                                        if (StationData_param.Site.Lon != null) site_formeas.m_lon = StationData_param.Site.Lon.GetValueOrDefault();
                                        if (StationData_param.Site.Region != null) site_formeas.m_region = StationData_param.Site.Region;
                                        if (StationData_param.Site.Adress != null) site_formeas.m_addres = StationData_param.Site.Adress;

                                        ID_site_formeas = site_formeas.Save(dbConnect, transaction);

                                        site_formeas.Close();
                                        site_formeas.Dispose();
                                        //}
                                    }
                                    prm_loc.m_id_xbs_stationsite = ID_site_formeas;
                                    prm_loc.m_id_xbs_ownerdata = ID_Ownerdata;

                                    ID_loc_params = prm_loc.Save(dbConnect, transaction);
                                    prm_loc.Close();
                                    prm_loc.Dispose();

                                    if (ID_loc_params > 0)
                                    {
                                        YXbsLinkMeasStation yXbsLinkMeasStation = new YXbsLinkMeasStation();
                                        yXbsLinkMeasStation.Format("*");
                                        yXbsLinkMeasStation.New();
                                        yXbsLinkMeasStation.m_id_xbs_station = ID_loc_params;
                                        yXbsLinkMeasStation.m_id_xbs_meastask = ID;
                                        yXbsLinkMeasStation.Save(dbConnect, transaction);
                                        yXbsLinkMeasStation.Close();
                                        yXbsLinkMeasStation.Dispose();
                                    }

                                    if (StationData_param.Sectors != null)
                                    {
                                        foreach (SectorStationForMeas sec in StationData_param.Sectors.ToArray())
                                        {
                                            YXbsSector sect_formeas = new YXbsSector();
                                            sect_formeas.Format("*");
                                            sect_formeas.New();
                                            if (sec.AGL != null) sect_formeas.m_agl = sec.AGL.GetValueOrDefault();
                                            if (sec.Azimut != null) sect_formeas.m_azimut = sec.Azimut.GetValueOrDefault();
                                            if (sec.BW != null) sect_formeas.m_bw = sec.BW.GetValueOrDefault();
                                            if (sec.ClassEmission != null) sect_formeas.m_classemission = sec.ClassEmission;
                                            if (sec.EIRP != null) sect_formeas.m_eirp = sec.EIRP.GetValueOrDefault();
                                            sect_formeas.m_idsector = sec.IdSector;
                                            sect_formeas.m_id_xbs_station = ID_loc_params;
                                            int? ID_secformeas = sect_formeas.Save(dbConnect, transaction);
                                            sect_formeas.Close();
                                            sect_formeas.Dispose();

                                            if (sec.Frequencies != null)
                                            {
                                                List<Yyy> BlockInsert = new List<Yyy>();
                                                foreach (FrequencyForSectorFormICSM F in sec.Frequencies)
                                                {
                                                    int? ID_sectorfreq = null;
                                                    List<string> valueKey = new List<string> { F.ChannalNumber.GetValueOrDefault().ToString(), F.IdPlan.GetValueOrDefault().ToString(), F.Frequency.ToString() };
                                                    YXbsSectorFreq freq_formeas = new YXbsSectorFreq();
                                                    freq_formeas.Format("*");

                                                    //if (!freq_formeas.Fetch(string.Format("(CHANNALNUMBER={0}) AND (IDPLAN={1}) AND (FREQUENCY={2})", F.ChannalNumber.GetValueOrDefault(), F.IdPlan.GetValueOrDefault(), ((double?)F.Frequency).ToString().Replace(",", "."))))
                                                    {
                                                        freq_formeas.New();
                                                        if (F.ChannalNumber != null) freq_formeas.m_channalnumber = F.ChannalNumber.GetValueOrDefault();
                                                        if (F.Frequency != null) freq_formeas.m_frequency = (double?)F.Frequency;
                                                        if (F.IdPlan != null) freq_formeas.m_idplan = F.IdPlan.GetValueOrDefault();
                                                        ID_sectorfreq = freq_formeas.Save(dbConnect, transaction);
                                                    }
                                                    //else
                                                    //{
                                                    //ID_sectorfreq = freq_formeas.m_id;
                                                    //}


                                                    freq_formeas.Close();
                                                    freq_formeas.Dispose();

                                                    if ((ID_sectorfreq != null) && (ID_secformeas != null))
                                                    {
                                                        YXbsLinkSectorFreq yXbsLinkSectorFreq = new YXbsLinkSectorFreq();
                                                        yXbsLinkSectorFreq.Format("*");
                                                        yXbsLinkSectorFreq.New();
                                                        yXbsLinkSectorFreq.m_id_xbs_sectorfreq = ID_sectorfreq;
                                                        yXbsLinkSectorFreq.m_id_xbs_sector = ID_secformeas;

                                                        for (int i = 0; i < yXbsLinkSectorFreq.getAllFields.Count; i++)
                                                            yXbsLinkSectorFreq.getAllFields[i].Value = yXbsLinkSectorFreq.valc[i];
                                                        BlockInsert.Add(yXbsLinkSectorFreq);
                                                        yXbsLinkSectorFreq.Close();
                                                        yXbsLinkSectorFreq.Dispose();
                                                    }

                                                }
                                                if (BlockInsert.Count > 0)
                                                {
                                                    YXbsLinkSectorFreq freq_formeas11 = new YXbsLinkSectorFreq();
                                                    freq_formeas11.Format("*");
                                                    freq_formeas11.New();
                                                    freq_formeas11.SaveBath(BlockInsert, dbConnect, transaction);
                                                    freq_formeas11.Close();
                                                    freq_formeas11.Dispose();
                                                }
                                            }

                                            if (sec.MaskBW != null)
                                            {
                                                List<Yyy> BlockInsert = new List<Yyy>();
                                                foreach (MaskElements F in sec.MaskBW)
                                                {
                                                    int? SectorMaskElemId = null;
                                                    YXbsSectorMaskElem yXbsMaskelements = new YXbsSectorMaskElem();
                                                    yXbsMaskelements.Format("*");
                                                    //if (!yXbsMaskelements.Fetch(string.Format("(BW={0}) AND (LEVEL={1})", F.BW.GetValueOrDefault().ToString().Replace(",", "."), F.level.GetValueOrDefault().ToString().Replace(",", "."))))
                                                    {
                                                        yXbsMaskelements.New();
                                                        if (F.BW != null) yXbsMaskelements.m_bw = F.BW.GetValueOrDefault();
                                                        if (F.level != null) yXbsMaskelements.m_level = F.level.GetValueOrDefault();
                                                        SectorMaskElemId = yXbsMaskelements.Save(dbConnect, transaction);
                                                    }
                                                    //else
                                                    //{
                                                    //SectorMaskElemId = yXbsMaskelements.m_id;
                                                    //}

                                                    yXbsMaskelements.Close();
                                                    yXbsMaskelements.Dispose();

                                                    if ((SectorMaskElemId != null) && (ID_secformeas != null))
                                                    {
                                                        YXbsLinkSectorMask yXbsLinkSectorMask = new YXbsLinkSectorMask();
                                                        yXbsLinkSectorMask.Format("*");
                                                        yXbsLinkSectorMask.New();
                                                        yXbsLinkSectorMask.m_id_sectormaskelem = SectorMaskElemId;
                                                        yXbsLinkSectorMask.m_id_xbs_sector = ID_secformeas;

                                                        for (int i = 0; i < yXbsLinkSectorMask.getAllFields.Count; i++)
                                                            yXbsLinkSectorMask.getAllFields[i].Value = yXbsLinkSectorMask.valc[i];
                                                        BlockInsert.Add(yXbsLinkSectorMask);
                                                        yXbsLinkSectorMask.Close();
                                                        yXbsLinkSectorMask.Dispose();
                                                    }
                                                }
                                                if (BlockInsert.Count > 0)
                                                {
                                                    YXbsLinkSectorMask freq_formeas11 = new YXbsLinkSectorMask();
                                                    freq_formeas11.Format("*");
                                                    freq_formeas11.New();
                                                    freq_formeas11.SaveBath(BlockInsert, dbConnect, transaction);
                                                    freq_formeas11.Close();
                                                    freq_formeas11.Dispose();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                   */
                    }
                }
            }
            return ID;
        }

    }
}


