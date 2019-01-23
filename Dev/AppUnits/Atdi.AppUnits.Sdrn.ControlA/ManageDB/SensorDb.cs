using System;
using System.Collections.Generic;
using Atdi.AppServer.Contracts.Sdrns;
using NHibernate;
using Atdi.AppUnits.Sdrn.ControlA.Bus;
using System.Xml.Serialization;

namespace Atdi.AppUnits.Sdrn.ControlA.ManageDB
{
    public enum AllStatusSensor
    {
        N,
        A,
        Z,
        E,
        C,
        F,
        P,
        O
    }
    public enum AllStatusLocation
    {
        A,
        Z
    }
    public class SensorDb
    {

        public Sensor GetSensorFromDefaultConfigFile()
        {
            Sensor sensorDeserialize = null;
            XmlSerializer sersens = new XmlSerializer(typeof(Sensor));
            var readersenss = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\sensor.xml");
            object obj = sersens.Deserialize(readersenss);
            return sensorDeserialize = obj as Sensor;
        }

        /// <summary>
        /// Update status sensor
        /// </summary>
        public void UpdateStatus()
        {
            var seCurr = GetCurrentSensor();
            if (seCurr != null)
            {
                UpdateSensorStatus(seCurr, AllStatusSensor.A);
            }
        }
        /// <summary>
        /// Set old Location.Status='Z'
        /// </summary>
        /// <param name="sens_loc"></param>
        public void CloseOldSensorLocation(NH_SensorLocation sensLoc)
        {
            try
            {
                if (sensLoc != null)
                {
                    var cl = new ClassObjectsSensorOnSDR();
                    if (Domain.sessionFactory == null) Domain.Init();
                    ISession session = Domain.CurrentSession;
                    ITransaction transaction = session.BeginTransaction();
                    sensLoc.Status = AllStatusLocation.Z.ToString();
                    sensLoc.DataTo = DateTime.Now;
                    cl.UpdateObject<NH_SensorLocation>(sensLoc.ID, sensLoc);
                    transaction.Commit();
                    session.Flush();
                    transaction.Dispose();
                }
                LoadObjectSensor();
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.UpdateSensorStatus, Events.UpdateSensorStatus, ex.Message, null);
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
        public void AddNewLocations(int Idsensor, double Lon, double Lat, double ASL, DateTime DateFrom, DateTime DateTo, AllStatusLocation locstatus)
        {
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                ITransaction transaction = session.BeginTransaction();
                var nHSensLocation = new NH_SensorLocation();
                nHSensLocation.ASL = ASL;
                nHSensLocation.DataCreated = DateTime.Now;
                nHSensLocation.DataFrom = DateFrom;
                nHSensLocation.Lat = Lat;
                nHSensLocation.Lon = Lon;
                nHSensLocation.Status = locstatus.ToString();
                nHSensLocation.SensorID = Idsensor;
                nHSensLocation = (NH_SensorLocation)MeasTaskSDRExtend.SetNullValue(nHSensLocation);
                session.Save(nHSensLocation);
                transaction.Commit();
                session.Flush();
                transaction.Dispose();
                Launcher._logger.Info(Contexts.ThisComponent, Categories.UpdateSensorStatus, Events.SuccessfullySavedIntoTablenHSensLocation);
                LoadObjectSensor();
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.UpdateSensorStatus, Events.UpdateSensorStatus, ex.Message, null);
            }
        }

