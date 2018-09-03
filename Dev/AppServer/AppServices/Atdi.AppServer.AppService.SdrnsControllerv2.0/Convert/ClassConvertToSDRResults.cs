using System;
using System.Collections.Generic;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{
    public class ClassConvertToSDRResults
    {
        public static ILogger logger;
        public ClassConvertToSDRResults(ILogger log)
        {
            if (logger == null) logger = log;
        }
        public MeasurementResults[] ConvertTo_SDRObjects(List<ClassSDRResults> objs)
        {
            List<MeasurementResults> L_OUT = new List<MeasurementResults>();
            try
            {
                logger.Trace("Start procedure ConvertTo_SDRObjects...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    foreach (ClassSDRResults obj in objs.ToArray())
                    {
                        MeasurementResults s_out = new MeasurementResults();
                        s_out.Id = new MeasurementResultsIdentifier();
                        s_out.AntVal = obj.meas_res.m_antval;
                        s_out.DataRank = obj.meas_res.m_datarank;
                        s_out.Id.MeasTaskId = new MeasTaskIdentifier();
                        s_out.Id.MeasTaskId.Value = obj.meas_res.m_meastaskid.Value;
                        s_out.Id.MeasSdrResultsId = obj.meas_res.m_id.Value;
                        s_out.N = obj.meas_res.m_n;
                        s_out.StationMeasurements = new StationMeasurements();
                        s_out.StationMeasurements.StationId = new SensorIdentifier();
                        s_out.StationMeasurements.StationId.Value = obj.meas_res.m_sensorid.Value;
                        s_out.Status = obj.meas_res.m_status;
                        s_out.Id.SubMeasTaskId = obj.meas_res.m_submeastaskid.Value;
                        s_out.Id.SubMeasTaskStationId = obj.meas_res.m_submeastaskstationid.Value;
                        s_out.TimeMeas = obj.meas_res.m_timemeas.Value;
                        MeasurementType out_res_type;
                        if (Enum.TryParse<MeasurementType>(obj.meas_res.m_typemeasurements, out out_res_type))
                            s_out.TypeMeasurements = out_res_type;
                        /// Freq
                        List<FrequencyMeasurement> L_FM = new List<FrequencyMeasurement>();
                        if (obj.freq_meas != null)
                        {
                            foreach (YXbsFrequencymeas fmeas in obj.freq_meas.ToArray())
                            {
                                FrequencyMeasurement t_FM = new FrequencyMeasurement();
                                t_FM.Id = fmeas.m_num.Value;
                                t_FM.Freq = fmeas.m_freq.Value;
                                L_FM.Add(t_FM);
                            }
                        }
                        s_out.FrequenciesMeasurements = L_FM.ToArray();
                        /// Location
                        List<LocationSensorMeasurement> L_SM = new List<LocationSensorMeasurement>();
                        if (obj.freq_meas != null)
                        {
                            foreach (YXbsLocationsensorm fmeas in obj.loc_sensorM.ToArray())
                            {
                                LocationSensorMeasurement t_SM = new LocationSensorMeasurement();
                                t_SM.ASL = fmeas.m_asl;
                                t_SM.Lon = fmeas.m_lon;
                                t_SM.Lat = fmeas.m_lat;
                                L_SM.Add(t_SM);
                            }
                        }
                        s_out.LocationSensorMeasurement = L_SM.ToArray();
                        List<MeasurementResult> L_MSR = new List<MeasurementResult>();
                        if (obj.level_meas_res != null)
                        {
                            if (obj.level_meas_res.Count > 0)
                            {
                                foreach (YXbsLevelmeasres flevmeas in obj.level_meas_res.ToArray())
                                {
                                    if (obj.meas_res.m_typemeasurements == MeasurementType.Level.ToString())
                                    {
                                        LevelMeasurementResult rsd = new LevelMeasurementResult();
                                        rsd.Id = new MeasurementResultIdentifier();
                                        rsd.Id.Value = flevmeas.m_id.Value;
                                        rsd.Value = flevmeas.m_value;
                                        rsd.PMin = flevmeas.m_pmin;
                                        rsd.PMax = flevmeas.m_pmax;
                                        L_MSR.Add(rsd);
                                    }
                                }
                            }
                        }
                        if (obj.level_meas_onl_res != null)
                        {
                            if (obj.level_meas_onl_res.Count > 0)
                            {
                                foreach (YXbsLevelmeasonlres flevmeas in obj.level_meas_onl_res.ToArray())
                                {
                                    LevelMeasurementOnlineResult rsd = new LevelMeasurementOnlineResult();
                                    rsd.Id = new MeasurementResultIdentifier();
                                    rsd.Id.Value = flevmeas.m_id.Value;
                                    rsd.Value = flevmeas.m_value.Value;
                                    L_MSR.Add(rsd);
                                }
                            }
                        }
                        if (obj.spect_occup_meas != null)
                        {
                            if (obj.spect_occup_meas.Count > 0)
                            {
                                foreach (YXbsSpectoccupmeas flevmeas in obj.spect_occup_meas.ToArray())
                                {
                                    if (obj.meas_res.m_typemeasurements == MeasurementType.SpectrumOccupation.ToString())
                                    {
                                        SpectrumOccupationMeasurementResult rsd = new SpectrumOccupationMeasurementResult();
                                        rsd.Id = new MeasurementResultIdentifier();
                                        rsd.Id.Value = flevmeas.m_id.Value;
                                        rsd.Value = flevmeas.m_occupancy;
                                        L_MSR.Add(rsd);
                                    }
                                }
                            }
                        }
                        s_out.MeasurementsResults = L_MSR.ToArray();
                        L_OUT.Add(s_out);
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
        public static MeasurementResults GenerateMeasResults2_0(Atdi.DataModels.Sdrns.Device.MeasResults sdr_t)
        {
            MeasurementResults res = new MeasurementResults();
            if (sdr_t != null) res = sdr_t.CreateMeasurementResults2_0();
            return res;
        }
    }
}