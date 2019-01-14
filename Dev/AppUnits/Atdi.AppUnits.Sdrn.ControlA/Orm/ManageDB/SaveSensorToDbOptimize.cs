using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Contracts.Sdrns;
using NHibernate;
using Atdi.AppUnits.Sdrn.ControlA;

namespace Atdi.AppUnits.Sdrn.ControlA.Handlers
{
    public enum AllStatusSensor
    {
        N,
        A,
        Z,
        E,
        C,
        F
    }
    public enum AllStatusLocation
    {
        A,
        Z
    }
    public class SensorDBExtension
    {

        /// <summary>
        /// Update status sensor
        /// </summary>
        public void UpdateStatus()
        {
            SensorDBExtension extension = new SensorDBExtension();
            Sensor se_curr = extension.GetCurrentSensor();
            if (se_curr != null)  {
                 UpdateSensorStatus(se_curr, AllStatusSensor.A);
            }
        }
        /// <summary>
        /// Set old Location.Status='Z'
        /// </summary>
        /// <param name="sens_loc"></param>
        public void CloseOldSensorLocation(NH_SensorLocation sens_loc)
        {
            try {
                if (sens_loc != null) {
                    ClassObjectsSensorOnSDR cl = new ClassObjectsSensorOnSDR();
                    if (Domain.sessionFactory == null) Domain.Init();
                    ISession session = Domain.CurrentSession;
                    ITransaction tr_1 = session.BeginTransaction();
                    sens_loc.Status = AllStatusLocation.Z.ToString();
                    sens_loc.DataTo = DateTime.Now;
                    cl.UpdateObject<NH_SensorLocation>(sens_loc.ID, sens_loc);
                    tr_1.Commit();
                    session.Flush();
                    tr_1.Dispose();
                }
                LoadObjectSensor();
            }
            catch (Exception ex) {
                BusManager._logger.Error(Contexts.ThisComponent, Categories.UpdateSensorStatus, Events.UpdateSensorStatus, ex.Message, null);
            }

        }

        /// <summary>
        /// Add new location into DataBase
        /// </summary>
        /// <param name="Id_sensor"></param>
        /// <param name="Lon"></param>
        /// <param name="Lat"></param>
        /// <param name="ASL"></param>
        /// <param name="DateFrom"></param>
        /// <param name="DateTo"></param>
        /// <param name="loc_status"></param>
        public void AddNewLocations(int Id_sensor, double Lon, double Lat, double ASL, DateTime DateFrom, DateTime DateTo, AllStatusLocation loc_status)
        {
            try
            {
                    if (Domain.sessionFactory == null) Domain.Init();
                    ISession session = Domain.CurrentSession;
                    ITransaction tr_1 = session.BeginTransaction();
                    NH_SensorLocation NH_sens_location = new NH_SensorLocation();
                    NH_sens_location.ASL = ASL;
                    NH_sens_location.DataCreated = DateTime.Now;
                    NH_sens_location.DataFrom = DateFrom;
                    //NH_sens_location.DataTo = DateTo;
                    NH_sens_location.Lat = Lat;
                    NH_sens_location.Lon = Lon;
                    NH_sens_location.Status = loc_status.ToString();
                    NH_sens_location.SensorID = Id_sensor;
                    NH_sens_location = (NH_SensorLocation)SetNullValue(NH_sens_location);
                    session.Save(NH_sens_location);
                    Console.WriteLine("Successfully saved into table - NH_SensorLocation");
                    tr_1.Commit();
                    session.Flush();
                    tr_1.Dispose();
                
                LoadObjectSensor();
            }
            catch (Exception ex) {
                BusManager._logger.Error(Contexts.ThisComponent, Categories.UpdateSensorStatus, Events.UpdateSensorStatus, ex.Message, null);
            }
        }

