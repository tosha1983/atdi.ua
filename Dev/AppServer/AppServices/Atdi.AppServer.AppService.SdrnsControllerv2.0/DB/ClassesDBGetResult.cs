using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Contracts.Sdrns;
using System.Data.Common;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{

    public class ClassesDBGetResult 
    {
        public static ILogger logger;
        public ClassesDBGetResult(ILogger log)
        {
            if (logger == null) logger = log;
        }



        public int? SaveResultToDB(MeasurementResults obj, Atdi.DataModels.Sdrns.Device.MeasResults api2Result, string taskId, out string Error)
        {
            Error = "";
            string  ErrorValue = "";
            int? ID = Constants.NullI;
            System.Threading.Thread tsk = new System.Threading.Thread(() =>
            {
                {
                logger.Trace("Start procedure SaveResultToDB.");
                Yyy yyy = new Yyy();
                yyy.New();
                DbConnection dbConnect = null;
                try
                {
                    dbConnect = yyy.NewConnection(yyy.GetConnectionString());
                    if (dbConnect.State == System.Data.ConnectionState.Open)
                    {
                        DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                        try
                        {
                            List<Yyy> BlockInsert_YXbsLevelmeasres1 = new List<Yyy>();
                            /// Create new record in YXbsMeastask
                            if (obj != null)
                            {
                                if ((obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != Constants.NullI) && (obj.Id.SubMeasTaskStationId != Constants.NullI))
                                {

                                    if (obj.StationMeasurements.StationId != null)
                                    {
                                        YXbsResMeas measRes = new YXbsResMeas();
                                        measRes.rs = yyy.rs;
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
                                    }
                                }
                                if (ID != Constants.NullI)
                                {
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
                                                    resRoutes.rs = yyy.rs;
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
                                                }
                                            }
                                        }
                                    }


                                    if (obj.ResultsMeasStation != null)
                                    {
                                        for (int n=0; n< obj.ResultsMeasStation.Length; n++)
                                        {
                                            ResultsMeasurementsStation station = obj.ResultsMeasStation[n];

                                            DataModels.Sdrns.Device.StationMeasResult rFinded = new DataModels.Sdrns.Device.StationMeasResult();
                                            YXbsResmeasstation measResStation = new YXbsResmeasstation();
                                            measResStation.rs = yyy.rs;
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
                                            measResStation.m_standard = station.Standard;
                                           
                                            if (api2Result.StationResults != null)
                                            {
                                                if ((station.Idstation != null) && (station.IdSector != null))
                                                {
                                                    rFinded = api2Result.StationResults.ToList().Find(t => t.StationId == station.Idstation.ToString() && t.SectorId == station.IdSector.ToString() && t.RealGlobalSid == station.MeasGlobalSID && t.TaskGlobalSid == station.GlobalSID);
                                                    //if (rFinded != null)
                                                    //{
                                                        //measResStation.m_standard = rFinded.Standard;
                                                    //}
                                                }
                                            }
                                           

                                            int? IDStation = measResStation.Save(dbConnect, transaction);
                                            if (IDStation > 0)
                                            {
                                                //правки от 20.09.2018
                                                YXbsLinkResSensor linkResSensor = new YXbsLinkResSensor();
                                                linkResSensor.rs = yyy.rs;
                                                linkResSensor.Format("*");
                                                linkResSensor.Filter = "ID=-1";
                                                linkResSensor.New();
                                                linkResSensor.m_id_xbs_sensor = obj.StationMeasurements.StationId.Value;
                                                linkResSensor.m_idxbsresmeassta = IDStation;
                                                int? IDXbsLinkResSensor = linkResSensor.Save(dbConnect, transaction);
                                                var generalResult = station.GeneralResult;
                                                if (generalResult != null)
                                                {
                                                    YXbsResStGeneral measResGeneral = new YXbsResStGeneral();
                                                    measResGeneral.rs = yyy.rs;
                                                    measResGeneral.Format("*");
                                                    measResGeneral.Filter = "ID=-1";
                                                    measResGeneral.New();
                                                    measResGeneral.m_centralfrequency = generalResult.CentralFrequency;
                                                    measResGeneral.m_centralfrequencymeas = generalResult.CentralFrequencyMeas;
                                                    measResGeneral.m_durationmeas = generalResult.DurationMeas;
                                                    measResGeneral.m_markerindex = generalResult.MarkerIndex;
                                                    measResGeneral.m_offsetfrequency = generalResult.OffsetFrequency;
                                                    measResGeneral.m_specrumstartfreq = (double?)generalResult.SpecrumStartFreq;
                                                    measResGeneral.m_specrumsteps = (double?)generalResult.SpecrumSteps;
                                                    measResGeneral.m_t1 = generalResult.T1;
                                                    measResGeneral.m_t2 = generalResult.T2;
                                                    measResGeneral.m_timefinishmeas = generalResult.TimeFinishMeas;
                                                    measResGeneral.m_timestartmeasdate = generalResult.TimeStartMeas;
                                                    measResGeneral.m_resmeasstationid = IDStation;

                                                    if (rFinded != null)
                                                    {
                                                        if (rFinded.GeneralResult != null)
                                                        {
                                                            if (rFinded.GeneralResult.BandwidthResult != null)
                                                            {
                                                                var bandwidthResult = rFinded.GeneralResult.BandwidthResult;
                                                                measResGeneral.m_tracecount = bandwidthResult.TraceCount;
                                                                measResGeneral.m_сorrectnessestim = bandwidthResult.СorrectnessEstimations == true ? 1 : 0;
                                                            }
                                                        }
                                                    }

                                                    int? IDResGeneral = measResGeneral.Save(dbConnect, transaction);
                                                    if (IDResGeneral > 0)
                                                    {
                                                        if (rFinded != null)
                                                        {
                                                            if (rFinded.GeneralResult != null)
                                                            {
                                                                var r = rFinded;
                                                                if (r.GeneralResult.StationSysInfo != null)
                                                                {
                                                                    var stationSysInfo = r.GeneralResult.StationSysInfo;

                                                                    YXbsResSysInfo resSysInfo = new YXbsResSysInfo();
                                                                    resSysInfo.rs = yyy.rs;
                                                                    resSysInfo.Format("*");
                                                                    resSysInfo.Filter = "ID=-1";
                                                                    resSysInfo.New();

                                                                    if (stationSysInfo.Location != null)
                                                                    {
                                                                        var stationSysInfoLocation = stationSysInfo.Location;

                                                                        resSysInfo.m_AGL = stationSysInfoLocation.AGL;
                                                                        resSysInfo.m_ASL = stationSysInfoLocation.ASL;
                                                                        resSysInfo.m_Lat = stationSysInfoLocation.Lat;
                                                                        resSysInfo.m_Lon = stationSysInfoLocation.Lon;
                                                                    }
                                                                    resSysInfo.m_BandWidth = stationSysInfo.BandWidth;
                                                                    resSysInfo.m_BaseID = stationSysInfo.BaseID;
                                                                    resSysInfo.m_BSIC = stationSysInfo.BSIC;
                                                                    resSysInfo.m_ChannelNumber = stationSysInfo.ChannelNumber;
                                                                    resSysInfo.m_CID = stationSysInfo.CID;
                                                                    resSysInfo.m_Code = stationSysInfo.Code;
                                                                    resSysInfo.m_CtoI = stationSysInfo.CtoI;
                                                                    resSysInfo.m_ECI = stationSysInfo.ECI;
                                                                    resSysInfo.m_eNodeBId = stationSysInfo.eNodeBId;
                                                                    resSysInfo.m_Freq = stationSysInfo.Freq;
                                                                    resSysInfo.m_IcIo = stationSysInfo.IcIo;
                                                                    resSysInfo.m_INBAND_POWER = stationSysInfo.INBAND_POWER;
                                                                    resSysInfo.m_ISCP = stationSysInfo.ISCP;
                                                                    resSysInfo.m_LAC = stationSysInfo.LAC;
                                                                    resSysInfo.m_MCC = stationSysInfo.MCC;
                                                                    resSysInfo.m_MNC = stationSysInfo.MNC;
                                                                    resSysInfo.m_NID = stationSysInfo.NID;
                                                                    resSysInfo.m_PCI = stationSysInfo.PCI;
                                                                    resSysInfo.m_PN = stationSysInfo.PN;
                                                                    resSysInfo.m_Power = stationSysInfo.Power;
                                                                    resSysInfo.m_Ptotal = stationSysInfo.Ptotal;
                                                                    resSysInfo.m_RNC = stationSysInfo.RNC;
                                                                    resSysInfo.m_RSCP = stationSysInfo.RSCP;
                                                                    resSysInfo.m_RSRP = stationSysInfo.RSRP;
                                                                    resSysInfo.m_RSRQ = stationSysInfo.RSRQ;
                                                                    resSysInfo.m_SC = stationSysInfo.SC;
                                                                    resSysInfo.m_SID = stationSysInfo.SID;
                                                                    resSysInfo.m_TAC = stationSysInfo.TAC;
                                                                    resSysInfo.m_TypeCDMAEVDO = stationSysInfo.TypeCDMAEVDO;
                                                                    resSysInfo.m_UCID = stationSysInfo.UCID;
                                                                    resSysInfo.m_xbsresstgeneralid = IDResGeneral;
                                                                    int? ID_SysInfo = resSysInfo.Save(dbConnect, transaction);

                                                                    if (ID_SysInfo != null)
                                                                    {
                                                                        if (r.GeneralResult.StationSysInfo.InfoBlocks != null)
                                                                        {
                                                                            foreach (DataModels.Sdrns.Device.StationSysInfoBlock b in r.GeneralResult.StationSysInfo.InfoBlocks)
                                                                            {
                                                                                YXbsResSysInfoBlocks resSysInfoBlocks = new YXbsResSysInfoBlocks();
                                                                                resSysInfoBlocks.rs = yyy.rs;
                                                                                resSysInfoBlocks.Format("*");
                                                                                resSysInfoBlocks.Filter = "ID=-1";
                                                                                resSysInfoBlocks.New();
                                                                                resSysInfoBlocks.m_xbs_ressysinfo_id = ID_SysInfo;
                                                                                resSysInfoBlocks.m_data = b.Data;
                                                                                resSysInfoBlocks.m_type = b.Type;
                                                                                resSysInfoBlocks.Save(dbConnect, transaction);
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
                                                                List<Yyy> BlockInsert_MaskElements = new List<Yyy>();
                                                                for (int l = 0;l < station.GeneralResult.MaskBW.Length; l++)
                                                                {
                                                                    MaskElements mslel = station.GeneralResult.MaskBW[l];

                                                                    YXbsResStMaskElm resmaskBw = new YXbsResStMaskElm();
                                                                    resmaskBw.rs = yyy.rs;
                                                                    resmaskBw.Format("*");
                                                                    resmaskBw.Filter = "ID=-1";
                                                                    resmaskBw.New();
                                                                    resmaskBw.m_bw = mslel.BW;
                                                                    resmaskBw.m_level = mslel.level;
                                                                    resmaskBw.m_xbs_resstgeneralid = IDResGeneral;
                                                                    //int? IDYXbsResmaskBw = resmaskBw.Save(dbConnect, transaction);
                                                                    for (int i = 0; i < resmaskBw.getAllFields.Count; i++)
                                                                        resmaskBw.getAllFields[i].Value = resmaskBw.valc[i];
                                                                    BlockInsert_MaskElements.Add(resmaskBw);
                                                                    System.Threading.Thread.Yield();
                                                                }
                                                                if (BlockInsert_MaskElements.Count > 0)
                                                                {
                                                                    YXbsResStMaskElm YXbsResStMaskElm1 = new YXbsResStMaskElm();
                                                                    YXbsResStMaskElm1.rs = yyy.rs;
                                                                    YXbsResStMaskElm1.Format("*");
                                                                    YXbsResStMaskElm1.New();
                                                                    YXbsResStMaskElm1.SaveBath(BlockInsert_MaskElements, dbConnect, transaction);
                                                                }
                                                            }
                                                        }
                                                        if (station.GeneralResult.LevelsSpecrum != null)
                                                        {
                                                            if (station.GeneralResult.LevelsSpecrum.Length > 0)
                                                            {
                                                                List<Yyy> BlockInsert_YXbsResStLevelsSpect = new List<Yyy>();
                                                                for (int l = 0; l < station.GeneralResult.LevelsSpecrum.Length; l++)
                                                                {
                                                                    double lvl = station.GeneralResult.LevelsSpecrum[l];
                                                                    YXbsResStLevelsSpect reslevelSpecrum = new YXbsResStLevelsSpect();
                                                                    reslevelSpecrum.rs = yyy.rs;
                                                                    reslevelSpecrum.Format("*");
                                                                    reslevelSpecrum.Filter = "ID=-1";
                                                                    reslevelSpecrum.New();
                                                                    reslevelSpecrum.m_levelspecrum = lvl;
                                                                    reslevelSpecrum.m_xbs_resstgeneralid = IDResGeneral;
                                                                    //int? IDreslevelSpecrum = reslevelSpecrum.Save(dbConnect, transaction);
                                                                    for (int i = 0; i < reslevelSpecrum.getAllFields.Count; i++)
                                                                        reslevelSpecrum.getAllFields[i].Value = reslevelSpecrum.valc[i];
                                                                    BlockInsert_YXbsResStLevelsSpect.Add(reslevelSpecrum);

                                                                    System.Threading.Thread.Yield();
                                                                }

                                                                if (BlockInsert_YXbsResStLevelsSpect.Count > 0)
                                                                {
                                                                    YXbsResStLevelsSpect YXbsResStLevelsSpect1 = new YXbsResStLevelsSpect();
                                                                    YXbsResStLevelsSpect1.rs = yyy.rs;
                                                                    YXbsResStLevelsSpect1.Format("*");
                                                                    YXbsResStLevelsSpect1.New();
                                                                    YXbsResStLevelsSpect1.SaveBath(BlockInsert_YXbsResStLevelsSpect, dbConnect, transaction);
                                                                }
                                                            }
                                                        }

                                                    }
                                                    if (station.LevelMeasurements != null)
                                                    {
                                                        if (station.LevelMeasurements.Length > 0)
                                                        {
                                                            List<Yyy> BlockInsert_LevelMeasurementsCar = new List<Yyy>();
                                                            for (int l = 0; l < station.LevelMeasurements.Length; l++)
                                                            {
                                                                LevelMeasurementsCar car = station.LevelMeasurements[l];
                                                                YXbsResStLevelCar yXbsResLevelMeas = new YXbsResStLevelCar();
                                                                yXbsResLevelMeas.rs = yyy.rs;
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
                                                                        if (res != null)
                                                                        {
                                                                            yXbsResLevelMeas.m_agl = res.Location.AGL;
                                                                        }
                                                                    }
                                                                }
                                                                //int? IDyXbsResLevelMeas = yXbsResLevelMeas.Save(dbConnect, transaction);
                                                                for (int i = 0; i < yXbsResLevelMeas.getAllFields.Count; i++)
                                                                    yXbsResLevelMeas.getAllFields[i].Value = yXbsResLevelMeas.valc[i];
                                                                BlockInsert_LevelMeasurementsCar.Add(yXbsResLevelMeas);
                                                                System.Threading.Thread.Yield();
                                                            }

                                                            if (BlockInsert_LevelMeasurementsCar.Count > 0)
                                                            {
                                                                YXbsResStLevelCar YXbsResStLevelCar1 = new YXbsResStLevelCar();
                                                                YXbsResStLevelCar1.rs = yyy.rs;
                                                                YXbsResStLevelCar1.Format("*");
                                                                YXbsResStLevelCar1.New();
                                                                YXbsResStLevelCar1.SaveBath(BlockInsert_LevelMeasurementsCar, dbConnect, transaction);
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
                                            for (int l = 0; l < obj.LocationSensorMeasurement.Length; l++)
                                            {
                                                LocationSensorMeasurement dt_param = obj.LocationSensorMeasurement[l];
                                                if (dt_param != null)
                                                {
                                                    YXbsResLocSensorMeas dtr = new YXbsResLocSensorMeas();
                                                    dtr.rs = yyy.rs;
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

                                                    System.Threading.Thread.Yield();
                                                }
                                            }
                                            if (BlockInsert_LocationSensorMeasurement.Count > 0)
                                            {
                                                YXbsResLocSensorMeas YXbsLocationsensorm11 = new YXbsResLocSensorMeas();
                                                YXbsLocationsensorm11.rs = yyy.rs;
                                                YXbsLocationsensorm11.Format("*");
                                                YXbsLocationsensorm11.New();
                                                YXbsLocationsensorm11.SaveBath(BlockInsert_LocationSensorMeasurement, dbConnect, transaction);
                                            }
                                    }

                                    int AllIdx = 0;
                                    if (obj.MeasurementsResults != null)
                                    {
                                        int idx_cnt = 0;
                                        for (int l = 0; l < obj.MeasurementsResults.Length; l++)
                                        {
                                            MeasurementResult dt_param  = obj.MeasurementsResults[l];
                                            if ((obj.TypeMeasurements == MeasurementType.Level) || (obj.TypeMeasurements == MeasurementType.SpectrumOccupation))
                                            {
                                                YXbsResLevels dtrR = new YXbsResLevels();
                                                dtrR.rs = yyy.rs;
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
                                                               dtrR.m_freqmeas = obj.FrequenciesMeasurements[l].Freq;
                                                               /*
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
                                                                */
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
                                                                dtrR.m_freqmeas = obj.FrequenciesMeasurements[l].Freq;
                                                                /*
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
                                                                */
                                                            }
                                                        }
                                                    }
                                                }

                                                dtrR.m_xbsresmeasid = ID;
                                                for (int i = 0; i < dtrR.getAllFields.Count; i++)
                                                    dtrR.getAllFields[i].Value = dtrR.valc[i];
                                                BlockInsert_YXbsLevelmeasres1.Add(dtrR);
                                            }

                                            if (dt_param != null)
                                            {
                                                if (dt_param is LevelMeasurementOnlineResult)
                                                {
                                                    YXbsResLevmeasonline dtr = new YXbsResLevmeasonline();
                                                    dtr.rs = yyy.rs;
                                                    dtr.Format("*");
                                                    dtr.Filter = "ID=-1";
                                                    dtr.New();
                                                    if ((dt_param as LevelMeasurementOnlineResult).Value != Constants.NullD) dtr.m_value = (dt_param as LevelMeasurementOnlineResult).Value;
                                                    dtr.m_xbsresmeasid = ID;
                                                    int? ID_DT_params = dtr.Save(dbConnect, transaction);
                                                    dt_param.Id = new MeasurementResultIdentifier();
                                                    dt_param.Id.Value = ID_DT_params.Value;
                                                }
                                            }

                                            idx_cnt++;
                                            System.Threading.Thread.Yield();
                                         }
                                    }
                                    if (BlockInsert_YXbsLevelmeasres1.Count > 0)
                                    {
                                        int iu = AllIdx;
                                        YXbsResLevels YXbsLevelmeasres11 = new YXbsResLevels();
                                        YXbsLevelmeasres11.rs = yyy.rs;
                                        YXbsLevelmeasres11.Format("*");
                                        YXbsLevelmeasres11.New();
                                        YXbsLevelmeasres11.SaveBath(BlockInsert_YXbsLevelmeasres1, dbConnect, transaction);
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            ErrorValue = ex.Message;
                            ID = Constants.NullI;
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e) { transaction.Dispose(); logger.Error(e.Message); }
                            logger.Error("Error in procedure SaveResultToDB: " + ex.Message);
                        }
                        finally
                        {
                            transaction.Dispose();
                        }
                    }
                    else
                    {
                        ErrorValue = "Error connection  to Database";
                        logger.Error("[SaveResultToDB] Error connection  to Database");
                        ID = Constants.NullI;
                    }
                }
                catch (Exception ex)
                {
                    ErrorValue = ex.Message;
                    ID = Constants.NullI;
                    logger.Error(ex.Message);
                }
                finally
                {
                    dbConnect.Close();
                    dbConnect.Dispose();
                }
                yyy.Close();
                yyy.Dispose();
                logger.Trace("End procedure SaveResultToDB");
            }
            });
            tsk.Start();
            tsk.Join();
            Error = ErrorValue;
            return ID;
        }


        

        public byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
            return null;
        }
    }

}
