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
                                    if (flevmeas.m_idstation!=null) s_out.ResultsMeasStation[ii].Idstation = flevmeas.m_idstation;
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