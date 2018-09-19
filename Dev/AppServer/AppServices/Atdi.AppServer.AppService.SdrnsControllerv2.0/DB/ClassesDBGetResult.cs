using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Contracts.Sdrns;
using System.Data.Common;

namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{

    public class ClassesDBGetResult 
    {
        public static ILogger logger;
        public ClassesDBGetResult(ILogger log)
        {
            if (logger == null) logger = log;
        }
      
        


        public int? SaveResultToDB(MeasurementResults obj, Atdi.DataModels.Sdrns.Device.MeasResults api2Result, string taskId)
        {
            //obj.ResultsMeasStation
            int? ID = Constants.NullI;
            //if (((obj.TypeMeasurements == MeasurementType.SpectrumOccupation) && (obj.Status == "C")) || (obj.TypeMeasurements != MeasurementType.SpectrumOccupation))
            {
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                Yyy yyy = new Yyy();
                DbConnection dbConnect = yyy.NewConnection(yyy.GetConnectionString());
                if (dbConnect.State == System.Data.ConnectionState.Open)
                {
                    DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    try
                    {
                        logger.Trace("Start procedure SaveResultToDB.");
                        List<Yyy> BlockInsert_FrequencyMeasurement2 = new List<Yyy>();
                        List<Yyy> BlockInsert_YXbsLevelmeasres1 = new List<Yyy>();
                        List<Yyy> BlockInsert_YXbsSpectoccupmeas1 = new List<Yyy>();
                        /// Create new record in YXbsMeastask
                        if (obj != null)
                        {

                            if ((taskId != null) && (obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != Constants.NullI) && (obj.Id.SubMeasTaskStationId != Constants.NullI))
                            {

                                if (obj.StationMeasurements.StationId != null)
                                {
                                    YXbsMeasurementres measRes = new YXbsMeasurementres();
                                    measRes.Format("*");
                                    measRes.Filter = "ID=-1";
                                    measRes.New();
                                    if (obj.AntVal != null) measRes.m_antval = obj.AntVal.GetValueOrDefault();
                                    if (obj.DataRank != null) measRes.m_datarank = obj.DataRank.GetValueOrDefault();
                                    measRes.m_status = obj.Status;
                                    measRes.m_meastaskid = taskId;
                                    if (obj.N != null) measRes.m_n = obj.N.GetValueOrDefault();
                                    measRes.m_sensorid = obj.StationMeasurements.StationId.Value;
                                    measRes.m_submeastaskid = obj.Id.SubMeasTaskId;
                                    measRes.m_submeastaskstationid = obj.Id.SubMeasTaskStationId;
                                    measRes.m_timemeas = obj.TimeMeas;
                                    measRes.m_typemeasurements = obj.TypeMeasurements.ToString();
                                    ID = measRes.Save(dbConnect, transaction);
                                    obj.Id.MeasSdrResultsId = ID.Value;
                                    measRes.Close();
                                    measRes.Dispose();
                                }
                            }
                            if (ID != Constants.NullI)
                            {

                                //////////////////////////////////////////
                                if (api2Result.Routes != null)
                                {
                                    foreach (DataModels.Sdrns.Device.Route route in api2Result.Routes)
                                    {
                                        if (route.RoutePoints != null)
                                        {
                                            foreach (DataModels.Sdrns.Device.RoutePoint routePoint in route.RoutePoints)
                                            {
                                                YXbsRoutes resRoutes = new YXbsRoutes();
                                                resRoutes.Format("*");
                                                resRoutes.Filter = "ID=-1";
                                                resRoutes.New();
                                                resRoutes.m_agl = routePoint.AGL;
                                                resRoutes.m_asl = routePoint.ASL;
                                                resRoutes.m_finishtime = routePoint.FinishTime;
                                                resRoutes.m_starttime = routePoint.StartTime;
                                                resRoutes.m_routeid = route.RouteId;
                                                resRoutes.m_pointstaytype = routePoint.PointStayType.ToString();
                                                resRoutes.m_lat = routePoint.Lat;
                                                resRoutes.m_lon = routePoint.Lon;
                                                resRoutes.m_idxbsmeasurementres = ID;
                                                resRoutes.Save(dbConnect, transaction);
                                                resRoutes.Close();
                                                resRoutes.Dispose();
                                            }
                                        }
                                    }
                                }

                                /////////////////////////////////////////

                                if (obj.ResultsMeasStation != null)
                                {
                                    foreach (ResultsMeasurementsStation station in obj.ResultsMeasStation)
                                    {
                                        DataModels.Sdrns.Device.StationMeasResult rFinded = new DataModels.Sdrns.Device.StationMeasResult();
                                        YXbsResmeasstation measResStation = new YXbsResmeasstation();
                                        measResStation.Format("*");
                                        measResStation.Filter = "ID=-1";
                                        measResStation.New();
                                        measResStation.m_globalsid = station.GlobalSID;
                                        measResStation.m_idsector = station.IdSector;
                                        measResStation.m_idstation = station.Idstation;
                                        measResStation.m_status = station.Status;
                                        measResStation.m_measglobalsid = station.MeasGlobalSID;
                                        measResStation.m_idxbsmeasurementres = ID;
                                        if (api2Result.StationResults != null)
                                        {
                                                rFinded = api2Result.StationResults.ToList().Find(t => t.StationId == station.Idstation.ToString() && t.SectorId == station.IdSector.ToString() && t.RealGlobalSid == station.MeasGlobalSID && t.TaskGlobalSid == station.GlobalSID);
                                                if (rFinded != null)
                                                {
                                                    measResStation.m_standard = rFinded.Standard;
                                                }
                                         }
        
                                            int? IDStation = measResStation.Save(dbConnect, transaction);
                                            measResStation.Close();
                                            measResStation.Dispose();
                                            if (IDStation > 0)
                                            {
                                                if (station.GeneralResult != null)
                                                {
                                                    YXbsResGeneral measResGeneral = new YXbsResGeneral();
                                                    measResGeneral.Format("*");
                                                    measResGeneral.Filter = "ID=-1";
                                                    measResGeneral.New();
                                                    measResGeneral.m_centralfrequency = station.GeneralResult.CentralFrequency;
                                                    measResGeneral.m_centralfrequencymeas = station.GeneralResult.CentralFrequencyMeas;
                                                    measResGeneral.m_durationmeas = station.GeneralResult.DurationMeas;
                                                    measResGeneral.m_markerindex = station.GeneralResult.MarkerIndex;
                                                    measResGeneral.m_offsetfrequency = station.GeneralResult.OffsetFrequency;
                                                    measResGeneral.m_specrumstartfreq = (double?)station.GeneralResult.SpecrumStartFreq;
                                                    measResGeneral.m_specrumsteps = (double?)station.GeneralResult.SpecrumSteps;
                                                    measResGeneral.m_t1 = station.GeneralResult.T1;
                                                    measResGeneral.m_t2 = station.GeneralResult.T2;
                                                    measResGeneral.m_timefinishmeas = station.GeneralResult.TimeFinishMeas;
                                                    measResGeneral.m_timestartmeasdate = station.GeneralResult.TimeStartMeas;
                                                    measResGeneral.m_resultsmeasstationid = IDStation;

                                                    if (rFinded != null)
                                                    {
                                                        if (rFinded.GeneralResult != null)
                                                        {
                                                            if (rFinded.GeneralResult.BandwidthResult != null)
                                                            {
                                                                measResGeneral.m_tracecount = rFinded.GeneralResult.BandwidthResult.TraceCount;
                                                                measResGeneral.m_сorrectnessestim = rFinded.GeneralResult.BandwidthResult.СorrectnessEstimations == true ? 1 : 0;
                                                            }
                                                        }
                                                    }

                                                    int? IDResGeneral = measResGeneral.Save(dbConnect, transaction);
                                                    measResGeneral.Close();
                                                    measResGeneral.Dispose();

                                                    if (IDResGeneral > 0)
                                                    {

                                                        //////////////////////////////////////////
                                                        if (api2Result.StationResults != null)
                                                        {
                                                            foreach (DataModels.Sdrns.Device.StationMeasResult r in api2Result.StationResults)
                                                            {

                                                                if (r.GeneralResult.StationSysInfo != null)
                                                                {

                                                                    YXbsSysInfo resSysInfo = new YXbsSysInfo();
                                                                    resSysInfo.Format("*");
                                                                    resSysInfo.Filter = "ID=-1";
                                                                    resSysInfo.New();

                                                                    if (r.GeneralResult.StationSysInfo.Location != null)
                                                                    {
                                                                        resSysInfo.m_AGL = r.GeneralResult.StationSysInfo.Location.AGL;
                                                                        resSysInfo.m_ASL = r.GeneralResult.StationSysInfo.Location.ASL;
                                                                        resSysInfo.m_Lat = r.GeneralResult.StationSysInfo.Location.Lat;
                                                                        resSysInfo.m_Lon = r.GeneralResult.StationSysInfo.Location.Lon;
                                                                    }


                                                                    resSysInfo.m_BandWidth = r.GeneralResult.StationSysInfo.BandWidth;
                                                                    resSysInfo.m_BaseID = r.GeneralResult.StationSysInfo.BaseID;
                                                                    resSysInfo.m_BSIC = r.GeneralResult.StationSysInfo.BSIC;
                                                                    resSysInfo.m_ChannelNumber = r.GeneralResult.StationSysInfo.ChannelNumber;
                                                                    resSysInfo.m_CID = r.GeneralResult.StationSysInfo.CID;
                                                                    resSysInfo.m_Code = r.GeneralResult.StationSysInfo.Code;
                                                                    resSysInfo.m_CtoI = r.GeneralResult.StationSysInfo.CtoI;
                                                                    resSysInfo.m_ECI = r.GeneralResult.StationSysInfo.ECI;
                                                                    resSysInfo.m_eNodeBId = r.GeneralResult.StationSysInfo.eNodeBId;
                                                                    resSysInfo.m_Freq = r.GeneralResult.StationSysInfo.Freq;
                                                                    resSysInfo.m_IcIo = r.GeneralResult.StationSysInfo.IcIo;
                                                                    resSysInfo.m_INBAND_POWER = r.GeneralResult.StationSysInfo.INBAND_POWER;
                                                                    resSysInfo.m_ISCP = r.GeneralResult.StationSysInfo.ISCP;
                                                                    resSysInfo.m_LAC = r.GeneralResult.StationSysInfo.LAC;
                                                                    resSysInfo.m_MCC = r.GeneralResult.StationSysInfo.MCC;
                                                                    resSysInfo.m_MNC = r.GeneralResult.StationSysInfo.MNC;
                                                                    resSysInfo.m_NID= r.GeneralResult.StationSysInfo.NID;
                                                                    resSysInfo.m_PCI = r.GeneralResult.StationSysInfo.PCI;
                                                                    resSysInfo.m_PN = r.GeneralResult.StationSysInfo.PN;
                                                                    resSysInfo.m_Power = r.GeneralResult.StationSysInfo.Power;
                                                                    resSysInfo.m_Ptotal = r.GeneralResult.StationSysInfo.Ptotal;
                                                                    resSysInfo.m_RNC = r.GeneralResult.StationSysInfo.RNC;
                                                                    resSysInfo.m_RSCP= r.GeneralResult.StationSysInfo.RSCP;
                                                                    resSysInfo.m_RSRP = r.GeneralResult.StationSysInfo.RSRP;
                                                                    resSysInfo.m_RSRQ = r.GeneralResult.StationSysInfo.RSRQ;
                                                                    resSysInfo.m_SC = r.GeneralResult.StationSysInfo.SC;
                                                                    resSysInfo.m_SID = r.GeneralResult.StationSysInfo.SID;
                                                                    resSysInfo.m_TAC= r.GeneralResult.StationSysInfo.TAC;
                                                                    resSysInfo.m_TypeCDMAEVDO = r.GeneralResult.StationSysInfo.TypeCDMAEVDO;
                                                                    resSysInfo.m_UCID = r.GeneralResult.StationSysInfo.UCID;
                                                                    resSysInfo.m_IDXBSRESGENERAL = IDResGeneral;
                                                                    int? ID_SysInfo = resSysInfo.Save(dbConnect, transaction);
                                                                    resSysInfo.Close();
                                                                    resSysInfo.Dispose();

                                                                    if (ID_SysInfo != null)
                                                                    {
                                                                        if (r.GeneralResult.StationSysInfo.InfoBlocks != null)
                                                                        {
                                                                            foreach (DataModels.Sdrns.Device.StationSysInfoBlock b in r.GeneralResult.StationSysInfo.InfoBlocks)
                                                                            {
                                                                                YXbsSysInfoBlocks resSysInfoBlocks = new YXbsSysInfoBlocks();
                                                                                resSysInfoBlocks.Format("*");
                                                                                resSysInfoBlocks.Filter = "ID=-1";
                                                                                resSysInfoBlocks.New();
                                                                                resSysInfoBlocks.m_idxbssysinfo = ID_SysInfo;
                                                                                resSysInfoBlocks.m_data = b.Data;
                                                                                resSysInfoBlocks.m_type = b.Type;
                                                                                resSysInfoBlocks.Save(dbConnect, transaction);
                                                                                resSysInfoBlocks.Close();
                                                                                resSysInfoBlocks.Dispose();
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        /////////////////////////////////////////

                                                        if (station.GeneralResult.MaskBW != null)
                                                        {
                                                            if (station.GeneralResult.MaskBW.Length > 0)
                                                            {
                                                                foreach (MaskElements mslel in station.GeneralResult.MaskBW)
                                                                {
                                                                    YXbsResmaskBw resmaskBw = new YXbsResmaskBw();
                                                                    resmaskBw.Format("*");
                                                                    resmaskBw.Filter = "ID=-1";
                                                                    resmaskBw.New();
                                                                    resmaskBw.m_bw = mslel.BW;
                                                                    resmaskBw.m_level = mslel.level;
                                                                    resmaskBw.m_xbsgeneralid = IDResGeneral;
                                                                    int? IDYXbsResmaskBw = resmaskBw.Save(dbConnect, transaction);
                                                                    resmaskBw.Close();
                                                                    resmaskBw.Dispose();
                                                                }
                                                            }
                                                        }


                                                        if (station.GeneralResult.LevelsSpecrum != null)
                                                        {
                                                            if (station.GeneralResult.LevelsSpecrum.Length > 0)
                                                            {
                                                                foreach (float lvl in station.GeneralResult.LevelsSpecrum)
                                                                {
                                                                    YXbsLevelSpecrum reslevelSpecrum = new YXbsLevelSpecrum();
                                                                    reslevelSpecrum.Format("*");
                                                                    reslevelSpecrum.Filter = "ID=-1";
                                                                    reslevelSpecrum.New();
                                                                    reslevelSpecrum.m_levelspecrum = lvl;
                                                                    reslevelSpecrum.m_xbsgeneralid = IDResGeneral;
                                                                    int? IDreslevelSpecrum = reslevelSpecrum.Save(dbConnect, transaction);
                                                                    reslevelSpecrum.Close();
                                                                    reslevelSpecrum.Dispose();
                                                                }
                                                            }
                                                        }

                                                    }

                                                    if (station.LevelMeasurements != null)
                                                    {
                                                        if (station.LevelMeasurements.Length > 0)
                                                        {
                                                            foreach (LevelMeasurementsCar car in station.LevelMeasurements)
                                                            {
                                                                YXbsResLevelMeas yXbsResLevelMeas = new YXbsResLevelMeas();
                                                                yXbsResLevelMeas.Format("*");
                                                                yXbsResLevelMeas.Filter = "ID=-1";
                                                                yXbsResLevelMeas.New();
                                                                yXbsResLevelMeas.m_altitude = car.Altitude;
                                                                yXbsResLevelMeas.m_bw = car.BW;
                                                                yXbsResLevelMeas.m_centralfrequency = (double?)car.CentralFrequency;
                                                                yXbsResLevelMeas.m_differencetimestamp = car.DifferenceTimestamp;
                                                                yXbsResLevelMeas.m_lat = car.Lat;
                                                                yXbsResLevelMeas.m_leveldbm = car.LeveldBm;
                                                                yXbsResLevelMeas.m_leveldbmkvm = car.LeveldBmkVm;
                                                                yXbsResLevelMeas.m_lon = car.Lon;
                                                                yXbsResLevelMeas.m_rbw = car.RBW;
                                                                yXbsResLevelMeas.m_timeofmeasurements = car.TimeOfMeasurements;
                                                                yXbsResLevelMeas.m_vbw = car.VBW;
                                                                yXbsResLevelMeas.m_resultsmeasstationid = IDStation;
                                                                //////////////////
                                                                if (rFinded != null)
                                                                {
                                                                    if (rFinded.LevelResults != null)
                                                                    {
                                                                        DataModels.Sdrns.Device.LevelMeasResult res = rFinded.LevelResults.ToList().Find(t => t.DifferenceTimeStamp_ns == car.DifferenceTimestamp && t.Level_dBm == car.LeveldBm && t.Level_dBmkVm == car.LeveldBmkVm && t.MeasurementTime == car.TimeOfMeasurements && t.Location.ASL == car.Altitude && t.Location.Lat == car.Lat && t.Location.Lon == car.Lon);
                                                                        if (res!=null)
                                                                        {
                                                                            yXbsResLevelMeas.m_agl = res.Location.AGL;
                                                                        }
                                                                    }
                                                                }
                                                                //////////////////

                                                                int? IDyXbsResLevelMeas = yXbsResLevelMeas.Save(dbConnect, transaction);
                                                                yXbsResLevelMeas.Close();
                                                                yXbsResLevelMeas.Dispose();
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }


                                    if (obj.LocationSensorMeasurement != null)
                                    {
                                        List<Yyy> BlockInsert_LocationSensorMeasurement = new List<Yyy>();
                                        foreach (LocationSensorMeasurement dt_param in obj.LocationSensorMeasurement.ToArray())
                                        {
                                            if (dt_param != null)
                                            {
                                                YXbsLocationsensorm dtr = new YXbsLocationsensorm();
                                                dtr.Format("*");
                                                dtr.Filter = "ID=-1";
                                                dtr.New();
                                                if (dt_param.ASL != null) dtr.m_asl = dt_param.ASL.GetValueOrDefault();
                                                if (dt_param.Lon != null) dtr.m_lon = dt_param.Lon.GetValueOrDefault();
                                                if (dt_param.Lat != null) dtr.m_lat = dt_param.Lat.GetValueOrDefault();
                                                dtr.m_id_xbs_measurementres = ID;
                                                for (int i = 0; i < dtr.getAllFields.Count; i++)
                                                    dtr.getAllFields[i].Value = dtr.valc[i];
                                                BlockInsert_LocationSensorMeasurement.Add(dtr);
                                                dtr.Close();
                                                dtr.Dispose();
                                            }
                                        }
                                        if (BlockInsert_LocationSensorMeasurement.Count > 0)
                                        {
                                            YXbsLocationsensorm YXbsLocationsensorm11 = new YXbsLocationsensorm();
                                            YXbsLocationsensorm11.Format("*");
                                            YXbsLocationsensorm11.New();
                                            YXbsLocationsensorm11.SaveBath(BlockInsert_LocationSensorMeasurement, dbConnect, transaction);
                                            YXbsLocationsensorm11.Close();
                                            YXbsLocationsensorm11.Dispose();
                                        }
                                    }

                                    int AllIdx = 0;
                                    if (obj.MeasurementsResults != null)
                                    {
                                        YXbsLevelmeasres dtr_ = new YXbsLevelmeasres();
                                        int idx_cnt = 0;
                                        YXbsLevelmeasres d_level = new YXbsLevelmeasres();
                                        d_level.Format("*");
                                        int? indexerYXbsLevelmeasres = d_level.GetNextId(d_level.GetTableName() + "_SEQ");
                                        YXbsSpectoccupmeas dx_spectrum = new YXbsSpectoccupmeas();
                                        dx_spectrum.Format("*");
                                        int? indexerYXbsSpectoccupmeas = dx_spectrum.GetNextId(dx_spectrum.GetTableName() + "_SEQ");
                                        foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray())
                                        {
                                            if ((obj.TypeMeasurements == MeasurementType.Level) && (obj.Status != "O"))
                                            {
                                                if (dt_param != null)
                                                {
                                                    if (dt_param is LevelMeasurementResult)
                                                    {
                                                        YXbsLevelmeasres dtrR = new YXbsLevelmeasres();
                                                        dtrR.Format("*");
                                                        dtrR.Filter = "ID=-1";
                                                        dtrR.New();
                                                        if ((dt_param as LevelMeasurementResult).Value != null) dtrR.m_value = (dt_param as LevelMeasurementResult).Value.GetValueOrDefault();
                                                        if ((dt_param as LevelMeasurementResult).PMax != null) dtrR.m_pmax = (dt_param as LevelMeasurementResult).PMax.GetValueOrDefault();
                                                        if ((dt_param as LevelMeasurementResult).PMin != null) dtrR.m_pmin = (dt_param as LevelMeasurementResult).PMin.GetValueOrDefault();
                                                        dtrR.m_id_xbs_measurementres = ID;
                                                        for (int i = 0; i < dtrR.getAllFields.Count; i++)
                                                            dtrR.getAllFields[i].Value = dtrR.valc[i];
                                                        BlockInsert_YXbsLevelmeasres1.Add(dtrR);
                                                        dtrR.Close();
                                                        dtrR.Dispose();
                                                        ++indexerYXbsLevelmeasres;

                                                        if (obj.FrequenciesMeasurements != null)
                                                        {
                                                            List<FrequencyMeasurement> Fr_e = obj.FrequenciesMeasurements.ToList().FindAll(t => t.Id == dt_param.Id.Value);
                                                            if (Fr_e != null)
                                                            {
                                                                if (Fr_e.Count > 0)
                                                                {
                                                                    if (Fr_e.Count > 1)
                                                                    {
                                                                        int ddddd = Fr_e.Count;
                                                                    }

                                                                    foreach (FrequencyMeasurement dt_param_freq in Fr_e.ToArray())
                                                                    {
                                                                        if (dt_param_freq != null)
                                                                        {
                                                                            YXbsFrequencymeas dtr_freq = new YXbsFrequencymeas();
                                                                            dtr_freq.Format("*");
                                                                            dtr_freq.Filter = "ID=-1";
                                                                            dtr_freq.New();
                                                                            dtr_freq.m_freq = dt_param_freq.Freq;
                                                                            dtr_freq.m_id_xbs_measurementres = ID;
                                                                            dtr_freq.m_num = indexerYXbsLevelmeasres;

                                                                            for (int i = 0; i < dtr_freq.getAllFields.Count; i++)
                                                                                dtr_freq.getAllFields[i].Value = dtr_freq.valc[i];
                                                                            if (BlockInsert_FrequencyMeasurement2.Find(t => ((YXbsFrequencymeas)t).m_num == indexerYXbsLevelmeasres && ((YXbsFrequencymeas)t).m_freq == dt_param_freq.Freq && ((YXbsFrequencymeas)t).m_id_xbs_measurementres == ID) == null)
                                                                                BlockInsert_FrequencyMeasurement2.Add(dtr_freq);
                                                                            dtr_freq.Close();
                                                                            dtr_freq.Dispose();
                                                                            AllIdx++;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if ((obj.TypeMeasurements == MeasurementType.SpectrumOccupation) /*&& (obj.Status == "C")*/)
                                            {
                                                if (dt_param != null)
                                                {
                                                    if (dt_param is SpectrumOccupationMeasurementResult)
                                                    {
                                                        YXbsSpectoccupmeas dtr = new YXbsSpectoccupmeas();
                                                        dtr.Format("*");
                                                        dtr.Filter = "ID=-1";
                                                        dtr.New();
                                                        if ((dt_param as SpectrumOccupationMeasurementResult).Value != null) dtr.m_value = (dt_param as SpectrumOccupationMeasurementResult).Value.GetValueOrDefault();
                                                        if ((dt_param as SpectrumOccupationMeasurementResult).Occupancy != null) dtr.m_occupancy = (dt_param as SpectrumOccupationMeasurementResult).Occupancy.GetValueOrDefault();
                                                        dtr.m_id_xbs_measurementres = ID;
                                                        for (int i = 0; i < dtr.getAllFields.Count; i++)
                                                            dtr.getAllFields[i].Value = dtr.valc[i];
                                                        BlockInsert_YXbsSpectoccupmeas1.Add(dtr);
                                                        dtr.Close();
                                                        dtr.Dispose();

                                                        ++indexerYXbsSpectoccupmeas;

                                                        if (obj.FrequenciesMeasurements != null)
                                                        {
                                                            List<FrequencyMeasurement> Fr_e = obj.FrequenciesMeasurements.ToList().FindAll(t => t.Id == dt_param.Id.Value);
                                                            if (Fr_e != null)
                                                            {
                                                                if (Fr_e.Count > 0)
                                                                {
                                                                    foreach (FrequencyMeasurement dt_param_freq in Fr_e.ToArray())
                                                                    {
                                                                        if (dt_param_freq != null)
                                                                        {
                                                                            YXbsFrequencymeas dtr_freq = new YXbsFrequencymeas();
                                                                            dtr_freq.Format("*");
                                                                            dtr_freq.Filter = "ID=-1";
                                                                            dtr_freq.New();
                                                                            dtr_freq.m_freq = dt_param_freq.Freq;
                                                                            dtr_freq.m_id_xbs_measurementres = ID;
                                                                            dtr_freq.m_num = indexerYXbsSpectoccupmeas;
                                                                            for (int i = 0; i < dtr_freq.getAllFields.Count; i++)
                                                                                dtr_freq.getAllFields[i].Value = dtr_freq.valc[i];

                                                                            if (BlockInsert_FrequencyMeasurement2.Find(t => ((YXbsFrequencymeas)t).m_num == indexerYXbsSpectoccupmeas && ((YXbsFrequencymeas)t).m_freq == dt_param_freq.Freq && ((YXbsFrequencymeas)t).m_id_xbs_measurementres == ID) == null)
                                                                                BlockInsert_FrequencyMeasurement2.Add(dtr_freq);
                                                                            dtr_freq.Close();
                                                                            dtr_freq.Dispose();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                            }

                                            if (dt_param != null)
                                            {
                                                if (dt_param is LevelMeasurementOnlineResult)
                                                {
                                                    YXbsLevelmeasonlres dtr = new YXbsLevelmeasonlres();
                                                    dtr.Format("*");
                                                    dtr.Filter = "ID=-1";
                                                    dtr.New();
                                                    if ((dt_param as LevelMeasurementOnlineResult).Value != Constants.NullD) dtr.m_value = (dt_param as LevelMeasurementOnlineResult).Value;
                                                    dtr.m_id_xbs_measurementres = ID;
                                                    int? ID_DT_params = dtr.Save(dbConnect, transaction);
                                                    dt_param.Id = new MeasurementResultIdentifier();
                                                    dt_param.Id.Value = ID_DT_params.Value;
                                                    dtr.Close();
                                                    dtr.Dispose();
                                                }
                                            }

                                            idx_cnt++;

                                        }
                                    }
                                    if (BlockInsert_YXbsLevelmeasres1.Count > 0)
                                    {
                                        int iu = AllIdx;
                                        YXbsLevelmeasres YXbsLevelmeasres11 = new YXbsLevelmeasres();
                                        YXbsLevelmeasres11.Format("*");
                                        YXbsLevelmeasres11.New();
                                        YXbsLevelmeasres11.SaveBath(BlockInsert_YXbsLevelmeasres1, dbConnect, transaction);
                                        YXbsLevelmeasres11.Close();
                                        YXbsLevelmeasres11.Dispose();
                                    }

                                    if (BlockInsert_YXbsSpectoccupmeas1.Count > 0)
                                    {
                                        YXbsSpectoccupmeas YXbsSpectoccupmeas11 = new YXbsSpectoccupmeas();
                                        YXbsSpectoccupmeas11.Format("*");
                                        YXbsSpectoccupmeas11.New();
                                        YXbsSpectoccupmeas11.SaveBath(BlockInsert_YXbsSpectoccupmeas1, dbConnect, transaction);
                                        YXbsSpectoccupmeas11.Close();
                                        YXbsSpectoccupmeas11.Dispose();
                                    }

                                    if (BlockInsert_FrequencyMeasurement2.Count > 0)
                                    {
                                        YXbsFrequencymeas YXbsFrequencymeas11 = new YXbsFrequencymeas();
                                        YXbsFrequencymeas11.Format("*");
                                        YXbsFrequencymeas11.New();
                                        YXbsFrequencymeas11.SaveBath(BlockInsert_FrequencyMeasurement2, dbConnect, transaction);
                                        YXbsFrequencymeas11.Close();
                                        YXbsFrequencymeas11.Dispose();
                                    }
                                }
                            }
                            transaction.Commit();
                            logger.Trace("End procedure SaveResultToDB.");
                        }
                        catch (Exception ex)
                        {
                                try
                                {
                                    transaction.Rollback();
                                }
                            catch (Exception e) { transaction.Dispose(); dbConnect.Close(); dbConnect.Dispose(); logger.Error(e.Message); }
                            logger.Error("Error in procedure SaveResultToDB: " + ex.Message);
                        }
                        finally
                        {
                            transaction.Dispose();
                            dbConnect.Close();
                            dbConnect.Dispose();
                        }
                    }
                    else
                    {
                        dbConnect.Close();
                        dbConnect.Dispose();
                    }
                });
                tsk.Start();
                tsk.Join();
            }
            return ID;
        }
        
    }
}
