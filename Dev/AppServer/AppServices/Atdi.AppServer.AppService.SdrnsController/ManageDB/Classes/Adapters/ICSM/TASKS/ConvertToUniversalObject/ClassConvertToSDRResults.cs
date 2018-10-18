using System;
using System.Collections.Generic;
using Atdi.Oracle.DataAccess;
using Atdi.AppServer;

using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    public class ClassConvertToSDRResults : IDisposable
    {
        public static ILogger logger;
        public ClassConvertToSDRResults(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassConvertToSDRResults()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public ResultsMeasurementsStation[] ConvertTo_ResultsMeasurementsStation(List<ClassSDRResults> objs)
        {
            List<ResultsMeasurementsStation> L_OUT = new List<ResultsMeasurementsStation>();
            try
            {
                logger.Trace("Start procedure ConvertTo_ResultsMeasurementsStation...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        foreach (ClassSDRResults obj in objs.ToArray())
                        {
                            if (obj.XbsResmeasstation != null)
                            {
                                if (obj.XbsResmeasstation.Count > 0)
                                { 
                                    int ii = 0;
                                    foreach (YXbsResmeasstation flevmeas in obj.XbsResmeasstation.ToArray())
                                    {
                                        ResultsMeasurementsStation ResultsMeasStation = new ResultsMeasurementsStation();
                                        if (flevmeas.m_idsector != null) ResultsMeasStation.IdSector = flevmeas.m_idsector;
                                        if (flevmeas.m_idstation != null) ResultsMeasStation.Idstation = flevmeas.m_idstation.Value.ToString();

                                        ResultsMeasStation.GlobalSID = flevmeas.m_globalsid;
                                        ResultsMeasStation.MeasGlobalSID = flevmeas.m_measglobalsid;
                                        ResultsMeasStation.Status = flevmeas.m_status;
                                        if (obj.XbsResLevelMeas != null)
                                        {
                                            List<YXbsResStLevelCar> resF = obj.XbsResLevelMeas.FindAll(t => t.m_xbs_resmeasstationid == flevmeas.m_id);
                                            if (resF.Count > 0)
                                            {
                                                ResultsMeasStation.LevelMeasurements = new LevelMeasurementsCar[resF.Count];
                                                int u = 0;
                                                foreach (YXbsResStLevelCar x in resF)
                                                {
                                                    ResultsMeasStation.LevelMeasurements[u] = new LevelMeasurementsCar();
                                                    if (x.m_altitude != null) ResultsMeasStation.LevelMeasurements[u].Altitude = x.m_altitude;
                                                    if (x.m_bw != null) ResultsMeasStation.LevelMeasurements[u].BW = x.m_bw;
                                                    if (x.m_centralfrequency != null) ResultsMeasStation.LevelMeasurements[u].CentralFrequency = (decimal?)x.m_centralfrequency;
                                                    if (x.m_differencetimestamp != null) ResultsMeasStation.LevelMeasurements[u].DifferenceTimestamp = x.m_differencetimestamp;
                                                    if (x.m_lat != null) ResultsMeasStation.LevelMeasurements[u].Lat = x.m_lat;
                                                    if (x.m_lon != null) ResultsMeasStation.LevelMeasurements[u].Lon = x.m_lon;
                                                    if (x.m_leveldbm != null) ResultsMeasStation.LevelMeasurements[u].LeveldBm = x.m_leveldbm;
                                                    if (x.m_leveldbmkvm != null) ResultsMeasStation.LevelMeasurements[u].LeveldBmkVm = x.m_leveldbmkvm;
                                                    if (x.m_rbw != null) ResultsMeasStation.LevelMeasurements[u].RBW = x.m_rbw;
                                                    if (x.m_timeofmeasurements != null) ResultsMeasStation.LevelMeasurements[u].TimeOfMeasurements = x.m_timeofmeasurements.GetValueOrDefault();
                                                    if (x.m_vbw != null) ResultsMeasStation.LevelMeasurements[u].VBW = x.m_vbw;
                                                    u++;
                                                }
                                            }
                                        }


                                        if (obj.XbsResGeneral != null)
                                        {
                                            ResultsMeasStation.GeneralResult = new MeasurementsParameterGeneral();
                                            List<YXbsResStGeneral> resF = obj.XbsResGeneral.FindAll(t => t.m_resmeasstationid == flevmeas.m_id);
                                            if (resF != null)
                                            {
                                                foreach (YXbsResStGeneral x in resF)
                                                {
                                                    if (x.m_centralfrequency != null) ResultsMeasStation.GeneralResult.CentralFrequency = x.m_centralfrequency;
                                                    if (x.m_centralfrequencymeas != null) ResultsMeasStation.GeneralResult.CentralFrequencyMeas = x.m_centralfrequencymeas;
                                                    if (x.m_durationmeas != null) ResultsMeasStation.GeneralResult.DurationMeas = x.m_durationmeas;
                                                    if (x.m_markerindex != null) ResultsMeasStation.GeneralResult.MarkerIndex = x.m_markerindex;
                                                    if (x.m_offsetfrequency != null) ResultsMeasStation.GeneralResult.OffsetFrequency = x.m_offsetfrequency;
                                                    if (x.m_specrumstartfreq != null) ResultsMeasStation.GeneralResult.SpecrumStartFreq = (decimal?)x.m_specrumstartfreq;
                                                    if (x.m_specrumsteps != null) ResultsMeasStation.GeneralResult.SpecrumSteps = (decimal?)x.m_specrumsteps;
                                                    if (x.m_t1 != null) ResultsMeasStation.GeneralResult.T1 = x.m_t1;
                                                    if (x.m_t2 != null) ResultsMeasStation.GeneralResult.T2 = x.m_t2;
                                                    if (x.m_timefinishmeas != null) ResultsMeasStation.GeneralResult.TimeFinishMeas = x.m_timefinishmeas;
                                                    if (x.m_timestartmeasdate != null) ResultsMeasStation.GeneralResult.TimeStartMeas = x.m_timestartmeasdate;

                                                    if (obj.XbsResmaskBw != null)
                                                    {
                                                        List<YXbsResStMaskElm> resYXbsResmaskBw = obj.XbsResmaskBw.FindAll(t => t.m_xbs_resstgeneralid == x.m_id);
                                                        if (resYXbsResmaskBw.Count > 0)
                                                        {
                                                            ResultsMeasStation.GeneralResult.MaskBW = new MaskElements[resYXbsResmaskBw.Count];
                                                            int u = 0;
                                                            foreach (YXbsResStMaskElm xv in resYXbsResmaskBw)
                                                            {
                                                                ResultsMeasStation.GeneralResult.MaskBW[u] = new MaskElements();
                                                                if (xv.m_bw != null) ResultsMeasStation.GeneralResult.MaskBW[u].BW = xv.m_bw;
                                                                if (xv.m_level != null) ResultsMeasStation.GeneralResult.MaskBW[u].level = xv.m_level;
                                                                u++;
                                                            }
                                                        }
                                                    }

                                                    if (obj.XbsLevelSpecrum != null)
                                                    {
                                                        List<YXbsResStLevelsSpect> resYXbsLevelSpecrum = obj.XbsLevelSpecrum.FindAll(t => t.m_xbs_resstgeneralid == x.m_id);
                                                        if (resYXbsLevelSpecrum.Count > 0)
                                                        {
                                                            ResultsMeasStation.GeneralResult.LevelsSpecrum = new float[resYXbsLevelSpecrum.Count];
                                                            int u = 0;
                                                            foreach (YXbsResStLevelsSpect xv in resYXbsLevelSpecrum)
                                                            {
                                                                ResultsMeasStation.GeneralResult.LevelsSpecrum[u] = new float();
                                                                if (xv.m_levelspecrum != null) ResultsMeasStation.GeneralResult.LevelsSpecrum[u] = (float)xv.m_levelspecrum;
                                                                u++;
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        ii++;
                                        L_OUT.Add(ResultsMeasStation);
                                    }

                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ConvertTo_ResultsMeasurementsStation... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ConvertTo_ResultsMeasurementsStation...");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ConvertTo_ResultsMeasurementsStation..." + ex.Message);
            }
            return L_OUT.ToArray();
        }

        public ShortResultsMeasurementsStation[] ConvertTo_ShortResultsMeasurementsStation(List<ClassSDRResults> objs)
        {
            List<ShortResultsMeasurementsStation> L_OUT = new List<ShortResultsMeasurementsStation>();
            try
            {
                logger.Trace("Start procedure ConvertTo_ShortResultsMeasurementsStation...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        foreach (ClassSDRResults obj in objs.ToArray())
                        {
                            if (obj.XbsResmeasstation != null)
                            {
                                if (obj.XbsResmeasstation.Count > 0)
                                {
                                    int ii = 0;
                                    foreach (YXbsResmeasstation flevmeas in obj.XbsResmeasstation.ToArray())
                                    {
                                        ShortResultsMeasurementsStation s_out = new ShortResultsMeasurementsStation();
                                        if (flevmeas.m_idsector != null) s_out.IdSector = flevmeas.m_idsector;
                                        if (flevmeas.m_idstation != null) s_out.Idstation = flevmeas.m_idstation.HasValue ? flevmeas.m_idstation.Value.ToString() : "";
                                        s_out.GlobalSID = flevmeas.m_globalsid;
                                        s_out.MeasGlobalSID = flevmeas.m_measglobalsid;
                                        s_out.Status = flevmeas.m_status;
                                        List<SiteStationForMeas> ListStationSite = new  List<SiteStationForMeas>();
                                        YXbsStation Station = new YXbsStation();
                                        Station.Format("*");
                                        Station.Filter = string.Format("(ID={0})", flevmeas.m_idxbsstation);
                                        for (Station.OpenRs(); !Station.IsEOF(); Station.MoveNext())
                                        {
                                            SiteStationForMeas siteStationForMeas = new SiteStationForMeas();
                                            YXbsStationSite StationSite = new YXbsStationSite();
                                            StationSite.Format("*");
                                            StationSite.Filter = string.Format("(ID={0})", Station.m_id_xbs_stationsite);
                                            for (StationSite.OpenRs(); !StationSite.IsEOF(); StationSite.MoveNext())
                                            {
                                                siteStationForMeas.Lon = StationSite.m_lon;
                                                siteStationForMeas.Lat = StationSite.m_lat;
                                                siteStationForMeas.Adress = StationSite.m_addres;
                                                siteStationForMeas.Region = StationSite.m_region;
                                                ListStationSite.Add(siteStationForMeas);
                                            }
                                            StationSite.Close();
                                            StationSite.Dispose();
                                        }
                                        Station.Close();
                                        Station.Dispose();
                                        s_out.StationLocations = ListStationSite.ToArray();
                                        s_out.Standard = flevmeas.m_standard;
                                        
                                        YXbsResStGeneral XbsResStGeneral = new YXbsResStGeneral();
                                        XbsResStGeneral.Format("*");
                                        XbsResStGeneral.Filter = string.Format("(RESMEASSTATIONID={0})", flevmeas.m_id);
                                        for (XbsResStGeneral.OpenRs(); !XbsResStGeneral.IsEOF(); XbsResStGeneral.MoveNext())
                                        {
                                            s_out.CentralFrequencyMeas_MHz = XbsResStGeneral.m_centralfrequencymeas;
                                        }
                                        XbsResStGeneral.Close();
                                        XbsResStGeneral.Dispose();
                                        L_OUT.Add(s_out);
                                    }

                                }
                            }
                          
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ConvertTo_ShortResultsMeasurementsStation... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ConvertTo_ShortResultsMeasurementsStation...");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ConvertTo_ShortResultsMeasurementsStation..." + ex.Message);
            }
            return L_OUT.ToArray();
        }


        public MeasurementResults[] ConvertTo_SDRObjects(List<ClassSDRResults> objs)
        {
            List<MeasurementResults> L_OUT = new List<MeasurementResults>();
            try
            {
                logger.Trace("Start procedure ConvertTo_SDRObjects...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    { 
                    foreach (ClassSDRResults obj in objs.ToArray())
                    {
                        MeasurementResults s_out = new MeasurementResults();
                        s_out.Id = new MeasurementResultsIdentifier();
                        s_out.AntVal = obj.meas_res.m_antval;
                        s_out.DataRank = obj.meas_res.m_datarank;
                        s_out.Id.MeasTaskId = new MeasTaskIdentifier();
                        int MeasTaskId = -1; int.TryParse(obj.meas_res.m_meastaskid, out MeasTaskId);
                        s_out.Id.MeasTaskId.Value = MeasTaskId;
                        s_out.Id.MeasSdrResultsId = obj.meas_res.m_id.Value;
                        s_out.N = obj.meas_res.m_n;
                        s_out.StationMeasurements = new StationMeasurements();
                        s_out.StationMeasurements.StationId = new SensorIdentifier();
                        s_out.StationMeasurements.StationId.Value = obj.meas_res.m_sensorid.Value;
                        s_out.Status = obj.meas_res.m_status;
                        s_out.Id.SubMeasTaskId = obj.meas_res.m_submeastaskid.Value;
                        s_out.Id.SubMeasTaskStationId = obj.meas_res.m_submeastaskstationid.Value;
                        s_out.TimeMeas = (DateTime)obj.meas_res.m_timemeas;
                        MeasurementType out_res_type;
                        if (Enum.TryParse<MeasurementType>(obj.meas_res.m_typemeasurements, out out_res_type))
                            s_out.TypeMeasurements = out_res_type;
                        /// Freq
                        List<FrequencyMeasurement> L_FM = new List<FrequencyMeasurement>();
                        if (obj.resLevels != null)
                        {
                            foreach (YXbsResLevels fmeas in obj.resLevels.ToArray())
                            {
                                FrequencyMeasurement t_FM = new FrequencyMeasurement();
                                t_FM.Id = fmeas.m_id.Value;
                                //t_FM.Id = fmeas.m_nummeas.Value;
                                t_FM.Freq = fmeas.m_freqmeas.Value;
                                L_FM.Add(t_FM);
                            }
                        }
                        s_out.FrequenciesMeasurements = L_FM.ToArray();
                        /// Location
                        List<LocationSensorMeasurement> L_SM = new List<LocationSensorMeasurement>();
                            foreach (YXbsResLocSensorMeas fmeas in obj.loc_sensorM.ToArray())
                            {
                                LocationSensorMeasurement t_SM = new LocationSensorMeasurement();
                                t_SM.ASL = fmeas.m_asl;
                                t_SM.Lon = fmeas.m_lon;
                                t_SM.Lat = fmeas.m_lat;
                                L_SM.Add(t_SM);
                            }
                       
                        s_out.LocationSensorMeasurement = L_SM.ToArray();
                        List<MeasurementResult> L_MSR = new List<MeasurementResult>();
                        if (obj.resLevels != null)
                        {
                            if (obj.resLevels.Count > 0)
                            {
                                foreach (YXbsResLevels flevmeas in obj.resLevels.ToArray())
                                {
                                    if (obj.meas_res.m_typemeasurements == MeasurementType.Level.ToString())
                                    {
                                        LevelMeasurementResult rsd = new LevelMeasurementResult();
                                        rsd.Id = new MeasurementResultIdentifier();
                                        rsd.Id.Value = flevmeas.m_id.Value;
                                        rsd.Value = flevmeas.m_valuelvl;
                                        rsd.PMin = flevmeas.m_pminlvl;
                                        rsd.PMax = flevmeas.m_pmaxlvl;
                                        L_MSR.Add(rsd);
                                    }
                                }
                            }
                        }
                        if (obj.level_meas_onl_res != null)
                        {
                            if (obj.level_meas_onl_res.Count > 0)
                            {
                                foreach (YXbsResLevmeasonline flevmeas in obj.level_meas_onl_res.ToArray())
                                {
                                    LevelMeasurementOnlineResult rsd = new LevelMeasurementOnlineResult();
                                    rsd.Id = new MeasurementResultIdentifier();
                                    rsd.Id.Value = flevmeas.m_id.Value;
                                    rsd.Value = flevmeas.m_value.Value;
                                    L_MSR.Add(rsd);
                                }
                            }
                        }
                        if (obj.resLevels != null)
                        {
                            if (obj.resLevels.Count > 0)
                            {
                                foreach (YXbsResLevels flevmeas in obj.resLevels.ToArray())
                                {
                                    if (obj.meas_res.m_typemeasurements == MeasurementType.SpectrumOccupation.ToString())
                                    {
                                        SpectrumOccupationMeasurementResult rsd = new SpectrumOccupationMeasurementResult();
                                        rsd.Id = new MeasurementResultIdentifier();
                                        rsd.Id.Value = flevmeas.m_id.Value;
                                        rsd.Value = flevmeas.m_occupancyspect;
                                        L_MSR.Add(rsd);
                                    }
                                }
                            }
                        }

                        //////////////////////////////////////////////
                        if (obj.XbsResmeasstation != null)
                        {
                            if (obj.XbsResmeasstation.Count > 0)
                            {
                                s_out.ResultsMeasStation = new ResultsMeasurementsStation[obj.XbsResmeasstation.Count];
                                int ii = 0;
                                foreach (YXbsResmeasstation flevmeas in obj.XbsResmeasstation.ToArray())
                                {
                                    s_out.ResultsMeasStation[ii] = new ResultsMeasurementsStation();
                                    if (flevmeas.m_idsector!=null) s_out.ResultsMeasStation[ii].IdSector = flevmeas.m_idsector;
                                    if (flevmeas.m_idstation!=null) s_out.ResultsMeasStation[ii].Idstation = flevmeas.m_idstation.Value.ToString();
                                    s_out.ResultsMeasStation[ii].GlobalSID = flevmeas.m_globalsid;
                                    s_out.ResultsMeasStation[ii].MeasGlobalSID = flevmeas.m_measglobalsid;
                                    s_out.ResultsMeasStation[ii].Status = flevmeas.m_status;
                                    if (obj.XbsResLevelMeas != null)
                                    {
                                        List<YXbsResStLevelCar> resF = obj.XbsResLevelMeas.FindAll(t => t.m_xbs_resmeasstationid == flevmeas.m_id);
                                        if (resF.Count > 0)
                                        {
                                            s_out.ResultsMeasStation[ii].LevelMeasurements = new LevelMeasurementsCar[resF.Count];
                                            int u = 0;
                                            foreach (YXbsResStLevelCar x in resF)
                                            {
                                                s_out.ResultsMeasStation[ii].LevelMeasurements[u] = new LevelMeasurementsCar();
                                                if (x.m_altitude!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].Altitude = x.m_altitude;
                                                if (x.m_bw!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].BW = x.m_bw;
                                                if (x.m_centralfrequency!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].CentralFrequency = (decimal?)x.m_centralfrequency;
                                                if (x.m_differencetimestamp!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].DifferenceTimestamp = x.m_differencetimestamp;
                                                if (x.m_lat!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].Lat = x.m_lat;
                                                if (x.m_lon!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].Lon = x.m_lon;
                                                if (x.m_leveldbm!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].LeveldBm = x.m_leveldbm;
                                                if (x.m_leveldbmkvm!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].LeveldBmkVm = x.m_leveldbmkvm;
                                                if (x.m_rbw!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].RBW = x.m_rbw;
                                                if (x.m_timeofmeasurements!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].TimeOfMeasurements = x.m_timeofmeasurements.GetValueOrDefault();
                                                if (x.m_vbw!=null) s_out.ResultsMeasStation[ii].LevelMeasurements[u].VBW = x.m_vbw;
                                                u++;
                                            }
                                        }
                                    }


                                    if (obj.XbsResGeneral != null)
                                    {
                                        s_out.ResultsMeasStation[ii].GeneralResult = new MeasurementsParameterGeneral();
                                        List<YXbsResStGeneral> resF = obj.XbsResGeneral.FindAll(t => t.m_resmeasstationid == flevmeas.m_id);
                                        if (resF!=null)
                                        {
                                            foreach (YXbsResStGeneral x in resF)
                                            {
                                                if (x.m_centralfrequency!=null) s_out.ResultsMeasStation[ii].GeneralResult.CentralFrequency = x.m_centralfrequency;
                                                if (x.m_centralfrequencymeas!=null) s_out.ResultsMeasStation[ii].GeneralResult.CentralFrequencyMeas = x.m_centralfrequencymeas;
                                                if (x.m_durationmeas!=null) s_out.ResultsMeasStation[ii].GeneralResult.DurationMeas = x.m_durationmeas;
                                                if (x.m_markerindex!=null) s_out.ResultsMeasStation[ii].GeneralResult.MarkerIndex = x.m_markerindex;
                                                if (x.m_offsetfrequency!=null) s_out.ResultsMeasStation[ii].GeneralResult.OffsetFrequency = x.m_offsetfrequency;
                                                if (x.m_specrumstartfreq!=null) s_out.ResultsMeasStation[ii].GeneralResult.SpecrumStartFreq = (decimal?)x.m_specrumstartfreq;
                                                if (x.m_specrumsteps!=null) s_out.ResultsMeasStation[ii].GeneralResult.SpecrumSteps = (decimal?)x.m_specrumsteps;
                                                if (x.m_t1!=null) s_out.ResultsMeasStation[ii].GeneralResult.T1 = x.m_t1;
                                                if (x.m_t2!=null) s_out.ResultsMeasStation[ii].GeneralResult.T2 = x.m_t2;
                                                if (x.m_timefinishmeas!=null) s_out.ResultsMeasStation[ii].GeneralResult.TimeFinishMeas = x.m_timefinishmeas;
                                                if (x.m_timestartmeasdate!=null)  s_out.ResultsMeasStation[ii].GeneralResult.TimeStartMeas = x.m_timestartmeasdate;

                                                if (obj.XbsResmaskBw != null)
                                                {
                                                    List<YXbsResStMaskElm> resYXbsResmaskBw = obj.XbsResmaskBw.FindAll(t => t.m_xbs_resstgeneralid == x.m_id);
                                                    if (resYXbsResmaskBw.Count > 0)
                                                    {
                                                        s_out.ResultsMeasStation[ii].GeneralResult.MaskBW = new MaskElements[resYXbsResmaskBw.Count];
                                                        int u = 0;
                                                        foreach (YXbsResStMaskElm xv in resYXbsResmaskBw)
                                                        {
                                                            s_out.ResultsMeasStation[ii].GeneralResult.MaskBW[u] = new MaskElements();
                                                            if (xv.m_bw!=null) s_out.ResultsMeasStation[ii].GeneralResult.MaskBW[u].BW = xv.m_bw;
                                                            if (xv.m_level != null) s_out.ResultsMeasStation[ii].GeneralResult.MaskBW[u].level = xv.m_level;
                                                            u++;
                                                        }
                                                    }
                                                }

                                                if (obj.XbsLevelSpecrum != null)
                                                {
                                                    List<YXbsResStLevelsSpect> resYXbsLevelSpecrum = obj.XbsLevelSpecrum.FindAll(t => t.m_xbs_resstgeneralid == x.m_id);
                                                    if (resYXbsLevelSpecrum.Count > 0)
                                                    {
                                                        s_out.ResultsMeasStation[ii].GeneralResult.LevelsSpecrum = new float[resYXbsLevelSpecrum.Count];
                                                        int u = 0;
                                                        foreach (YXbsResStLevelsSpect xv in resYXbsLevelSpecrum)
                                                        {
                                                            s_out.ResultsMeasStation[ii].GeneralResult.LevelsSpecrum[u] = new float();
                                                            if (xv.m_levelspecrum!=null) s_out.ResultsMeasStation[ii].GeneralResult.LevelsSpecrum[u] = (float)xv.m_levelspecrum;
                                                            u++;
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                    ii++;
                                }

                                


                            }
                        }
 
                        s_out.MeasurementsResults = L_MSR.ToArray();

                        L_OUT.Add(s_out);
                    }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ConvertTo_SDRObjects... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ConvertTo_SDRObjects...");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ConvertTo_SDRObjects..." + ex.Message);
            }
            return L_OUT.ToArray();
        }

        /// <summary>
        /// Метод, выполняющий 
        /// </summary>
        /// <param name="sdr_t"></param>
        /// <returns></returns>
        public static MeasurementResults GenerateMeasResults(MeasSdrResults sdr_t)
        {
            MeasurementResults res = new MeasurementResults();
            if (sdr_t!=null) res = sdr_t.CreateMeasurementResults();
            return res;
        }
    }
}