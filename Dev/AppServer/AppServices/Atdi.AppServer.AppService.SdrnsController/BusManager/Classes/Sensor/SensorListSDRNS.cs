using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLLibrary;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer;
using RabbitMQ.Client;

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
        public static ILogger logger;
        public SensorListSDRNS(ILogger log)
        {
            if (logger == null) logger = log;
        }
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
            logger.Trace("Start procedure CreateShortSensorList...");
            List<ShortSensor> val = new List<ShortSensor>();
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                ClassDBGetSensor DB = new ClassDBGetSensor(logger);
                List<Sensor> val_all_s = DB.LoadObjectSensor();
                {
                    if (val_all_s != null)
                    {
                        foreach (Sensor sd in val_all_s.ToArray())
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

                            val.Add(sh);
                        }
                    }
                }
            });
            thread.Start();
            thread.Join();
            logger.Trace("End procedure CreateShortSensorList.");
            return val;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ShortSensor CreateShortSensorListBySensorId(int Id)
        {
            logger.Trace("Start procedure CreateShortSensorListBySensorId...");
            ShortSensor val = new ShortSensor();
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                ClassDBGetSensor DB = new ClassDBGetSensor(logger);
                Sensor sd = DB.LoadObjectSensor(Id);
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
                    val = sh;
                }
            });
            thread.Start();
            thread.Join();
            logger.Trace("End procedure CreateShortSensorListBySensorId.");
            return val;
        }
        /// <summary>
        /// проверка активности сенсоров
        /// </summary>
        public void CheckActivitySensor()
        {
            try
            {
                logger.Trace("Start procedure CheckActivitySensor...");
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    ClassDBGetSensor gsd = new ClassDBGetSensor(logger);
                    BusManager<Sensor> busManager = new BusManager<Sensor>();
                    List<Sensor> L_S = gsd.LoadObjectAllSensor();
                    gsd.Dispose();
                    if (L_S != null)
                    {
                        if (L_S.Count > 0)
                        {
                            foreach (Sensor message in L_S.ToArray())
                            {
                                //busManager.SendDataObject(message, GlobalInit.Template_Event_CheckActivitySensor_Req + message.Name + message.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTemp.ToString());
                                busManager.SendDataObject(message, GlobalInit.Template_Event_CheckActivitySensor_Req + message.Name + message.Equipment.TechId);
                            }
                        }


                        foreach (Sensor message in L_S.ToArray())
                        {
                            if (GlobalInit.Lst_timers.Find(t => t.se.Name == message.Name && t.se.Equipment.TechId == message.Equipment.TechId) == null)
                            {
                                Mdx RW = new Mdx(message);
                                GlobalInit.Lst_timers.Add(RW);
                            }
                        }
                        foreach (Mdx se in GlobalInit.Lst_timers.ToArray())
                        {
                            if (ClassStaticBus.factory != null)
                            {
                                uint MessCount = busManager.GetMessageCount(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
                                for (int i = 0; i < MessCount; i++)
                                {
                                    var message_x = busManager.GetDataObject<Sensor>(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
                                    if (message_x != null)
                                    {
                                        if ((message_x as Sensor).Name == se.se.Name && (message_x as Sensor).Equipment.TechId == se.se.Equipment.TechId)
                                        {
                                            se.Cnt_sensor_New++; se.Cnt_timer = 0; se.Cnt_all_time = 0;
                                            Sensor f = L_S.Find(t => t.Name == se.se.Name && t.Equipment.TechId == se.se.Equipment.TechId);
                                            if (f != null)
                                            {
                                                se.Cnt_sensor_New++; f.Status = "A"; se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_Old = 0; se.BZ.Close(); se.BZ.Start(); ClassDBGetSensor.UpdateStatusSensor(f);
                                            }
                                            else
                                            {
                                                se.Cnt_timer = 0; se.Cnt_all_time = 0; se.Cnt_sensor_New = 0;
                                            }
                                        }
                                    }
                                    else break;
                                }

                            }
                            else
                            {
                                ClassStaticBus.factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword, VirtualHost = GlobalInit.RabbitVirtualHost, SocketReadTimeout = 2147000000, SocketWriteTimeout = 2147000000 };
                            }

                            Sensor fc = L_S.Find(t => t.Name == se.se.Name && t.Equipment.TechId == se.se.Equipment.TechId);
                            if (fc != null)
                            {
                                if ((se.Cnt_timer >= BaseXMLConfiguration.xml_conf._CheckActivitySensor) && (se.Cnt_all_time < BaseXMLConfiguration.xml_conf._MaxTimeNotActivateStatusSensor))
                                {
                                    if (ClassStaticBus.factory != null)
                                    {
                                        //busManager.SendDataObject(se.se, GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                        busManager.SendDataObject(se.se, GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId);
                                        uint MessCount = busManager.GetMessageCount(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
                                        for (int i = 0; i < MessCount; i++)
                                        {
                                            var message_x = busManager.GetDataObject<Sensor>(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
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

                                    }
                                    else
                                    {
                                        ClassStaticBus.factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword, VirtualHost = GlobalInit.RabbitVirtualHost, SocketReadTimeout = 2147000000, SocketWriteTimeout = 2147000000 };
                                    }
                                    fc.Status = AllStatusSensor.F.ToString();
                                    se.Cnt_timer = 0;
                                    se.Cnt_sensor_New = 0;
                                    ClassDBGetSensor.UpdateStatusSensor(fc);
                                }
                                else if ((se.Cnt_all_time >= BaseXMLConfiguration.xml_conf._MaxTimeNotActivateStatusSensor))
                                {
                                    bool isCheck = false;
                                    if (ClassStaticBus.factory != null)
                                    {
                                        //busManager.SendDataObject(se.se, GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                        busManager.SendDataObject(se.se, GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId);
                                        uint MessCount = busManager.GetMessageCount(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
                                        for (int i = 0; i < MessCount; i++)
                                        {
                                            var message_x = busManager.GetDataObject<Sensor>(GlobalInit.Template_Event_CheckActivitySensor_Resp + se.se.Name + se.se.Equipment.TechId);
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
                                    }
                                    else
                                    {
                                        ClassStaticBus.factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword, VirtualHost = GlobalInit.RabbitVirtualHost, SocketReadTimeout = 2147000000, SocketWriteTimeout = 2147000000 };
                                    }
                                    if (!isCheck) busManager.DeleteQueue(GlobalInit.Template_Event_CheckActivitySensor_Req + se.se.Name + se.se.Equipment.TechId);
                                    if (!isCheck) se.Cnt_sensor_Old++;
                                    if ((se.Cnt_sensor_New == 0) || (se.Cnt_sensor_Old > 2))
                                    {
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
                });
                thread.Start();
                thread.Join();
                logger.Trace("End procedure CheckActivitySensor.");
            }
            catch (Exception ex)
            {
                logger.Trace("Error in procedure CheckActivitySensor.");
            }
        }
     
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
