using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.DataModels.Sdrns;
using Atdi.DataModels.Sdrns.Device;
using System.Data.Common;
using DM = Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{
    public class ClassDBGetSensor 
    {
        public static ILogger logger;
        public ClassDBGetSensor(ILogger log)
        {
            if (logger == null) logger = log;
        }


        public List<DM.Sensor> LoadObjectAllSensorAPI1_0()
        {
            var val = new List<DM.Sensor>();
            {
                try
                {
                    logger.Trace("Start procedure LoadObjectAllSensor...");
                    System.Threading.Thread tsk = new System.Threading.Thread(() =>
                    {
                        YXbsSensor s_l_sensor = new YXbsSensor();
                        s_l_sensor.Format("*");
                        // выбирать только сенсоры, для которых STATUS не NULL
                        // AND ((APIVERSION <> '2.0') OR  (APIVERSION IS NULL))
                        s_l_sensor.Filter = string.Format("(ID>0)");
                        s_l_sensor.Order = "[ID] DESC";
                        for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                        {
                            DM.Sensor it_out = new DM.Sensor();
                            it_out.Id = new DM.SensorIdentifier();
                            it_out.Administration = s_l_sensor.m_administration;
                            it_out.AGL = s_l_sensor.m_agl;
                            it_out.Azimuth = s_l_sensor.m_azimuth;
                            it_out.BiuseDate = s_l_sensor.m_biusedate;
                            it_out.CreatedBy = s_l_sensor.m_createdby;
                            it_out.CustData1 = s_l_sensor.m_custdata1;
                            it_out.CustNbr1 = s_l_sensor.m_custnbr1;
                            it_out.CustTxt1 = s_l_sensor.m_custtxt1;
                            it_out.DateCreated = s_l_sensor.m_datecreated;
                            it_out.Elevation = s_l_sensor.m_elevation;
                            it_out.EouseDate = s_l_sensor.m_eousedate;
                            it_out.IdSysARGUS = s_l_sensor.m_idsysargus;
                            if (s_l_sensor.m_id != null) it_out.Id.Value = (int)s_l_sensor.m_id;
                            it_out.Name = s_l_sensor.m_name;
                            it_out.NetworkId = s_l_sensor.m_networkid;
                            it_out.OpDays = s_l_sensor.m_opdays;
                            it_out.OpHHFr = s_l_sensor.m_ophhfr;
                            it_out.OpHHTo = s_l_sensor.m_ophhto;
                            it_out.Remark = s_l_sensor.m_remark;
                            it_out.RxLoss = s_l_sensor.m_rxloss;
                            it_out.Status = s_l_sensor.m_status;
                            it_out.StepMeasTime = s_l_sensor.m_stepmeastime;
                            it_out.TypeSensor = s_l_sensor.m_typesensor;
                            it_out.Antenna = new DM.SensorAntenna();

                            //Antenna
                            YXbsSensorantenna mpt = new YXbsSensorantenna();
                            mpt.Format("*");
                            mpt.Filter = string.Format("SENSORID={0}", it_out.Id.Value);
                            for (mpt.OpenRs(); !mpt.IsEOF(); mpt.MoveNext())
                            {
                                List<DM.AntennaPattern> L_ant_patt = new List<DM.AntennaPattern>();
                                YXbsAntennapattern itr = new YXbsAntennapattern();
                                itr.Format("*");
                                itr.Filter = string.Format("SENSORANTENNA_ID={0}", mpt.m_id);
                                for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                {
                                    DM.AntennaPattern ptu = new DM.AntennaPattern();
                                    ptu.DiagA = itr.m_diaga;
                                    ptu.DiagH = itr.m_diagh;
                                    ptu.DiagV = itr.m_diagv;
                                    if (itr.m_freq != null) ptu.Freq = itr.m_freq.Value;
                                    if (itr.m_gain != null) ptu.Gain = itr.m_gain.Value;
                                    L_ant_patt.Add(ptu);
                                }
                                itr.Close();
                                itr.Dispose();
                                it_out.Antenna.AntennaPatterns = L_ant_patt.ToArray();
                            }
                            mpt.Close();
                            mpt.Dispose();

                            // Equipments
                            it_out.Equipment = new DM.SensorEquip();
                            {
                                YXbsSensorequip mpt_ = new YXbsSensorequip();
                                mpt_.Format("*");
                                mpt_.Filter = string.Format("(SENSORID={0})", it_out.Id.Value);
                                for (mpt_.OpenRs(); !mpt_.IsEOF(); mpt_.MoveNext())
                                {
                                    it_out.Equipment.Category = mpt_.m_category;
                                    it_out.Equipment.Code = mpt_.m_code;
                                    it_out.Equipment.CustData1 = mpt_.m_custdata1;
                                    it_out.Equipment.CustNbr1 = mpt_.m_custnbr1;
                                    it_out.Equipment.CustTxt1 = mpt_.m_custtxt1;
                                    it_out.Equipment.EquipClass = mpt_.m_equipclass;
                                    it_out.Equipment.Family = mpt_.m_family;
                                    it_out.Equipment.FFTPointMax = mpt_.m_fftpointmax;
                                    it_out.Equipment.LowerFreq = mpt_.m_lowerfreq;
                                    it_out.Equipment.Manufacturer = mpt_.m_manufacturer;
                                    it_out.Equipment.Mobility = mpt_.m_mobility == 1 ? true : false;
                                    it_out.Equipment.Name = mpt_.m_name;
                                    it_out.Equipment.OperationMode = mpt_.m_operationmode;
                                    it_out.Equipment.RBWMax = mpt_.m_rbwmax;
                                    it_out.Equipment.RBWMin = mpt_.m_rbwmin;
                                    it_out.Equipment.RefLeveldBm = mpt_.m_refleveldbm;
                                    it_out.Equipment.Remark = mpt_.m_remark;
                                    it_out.Equipment.TechId = mpt_.m_techid;
                                    it_out.Equipment.TuningStep = mpt_.m_tuningstep;
                                    it_out.Equipment.Type = mpt_.m_type;
                                    it_out.Equipment.UpperFreq = mpt_.m_upperfreq;
                                    it_out.Equipment.UseType = mpt_.m_usetype;
                                    it_out.Equipment.VBWMax = mpt_.m_vbwmax;
                                    it_out.Equipment.VBWMin = mpt_.m_vbwmin;
                                    it_out.Equipment.Version = mpt_.m_version;

                                    List<DM.SensorEquipSensitivity> L_sens = new List<DM.SensorEquipSensitivity>();
                                    YXbsSensorequipsens itr = new YXbsSensorequipsens();
                                    itr.Format("*");
                                    itr.Filter = string.Format("SENSOREQUIP_ID={0}", mpt_.m_id);
                                    for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                    {
                                        DM.SensorEquipSensitivity ptu = new DM.SensorEquipSensitivity();
                                        ptu.AddLoss = itr.m_addloss;
                                        if (itr.m_freq != null) ptu.Freq = itr.m_freq.Value;
                                        ptu.FreqStability = itr.m_freqstability;
                                        ptu.KTBF = itr.m_ktbf;
                                        ptu.NoiseF = itr.m_noisef;
                                        L_sens.Add(ptu);
                                    }
                                    itr.Close();
                                    itr.Dispose();
                                    it_out.Equipment.SensorEquipSensitivities = L_sens.ToArray();
                                }
                                mpt_.Close();
                                mpt_.Dispose();
                            }

                            List<DM.SensorLocation> L_Sens_loc = new List<DM.SensorLocation>();
                            YXbsSensorlocation mpt_loc = new YXbsSensorlocation();
                            mpt_loc.Format("*");
                            mpt_loc.Filter = string.Format("(SENSORID={0})", it_out.Id.Value);
                            for (mpt_loc.OpenRs(); !mpt_loc.IsEOF(); mpt_loc.MoveNext())
                            {
                                DM.SensorLocation s_l = new DM.SensorLocation();
                                s_l.ASL = mpt_loc.m_asl;
                                s_l.DataCreated = mpt_loc.m_datacreated;
                                s_l.DataFrom = mpt_loc.m_datafrom;
                                s_l.DataTo = mpt_loc.m_datato;
                                s_l.Lat = mpt_loc.m_lat;
                                s_l.Lon = mpt_loc.m_lon;
                                s_l.Status = mpt_loc.m_status;
                                L_Sens_loc.Add(s_l);
                            }
                            mpt_loc.Close();
                            mpt_loc.Dispose();
                            it_out.Locations = L_Sens_loc.ToArray();
                            val.Add(it_out);
                            //RescanSensors(it_out);
                        }
                        s_l_sensor.Close();
                        s_l_sensor.Dispose();
                    });
                    tsk.Start();
                    tsk.Join();
                    logger.Trace("End procedure LoadObjectAllSensor.");
                }
                catch (Exception ex)
                {
                    logger.Error("Error in procedure LoadObjectAllSensor: " + ex.Message);
                }

            }
            return val;
        }


        public List<Sensor> LoadObjectAllSensorAPI2_0()
        {
            var val = new List<Sensor>();
            {
                try
                {
                    logger.Trace("Start procedure LoadObjectAllSensor...");
                    System.Threading.Thread tsk = new System.Threading.Thread(() =>
                    {
                        try
                        {
                            YXbsSensor s_l_sensor = new YXbsSensor();
                            s_l_sensor.Format("*");
                            // выбирать только сенсоры, для которых STATUS не NULL
                            s_l_sensor.Filter = string.Format("(ID>0)");
                            s_l_sensor.Order = "[ID] DESC";
                            for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                            {
                                Sensor it_out = new Sensor();
                                it_out.Administration = s_l_sensor.m_administration;
                                it_out.BiuseDate = s_l_sensor.m_biusedate;
                                it_out.CreatedBy = s_l_sensor.m_createdby;
                                it_out.CustDate1 = s_l_sensor.m_custdata1;
                                it_out.CustNbr1 = s_l_sensor.m_custnbr1;
                                it_out.CustTxt1 = s_l_sensor.m_custtxt1;
                                it_out.Created = s_l_sensor.m_datecreated;
                                it_out.EouseDate = s_l_sensor.m_eousedate;
                                it_out.Name = s_l_sensor.m_name;
                                it_out.NetworkId = s_l_sensor.m_networkid;
                                it_out.Remark = s_l_sensor.m_remark;
                                it_out.RxLoss = s_l_sensor.m_rxloss;
                                it_out.Status = s_l_sensor.m_status;
                                it_out.StepMeasTime = s_l_sensor.m_stepmeastime;
                                it_out.Type = s_l_sensor.m_typesensor;
                                it_out.Antenna = new SensorAntenna();

                                //Antenna
                                YXbsSensorantenna mpt = new YXbsSensorantenna();
                                mpt.Format("*");
                                mpt.Filter = string.Format("SENSORID={0}", s_l_sensor.m_id.Value);
                                for (mpt.OpenRs(); !mpt.IsEOF(); mpt.MoveNext())
                                {
                                    List<AntennaPattern> L_ant_patt = new List<AntennaPattern>();
                                    YXbsAntennapattern itr = new YXbsAntennapattern();
                                    itr.Format("*");
                                    itr.Filter = string.Format("SENSORANTENNA_ID={0}", mpt.m_id);
                                    for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                    {
                                        AntennaPattern ptu = new AntennaPattern();
                                        ptu.DiagA = itr.m_diaga;
                                        ptu.DiagH = itr.m_diagh;
                                        ptu.DiagV = itr.m_diagv;
                                        ptu.Freq_MHz = itr.m_freq.Value;
                                        ptu.Gain = itr.m_gain.Value;
                                        L_ant_patt.Add(ptu);
                                    }
                                    itr.Close();
                                    itr.Dispose();
                                    it_out.Antenna.Patterns = L_ant_patt.ToArray();
                                }
                                mpt.Close();
                                mpt.Dispose();

                                // Equipments
                                it_out.Equipment = new SensorEquipment();
                                {
                                    YXbsSensorequip mpt_ = new YXbsSensorequip();
                                    mpt_.Format("*");
                                    mpt_.Filter = string.Format("(SENSORID={0})", s_l_sensor.m_id.Value);
                                    for (mpt_.OpenRs(); !mpt_.IsEOF(); mpt_.MoveNext())
                                    {
                                        it_out.Equipment.Category = mpt_.m_category;
                                        it_out.Equipment.Code = mpt_.m_code;
                                        it_out.Equipment.CustDate1 = mpt_.m_custdata1;
                                        it_out.Equipment.CustNbr1 = mpt_.m_custnbr1;
                                        it_out.Equipment.CustTxt1 = mpt_.m_custtxt1;
                                        it_out.Equipment.Class = mpt_.m_equipclass;
                                        it_out.Equipment.Family = mpt_.m_family;
                                        it_out.Equipment.FFTPointMax = mpt_.m_fftpointmax;
                                        it_out.Equipment.LowerFreq_MHz = mpt_.m_lowerfreq;
                                        it_out.Equipment.Manufacturer = mpt_.m_manufacturer;
                                        it_out.Equipment.Mobility = mpt_.m_mobility == 1 ? true : false;
                                        it_out.Equipment.Name = mpt_.m_name;
                                        it_out.Equipment.OperationMode = mpt_.m_operationmode;
                                        it_out.Equipment.RBWMax_kHz = mpt_.m_rbwmax;
                                        it_out.Equipment.RBWMin_kHz = mpt_.m_rbwmin;
                                        it_out.Equipment.MaxRefLevel_dBm = mpt_.m_refleveldbm;
                                        it_out.Equipment.Remark = mpt_.m_remark;
                                        it_out.Equipment.TechId = mpt_.m_techid;
                                        it_out.Equipment.TuningStep_Hz = mpt_.m_tuningstep;
                                        it_out.Equipment.Type = mpt_.m_type;
                                        it_out.Equipment.UpperFreq_MHz = mpt_.m_upperfreq;
                                        it_out.Equipment.UseType = mpt_.m_usetype;
                                        it_out.Equipment.VBWMax_kHz = mpt_.m_vbwmax;
                                        it_out.Equipment.VBWMin_kHz = mpt_.m_vbwmin;
                                        it_out.Equipment.Version = mpt_.m_version;

                                        List<EquipmentSensitivity> L_sens = new List<EquipmentSensitivity>();
                                        YXbsSensorequipsens itr = new YXbsSensorequipsens();
                                        itr.Format("*");
                                        itr.Filter = string.Format("SENSOREQUIP_ID={0}", mpt_.m_id);
                                        for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                        {

                                            EquipmentSensitivity ptu = new EquipmentSensitivity();
                                            ptu.AddLoss = itr.m_addloss;
                                            ptu.Freq_MHz = itr.m_freq.Value;
                                            ptu.FreqStability = itr.m_freqstability;
                                            ptu.KTBF_dBm = itr.m_ktbf;
                                            ptu.NoiseF = itr.m_noisef;
                                            L_sens.Add(ptu);
                                        }
                                        itr.Close();
                                        itr.Dispose();
                                        it_out.Equipment.Sensitivities = L_sens.ToArray();
                                    }
                                    mpt_.Close();
                                    mpt_.Dispose();
                                }

                                List<SensorLocation> L_Sens_loc = new List<SensorLocation>();
                                YXbsSensorlocation mpt_loc = new YXbsSensorlocation();
                                mpt_loc.Format("*");
                                mpt_loc.Filter = string.Format("(SENSORID={0})", s_l_sensor.m_id.Value);
                                for (mpt_loc.OpenRs(); !mpt_loc.IsEOF(); mpt_loc.MoveNext())
                                {
                                    SensorLocation s_l = new SensorLocation();
                                    s_l.ASL = mpt_loc.m_asl;
                                    s_l.Created = mpt_loc.m_datacreated;
                                    s_l.From = mpt_loc.m_datafrom;
                                    s_l.To = mpt_loc.m_datato;
                                    s_l.Lat = mpt_loc.m_lat.Value;
                                    s_l.Lon = mpt_loc.m_lon.Value;
                                    s_l.Status = mpt_loc.m_status;
                                    L_Sens_loc.Add(s_l);
                                }
                                mpt_loc.Close();
                                mpt_loc.Dispose();
                                val.Add(it_out);
                            }
                            s_l_sensor.Close();
                            s_l_sensor.Dispose();
                        }
                        catch (Exception ex)
                        {
                            logger.Trace("Error in procedure LoadObjectAllSensor... " + ex.Message);
                        }
                    });
                    tsk.Start();
                    tsk.Join();
                    logger.Trace("End procedure LoadObjectAllSensor.");
                }
                catch (Exception ex)
                {
                    logger.Error("Error in procedure LoadObjectAllSensor: " + ex.Message);
                }

            }
            return val;
        }

        public Sensor LoadObjectSensor(int Id)
        {
            var val = new Sensor();
            {
                try
                {
                    logger.Trace("Start procedure LoadObjectAllSensor...");
                    System.Threading.Thread tsk = new System.Threading.Thread(() =>
                    {
                        try
                        {
                            YXbsSensor s_l_sensor = new YXbsSensor();
                            s_l_sensor.Format("*");
                            s_l_sensor.Filter = string.Format("(ID={0})", Id);
                            s_l_sensor.Order = "[ID] DESC";
                            for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                            {
                                Sensor it_out = new Sensor();
                                it_out.Administration = s_l_sensor.m_administration;
                                it_out.BiuseDate = s_l_sensor.m_biusedate;
                                it_out.CreatedBy = s_l_sensor.m_createdby;
                                it_out.CustDate1 = s_l_sensor.m_custdata1;
                                it_out.CustNbr1 = s_l_sensor.m_custnbr1;
                                it_out.CustTxt1 = s_l_sensor.m_custtxt1;
                                it_out.Created = s_l_sensor.m_datecreated;
                                it_out.EouseDate = s_l_sensor.m_eousedate;
                                it_out.Name = s_l_sensor.m_name;
                                it_out.NetworkId = s_l_sensor.m_networkid;
                                it_out.Remark = s_l_sensor.m_remark;
                                it_out.RxLoss = s_l_sensor.m_rxloss;
                                it_out.Status = s_l_sensor.m_status;
                                it_out.StepMeasTime = s_l_sensor.m_stepmeastime;
                                it_out.Type = s_l_sensor.m_typesensor;
                                it_out.Antenna = new SensorAntenna();

                                //Antenna
                                YXbsSensorantenna mpt = new YXbsSensorantenna();
                                mpt.Format("*");
                                mpt.Filter = string.Format("SENSORID={0}", s_l_sensor.m_id.Value);
                                for (mpt.OpenRs(); !mpt.IsEOF(); mpt.MoveNext())
                                {
                                    List<AntennaPattern> L_ant_patt = new List<AntennaPattern>();
                                    YXbsAntennapattern itr = new YXbsAntennapattern();
                                    itr.Format("*");
                                    itr.Filter = string.Format("SENSORANTENNA_ID={0}", mpt.m_id);
                                    for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                    {
                                        AntennaPattern ptu = new AntennaPattern();
                                        ptu.DiagA = itr.m_diaga;
                                        ptu.DiagH = itr.m_diagh;
                                        ptu.DiagV = itr.m_diagv;
                                        ptu.Freq_MHz = itr.m_freq.Value;
                                        ptu.Gain = itr.m_gain.Value;
                                        L_ant_patt.Add(ptu);
                                    }
                                    itr.Close();
                                    itr.Dispose();
                                    it_out.Antenna.Patterns = L_ant_patt.ToArray();
                                }
                                mpt.Close();
                                mpt.Dispose();

                                // Equipments
                                it_out.Equipment = new SensorEquipment();
                                {
                                    YXbsSensorequip mpt_ = new YXbsSensorequip();
                                    mpt_.Format("*");
                                    mpt_.Filter = string.Format("(SENSORID={0})", s_l_sensor.m_id.Value);
                                    for (mpt_.OpenRs(); !mpt_.IsEOF(); mpt_.MoveNext())
                                    {
                                        it_out.Equipment.Category = mpt_.m_category;
                                        it_out.Equipment.Code = mpt_.m_code;
                                        it_out.Equipment.CustDate1 = mpt_.m_custdata1;
                                        it_out.Equipment.CustNbr1 = mpt_.m_custnbr1;
                                        it_out.Equipment.CustTxt1 = mpt_.m_custtxt1;
                                        it_out.Equipment.Class = mpt_.m_equipclass;
                                        it_out.Equipment.Family = mpt_.m_family;
                                        it_out.Equipment.FFTPointMax = mpt_.m_fftpointmax;
                                        it_out.Equipment.LowerFreq_MHz = mpt_.m_lowerfreq;
                                        it_out.Equipment.Manufacturer = mpt_.m_manufacturer;
                                        it_out.Equipment.Mobility = mpt_.m_mobility == 1 ? true : false;
                                        it_out.Equipment.Name = mpt_.m_name;
                                        it_out.Equipment.OperationMode = mpt_.m_operationmode;
                                        it_out.Equipment.RBWMax_kHz = mpt_.m_rbwmax;
                                        it_out.Equipment.RBWMin_kHz = mpt_.m_rbwmin;
                                        it_out.Equipment.MaxRefLevel_dBm = mpt_.m_refleveldbm;
                                        it_out.Equipment.Remark = mpt_.m_remark;
                                        it_out.Equipment.TechId = mpt_.m_techid;
                                        it_out.Equipment.TuningStep_Hz = mpt_.m_tuningstep;
                                        it_out.Equipment.Type = mpt_.m_type;
                                        it_out.Equipment.UpperFreq_MHz = mpt_.m_upperfreq;
                                        it_out.Equipment.UseType = mpt_.m_usetype;
                                        it_out.Equipment.VBWMax_kHz = mpt_.m_vbwmax;
                                        it_out.Equipment.VBWMin_kHz = mpt_.m_vbwmin;
                                        it_out.Equipment.Version = mpt_.m_version;

                                        List<EquipmentSensitivity> L_sens = new List<EquipmentSensitivity>();
                                        YXbsSensorequipsens itr = new YXbsSensorequipsens();
                                        itr.Format("*");
                                        itr.Filter = string.Format("SENSOREQUIP_ID={0}", mpt_.m_id);
                                        for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                        {

                                            EquipmentSensitivity ptu = new EquipmentSensitivity();
                                            ptu.AddLoss = itr.m_addloss;
                                            ptu.Freq_MHz = itr.m_freq.Value;
                                            ptu.FreqStability = itr.m_freqstability;
                                            ptu.KTBF_dBm = itr.m_ktbf;
                                            ptu.NoiseF = itr.m_noisef;
                                            L_sens.Add(ptu);
                                        }
                                        itr.Close();
                                        itr.Dispose();
                                        it_out.Equipment.Sensitivities = L_sens.ToArray();
                                    }
                                    mpt_.Close();
                                    mpt_.Dispose();
                                }

                                List<SensorLocation> L_Sens_loc = new List<SensorLocation>();
                                YXbsSensorlocation mpt_loc = new YXbsSensorlocation();
                                mpt_loc.Format("*");
                                mpt_loc.Filter = string.Format("(SENSORID={0})", s_l_sensor.m_id.Value);
                                for (mpt_loc.OpenRs(); !mpt_loc.IsEOF(); mpt_loc.MoveNext())
                                {
                                    SensorLocation s_l = new SensorLocation();
                                    s_l.ASL = mpt_loc.m_asl;
                                    s_l.Created = mpt_loc.m_datacreated;
                                    s_l.From = mpt_loc.m_datafrom;
                                    s_l.To = mpt_loc.m_datato;
                                    s_l.Lat = mpt_loc.m_lat.Value;
                                    s_l.Lon = mpt_loc.m_lon.Value;
                                    s_l.Status = mpt_loc.m_status;
                                    L_Sens_loc.Add(s_l);
                                }
                                mpt_loc.Close();
                                mpt_loc.Dispose();
                                val = it_out;
                                break;
                            }
                            s_l_sensor.Close();
                            s_l_sensor.Dispose();
                        }
                        catch (Exception ex)
                        {
                            logger.Trace("Error in procedure LoadObjectAllSensor... " + ex.Message);
                        }
                    });
                    tsk.Start();
                    tsk.Join();
                    logger.Trace("End procedure LoadObjectAllSensor.");
                }
                catch (Exception ex)
                {
                    logger.Error("Error in procedure LoadObjectAllSensor: " + ex.Message);
                }

            }
            return val;
        }



        public bool IsFindSensorInDB(string Name, string TechId)
        {
            var Res = false;
            try
            {
                logger.Trace("Start procedure IsFindSensorInDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        YXbsSensor s_l_sensor = new YXbsSensor();
                        s_l_sensor.Format("*");
                        s_l_sensor.Filter = string.Format("(ID>0) AND (NAME='{0}')", Name);
                        s_l_sensor.Order = "[ID] DESC";
                        for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                        {
                            Sensor it_out = new Sensor();
                            YXbsSensorequip mpt_ = new YXbsSensorequip();
                            mpt_.Format("*");
                            mpt_.Filter = string.Format("(SENSORID={0}) AND (TECHID='{1}') ", s_l_sensor.m_id.Value, TechId);
                            for (mpt_.OpenRs(); !mpt_.IsEOF(); mpt_.MoveNext())
                            {
                                Res = true;
                                break;
                            }
                            mpt_.Close();
                            mpt_.Dispose();
                            if (Res) break;
                        }
                        s_l_sensor.Close();
                        s_l_sensor.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure IsFindSensorInDB... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure IsFindSensorInDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure IsFindSensorInDB: " + ex.Message);
            }
            return Res;
        }

        public int? GetIdSensor(string Name, string TechId)
        {
            int? res = null;
            try
            {
                logger.Trace("Start procedure IsFindSensorInDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        YXbsSensor s_l_sensor = new YXbsSensor();
                        s_l_sensor.Format("*");
                        s_l_sensor.Filter = string.Format("(ID>0) AND (NAME='{0}')", Name);
                        s_l_sensor.Order = "[ID] DESC";
                        for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                        {
                            Sensor it_out = new Sensor();
                            YXbsSensorequip mpt_ = new YXbsSensorequip();
                            mpt_.Format("*");
                            mpt_.Filter = string.Format("(SENSORID={0}) AND (TECHID='{1}') ", s_l_sensor.m_id.Value, TechId);
                            for (mpt_.OpenRs(); !mpt_.IsEOF(); mpt_.MoveNext())
                            {
                                res = mpt_.m_id;
                                break;
                            }
                            mpt_.Close();
                            mpt_.Dispose();
                        }
                        s_l_sensor.Close();
                        s_l_sensor.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure IsFindSensorInDB... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure IsFindSensorInDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure IsFindSensorInDB: " + ex.Message);
            }
            return res;
        }

        public List<Sensor> LoadObjectSensor(string Name, string TechId, string status)
        {
            var val = new List<Sensor>();
            {
                try
                {
                    logger.Trace("Start procedure LoadObjectSensor...");
                    System.Threading.Thread tsk = new System.Threading.Thread(() =>
                    {
                        try
                        {
                            YXbsSensor s_l_sensor = new YXbsSensor();
                            s_l_sensor.Format("*");
                            s_l_sensor.Filter = string.Format("(ID>0) AND (STATUS='{0}') AND (NAME='{1}')", status, Name);
                            s_l_sensor.Order = "[ID] DESC";
                            for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                            {
                                Sensor it_out = new Sensor();
                                it_out.Administration = s_l_sensor.m_administration;
                                it_out.BiuseDate = s_l_sensor.m_biusedate;
                                it_out.CreatedBy = s_l_sensor.m_createdby;
                                it_out.CustDate1 = s_l_sensor.m_custdata1;
                                it_out.CustNbr1 = s_l_sensor.m_custnbr1;
                                it_out.CustTxt1 = s_l_sensor.m_custtxt1;
                                it_out.Created = s_l_sensor.m_datecreated;
                                it_out.EouseDate = s_l_sensor.m_eousedate;
                                it_out.Name = s_l_sensor.m_name;
                                it_out.NetworkId = s_l_sensor.m_networkid;
                                it_out.Remark = s_l_sensor.m_remark;
                                it_out.RxLoss = s_l_sensor.m_rxloss;
                                it_out.Status = s_l_sensor.m_status;
                                it_out.StepMeasTime = s_l_sensor.m_stepmeastime;
                                it_out.Type = s_l_sensor.m_typesensor;
                                it_out.Antenna = new SensorAntenna();

                                //Antenna
                                YXbsSensorantenna mpt = new YXbsSensorantenna();
                                mpt.Format("*");
                                mpt.Filter = string.Format("SENSORID={0}", s_l_sensor.m_id.Value);
                                for (mpt.OpenRs(); !mpt.IsEOF(); mpt.MoveNext())
                                {
                                    List<AntennaPattern> L_ant_patt = new List<AntennaPattern>();
                                    YXbsAntennapattern itr = new YXbsAntennapattern();
                                    itr.Format("*");
                                    itr.Filter = string.Format("SENSORANTENNA_ID={0}", mpt.m_id);
                                    for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                    {
                                        AntennaPattern ptu = new AntennaPattern();
                                        ptu.DiagA = itr.m_diaga;
                                        ptu.DiagH = itr.m_diagh;
                                        ptu.DiagV = itr.m_diagv;
                                        ptu.Freq_MHz = itr.m_freq.Value;
                                        ptu.Gain = itr.m_gain.Value;
                                        L_ant_patt.Add(ptu);
                                    }
                                    itr.Close();
                                    itr.Dispose();
                                    it_out.Antenna.Patterns = L_ant_patt.ToArray();
                                }
                                mpt.Close();
                                mpt.Dispose();

                                // Equipments
                                it_out.Equipment = new SensorEquipment();
                                {
                                    YXbsSensorequip mpt_ = new YXbsSensorequip();
                                    mpt_.Format("*");
                                    mpt_.Filter = string.Format("(SENSORID={0}) AND (TECHID='{1}') ", s_l_sensor.m_id.Value, TechId);
                                    for (mpt_.OpenRs(); !mpt_.IsEOF(); mpt_.MoveNext())
                                    {
                                        it_out.Equipment.Category = mpt_.m_category;
                                        it_out.Equipment.Code = mpt_.m_code;
                                        it_out.Equipment.CustDate1 = mpt_.m_custdata1;
                                        it_out.Equipment.CustNbr1 = mpt_.m_custnbr1;
                                        it_out.Equipment.CustTxt1 = mpt_.m_custtxt1;
                                        it_out.Equipment.Class = mpt_.m_equipclass;
                                        it_out.Equipment.Family = mpt_.m_family;
                                        it_out.Equipment.FFTPointMax = mpt_.m_fftpointmax;
                                        it_out.Equipment.LowerFreq_MHz = mpt_.m_lowerfreq;
                                        it_out.Equipment.Manufacturer = mpt_.m_manufacturer;
                                        it_out.Equipment.Mobility = mpt_.m_mobility == 1 ? true : false;
                                        it_out.Equipment.Name = mpt_.m_name;
                                        it_out.Equipment.OperationMode = mpt_.m_operationmode;
                                        it_out.Equipment.RBWMax_kHz = mpt_.m_rbwmax;
                                        it_out.Equipment.RBWMin_kHz = mpt_.m_rbwmin;
                                        it_out.Equipment.MaxRefLevel_dBm = mpt_.m_refleveldbm;
                                        it_out.Equipment.Remark = mpt_.m_remark;
                                        it_out.Equipment.TechId = mpt_.m_techid;
                                        it_out.Equipment.TuningStep_Hz = mpt_.m_tuningstep;
                                        it_out.Equipment.Type = mpt_.m_type;
                                        it_out.Equipment.UpperFreq_MHz = mpt_.m_upperfreq;
                                        it_out.Equipment.UseType = mpt_.m_usetype;
                                        it_out.Equipment.VBWMax_kHz = mpt_.m_vbwmax;
                                        it_out.Equipment.VBWMin_kHz = mpt_.m_vbwmin;
                                        it_out.Equipment.Version = mpt_.m_version;

                                        List<EquipmentSensitivity> L_sens = new List<EquipmentSensitivity>();
                                        YXbsSensorequipsens itr = new YXbsSensorequipsens();
                                        itr.Format("*");
                                        itr.Filter = string.Format("SENSOREQUIP_ID={0}", mpt_.m_id);
                                        for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                        {
                                            EquipmentSensitivity ptu = new EquipmentSensitivity();
                                            ptu.AddLoss = itr.m_addloss;
                                            ptu.Freq_MHz = itr.m_freq.Value;
                                            ptu.FreqStability = itr.m_freqstability;
                                            ptu.KTBF_dBm = itr.m_ktbf;
                                            ptu.NoiseF = itr.m_noisef;
                                            L_sens.Add(ptu);
                                        }
                                        itr.Close();
                                        itr.Dispose();
                                        it_out.Equipment.Sensitivities = L_sens.ToArray();
                                    }
                                    mpt_.Close();
                                    mpt_.Dispose();
                                }

                                List<SensorLocation> L_Sens_loc = new List<SensorLocation>();
                                YXbsSensorlocation mpt_loc = new YXbsSensorlocation();
                                mpt_loc.Format("*");
                                mpt_loc.Filter = string.Format("(SENSORID={0}) AND (STATUS='{1}')", s_l_sensor.m_id.Value, status);
                                for (mpt_loc.OpenRs(); !mpt_loc.IsEOF(); mpt_loc.MoveNext())
                                {
                                    SensorLocation s_l = new SensorLocation();
                                    s_l.ASL = mpt_loc.m_asl;
                                    s_l.Created = mpt_loc.m_datacreated;
                                    s_l.From = mpt_loc.m_datafrom;
                                    s_l.To = mpt_loc.m_datato;
                                    s_l.Lat = mpt_loc.m_lat.Value;
                                    s_l.Lon = mpt_loc.m_lon.Value;
                                    s_l.Status = mpt_loc.m_status;
                                    L_Sens_loc.Add(s_l);
                                }
                                mpt_loc.Close();
                                mpt_loc.Dispose();
                                val.Add(it_out);
                            }
                            s_l_sensor.Close();
                            s_l_sensor.Dispose();
                        }
                        catch (Exception ex)
                        {
                            logger.Trace("Error in procedure LoadObjectSensor... " + ex.Message);
                        }
                    });
                    tsk.Start();
                    tsk.Join();

                    logger.Trace("End procedure LoadObjectSensor.");
                }
                catch (Exception ex)
                {
                    logger.Error("Error in procedure LoadObjectSensor: " + ex.Message);
                }

            }
            return val;
        }

        public List<Sensor> LoadObjectSensor(string Name, string TechId)
        {
            var val = new List<Sensor>();
            {
                try
                {
                    logger.Trace("Start procedure LoadObjectSensor...");
                    System.Threading.Thread tsk = new System.Threading.Thread(() =>
                    {
                        try
                        {
                            YXbsSensor s_l_sensor = new YXbsSensor();
                            s_l_sensor.Format("*");
                            s_l_sensor.Filter = string.Format("(ID>0) AND (NAME='{0}')", Name);
                            s_l_sensor.Order = "[ID] DESC";
                            for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                            {
                                Sensor it_out = new Sensor();
                                it_out.Administration = s_l_sensor.m_administration;
                                it_out.BiuseDate = s_l_sensor.m_biusedate;
                                it_out.CreatedBy = s_l_sensor.m_createdby;
                                it_out.CustDate1 = s_l_sensor.m_custdata1;
                                it_out.CustNbr1 = s_l_sensor.m_custnbr1;
                                it_out.CustTxt1 = s_l_sensor.m_custtxt1;
                                it_out.Created = s_l_sensor.m_datecreated;
                                it_out.EouseDate = s_l_sensor.m_eousedate;
                                it_out.Name = s_l_sensor.m_name;
                                it_out.NetworkId = s_l_sensor.m_networkid;
                                it_out.Remark = s_l_sensor.m_remark;
                                it_out.RxLoss = s_l_sensor.m_rxloss;
                                it_out.Status = s_l_sensor.m_status;
                                it_out.StepMeasTime = s_l_sensor.m_stepmeastime;
                                it_out.Type = s_l_sensor.m_typesensor;
                                it_out.Antenna = new SensorAntenna();

                                //Antenna
                                YXbsSensorantenna mpt = new YXbsSensorantenna();
                                mpt.Format("*");
                                mpt.Filter = string.Format("SENSORID={0}", s_l_sensor.m_id.Value);
                                for (mpt.OpenRs(); !mpt.IsEOF(); mpt.MoveNext())
                                {
                                    List<AntennaPattern> L_ant_patt = new List<AntennaPattern>();
                                    YXbsAntennapattern itr = new YXbsAntennapattern();
                                    itr.Format("*");
                                    itr.Filter = string.Format("SENSORANTENNA_ID={0}", mpt.m_id);
                                    for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                    {
                                        AntennaPattern ptu = new AntennaPattern();
                                        ptu.DiagA = itr.m_diaga;
                                        ptu.DiagH = itr.m_diagh;
                                        ptu.DiagV = itr.m_diagv;
                                        ptu.Freq_MHz = itr.m_freq.Value;
                                        ptu.Gain = itr.m_gain.Value;
                                        L_ant_patt.Add(ptu);
                                    }
                                    itr.Close();
                                    itr.Dispose();
                                    it_out.Antenna.Patterns = L_ant_patt.ToArray();
                                }
                                mpt.Close();
                                mpt.Dispose();

                                // Equipments
                                it_out.Equipment = new SensorEquipment();
                                {
                                    YXbsSensorequip mpt_ = new YXbsSensorequip();
                                    mpt_.Format("*");
                                    mpt_.Filter = string.Format("(SENSORID={0}) AND (TECHID='{1}') ", s_l_sensor.m_id.Value, TechId);
                                    for (mpt_.OpenRs(); !mpt_.IsEOF(); mpt_.MoveNext())
                                    {
                                        it_out.Equipment.Category = mpt_.m_category;
                                        it_out.Equipment.Code = mpt_.m_code;
                                        it_out.Equipment.CustDate1 = mpt_.m_custdata1;
                                        it_out.Equipment.CustNbr1 = mpt_.m_custnbr1;
                                        it_out.Equipment.CustTxt1 = mpt_.m_custtxt1;
                                        it_out.Equipment.Class = mpt_.m_equipclass;
                                        it_out.Equipment.Family = mpt_.m_family;
                                        it_out.Equipment.FFTPointMax = mpt_.m_fftpointmax;
                                        it_out.Equipment.LowerFreq_MHz = mpt_.m_lowerfreq;
                                        it_out.Equipment.Manufacturer = mpt_.m_manufacturer;
                                        it_out.Equipment.Mobility = mpt_.m_mobility == 1 ? true : false;
                                        it_out.Equipment.Name = mpt_.m_name;
                                        it_out.Equipment.OperationMode = mpt_.m_operationmode;
                                        it_out.Equipment.RBWMax_kHz = mpt_.m_rbwmax;
                                        it_out.Equipment.RBWMin_kHz = mpt_.m_rbwmin;
                                        it_out.Equipment.MaxRefLevel_dBm = mpt_.m_refleveldbm;
                                        it_out.Equipment.Remark = mpt_.m_remark;
                                        it_out.Equipment.TechId = mpt_.m_techid;
                                        it_out.Equipment.TuningStep_Hz = mpt_.m_tuningstep;
                                        it_out.Equipment.Type = mpt_.m_type;
                                        it_out.Equipment.UpperFreq_MHz = mpt_.m_upperfreq;
                                        it_out.Equipment.UseType = mpt_.m_usetype;
                                        it_out.Equipment.VBWMax_kHz = mpt_.m_vbwmax;
                                        it_out.Equipment.VBWMin_kHz = mpt_.m_vbwmin;
                                        it_out.Equipment.Version = mpt_.m_version;

                                        List<EquipmentSensitivity> L_sens = new List<EquipmentSensitivity>();
                                        YXbsSensorequipsens itr = new YXbsSensorequipsens();
                                        itr.Format("*");
                                        itr.Filter = string.Format("SENSOREQUIP_ID={0}", mpt_.m_id);
                                        for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                        {
                                            EquipmentSensitivity ptu = new EquipmentSensitivity();
                                            ptu.AddLoss = itr.m_addloss;
                                            ptu.Freq_MHz = itr.m_freq.Value;
                                            ptu.FreqStability = itr.m_freqstability;
                                            ptu.KTBF_dBm = itr.m_ktbf;
                                            ptu.NoiseF = itr.m_noisef;
                                            L_sens.Add(ptu);
                                        }
                                        itr.Close();
                                        itr.Dispose();
                                        it_out.Equipment.Sensitivities = L_sens.ToArray();
                                    }
                                    mpt_.Close();
                                    mpt_.Dispose();
                                }

                                List<SensorLocation> L_Sens_loc = new List<SensorLocation>();
                                YXbsSensorlocation mpt_loc = new YXbsSensorlocation();
                                mpt_loc.Format("*");
                                mpt_loc.Filter = string.Format("(SENSORID={0})", s_l_sensor.m_id.Value);
                                for (mpt_loc.OpenRs(); !mpt_loc.IsEOF(); mpt_loc.MoveNext())
                                {
                                    SensorLocation s_l = new SensorLocation();
                                    s_l.ASL = mpt_loc.m_asl;
                                    s_l.Created = mpt_loc.m_datacreated;
                                    s_l.From = mpt_loc.m_datafrom;
                                    s_l.To = mpt_loc.m_datato;
                                    s_l.Lat = mpt_loc.m_lat.Value;
                                    s_l.Lon = mpt_loc.m_lon.Value;
                                    s_l.Status = mpt_loc.m_status;
                                    L_Sens_loc.Add(s_l);
                                }
                                mpt_loc.Close();
                                mpt_loc.Dispose();
                                val.Add(it_out);
                            }
                            s_l_sensor.Close();
                            s_l_sensor.Dispose();
                        }
                        catch (Exception ex)
                        {
                            logger.Trace("Error in procedure LoadObjectSensor... " + ex.Message);
                        }
                    });
                    tsk.Start();
                    tsk.Join();

                    logger.Trace("End procedure LoadObjectSensor.");
                }
                catch (Exception ex)
                {
                    logger.Error("Error in procedure LoadObjectSensor: " + ex.Message);
                }
            }
            return val;
        }

        /// <summary>
        /// Load objects
        /// </summary>
        /// <returns></returns>
        public List<Sensor> LoadObjectSensor()
        {
            var val = new List<Sensor>();
            logger.Trace("Start procedure LoadObjectSensor...");
            System.Threading.Thread tsk = new System.Threading.Thread(() =>
            {
                try
                {
                    YXbsSensor s_l_sensor = new YXbsSensor();
                    s_l_sensor.Format("*");
                    s_l_sensor.Filter = "(ID>0) AND ((STATUS<>'Z') OR (STATUS IS NULL))";
                    for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                    {
                        Sensor it_out = new Sensor();
                        it_out.Administration = s_l_sensor.m_administration;
                        it_out.BiuseDate = s_l_sensor.m_biusedate;
                        it_out.CreatedBy = s_l_sensor.m_createdby;
                        it_out.CustDate1 = s_l_sensor.m_custdata1;
                        it_out.CustNbr1 = s_l_sensor.m_custnbr1;
                        it_out.CustTxt1 = s_l_sensor.m_custtxt1;
                        it_out.Created = s_l_sensor.m_datecreated;
                        it_out.EouseDate = s_l_sensor.m_eousedate;
                        it_out.Name = s_l_sensor.m_name;
                        it_out.NetworkId = s_l_sensor.m_networkid;
                        it_out.Remark = s_l_sensor.m_remark;
                        it_out.RxLoss = s_l_sensor.m_rxloss;
                        it_out.Status = s_l_sensor.m_status;
                        it_out.StepMeasTime = s_l_sensor.m_stepmeastime;
                        it_out.Type = s_l_sensor.m_typesensor;
                        it_out.Antenna = new SensorAntenna();

                        //Antenna
                        YXbsSensorantenna mpt = new YXbsSensorantenna();
                        mpt.Format("*");
                        mpt.Filter = string.Format("(SENSORID={0})", s_l_sensor.m_id.Value);
                        for (mpt.OpenRs(); !mpt.IsEOF(); mpt.MoveNext())
                        {
                            List<AntennaPattern> L_ant_patt = new List<AntennaPattern>();
                            YXbsAntennapattern itr = new YXbsAntennapattern();
                            itr.Format("*");
                            itr.Filter = string.Format("SENSORANTENNA_ID={0}", mpt.m_id);
                            for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                            {
                                AntennaPattern ptu = new AntennaPattern();
                                ptu.DiagA = itr.m_diaga;
                                ptu.DiagH = itr.m_diagh;
                                ptu.DiagV = itr.m_diagv;
                                ptu.Freq_MHz = itr.m_freq.Value;
                                ptu.Gain = itr.m_gain.Value;
                                L_ant_patt.Add(ptu);
                            }
                            itr.Close();
                            itr.Dispose();
                            it_out.Antenna.Patterns = L_ant_patt.ToArray();
                        }
                        mpt.Close();
                        mpt.Dispose();

                        // Equipments
                        it_out.Equipment = new SensorEquipment();
                        {
                            YXbsSensorequip mpt_ = new YXbsSensorequip();
                            mpt_.Format("*");
                            mpt_.Filter = string.Format("SENSORID={0}", s_l_sensor.m_id.Value);
                            for (mpt_.OpenRs(); !mpt_.IsEOF(); mpt_.MoveNext())
                            {
                                it_out.Equipment.Category = mpt_.m_category;
                                it_out.Equipment.Code = mpt_.m_code;
                                it_out.Equipment.CustDate1 = mpt_.m_custdata1;
                                it_out.Equipment.CustNbr1 = mpt_.m_custnbr1;
                                it_out.Equipment.CustTxt1 = mpt_.m_custtxt1;
                                it_out.Equipment.Class = mpt_.m_equipclass;
                                it_out.Equipment.Family = mpt_.m_family;
                                it_out.Equipment.FFTPointMax = mpt_.m_fftpointmax;
                                it_out.Equipment.LowerFreq_MHz = mpt_.m_lowerfreq;
                                it_out.Equipment.Manufacturer = mpt_.m_manufacturer;
                                it_out.Equipment.Mobility = mpt_.m_mobility == 1 ? true : false;
                                it_out.Equipment.Name = mpt_.m_name;
                                it_out.Equipment.OperationMode = mpt_.m_operationmode;
                                it_out.Equipment.RBWMax_kHz = mpt_.m_rbwmax;
                                it_out.Equipment.RBWMin_kHz = mpt_.m_rbwmin;
                                it_out.Equipment.MaxRefLevel_dBm = mpt_.m_refleveldbm;
                                it_out.Equipment.Remark = mpt_.m_remark;
                                it_out.Equipment.TechId = mpt_.m_techid;
                                it_out.Equipment.TuningStep_Hz = mpt_.m_tuningstep;
                                it_out.Equipment.Type = mpt_.m_type;
                                it_out.Equipment.UpperFreq_MHz = mpt_.m_upperfreq;
                                it_out.Equipment.UseType = mpt_.m_usetype;
                                it_out.Equipment.VBWMax_kHz = mpt_.m_vbwmax;
                                it_out.Equipment.VBWMin_kHz = mpt_.m_vbwmin;
                                it_out.Equipment.Version = mpt_.m_version;


                                List<EquipmentSensitivity> L_sens = new List<EquipmentSensitivity>();
                                YXbsSensorequipsens itr = new YXbsSensorequipsens();
                                itr.Format("*");
                                itr.Filter = string.Format("SENSOREQUIP_ID={0}", mpt_.m_id);
                                for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                {
                                    EquipmentSensitivity ptu = new EquipmentSensitivity();
                                    ptu.AddLoss = itr.m_addloss;
                                    ptu.Freq_MHz = itr.m_freq.Value;
                                    ptu.FreqStability = itr.m_freqstability;
                                    ptu.KTBF_dBm = itr.m_ktbf;
                                    ptu.NoiseF = itr.m_noisef;
                                    L_sens.Add(ptu);
                                }
                                itr.Close();
                                itr.Dispose();
                                it_out.Equipment.Sensitivities = L_sens.ToArray();
                            }
                            mpt_.Close();
                            mpt_.Dispose();
                        }

                        List<SensorLocation> L_Sens_loc = new List<SensorLocation>();
                        YXbsSensorlocation mpt_loc = new YXbsSensorlocation();
                        mpt_loc.Format("*");
                        mpt_loc.Filter = string.Format("(SENSORID={0}) AND (STATUS<>'{1}')", s_l_sensor.m_id.Value, "Z");
                        for (mpt_loc.OpenRs(); !mpt_loc.IsEOF(); mpt_loc.MoveNext())
                        {
                            SensorLocation s_lw = new SensorLocation();
                            s_lw.ASL = mpt_loc.m_asl;
                            s_lw.Created = mpt_loc.m_datacreated;
                            s_lw.From = mpt_loc.m_datafrom;
                            s_lw.To = mpt_loc.m_datato;
                            s_lw.Lat = mpt_loc.m_lat.Value;
                            s_lw.Lon = mpt_loc.m_lon.Value;
                            s_lw.Status = mpt_loc.m_status;
                            L_Sens_loc.Add(s_lw);
                        }
                        mpt_loc.Close();
                        mpt_loc.Dispose();

                        SensorPolygon s_l = new SensorPolygon();
                        YXbsSensorpolig mpt_loc_pt = new YXbsSensorpolig();
                        List<GeoPoint> geoPoint = new List<GeoPoint>();
                        mpt_loc_pt.Format("*");
                        mpt_loc_pt.Filter = string.Format("(SENSORID={0})", s_l_sensor.m_id.Value);
                        for (mpt_loc_pt.OpenRs(); !mpt_loc_pt.IsEOF(); mpt_loc_pt.MoveNext())
                        {
                            GeoPoint g = new GeoPoint();
                            g.Lat = mpt_loc.m_lat;
                            g.Lon = mpt_loc.m_lon;
                            geoPoint.Add(g);
                        }
                        s_l.Points = geoPoint.ToArray();
                        mpt_loc_pt.Close();
                        mpt_loc_pt.Dispose();
                        it_out.Polygon = s_l;
                        val.Add(it_out);
                    }
                    s_l_sensor.Close();
                    s_l_sensor.Dispose();
                }
                catch (Exception ex)
                {
                    logger.Error("Error in procedure LoadObjectSensor." + ex.Message);
                }
            });
            tsk.Start();
            tsk.Join();

            logger.Trace("End procedure LoadObjectSensor.");
            return val;
        }



        /// <summary>
        /// Create new object
        /// </summary>
        /// <param name="sens"></param>
        /// <returns></returns>
        public bool CreateNewObjectSensor(DM.Sensor sens)
        {
            System.Threading.Thread tsk = new System.Threading.Thread(() =>
            {
                logger.Trace("Start procedure CreateNewObjectSensor...");
                Yyy yyy = new Yyy();
                DbConnection dbConnect = null;
                try
                {
                    dbConnect = yyy.NewConnection(yyy.GetConnectionString());
                    if (dbConnect.State == System.Data.ConnectionState.Open)
                    {
                        DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                        try
                        {
                            if (sens != null)
                            {
                                double? val = null;
                                int? m_ID_Sensor = -1;
                                List<Sensor> R_s_find = LoadObjectSensor();
                                bool isNew = false;
                                Sensor Fnd = R_s_find.Find(t => t.Name == sens.Name && t.Equipment.TechId == sens.Equipment.TechId);
                                YXbsSensor se = new YXbsSensor();
                                se.Format("*");
                                if (!se.Fetch(string.Format("NAME='{0}' AND TECHID='{1}'", sens.Name, sens.Equipment.TechId)))
                                {
                                    se.Filter = "(ID=-1)";
                                    se.New();
                                    isNew = true;
                                }
                                else { m_ID_Sensor = se.m_id; }

                                se.m_administration = sens.Administration;
                                if (sens.AGL != null) se.m_agl = sens.AGL.GetValueOrDefault();
                                if (sens.Azimuth != null) se.m_azimuth = sens.Azimuth.GetValueOrDefault();
                                if (sens.BiuseDate != null) se.m_biusedate = sens.BiuseDate.GetValueOrDefault();
                                se.m_createdby = sens.CreatedBy;
                                if (sens.CustData1 != null) se.m_custdata1 = sens.CustData1.GetValueOrDefault();
                                if (sens.CustNbr1 != null) se.m_custnbr1 = sens.CustNbr1.GetValueOrDefault();
                                se.m_custtxt1 = sens.CustTxt1;
                                if (sens.DateCreated != null) se.m_datecreated = sens.DateCreated.GetValueOrDefault();
                                if (sens.Elevation != null) se.m_elevation = sens.Elevation.GetValueOrDefault();
                                if (sens.EouseDate != null) se.m_eousedate = sens.EouseDate.GetValueOrDefault();
                                se.m_idsysargus = sens.IdSysARGUS;
                                se.m_name = sens.Name;
                                se.m_networkid = sens.NetworkId;
                                se.m_opdays = sens.OpDays;
                                if (sens.OpHHFr != null) se.m_ophhfr = sens.OpHHFr.GetValueOrDefault();
                                if (sens.OpHHTo != null) se.m_ophhto = sens.OpHHTo.GetValueOrDefault();
                                se.m_remark = sens.Remark;
                                if (sens.RxLoss != null) se.m_rxloss = sens.RxLoss.GetValueOrDefault();
                                se.m_status = sens.Status;
                                if (sens.StepMeasTime != null) se.m_stepmeastime = sens.StepMeasTime.GetValueOrDefault();
                                se.m_typesensor = sens.TypeSensor;
                                se.m_techid = sens.Equipment.TechId;
                                se.m_apiversion = "v2.0";
                                if (isNew)
                                {
                                    m_ID_Sensor = se.Save(dbConnect, transaction);
                                    se.m_sensoridentifier_id = m_ID_Sensor;
                                    se.SaveUpdate(dbConnect, transaction);
                                }
                                else
                                {
                                    se.SaveUpdate(dbConnect, transaction);
                                }

                                se.Close();
                                se.Dispose();

                                if (sens.Antenna != null)
                                {
                                    int? ID_sensor_ant = -1;
                                    YXbsSensorantenna NH_sensor_ant = new YXbsSensorantenna();
                                    NH_sensor_ant.Format("*");
                                    if (!NH_sensor_ant.Fetch(string.Format("(SENSORID={0})", m_ID_Sensor)))
                                    {
                                        NH_sensor_ant.New();
                                    }
                                    else
                                    {
                                        if (NH_sensor_ant.m_id != null) ID_sensor_ant = (int)NH_sensor_ant.m_id;
                                    }
                                    NH_sensor_ant.m_addloss = sens.Antenna.AddLoss;
                                    NH_sensor_ant.m_antclass = sens.Antenna.AntClass;
                                    NH_sensor_ant.m_antdir = sens.Antenna.AntDir.ToString();
                                    NH_sensor_ant.m_category = sens.Antenna.Category;
                                    NH_sensor_ant.m_code = sens.Antenna.Code;
                                    NH_sensor_ant.m_custdata1 = sens.Antenna.CustData1;
                                    NH_sensor_ant.m_custnbr1 = sens.Antenna.CustNbr1;
                                    NH_sensor_ant.m_custtxt1 = sens.Antenna.CustTxt1;
                                    NH_sensor_ant.m_gainmax = sens.Antenna.GainMax;
                                    NH_sensor_ant.m_gaintype = sens.Antenna.GainType;
                                    if (sens.Antenna.HBeamwidth != null) NH_sensor_ant.m_hbeamwidth = sens.Antenna.HBeamwidth.GetValueOrDefault();
                                    if (sens.Antenna.LowerFreq != null) NH_sensor_ant.m_lowerfreq = sens.Antenna.LowerFreq.GetValueOrDefault();
                                    NH_sensor_ant.m_manufacturer = sens.Antenna.Manufacturer;
                                    NH_sensor_ant.m_name = sens.Antenna.Name;
                                    NH_sensor_ant.m_polarization = sens.Antenna.Polarization.ToString();
                                    NH_sensor_ant.m_remark = sens.Antenna.Remark;
                                    if (sens.Antenna.SlewAng != null) NH_sensor_ant.m_slewang = sens.Antenna.SlewAng.GetValueOrDefault();
                                    NH_sensor_ant.m_techid = sens.Antenna.TechId;
                                    if (sens.Antenna.UpperFreq != null) NH_sensor_ant.m_upperfreq = sens.Antenna.UpperFreq.GetValueOrDefault();
                                    NH_sensor_ant.m_usetype = sens.Antenna.UseType;
                                    if (sens.Antenna.VBeamwidth != null) NH_sensor_ant.m_vbeamwidth = sens.Antenna.VBeamwidth.GetValueOrDefault();
                                    if (sens.Antenna.XPD != null) NH_sensor_ant.m_xpd = sens.Antenna.XPD.GetValueOrDefault();
                                    NH_sensor_ant.m_sensorid = m_ID_Sensor;
                                    ID_sensor_ant = NH_sensor_ant.Save(dbConnect, transaction);
                                    NH_sensor_ant.Close();
                                    NH_sensor_ant.Dispose();

                                    if (sens.Antenna.AntennaPatterns != null)
                                    {
                                        foreach (DM.AntennaPattern patt_a in sens.Antenna.AntennaPatterns.ToArray())
                                        {
                                            YXbsAntennapattern NH_ant_patt = new YXbsAntennapattern();
                                            NH_ant_patt.Format("*");
                                            if (!NH_ant_patt.Fetch(string.Format("(SENSORANTENNA_ID={0}) and (FREQ{1}) and (GAIN{2}) ", ID_sensor_ant, patt_a.Freq != Constants.NullD ? "=" + patt_a.Freq.ToString().Replace(",", ".") : " IS NULL", patt_a.Gain != Constants.NullI ? "=" + patt_a.Gain.ToString().Replace(",", ".") : " IS NULL")))
                                            {
                                                NH_ant_patt.New();
                                            }
                                            NH_ant_patt.m_diaga = patt_a.DiagA;
                                            NH_ant_patt.m_diagh = patt_a.DiagH;
                                            NH_ant_patt.m_diagv = patt_a.DiagV;
                                            NH_ant_patt.m_freq = patt_a.Freq;
                                            NH_ant_patt.m_gain = patt_a.Gain;
                                            NH_ant_patt.m_sensorantenna_id = ID_sensor_ant;
                                            NH_ant_patt.Save(dbConnect, transaction);
                                            NH_ant_patt.Close();
                                            NH_ant_patt.Dispose();
                                        }
                                    }
                                }
                                if (sens.Equipment != null)
                                {
                                    int? ID_NH_sens_eqp = -1;
                                    YXbsSensorequip NH_sens_eqp = new YXbsSensorequip();
                                    NH_sens_eqp.Format("*");
                                    if (!NH_sens_eqp.Fetch(string.Format("(SENSORID={0})", m_ID_Sensor)))
                                    {
                                        NH_sens_eqp.New();
                                    }
                                    else
                                    {
                                        ID_NH_sens_eqp = NH_sens_eqp.m_id;
                                    }
                                    NH_sens_eqp.m_category = sens.Equipment.Category;
                                    NH_sens_eqp.m_code = sens.Equipment.Code;
                                    if (sens.Equipment.CustData1 != null) NH_sens_eqp.m_custdata1 = sens.Equipment.CustData1.GetValueOrDefault();
                                    if (sens.Equipment.CustNbr1 != null) NH_sens_eqp.m_custnbr1 = sens.Equipment.CustNbr1.GetValueOrDefault();
                                    NH_sens_eqp.m_custtxt1 = sens.Equipment.CustTxt1;
                                    NH_sens_eqp.m_equipclass = sens.Equipment.EquipClass;
                                    NH_sens_eqp.m_family = sens.Equipment.Family;
                                    if (sens.Equipment.FFTPointMax != null) NH_sens_eqp.m_fftpointmax = sens.Equipment.FFTPointMax.GetValueOrDefault();
                                    if (sens.Equipment.LowerFreq != null) NH_sens_eqp.m_lowerfreq = sens.Equipment.LowerFreq.GetValueOrDefault();
                                    NH_sens_eqp.m_manufacturer = sens.Equipment.Manufacturer;
                                    NH_sens_eqp.m_mobility = sens.Equipment.Mobility == true ? 1 : 0;
                                    NH_sens_eqp.m_name = sens.Equipment.Name;
                                    NH_sens_eqp.m_operationmode = sens.Equipment.OperationMode;
                                    if (sens.Equipment.RBWMax != null) NH_sens_eqp.m_rbwmax = sens.Equipment.RBWMax.GetValueOrDefault();
                                    if (sens.Equipment.RBWMin != null) NH_sens_eqp.m_rbwmin = sens.Equipment.RBWMin.GetValueOrDefault();
                                    if (sens.Equipment.RefLeveldBm != null) NH_sens_eqp.m_refleveldbm = sens.Equipment.RefLeveldBm.GetValueOrDefault();
                                    NH_sens_eqp.m_remark = sens.Equipment.Remark;
                                    NH_sens_eqp.m_techid = sens.Equipment.TechId;
                                    if (sens.Equipment.TuningStep != null) NH_sens_eqp.m_tuningstep = sens.Equipment.TuningStep.GetValueOrDefault();
                                    NH_sens_eqp.m_type = sens.Equipment.Type;
                                    if (sens.Equipment.UpperFreq != null) NH_sens_eqp.m_upperfreq = sens.Equipment.UpperFreq.GetValueOrDefault();
                                    NH_sens_eqp.m_usetype = sens.Equipment.UseType;
                                    if (sens.Equipment.VBWMax != null) NH_sens_eqp.m_vbwmax = sens.Equipment.VBWMax.GetValueOrDefault();
                                    if (sens.Equipment.VBWMin != null) NH_sens_eqp.m_vbwmin = sens.Equipment.VBWMin.GetValueOrDefault();
                                    NH_sens_eqp.m_version = sens.Equipment.Version;
                                    NH_sens_eqp.m_sensorid = m_ID_Sensor;
                                    ID_NH_sens_eqp = NH_sens_eqp.Save(dbConnect, transaction);
                                    NH_sens_eqp.Close();
                                    NH_sens_eqp.Dispose();
                                    if (sens.Equipment.SensorEquipSensitivities != null)
                                    {
                                        try
                                        {
                                            foreach (DM.SensorEquipSensitivity sens_eqp_s in sens.Equipment.SensorEquipSensitivities.ToArray())
                                            {
                                                YXbsSensorequipsens NH_SensorEquipSensitivity_ = new YXbsSensorequipsens();
                                                NH_SensorEquipSensitivity_.Format("*");
                                                if (!NH_SensorEquipSensitivity_.Fetch(string.Format("(SENSOREQUIP_ID={0}) and (ADDLOSS{1}) and (FREQ{2}) and (FREQSTABILITY{3}) and (KTBF{4}) and (NOISEF{5})", ID_NH_sens_eqp, sens_eqp_s.AddLoss.HasValue ? "=" + sens_eqp_s.AddLoss.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.Freq != Constants.NullD ? "=" + sens_eqp_s.Freq.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.FreqStability.HasValue ? "=" + sens_eqp_s.FreqStability.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.KTBF.HasValue ? "=" + sens_eqp_s.KTBF.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.NoiseF.HasValue ? "=" + sens_eqp_s.NoiseF.ToString().Replace(",", ".") : " IS NULL")))
                                                {
                                                    NH_SensorEquipSensitivity_.New();
                                                }
                                                NH_SensorEquipSensitivity_.m_addloss = sens_eqp_s.AddLoss.GetValueOrDefault();
                                                NH_SensorEquipSensitivity_.m_freq = sens_eqp_s.Freq;
                                                if (sens_eqp_s.FreqStability != null) NH_SensorEquipSensitivity_.m_freqstability = sens_eqp_s.FreqStability.GetValueOrDefault();
                                                if (sens_eqp_s.KTBF != null) NH_SensorEquipSensitivity_.m_ktbf = sens_eqp_s.KTBF.GetValueOrDefault();
                                                if (sens_eqp_s.NoiseF != null) NH_SensorEquipSensitivity_.m_noisef = sens_eqp_s.NoiseF.GetValueOrDefault();
                                                NH_SensorEquipSensitivity_.m_sensorequip_id = ID_NH_sens_eqp;
                                                NH_SensorEquipSensitivity_.Save(dbConnect, transaction);
                                                NH_SensorEquipSensitivity_.Close();
                                                NH_SensorEquipSensitivity_.Dispose();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error("Error in SensorEquipSensitivity: " + ex.Message);
                                        }
                                    }
                                    if (sens.Locations != null)
                                    {
                                        try
                                        {
                                            foreach (DM.SensorLocation sens_Locations in sens.Locations.ToArray())
                                            {
                                                YXbsSensorlocation NH_sens_location_old = new YXbsSensorlocation();
                                                NH_sens_location_old.Format("*");
                                                NH_sens_location_old.Filter = string.Format("(SENSORID={0}) and (STATUS='A')", m_ID_Sensor);
                                                for (NH_sens_location_old.OpenRs(); !NH_sens_location_old.IsEOF(); NH_sens_location_old.MoveNext())
                                                {
                                                    NH_sens_location_old.m_status = "Z";
                                                    NH_sens_location_old.Save(dbConnect, transaction);
                                                }
                                                NH_sens_location_old.Close();
                                                NH_sens_location_old.Dispose();

                                                YXbsSensorlocation NH_sens_location = new YXbsSensorlocation();
                                                NH_sens_location.Format("*");
                                                if (!NH_sens_location.Fetch(string.Format("(SENSORID={0}) and (ASL{1}) and (LAT{2}) and (LON{3})", m_ID_Sensor, sens_Locations.ASL.HasValue ? "=" + sens_Locations.ASL.ToString().Replace(",", ".") : " IS NULL", sens_Locations.Lat.HasValue ? "=" + sens_Locations.Lat.ToString().Replace(",", ".") : " IS NULL", sens_Locations.Lon.HasValue ? "=" + sens_Locations.Lon.ToString().Replace(",", ".") : " IS NULL")))
                                                {
                                                    NH_sens_location.New();
                                                }
                                                if (sens_Locations.ASL != null) NH_sens_location.m_asl = sens_Locations.ASL.GetValueOrDefault();
                                                if (sens_Locations.DataCreated != null) NH_sens_location.m_datacreated = sens_Locations.DataCreated.GetValueOrDefault();
                                                if (sens_Locations.DataFrom != null) NH_sens_location.m_datafrom = sens_Locations.DataFrom.GetValueOrDefault();
                                                if (sens_Locations.DataTo != null) NH_sens_location.m_datato = sens_Locations.DataTo.GetValueOrDefault();
                                                if (sens_Locations.Lat != null) NH_sens_location.m_lat = sens_Locations.Lat.GetValueOrDefault();
                                                if (sens_Locations.Lon != null) NH_sens_location.m_lon = sens_Locations.Lon.GetValueOrDefault();
                                                NH_sens_location.m_status = sens_Locations.Status;
                                                NH_sens_location.m_sensorid = m_ID_Sensor;
                                                NH_sens_location.Save(dbConnect, transaction);
                                                NH_sens_location.Close();
                                                NH_sens_location.Dispose();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error("Error in SensorLocation: " + ex.Message);
                                        }
                                    }
                                    if (sens.Poligon != null)
                                    {
                                        try
                                        {
                                            foreach (DM.SensorPoligonPoint sens_Locations in sens.Poligon.ToArray())
                                            {
                                                YXbsSensorpolig NH_sens_location = new YXbsSensorpolig();
                                                NH_sens_location.Format("*");
                                                if (!NH_sens_location.Fetch(string.Format("(SENSORID={0}) and (LAT{1}) and (LON{2})", m_ID_Sensor, sens_Locations.Lat.HasValue ? "=" + sens_Locations.Lat.ToString().Replace(",", ".") : " IS NULL", sens_Locations.Lon.HasValue ? "=" + sens_Locations.Lon.ToString().Replace(",", ".") : " IS NULL")))
                                                {
                                                    NH_sens_location.New();
                                                }
                                                if (sens_Locations.Lat != null) NH_sens_location.m_lat = sens_Locations.Lat.GetValueOrDefault();
                                                if (sens_Locations.Lon != null) NH_sens_location.m_lon = sens_Locations.Lon.GetValueOrDefault();
                                                NH_sens_location.m_sensorid = m_ID_Sensor;
                                                NH_sens_location.Save(dbConnect, transaction);
                                                NH_sens_location.Close();
                                                NH_sens_location.Dispose();
                                                logger.Trace("Success created record for table: YXbsSensorpolig");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error("Error in SensorPoligonPoint: " + ex.Message);
                                        }
                                    }
                                }

                            }
                            transaction.Commit();
                            LoadObjectSensor();
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e) { transaction.Dispose(); logger.Error(e.Message); }
                            logger.Error("Error in  procedure CreateNewObjectSensor: " + ex.Message);
                        }
                        finally
                        {
                            transaction.Dispose();
                        }
                    }
                    else
                    {
                        logger.Error("[CreateNewObjectSensor] Error connection  to Database");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                finally
                {
                    dbConnect.Close();
                    dbConnect.Dispose();
                }
            });
            tsk.Start();
            tsk.Join();
            logger.Trace("End procedure CreateNewObjectSensor.");
            return true;
        }

        /// <summary>
        /// Create new object
        /// </summary>
        /// <param name="sens"></param>
        /// <returns></returns>
        public bool CreateNewObjectSensor(Sensor sens, string apiVersion)
        {
            System.Threading.Thread tsk = new System.Threading.Thread(() =>
            {
                logger.Trace("Start procedure CreateNewObjectSensor...");
                Yyy yyy = new Yyy();
                DbConnection dbConnect = null;
                try
                {
                    dbConnect = yyy.NewConnection(yyy.GetConnectionString());
                    if (dbConnect.State == System.Data.ConnectionState.Open)
                    {
                        DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                        try
                        {
                            if (sens != null)
                            {
                                int? m_ID_Sensor = null;
                                double? val = null;
                                List<Sensor> R_s_find = LoadObjectSensor();
                                Sensor Fnd = R_s_find.Find(t => t.Name == sens.Name && t.Equipment.TechId == sens.Equipment.TechId);
                                YXbsSensor se = new YXbsSensor();
                                se.Format("*");
                                if (Fnd == null)
                                {
                                    se.Filter = "(ID=-1)";
                                    se.New();
                                    se.m_administration = sens.Administration;
                                    if (sens.BiuseDate != null) se.m_biusedate = sens.BiuseDate.GetValueOrDefault();
                                    se.m_createdby = sens.CreatedBy;
                                    if (sens.CustDate1 != null) se.m_custdata1 = sens.CustDate1.GetValueOrDefault();
                                    if (sens.CustNbr1 != null) se.m_custnbr1 = sens.CustNbr1.GetValueOrDefault();
                                    se.m_custtxt1 = sens.CustTxt1;
                                    if (sens.Created != null) se.m_datecreated = sens.Created.GetValueOrDefault();
                                    if (sens.EouseDate != null) se.m_eousedate = sens.EouseDate.GetValueOrDefault();
                                    se.m_name = sens.Name;
                                    se.m_networkid = sens.NetworkId;
                                    se.m_remark = sens.Remark;
                                    if (sens.RxLoss != null) se.m_rxloss = sens.RxLoss.GetValueOrDefault();
                                    se.m_status = sens.Status;
                                    if (sens.StepMeasTime != null) se.m_stepmeastime = sens.StepMeasTime.GetValueOrDefault();
                                    se.m_typesensor = sens.Type;
                                    se.m_apiversion = apiVersion;
                                    se.m_techid = sens.Equipment.TechId;
                                    m_ID_Sensor = se.Save(dbConnect, transaction);

                                    if (se.Fetch(string.Format("ID={0}", m_ID_Sensor)))
                                    {
                                        se.m_sensoridentifier_id = m_ID_Sensor;
                                        se.SaveUpdate(dbConnect, transaction);
                                    }

                                    se.Close();
                                    se.Dispose();

                                    if (sens.Antenna != null)
                                    {
                                        int? ID_sensor_ant = -1;
                                        YXbsSensorantenna NH_sensor_ant = new YXbsSensorantenna();
                                        NH_sensor_ant.Format("*");
                                        if (!NH_sensor_ant.Fetch(string.Format("(SENSORID={0})", m_ID_Sensor)))
                                        {
                                            NH_sensor_ant.New();
                                        }
                                        else ID_sensor_ant = NH_sensor_ant.m_id;
                                        NH_sensor_ant.m_addloss = sens.Antenna.AddLoss;
                                        NH_sensor_ant.m_antclass = sens.Antenna.Class;
                                        NH_sensor_ant.m_antdir = sens.Antenna.Direction.ToString();
                                        NH_sensor_ant.m_category = sens.Antenna.Category;
                                        NH_sensor_ant.m_code = sens.Antenna.Code;
                                        if (sens.Antenna.CustDate1 != null) NH_sensor_ant.m_custdata1 = sens.Antenna.CustDate1.ToString();
                                        NH_sensor_ant.m_custnbr1 = sens.Antenna.CustNbr1;
                                        NH_sensor_ant.m_custtxt1 = sens.Antenna.CustTxt1;
                                        NH_sensor_ant.m_gainmax = sens.Antenna.GainMax;
                                        NH_sensor_ant.m_gaintype = sens.Antenna.GainType;
                                        if (sens.Antenna.HBeamwidth != null) NH_sensor_ant.m_hbeamwidth = sens.Antenna.HBeamwidth.GetValueOrDefault();
                                        if (sens.Antenna.LowerFreq_MHz != null) NH_sensor_ant.m_lowerfreq = sens.Antenna.LowerFreq_MHz.GetValueOrDefault();
                                        NH_sensor_ant.m_manufacturer = sens.Antenna.Manufacturer;
                                        NH_sensor_ant.m_name = sens.Antenna.Name;
                                        NH_sensor_ant.m_polarization = sens.Antenna.Polarization.ToString();
                                        NH_sensor_ant.m_remark = sens.Antenna.Remark;
                                        if (sens.Antenna.SlewAng != null) NH_sensor_ant.m_slewang = sens.Antenna.SlewAng.GetValueOrDefault();
                                        NH_sensor_ant.m_techid = sens.Antenna.TechId;
                                        if (sens.Antenna.UpperFreq_MHz != null) NH_sensor_ant.m_upperfreq = sens.Antenna.UpperFreq_MHz.GetValueOrDefault();
                                        NH_sensor_ant.m_usetype = sens.Antenna.UseType;
                                        if (sens.Antenna.VBeamwidth != null) NH_sensor_ant.m_vbeamwidth = sens.Antenna.VBeamwidth.GetValueOrDefault();
                                        if (sens.Antenna.XPD != null) NH_sensor_ant.m_xpd = sens.Antenna.XPD.GetValueOrDefault();
                                        NH_sensor_ant.m_sensorid = m_ID_Sensor;
                                        ID_sensor_ant = NH_sensor_ant.Save(dbConnect, transaction);
                                        NH_sensor_ant.Close();
                                        NH_sensor_ant.Dispose();

                                        if (sens.Antenna.Patterns != null)
                                        {
                                            foreach (AntennaPattern patt_a in sens.Antenna.Patterns.ToArray())
                                            {
                                                YXbsAntennapattern NH_ant_patt = new YXbsAntennapattern();
                                                NH_ant_patt.Format("*");
                                                if (!NH_ant_patt.Fetch(string.Format("(SENSORANTENNA_ID={0}) and (FREQ{1}) and (GAIN{2}) ", ID_sensor_ant, patt_a.Freq_MHz != Constants.NullD ? "=" + patt_a.Freq_MHz.ToString().Replace(",", ".") : " IS NULL", patt_a.Gain != Constants.NullI ? "=" + patt_a.Gain.ToString().Replace(",", ".") : " IS NULL")))
                                                {
                                                    NH_ant_patt.New();
                                                }
                                                NH_ant_patt.m_diaga = patt_a.DiagA;
                                                NH_ant_patt.m_diagh = patt_a.DiagH;
                                                NH_ant_patt.m_diagv = patt_a.DiagV;
                                                NH_ant_patt.m_freq = patt_a.Freq_MHz;
                                                NH_ant_patt.m_gain = patt_a.Gain;
                                                NH_ant_patt.m_sensorantenna_id = ID_sensor_ant;
                                                NH_ant_patt.Save(dbConnect, transaction);
                                                NH_ant_patt.Close();
                                                NH_ant_patt.Dispose();
                                            }
                                        }
                                    }
                                    if (sens.Equipment != null)
                                    {
                                        int? ID_NH_sens_eqp = -1;
                                        YXbsSensorequip NH_sens_eqp = new YXbsSensorequip();
                                        NH_sens_eqp.Format("*");
                                        if (!NH_sens_eqp.Fetch(string.Format("(SENSORID={0})", m_ID_Sensor)))
                                        {
                                            NH_sens_eqp.New();
                                        }
                                        else ID_NH_sens_eqp = NH_sens_eqp.m_id;
                                        NH_sens_eqp.m_category = sens.Equipment.Category;
                                        NH_sens_eqp.m_code = sens.Equipment.Code;
                                        if (sens.Equipment.CustDate1 != null) NH_sens_eqp.m_custdata1 = sens.Equipment.CustDate1.GetValueOrDefault();
                                        if (sens.Equipment.CustNbr1 != null) NH_sens_eqp.m_custnbr1 = sens.Equipment.CustNbr1.GetValueOrDefault();
                                        NH_sens_eqp.m_custtxt1 = sens.Equipment.CustTxt1;
                                        NH_sens_eqp.m_equipclass = sens.Equipment.Class;
                                        NH_sens_eqp.m_family = sens.Equipment.Family;
                                        if (sens.Equipment.FFTPointMax != null) NH_sens_eqp.m_fftpointmax = sens.Equipment.FFTPointMax.GetValueOrDefault();
                                        if (sens.Equipment.LowerFreq_MHz != null) NH_sens_eqp.m_lowerfreq = sens.Equipment.LowerFreq_MHz.GetValueOrDefault();
                                        NH_sens_eqp.m_manufacturer = sens.Equipment.Manufacturer;
                                        NH_sens_eqp.m_mobility = sens.Equipment.Mobility == true ? 1 : 0;
                                        NH_sens_eqp.m_name = sens.Equipment.Name;
                                        NH_sens_eqp.m_operationmode = sens.Equipment.OperationMode;
                                        if (sens.Equipment.RBWMax_kHz != null) NH_sens_eqp.m_rbwmax = sens.Equipment.RBWMax_kHz.GetValueOrDefault();
                                        if (sens.Equipment.RBWMin_kHz != null) NH_sens_eqp.m_rbwmin = sens.Equipment.RBWMin_kHz.GetValueOrDefault();
                                        if (sens.Equipment.MaxRefLevel_dBm != null) NH_sens_eqp.m_refleveldbm = sens.Equipment.MaxRefLevel_dBm.GetValueOrDefault();
                                        NH_sens_eqp.m_remark = sens.Equipment.Remark;
                                        NH_sens_eqp.m_techid = sens.Equipment.TechId;
                                        if (sens.Equipment.TuningStep_Hz != null) NH_sens_eqp.m_tuningstep = sens.Equipment.TuningStep_Hz.GetValueOrDefault();
                                        NH_sens_eqp.m_type = sens.Equipment.Type;
                                        if (sens.Equipment.UpperFreq_MHz != null) NH_sens_eqp.m_upperfreq = sens.Equipment.UpperFreq_MHz.GetValueOrDefault();
                                        NH_sens_eqp.m_usetype = sens.Equipment.UseType;
                                        if (sens.Equipment.VBWMax_kHz != null) NH_sens_eqp.m_vbwmax = sens.Equipment.VBWMax_kHz.GetValueOrDefault();
                                        if (sens.Equipment.VBWMin_kHz != null) NH_sens_eqp.m_vbwmin = sens.Equipment.VBWMin_kHz.GetValueOrDefault();
                                        NH_sens_eqp.m_version = sens.Equipment.Version;
                                        NH_sens_eqp.m_sensorid = m_ID_Sensor;
                                        ID_NH_sens_eqp = NH_sens_eqp.Save(dbConnect, transaction);
                                        NH_sens_eqp.Close();
                                        NH_sens_eqp.Dispose();

                                        if (sens.Equipment.Sensitivities != null)
                                        {
                                            try
                                            {
                                                foreach (EquipmentSensitivity sens_eqp_s in sens.Equipment.Sensitivities.ToArray())
                                                {
                                                    YXbsSensorequipsens NH_SensorEquipSensitivity_ = new YXbsSensorequipsens();
                                                    NH_SensorEquipSensitivity_.Format("*");
                                                    if (!NH_SensorEquipSensitivity_.Fetch(string.Format("(SENSOREQUIP_ID={0}) and (ADDLOSS{1}) and (FREQ{2}) and (FREQSTABILITY{3}) and (KTBF{4}) and (NOISEF{5})", ID_NH_sens_eqp, sens_eqp_s.AddLoss.HasValue ? "=" + sens_eqp_s.AddLoss.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.Freq_MHz != Constants.NullD ? "=" + sens_eqp_s.Freq_MHz.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.FreqStability.HasValue ? "=" + sens_eqp_s.FreqStability.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.KTBF_dBm.HasValue ? "=" + sens_eqp_s.KTBF_dBm.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.NoiseF.HasValue ? "=" + sens_eqp_s.NoiseF.ToString().Replace(",", ".") : " IS NULL")))
                                                    {
                                                        NH_SensorEquipSensitivity_.New();
                                                    }
                                                    NH_SensorEquipSensitivity_.m_addloss = sens_eqp_s.AddLoss.GetValueOrDefault();
                                                    NH_SensorEquipSensitivity_.m_freq = sens_eqp_s.Freq_MHz;
                                                    if (sens_eqp_s.FreqStability != null) NH_SensorEquipSensitivity_.m_freqstability = sens_eqp_s.FreqStability.GetValueOrDefault();
                                                    if (sens_eqp_s.KTBF_dBm != null) NH_SensorEquipSensitivity_.m_ktbf = sens_eqp_s.KTBF_dBm.GetValueOrDefault();
                                                    if (sens_eqp_s.NoiseF != null) NH_SensorEquipSensitivity_.m_noisef = sens_eqp_s.NoiseF.GetValueOrDefault();
                                                    NH_SensorEquipSensitivity_.m_sensorequip_id = ID_NH_sens_eqp;
                                                    NH_SensorEquipSensitivity_.Save(dbConnect, transaction);
                                                    NH_SensorEquipSensitivity_.Close();
                                                    NH_SensorEquipSensitivity_.Dispose();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error("Error in SensorEquipSensitivity: " + ex.Message);
                                            }
                                        }

                                        if (sens.Polygon != null)
                                        {
                                            try
                                            {
                                                SensorPolygon Locations = sens.Polygon;
                                                foreach (GeoPoint sens_Locations in Locations.Points)
                                                {
                                                    YXbsSensorpolig NH_sens_location = new YXbsSensorpolig();
                                                    NH_sens_location.Format("*");
                                                    if (!NH_sens_location.Fetch(string.Format("(SENSORID={0}) and (LAT{1}) and (LON{2})", m_ID_Sensor, sens_Locations.Lat.HasValue ? "=" + sens_Locations.Lat.ToString().Replace(",", ".") : " IS NULL", sens_Locations.Lon.HasValue ? "=" + sens_Locations.Lon.ToString().Replace(",", ".") : " IS NULL")))
                                                    {
                                                        NH_sens_location.New();
                                                    }
                                                    if (sens_Locations.Lat != null) NH_sens_location.m_lat = sens_Locations.Lat.GetValueOrDefault();
                                                    if (sens_Locations.Lon != null) NH_sens_location.m_lon = sens_Locations.Lon.GetValueOrDefault();
                                                    NH_sens_location.m_sensorid = m_ID_Sensor;
                                                    NH_sens_location.Save(dbConnect, transaction);
                                                    NH_sens_location.Close();
                                                    NH_sens_location.Dispose();
                                                    logger.Trace("Success created record for table: YXbsSensorpolig");
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error("Error in SensorPoligonPoint: " + ex.Message);
                                            }
                                        }
                                    }
                                }

                            }
                            transaction.Commit();
                            LoadObjectSensor();
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e) { transaction.Dispose();  logger.Error(e.Message); }
                            logger.Error("Error in  procedure CreateNewObjectSensor: " + ex.Message);
                        }
                        finally
                        {
                            transaction.Dispose();
                        }
                    }
                    else
                    {
                        logger.Error("[CreateNewObjectSensor] Error connection  to Database");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                finally
                {
                    dbConnect.Close();
                    dbConnect.Dispose();
                }
            });
            tsk.Start();
            tsk.Join();
            logger.Trace("End procedure CreateNewObjectSensor.");
            return true;
        }

        public bool UpdateObjectSensor(Sensor sens, string apiVersion)
        {
            bool isSuccess = false;
            System.Threading.Thread tsk = new System.Threading.Thread(() =>
            {
                logger.Trace("Start procedure CreateNewObjectSensor...");
                Yyy yyy = new Yyy();
                DbConnection dbConnect = null;
                try
                {
                    dbConnect = yyy.NewConnection(yyy.GetConnectionString());
                    if (dbConnect.State == System.Data.ConnectionState.Open)
                    {
                        DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                        try
                        {
                            if (sens != null)
                            {
                                int? m_ID_Sensor = null;
                                double? val = null;
                                List<Sensor> R_s_find = LoadObjectSensor();
                                Sensor Fnd = R_s_find.Find(t => t.Name == sens.Name && t.Equipment.TechId == sens.Equipment.TechId);
                                YXbsSensor se = new YXbsSensor();
                                se.Format("*");
                                if (se.Fetch(string.Format("NAME='{0}' AND TECHID='{1}'", Fnd.Name, Fnd.Equipment.TechId)))
                                {
                                    m_ID_Sensor = se.m_id;
                                    se.m_administration = sens.Administration;
                                    if (sens.BiuseDate != null) se.m_biusedate = sens.BiuseDate.GetValueOrDefault();
                                    se.m_createdby = sens.CreatedBy;
                                    if (sens.CustDate1 != null) se.m_custdata1 = sens.CustDate1.GetValueOrDefault();
                                    if (sens.CustNbr1 != null) se.m_custnbr1 = sens.CustNbr1.GetValueOrDefault();
                                    se.m_custtxt1 = sens.CustTxt1;
                                    if (sens.Created != null) se.m_datecreated = sens.Created.GetValueOrDefault();
                                    if (sens.EouseDate != null) se.m_eousedate = sens.EouseDate.GetValueOrDefault();
                                    se.m_name = sens.Name;
                                    se.m_networkid = sens.NetworkId;
                                    se.m_remark = sens.Remark;
                                    if (sens.RxLoss != null) se.m_rxloss = sens.RxLoss.GetValueOrDefault();
                                    se.m_status = sens.Status;
                                    if (sens.StepMeasTime != null) se.m_stepmeastime = sens.StepMeasTime.GetValueOrDefault();
                                    se.m_typesensor = sens.Type;
                                    se.m_apiversion = apiVersion;
                                    se.m_techid = sens.Equipment.TechId;
                                    se.m_sensoridentifier_id = m_ID_Sensor;
                                    se.SaveUpdate(dbConnect, transaction);

                                    se.Close();
                                    se.Dispose();

                                    if (sens.Antenna != null)
                                    {
                                        int? ID_sensor_ant = -1;
                                        YXbsSensorantenna NH_sensor_ant = new YXbsSensorantenna();
                                        NH_sensor_ant.Format("*");
                                        if (NH_sensor_ant.Fetch(string.Format("(SENSORID={0})", m_ID_Sensor)))
                                        {
                                            ID_sensor_ant = NH_sensor_ant.m_id;
                                            NH_sensor_ant.m_addloss = sens.Antenna.AddLoss;
                                            NH_sensor_ant.m_antclass = sens.Antenna.Class;
                                            NH_sensor_ant.m_antdir = sens.Antenna.Direction.ToString();
                                            NH_sensor_ant.m_category = sens.Antenna.Category;
                                            NH_sensor_ant.m_code = sens.Antenna.Code;
                                            if (sens.Antenna.CustDate1 != null) NH_sensor_ant.m_custdata1 = sens.Antenna.CustDate1.ToString();
                                            NH_sensor_ant.m_custnbr1 = sens.Antenna.CustNbr1;
                                            NH_sensor_ant.m_custtxt1 = sens.Antenna.CustTxt1;
                                            NH_sensor_ant.m_gainmax = sens.Antenna.GainMax;
                                            NH_sensor_ant.m_gaintype = sens.Antenna.GainType;
                                            if (sens.Antenna.HBeamwidth != null) NH_sensor_ant.m_hbeamwidth = sens.Antenna.HBeamwidth.GetValueOrDefault();
                                            if (sens.Antenna.LowerFreq_MHz != null) NH_sensor_ant.m_lowerfreq = sens.Antenna.LowerFreq_MHz.GetValueOrDefault();
                                            NH_sensor_ant.m_manufacturer = sens.Antenna.Manufacturer;
                                            NH_sensor_ant.m_name = sens.Antenna.Name;
                                            NH_sensor_ant.m_polarization = sens.Antenna.Polarization.ToString();
                                            NH_sensor_ant.m_remark = sens.Antenna.Remark;
                                            if (sens.Antenna.SlewAng != null) NH_sensor_ant.m_slewang = sens.Antenna.SlewAng.GetValueOrDefault();
                                            NH_sensor_ant.m_techid = sens.Antenna.TechId;
                                            if (sens.Antenna.UpperFreq_MHz != null) NH_sensor_ant.m_upperfreq = sens.Antenna.UpperFreq_MHz.GetValueOrDefault();
                                            NH_sensor_ant.m_usetype = sens.Antenna.UseType;
                                            if (sens.Antenna.VBeamwidth != null) NH_sensor_ant.m_vbeamwidth = sens.Antenna.VBeamwidth.GetValueOrDefault();
                                            if (sens.Antenna.XPD != null) NH_sensor_ant.m_xpd = sens.Antenna.XPD.GetValueOrDefault();
                                            NH_sensor_ant.m_sensorid = m_ID_Sensor;
                                            NH_sensor_ant.SaveUpdate(dbConnect, transaction);
                                            //ID_sensor_ant = (int)NH_sensor_ant.Save(dbConnect, transaction);
                                            NH_sensor_ant.Close();
                                            NH_sensor_ant.Dispose();
                                        }

                                        if (sens.Antenna.Patterns != null)
                                        {
                                            foreach (AntennaPattern patt_a in sens.Antenna.Patterns.ToArray())
                                            {
                                                YXbsAntennapattern NH_ant_patt = new YXbsAntennapattern();
                                                NH_ant_patt.Format("*");
                                                if (NH_ant_patt.Fetch(string.Format("(SENSORANTENNA_ID={0}) and (FREQ{1}) and (GAIN{2}) ", ID_sensor_ant, patt_a.Freq_MHz != Constants.NullD ? "=" + patt_a.Freq_MHz.ToString().Replace(",", ".") : " IS NULL", patt_a.Gain != Constants.NullI ? "=" + patt_a.Gain.ToString().Replace(",", ".") : " IS NULL")))
                                                {
                                                    NH_ant_patt.m_diaga = patt_a.DiagA;
                                                    NH_ant_patt.m_diagh = patt_a.DiagH;
                                                    NH_ant_patt.m_diagv = patt_a.DiagV;
                                                    NH_ant_patt.m_freq = patt_a.Freq_MHz;
                                                    NH_ant_patt.m_gain = patt_a.Gain;
                                                    NH_ant_patt.m_sensorantenna_id = ID_sensor_ant;
                                                    NH_ant_patt.SaveUpdate(dbConnect, transaction);
                                                    NH_ant_patt.Close();
                                                    NH_ant_patt.Dispose();
                                                }
                                            }
                                        }
                                    }
                                    if (sens.Equipment != null)
                                    {
                                        int? ID_NH_sens_eqp = -1;
                                        YXbsSensorequip NH_sens_eqp = new YXbsSensorequip();
                                        NH_sens_eqp.Format("*");
                                        if (NH_sens_eqp.Fetch(string.Format("(SENSORID={0})", m_ID_Sensor)))
                                        {

                                            ID_NH_sens_eqp = NH_sens_eqp.m_id;
                                            NH_sens_eqp.m_category = sens.Equipment.Category;
                                            NH_sens_eqp.m_code = sens.Equipment.Code;
                                            if (sens.Equipment.CustDate1 != null) NH_sens_eqp.m_custdata1 = sens.Equipment.CustDate1.GetValueOrDefault();
                                            if (sens.Equipment.CustNbr1 != null) NH_sens_eqp.m_custnbr1 = sens.Equipment.CustNbr1.GetValueOrDefault();
                                            NH_sens_eqp.m_custtxt1 = sens.Equipment.CustTxt1;
                                            NH_sens_eqp.m_equipclass = sens.Equipment.Class;
                                            NH_sens_eqp.m_family = sens.Equipment.Family;
                                            if (sens.Equipment.FFTPointMax != null) NH_sens_eqp.m_fftpointmax = sens.Equipment.FFTPointMax.GetValueOrDefault();
                                            if (sens.Equipment.LowerFreq_MHz != null) NH_sens_eqp.m_lowerfreq = sens.Equipment.LowerFreq_MHz.GetValueOrDefault();
                                            NH_sens_eqp.m_manufacturer = sens.Equipment.Manufacturer;
                                            NH_sens_eqp.m_mobility = sens.Equipment.Mobility == true ? 1 : 0;
                                            NH_sens_eqp.m_name = sens.Equipment.Name;
                                            NH_sens_eqp.m_operationmode = sens.Equipment.OperationMode;
                                            if (sens.Equipment.RBWMax_kHz != null) NH_sens_eqp.m_rbwmax = sens.Equipment.RBWMax_kHz.GetValueOrDefault();
                                            if (sens.Equipment.RBWMin_kHz != null) NH_sens_eqp.m_rbwmin = sens.Equipment.RBWMin_kHz.GetValueOrDefault();
                                            if (sens.Equipment.MaxRefLevel_dBm != null) NH_sens_eqp.m_refleveldbm = sens.Equipment.MaxRefLevel_dBm.GetValueOrDefault();
                                            NH_sens_eqp.m_remark = sens.Equipment.Remark;
                                            NH_sens_eqp.m_techid = sens.Equipment.TechId;
                                            if (sens.Equipment.TuningStep_Hz != null) NH_sens_eqp.m_tuningstep = sens.Equipment.TuningStep_Hz.GetValueOrDefault();
                                            NH_sens_eqp.m_type = sens.Equipment.Type;
                                            if (sens.Equipment.UpperFreq_MHz != null) NH_sens_eqp.m_upperfreq = sens.Equipment.UpperFreq_MHz.GetValueOrDefault();
                                            NH_sens_eqp.m_usetype = sens.Equipment.UseType;
                                            if (sens.Equipment.VBWMax_kHz != null) NH_sens_eqp.m_vbwmax = sens.Equipment.VBWMax_kHz.GetValueOrDefault();
                                            if (sens.Equipment.VBWMin_kHz != null) NH_sens_eqp.m_vbwmin = sens.Equipment.VBWMin_kHz.GetValueOrDefault();
                                            NH_sens_eqp.m_version = sens.Equipment.Version;
                                            NH_sens_eqp.m_sensorid = m_ID_Sensor;
                                            NH_sens_eqp.SaveUpdate(dbConnect, transaction);
                                            NH_sens_eqp.Close();
                                            NH_sens_eqp.Dispose();
                                        }

                                        if (sens.Equipment.Sensitivities != null)
                                        {
                                            try
                                            {
                                                foreach (EquipmentSensitivity sens_eqp_s in sens.Equipment.Sensitivities.ToArray())
                                                {
                                                    YXbsSensorequipsens NH_SensorEquipSensitivity_ = new YXbsSensorequipsens();
                                                    NH_SensorEquipSensitivity_.Format("*");
                                                    if (NH_SensorEquipSensitivity_.Fetch(string.Format("(SENSOREQUIP_ID={0}) and (ADDLOSS{1}) and (FREQ{2}) and (FREQSTABILITY{3}) and (KTBF{4}) and (NOISEF{5})", ID_NH_sens_eqp, sens_eqp_s.AddLoss.HasValue ? "=" + sens_eqp_s.AddLoss.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.Freq_MHz != Constants.NullD ? "=" + sens_eqp_s.Freq_MHz.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.FreqStability.HasValue ? "=" + sens_eqp_s.FreqStability.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.KTBF_dBm.HasValue ? "=" + sens_eqp_s.KTBF_dBm.ToString().Replace(",", ".") : " IS NULL", sens_eqp_s.NoiseF.HasValue ? "=" + sens_eqp_s.NoiseF.ToString().Replace(",", ".") : " IS NULL")))
                                                    {
                                                        NH_SensorEquipSensitivity_.m_addloss = sens_eqp_s.AddLoss.GetValueOrDefault();
                                                        NH_SensorEquipSensitivity_.m_freq = sens_eqp_s.Freq_MHz;
                                                        if (sens_eqp_s.FreqStability != null) NH_SensorEquipSensitivity_.m_freqstability = sens_eqp_s.FreqStability.GetValueOrDefault();
                                                        if (sens_eqp_s.KTBF_dBm != null) NH_SensorEquipSensitivity_.m_ktbf = sens_eqp_s.KTBF_dBm.GetValueOrDefault();
                                                        if (sens_eqp_s.NoiseF != null) NH_SensorEquipSensitivity_.m_noisef = sens_eqp_s.NoiseF.GetValueOrDefault();
                                                        NH_SensorEquipSensitivity_.m_sensorequip_id = ID_NH_sens_eqp;
                                                        NH_SensorEquipSensitivity_.SaveUpdate(dbConnect, transaction);
                                                        NH_SensorEquipSensitivity_.Close();
                                                        NH_SensorEquipSensitivity_.Dispose();
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error("Error in SensorEquipSensitivity: " + ex.Message);
                                            }
                                        }

                                        if (sens.Polygon != null)
                                        {
                                            try
                                            {
                                                SensorPolygon Locations = sens.Polygon;
                                                foreach (GeoPoint sens_Locations in Locations.Points)
                                                {
                                                    YXbsSensorpolig NH_sens_location = new YXbsSensorpolig();
                                                    NH_sens_location.Format("*");
                                                    if (NH_sens_location.Fetch(string.Format("(SENSORID={0}) and (LAT{1}) and (LON{2})", m_ID_Sensor, sens_Locations.Lat.HasValue ? "=" + sens_Locations.Lat.ToString().Replace(",", ".") : " IS NULL", sens_Locations.Lon.HasValue ? "=" + sens_Locations.Lon.ToString().Replace(",", ".") : " IS NULL")))
                                                    {
                                                        if (sens_Locations.Lat != null) NH_sens_location.m_lat = sens_Locations.Lat.GetValueOrDefault();
                                                        if (sens_Locations.Lon != null) NH_sens_location.m_lon = sens_Locations.Lon.GetValueOrDefault();
                                                        NH_sens_location.m_sensorid = m_ID_Sensor;
                                                        NH_sens_location.SaveUpdate(dbConnect, transaction);
                                                        NH_sens_location.Close();
                                                        NH_sens_location.Dispose();
                                                        logger.Trace("Success created record for table: YXbsSensorpolig");
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error("Error in SensorPoligonPoint: " + ex.Message);
                                            }
                                        }
                                    }
                                    isSuccess = true;
                                }
                                else
                                {
                                    isSuccess = false;
                                }

                            }
                            transaction.Commit();
                            LoadObjectSensor();
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e) { transaction.Dispose(); logger.Error(e.Message); }
                            logger.Error("Error in  procedure CreateNewObjectSensor: " + ex.Message);
                        }
                        finally
                        {
                            transaction.Dispose();
                        }
                    }
                    else
                    {
                        logger.Error("[CreateNewObjectSensor] Error connection  to Database");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
                finally
                {
                    dbConnect.Close();
                    dbConnect.Dispose();
                }
            });
            tsk.Start();
            tsk.Join();
            logger.Trace("End procedure CreateNewObjectSensor.");
            return isSuccess;
        }

        /// <summary>
        /// Update status sensor in DB
        /// </summary>
        /// <param name="sens"></param>
        /// <returns></returns>
        public bool UpdateStatusSensor(Sensor sens)
        {
            bool isSaved = false;
            try
            {
                logger.Trace("Start procedure UpdateStatusSensor.");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        if (sens != null)
                        {
                            List<Sensor> R_s_find = LoadObjectSensor();
                            if (R_s_find != null)
                            {
                                Sensor Fnd = R_s_find.Find(t => t.Name == sens.Name && t.Equipment.TechId == sens.Equipment.TechId);
                                if (Fnd != null)
                                {
                                    YXbsSensorequip eq = new YXbsSensorequip();
                                    eq.Format("*");
                                    eq.Filter = string.Format("TECHID='{0}'", sens.Equipment.TechId);
                                    for (eq.OpenRs(); !eq.IsEOF(); eq.MoveNext())
                                    {
                                        YXbsSensor se = new YXbsSensor();
                                        se.Format("*");
                                        se.Filter = string.Format("ID={0}", eq.m_sensorid);
                                        for (se.OpenRs(); !se.IsEOF(); se.MoveNext())
                                        {
                                            if (se.m_name == sens.Name)
                                            {
                                                se.m_status = sens.Status;
                                                se.Save(null, null);
                                                isSaved = true;
                                                break;
                                            }
                                        }
                                        se.Close();
                                        se.Dispose();

                                    }
                                    eq.Close();
                                    eq.Dispose();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure UpdateStatusSensor... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure UpdateStatusSensor.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure UpdateStatusSensor:" + ex.Message);
            }
            return isSaved;
        }

    }
}
