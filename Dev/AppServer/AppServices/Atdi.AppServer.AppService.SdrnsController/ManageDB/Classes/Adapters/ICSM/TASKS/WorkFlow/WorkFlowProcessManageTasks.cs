using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.Sheduler;
using Atdi.AppServer;


namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    public class WorkFlowProcessManageTasks: IDisposable
    {
        private ILogger logger;

        public WorkFlowProcessManageTasks(ILogger log)
        {
            logger = log;
        }

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~WorkFlowProcessManageTasks()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void UpdateListMeasTask()
        {
            ClassConvertTasks ts = new ClassConvertTasks();
            ClassesDBGetTasks cl = new ClassesDBGetTasks(logger);
            Task<MeasTask[]> task = ts.ConvertTo_MEAS_TASKObjects(cl.ReadlAllSTasksFromDB());
            task.Wait();
            List<MeasTask> mts_ = task.Result.ToList();
            //List<MeasTask> mts_ = ts.ConvertTo_MEAS_TASKObjects(cl.ReadlAllSTasksFromDB()).ToList();
            foreach (MeasTask FND in mts_.ToArray())
            {
                // Удаляем данные об объекте с глобального списка
                if (GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == FND.Id.Value) == null) {
                    GlobalInit.LIST_MEAS_TASK.Add(FND);
                }
                else {
                    //lock (GlobalInit.LIST_MEAS_TASK)
                    if ((FND.Status!=null) && (FND.Status != "N"))
                    {
                        MeasTask fnd = GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == FND.Id.Value);
                        if (fnd != null)
                            GlobalInit.LIST_MEAS_TASK.ReplaceAll<MeasTask>(fnd, FND);
                        else GlobalInit.LIST_MEAS_TASK.Add(FND);
                    }
                }
            }
            ts.Dispose();
            cl.Dispose();
        }

        /// <summary>
        /// Добавление в очередь новых тасков
        /// </summary>
        /// <param name="s_out"></param>
        /// <returns>количество новых объектов, добавленных в глобальный список</returns>
        public int Create_New_Meas_Task(MeasTask s_out, string ActionType)
        {
            ClassesDBGetTasks cl = new ClassesDBGetTasks(logger);
            ClassConvertTasks ts = new ClassConvertTasks();
            int NewIdMeasTask = -1;
            try
            {
                    if (s_out != null)
                    {
                        //lock (GlobalInit.LIST_MEAS_TASK)
                        //UpdateListMeasTask();
                        MeasTask Data_ = s_out;
                        // создаём объект список подзадач типа MEAS_SUB_TASK и записываем в объект Data_
                        Data_.CreateAllSubTasks();
                        //CoreICSM.Logs.CLogs.WriteInfo(ELogsWhat.Unknown, "[CreateAllSubTasks] success...");
                        // конвертируем объекты тасков с БД в список List<MEAS_TASK>
                        //List<MeasTask> mts_ = ts.ConvertTo_MEAS_TASKObjects(cl.ReadlAllSTasksFromDB()).ToList();
                        List<MeasTask> mts_ = GlobalInit.LIST_MEAS_TASK;
                        if (mts_.Find(r => r.Id.Value == Data_.Id.Value) == null)
                        {
                            if (((GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == Data_.Id.Value) == null)))
                            {
                                Data_.UpdateStatus(ActionType);
                                //CoreICSM.Logs.CLogs.WriteInfo(ELogsWhat.Unknown, "Success UpdateStatus !!!...");
                                NewIdMeasTask = cl.SaveTaskToDB(Data_);
                                //CoreICSM.Logs.CLogs.WriteInfo(ELogsWhat.Unknown, "Success create new TASK !!!...");
                                MeasTask fnd = GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == Data_.Id.Value);
                                if (fnd != null)
                                    GlobalInit.LIST_MEAS_TASK.ReplaceAll<MeasTask>(fnd, Data_);
                                else GlobalInit.LIST_MEAS_TASK.Add(Data_);

                            }
                            else {
                                if (GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == Data_.Id.Value) != null)
                                {
                                    Data_ = GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == Data_.Id.Value);
                                    Data_.UpdateStatus(ActionType);
                                    //cl.SaveStatusTaskToDB(Data_);
                                    MeasTask fnd = GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == Data_.Id.Value);
                                    if (fnd != null)
                                        GlobalInit.LIST_MEAS_TASK.ReplaceAll<MeasTask>(fnd, Data_);
                                    else GlobalInit.LIST_MEAS_TASK.Add(Data_);
                                }
                            }
                        }
                        else {
                            if (GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == Data_.Id.Value) != null)
                            {
                                Data_ = GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == Data_.Id.Value);
                                Data_.UpdateStatus();
                                //cl.SaveStatusTaskToDB(Data_);
                                MeasTask fnd = GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == Data_.Id.Value);
                                if (fnd != null)
                                    GlobalInit.LIST_MEAS_TASK.ReplaceAll<MeasTask>(fnd, Data_);
                                else GlobalInit.LIST_MEAS_TASK.Add(Data_);
                            }
                        }

                    }
                    //CoreICSM.Logs.CLogs.WriteInfo(ELogsWhat.Unknown, "[Create_New_Meas_Task] success...");
              
            }
            catch (Exception er) {
            //    CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[Create_New_Meas_Task]: " + er.Message);
            }
            cl.Dispose();
            ts.Dispose();
            return NewIdMeasTask;
        }

      
        /// <summary>
        /// 
        /// </summary>
        public bool Process_Multy_Meas(MeasTask mt, List<int> SensorIds, string ActionType, bool isOnline)
        {
            bool isSendSuccess = false;
            try
            {
                ClassesDBGetTasks cl = new ClassesDBGetTasks(logger);
                List<MeasSdrTask> Checked_L = new List<MeasSdrTask>();
                //Task tdf = new Task(() =>
                //{
                if (mt != null)
                {
                    List<MeasSdrTask> LM_SDR = new List<MeasSdrTask>();
                    foreach (int SensorId in SensorIds.ToArray())
                    {
                        Sensor fnd_s = GlobalInit.SensorListSDRNS.Find(t => t.Id.Value == SensorId);
                        if (fnd_s != null)
                        {
                            System.Threading.Thread tsk = new System.Threading.Thread(() =>
                            {
                                Checked_L.Clear();
                                if (GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == mt.Id.Value) != null)
                                {
                                    MeasTask M = GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == mt.Id.Value);
                                    int Id_Old = M.Id.Value;
                                    MeasSubTask[] msbd_old = M.MeasSubTasks;
                                    M = mt;
                                    M.Id.Value = Id_Old;
                                    if (M.Stations.ToList().FindAll(e => e.StationId.Value == SensorId) != null)
                                    {
                                        M.UpdateStatusSubTasks(SensorId, ActionType, isOnline);
                                        LM_SDR = M.CreateeasTaskSDRs(ActionType);

                                        //if (isCreateTasksSdr == false) {  LM_SDR = M.CreateeasTaskSDRs(ActionType); }
                                        if (LM_SDR != null)
                                        {
                                            string ids = "";
                                            int MaxVal = cl.GetMaXIdsSdrTasks(M);
                                            int idx = MaxVal + 1;
                                            foreach (MeasSdrTask mx in LM_SDR.ToArray())
                                            {
                                                mx.Id = idx;
                                                ids = idx.ToString() + ";";
                                                //Task ts = new Task(() => {
                                                //если сенсор активен
                                                //if (fnd_s.Status == "A")
                                                //{
                                                if (mx.SensorId.Value == SensorId)
                                                {
                                                    /// Перед отправкой включаем валидацию созданных объектов MeasTaskSDR
                                                    if (mx.ValidationMeas())
                                                    {
                                                        Checked_L.Add(mx);
                                                        M.UpdateStatus(ActionType);
                                                    }
                                                    else {
                                                        // если вадидация не прошла
                                                        M.UpdateStatusE_E(mx.MeasSubTaskStationId, "E_E");
                                                        M.UpdateStatus(ActionType);
                                                        //обновление группы статусов для объекта MeasTask
                                                        cl.SaveStatusTaskToDB(M);
                                                    }
                                                }
                                                idx++;
                                            }
                                            if (ids.Length > 0) ids = ids.Remove(ids.Count() - 1, 1);
                                            cl.SaveIdsSdrTasks(M, ids);
                                        }
                                    }

                                    M.MeasSubTasks = msbd_old;
                                    MeasTask fnd = GlobalInit.LIST_MEAS_TASK.Find(j => j.Id.Value == Id_Old);
                                    if (fnd != null)
                                        GlobalInit.LIST_MEAS_TASK.ReplaceAll<MeasTask>(fnd, M);
                                    else GlobalInit.LIST_MEAS_TASK.Add(M);
                                    if (ActionType == "Del")
                                    {
                                        GlobalInit.LIST_MEAS_TASK.RemoveAll(t => t.Id.Value == M.Id.Value);
                                    }
                                }
                            });
                            tsk.Start();
                            tsk.Join();


                            if (Checked_L.Count > 0)
                            {
                                //CoreICSM.Logs.CLogs.WriteInfo(ELogsWhat.Unknown, "--> Start busManager send new task...");
                                BusManager<List<MeasSdrTask>> busManager = new BusManager<List<MeasSdrTask>>();
                                if (busManager.SendDataObject(Checked_L, GlobalInit.Template_MEAS_TASK_Main_List_APPServer + fnd_s.Name + fnd_s.Equipment.TechId, XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString()))
                                {
                                    isSendSuccess = true;
                                    //CoreICSM.Logs.CLogs.WriteInfo(ELogsWhat.Unknown, "--> Success send new task...");
                                }
                                else {
                                    isSendSuccess = false;
                                    //Sheduler_Send_MeasSdr shed = new Sheduler_Send_MeasSdr();
                                    //если отправка не получилась - пытаемся отправить сообщение через 1 минуту
                                    //shed.ShedulerRepeatStart(60, mt, SensorIds, ActionType, isOnline);
                                }

                                // Отправка сообщения в СТОП-ЛИСТ
                                if ((ActionType == "Stop") && (isOnline) && ((Checked_L[0].status == "F") || (Checked_L[0].status == "P")))
                                {
                                    if (busManager.SendDataObject(Checked_L, GlobalInit.Template_MEAS_TASK_Stop_List + fnd_s.Name + fnd_s.Equipment.TechId + Checked_L[0].MeasTaskId.Value.ToString() + Checked_L[0].SensorId.Value.ToString(), XMLLibrary.BaseXMLConfiguration.xml_conf._TimeExpirationTask.ToString()))
                                    {
                                        isSendSuccess = true;
                                    }
                                }
                                else {
                                    //busManager.DeleteQueue(GlobalInit.Template_MEAS_TASK_Stop_List + fnd_s.Name + fnd_s.Equipment.TechId + Checked_L[0].MeasTaskId.Value.ToString() + Checked_L[0].SensorId.Value.ToString());
                                }
                            }
                            Checked_L.Clear();
                        }
                    }
                    //});
                    //tsk.Start();
                    //tsk.Wait();
                }
                       
                   
                //cl.Dispose();
               //});
               //tdf.Start();
               //tdf.Wait();
            }
            catch (Exception ex)
            {
                //CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[Process_Multy_Meas]: " + ex.Message);
            }
            return isSendSuccess;


        }
      
      

    }
}
