using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Oracle.DataAccess;
using System.Windows.Forms;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.AppServer.Contracts.Sdrns;
using System.IO;
using Atdi.AppServer;
using System.Data.Common;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    public class ClassDBGetSensor : IDisposable
    {
        public static ILogger logger;
        public ClassDBGetSensor(ILogger log)
        {
            if (logger == null) logger = log;
        }

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassDBGetSensor()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<Sensor> LoadObjectAllSensor()
        {
            var val = new List<Sensor>();
            {
                try
                {
                    logger.Trace("Start procedure LoadObjectAllSensor...");
                    System.Threading.Thread tsk = new System.Threading.Thread(() =>
                    {
                        YXbsSensor s_l_sensor = new YXbsSensor();
                        s_l_sensor.Format("*");
                        // выбирать только сенсоры, для которых STATUS не NULL
                        s_l_sensor.Filter = string.Format("(ID>0)");
                        s_l_sensor.Order = "[ID] DESC";
                        for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                        {
                            Sensor it_out = new Sensor();
                            it_out.Id = new SensorIdentifier();
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
                            it_out.Id.Value = (int)s_l_sensor.m_id;
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
                            it_out.Antenna = new SensorAntenna();

                            //Antenna
                            YXbsSensorantenna mpt = new YXbsSensorantenna();
                            mpt.Format("*");
                            mpt.Filter = string.Format("SENSORID={0}", it_out.Id.Value);
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
                                    ptu.Freq = (double)itr.m_freq;
                                    ptu.Gain = (double)itr.m_gain;
                                    L_ant_patt.Add(ptu);
                                }
                                itr.Close();
                                itr.Dispose();
                                it_out.Antenna.AntennaPatterns = L_ant_patt.ToArray();
                            }
                            mpt.Close();
                            mpt.Dispose();

                            // Equipments
                            it_out.Equipment = new SensorEquip();
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

                                    List<SensorEquipSensitivity> L_sens = new List<SensorEquipSensitivity>();
                                    YXbsSensorequipsens itr = new YXbsSensorequipsens();
                                    itr.Format("*");
                                    itr.Filter = string.Format("SENSOREQUIP_ID={0}", mpt_.m_id);
                                    for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                    {
                                        SensorEquipSensitivity ptu = new SensorEquipSensitivity();
                                        ptu.AddLoss = itr.m_addloss;
                                        ptu.Freq = (double)itr.m_freq;
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

                            List<SensorLocation> L_Sens_loc = new List<SensorLocation>();
                            YXbsSensorlocation mpt_loc = new YXbsSensorlocation();
                            mpt_loc.Format("*");
                            mpt_loc.Filter = string.Format("(SENSORID={0})", it_out.Id.Value);
                            for (mpt_loc.OpenRs(); !mpt_loc.IsEOF(); mpt_loc.MoveNext())
                            {
                                SensorLocation s_l = new SensorLocation();
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
                catch (Exception ex) {
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
                        YXbsSensor s_l_sensor = new YXbsSensor();
                        s_l_sensor.Format("*");
                        // выбирать только сенсоры, для которых STATUS не NULL
                        s_l_sensor.Filter = string.Format("(ID={0})", Id);
                        s_l_sensor.Order = "[ID] DESC";
                        for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                        {
                            Sensor it_out = new Sensor();
                            it_out.Id = new SensorIdentifier();
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
                            it_out.Id.Value = (int)s_l_sensor.m_id;
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
                            it_out.Antenna = new SensorAntenna();

                            //Antenna
                            YXbsSensorantenna mpt = new YXbsSensorantenna();
                            mpt.Format("*");
                            mpt.Filter = string.Format("SENSORID={0}", it_out.Id.Value);
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
                                    ptu.Freq = (double)itr.m_freq;
                                    ptu.Gain = (double)itr.m_gain;
                                    L_ant_patt.Add(ptu);
                                }
                                itr.Close();
                                itr.Dispose();
                                it_out.Antenna.AntennaPatterns = L_ant_patt.ToArray();
                            }
                            mpt.Close();
                            mpt.Dispose();

                            // Equipments
                            it_out.Equipment = new SensorEquip();
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

                                    List<SensorEquipSensitivity> L_sens = new List<SensorEquipSensitivity>();
                                    YXbsSensorequipsens itr = new YXbsSensorequipsens();
                                    itr.Format("*");
                                    itr.Filter = string.Format("SENSOREQUIP_ID={0}", mpt_.m_id);
                                    for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                    {
                                        SensorEquipSensitivity ptu = new SensorEquipSensitivity();
                                        ptu.AddLoss = itr.m_addloss;
                                        ptu.Freq = (double)itr.m_freq;
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

                            List<SensorLocation> L_Sens_loc = new List<SensorLocation>();
                            YXbsSensorlocation mpt_loc = new YXbsSensorlocation();
                            mpt_loc.Format("*");
                            mpt_loc.Filter = string.Format("(SENSORID={0})", it_out.Id.Value);
                            for (mpt_loc.OpenRs(); !mpt_loc.IsEOF(); mpt_loc.MoveNext())
                            {
                                SensorLocation s_l = new SensorLocation();
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
                            val = it_out;
                            break;
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

        public List<Sensor> LoadObjectSensor(string Name, string TechId, string status)
        {
            var val = new List<Sensor>();
            {
                try
                {
                    logger.Trace("Start procedure LoadObjectSensor...");
                    System.Threading.Thread tsk = new System.Threading.Thread(() =>
                    {
                        YXbsSensor s_l_sensor = new YXbsSensor();
                        s_l_sensor.Format("*");
                        // выбирать только сенсоры, для которых STATUS не NULL
                        s_l_sensor.Filter = string.Format("(ID>0) AND (STATUS='{0}') AND (NAME='{1}')", status, Name);
                        s_l_sensor.Order = "[ID] DESC";
                        for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                        {
                            Sensor it_out = new Sensor();
                            it_out.Id = new SensorIdentifier();
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
                            it_out.Id.Value = (int)s_l_sensor.m_id;
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
                            it_out.Antenna = new SensorAntenna();

                            //Antenna
                            YXbsSensorantenna mpt = new YXbsSensorantenna();
                            mpt.Format("*");
                            mpt.Filter = string.Format("SENSORID={0}", it_out.Id.Value);
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
                                    ptu.Freq = (double)itr.m_freq;
                                    ptu.Gain = (double)itr.m_gain;
                                    L_ant_patt.Add(ptu);
                                }
                                itr.Close();
                                itr.Dispose();
                                it_out.Antenna.AntennaPatterns = L_ant_patt.ToArray();
                            }
                            mpt.Close();
                            mpt.Dispose();

                            // Equipments
                            it_out.Equipment = new SensorEquip();
                            {
                                YXbsSensorequip mpt_ = new YXbsSensorequip();
                                mpt_.Format("*");
                                mpt_.Filter = string.Format("(SENSORID={0}) AND (TECHID='{1}') ", it_out.Id.Value, TechId);
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

                                    List<SensorEquipSensitivity> L_sens = new List<SensorEquipSensitivity>();
                                    YXbsSensorequipsens itr = new YXbsSensorequipsens();
                                    itr.Format("*");
                                    itr.Filter = string.Format("SENSOREQUIP_ID={0}", mpt_.m_id);
                                    for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                    {
                                        SensorEquipSensitivity ptu = new SensorEquipSensitivity();
                                        ptu.AddLoss = itr.m_addloss;
                                        ptu.Freq = (double)itr.m_freq;
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

                            List<SensorLocation> L_Sens_loc = new List<SensorLocation>();
                            YXbsSensorlocation mpt_loc = new YXbsSensorlocation();
                            mpt_loc.Format("*");
                            mpt_loc.Filter = string.Format("(SENSORID={0}) AND (STATUS='{1}')", it_out.Id.Value, status);
                            for (mpt_loc.OpenRs(); !mpt_loc.IsEOF(); mpt_loc.MoveNext())
                            {
                                SensorLocation s_l = new SensorLocation();
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
                    
                    logger.Trace("End procedure LoadObjectSensor.");
                }
                catch (Exception ex) {
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
                        // выбирать только сенсоры, для которых STATUS не NULL
                        s_l_sensor.Filter = "(ID>0) AND (STATUS<>'Z')";
                        for (s_l_sensor.OpenRs(); !s_l_sensor.IsEOF(); s_l_sensor.MoveNext())
                        {
                            Sensor it_out = new Sensor();
                            it_out.Id = new SensorIdentifier();
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
                            it_out.Id.Value = (int)s_l_sensor.m_id;
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
                            it_out.Antenna = new SensorAntenna();

                            //Antenna
                            YXbsSensorantenna mpt = new YXbsSensorantenna();
                            mpt.Format("*");
                            mpt.Filter = string.Format("(SENSORID={0})", it_out.Id.Value);
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
                                    ptu.Freq = (double)itr.m_freq;
                                    ptu.Gain = (double)itr.m_gain;
                                    L_ant_patt.Add(ptu);
                                }
                                itr.Close();
                                itr.Dispose();
                                it_out.Antenna.AntennaPatterns = L_ant_patt.ToArray();
                            }
                            mpt.Close();
                            mpt.Dispose();

                            // Equipments
                            it_out.Equipment = new SensorEquip();
                            {
                                YXbsSensorequip mpt_ = new YXbsSensorequip();
                                mpt_.Format("*");
                                mpt_.Filter = string.Format("SENSORID={0}", it_out.Id.Value);
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

                                    List<SensorEquipSensitivity> L_sens = new List<SensorEquipSensitivity>();
                                    YXbsSensorequipsens itr = new YXbsSensorequipsens();
                                    itr.Format("*");
                                    itr.Filter = string.Format("SENSOREQUIP_ID={0}", mpt_.m_id);
                                    for (itr.OpenRs(); !itr.IsEOF(); itr.MoveNext())
                                    {
                                        SensorEquipSensitivity ptu = new SensorEquipSensitivity();
                                        ptu.AddLoss = itr.m_addloss;
                                        ptu.Freq = (double)itr.m_freq;
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

                            List<SensorLocation> L_Sens_loc = new List<SensorLocation>();
                            YXbsSensorlocation mpt_loc = new YXbsSensorlocation();
                            mpt_loc.Format("*");
                            mpt_loc.Filter = string.Format("(SENSORID={0}) AND (STATUS<>'{1}')", it_out.Id.Value, "Z");
                            for (mpt_loc.OpenRs(); !mpt_loc.IsEOF(); mpt_loc.MoveNext())
                            {
                                SensorLocation s_l = new SensorLocation();
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

                            List<SensorPoligonPoint> L_Sens_loc_pt = new List<SensorPoligonPoint>();
                            YXbsSensorpolig mpt_loc_pt = new YXbsSensorpolig();
                            mpt_loc_pt.Format("*");
                            mpt_loc_pt.Filter = string.Format("(SENSORID={0})", it_out.Id.Value);
                            for (mpt_loc_pt.OpenRs(); !mpt_loc_pt.IsEOF(); mpt_loc_pt.MoveNext())
                            {
                                SensorPoligonPoint s_l = new SensorPoligonPoint();
                                s_l.Lat = mpt_loc.m_lat;
                                s_l.Lon = mpt_loc.m_lon;
                                L_Sens_loc_pt.Add(s_l);
                            }
                            mpt_loc_pt.Close();
                            mpt_loc_pt.Dispose();
                            it_out.Poligon = L_Sens_loc_pt.ToArray();
                            val.Add(it_out);


                        }
                        s_l_sensor.Close();
                        s_l_sensor.Dispose();
                    }
                    catch (Exception ex) {
                        logger.Error("Error in procedure LoadObjectSensor." + ex.Message);
                    }
                });
            tsk.Start();
            tsk.Join();
            
            logger.Trace("End procedure LoadObjectSensor.");
            return val;
        }


        public bool SaveLocationCoordSensor(Sensor sens)
        {
            Yyy yyy = new Yyy();
            DbConnection dbConnect = yyy.NewConnection(yyy.GetConnectionString());
            DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                logger.Trace("Start procedure SaveLocationCoordSensor...");
                if (sens != null)
                {
                    System.Threading.Thread tsk = new System.Threading.Thread(() =>
                    {
                        double? val = null;
                        int m_ID_Sensor = -1;
                        List<Sensor> R_s_find = LoadObjectSensor();
                        Sensor Fnd = R_s_find.Find(t => t.Name == sens.Name && t.Equipment.TechId == sens.Equipment.TechId);
                        YXbsSensor se = new YXbsSensor();
                        se.Format("*");
                        if (Fnd != null)
                        {
                            se.Fetch(Fnd.Id.Value); m_ID_Sensor = Fnd.Id.Value;

                            if (sens.Locations != null)
                            {
                                foreach (SensorLocation sens_Locations in sens.Locations.ToArray())
                                {
                                    YXbsSensorlocation NH_sens_location_old = new YXbsSensorlocation();
                                    NH_sens_location_old.Format("*");
                                    NH_sens_location_old.Filter = string.Format("(SENSORID={0}) and (STATUS='A')", m_ID_Sensor);
                                    for (NH_sens_location_old.OpenRs(); !NH_sens_location_old.IsEOF(); NH_sens_location_old.MoveNext())
                                    {
                                        NH_sens_location_old.m_status = "Z";
                                        NH_sens_location_old.Save(dbConnect,transaction);
                                    }
                                    NH_sens_location_old.Close();
                                    NH_sens_location_old.Dispose();

                                    logger.Trace(string.Format("Format SaveLocationCoordSensor: {0}", string.Format("(SENSORID={0}) and (ASL{1}) and (LAT{2}) and (LON{3})", m_ID_Sensor, sens_Locations.ASL.HasValue ? "=" + sens_Locations.ASL.ToString().Replace(",", ".") : " IS NULL", sens_Locations.Lat.HasValue ? "=" + sens_Locations.Lat.ToString().Replace(",", ".") : " IS NULL", sens_Locations.Lon.HasValue ? "=" + sens_Locations.Lon.ToString().Replace(",", ".") : " IS NULL")));
                                    
                                    YXbsSensorlocation NH_sens_location = new YXbsSensorlocation();
                                    NH_sens_location.Format("*");
                                    if (!NH_sens_location.Fetch(string.Format("(SENSORID={0}) and (ASL{1}) and (LAT{2}) and (LON{3})", m_ID_Sensor, sens_Locations.ASL.HasValue ? "=" + sens_Locations.ASL.ToString().Replace(",", ".") : " IS NULL", sens_Locations.Lat.HasValue ? "=" + sens_Locations.Lat.ToString().Replace(",", ".") : " IS NULL", sens_Locations.Lon.HasValue ? "=" + sens_Locations.Lon.ToString().Replace(",", ".") : " IS NULL")))
                                    {
                                        NH_sens_location.New();
                                        //NH_sens_location.AllocID();
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

                        }
                        se.Close();
                        se.Dispose();
                        LoadObjectSensor();
                    });
                    tsk.Start();
                    tsk.Join();
                    logger.Trace("End procedure SaveLocationCoordSensor.");
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                logger.Error("End procedure SaveLocationCoordSensor: " + ex.Message);
            }
            transaction.Dispose();
            dbConnect.Close();
            dbConnect.Dispose();
            return true;
        }

        /// <summary>
        /// Create new object
        /// </summary>
        /// <param name="sens"></param>
        /// <returns></returns>
        public bool CreateNewObjectSensor(Sensor sens)
        {
            Yyy yyy = new Yyy();
            DbConnection dbConnect = yyy.NewConnection(yyy.GetConnectionString());
            DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                logger.Trace("Start procedure CreateNewObjectSensor...");
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    if (sens != null)
                    {
                        double? val = null;
                        int m_ID_Sensor = -1;
                        List<Sensor> R_s_find = LoadObjectSensor();
                        Sensor Fnd = R_s_find.Find(t => t.Name == sens.Name && t.Equipment.TechId == sens.Equipment.TechId);
                        YXbsSensor se = new YXbsSensor();
                        se.Format("*");
                        if (Fnd == null)
                        {
                            se.Filter = "(ID=-1)";
                            se.New();
                        }
                        else { se.Fetch(Fnd.Id.Value); m_ID_Sensor = Fnd.Id.Value; }
                        
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
                        m_ID_Sensor = (int)se.Save(dbConnect, transaction);

                        if (se.Fetch(string.Format("ID={0}", m_ID_Sensor)))
                        {
                            se.m_sensoridentifier_id = m_ID_Sensor;
                            se.SaveUpdate(dbConnect, transaction);
                        }

                        se.Close();
                        se.Dispose();

                        if (sens.Antenna != null)
                        {
                            int ID_sensor_ant = -1;
                            YXbsSensorantenna NH_sensor_ant = new YXbsSensorantenna();
                            NH_sensor_ant.Format("*");
                            if (!NH_sensor_ant.Fetch(string.Format("(SENSORID={0})", m_ID_Sensor)))
                            {
                                NH_sensor_ant.New();
                            }
                            else ID_sensor_ant = (int)NH_sensor_ant.m_id;
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
                            ID_sensor_ant = (int)NH_sensor_ant.Save(dbConnect, transaction);
                            NH_sensor_ant.Close();
                            NH_sensor_ant.Dispose();

                            if (sens.Antenna.AntennaPatterns != null)
                            {
                                foreach (AntennaPattern patt_a in sens.Antenna.AntennaPatterns.ToArray())
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
                            int ID_NH_sens_eqp = -1;
                            YXbsSensorequip NH_sens_eqp = new YXbsSensorequip();
                            NH_sens_eqp.Format("*");
                            if (!NH_sens_eqp.Fetch(string.Format("(SENSORID={0})", m_ID_Sensor)))
                            {
                                NH_sens_eqp.New();
                            }
                            else ID_NH_sens_eqp = (int)NH_sens_eqp.m_id;
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
                            ID_NH_sens_eqp = (int)NH_sens_eqp.Save(dbConnect, transaction);
                            NH_sens_eqp.Close();
                            NH_sens_eqp.Dispose();
                            if (sens.Equipment.SensorEquipSensitivities != null)
                            {
                                try
                                {
                                foreach (SensorEquipSensitivity sens_eqp_s in sens.Equipment.SensorEquipSensitivities.ToArray())
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
                              catch (Exception ex) {
                                    logger.Error("Error in SensorEquipSensitivity: " + ex.Message);
                                }
                            }
                            if (sens.Locations != null)
                            {
                                try
                                {
                                    foreach (SensorLocation sens_Locations in sens.Locations.ToArray())
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
                                catch (Exception ex) {
                                    logger.Error("Error in SensorLocation: " + ex.Message); }
                            }
                            if (sens.Poligon != null)
                            {
                                try
                                {
                                    foreach (SensorPoligonPoint sens_Locations in sens.Poligon.ToArray()) {
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
                                catch (Exception ex) {
                                    logger.Error("Error in SensorPoligonPoint: " + ex.Message); }
                            }
                        }
                    
                }
                transaction.Commit();
                LoadObjectSensor();
                });
                tsk.Start();
                tsk.Join();
                
                logger.Trace("End procedure CreateNewObjectSensor.");
            }
            catch (Exception ex) {
                transaction.Rollback();
                logger.Error("Error in  procedure CreateNewObjectSensor: "+ex.Message);
            }
            transaction.Dispose();
            dbConnect.Close();
            dbConnect.Dispose();
            return true;
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
                    if (sens != null)
                    {
                        List<Sensor> R_s_find = LoadObjectSensor();
                        if (R_s_find != null)
                        {
                            Sensor Fnd = R_s_find.Find(t => t.Name == sens.Name && t.Equipment.TechId == sens.Equipment.TechId);
                            if (Fnd != null)
                            {
                                YXbsSensor se = new YXbsSensor();
                                se.Format("*");
                                se.Filter = string.Format("ID={0}", Fnd.Id.Value);
                                for (se.OpenRs(); !se.IsEOF(); se.MoveNext())
                                {
                                    se.m_status = sens.Status;
                                    se.Save(null,null);
                                    break;
                                }
                                se.Close();
                                se.Dispose();
                            }
                        }
                        isSaved = true;
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

        public bool UpdateStatusSensorWithArchive(Sensor sens)
        {
            bool isSaved = false;
            try
            {
                logger.Trace("Start procedure UpdateStatusSensorWithArchive...");
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    if (sens != null)
                    {
                        YXbsSensor se = new YXbsSensor();
                        se.Format("*");
                        se.Filter = string.Format("ID={0}", sens.Id.Value);
                        for (se.OpenRs(); !se.IsEOF(); se.MoveNext())
                        {
                            se.m_status = sens.Status;
                            se.Save(null,null);
                            break;
                        }
                        se.Close();
                        se.Dispose();

                        isSaved = true;
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End  procedure UpdateStatusSensorWithArchive...");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure UpdateStatusSensorWithArchive..." + ex.Message);
            }
            return isSaved;
        }
    }
}