        /// <summary>
        /// Update status sensor
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="newStatus"></param>
        public void UpdateSensorStatus(Sensor sensor, AllStatusSensor newStatus)
        {
            try {
                NH_Sensor ngh_s = LoadSensorFromDB(sensor.Name, sensor.Equipment.TechId);
                if (ngh_s != null) {
                    ClassObjectsSensorOnSDR cl = new ClassObjectsSensorOnSDR();
                    if (Domain.sessionFactory == null) Domain.Init();
                    ISession session = Domain.CurrentSession;
                    ITransaction tr_1 = session.BeginTransaction();
                    ngh_s.Status = newStatus.ToString();
                    ngh_s.Administration = sensor.Administration;
                    ngh_s.AGL = sensor.AGL;
                    ngh_s.Azimuth = sensor.Azimuth;
                    ngh_s.BiuseDate = sensor.BiuseDate;
                    ngh_s.CreatedBy = sensor.CreatedBy;
                    ngh_s.CustData1 = sensor.CustData1;
                    ngh_s.CustNbr1 = sensor.CustNbr1;
                    ngh_s.CustTxt1 = sensor.CustTxt1;
                    ngh_s.DateCreated = sensor.DateCreated;
                    ngh_s.Elevation = sensor.Elevation;
                    ngh_s.EouseDate = sensor.EouseDate;
                    ngh_s.IdSysARGUS = sensor.IdSysARGUS;
                    ngh_s.Name = sensor.Name;
                    ngh_s.NetworkId = sensor.NetworkId;
                    ngh_s.OpDays = sensor.OpDays;
                    ngh_s.OpHHFr = sensor.OpHHFr;
                    ngh_s.OpHHTo = sensor.OpHHTo;
                    ngh_s.Remark = sensor.Remark;
                    ngh_s.RxLoss = sensor.RxLoss;
                    ngh_s.StepMeasTime = sensor.StepMeasTime;
                    ngh_s.TypeSensor = sensor.TypeSensor;
                    cl.UpdateObject<NH_Sensor>(ngh_s.ID, ngh_s);
                    tr_1.Commit();
                    session.Flush();
                    tr_1.Dispose();
                }
                LoadObjectSensor();
            }
            catch (Exception ex) {
                BusManager._logger.Error(Contexts.ThisComponent, Categories.UpdateSensorStatus, Events.UpdateSensorStatus, ex.Message, null);
            }
        }

        /// <summary>
        /// Load sensor object from Database
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="TechId"></param>
        /// <returns></returns>
        public NH_Sensor LoadSensorFromDB(string Name, string TechId)
        {
            NH_Sensor sens = new NH_Sensor();
            if (Domain.sessionFactory == null) Domain.Init();
            IList<NH_Sensor> s_l_sensor = null;
            ISession session = Domain.CurrentSession;
            ITransaction tr_1 = session.BeginTransaction();
            try {
                s_l_sensor =
                   session.QueryOver<NH_Sensor>()
                   .Fetch(x => x.ID).Eager.Where(x => x.Name == Name).List();
                foreach (NH_Sensor item in s_l_sensor)  {
                    IList<NH_SensorEquip> s_l_rquip = session.QueryOver<NH_SensorEquip>().Fetch(x => x.SensorID).Eager.Where(x => x.TechId == TechId).List();
                    if (s_l_rquip.Count > 0) {
                        sens = item;
                        break;
                    }
                }
            }
            catch (Exception ex) {
                tr_1.Rollback();
                BusManager._logger.Error(Contexts.ThisComponent, Categories.LoadSensor, Events.LoadSensor, ex.Message, null);
            }
            tr_1.Dispose();
            return sens;
        }

