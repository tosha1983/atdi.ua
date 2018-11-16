using System;
using System.Collections.Generic;
using Atdi.Oracle.DataAccess;
using Atdi.AppServer;
using System.Runtime.Serialization.Formatters.Binary;
using Atdi.AppServer.Contracts.Sdrns;
using System.IO;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    public class ClassConvertToSDRResultsOptimize : IDisposable
    {
        public static ILogger logger;
        public ClassConvertToSDRResultsOptimize(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassConvertToSDRResultsOptimize()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public List<MeasurementResults> ConvertMeasurementResults(List<ClassSDRResults> objs)
        {
            List<MeasurementResults> L_OUT = new List<MeasurementResults>();
            try
            {
                logger.Trace("Start procedure ConvertMeasurementResults...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        foreach (ClassSDRResults obj in objs)
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
                                foreach (YXbsResLevels fmeas in obj.resLevels)
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
                            foreach (YXbsResLocSensorMeas fmeas in obj.loc_sensorM)
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
                                    foreach (YXbsResLevels flevmeas in obj.resLevels)
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
                                    foreach (YXbsResLevmeasonline flevmeas in obj.level_meas_onl_res)
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
                                    foreach (YXbsResLevels flevmeas in obj.resLevels)
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
            return L_OUT;
        }


        public List<KeyValuePair<MeasurementResults, string>> ConvertMeasurementResultsExt(List<ClassSDRResults> objs)
        {
            List<KeyValuePair<MeasurementResults, string>> L_OUT = new List<KeyValuePair<MeasurementResults, string>>();
            string SensorName = "";
            try
            {
                logger.Trace("Start procedure ConvertMeasurementResults...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        foreach (ClassSDRResults obj in objs)
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
                                foreach (YXbsResLevels fmeas in obj.resLevels)
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
                            foreach (YXbsResLocSensorMeas fmeas in obj.loc_sensorM)
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
                                    foreach (YXbsResLevels flevmeas in obj.resLevels)
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
                                    foreach (YXbsResLevmeasonline flevmeas in obj.level_meas_onl_res)
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
                                    foreach (YXbsResLevels flevmeas in obj.resLevels)
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
                            SensorName = "";
                            if (obj.SensorName != null)
                            {
                                SensorName = obj.SensorName;
                            }

                            s_out.MeasurementsResults = L_MSR.ToArray();
                            L_OUT.Add(new KeyValuePair<MeasurementResults, string>(s_out, SensorName));
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
            return L_OUT;
        }


        public MeasurementResults ConvertMeasurementResultsOne(List<ClassSDRResults> objs)
        {
            MeasurementResults L_OUT = new MeasurementResults();
            try
            {
                logger.Trace("Start procedure ConvertMeasurementResults...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        foreach (ClassSDRResults obj in objs)
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
                                foreach (YXbsResLevels fmeas in obj.resLevels)
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
                            foreach (YXbsResLocSensorMeas fmeas in obj.loc_sensorM)
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
                                    foreach (YXbsResLevels flevmeas in obj.resLevels)
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
                                    foreach (YXbsResLevmeasonline flevmeas in obj.level_meas_onl_res)
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
                                    foreach (YXbsResLevels flevmeas in obj.resLevels)
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
                            s_out.MeasurementsResults = L_MSR.ToArray();
                            L_OUT = s_out;
                            break;
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
            return L_OUT;
        }


        public List<MeasurementResults> ConvertHeader(List<ClassSDRResults> objs)
        {
            List<MeasurementResults> L_OUT = new List<MeasurementResults>();
            try
            {
                logger.Trace("Start procedure ConvertHeader...");
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
                            L_OUT.Add(s_out);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ConvertHeader... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ConvertHeader...");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ConvertHeader..." + ex.Message);
            }
            return L_OUT;
        }



        public List<ResultsMeasurementsStationExtended> ConvertStationHeaders(List<ClassSDRResults> objs)
        {
            List<ResultsMeasurementsStationExtended> L_OUT = new List<ResultsMeasurementsStationExtended>();
            try
            {
                logger.Trace("Start procedure ConvertTo_SDRObjects...");
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
                                    ResultsMeasurementsStationExtended[] ResultsMeasStation = new ResultsMeasurementsStationExtended[obj.XbsResmeasstation.Count];
                                    int ii = 0;
                                    foreach (YXbsResmeasstation flevmeas in obj.XbsResmeasstation.ToArray())
                                    {
                                        ResultsMeasStation[ii] = new ResultsMeasurementsStationExtended();
                                        ResultsMeasStation[ii].Id = flevmeas.m_id.Value;
                                        if (flevmeas.m_idsector != null) ResultsMeasStation[ii].IdSector = flevmeas.m_idsector;
                                        if (flevmeas.m_idstation != null) ResultsMeasStation[ii].Idstation = flevmeas.m_idstation.Value.ToString();
                                        ResultsMeasStation[ii].GlobalSID = flevmeas.m_globalsid;
                                        ResultsMeasStation[ii].MeasGlobalSID = flevmeas.m_measglobalsid;
                                        ResultsMeasStation[ii].Status = flevmeas.m_status;
                                        ii++;
                                    }
                                    L_OUT.AddRange(ResultsMeasStation);
                                }
                            }
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
            return L_OUT;
        }


        public ResultsMeasurementsStation ConvertTo_ResultsMeasurementsOneStation(List<ClassSDRResults> objs)
        {
            ResultsMeasurementsStation L_OUT = new ResultsMeasurementsStation();
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

                                                    if (x.m_resstmaskelm != null)
                                                    {
                                                        object m_resstmaskelm = Deserialize<MaskElements[]>(x.m_resstmaskelm);
                                                        if (m_resstmaskelm != null)
                                                        {
                                                            ResultsMeasStation.GeneralResult.MaskBW = m_resstmaskelm as MaskElements[];
                                                        }
                                                    }
                                                    else
                                                    {

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
                                                    }

                                                    if (x.m_resstlevelsspect != null)
                                                    {
                                                        object m_resstlevelsspect = Deserialize<float[]>(x.m_resstlevelsspect);
                                                        if (m_resstlevelsspect != null)
                                                        {
                                                            ResultsMeasStation.GeneralResult.LevelsSpecrum = m_resstlevelsspect as float[];
                                                        }
                                                    }
                                                    else
                                                    {
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
                                        }
                                        ii++;
                                        L_OUT = ResultsMeasStation;
                                        break;
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
            return L_OUT;
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

                                                    if (x.m_resstmaskelm != null)
                                                    {
                                                        object m_resstmaskelm = Deserialize<MaskElements[]>(x.m_resstmaskelm);
                                                        if (m_resstmaskelm != null)
                                                        {
                                                            ResultsMeasStation.GeneralResult.MaskBW = m_resstmaskelm as MaskElements[];
                                                        }
                                                    }
                                                    else
                                                    {

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
                                                    }

                                                    if (x.m_resstlevelsspect != null)
                                                    {
                                                        object m_resstlevelsspect = Deserialize<float[]>(x.m_resstlevelsspect);
                                                        if (m_resstlevelsspect != null)
                                                        {
                                                            ResultsMeasStation.GeneralResult.LevelsSpecrum = m_resstlevelsspect as float[];
                                                        }
                                                    }
                                                    else
                                                    {
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

        private T Deserialize<T>(byte[] param)
        {
            T val = default(T);
            if (param != null)
            {
                using (MemoryStream ms = new MemoryStream(param))
                {
                    BinaryFormatter br = new BinaryFormatter();
                    val = (T)br.Deserialize(ms);
                }
            }
            return val;
        }
    }
}
 