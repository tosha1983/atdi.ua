using System;
using System.Collections.Generic;
using EasyNetQ;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer;


namespace Atdi.SDNRS.AppServer.BusManager
{
    /// <summary>
    /// Класс предназначен для периодического выполнения операций проверки очереди сообщений GlobalInit.Template_SENSORS_List_ (в данной очереди хранятся объекты Sensor, которые отправляются с SDR вместе с новыми координатами)
    /// 
    /// </summary>
    public class ClassReceiveAllSensors : IDisposable
    {
        public static ILogger logger;
        public ClassReceiveAllSensors(ILogger log)
        {
            if(logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassReceiveAllSensors()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Метод, выполняющий проверку наличия данных о сенсорах в БД
        /// Если сенсоры в БД обнаружены - выполняется добавление данных в глобальный список SensorListSDRNS
        /// В случае, если в очереди GlobalInit.Template_SENSORS_List_ обнаружены сообщения, выполняется пересохранение объекта Sensor в БД
        /// Также, после обновления данных о сенсоре в БД осуществляется отправка уведомления(в виде объекта Sensor) на SDR
        /// 
        /// </summary>
        public void ReceiveAllSensorList()
        {
            try{ 
            BusManager<Sensor> busManager = new BusManager<Sensor>();
              System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    logger.Trace("Start procedure ReceiveAllSensorList...");
                    ClassDBGetSensor DB = new ClassDBGetSensor(logger);
                    List<Sensor> SensorListSDRNS = DB.LoadObjectSensor();
                    foreach (Sensor s in SensorListSDRNS)
                    {
                        string apiVer = DB.GetSensorApiVersion(s.Id.Value);
                        if (apiVer == "v2.0")
                        {
                            busManager.RegisterQueue(s.Name, s.Equipment.TechId, apiVer);
                        }
                    }
                    if (ClassStaticBus.bus.Advanced.IsConnected)
                        {
                            uint cnt = busManager.GetMessageCount(GlobalInit.Template_SENSORS_List_);
                            List<Sensor> distinctSensors = new List<Sensor>();
                            for (int i = 0; i < cnt; i++)
                            {
                                var message = busManager.GetDataObject(GlobalInit.Template_SENSORS_List_);
                                if (message != null)
                                {
                                    Sensor fnd_s = distinctSensors.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId);
                                    if (fnd_s == null)
                                    {
                                        distinctSensors.Add((message as Sensor));
                                    }
                                }
                            }
                            for (int i=0; i< distinctSensors.Count; i++)
                            {
                                var message = distinctSensors[i];
                                if (message != null)
                                {
                                    Sensor fnd_s = SensorListSDRNS.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId);
                                    if (fnd_s == null)
                                    {
                                        bool isFindInDB = false;
                                        List<Sensor> L_S = DB.LoadObjectSensor((message as Sensor).Name, (message as Sensor).Equipment.TechId, "Z");
                                        if (L_S != null)
                                        {
                                            if (L_S.Count > 0)
                                            {
                                                if (L_S[0].Name == (message as Sensor).Name)
                                                {
                                                    if (L_S[0].Equipment != null)
                                                    {
                                                        if (L_S[0].Equipment.TechId == (message as Sensor).Equipment.TechId)
                                                        {
                                                            isFindInDB = true;
                                                            //SensorListSDRNS.Add(L_S[0]);
                                                            L_S[0].Status = "A";
                                                            DB.UpdateStatusSensorWithArchive(L_S[0]);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (isFindInDB == false)
                                        {
                                            DB.CreateNewObjectSensor((message as Sensor));
                                            //Отправка на SDR уведомления о приеме сообщения
                                            busManager.SendDataObject((message as Sensor), GlobalInit.Template_Event_Confirm_SENSORS_Send_ + (message as Sensor).Name + (message as Sensor).Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                            L_S = DB.LoadObjectSensor();
                                            //if (L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId) != null)
                                            //{
                                                //Sensor fnd = SensorListSDRNS.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId);
                                                //if (fnd != null)
                                                    //GlobalInit.SensorListSDRNS.ReplaceAll<Sensor>(fnd, L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId));
                                                //else GlobalInit.SensorListSDRNS.Add(L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId));
                                            //}
                                        }
                                    }
                                    else
                                    {
                                        //Отправка на SDR уведомления о приеме сообщения
                                        DB.SaveLocationCoordSensor((message as Sensor));
                                        busManager.SendDataObject((message as Sensor), GlobalInit.Template_Event_Confirm_SENSORS_Send_ + (message as Sensor).Name + (message as Sensor).Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                        List<Sensor> L_S = DB.LoadObjectSensor();
                                        //if (L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId) != null)
                                        //{
                                            //Sensor fnd = GlobalInit.SensorListSDRNS.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId);
                                            //if (fnd != null)
                                                //GlobalInit.SensorListSDRNS.ReplaceAll<Sensor>(fnd, L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId));
                                            //else GlobalInit.SensorListSDRNS.Add(L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId));
                                        //}
                                    }
                                    DB.Dispose();
                                    GC.Collect();
                                }
                                else break;
                                }
                            }
                           
                        else {
                            ClassStaticBus.bus.Dispose();
                            GC.SuppressFinalize(ClassStaticBus.bus);
                            ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                        }

                 
                    logger.Trace("End procedure ReceiveAllSensorList.");
                });
                tsk.Start();
                tsk.Join();
            }
            catch (Exception ex) {
                logger.Error("Error in ReceiveAllSensorList:" + ex.Message);
            }

        }
    }   
}
