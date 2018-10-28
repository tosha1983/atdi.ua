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
                        List<Yyy> BlockInsert_YXbsLevelmeasres1 = new List<Yyy>();
                        /// Create new record in YXbsMeastask
                        if (obj != null)
                        {

                            if ((taskId != null) && (obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != Constants.NullI) && (obj.Id.SubMeasTaskStationId != Constants.NullI))
                            {

                                if (obj.StationMeasurements.StationId != null)
                                {
                                    YXbsResMeas measRes = new YXbsResMeas();
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
                                    measRes.m_meassdrresultsid = api2Result.ResultId;
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
                                                YXbsResRoutes resRoutes = new YXbsResRoutes();
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
                                                resRoutes.m_xbsresmeasid = ID;
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
                                        int m_idstation = -1;
                                        if (int.TryParse(station.Idstation, out m_idstation))
                                            measResStation.m_idstation = m_idstation;
                                        measResStation.m_status = station.Status;
                                        measResStation.m_measglobalsid = station.MeasGlobalSID;
                                        measResStation.m_xbsresmeasid = ID;
                                        if (api2Result.StationResults != null)
                                        {
                                                if ((station.Idstation != null) && (station.IdSector!=null))
                                                {
                                                    rFinded = api2Result.StationResults.ToList().Find(t => t.StationId == station.Idstation.ToString() && t.SectorId == station.IdSector.ToString() && t.RealGlobalSid == station.MeasGlobalSID && t.TaskGlobalSid == station.GlobalSID);
                                                    if (rFinded != null)
                                                    {
                                                        measResStation.m_standard = rFinded.Standard;
                                                    }
                                                }
                                         }
        
                                            int? IDStation = measResStation.Save(dbConnect, transaction);
                                            measResStation.Close();
                                            measResStation.Dispose();
                                            if (IDStation > 0)
                                            {
                                                //правки от 20.09.2018
                                                YXbsLinkResSensor linkResSensor = new YXbsLinkResSensor();
                                                linkResSensor.Format("*");
                                                linkResSensor.Filter = "ID=-1";
                                                linkResSensor.New();
                                                linkResSensor.m_id_xbs_sensor = obj.StationMeasurements.StationId.Value;
                                                linkResSensor.m_idxbsresmeassta = IDStation;
                                                int? IDXbsLinkResSensor = linkResSensor.Save(dbConnect, transaction);
                                                linkResSensor.Close();
                                                linkResSensor.Dispose();

                                                if (station.GeneralResult != null)
                                                {
                                                    YXbsResStGeneral measResGeneral = new YXbsResStGeneral();
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
                                                    measResGeneral.m_resmeasstationid = IDStation;

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
                                                        if (api2Result.StationResults != null)
                                                        {
                                                            foreach (DataModels.Sdrns.Device.StationMeasResult r in api2Result.StationResults)
                                                            {

                                                                if (r.GeneralResult.StationSysInfo != null)
                                                                {

                                                                    YXbsResSysInfo resSysInfo = new YXbsResSysInfo();
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
                                                                    resSysInfo.m_xbsresstgeneralid = IDResGeneral;
                                                                    int? ID_SysInfo = resSysInfo.Save(dbConnect, transaction);
                                                                    resSysInfo.Close();
                                                                    resSysInfo.Dispose();

                                                                    if (ID_SysInfo != null)
                                                                    {
                                                                        if (r.GeneralResult.StationSysInfo.InfoBlocks != null)
                                                                        {
                                                                            foreach (DataModels.Sdrns.Device.StationSysInfoBlock b in r.GeneralResult.StationSysInfo.InfoBlocks)
                                                                            {
                                                                                YXbsResSysInfoBlocks resSysInfoBlocks = new YXbsResSysInfoBlocks();
                                                                                resSysInfoBlocks.Format("*");
                                                                                resSysInfoBlocks.Filter = "ID=-1";
                                                                                resSysInfoBlocks.New();
                                                                                resSysInfoBlocks.m_xbs_ressysinfo_id = ID_SysInfo;
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
                                                        if (station.GeneralResult.MaskBW != null)
                                                        {
                                                            if (station.GeneralResult.MaskBW.Length > 0)
                                                            {
                                                                foreach (MaskElements mslel in station.GeneralResult.MaskBW)
                                                                {
                                                                    YXbsResStMaskElm resmaskBw = new YXbsResStMaskElm();
                                                                    resmaskBw.Format("*");
                                                                    resmaskBw.Filter = "ID=-1";
                                                                    resmaskBw.New();
                                                                    resmaskBw.m_bw = mslel.BW;
                                                                    resmaskBw.m_level = mslel.level;
                                                                    resmaskBw.m_xbs_resstgeneralid = IDResGeneral;
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
                                                                    YXbsResStLevelsSpect reslevelSpecrum = new YXbsResStLevelsSpect();
                                                                    reslevelSpecrum.Format("*");
                                                                    reslevelSpecrum.Filter = "ID=-1";
                                                                    reslevelSpecrum.New();
                                                                    reslevelSpecrum.m_levelspecrum = lvl;
                                                                    reslevelSpecrum.m_xbs_resstgeneralid = IDResGeneral;
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
                                                                YXbsResStLevelCar yXbsResLevelMeas = new YXbsResStLevelCar();
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
                                                                yXbsResLevelMeas.m_xbs_resmeasstationid = IDStation;
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
                                                YXbsResLocSensorMeas dtr = new YXbsResLocSensorMeas();
                                                dtr.Format("*");
                                                dtr.Filter = "ID=-1";
                                                dtr.New();
                                                if (dt_param.ASL != null) dtr.m_asl = dt_param.ASL.GetValueOrDefault();
                                                if (dt_param.Lon != null) dtr.m_lon = dt_param.Lon.GetValueOrDefault();
                                                if (dt_param.Lat != null) dtr.m_lat = dt_param.Lat.GetValueOrDefault();
                                                dtr.m_xbsresmeasid = ID;
                                                for (int i = 0; i < dtr.getAllFields.Count; i++)
                                                    dtr.getAllFields[i].Value = dtr.valc[i];
                                                BlockInsert_LocationSensorMeasurement.Add(dtr);
                                                dtr.Close();
                                                dtr.Dispose();
                                            }
                                        }
                                        if (BlockInsert_LocationSensorMeasurement.Count > 0)
                                        {
                                            YXbsResLocSensorMeas YXbsLocationsensorm11 = new YXbsResLocSensorMeas();
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
                                        int idx_cnt = 0;
                                        YXbsResLevels d_level = new YXbsResLevels();
                                        d_level.Format("*");
                                        //int? indexerYXbsLevelmeasres = d_level.GetNextId(d_level.GetTableName() + "_SEQ");
                                        foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray())
                                        {
                                            if ((obj.TypeMeasurements == MeasurementType.Level) || (obj.TypeMeasurements == MeasurementType.SpectrumOccupation))
                                            {
                                                YXbsResLevels dtrR = new YXbsResLevels();
                                                dtrR.Format("*");
                                                dtrR.Filter = "ID=-1";
                                                dtrR.New();

                                                if ((obj.TypeMeasurements == MeasurementType.Level) && (obj.Status != "O"))
                                                {
                                                    if (dt_param != null)
                                                    {
                                                        if (dt_param is LevelMeasurementResult)
                                                        {

                                                            if ((dt_param as LevelMeasurementResult).Value != null) dtrR.m_valuelvl = (dt_param as LevelMeasurementResult).Value.GetValueOrDefault();
                                                            if ((dt_param as LevelMeasurementResult).PMax != null) dtrR.m_pmaxlvl = (dt_param as LevelMeasurementResult).PMax.GetValueOrDefault();
                                                            if ((dt_param as LevelMeasurementResult).PMin != null) dtrR.m_pminlvl = (dt_param as LevelMeasurementResult).PMin.GetValueOrDefault();

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
                                                                                dtrR.m_freqmeas = dt_param_freq.Freq;
                                                                                //dtrR.m_nummeas = indexerYXbsLevelmeasres;
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
                                                            if ((dt_param as SpectrumOccupationMeasurementResult).Value != null) dtrR.m_valuespect = (dt_param as SpectrumOccupationMeasurementResult).Value.GetValueOrDefault();
                                                            if ((dt_param as SpectrumOccupationMeasurementResult).Occupancy != null) dtrR.m_occupancyspect = (dt_param as SpectrumOccupationMeasurementResult).Occupancy.GetValueOrDefault();

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
                                                                                dtrR.m_freqmeas = dt_param_freq.Freq;
                                                                                //dtrR.m_nummeas = indexerYXbsLevelmeasres;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                dtrR.m_xbsresmeasid = ID;
                                                for (int i = 0; i < dtrR.getAllFields.Count; i++)
                                                    dtrR.getAllFields[i].Value = dtrR.valc[i];
                                                BlockInsert_YXbsLevelmeasres1.Add(dtrR);
                                                dtrR.Close();
                                                dtrR.Dispose();
                                                //++indexerYXbsLevelmeasres;
                                            }

                                            if (dt_param != null)
                                            {
                                                if (dt_param is LevelMeasurementOnlineResult)
                                                {
                                                    YXbsResLevmeasonline dtr = new YXbsResLevmeasonline();
                                                    dtr.Format("*");
                                                    dtr.Filter = "ID=-1";
                                                    dtr.New();
                                                    if ((dt_param as LevelMeasurementOnlineResult).Value != Constants.NullD) dtr.m_value = (dt_param as LevelMeasurementOnlineResult).Value;
                                                    dtr.m_xbsresmeasid = ID;
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
                                        YXbsResLevels YXbsLevelmeasres11 = new YXbsResLevels();
                                        YXbsLevelmeasres11.Format("*");
                                        YXbsLevelmeasres11.New();
                                        YXbsLevelmeasres11.SaveBath(BlockInsert_YXbsLevelmeasres1, dbConnect, transaction);
                                        YXbsLevelmeasres11.Close();
                                        YXbsLevelmeasres11.Dispose();
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
