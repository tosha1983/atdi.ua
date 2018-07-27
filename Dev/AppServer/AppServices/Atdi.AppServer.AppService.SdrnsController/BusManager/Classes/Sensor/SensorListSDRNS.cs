using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using XMLLibrary;
using CoreICSM.Logs;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.SDNRS.AppServer.BusManager
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
    public class SensorListSDRNS : IDisposable
    {
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~SensorListSDRNS()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// формирует список ShortSensor для ответа на запроса клиента.
        /// </summary>
        /// <returns></returns>
        public List<ShortSensor> CreateShortSensorList()
        {
            List<ShortSensor> val = new List<ShortSensor>();
            List<Sensor> val_all_s = new List<Sensor>();
            {
                val_all_s = GlobalInit.SensorListSDRNS;
                if (val_all_s != null)
                {
                    foreach (Sensor sd in val_all_s.ToArray())
                    {
                        ShortSensor sh = new ShortSensor();
                        sh.Name = sd.Name;
                        sh.Status = sd.Status;
                        sh.Administration = sd.Administration;
                        if (sd.Antenna != null) {
                            sh.AntGainMax = sd.Antenna.GainMax;
                            sh.AntManufacturer = sd.Antenna.Manufacturer;
                            sh.AntName = sd.Antenna.Name;
                        }
                        sh.NetworkId = sd.NetworkId;
                        sh.RxLoss = sd.RxLoss;
                        if (sd.Equipment != null) {
                            sh.UpperFreq = sd.Equipment.UpperFreq;
                            sh.EquipName = sd.Equipment.Name;
                            sh.EquipManufacturer = sd.Equipment.Manufacturer;
                            sh.EquipCode = sd.Equipment.Code;
                            sh.LowerFreq = sd.Equipment.LowerFreq;
                        }
                        sh.Id = sd.Id;
                        sh.EouseDate = sd.EouseDate;
                        sh.DateCreated = sd.DateCreated;
                        sh.CreatedBy = sd.CreatedBy;
                        sh.BiuseDate = sd.BiuseDate;
                        
                        val.Add(sh);
                    }
                }
            }
            return val;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ShortSensor CreateShortSensorListBySensorId(int Id)
        {
            ShortSensor val = new ShortSensor();
            {
                Sensor sd = GlobalInit.SensorListSDRNS.Find(t=>t.Id.Value == Id);
                if (sd != null)
                {
                    ShortSensor sh = new ShortSensor();
                    sh.Name = sd.Name;
                    sh.Status = sd.Status;
                    sh.Administration = sd.Administration;
                    if (sd.Antenna != null)
                    {
                        sh.AntGainMax = sd.Antenna.GainMax;
                        sh.AntManufacturer = sd.Antenna.Manufacturer;
                        sh.AntName = sd.Antenna.Name;
                    }
                    sh.NetworkId = sd.NetworkId;
                    sh.RxLoss = sd.RxLoss;
                    if (sd.Equipment != null)
                    {
                        sh.UpperFreq = sd.Equipment.UpperFreq;
                        sh.EquipName = sd.Equipment.Name;
                        sh.EquipManufacturer = sd.Equipment.Manufacturer;
                        sh.EquipCode = sd.Equipment.Code;
                        sh.LowerFreq = sd.Equipment.LowerFreq;
                    }
                    sh.Id = sd.Id;
                    sh.EouseDate = sd.EouseDate;
                    sh.DateCreated = sd.DateCreated;
                    sh.CreatedBy = sd.CreatedBy;
                    sh.BiuseDate = sd.BiuseDate;
                    val =sh;
                }
            }
            return val;
        }
        /// <summary>
        /// проверка активности сенсоров
        /// </summary>
        public void CheckActivitySensor()
        {
            try
            {
                CoreICSM.Logs.CLogs.WriteInfo(ELogsWhat.Unknown, "CheckActivitySensor ");
                ClassDBGetSensor gsd = new ClassDBGetSensor();
                BusManager<Sensor> busManager = new BusManager<Sensor>();
                ClassDBGetSensor DB = new ClassDBGetSensor();
                    List<Sensor> L_S = DB.LoadObjectAllSensor();
                    DB.Dispose();
                    if (L_S != null) {
                        if (L_S.Count > 0) {
                            foreach (Sensor message in L_S.ToArray())
                                busManager.SendDataObject(message, GlobalInit.Template_Event_CheckActivitySensor_Req + message.Name + message.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTemp.ToString());
                        }
                    }
                    {
                        foreach (Sensor message in GlobalInit.SensorListSDRNS.ToArray()) {
                            if (GlobalInit.Lst_timers.Find(t => t.se.Name == message.Name && t.se.Equipment.TechId == message.Equipment.TechId) == null) {
                                Mdx RW = new Mdx(message);
                                GlobalInit.Lst_timers.Add(RW);
                            }
                        }
                        foreach (Mdx se in GlobalInit.Lst_timers.ToArray()) {
                                if (ClassStaticBus.bus.Advanced.IsConnected)
                                {
                                    uint MessCount = busManager.GetMessageCount(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
                                    for (int i = 0; i < MessCount; i++) 
                                    {
                                    var message_x = busManager.GetDataObject(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
                                    if (message_x != null)
                                    {
                                    if ((message_x as Sensor).Name == se.se.Name && (message_x as Sensor).Equipment.TechId == se.se.Equipment.TechId)
                                    {
                                        se.Cnt_sensor_New++; se.Cnt_timer = 0; se.Cnt_all_time = 0;
                                        Sensor f = GlobalInit.SensorListSDRNS.Find(t => t.Name == se.se.Name && t.Equipment.TechId == se.se.Equipment.TechId);
                                        if (f != null)
                                        {
                                            se.Cnt_sensor_New++; f.Status = "A"; se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_Old = 0; se.BZ.Close(); se.BZ.Start(); gsd.UpdateStatusSensor(f);
                                        }
                                        else {
                                            se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_New = 0;
                                        }
                                    }
                                    }
                                    else break;
                                    }

                                /*
                                GlobalInit.Lds_Activity_Sensor_Receiver.Add(ClassStaticBus.bus.Receive(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId, x => x
                                    .Add<Sensor>(message_x =>
                                    {
                                        if (message_x.Name == se.se.Name && message_x.Equipment.TechId == se.se.Equipment.TechId) {
                                            se.Cnt_sensor_New++; se.Cnt_timer = 0; se.Cnt_all_time = 0;
                                            Sensor f = GlobalInit.SensorListSDRNS.Find(t => t.Name == se.se.Name && t.Equipment.TechId == se.se.Equipment.TechId);
                                            if (f != null) {
                                                se.Cnt_sensor_New++; f.Status = "A"; se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_Old = 0; se.BZ.Close(); se.BZ.Start(); gsd.UpdateStatusSensor(f);
                                            }
                                            else {
                                                se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_New = 0;
                                            }
                                        }
                                    })));
                                    */
                                 
                                }
                                else
                                {
                                    ClassStaticBus.bus.Dispose();
                                    GC.SuppressFinalize(ClassStaticBus.bus);
                                    ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                                    CoreICSM.Logs.CLogs.WriteInfo(CoreICSM.Logs.ELogsWhat.Unknown, "-> Bus dispose... ");
                                }
                            
                            Sensor fc = GlobalInit.SensorListSDRNS.Find(t => t.Name == se.se.Name && t.Equipment.TechId == se.se.Equipment.TechId);
                            if (fc != null) {
                                if ((se.Cnt_timer >= BaseXMLConfiguration.xml_conf._CheckActivitySensor) && (se.Cnt_all_time < BaseXMLConfiguration.xml_conf._MaxTimeNotActivateStatusSensor)) {
                                    if (ClassStaticBus.bus.Advanced.IsConnected) {
                                        busManager.SendDataObject(se.se, GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                        uint MessCount = busManager.GetMessageCount(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
                                        for (int i = 0; i < MessCount; i++)
                                        {
                                        var message_x = busManager.GetDataObject(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
                                        if (message_x != null)
                                        {
                                            if ((message_x as Sensor).Name == se.se.Name && (message_x as Sensor).Equipment.TechId == se.se.Equipment.TechId)
                                            {
                                                se.Cnt_sensor_New++;
                                                se.Cnt_timer = 0;
                                                se.Cnt_all_time = 0;
                                                se.Cnt_sensor_Old = 0;
                                                se.BZ.Close();
                                                se.BZ.Start();
                                            }
                                            else break;
                                        }
                                        }

                                            /*
                                            GlobalInit.Lds_Activity_Sensor_Receiver.Add(ClassStaticBus.bus.Receive(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId, x => x
                                                    .Add<Sensor>(message_x =>
                                                    {
                                                        if (message_x.Name == se.se.Name && message_x.Equipment.TechId == se.se.Equipment.TechId) {
                                                            se.Cnt_sensor_New++;
                                                            se.Cnt_timer = 0;
                                                            se.Cnt_all_time = 0;
                                                            se.Cnt_sensor_Old = 0;
                                                            se.BZ.Close();
                                                            se.BZ.Start();

                                                        }
                                                    })));
                                            */
                                        }
                                    else {
                                        ClassStaticBus.bus.Dispose();
                                    GC.SuppressFinalize(ClassStaticBus.bus);
                                    ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                                    CoreICSM.Logs.CLogs.WriteInfo(CoreICSM.Logs.ELogsWhat.Unknown, "-> Bus dispose... ");
                                }
                                    fc.Status = AllStatusSensor.F.ToString();
                                    se.Cnt_timer = 0;
                                    se.Cnt_sensor_New = 0;
                                    gsd.UpdateStatusSensor(fc);
                                }
                                else if ((se.Cnt_all_time >= BaseXMLConfiguration.xml_conf._MaxTimeNotActivateStatusSensor)) {
                                    bool isCheck = false;
                                    if (ClassStaticBus.bus.Advanced.IsConnected) {
                                        busManager.SendDataObject(se.se, GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                        uint MessCount = busManager.GetMessageCount(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
                                        for (int i = 0; i < MessCount; i++)
                                        {
                                        var message_x = busManager.GetDataObject(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
                                        if (message_x != null)
                                        {
                                            if ((message_x as Sensor).Name == se.se.Name && (message_x as Sensor).Equipment.TechId == se.se.Equipment.TechId)
                                            {
                                                se.Cnt_sensor_New++; se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_Old = 0; se.BZ.Close(); se.BZ.Start();
                                                isCheck = true;
                                            }
                                            else break;
                                        }
                                        }
                                        /*
                                        GlobalInit.Lds_Activity_Sensor_Receiver.Add(ClassStaticBus.bus.Receive(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId, x => x
                                            .Add<Sensor>(message_x =>
                                            {
                                                if (message_x.Name == se.se.Name && message_x.Equipment.TechId == se.se.Equipment.TechId) {
                                                    se.Cnt_sensor_New++; se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_Old = 0; se.BZ.Close(); se.BZ.Start();
                                                    isCheck = true;
                                                }
                                            })));
                                        */
                                    }
                                    else {
                                        ClassStaticBus.bus.Dispose();
                                        GC.SuppressFinalize(ClassStaticBus.bus);
                                        ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                                        CoreICSM.Logs.CLogs.WriteInfo(CoreICSM.Logs.ELogsWhat.Unknown, "-> Bus dispose... ");
                                    }
                                    if (!isCheck) busManager.DeleteQueue(GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId);
                                    if (!isCheck) se.Cnt_sensor_Old++;
                                    if ((se.Cnt_sensor_New == 0) || (se.Cnt_sensor_Old>2)) {
                                        //------------ ДАННЫЙ МЕХАНИЗМ ПОКА ДЕАКТИВИРУЕМ (ПО ПРИЧИНЕ ИСПОЛЬЗОВАНИЯ СЕРВЕРА СТОРОННЕЙ ОРГАНИЗАЦИЕЙ) ----------------------
                                        //------------ т.е. сенсор не должен удаляться из БД (он может быть активен или неактивен)
                                        //fc.Status = AllStatusSensor.Z.ToString(); se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_New = 0; se.Cnt_sensor_Old = 0;
                                        //gsd.UpdateStatusSensor(fc);
                                        //GlobalInit.SensorListSDRNS.RemoveAll(t => t.Name == fc.Name && t.Equipment.TechId == fc.Equipment.TechId);
                                        //se.BZ.Stop();
                                        //----------------------------------
                                        

                                        se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_New = 0; se.Cnt_sensor_Old = 0;
                                        se.BZ.Stop();
                                        busManager.DeleteQueue(GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId);
                                    }
                                }
                            }
                        }
                    }
                gsd.Dispose();  
            }
            catch (Exception ex)
            {
                Console.WriteLine("--> Error execute proc CheckActivitySensor " + ex.Message);
            }
        }
        /*
        /// <summary>
        /// Проверка активности заданного сенсора
        /// </summary>
        /// <param name="Id"></param>
        public bool CheckActivitySensor(int Id)
        {
            bool isActive = false;
            try
            {
                BusManager<Sensor> busManager = new BusManager<Sensor>();
                //lock (GlobalInit.SensorListSDRNS)
                {
                    Sensor FndSens = GlobalInit.SensorListSDRNS.Find(t => t.Id.Value == Id);
                    if (FndSens != null)  {
                        busManager.SendDataObject(FndSens, GlobalInit.Template_Event_CheckActivitySensor_Req + FndSens.Name + FndSens.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                        Mdx RW = new Mdx(FndSens);
                        //lock (GlobalInit.Lst_timers)
                        {
                            if (GlobalInit.Lst_timers.Find(t => t.se.Name == FndSens.Name && t.se.Equipment.TechId == FndSens.Equipment.TechId) == null)
                                GlobalInit.Lst_timers.Add(RW);

                            List<Mdx> FndMdx = GlobalInit.Lst_timers.FindAll(e => e.se.Name == FndSens.Name && e.se.Equipment.TechId == FndSens.Equipment.TechId);
                            if (FndMdx != null)
                            {
                                foreach (Mdx se in GlobalInit.Lst_timers.ToArray())
                                {
                                    if (ClassStaticBus.bus.IsConnected)
                                    {
                                        GlobalInit.Lds_Activity_Sensor_Receiver.Add(ClassStaticBus.bus.Receive(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId, x => x
                                        .Add<Sensor>(message_x =>
                                        {
                                            if (message_x.Name == se.se.Name && message_x.Equipment.TechId == se.se.Equipment.TechId)
                                            {
                                                se.Cnt_sensor_New++; se.Cnt_timer = 0; se.Cnt_all_time = 0;
                                                Sensor f = GlobalInit.SensorListSDRNS.Find(t => t.Name == se.se.Name && t.Equipment.TechId == se.se.Equipment.TechId);
                                                if (f != null)
                                                {
                                                    ClassDBGetSensor gsd = new ClassDBGetSensor();
                                                    se.Cnt_sensor_New++; f.Status = message_x.Status; se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_Old = 0; se.BZ.Start(); gsd.UpdateStatusSensor(f);
                                                }
                                                else {
                                                    se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_New = 0;
                                                }
                                            }
                                        })));
                                    }
                                    else
                                    {
                                        ClassStaticBus.bus.Dispose();
                                        GC.SuppressFinalize(ClassStaticBus.bus);
                                        ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                                    }
                                    Sensor fc = GlobalInit.SensorListSDRNS.Find(t => t.Name == se.se.Name && t.Equipment.TechId == se.se.Equipment.TechId);
                                    if (fc != null)
                                    {
                                        if ((se.Cnt_timer >= BaseXMLConfiguration.xml_conf._CheckActivitySensor) && (se.Cnt_timer < BaseXMLConfiguration.xml_conf._MaxTimeNotActivateStatusSensor))
                                        {
                                            if (ClassStaticBus.bus.IsConnected)
                                            {
                                                busManager.SendDataObject(se.se, GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                                GlobalInit.Lds_Activity_Sensor_Receiver.Add(ClassStaticBus.bus.Receive(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId, x => x
                                                        .Add<Sensor>(message_x =>
                                                        {
                                                            if (message_x.Name == se.se.Name && message_x.Equipment.TechId == se.se.Equipment.TechId)
                                                            {
                                                                se.Cnt_sensor_New++;
                                                                se.Cnt_timer = 0;
                                                                se.Cnt_all_time = 0;
                                                                se.Cnt_sensor_Old = 0;
                                                                se.BZ.Start();

                                                            }
                                                        })));
                                            }
                                            else
                                            {
                                                ClassStaticBus.bus.Dispose();
                                                GC.SuppressFinalize(ClassStaticBus.bus);
                                                ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                                            }
                                            fc.Status = AllStatusSensor.F.ToString();
                                            se.Cnt_timer = 0;
                                            se.Cnt_sensor_New = 0;
                                            ClassDBGetSensor gsd = new ClassDBGetSensor();
                                            gsd.UpdateStatusSensor(fc);
                                        }
                                        else if ((se.Cnt_all_time >= BaseXMLConfiguration.xml_conf._MaxTimeNotActivateStatusSensor))
                                        {
                                            bool isCheck = false;
                                            if (ClassStaticBus.bus.IsConnected)
                                            {
                                                busManager.SendDataObject(se.se, GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                                GlobalInit.Lds_Activity_Sensor_Receiver.Add(ClassStaticBus.bus.Receive(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId, x => x
                                                    .Add<Sensor>(message_x =>
                                                    {
                                                        if (message_x.Name == se.se.Name && message_x.Equipment.TechId == se.se.Equipment.TechId)
                                                        {
                                                            se.Cnt_sensor_New++; se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_Old = 0; se.BZ.Start();
                                                            isCheck = true;
                                                        }
                                                    })));
                                            }
                                            else
                                            {
                                                ClassStaticBus.bus.Dispose();
                                                GC.SuppressFinalize(ClassStaticBus.bus);
                                                ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                                            }
                                            if (!isCheck) se.Cnt_sensor_Old++;

                                            if ((se.Cnt_sensor_New == 0) || (se.Cnt_sensor_Old > 2))
                                            {
                                                fc.Status = AllStatusSensor.Z.ToString(); se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_New = 0; se.Cnt_sensor_Old = 0;
                                                ClassDBGetSensor gsd = new ClassDBGetSensor();
                                                gsd.UpdateStatusSensor(fc);
                                                GlobalInit.SensorListSDRNS.RemoveAll(t => t.Name == fc.Name && t.Equipment.TechId == fc.Equipment.TechId);
                                                se.BZ.Stop();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("--> Error execute proc CheckActivitySensor(ID) " + ex.Message);
            }
            return isActive;
        }
        */
    }

    /// <summary>
    /// Специальный класс для хранения временных отрезков, указывающих на активность или недоступность сенсора
    /// </summary>
    public class Mdx
    {
        public Sensor se { get; set; }
        public System.Timers.Timer BZ { get; set; }
        public Int64 Cnt_timer { get; set; }
        public Int64 Cnt_all_time { get; set; }
        public Int64 Cnt_sensor_New { get; set; }
        public Int64 Cnt_sensor_Old { get; set; }
        public Mdx(Sensor v)
        {
            BZ = new System.Timers.Timer();
            BZ.Interval = 1000;
            BZ.Elapsed += BZ_Elapsed;
            BZ.Start();
            Cnt_timer = 0;
            Cnt_all_time = 0;
            Cnt_sensor_New = 0;
            Cnt_sensor_Old = 0;
            se = new Sensor();
            se = v;
        }

        private void BZ_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Cnt_timer++; Cnt_all_time++;
        }
    }
}