        /// <summary>
        /// Update status sensor
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="newStatus"></param>
        public void UpdateSensorStatus(Sensor sensor, AllStatusSensor newStatus)
        {
            try
            {
                var nghs = LoadSensorFromDB(sensor.Name, sensor.Equipment.TechId);
                if (nghs != null)
                {
                    var cl = new ClassObjectsSensorOnSDR();
                    if (Domain.sessionFactory == null) Domain.Init();
                    ISession session = Domain.CurrentSession;
                    ITransaction transaction = session.BeginTransaction();
                    nghs.Status = newStatus.ToString();
                    nghs.Administration = sensor.Administration;
                    nghs.AGL = sensor.AGL;
                    nghs.Azimuth = sensor.Azimuth;
                    nghs.BiuseDate = sensor.BiuseDate;
                    nghs.CreatedBy = sensor.CreatedBy;
                    nghs.CustData1 = sensor.CustData1;
                    nghs.CustNbr1 = sensor.CustNbr1;
                    nghs.CustTxt1 = sensor.CustTxt1;
                    nghs.DateCreated = sensor.DateCreated;
                    nghs.Elevation = sensor.Elevation;
                    nghs.EouseDate = sensor.EouseDate;
                    nghs.IdSysARGUS = sensor.IdSysARGUS;
                    nghs.Name = sensor.Name;
                    nghs.NetworkId = sensor.NetworkId;
                    nghs.OpDays = sensor.OpDays;
                    nghs.OpHHFr = sensor.OpHHFr;
                    nghs.OpHHTo = sensor.OpHHTo;
                    nghs.Remark = sensor.Remark;
                    nghs.RxLoss = sensor.RxLoss;
                    nghs.StepMeasTime = sensor.StepMeasTime;
                    nghs.TypeSensor = sensor.TypeSensor;
                    cl.UpdateObject<NH_Sensor>(nghs.ID, nghs);
                    transaction.Commit();
                    session.Flush();
                    transaction.Dispose();
                }
                LoadObjectSensor();
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.UpdateSensorStatus, Events.UpdateSensorStatus, ex.Message, null);
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
            var sens = new NH_Sensor();
            if (Domain.sessionFactory == null) Domain.Init();
            IList<NH_Sensor> slSensor = null;
            ISession session = Domain.CurrentSession;
            ITransaction transaction = session.BeginTransaction();
            try
            {
                slSensor = session.QueryOver<NH_Sensor>().Where(x => x.Name == Name).List();
                foreach (var item in slSensor)
                {
                    var slEquip = session.QueryOver<NH_SensorEquip>().Where(x => x.TechId == TechId).List();
                    if (slEquip.Count > 0)
                    {
                        sens = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Launcher._logger.Error(Contexts.ThisComponent, Categories.LoadSensor, Events.LoadSensor, ex.Message, null);
            }
            transaction.Dispose();
            return sens;
        }

        /// <summary>
        /// Load list objects NH_SensorLocation from DB
        /// </summary>
        /// <param name="Id_sensor"></param>
        /// <returns></returns>
        public List<NH_SensorLocation> LoadSensorLocationsFromDB(int Id_sensor)
        {
            var slLoc = new List<NH_SensorLocation>();
            if (Domain.sessionFactory == null) Domain.Init();
            IList<NH_Sensor> slSensor = null;
            ISession session = Domain.CurrentSession;
            ITransaction transaction = session.BeginTransaction();
            try
            {
                List<int> Xs = new List<int>();
                slSensor = session.QueryOver<NH_Sensor>().Where(x => x.ID == Id_sensor).List();
                foreach (var item in slSensor)
                {
                    var slEquip = session.QueryOver<NH_SensorEquip>().Where(x => x.SensorID == Id_sensor).List();
                    if (slEquip.Count > 0)
                    {
                        var sllocation = session.QueryOver<NH_SensorLocation>().Where(x => x.SensorID == item.ID && x.Status != AllStatusLocation.Z.ToString()).List();
                        if (sllocation.Count > 0)
                        {
                            foreach (var fd in sllocation)
                            {
                                slLoc.Add(fd);
                                Xs.Add(fd.ID);
                            }
                        }
                        Xs.Sort();
                        Xs.Reverse();
                        if (Xs.Count > 0) slLoc.RemoveAll(t => t.ID == Xs[0]);
                    }
                }
            }
            catch (Exception) { transaction.Rollback(); }
            transaction.Dispose();
            return slLoc;
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
            var val = new List<Sensor>();
            if (Domain.sessionFactory == null) Domain.Init();
            IList<NH_Sensor> slSensor = null;
            ISession session = Domain.CurrentSession;
            {
                ITransaction transaction = session.BeginTransaction();
                try
                {
                    slSensor = session.QueryOver<NH_Sensor>().Where(x => x.ID > 0).List();
                    foreach (var item in slSensor)
                    {
                        var itOut = new Sensor();
                        itOut.Id = new SensorIdentifier();
                        itOut.Administration = item.Administration;
                        itOut.AGL = item.AGL;
                        itOut.Azimuth = item.Azimuth;
                        itOut.BiuseDate = item.BiuseDate;
                        itOut.CreatedBy = item.CreatedBy;
                        itOut.CustData1 = item.CustData1;
                        itOut.CustNbr1 = item.CustNbr1;
                        itOut.CustTxt1 = item.CustTxt1;
                        itOut.DateCreated = item.DateCreated;
                        itOut.Elevation = item.Elevation;
                        itOut.EouseDate = item.EouseDate;
                        itOut.IdSysARGUS = item.IdSysARGUS;
                        itOut.Id.Value = item.ID;
                        itOut.Name = item.Name;
                        itOut.NetworkId = item.NetworkId;
                        itOut.OpDays = item.OpDays;
                        itOut.OpHHFr = item.OpHHFr;
                        itOut.OpHHTo = item.OpHHTo;
                        itOut.Remark = item.Remark;
                        itOut.RxLoss = item.RxLoss;
                        itOut.Status = item.Status;
                        itOut.StepMeasTime = item.StepMeasTime;
                        itOut.TypeSensor = item.TypeSensor;
                        itOut.Antenna = new SensorAntenna();
                        var slAntenna = session.QueryOver<NH_SensorAntenna>().Where(x => x.SensorID == itOut.Id.Value).List();
                        foreach (var mpt in slAntenna)
                        {
                            var lAntPatt = new List<AntennaPattern>();
                            var slAntennaMpt = session.QueryOver<NH_AntennaPattern>().Where(x => x.SensorAntenna_ID == mpt.ID).List();
                            foreach (var itr in slAntennaMpt)
                            {
                                var ptu = new AntennaPattern();
                                ptu.DiagA = itr.DiagA;
                                ptu.DiagH = itr.DiagH;
                                ptu.DiagV = itr.DiagV;
                                ptu.Freq = itr.Freq;
                                ptu.Gain = itr.Gain;
                                lAntPatt.Add(ptu);
                            }
                            itOut.Antenna.AntennaPatterns = lAntPatt.ToArray();
                        }

                        itOut.Equipment = new SensorEquip();
                        var slRquip = session.QueryOver<NH_SensorEquip>().Where(x => x.SensorID == itOut.Id.Value).List();
                        foreach (var mpt in slRquip)
                        {
                            itOut.Equipment.Category = mpt.Category;
                            itOut.Equipment.Code = mpt.Code;
                            itOut.Equipment.CustData1 = mpt.CustData1;
                            itOut.Equipment.CustNbr1 = mpt.CustNbr1;
                            itOut.Equipment.CustTxt1 = mpt.CustTxt1;
                            itOut.Equipment.EquipClass = mpt.EquipClass;
                            itOut.Equipment.Family = mpt.Family;
                            itOut.Equipment.FFTPointMax = mpt.FFTPointMax;
                            itOut.Equipment.LowerFreq = mpt.LowerFreq;
                            itOut.Equipment.Manufacturer = mpt.Manufacturer;
                            itOut.Equipment.Mobility = mpt.Mobility;
                            itOut.Equipment.Name = mpt.Name;
                            itOut.Equipment.OperationMode = mpt.OperationMode;
                            itOut.Equipment.RBWMax = mpt.RBWMax;
                            itOut.Equipment.RBWMin = mpt.RBWMin;
                            itOut.Equipment.RefLeveldBm = mpt.RefLeveldBm;
                            itOut.Equipment.Remark = mpt.Remark;
                            itOut.Equipment.TechId = mpt.TechId;
                            itOut.Equipment.TuningStep = mpt.TuningStep;
                            itOut.Equipment.Type = mpt.Type;
                            itOut.Equipment.UpperFreq = mpt.UpperFreq;
                            itOut.Equipment.UseType = mpt.UseType;
                            itOut.Equipment.VBWMax = mpt.VBWMax;
                            itOut.Equipment.VBWMin = mpt.VBWMin;
                            itOut.Equipment.Version = mpt.Version;

                            var lSens = new List<SensorEquipSensitivity>();
                            var slSenspt = session.QueryOver<NH_SensorEquipSensitivity>().Where(x => x.SensorEquip_ID == mpt.ID).List();
                            foreach (var itr in slSenspt)
                            {
                                var ptu = new SensorEquipSensitivity();
                                ptu.AddLoss = itr.AddLoss;
                                ptu.Freq = itr.Freq.GetValueOrDefault();
                                ptu.FreqStability = itr.FreqStability;
                                ptu.KTBF = itr.KTBF;
                                ptu.NoiseF = itr.NoiseF;
                                lSens.Add(ptu);
                            }
                            itOut.Equipment.SensorEquipSensitivities = lSens.ToArray();
                        }

                        var lSensLoc = new List<SensorLocation>();
                        var slLocation = session.QueryOver<NH_SensorLocation>().Where(x => x.SensorID == itOut.Id.Value && x.Status != AllStatusLocation.Z.ToString()).List();
                        foreach (var mpt in slLocation)
                        {
                            var s_l = new SensorLocation();
                            s_l.ASL = mpt.ASL;
                            s_l.DataCreated = mpt.DataCreated;
                            s_l.DataFrom = mpt.DataFrom;
                            s_l.DataTo = mpt.DataTo;
                            s_l.Lat = mpt.Lat;
                            s_l.Lon = mpt.Lon;
                            s_l.Status = mpt.Status;
                            lSensLoc.Add(s_l);
                        }
                        itOut.Locations = lSensLoc.ToArray();

                        var lSensPoint = new List<SensorPoligonPoint>();
                        var slPoli = session.QueryOver<NH_SensorPoligon>().Where(x => x.SensorID == itOut.Id.Value).List();
                        foreach (var mpt in slPoli)
                        {
                            var sl = new SensorPoligonPoint();
                            sl.Lat = mpt.Lat;
                            sl.Lon = mpt.Lon;
                            lSensPoint.Add(sl);
                        }
                        itOut.Poligon = lSensPoint.ToArray();
                        val.Add(itOut);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Launcher._logger.Error(Contexts.ThisComponent, Categories.LoadSensor, Events.LoadSensor, ex.Message, null);
                    transaction.Dispose();
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
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    ITransaction transaction = session.BeginTransaction();
                    if (sens != null)
                    {
                        var se = new NH_Sensor();
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
                        se = (NH_Sensor)MeasTaskSDRExtend.SetNullValue(se);
                        object idSensor = session.Save(se);
                        Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewObjectSensor, Events.SuccessfullySavedIntoTableNH_Sensor);

                        if (sens.Antenna != null)
                        {
                            var nHSensorAnt = new NH_SensorAntenna();
                            nHSensorAnt.AddLoss = sens.Antenna.AddLoss;
                            nHSensorAnt.AntClass = sens.Antenna.AntClass;
                            nHSensorAnt.AntDir = sens.Antenna.AntDir.ToString();
                            nHSensorAnt.Category = sens.Antenna.Category;
                            nHSensorAnt.Code = sens.Antenna.Code;
                            nHSensorAnt.CustData1 = sens.Antenna.CustData1;
                            nHSensorAnt.CustNbr1 = sens.Antenna.CustNbr1;
                            nHSensorAnt.CustTxt1 = sens.Antenna.CustTxt1;
                            nHSensorAnt.GainMax = sens.Antenna.GainMax;
                            nHSensorAnt.GainType = sens.Antenna.GainType;
                            nHSensorAnt.HBeamwidth = sens.Antenna.HBeamwidth;
                            nHSensorAnt.LowerFreq = sens.Antenna.LowerFreq;
                            nHSensorAnt.Manufacturer = sens.Antenna.Manufacturer;
                            nHSensorAnt.Name = sens.Antenna.Name;
                            nHSensorAnt.Polarization = sens.Antenna.Polarization.ToString();
                            nHSensorAnt.Remark = sens.Antenna.Remark;
                            nHSensorAnt.SlewAng = sens.Antenna.SlewAng;
                            nHSensorAnt.TechId = sens.Antenna.TechId;
                            nHSensorAnt.UpperFreq = sens.Antenna.UpperFreq;
                            nHSensorAnt.UseType = sens.Antenna.UseType;
                            nHSensorAnt.VBeamwidth = sens.Antenna.VBeamwidth;
                            nHSensorAnt.XPD = sens.Antenna.XPD;
                            nHSensorAnt.SensorID = (int)idSensor;
                            nHSensorAnt = (NH_SensorAntenna)MeasTaskSDRExtend.SetNullValue(nHSensorAnt);
                            object idSensorAnt = session.Save(nHSensorAnt);
                            Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewObjectSensor, Events.SuccessfullySavedIntoTableNH_SensorAntenna);

                            if (sens.Antenna.AntennaPatterns != null)
                            {
                                foreach (AntennaPattern pattA in sens.Antenna.AntennaPatterns)
                                {
                                    NH_AntennaPattern nHAntPatt = new NH_AntennaPattern();
                                    nHAntPatt.DiagA = pattA.DiagA;
                                    nHAntPatt.DiagH = pattA.DiagH;
                                    nHAntPatt.DiagV = pattA.DiagV;
                                    nHAntPatt.Freq = pattA.Freq;
                                    nHAntPatt.Gain = pattA.Gain;
                                    nHAntPatt.SensorAntenna_ID = (int)idSensorAnt;
                                    nHAntPatt = (NH_AntennaPattern)MeasTaskSDRExtend.SetNullValue(nHAntPatt);
                                    session.Save(nHAntPatt);
                                    Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewObjectSensor, Events.SuccessfullySavedIntoTableNH_AntennaPattern);
                                }
                            }
                        }
                        if (sens.Equipment != null)
                        {
                            var nhSensEqp = new NH_SensorEquip();
                            nhSensEqp.Category = sens.Equipment.Category;
                            nhSensEqp.Code = sens.Equipment.Code;
                            nhSensEqp.CustData1 = sens.Equipment.CustData1;
                            nhSensEqp.CustNbr1 = sens.Equipment.CustNbr1;
                            nhSensEqp.CustTxt1 = sens.Equipment.CustTxt1;
                            nhSensEqp.EquipClass = sens.Equipment.EquipClass;
                            nhSensEqp.Family = sens.Equipment.Family;
                            nhSensEqp.FFTPointMax = sens.Equipment.FFTPointMax;
                            nhSensEqp.LowerFreq = sens.Equipment.LowerFreq;
                            nhSensEqp.Manufacturer = sens.Equipment.Manufacturer;
                            nhSensEqp.Mobility = sens.Equipment.Mobility;
                            nhSensEqp.Name = sens.Equipment.Name;
                            nhSensEqp.OperationMode = sens.Equipment.OperationMode;
                            nhSensEqp.RBWMax = sens.Equipment.RBWMax;
                            nhSensEqp.RBWMin = sens.Equipment.RBWMin;
                            nhSensEqp.RefLeveldBm = sens.Equipment.RefLeveldBm;
                            nhSensEqp.Remark = sens.Equipment.Remark;
                            nhSensEqp.TechId = sens.Equipment.TechId;
                            nhSensEqp.TuningStep = sens.Equipment.TuningStep;
                            nhSensEqp.Type = sens.Equipment.Type;
                            nhSensEqp.UpperFreq = sens.Equipment.UpperFreq;
                            nhSensEqp.UseType = sens.Equipment.UseType;
                            nhSensEqp.VBWMax = sens.Equipment.VBWMax;
                            nhSensEqp.VBWMin = sens.Equipment.VBWMin;
                            nhSensEqp.Version = sens.Equipment.Version;
                            nhSensEqp.SensorID = (int)idSensor;
                            nhSensEqp = (NH_SensorEquip)MeasTaskSDRExtend.SetNullValue(nhSensEqp);
                            object idNHSensEqp = session.Save(nhSensEqp);
                            Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewObjectSensor, Events.SuccessfullySavedIntoTableNH_SensorEquip);

                            if (sens.Equipment.SensorEquipSensitivities != null)
                            {
                                foreach (var sensEqps in sens.Equipment.SensorEquipSensitivities)
                                {
                                    var nHSensorEquipSensitivity = new NH_SensorEquipSensitivity();
                                    nHSensorEquipSensitivity.AddLoss = sensEqps.AddLoss;
                                    nHSensorEquipSensitivity.Freq = sensEqps.Freq;
                                    nHSensorEquipSensitivity.FreqStability = sensEqps.FreqStability;
                                    nHSensorEquipSensitivity.KTBF = sensEqps.KTBF;
                                    nHSensorEquipSensitivity.NoiseF = sensEqps.NoiseF;
                                    nHSensorEquipSensitivity.SensorEquip_ID = (int)idNHSensEqp;
                                    nHSensorEquipSensitivity = (NH_SensorEquipSensitivity)MeasTaskSDRExtend.SetNullValue(nHSensorEquipSensitivity);
                                    session.Save(nHSensorEquipSensitivity);
                                    Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewObjectSensor, Events.SuccessfullySavedIntoTablenHSensorEquipSensitivity);
                                }
                            }
                            if (sens.Locations != null)
                            {
                                foreach (var sensLocations in sens.Locations)
                                {
                                    var nHSensLocation = new NH_SensorLocation();
                                    nHSensLocation.ASL = sensLocations.ASL;
                                    nHSensLocation.DataCreated = sensLocations.DataCreated;
                                    nHSensLocation.DataFrom = sensLocations.DataFrom;
                                    nHSensLocation.DataTo = sensLocations.DataTo;
                                    nHSensLocation.Lat = sensLocations.Lat;
                                    nHSensLocation.Lon = sensLocations.Lon;
                                    nHSensLocation.Status = sensLocations.Status;
                                    nHSensLocation.SensorID = (int)idSensor;
                                    nHSensLocation = (NH_SensorLocation)MeasTaskSDRExtend.SetNullValue(nHSensLocation);
                                    session.Save(nHSensLocation);
                                    Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewObjectSensor, Events.SuccessfullySavedIntoTablenHSensLocation);
                                }
                            }

                            if (sens.Poligon != null)
                            {
                                foreach (var sensPoligonPoint in sens.Poligon)
                                {
                                    var nHSensLocation = new NH_SensorPoligon();
                                    nHSensLocation.Lat = sensPoligonPoint.Lat;
                                    nHSensLocation.Lon = sensPoligonPoint.Lon;
                                    nHSensLocation.SensorID = (int)idSensor;
                                    nHSensLocation = (NH_SensorPoligon)MeasTaskSDRExtend.SetNullValue(nHSensLocation);
                                    session.Save(nHSensLocation);
                                    Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewObjectSensor, Events.SuccessfullySavedIntoTablenHSensLocation);
                                }
                            }
                        }
                    }
                    transaction.Commit();
                    session.Flush();
                    transaction.Dispose();
                }
                LoadObjectSensor();
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.CreateNewObjectSensor, Events.CreateNewObjectSensor, ex.Message, null);
            }
            return true;
        }
    }
}
