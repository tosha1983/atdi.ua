using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer;
using Newtonsoft.Json;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    public class WorkFlowProcessManageTasks: IDisposable
    {
       public static ILogger logger;

        public WorkFlowProcessManageTasks(ILogger log)
        {
            if (logger == null) logger = log;
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


        /// <summary>
        /// Добавление в очередь новых тасков
        /// </summary>
        /// <param name="s_out"></param>
        /// <returns>количество новых объектов, добавленных в глобальный список</returns>
        public int? Create_New_Meas_Task(MeasTask s_out, string ActionType)
        {
            logger.Trace("Start procedure Create_New_Meas_Task...");
            var cl = new ClassesDBGetTasks(logger);
            var ts = new ClassConvertTasks(logger);
            int? NewIdMeasTask = -1;
            try
            {
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    if (s_out != null)
                    {
                        var Data_ = s_out;
                        var mts_ = ts.ConvertToShortMeasTasks(cl.ShortReadTask(Data_.Id.Value)).ToList();
                        if (mts_.Count() == 0)
                        {
                            Data_.UpdateStatus(ActionType);
                            NewIdMeasTask = cl.SaveTaskToDB(Data_);
                        }
                        else
                        {
                            Data_ = ts.ConvertToShortMeasTasks(cl.ShortReadTask(Data_.Id.Value)).ToList()[0];
                            Data_.UpdateStatus(ActionType);
                            cl.SaveStatusTaskToDB(Data_);
                        }
                    }
                });
                tsk.Start();
                tsk.Join();

            }
            catch (Exception er)
            {
                logger.Error("Error in procedure Create_New_Meas_Task: " + er.Message);
            }
            cl.Dispose();
            ts.Dispose();
            logger.Trace("End procedure Create_New_Meas_Task.");
            return NewIdMeasTask;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool Process_Multy_Meas(MeasTask mt, List<int> SensorIds, string ActionType, bool isOnline, out bool isSuccess, out int? IdTask)
        {
            IdTask = null;
            int? IdTsk = null;
            bool isSendSuccess = false;
            bool isSuccessTemp = false;
            isSuccess = false;
            try
            {
                logger.Trace("Start procedure Process_Multy_Meas...");
                var gsd = new ClassDBGetSensor(logger);
                var cl = new ClassesDBGetTasks(logger);
                var ts = new ClassConvertTasks(logger);
                var Checked_L = new List<MeasSdrTask>();
                var Checked_L_Device = new List<Atdi.DataModels.Sdrns.Device.MeasTask>();
                var Checked_L_Device_3_0 = new List<Atdi.AppServer.Contracts.Sdrns.MeasSdrTask>();

                System.Threading.Thread tsk_main = new System.Threading.Thread(() =>
                {
                    if (mt != null)
                    {
                        if (ActionType == "Del")
                        {
                            isSuccessTemp = cl.SetHistoryStatusTasksInDB(mt, "Z");
                        }
                        var Res = new MeasTask[1] { mt };
                        var LM_SDR = new List<MeasSdrTask>();
                        var LM_SDR_Device = new List<Atdi.DataModels.Sdrns.Device.MeasTask>();
                        foreach (int SensorId in SensorIds.ToArray())
                        {
                            var fnd_s = gsd.LoadObjectSensor(SensorId);
                            if (fnd_s != null)
                            {
                                if (fnd_s.Name != null)
                                {
                                    string apiVer = gsd.GetSensorApiVersion(fnd_s.Id.Value);
                                    Checked_L.Clear();
                                    if ((Res != null) && (Res.Length > 0))
                                    {
                                        var M = Res[0]; 
                                        if (ActionType == "New")
                                        {
                                             M.CreateAllSubTasksApi1_0();
                                        }
                                        int Id_Old = M.Id.Value;
                                        var msbd_old = M.MeasSubTasks;
                                        M = mt;
                                        M.Id.Value = Id_Old;
                                        if (M.Stations.ToList().FindAll(e => e.StationId.Value == SensorId) != null)
                                        {
                                            M.UpdateStatusSubTasks(SensorId, ActionType, isOnline);
                                            if (apiVer == "v1.0")
                                            {
                                                if (ActionType == "New")
                                                {
                                                    M.CreateeasTaskSDRsApi1_0(ActionType);
                                                    IdTsk = Create_New_Meas_Task(M, "New");
                                                    var mts_ = ts.ConvertToShortMeasTasks(cl.ShortReadTask(IdTsk.Value)).ToList();
                                                    if (mts_.Count()> 0)
                                                    {
                                                        var measTaskOld = M; 
                                                        M = new MeasTask()
                                                        {
                                                            CreatedBy = mts_[0].CreatedBy,
                                                            DateCreated = mts_[0].DateCreated,
                                                            ExecutionMode = mts_[0].ExecutionMode,
                                                            Id = mts_[0].Id,
                                                            MaxTimeBs = mts_[0].MaxTimeBs,
                                                            MeasDtParam = mts_[0].MeasDtParam,
                                                            MeasFreqParam = mts_[0].MeasFreqParam,
                                                            MeasLocParams = mts_[0].MeasLocParams,
                                                            MeasOther = mts_[0].MeasOther,
                                                            MeasSubTasks = mts_[0].MeasSubTasks,
                                                            MeasTimeParamList = mts_[0].MeasTimeParamList,
                                                            Name = mts_[0].Name,
                                                            OrderId = mts_[0].OrderId,
                                                            Prio = mts_[0].Prio,
                                                            ResultType = mts_[0].ResultType,
                                                            Status = mts_[0].Status,
                                                            Task = mts_[0].Task,
                                                            Type = mts_[0].Type,
                                                            Stations = measTaskOld.Stations,
                                                            StationsForMeasurements = measTaskOld.StationsForMeasurements
                                                        };
                                                        //M = mts_[0];
                                                    }
                                                }

                                                LM_SDR = M.CreateeasTaskSDRsApi1_0(ActionType);
                                                if (LM_SDR != null)
                                                {

                                                    string ids = "";
                                                    int MaxVal = cl.GetMaXIdsSdrTasks(M);
                                                    int idx = MaxVal + 1;
                                                    foreach (var mx in LM_SDR.ToArray())
                                                    {
                                                        mx.Id = idx;
                                                        ids = idx.ToString() + ";";
                                                        if (mx.SensorId.Value == SensorId)
                                                        {
                                                            /// Перед отправкой включаем валидацию созданных объектов MeasTaskSDR
                                                            if (mx.ValidationMeas1_0())
                                                            {
                                                                Checked_L.Add(mx);
                                                                M.UpdateStatus(ActionType);
                                                            }
                                                            else
                                                            {
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
                                            if (apiVer == "v2.0")
                                            {
                                                if (ActionType == "New")
                                                {
                                                    M.CreateeasTaskSDRsApi2_0(fnd_s.Name, GlobalInit.NameServer, fnd_s.Equipment.TechId, IdTsk, ActionType);
                                                    IdTsk = Create_New_Meas_Task(M, "New");
                                                    var mts_ = ts.ConvertToShortMeasTasks(cl.ShortReadTask(IdTsk.Value)).ToList();
                                                    if (mts_.Count() > 0)
                                                    {
                                                        var measTaskOld = M;
                                                        M = new MeasTask()
                                                        {
                                                            CreatedBy = mts_[0].CreatedBy,
                                                            DateCreated = mts_[0].DateCreated,
                                                            ExecutionMode = mts_[0].ExecutionMode,
                                                            Id = mts_[0].Id,
                                                            MaxTimeBs = mts_[0].MaxTimeBs,
                                                            MeasDtParam = mts_[0].MeasDtParam,
                                                            MeasFreqParam = mts_[0].MeasFreqParam,
                                                            MeasLocParams = mts_[0].MeasLocParams,
                                                            MeasOther = mts_[0].MeasOther,
                                                            MeasSubTasks = mts_[0].MeasSubTasks,
                                                            MeasTimeParamList = mts_[0].MeasTimeParamList,
                                                            Name = mts_[0].Name,
                                                            OrderId = mts_[0].OrderId,
                                                            Prio = mts_[0].Prio,
                                                            ResultType = mts_[0].ResultType,
                                                            Status = mts_[0].Status,
                                                            Task = mts_[0].Task,
                                                            Type = mts_[0].Type,
                                                            Stations = measTaskOld.Stations,
                                                            StationsForMeasurements = measTaskOld.StationsForMeasurements
                                                        };
                                                        //M = mts_[0];
                                                    }
                                                }
                                                else IdTsk = mt.Id.Value;
                                                LM_SDR_Device = M.CreateeasTaskSDRsApi2_0(fnd_s.Name, GlobalInit.NameServer, fnd_s.Equipment.TechId, IdTsk, ActionType);
                                                if (LM_SDR_Device != null)
                                                {

                                                    string ids = "";
                                                    int MaxVal = cl.GetMaXIdsSdrTasks(M);
                                                    int idx = MaxVal + 1;
                                                    foreach (var mx in LM_SDR_Device.ToArray())
                                                    {
                                                        ids = idx.ToString() + ";";
                                                        if (mx.SensorName == fnd_s.Name)
                                                        {
                                                            /// Перед отправкой включаем валидацию созданных объектов MeasTaskSDR
                                                            if (mx.ValidationMeas2_0())
                                                            {
                                                                Checked_L_Device.Add(mx);
                                                                M.UpdateStatus(ActionType);
                                                            }
                                                            else
                                                            {
                                                                M.UpdateStatus(ActionType);
                                                                cl.SaveStatusTaskToDB(M);
                                                            }
                                                        }
                                                        idx++;
                                                    }
                                                    if (ids.Length > 0) ids = ids.Remove(ids.Count() - 1, 1);
                                                    cl.SaveIdsSdrTasks(M, ids);
                                                }
                                            }
                                        }
                                        M.MeasSubTasks = msbd_old;
                                    }

                                  

                                    //Отправка тасков в очередь специфичную для версии API 2.0
                                    if (Checked_L.Count > 0)
                                    {
                                        string Queue = $"{GlobalInit.StartNameQueueDevice}.[{fnd_s.Name}].[{fnd_s.Equipment.TechId}].[{apiVer}]";
                                        BusManager<List<MeasSdrTask>> busManager = new BusManager<List<MeasSdrTask>>();
                                        if (ActionType == "Stop")
                                        {
                                            if (busManager.SendDataToDeviceCrypto<List<MeasSdrTask>>("StopMeasSdrTask", Checked_L, fnd_s.Name, fnd_s.Equipment.TechId, "v2.0", Guid.NewGuid().ToString()))
                                            {
                                                isSendSuccess = true;
                                                isSuccessTemp = true;
                                            }
                                        }
                                        else
                                        {
                                            if (busManager.SendDataToDeviceCrypto<List<MeasSdrTask>>("SendMeasSdrTask", Checked_L, fnd_s.Name, fnd_s.Equipment.TechId, "v2.0", Guid.NewGuid().ToString()))
                                            {
                                                isSendSuccess = true;
                                            }
                                            else
                                            {
                                                isSendSuccess = false;
                                            }
                                        }
                                        Checked_L.Clear();
                                    }


                                    //Отправка тасков в очередь специфичную для версии API 2.0
                                    if (Checked_L_Device.Count > 0)
                                    {
                                        string Queue = $"{GlobalInit.StartNameQueueDevice}.[{fnd_s.Name}].[{fnd_s.Equipment.TechId}].[{apiVer}]";
                                        BusManager<List<Atdi.DataModels.Sdrns.Device.MeasTask>> busManager = new BusManager<List<Atdi.DataModels.Sdrns.Device.MeasTask>>();
                                        if (ActionType == "Stop")
                                        {
                                            foreach (Atdi.DataModels.Sdrns.Device.MeasTask task in Checked_L_Device)
                                            {
                                                if (busManager.SendDataToDeviceCrypto<Atdi.DataModels.Sdrns.Device.MeasTask>("StopMeasTask", task, fnd_s.Name, fnd_s.Equipment.TechId, "v2.0", Guid.NewGuid().ToString()))
                                                {
                                                    isSendSuccess = true;
                                                    isSuccessTemp = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (Atdi.DataModels.Sdrns.Device.MeasTask task in Checked_L_Device)
                                            {
                                                if (busManager.SendDataToDeviceCrypto<Atdi.DataModels.Sdrns.Device.MeasTask>("SendMeasTask", task, fnd_s.Name, fnd_s.Equipment.TechId, "v2.0", Guid.NewGuid().ToString()))
                                                {
                                                    isSendSuccess = true;
                                                }
                                                else
                                                {
                                                    isSendSuccess = false;
                                                }
                                            }
                                        }
                                        Checked_L_Device.Clear();
                                    }
                                }
                            }
                        }
                    }
                    logger.Trace("End procedure Process_Multy_Meas.");
                });
                tsk_main.Start();
                tsk_main.Join();
                IdTask = IdTsk;
                isSuccess = isSuccessTemp;
            }
            catch (Exception ex)
            {
                logger.Error("Error procedure Process_Multy_Meas: " + ex.Message);
            }
            return isSendSuccess;


        }



    }
}