        /// <summary>
        /// Load list objects NH_SensorLocation from DB
        /// </summary>
        /// <param name="Id_sensor"></param>
        /// <returns></returns>
        public List<NH_SensorLocation> LoadSensorLocationsFromDB(int Id_sensor)
        {
            List<NH_SensorLocation> s_l_loc_ = new List<NH_SensorLocation>(); 
            if (Domain.sessionFactory == null) Domain.Init();
            IList<NH_Sensor> s_l_sensor = null;
            ISession session = Domain.CurrentSession;
            ITransaction tr_1 = session.BeginTransaction();
            try
            {
                List<int> X_s = new List<int>();
                s_l_sensor =
                   session.QueryOver<NH_Sensor>()
                   .Fetch(x => x.ID).Eager.Where(x => x.ID == Id_sensor).List();
                foreach (NH_Sensor item in s_l_sensor) {
                   IList<NH_SensorEquip> s_l_rquip = session.QueryOver<NH_SensorEquip>().Fetch(x => x.SensorID).Eager.Where(x => x.SensorID == Id_sensor).List();
                    if (s_l_rquip.Count > 0)
                    {
                        IList<NH_SensorLocation> s_l_loc = session.QueryOver<NH_SensorLocation>().Fetch(x => x.SensorID).Eager.Where(x => x.SensorID == item.ID && x.Status!=AllStatusLocation.Z.ToString()).List();
                        if (s_l_loc.Count > 0){
                            foreach (NH_SensorLocation fd in s_l_loc) {
                                s_l_loc_.Add(fd);
                                X_s.Add(fd.ID);
                            }
                        }
                        X_s.Sort();
                        X_s.Reverse();
                        if (X_s.Count > 0) s_l_loc_.RemoveAll(t => t.ID == X_s[0]);
                    }
                }
            }
            catch (Exception) { tr_1.Rollback(); }
            tr_1.Dispose();
            return s_l_loc_;
        }


        public Sensor GetCurrentSensor()
        {
            var list = LoadObjectSensor();
            if ((list != null) && (list.Count > 0))
            {
                return list[0];
            }
            else return null;
        }

