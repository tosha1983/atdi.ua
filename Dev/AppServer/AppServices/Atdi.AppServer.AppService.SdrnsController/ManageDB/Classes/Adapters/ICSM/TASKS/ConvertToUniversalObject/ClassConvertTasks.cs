using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Oracle.DataAccess;
using Atdi.AppServer;
using System.Data.Common;

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
                            if (Enum.TryParse<MeasurementMode>(r_dt.m_detecttype, out out_res_MeasurementMode))
                                dtx.Mode = out_res_MeasurementMode;

                            dtx.Preamplification = r_dt.m_preamplification.Value;
                            dtx.RBW = r_dt.m_rbw;
                            dtx.RfAttenuation = r_dt.m_rfattenuation.Value;
                            MeasurementType out_res_MeasurementType;
                            if (Enum.TryParse<MeasurementType>(r_dt.m_detecttype, out out_res_MeasurementType))
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

                    if (obj.MeasTimeParamList.valc != null)
                    {
                        s_out.MeasTimeParamList = new MeasTimeParamList();
                        YXbsMeastimeparaml r_dt = obj.MeasTimeParamList;
                        {
                            MeasTimeParamList time_param_list = new MeasTimeParamList();
                            time_param_list.Days = r_dt.m_days;
                            time_param_list.PerInterval = r_dt.m_perinterval;
                            time_param_list.PerStart = (DateTime)r_dt.m_perstart;
                            time_param_list.PerStop = (DateTime)r_dt.m_perstop;
                            time_param_list.TimeStart = r_dt.m_timestart;
                            time_param_list.TimeStop = r_dt.m_timestop;
                            s_out.MeasTimeParamList = time_param_list;
                        }
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
                            /*
                            List<StationDataForMeasurements> LStationDataForMeasurements = new List<StationDataForMeasurements>();
                            foreach (YXbsStationdatform r_dt in obj.XbsStationdatform.ToArray())
                            {
                                OracleDataAccess oracleDataAccess = new OracleDataAccess();
                                string[] aliasNames = new string[] { "STATIONDATFORM_ID", "STATIONDATFORM_GLOBALSID",  "STATIONDATFORM_STATUS", "STATIONDATFORM_STANDART", "STATIONDATFORM_ID_XBS_MEASTASK",
                                                                 "PERMASSIGN_ID","PERMASSIGN_STARTDATE", "PERMASSIGN_ENDDATE", "PERMASSIGN_CLOSEDATE",  "PERMASSIGN_DOZVILNAME",  "PERMASSIGN_ID_STATIONDATFORM",
                                                                 "SITESTFORM_ID", "SITESTFORM_LON", "SITESTFORM_LAT", "SITESTFORM_ADDRESS", "SITESTFORM_REGION", "SITESTFORM_ID_STATIONDATFORM",
                                                                 "SECTST_ID", "SECTST_AGL", "SECTST_EIRP", "SECTST_AZIMUT", "SECTST_BW", "SECTST_CLASSEMISSION", "SECTST_ID_STATIONDATFORM",
                                                                 "FREQ_ID", "FREQ_IDPLAN", "FREQ_CHANNALNUMBER", "FREQ_FREQUENCY", "FREQ_ID_SECTSTFORMEAS",
                                                                 "MASKEL_ID", "\"LEVEL\"", "MASKEL_BW"," MASKEL_ID_SECTSTFORMEAS",
                                                                 "OWNER_ID" ,"OWNER_OWNERNAME","OWNER_ADDRES","OWNER_CODE","OWNER_ID_STATIONDATFORM","OWNER_OKPO","OWNER_ZIP"};
                                string SQL = string.Format("SELECT STATIONDATFORM.ID as STATIONDATFORM_ID, STATIONDATFORM.GLOBALSID as STATIONDATFORM_GLOBALSID, STATIONDATFORM.STATUS as STATIONDATFORM_STATUS, STATIONDATFORM.STANDART as STATIONDATFORM_STANDART, STATIONDATFORM.ID_XBS_MEASTASK as STATIONDATFORM_ID_XBS_MEASTASK," +
                                                 "PERMASSIGN.ID as PERMASSIGN_ID, PERMASSIGN.STARTDATE as PERMASSIGN_STARTDATE, PERMASSIGN.ENDDATE as PERMASSIGN_ENDDATE, PERMASSIGN.CLOSEDATE as PERMASSIGN_CLOSEDATE, PERMASSIGN.DOZVILNAME as PERMASSIGN_DOZVILNAME, PERMASSIGN.ID_STATIONDATFORM as PERMASSIGN_ID_STATIONDATFORM, " +
                                                 "SITESTFORM.ID as SITESTFORM_ID, SITESTFORM.LON as SITESTFORM_LON, SITESTFORM.LAT as SITESTFORM_LAT, SITESTFORM.ADDRES as SITESTFORM_ADDRESS, SITESTFORM.REGION as SITESTFORM_REGION, SITESTFORM.ID_STATIONDATFORM AS SITESTFORM_ID_STATIONDATFORM, " +
                                                 "SECTST.ID as SECTST_ID,SECTST.AGL as SECTST_AGL,SECTST.EIRP as SECTST_EIRP,SECTST.AZIMUT as SECTST_AZIMUT,SECTST.BW as SECTST_BW,SECTST.CLASSEMISSION as SECTST_CLASSEMISSION,SECTST.ID_STATIONDATFORM as SECTST_ID_STATIONDATFORM, " +
                                                 "FREQ.ID AS FREQ_ID,FREQ.IDPLAN AS FREQ_IDPLAN,FREQ.CHANNALNUMBER AS FREQ_CHANNALNUMBER,FREQ.FREQUENCY AS FREQ_FREQUENCY,FREQ.ID_SECTSTFORMEAS AS FREQ_ID_SECTSTFORMEAS, " +
                                                 "MASKEL.ID as MASKEL_ID, MASKEL.\"LEVEL\" AS MASKEL_LEVEL, MASKEL.BW AS MASKEL_BW, MASKEL.ID_SECTSTFORMEAS as MASKEL_ID_SECTSTFORMEAS, " +
                                                 "OWNER.ID AS OWNER_ID, OWNER.OWNERNAME AS OWNER_OWNERNAME, OWNER.ADDRES as  OWNER_ADDRES,  OWNER.CODE as OWNER_CODE,  OWNER.ID_STATIONDATFORM as OWNER_ID_STATIONDATFORM, OWNER.OKPO as OWNER_OKPO, OWNER.ZIP as OWNER_ZIP " +
                                                 "FROM ICSM.XBS_STATIONDATFORM STATIONDATFORM " +
                                                 "LEFT OUTER JOIN ICSM.XBS_PERMASSIGN PERMASSIGN ON STATIONDATFORM.ID = PERMASSIGN.ID_STATIONDATFORM " +
                                                 "LEFT OUTER JOIN ICSM.XBS_SITESTFORMEAS SITESTFORM ON STATIONDATFORM.ID = SITESTFORM.ID_STATIONDATFORM " +
                                                 "LEFT OUTER JOIN ICSM.XBS_SECTSTFORMEAS SECTST ON STATIONDATFORM.ID = SECTST.ID_STATIONDATFORM " +
                                                 "LEFT OUTER JOIN ICSM.XBS_FREQFORSECTICS FREQ ON SECTST.ID = FREQ.ID_SECTSTFORMEAS " +
                                                 "LEFT OUTER JOIN ICSM.XBS_MASKELEMENTS MASKEL ON SECTST.ID = MASKEL.ID_SECTSTFORMEAS " +
                                                 "LEFT OUTER JOIN ICSM.XBS_OWNERDATA OWNER ON STATIONDATFORM.ID=OWNER.ID_STATIONDATFORM "+
                                                 "WHERE  STATIONDATFORM.ID={0} AND STATIONDATFORM.ID_XBS_MEASTASK={1}", r_dt.m_id, (int)obj.meas_task.m_id);
                                DbDataReader reader = oracleDataAccess.GetValuesSql(SQL);
                                List<SectorStationForMeas> LsectM = new List<SectorStationForMeas>();
                                StationDataForMeasurements MeasStation_ = new StationDataForMeasurements();
                                List<FrequencyForSectorFormICSM> LFreqICSM = new List<FrequencyForSectorFormICSM>();
                                List<MaskElements> L_Mask = new List<MaskElements>();
                                while (reader.Read())
                                {
                                    OwnerData own = new OwnerData();
                                    PermissionForAssignment perm = new PermissionForAssignment();
                                    SiteStationForMeas stM = new SiteStationForMeas();
                                    SectorStationForMeas sectM = new SectorStationForMeas();
                                    FrequencyForSectorFormICSM freqM = new FrequencyForSectorFormICSM();
                                    MaskElements MaskElementsM = new MaskElements();
                                    for (int p = 0; p < aliasNames.Length; p++)
                                    {
                                        if (aliasNames[p].Contains("STATIONDATFORM_"))
                                        {
                                            if (aliasNames[p] == "STATIONDATFORM_ID") MeasStation_.IdStation = (reader[p] as int?) == null ? 0 : (int)(reader[p]);
                                            if (aliasNames[p] == "STATIONDATFORM_GLOBALSID") MeasStation_.GlobalSID = reader[p].ToString();
                                            if (aliasNames[p] == "STATIONDATFORM_STATUS") MeasStation_.Status = reader[p].ToString();
                                            if (aliasNames[p] == "STATIONDATFORM_STANDART") MeasStation_.Standart = reader[p].ToString();
                                        }

                                        if (aliasNames[p].Contains("OWNER_"))
                                        {
                                            if (aliasNames[p] == "OWNER_ADDRES") own.Addres = reader[p].ToString();
                                            if (aliasNames[p] == "OWNER_CODE") own.Code = reader[p].ToString();
                                            if (aliasNames[p] == "OWNER_ID") own.Id = (reader[p] as int?) == null ? 0 : (int)(reader[p]);
                                            if (aliasNames[p] == "OWNER_OKPO") own.OKPO = reader[p].ToString();
                                            if (aliasNames[p] == "OWNER_OWNERNAME") own.OwnerName = reader[p].ToString();
                                            if (aliasNames[p] == "OWNER_ZIP") own.Zip = reader[p].ToString();
                                        }

                                        if (aliasNames[p].Contains("PERMASSIGN_"))
                                        {
                                            if (aliasNames[p] == "PERMASSIGN_CLOSEDATE") perm.CloseDate = reader[p] as DateTime?;
                                            if (aliasNames[p] == "PERMASSIGN_STARTDATE") perm.StartDate = reader[p] as DateTime?;
                                            if (aliasNames[p] == "PERMASSIGN_ENDDATE") perm.EndDate = reader[p] as DateTime?;
                                            if (aliasNames[p] == "PERMASSIGN_DOZVILNAME") perm.DozvilName = reader[p].ToString();
                                            if (aliasNames[p] == "PERMASSIGN_ID") perm.Id = reader[p] as int?;
                                            
                                        }

                                        if (aliasNames[p].Contains("SITESTFORM_"))
                                        {
                                            if (aliasNames[p] == "SITESTFORM_LON") stM.Lon = (reader[p] as double?);
                                            if (aliasNames[p] == "SITESTFORM_LAT") stM.Lat = (reader[p] as double?);
                                            if (aliasNames[p] == "SITESTFORM_ADDRESS") stM.Adress = reader[p].ToString();
                                            if (aliasNames[p] == "SITESTFORM_REGION") stM.Region = reader[p].ToString();
                                        }

                                        if (aliasNames[p].Contains("SECTST_"))
                                        {
                                            if (aliasNames[p] == "SECTST_AGL") sectM.AGL = (reader[p] as double?);
                                            if (aliasNames[p] == "SECTST_EIRP") sectM.EIRP = (reader[p] as double?);
                                            if (aliasNames[p] == "SECTST_AZIMUT") sectM.Azimut = (reader[p] as double?);
                                            if (aliasNames[p] == "SECTST_ID") sectM.IdSector = (reader[p] as int?) == null ? 0 : (int)(reader[p]);
                                            if (aliasNames[p] == "SECTST_BW") sectM.BW = (reader[p] as double?);
                                            if (aliasNames[p] == "SECTST_CLASSEMISSION") sectM.ClassEmission = reader[p].ToString();
                                        }

                                        if (aliasNames[p].Contains("FREQ_"))
                                        {
                                            if (aliasNames[p] == "FREQ_CHANNALNUMBER") freqM.ChannalNumber = reader[p] as int?;
                                            if (aliasNames[p] == "FREQ_FREQUENCY") freqM.Frequency = reader[p] as decimal?;
                                            if (aliasNames[p] == "FREQ_ID") freqM.Id = reader[p] as int?;
                                            if (aliasNames[p] == "FREQ_IDPLAN") freqM.IdPlan = reader[p] as int?;
                                            
                                        }

                                        if (aliasNames[p].Contains("MASKEL_")|| aliasNames[p].Contains("\"LEVEL\""))
                                        {
                                            if (aliasNames[p] == "MASKEL_BW") MaskElementsM.BW = reader[p] as double?;
                                            if (aliasNames[p] == "\"LEVEL\"") MaskElementsM.level = reader[p] as double?;
                                        }
                                       
                                    }

                                    if (freqM.Frequency != null)
                                        LFreqICSM.Add(freqM);

                                    if (MaskElementsM.BW != null || MaskElementsM.level != null)
                                        L_Mask.Add(MaskElementsM);

                                    if (stM.Lon != null || stM.Lat != null)
                                        MeasStation_.Site = stM;

                                    if (perm.CloseDate != null || perm.StartDate != null || perm.EndDate != null || perm.DozvilName != null || perm.Id != null || perm.StartDate != null)
                                        MeasStation_.LicenseParameter = perm;

                                    if (own.Addres != null || own.Code != null || own.Id != null || own.OKPO != null || own.OwnerName != null || own.Zip != null)
                                        MeasStation_.Owner = own;

                                    sectM.MaskBW = L_Mask.ToArray();
                                    sectM.Frequencies = LFreqICSM.ToArray();
                                    LsectM.Add(sectM);
                                    L_Mask = new List<MaskElements>();
                                    LFreqICSM = new List<FrequencyForSectorFormICSM>();
                                    sectM = new SectorStationForMeas();
                                }
                                reader.Close();
                                MeasStation_.Sectors = LsectM.ToArray();
                                LStationDataForMeasurements.Add(MeasStation_);
                            }
                            */
                   
                   
                            List<StationDataForMeasurements> LStationDataForMeasurements = new List<StationDataForMeasurements>();
                            foreach (YXbsStationdatform r_dt in obj.XbsStationdatform.ToArray())
                            {
                                StationDataForMeasurements MeasStation_ = new StationDataForMeasurements();
                                MeasStation_.IdStation = r_dt.m_id.Value;
                                MeasStation_.GlobalSID = r_dt.m_globalsid;
                                MeasStation_.Standart = r_dt.m_standart;
                                MeasStation_.Status = r_dt.m_status;


                                OwnerData own = new OwnerData();
                                YXbsOwnerdata XbsMeasstation_ = new YXbsOwnerdata();
                                XbsMeasstation_.Format("*");
                                XbsMeasstation_.Filter = string.Format("(ID_STATIONDATFORM={0})",  r_dt.m_id);
                                for (XbsMeasstation_.OpenRs(); !XbsMeasstation_.IsEOF(); XbsMeasstation_.MoveNext())
                                {
                                    own.Addres = XbsMeasstation_.m_addres;
                                    own.Code = XbsMeasstation_.m_code;
                                    own.Id = XbsMeasstation_.m_id.Value;
                                    own.OKPO = XbsMeasstation_.m_okpo;
                                    own.OwnerName = XbsMeasstation_.m_ownername;
                                    own.Zip = XbsMeasstation_.m_zip;
                                    MeasStation_.Owner = own;
                                    break;
                                }
                                XbsMeasstation_.Close();
                                XbsMeasstation_.Dispose();


                                PermissionForAssignment perm = new PermissionForAssignment();
                                YXbsPermassign Xbsperm_ = new YXbsPermassign();
                                Xbsperm_.Format("*");
                                Xbsperm_.Filter = string.Format("(ID_STATIONDATFORM={0})", r_dt.m_id);
                                for (Xbsperm_.OpenRs(); !Xbsperm_.IsEOF(); Xbsperm_.MoveNext())
                                {
                                    perm.CloseDate = Xbsperm_.m_closedate;
                                    perm.DozvilName = Xbsperm_.m_dozvilname;
                                    perm.EndDate = Xbsperm_.m_enddate;
                                    perm.Id = Xbsperm_.m_id;
                                    perm.StartDate = Xbsperm_.m_startdate;
                                    MeasStation_.LicenseParameter = perm;
                                    break;
                                }
                                Xbsperm_.Close();
                                Xbsperm_.Dispose();


                                SiteStationForMeas stM = new SiteStationForMeas();
                                YXbsSitestformeas Xbssite_ = new YXbsSitestformeas();
                                Xbssite_.Format("*");
                                Xbssite_.Filter = string.Format("(ID_STATIONDATFORM={0})", r_dt.m_id);
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
                                YXbsSectstformeas Xbssect_ = new YXbsSectstformeas();
                                Xbssect_.Format("*");
                                Xbssect_.Filter = string.Format("(ID_STATIONDATFORM={0})", r_dt.m_id);
                                for (Xbssect_.OpenRs(); !Xbssect_.IsEOF(); Xbssect_.MoveNext())
                                {
                                    SectorStationForMeas sectM = new SectorStationForMeas();

                                    sectM.AGL = Xbssect_.m_agl;
                                    sectM.Azimut = Xbssect_.m_azimut;
                                    sectM.BW = Xbssect_.m_bw;
                                    sectM.ClassEmission = Xbssect_.m_classemission;
                                    sectM.EIRP = Xbssect_.m_eirp;
                                    sectM.IdSector = Xbssect_.m_id.Value;

                                    List<FrequencyForSectorFormICSM> LFreqICSM = new List<FrequencyForSectorFormICSM>();
                                    YXbsFreqforsectics XbsFreqforsectics_ = new YXbsFreqforsectics();
                                    XbsFreqforsectics_.Format("*");
                                    XbsFreqforsectics_.Filter = string.Format("(ID_SECTSTFORMEAS={0})", Xbssect_.m_id);
                                    for (XbsFreqforsectics_.OpenRs(); !XbsFreqforsectics_.IsEOF(); XbsFreqforsectics_.MoveNext())
                                    {
                                        FrequencyForSectorFormICSM freqM = new FrequencyForSectorFormICSM();
                                        freqM.ChannalNumber = XbsFreqforsectics_.m_channalnumber;
                                        freqM.Frequency = (decimal)XbsFreqforsectics_.m_frequency;
                                        freqM.Id = XbsFreqforsectics_.m_id;
                                        freqM.IdPlan = XbsFreqforsectics_.m_idplan;
                                        LFreqICSM.Add(freqM);
                                    }
                                    XbsFreqforsectics_.Close();
                                    XbsFreqforsectics_.Dispose();
                                    sectM.Frequencies = LFreqICSM.ToArray();


                                    List<MaskElements> L_Mask = new List<MaskElements>();
                                    YXbsMaskelements Xbsaskelements_ = new YXbsMaskelements();
                                    Xbsaskelements_.Format("*");
                                    Xbsaskelements_.Filter = string.Format("(ID_SECTSTFORMEAS={0})", Xbssect_.m_id);
                                    for (Xbsaskelements_.OpenRs(); !Xbsaskelements_.IsEOF(); Xbsaskelements_.MoveNext())
                                    {
                                        MaskElements MaskElementsM = new MaskElements();
                                        MaskElementsM.BW = Xbsaskelements_.m_bw;
                                        MaskElementsM.level = Xbsaskelements_.m_level;
                                        L_Mask.Add(MaskElementsM);
                                    }
                                    Xbsaskelements_.Close();
                                    Xbsaskelements_.Dispose();
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
                                if (Enum.TryParse<MeasurementMode>(r_dt.m_detecttype, out out_res_MeasurementMode))
                                    dtx.Mode = out_res_MeasurementMode;

                                dtx.Preamplification = r_dt.m_preamplification.Value;
                                dtx.RBW = r_dt.m_rbw;
                                dtx.RfAttenuation = r_dt.m_rfattenuation.Value;
                                MeasurementType out_res_MeasurementType;
                                if (Enum.TryParse<MeasurementType>(r_dt.m_detecttype, out out_res_MeasurementType))
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

                        if (obj.MeasTimeParamList.valc != null)
                        {
                            s_out.MeasTimeParamList = new MeasTimeParamList();
                            YXbsMeastimeparaml r_dt = obj.MeasTimeParamList;
                            {
                                MeasTimeParamList time_param_list = new MeasTimeParamList();
                                time_param_list.Days = r_dt.m_days;
                                time_param_list.PerInterval = r_dt.m_perinterval;
                                time_param_list.PerStart = (DateTime)r_dt.m_perstart;
                                time_param_list.PerStop = (DateTime)r_dt.m_perstop;
                                time_param_list.TimeStart = r_dt.m_timestart;
                                time_param_list.TimeStop = r_dt.m_timestop;
                                s_out.MeasTimeParamList = time_param_list;
                            }
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
