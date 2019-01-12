using System;
using System.Collections.Generic;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Oracle.DataAccess;
using Atdi.AppServer;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassConvertTasksOptimize : IDisposable
    {
        public static ILogger logger;
        public ClassConvertTasksOptimize(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassConvertTasksOptimize()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public MeasTask[] ConvertToMeastTaskHeader(List<CLASS_TASKS> objs)
        {
            List<MeasTask> L_OUT = new List<MeasTask>();
            try
            {
                //System.Threading.Thread thread = new System.Threading.Thread(() =>
                //{
                    logger.Trace("Start procedure ConvertToMeastTaskHeader...");
                    try
                    {


                        #region Convert to MeasTask
                        foreach (CLASS_TASKS obj in objs.ToArray())
                        {
                            MeasTask s_out = new MeasTask();
                            s_out.Id = new MeasTaskIdentifier();
                            s_out.Id.Value = obj.meas_task.m_id.Value;
                            s_out.Name = obj.meas_task.m_name;
                            s_out.OrderId = obj.meas_task.m_orderid;
                            s_out.Prio = obj.meas_task.m_prio;
                            MeasTaskResultType out_res_type;
                            if (Enum.TryParse<MeasTaskResultType>(obj.meas_task.m_resulttype, out out_res_type))
                                s_out.ResultType = out_res_type;
                            s_out.Status = obj.meas_task.m_status;
                            MeasTaskExecutionMode out_res_execmode;
                            if (Enum.TryParse<MeasTaskExecutionMode>(obj.meas_task.m_executionmode, out out_res_execmode))
                                s_out.ExecutionMode = out_res_execmode;

                            s_out.MaxTimeBs = obj.meas_task.m_maxtimebs;
                            MeasTaskType out_res_meas_type_task;
                            if (Enum.TryParse<MeasTaskType>(obj.meas_task.m_executionmode, out out_res_meas_type_task))
                                s_out.Task = out_res_meas_type_task;
                            s_out.Type = obj.meas_task.m_type;
                            s_out.CreatedBy = obj.meas_task.m_createdby;
                            s_out.DateCreated = obj.meas_task.m_datecreated;
                            s_out.MeasDtParam = new MeasDtParam();
                            {
                                foreach (YXbsMeasdtparam r_dt in obj.MeasDtParam.ToArray())
                                {
                                    MeasDtParam dtx = new MeasDtParam();
                                    dtx.Demod = r_dt.m_demod;
                                    DetectingType out_res_DetectType;
                                    if (Enum.TryParse<DetectingType>(r_dt.m_detecttype, out out_res_DetectType))
                                        dtx.DetectType = out_res_DetectType;

                                    dtx.IfAttenuation = r_dt.m_ifattenuation.Value;
                                    dtx.MeasTime = r_dt.m_meastime;
                                    MeasurementMode out_res_MeasurementMode;
                                    if (Enum.TryParse<MeasurementMode>(r_dt.m_mode, out out_res_MeasurementMode))
                                        dtx.Mode = out_res_MeasurementMode;

                                    dtx.Preamplification = r_dt.m_preamplification.Value;
                                    dtx.RBW = r_dt.m_rbw;
                                    dtx.RfAttenuation = r_dt.m_rfattenuation.Value;
                                    MeasurementType out_res_MeasurementType;
                                    if (Enum.TryParse<MeasurementType>(r_dt.m_typemeasurements, out out_res_MeasurementType))
                                        dtx.TypeMeasurements = out_res_MeasurementType;
                                    dtx.VBW = r_dt.m_vbw;
                                    s_out.MeasDtParam = dtx;
                                }
                            }

                            s_out.MeasFreqParam = new MeasFreqParam();
                            {
                                foreach (KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>> r_dt in obj.MeasFreqLst_param.ToArray())
                                {
                                    MeasFreqParam fre_param = new MeasFreqParam();
                                    FrequencyMode out_res_FrequencyMode;
                                    if (Enum.TryParse<FrequencyMode>(r_dt.Key.m_mode, out out_res_FrequencyMode))
                                        fre_param.Mode = out_res_FrequencyMode;


                                    fre_param.RgL = r_dt.Key.m_rgl;
                                    fre_param.RgU = r_dt.Key.m_rgu;
                                    fre_param.Step = r_dt.Key.m_step;

                                    List<MeasFreq> meas_freq_lst = new List<MeasFreq>();
                                    foreach (YXbsMeasfreq F_lst in r_dt.Value.ToArray())
                                    {
                                        MeasFreq f_l = new MeasFreq();
                                        f_l.Freq = F_lst.m_freq.Value;
                                        meas_freq_lst.Add(f_l);
                                    }
                                    fre_param.MeasFreqs = meas_freq_lst.ToArray();
                                    s_out.MeasFreqParam = fre_param;
                                }
                            }


                            List<MeasSubTask> MsT = new List<MeasSubTask>();
                            {
                                foreach (KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>> r_dt in obj.meas_st.ToArray())
                                {
                                    MeasSubTask SUB = new MeasSubTask();
                                    SUB.Interval = r_dt.Key.m_interval;
                                    SUB.Status = r_dt.Key.m_status;
                                    SUB.TimeStart = (DateTime)r_dt.Key.m_timestart;
                                    SUB.TimeStop = (DateTime)r_dt.Key.m_timestop;
                                    SUB.Id = new MeasTaskIdentifier();
                                    SUB.Id.Value = r_dt.Key.m_id.Value;
                                    List<MeasSubTaskStation> L_MeasSubTaskStations = new List<MeasSubTaskStation>();
                                    foreach (YXbsMeassubtasksta F_lst in r_dt.Value.ToArray())
                                    {
                                        MeasSubTaskStation f_l = new MeasSubTaskStation();
                                        f_l.Count = F_lst.m_count;
                                        f_l.Id = F_lst.m_id.Value;
                                        f_l.StationId = new SensorIdentifier();
                                        f_l.StationId.Value = F_lst.m_id_xbs_sensor.Value;
                                        f_l.Status = F_lst.m_status;
                                        f_l.TimeNextTask = F_lst.m_timenexttask;
                                        L_MeasSubTaskStations.Add(f_l);
                                    }

                                    SUB.MeasSubTaskStations = L_MeasSubTaskStations.ToArray();
                                    MsT.Add(SUB);
                                }
                                s_out.MeasSubTasks = MsT.ToArray();
                            }

                            if (obj.meas_task != null)
                            {
                                s_out.MeasTimeParamList = new MeasTimeParamList();
                                MeasTimeParamList time_param_list = new MeasTimeParamList();
                                time_param_list.Days = obj.meas_task.m_days;
                                time_param_list.PerInterval = obj.meas_task.m_perinterval;
                                time_param_list.PerStart = (DateTime)obj.meas_task.m_perstart;
                                time_param_list.PerStop = (DateTime)obj.meas_task.m_perstop;
                                time_param_list.TimeStart = obj.meas_task.m_timestart;
                                time_param_list.TimeStop = obj.meas_task.m_timestop;
                                s_out.MeasTimeParamList = time_param_list;
                            }

                            if (obj.MeasOther.valc != null)
                            {
                                YXbsMeasother r_dt = obj.MeasOther;
                                MeasOther other = new MeasOther();
                                other.LevelMinOccup = r_dt.m_levelminoccup;
                                other.NChenal = r_dt.m_nchenal;
                                other.SwNumber = r_dt.m_swnumber;

                                SpectrumOccupationType out_res_SpectrumOccupationType;
                                if (Enum.TryParse<SpectrumOccupationType>(obj.MeasOther.m_typespectrumoccupation, out out_res_SpectrumOccupationType))
                                    other.TypeSpectrumOccupation = out_res_SpectrumOccupationType;

                                SpectrumScanType out_res_SpectrumScanType;
                                if (Enum.TryParse<SpectrumScanType>(obj.MeasOther.m_typespectrumscan, out out_res_SpectrumScanType))
                                    other.TypeSpectrumScan = out_res_SpectrumScanType;

                                s_out.MeasOther = other;
                            }


                            List<MeasLocParam> L_Lock = new List<MeasLocParam>();
                            foreach (YXbsMeaslocparam r_dt in obj.MeasLocParam.ToArray())
                            {
                                MeasLocParam LOCk = new MeasLocParam();
                                LOCk.ASL = r_dt.m_asl;
                                LOCk.Lat = r_dt.m_lat;
                                LOCk.Lon = r_dt.m_lon;
                                LOCk.MaxDist = r_dt.m_maxdist;
                                LOCk.Id = new MeasLocParamIdentifier();
                                LOCk.Id.Value = r_dt.m_id.Value;
                                L_Lock.Add(LOCk);
                            }
                            s_out.MeasLocParams = L_Lock.ToArray();

                            if (obj.Stations != null)
                            {
                                List<MeasStation> L_MeasStation = new List<MeasStation>();
                                foreach (YXbsMeasstation r_dt in obj.Stations.ToArray())
                                {
                                    MeasStation MeasStation = new MeasStation();
                                    MeasStation.StationId = new MeasStationIdentifier();
                                    MeasStation.StationId.Value = r_dt.m_stationid.Value;
                                    MeasStation.StationType = r_dt.m_stationtype;
                                    L_MeasStation.Add(MeasStation);
                                }
                                s_out.Stations = L_MeasStation.ToArray();
                            }
                            L_OUT.Add(s_out);
                            logger.Trace("Convert to ConvertToMeastTaskHeader Objects ...");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ConvertToMeastTaskHeader... " + ex.Message);
                    }
                    logger.Trace("End procedure ConvertToMeastTaskHeader...");
                //});
                //thread.Start();
                //thread.Join();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ConvertTo_MEAS_TASKObjects..." + ex.Message);
            }
            return L_OUT.ToArray();
        }


        public StationDataForMeasurements[] ConvertToStationDataForMeasurements(List<CLASS_TASKS> objs)
        {
            const int Cn = 900;
            StationDataForMeasurements[] L_OUT = null;
            try
            {
                //System.Threading.Thread thread = new System.Threading.Thread(() =>
                //{
                    logger.Trace("Start procedure ConvertToStationDataForMeasurements...");
                    try
                    {
                        List<StationDataForMeasurements> LStationData = new List<StationDataForMeasurements>();
                        List<StationDataForMeasurementsExtend> LStationDataForMeasurements = new List<StationDataForMeasurementsExtend>();
                        List<int> sql_XbsOwnerdata_in = new List<int>();
                        List<int> sql_YXbsStationSite_in = new List<int>();

                    
                        foreach (CLASS_TASKS obj in objs.ToArray())
                        {
                            foreach (YXbsStation r_dt in obj.XbsStationdatform.ToArray())
                            {

                                StationDataForMeasurementsExtend MeasStation_ = new StationDataForMeasurementsExtend();
                                MeasStation_.IdStation = r_dt.m_id_station.HasValue ? r_dt.m_id_station.Value : -1;
                                MeasStation_.GlobalSID = r_dt.m_globalsid;
                                MeasStation_.Standart = r_dt.m_standart;
                                MeasStation_.Status = r_dt.m_status;

                                PermissionForAssignment perm = new PermissionForAssignment();
                                perm.CloseDate = r_dt.m_closedate;
                                perm.DozvilName = r_dt.m_dozvilname;
                                perm.EndDate = r_dt.m_enddate;
                                perm.Id = null;
                                perm.StartDate = r_dt.m_startdate;
                                MeasStation_.LicenseParameter = perm;
                                MeasStation_.IdSite = r_dt.m_id_xbs_stationsite.Value;
                                MeasStation_.IdOwner = r_dt.m_id_xbs_ownerdata.Value;

                                LStationDataForMeasurements.Add(MeasStation_);

                                if (!sql_XbsOwnerdata_in.Contains(r_dt.m_id_xbs_ownerdata.Value))
                                {
                                    sql_XbsOwnerdata_in.Add(r_dt.m_id_xbs_ownerdata.Value);
                                }
                                if (!sql_YXbsStationSite_in.Contains(r_dt.m_id_xbs_stationsite.Value))
                                {
                                    sql_YXbsStationSite_in.Add(r_dt.m_id_xbs_stationsite.Value);
                                }
                            }
                          
                        }
                    
                        List<OwnerData> listOwn = new List<OwnerData>();
                        List<int> sql_XbsOwnerdata_in2 = new List<int>();
                        for (int i=0; i< sql_XbsOwnerdata_in.Count; i++)
                        {
                            if (sql_XbsOwnerdata_in2.Count <= Cn)
                            {
                                sql_XbsOwnerdata_in2.Add(sql_XbsOwnerdata_in[i]);
                            }
                            if ((sql_XbsOwnerdata_in2.Count > Cn) || ((i + 1) == sql_XbsOwnerdata_in.Count))
                            {
                                logger.Trace(string.Format("(ID IN ({0}))", string.Join(",", sql_XbsOwnerdata_in2)));

                                YXbsOwnerdata XbsOwnerdata_ = new YXbsOwnerdata();
                                XbsOwnerdata_.Format("*");
                                XbsOwnerdata_.Filter = string.Format("(ID IN ({0}))", string.Join(",", sql_XbsOwnerdata_in2));
                                for (XbsOwnerdata_.OpenRs(); !XbsOwnerdata_.IsEOF(); XbsOwnerdata_.MoveNext())
                                {
                                    var own = new OwnerData();
                                    own.Addres = XbsOwnerdata_.m_addres;
                                    own.Code = XbsOwnerdata_.m_code;
                                    own.OKPO = XbsOwnerdata_.m_okpo;
                                    own.OwnerName = XbsOwnerdata_.m_ownername;
                                    own.Zip = XbsOwnerdata_.m_zip;
                                    own.Id = XbsOwnerdata_.m_id.Value;
                                    listOwn.Add(own);
                                }
                                XbsOwnerdata_.Close();
                                XbsOwnerdata_.Dispose();

                                sql_XbsOwnerdata_in2.Clear();
                            }
                        }
                    
                        List<SiteStationForMeasExtend> listSiteStationForMeas = new List<SiteStationForMeasExtend>();
                        List<int> sql_YXbsStationSite_in2 = new List<int>();
                        for (int i = 0; i < sql_YXbsStationSite_in.Count; i++)
                        {
                            if (sql_YXbsStationSite_in2.Count <= Cn)
                            {
                                sql_YXbsStationSite_in2.Add(sql_YXbsStationSite_in[i]);
                            }
                            if ((sql_YXbsStationSite_in2.Count > Cn) || ((i + 1) == sql_YXbsStationSite_in.Count))
                            {

                                logger.Trace(string.Format("(ID IN ({0}))", string.Join(",", sql_YXbsStationSite_in2)));

                                YXbsStationSite Xbssite_ = new YXbsStationSite();
                                Xbssite_.Format("*");
                                Xbssite_.Filter = string.Format("(ID IN ({0}))", string.Join(",", sql_YXbsStationSite_in2));
                                for (Xbssite_.OpenRs(); !Xbssite_.IsEOF(); Xbssite_.MoveNext())
                                {
                                    var stM = new SiteStationForMeasExtend();
                                    stM.Id = Xbssite_.m_id.Value;
                                    stM.Adress = Xbssite_.m_addres;
                                    stM.Lat = Xbssite_.m_lat;
                                    stM.Lon = Xbssite_.m_lon;
                                    stM.Region = Xbssite_.m_region;
                                    listSiteStationForMeas.Add(stM);
                                }
                                Xbssite_.Close();
                                Xbssite_.Dispose();

                                sql_YXbsStationSite_in2.Clear();
                            }
                        }
                      
                        foreach (StationDataForMeasurementsExtend c in LStationDataForMeasurements)
                        {
                            var fnd_own = listOwn.Find(t => t.Id == c.IdOwner);
                            if (fnd_own != null)
                            {
                                c.Owner = new OwnerData { Id = fnd_own.Id, Addres = fnd_own.Addres, Code = fnd_own.Code, OKPO = fnd_own.OKPO, OwnerName = fnd_own.OwnerName, Zip = fnd_own.Zip };
                            }

                            var fnd_site = listSiteStationForMeas.Find(t => t.Id == c.IdSite);
                            if (fnd_site != null)
                            {
                                c.Site = new SiteStationForMeas { Adress = fnd_site.Adress, Lat = fnd_site.Lat, Lon = fnd_site.Lon, Region = fnd_site.Region };
                            }
                            LStationData.Add(c);
                        }
                    
                        L_OUT = LStationData.ToArray();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ConvertToStationDataForMeasurements... " + ex.Message);
                    }
                    logger.Trace("End procedure ConvertToStationDataForMeasurements...");
                //});
                //thread.Start();
                //thread.Join();

            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ConvertToStationDataForMeasurements..." + ex.Message);
            }
            return L_OUT;
        }

    }
}