            /// <summary>
            /// Load all sensors from DB
            /// </summary>
            /// <returns></returns>
        public List<Sensor> LoadObjectSensor()
        {
            List<Sensor> val = new List<Sensor>();
            if (Domain.sessionFactory == null) Domain.Init();
            IList<NH_Sensor> s_l_sensor = null;
            ISession session = Domain.CurrentSession;
            {
                ITransaction tr_1 = session.BeginTransaction();
                try
                {
                    s_l_sensor =
                       session.QueryOver<NH_Sensor>()
                       .Fetch(x => x.ID).Eager.List();
                    foreach (NH_Sensor item in s_l_sensor)
                    {
                        Sensor it_out = new Sensor();
                        it_out.Id = new SensorIdentifier();
                        it_out.Administration = item.Administration;
                        it_out.AGL = item.AGL;
                        it_out.Azimuth = item.Azimuth;
                        it_out.BiuseDate = item.BiuseDate;
                        it_out.CreatedBy = item.CreatedBy;
                        it_out.CustData1 = item.CustData1;
                        it_out.CustNbr1 = item.CustNbr1;
                        it_out.CustTxt1 = item.CustTxt1;
                        it_out.DateCreated = item.DateCreated;
                        it_out.Elevation = item.Elevation;
                        it_out.EouseDate = item.EouseDate;
                        it_out.IdSysARGUS = item.IdSysARGUS;
                        it_out.Id.Value = item.ID;
                        it_out.Name = item.Name;
                        it_out.NetworkId = item.NetworkId;
                        it_out.OpDays = item.OpDays;
                        it_out.OpHHFr = item.OpHHFr;
                        it_out.OpHHTo = item.OpHHTo;
                        it_out.Remark = item.Remark;
                        it_out.RxLoss = item.RxLoss;
                        it_out.Status = item.Status;
                        it_out.StepMeasTime = item.StepMeasTime;
                        it_out.TypeSensor = item.TypeSensor;
                        it_out.Antenna = new SensorAntenna();
                        IList<NH_SensorAntenna> s_l_antenna = session.QueryOver<NH_SensorAntenna>().Fetch(x => x.SensorID).Eager.Where(x => x.SensorID == it_out.Id.Value).List();
                        foreach (NH_SensorAntenna mpt in s_l_antenna)
                        {
                            List<AntennaPattern> L_ant_patt = new List<AntennaPattern>();
                            IList<NH_AntennaPattern> s_l_antenna_mpt = session.QueryOver<NH_AntennaPattern>().Fetch(x => x.SensorAntenna_ID).Eager.Where(x => x.SensorAntenna_ID == mpt.ID).List();
                            foreach (NH_AntennaPattern itr in s_l_antenna_mpt)
                            {
                                AntennaPattern ptu = new AntennaPattern();
                                ptu.DiagA = itr.DiagA;
                                ptu.DiagH = itr.DiagH;
                                ptu.DiagV = itr.DiagV;
                                ptu.Freq = itr.Freq;
                                ptu.Gain = itr.Gain;
                                L_ant_patt.Add(ptu);
                            }
                            it_out.Antenna.AntennaPatterns = L_ant_patt.ToArray();
                        }

                        it_out.Equipment = new SensorEquip();
                        IList<NH_SensorEquip> s_l_rquip = session.QueryOver<NH_SensorEquip>().Fetch(x => x.SensorID).Eager.Where(x => x.SensorID == it_out.Id.Value).List();
                        foreach (NH_SensorEquip mpt in s_l_rquip)
                        {
                            it_out.Equipment.Category = mpt.Category;
                            it_out.Equipment.Code = mpt.Code;
                            it_out.Equipment.CustData1 = mpt.CustData1;
                            it_out.Equipment.CustNbr1 = mpt.CustNbr1;
                            it_out.Equipment.CustTxt1 = mpt.CustTxt1;
                            it_out.Equipment.EquipClass = mpt.EquipClass;
                            it_out.Equipment.Family = mpt.Family;
                            it_out.Equipment.FFTPointMax = mpt.FFTPointMax;
                            it_out.Equipment.LowerFreq = mpt.LowerFreq;
                            it_out.Equipment.Manufacturer = mpt.Manufacturer;
                            it_out.Equipment.Mobility = mpt.Mobility;
                            it_out.Equipment.Name = mpt.Name;
                            it_out.Equipment.OperationMode = mpt.OperationMode;
                            it_out.Equipment.RBWMax = mpt.RBWMax;
                            it_out.Equipment.RBWMin = mpt.RBWMin;
                            it_out.Equipment.RefLeveldBm = mpt.RefLeveldBm;
                            it_out.Equipment.Remark = mpt.Remark;
                            it_out.Equipment.TechId = mpt.TechId;
                            it_out.Equipment.TuningStep = mpt.TuningStep;
                            it_out.Equipment.Type = mpt.Type;
                            it_out.Equipment.UpperFreq = mpt.UpperFreq;
                            it_out.Equipment.UseType = mpt.UseType;
                            it_out.Equipment.VBWMax = mpt.VBWMax;
                            it_out.Equipment.VBWMin = mpt.VBWMin;
                            it_out.Equipment.Version = mpt.Version;

                            List<SensorEquipSensitivity> L_sens = new List<SensorEquipSensitivity>();
                            IList<NH_SensorEquipSensitivity> s_l_sens_mpt = session.QueryOver<NH_SensorEquipSensitivity>().Fetch(x => x.SensorEquip_ID).Eager.Where(x => x.SensorEquip_ID == mpt.ID).List();
                            foreach (NH_SensorEquipSensitivity itr in s_l_sens_mpt)
                            {
                                SensorEquipSensitivity ptu = new SensorEquipSensitivity();
                                ptu.AddLoss = itr.AddLoss;
                                ptu.Freq = itr.Freq.GetValueOrDefault();
                                ptu.FreqStability = itr.FreqStability;
                                ptu.KTBF = itr.KTBF;
                                ptu.NoiseF = itr.NoiseF;
                                L_sens.Add(ptu);
                            }
                            it_out.Equipment.SensorEquipSensitivities = L_sens.ToArray();
                        }

                        List<SensorLocation> L_Sens_loc = new List<SensorLocation>();
                        IList<NH_SensorLocation> s_l_location = session.QueryOver<NH_SensorLocation>().Fetch(x => x.SensorID).Eager.Where(x => x.SensorID == it_out.Id.Value && x.Status != AllStatusLocation.Z.ToString()).List();
                        foreach (NH_SensorLocation mpt in s_l_location)
                        {
                            SensorLocation s_l = new SensorLocation();
                            s_l.ASL = mpt.ASL;
                            s_l.DataCreated = mpt.DataCreated;
                            s_l.DataFrom = mpt.DataFrom;
                            s_l.DataTo = mpt.DataTo;
                            s_l.Lat = mpt.Lat;
                            s_l.Lon = mpt.Lon;
                            s_l.Status = mpt.Status;
                            L_Sens_loc.Add(s_l);
                        }
                        it_out.Locations = L_Sens_loc.ToArray();

                        List<SensorPoligonPoint> L_Sens_Point = new List<SensorPoligonPoint>();
                        IList<NH_SensorPoligon> s_l_poli = session.QueryOver<NH_SensorPoligon>().Fetch(x => x.SensorID).Eager.Where(x => x.SensorID == it_out.Id.Value).List();
                        foreach (NH_SensorPoligon mpt in s_l_poli)
                        {
                            SensorPoligonPoint s_l = new SensorPoligonPoint();
                            s_l.Lat = mpt.Lat;
                            s_l.Lon = mpt.Lon;
                            L_Sens_Point.Add(s_l);
                        }
                        it_out.Poligon = L_Sens_Point.ToArray();
                        val.Add(it_out);
                    }
                }
                catch (Exception ex)
                {
                    tr_1.Rollback();
                    BusManager._logger.Error(Contexts.ThisComponent, Categories.LoadSensor, Events.LoadSensor, ex.Message, null);
                    tr_1.Dispose();
                }
            }
            return val;
        }
        /// <summary>
        /// Create sensor in DB
        /// </summary>
        /// <param name="sens"></param>
        /// <returns></returns>
        public bool CreateNewObjectSensor(Sensor sens)
        {
            try {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    ITransaction tr = session.BeginTransaction();
                    if (sens != null) {
                        NH_Sensor se = new NH_Sensor();
                        se.Administration = sens.Administration;
                        se.AGL = sens.AGL;
                        se.Azimuth = sens.Azimuth;
                        se.BiuseDate = sens.BiuseDate;
                        se.CreatedBy = sens.CreatedBy;
                        se.CustData1 = sens.CustData1;
                        se.CustNbr1 = sens.CustNbr1;
                        se.CustTxt1 = sens.CustTxt1;
                        se.DateCreated = sens.DateCreated;
                        se.Elevation = sens.Elevation;
                        se.EouseDate = sens.EouseDate;
                        se.IdSysARGUS = sens.IdSysARGUS;
                        se.Name = sens.Name;
                        se.NetworkId = sens.NetworkId;
                        se.OpDays = sens.OpDays;
                        se.OpHHFr = sens.OpHHFr;
                        se.OpHHTo = sens.OpHHTo;
                        se.Remark = sens.Remark;
                        se.RxLoss = sens.RxLoss;
                        se.Status = AllStatusSensor.N.ToString();
                        se.StepMeasTime = sens.StepMeasTime;
                        se.TypeSensor = sens.TypeSensor;
                        //se.SensorIdentifier_Id = sens.Id.Value;
                        se = (NH_Sensor)SetNullValue(se);
                        object ID_sensor = session.Save(se);
                        Console.WriteLine("Successfully saved into table - NH_Sensor");
                        if (sens.Antenna != null) {
                            NH_SensorAntenna NH_sensor_ant = new NH_SensorAntenna();
                            NH_sensor_ant.AddLoss = sens.Antenna.AddLoss;
                            NH_sensor_ant.AntClass = sens.Antenna.AntClass;
                            NH_sensor_ant.AntDir = sens.Antenna.AntDir.ToString();
                            NH_sensor_ant.Category = sens.Antenna.Category;
                            NH_sensor_ant.Code = sens.Antenna.Code;
                            NH_sensor_ant.CustData1 = sens.Antenna.CustData1;
                            NH_sensor_ant.CustNbr1 = sens.Antenna.CustNbr1;
                            NH_sensor_ant.CustTxt1 = sens.Antenna.CustTxt1;
                            NH_sensor_ant.GainMax = sens.Antenna.GainMax;
                            NH_sensor_ant.GainType = sens.Antenna.GainType;
                            NH_sensor_ant.HBeamwidth = sens.Antenna.HBeamwidth;
                            NH_sensor_ant.LowerFreq = sens.Antenna.LowerFreq;
                            NH_sensor_ant.Manufacturer = sens.Antenna.Manufacturer;
                            NH_sensor_ant.Name = sens.Antenna.Name;
                            NH_sensor_ant.Polarization = sens.Antenna.Polarization.ToString();
                            NH_sensor_ant.Remark = sens.Antenna.Remark;
                            NH_sensor_ant.SlewAng = sens.Antenna.SlewAng;
                            NH_sensor_ant.TechId = sens.Antenna.TechId;
                            NH_sensor_ant.UpperFreq = sens.Antenna.UpperFreq;
                            NH_sensor_ant.UseType = sens.Antenna.UseType;
                            NH_sensor_ant.VBeamwidth = sens.Antenna.VBeamwidth;
                            NH_sensor_ant.XPD = sens.Antenna.XPD;
                            NH_sensor_ant.SensorID = (int)ID_sensor;
                            NH_sensor_ant = (NH_SensorAntenna)SetNullValue(NH_sensor_ant);
                            object ID_sensor_ant = session.Save(NH_sensor_ant);
                            Console.WriteLine("Successfully saved into table - NH_SensorAntenna");
                            if (sens.Antenna.AntennaPatterns != null) {
                                foreach (AntennaPattern patt_a in sens.Antenna.AntennaPatterns)  {
                                    NH_AntennaPattern NH_ant_patt = new NH_AntennaPattern();
                                    NH_ant_patt.DiagA = patt_a.DiagA;
                                    NH_ant_patt.DiagH = patt_a.DiagH;
                                    NH_ant_patt.DiagV = patt_a.DiagV;
                                    NH_ant_patt.Freq = patt_a.Freq;
                                    NH_ant_patt.Gain = patt_a.Gain;
                                    NH_ant_patt.SensorAntenna_ID = (int)ID_sensor_ant;
                                    NH_ant_patt = (NH_AntennaPattern)SetNullValue(NH_ant_patt);
                                    session.Save(NH_ant_patt);
                                    Console.WriteLine("Successfully saved into table - NH_AntennaPattern");
                                }
                            }
                        }
                        if (sens.Equipment != null) {
                            NH_SensorEquip NH_sens_eqp = new NH_SensorEquip();
                            NH_sens_eqp.Category = sens.Equipment.Category;
                            NH_sens_eqp.Code = sens.Equipment.Code;
                            NH_sens_eqp.CustData1 = sens.Equipment.CustData1;
                            NH_sens_eqp.CustNbr1 = sens.Equipment.CustNbr1;
                            NH_sens_eqp.CustTxt1 = sens.Equipment.CustTxt1;
                            NH_sens_eqp.EquipClass = sens.Equipment.EquipClass;
                            NH_sens_eqp.Family = sens.Equipment.Family;
                            NH_sens_eqp.FFTPointMax = sens.Equipment.FFTPointMax;
                            NH_sens_eqp.LowerFreq = sens.Equipment.LowerFreq;
                            NH_sens_eqp.Manufacturer = sens.Equipment.Manufacturer;
                            NH_sens_eqp.Mobility = sens.Equipment.Mobility;
                            NH_sens_eqp.Name = sens.Equipment.Name;
                            NH_sens_eqp.OperationMode = sens.Equipment.OperationMode;
                            NH_sens_eqp.RBWMax = sens.Equipment.RBWMax;
                            NH_sens_eqp.RBWMin = sens.Equipment.RBWMin;
                            NH_sens_eqp.RefLeveldBm = sens.Equipment.RefLeveldBm;
                            NH_sens_eqp.Remark = sens.Equipment.Remark;
                            NH_sens_eqp.TechId = sens.Equipment.TechId;
                            NH_sens_eqp.TuningStep = sens.Equipment.TuningStep;
                            NH_sens_eqp.Type = sens.Equipment.Type;
                            NH_sens_eqp.UpperFreq = sens.Equipment.UpperFreq;
                            NH_sens_eqp.UseType = sens.Equipment.UseType;
                            NH_sens_eqp.VBWMax = sens.Equipment.VBWMax;
                            NH_sens_eqp.VBWMin = sens.Equipment.VBWMin;
                            NH_sens_eqp.Version = sens.Equipment.Version;
                            NH_sens_eqp.SensorID = (int)ID_sensor;
                            NH_sens_eqp = (NH_SensorEquip)SetNullValue(NH_sens_eqp);
                            object ID_NH_sens_eqp = session.Save(NH_sens_eqp);
                            Console.WriteLine("Successfully saved into table - NH_SensorEquip");
                            if (sens.Equipment.SensorEquipSensitivities != null) {
                                foreach (SensorEquipSensitivity sens_eqp_s in sens.Equipment.SensorEquipSensitivities) {
                                    NH_SensorEquipSensitivity NH_SensorEquipSensitivity_ = new NH_SensorEquipSensitivity();
                                    NH_SensorEquipSensitivity_.AddLoss = sens_eqp_s.AddLoss;
                                    NH_SensorEquipSensitivity_.Freq = sens_eqp_s.Freq;
                                    NH_SensorEquipSensitivity_.FreqStability = sens_eqp_s.FreqStability;
                                    NH_SensorEquipSensitivity_.KTBF = sens_eqp_s.KTBF;
                                    NH_SensorEquipSensitivity_.NoiseF = sens_eqp_s.NoiseF;
                                    NH_SensorEquipSensitivity_.SensorEquip_ID = (int)ID_NH_sens_eqp;
                                    NH_SensorEquipSensitivity_ = (NH_SensorEquipSensitivity)SetNullValue(NH_SensorEquipSensitivity_);
                                    session.Save(NH_SensorEquipSensitivity_);
                                    Console.WriteLine("Successfully saved into table - NH_SensorEquipSensitivity");
                                }
                            }
                            if (sens.Locations != null) {
                                foreach (SensorLocation sens_Locations in sens.Locations) {
                                    NH_SensorLocation NH_sens_location = new NH_SensorLocation();
                                    NH_sens_location.ASL = sens_Locations.ASL;
                                    NH_sens_location.DataCreated = sens_Locations.DataCreated;
                                    NH_sens_location.DataFrom = sens_Locations.DataFrom;
                                    NH_sens_location.DataTo = sens_Locations.DataTo;
                                    NH_sens_location.Lat = sens_Locations.Lat;
                                    NH_sens_location.Lon = sens_Locations.Lon;
                                    NH_sens_location.Status = sens_Locations.Status;
                                    NH_sens_location.SensorID = (int)ID_sensor;
                                    NH_sens_location = (NH_SensorLocation)SetNullValue(NH_sens_location);
                                    session.Save(NH_sens_location);
                                    Console.WriteLine("Successfully saved into table - NH_SensorLocation");
                                }
                            }

                            if (sens.Poligon != null)
                            {
                                foreach (SensorPoligonPoint sens_PoligonPoint in sens.Poligon)
                                {
                                    NH_SensorPoligon NH_sens_location = new NH_SensorPoligon();
                                    NH_sens_location.Lat = sens_PoligonPoint.Lat;
                                    NH_sens_location.Lon = sens_PoligonPoint.Lon;
                                    NH_sens_location.SensorID = (int)ID_sensor;
                                    NH_sens_location = (NH_SensorPoligon)SetNullValue(NH_sens_location);
                                    session.Save(NH_sens_location);
                                    Console.WriteLine("Successfully saved into table - NH_SensorLocation");
                                }
                            }
                        }
                    }
                    tr.Commit();
                    session.Flush();
                    tr.Dispose();
                }
                LoadObjectSensor();
            }
            catch (Exception ex) {
                BusManager._logger.Error(Contexts.ThisComponent, Categories.CreateNewObjectSensor, Events.CreateNewObjectSensor, ex.Message, null);
            }
            return true;
        }
        /// <summary>
        /// Set null Value
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static object SetNullValue(object v)
        {
            if (v != null) {
                Type myType = v.GetType();
                foreach (System.Reflection.PropertyInfo propertyInfo in myType.GetProperties()) {
                    string name = propertyInfo.Name;
                    object value = propertyInfo.GetValue(v, null);
                    if (value is int) {
                        if ((int)value == Constants.NullI)
                            propertyInfo.SetValue(v, new Nullable<int>());
                    }
                    else if (value is DateTime) {
                        if (((DateTime)value) == Constants.NullT)
                            propertyInfo.SetValue(v, new Nullable<DateTime>());
                    }
                    else if (value is double) {
                        if (((double)value) == Constants.NullD)
                            propertyInfo.SetValue(v, new Nullable<double>());
                    }
                }
            }
            return v;
        }
    }
}
