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
    /// <summary>
    /// Класс предназначен для периодического выполнения операций проверки очереди сообщений GlobalInit.Template_SENSORS_List_ (в данной очереди хранятся объекты Sensor, которые отправляются с SDR вместе с новыми координатами)
    /// 
    /// </summary>
    public class ClassReceiveAllSensors : IDisposable
    {
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
            BusManager<Sensor> busManager = new BusManager<Sensor>();
            //try {
                Task tsk = new Task(() =>
                {
                    if (GlobalInit.SensorListSDRNS.Count == 0)
                    {
                        ClassDBGetSensor DB = new ClassDBGetSensor();
                        List<Sensor> L_S = DB.LoadObjectSensor();
                        DB.Dispose();
                        if (L_S != null)
                        {
                            foreach (Sensor stx in L_S.ToArray())
                            {
                                Sensor fnd = GlobalInit.SensorListSDRNS.Find(t => t.Name == stx.Name && t.Equipment.TechId == stx.Equipment.TechId);
                                if (fnd != null)
                                    GlobalInit.SensorListSDRNS.ReplaceAll<Sensor>(fnd, stx);
                                else GlobalInit.SensorListSDRNS.Add(stx);
                            }
                        }
                    }
                    
                        if (ClassStaticBus.bus.Advanced.IsConnected)
                        {
                            uint cnt = busManager.GetMessageCount(GlobalInit.Template_SENSORS_List_);
                            for (int i=0; i< cnt; i++)
                            {
                                var message = busManager.GetDataObject(GlobalInit.Template_SENSORS_List_);
                                if (message != null)
                                {
                                    ClassDBGetSensor DB = new ClassDBGetSensor();
                                    Sensor fnd_s = GlobalInit.SensorListSDRNS.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId);
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
                                                            GlobalInit.SensorListSDRNS.Add(L_S[0]);
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
                                            if (L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId) != null)
                                            {
                                                Sensor fnd = GlobalInit.SensorListSDRNS.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId);
                                                if (fnd != null)
                                                    GlobalInit.SensorListSDRNS.ReplaceAll<Sensor>(fnd, L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId));
                                                else GlobalInit.SensorListSDRNS.Add(L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Отправка на SDR уведомления о приеме сообщения
                                        DB.SaveLocationCoordSensor((message as Sensor));
                                        busManager.SendDataObject((message as Sensor), GlobalInit.Template_Event_Confirm_SENSORS_Send_ + (message as Sensor).Name + (message as Sensor).Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                        List<Sensor> L_S = DB.LoadObjectSensor();
                                        if (L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId) != null)
                                        {
                                            Sensor fnd = GlobalInit.SensorListSDRNS.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId);
                                            if (fnd != null)
                                                GlobalInit.SensorListSDRNS.ReplaceAll<Sensor>(fnd, L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId));
                                            else GlobalInit.SensorListSDRNS.Add(L_S.Find(t => t.Name == (message as Sensor).Name && t.Equipment.TechId == (message as Sensor).Equipment.TechId));
                                        }
                                    }
                                    DB.Dispose();
                                    GC.Collect();
                                }
                                else break;
                                }
                            }
                            /*
                            GlobalInit.Lds_Activity_Sensor_List.Add(ClassStaticBus.bus.Receive<Sensor>(GlobalInit.Template_SENSORS_List_,
                            message =>
                            {
                                ClassDBGetSensor DB = new ClassDBGetSensor();
                                Sensor fnd_s = GlobalInit.SensorListSDRNS.Find(t => t.Name == message.Name && t.Equipment.TechId == message.Equipment.TechId);
                                if (fnd_s == null)
                                {
                                    bool isFindInDB = false;
                                    List<Sensor> L_S = DB.LoadObjectSensor(message.Name, message.Equipment.TechId, "Z");
                                    if (L_S != null)
                                    {
                                        if (L_S.Count > 0)
                                        {
                                            if (L_S[0].Name == message.Name)
                                            {
                                                if (L_S[0].Equipment != null)
                                                {
                                                    if (L_S[0].Equipment.TechId == message.Equipment.TechId)
                                                    {
                                                        isFindInDB = true;
                                                        GlobalInit.SensorListSDRNS.Add(L_S[0]);
                                                        L_S[0].Status = "A";
                                                        DB.UpdateStatusSensorWithArchive(L_S[0]);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (isFindInDB == false)
                                    {
                                        DB.CreateNewObjectSensor(message);
                                        //Отправка на SDR уведомления о приеме сообщения
                                        BusManager<Sensor> busManager = new BusManager<Sensor>();
                                        busManager.SendDataObject(message, GlobalInit.Template_Event_Confirm_SENSORS_Send_ + message.Name + message.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                        L_S = DB.LoadObjectSensor();
                                        if (L_S.Find(t => t.Name == message.Name && t.Equipment.TechId == message.Equipment.TechId) != null)
                                        {
                                            Sensor fnd = GlobalInit.SensorListSDRNS.Find(t => t.Name == message.Name && t.Equipment.TechId == message.Equipment.TechId);
                                            if (fnd != null)
                                                GlobalInit.SensorListSDRNS.ReplaceAll<Sensor>(fnd, L_S.Find(t => t.Name == message.Name && t.Equipment.TechId == message.Equipment.TechId));
                                            else GlobalInit.SensorListSDRNS.Add(L_S.Find(t => t.Name == message.Name && t.Equipment.TechId == message.Equipment.TechId));
                                        }
                                    }
                                }
                                else
                                {
                                    //Отправка на SDR уведомления о приеме сообщения
                                    DB.SaveLocationCoordSensor(message);
                                    BusManager<Sensor> busManager = new BusManager<Sensor>();
                                    busManager.SendDataObject(message, GlobalInit.Template_Event_Confirm_SENSORS_Send_ + message.Name + message.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString());
                                    List<Sensor> L_S = DB.LoadObjectSensor();
                                    if (L_S.Find(t => t.Name == message.Name && t.Equipment.TechId == message.Equipment.TechId) != null)
                                    {
                                        Sensor fnd = GlobalInit.SensorListSDRNS.Find(t => t.Name == message.Name && t.Equipment.TechId == message.Equipment.TechId);
                                        if (fnd != null)
                                            GlobalInit.SensorListSDRNS.ReplaceAll<Sensor>(fnd, L_S.Find(t => t.Name == message.Name && t.Equipment.TechId == message.Equipment.TechId));
                                        else GlobalInit.SensorListSDRNS.Add(L_S.Find(t => t.Name == message.Name && t.Equipment.TechId == message.Equipment.TechId));
                                    }
                                }
                                DB.Dispose();
                            }));
                            */
                        else {
                            ClassStaticBus.bus.Dispose();
                            GC.SuppressFinalize(ClassStaticBus.bus);
                            ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                            CoreICSM.Logs.CLogs.WriteInfo(CoreICSM.Logs.ELogsWhat.Unknown, "-> Bus dispose... ");
                        }
                });
                tsk.Start();
                //tsk.Wait();
            //}
            //catch (Exception ex) {
            //CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[ReceiveAllSensorList]:" + ex.Message);
            //}

        }
    }   
}
