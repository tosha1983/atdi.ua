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
    public class ClassConvertTasks : IDisposable
    {
        public static ILogger logger;
        public ClassConvertTasks(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassConvertTasks()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    
        public MeasTask[] ConvertTo_MEAS_TASKObjects(List<CLASS_TASKS> objs)
        {
            List<MeasTask> L_OUT = new List<MeasTask>();
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(()=>
                {
                logger.Trace("Start procedure ConvertTo_MEAS_TASKObjects...");
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

                            {
                               
                                List<StationDataForMeasurements> LStationDataForMeasurements = new List<StationDataForMeasurements>();
                                foreach (YXbsStation r_dt in obj.XbsStationdatform.ToArray())
                                {
                                    StationDataForMeasurements MeasStation_ = new StationDataForMeasurements();
                                    MeasStation_.IdStation = r_dt.m_id_station.HasValue ? r_dt.m_id_station.Value : -1;
                                    MeasStation_.GlobalSID = r_dt.m_globalsid;
                                    MeasStation_.Standart = r_dt.m_standart;
                                    MeasStation_.Status = r_dt.m_status;


                                    OwnerData own = new OwnerData();
                                    YXbsOwnerdata XbsOwnerdata_ = new YXbsOwnerdata();
                                    XbsOwnerdata_.Format("*");
                                    XbsOwnerdata_.Filter = string.Format("(ID={0})", r_dt.m_id_xbs_ownerdata);
                                    for (XbsOwnerdata_.OpenRs(); !XbsOwnerdata_.IsEOF(); XbsOwnerdata_.MoveNext())
                                    {
                                        own.Addres = XbsOwnerdata_.m_addres;
                                        own.Code = XbsOwnerdata_.m_code;
                                        own.OKPO = XbsOwnerdata_.m_okpo;
                                        own.OwnerName = XbsOwnerdata_.m_ownername;
                                        own.Zip = XbsOwnerdata_.m_zip;
                                        MeasStation_.Owner = own;
                                        break;
                                    }
                                    XbsOwnerdata_.Close();
                                    XbsOwnerdata_.Dispose();


                                    PermissionForAssignment perm = new PermissionForAssignment();
                                    perm.CloseDate = r_dt.m_closedate;
                                    perm.DozvilName = r_dt.m_dozvilname;
                                    perm.EndDate = r_dt.m_enddate;
                                    perm.Id = null;
                                    //perm.Id = r_dt.m_id;
                                    perm.StartDate = r_dt.m_startdate;
                                    MeasStation_.LicenseParameter = perm;
                                     
                                   


                                    SiteStationForMeas stM = new SiteStationForMeas();
                                    YXbsStationSite Xbssite_ = new YXbsStationSite();
                                    Xbssite_.Format("*");
                                    Xbssite_.Filter = string.Format("(ID={0})", r_dt.m_id_xbs_stationsite);
                                    for (Xbssite_.OpenRs(); !Xbssite_.IsEOF(); Xbssite_.MoveNext())
                                    {
                                        stM.Adress = Xbssite_.m_addres;
                                        stM.Lat = Xbssite_.m_lat;
                                        stM.Lon = Xbssite_.m_lon;
                                        stM.Region = Xbssite_.m_region;
                                        MeasStation_.Site = stM;
                                        break;
                                    }
                                    Xbssite_.Close();
                                    Xbssite_.Dispose();


                                    List<SectorStationForMeas> LsectM = new List<SectorStationForMeas>();
                                    YXbsSector Xbssect_ = new YXbsSector();
                                    Xbssect_.Format("*");
                                    Xbssect_.Filter = string.Format("(ID_XBS_STATION={0})", r_dt.m_id);
                                    for (Xbssect_.OpenRs(); !Xbssect_.IsEOF(); Xbssect_.MoveNext())
                                    {
                                        SectorStationForMeas sectM = new SectorStationForMeas();

                                        sectM.AGL = Xbssect_.m_agl;
                                        sectM.Azimut = Xbssect_.m_azimut;
                                        sectM.BW = Xbssect_.m_bw;
                                        sectM.ClassEmission = Xbssect_.m_classemission;
                                        sectM.EIRP = Xbssect_.m_eirp;
                                        sectM.IdSector = Xbssect_.m_idsector.HasValue ? Xbssect_.m_idsector.Value : -1;

                                        List<FrequencyForSectorFormICSM> LFreqICSM = new List<FrequencyForSectorFormICSM>();
                                        YXbsLinkSectorFreq linkSectorFreq = new YXbsLinkSectorFreq();
                                        linkSectorFreq.Format("*");
                                        linkSectorFreq.Filter = string.Format("(ID_XBS_SECTOR={0})", Xbssect_.m_id);
                                        for (linkSectorFreq.OpenRs(); !linkSectorFreq.IsEOF(); linkSectorFreq.MoveNext())
                                        {
                                            YXbsSectorFreq XbsFreqforsectics_ = new YXbsSectorFreq();
                                            XbsFreqforsectics_.Format("*");
                                            XbsFreqforsectics_.Filter = string.Format("(ID={0})", linkSectorFreq.m_id_xbs_sectorfreq);
                                            for (XbsFreqforsectics_.OpenRs(); !XbsFreqforsectics_.IsEOF(); XbsFreqforsectics_.MoveNext())
                                            {
                                                FrequencyForSectorFormICSM freqM = new FrequencyForSectorFormICSM();
                                                freqM.ChannalNumber = XbsFreqforsectics_.m_channalnumber;
                                                freqM.Frequency = (decimal)XbsFreqforsectics_.m_frequency;
                                                freqM.Id = null;
                                                //freqM.Id = XbsFreqforsectics_.m_id;
                                                freqM.IdPlan = XbsFreqforsectics_.m_idplan;
                                                LFreqICSM.Add(freqM);
                                            }
                                            XbsFreqforsectics_.Close();
                                            XbsFreqforsectics_.Dispose();
                                        }
                                        linkSectorFreq.Close();
                                        linkSectorFreq.Dispose();

                                        sectM.Frequencies = LFreqICSM.ToArray();


                                        List<MaskElements> L_Mask = new List<MaskElements>();
                                        YXbsLinkSectorMask yXbsLinkSectorMask = new YXbsLinkSectorMask();
                                        yXbsLinkSectorMask.Format("*");
                                        yXbsLinkSectorMask.Filter = string.Format("(ID_XBS_SECTOR={0})", Xbssect_.m_id);
                                        for (yXbsLinkSectorMask.OpenRs(); !yXbsLinkSectorMask.IsEOF(); yXbsLinkSectorMask.MoveNext())
                                        {
                                            YXbsSectorMaskElem Xbsaskelements_ = new YXbsSectorMaskElem();
                                            Xbsaskelements_.Format("*");
                                            Xbsaskelements_.Filter = string.Format("(ID={0})", yXbsLinkSectorMask.m_id_sectormaskelem);
                                            for (Xbsaskelements_.OpenRs(); !Xbsaskelements_.IsEOF(); Xbsaskelements_.MoveNext())
                                            {
                                                MaskElements MaskElementsM = new MaskElements();
                                                MaskElementsM.BW = Xbsaskelements_.m_bw;
                                                MaskElementsM.level = Xbsaskelements_.m_level;
                                                L_Mask.Add(MaskElementsM);
                                            }
                                            Xbsaskelements_.Close();
                                            Xbsaskelements_.Dispose();
                                        }
                                        sectM.MaskBW = L_Mask.ToArray();

                                        LsectM.Add(sectM);
                                        MeasStation_.Sectors = LsectM.ToArray();


                                     
                                    }
                                    Xbssect_.Close();
                                    Xbssect_.Dispose();
                                    LStationDataForMeasurements.Add(MeasStation_);
                                }
                                s_out.StationsForMeasurements = LStationDataForMeasurements.ToArray();
                            }
                            L_OUT.Add(s_out);
                            logger.Trace("Convert to MEAS_TASK Objects ...");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ConvertTo_MEAS_TASKObjects... " + ex.Message);
                    }
                logger.Trace("End procedure ConvertTo_MEAS_TASKObjects...");
                });
                thread.Start();
                thread.Join();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ConvertTo_MEAS_TASKObjects..." + ex.Message);
            }
            return L_OUT.ToArray();
        }

        public MeasTask[] ConvertToShortMeasTasks(List<CLASS_TASKS> objs)
        {
            List<MeasTask> L_OUT = new List<MeasTask>();
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    logger.Trace("Start procedure ConvertTo_MEAS_TASKObjects...");
                    try { 
                    #region Convert to MeasTask
                    foreach (CLASS_TASKS obj in objs.ToArray())
                    {
                        MeasTask s_out = new MeasTask();
                        s_out.Id = new MeasTaskIdentifier();
                        s_out.Id.Value = obj.meas_task.m_id.HasValue ? obj.meas_task.m_id.Value : -1;
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

                                dtx.IfAttenuation = r_dt.m_ifattenuation.HasValue ? r_dt.m_ifattenuation.Value : 0;
                                dtx.MeasTime = r_dt.m_meastime;
                                MeasurementMode out_res_MeasurementMode;
                                if (Enum.TryParse<MeasurementMode>(r_dt.m_mode, out out_res_MeasurementMode))
                                    dtx.Mode = out_res_MeasurementMode;

                                dtx.Preamplification = r_dt.m_preamplification.HasValue ? r_dt.m_preamplification.Value : -1;
                                dtx.RBW = r_dt.m_rbw;
                                dtx.RfAttenuation = r_dt.m_rfattenuation.HasValue ? r_dt.m_rfattenuation.Value : 0;
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
                        logger.Trace("Convert to MEAS_TASK Objects ...");
                    }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ConvertTo_MEAS_TASKObjects... " + ex.Message);
                    }
                    logger.Trace("End procedure ConvertTo_MEAS_TASKObjects...");
                });
                thread.Start();
                thread.Join();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ConvertTo_MEAS_TASKObjects..." + ex.Message);
            }
            return L_OUT.ToArray();
        }

    }
}
