using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Atdi.SDNRS.AppServer;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.AppServer.Contracts.Sdrns;
using EasyNetQ;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class ShedulerGetMeasTask: InterfaceSheduler, IDisposable
    {
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ShedulerGetMeasTask()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        ISchedulerFactory schedFact = new StdSchedulerFactory();
        // Инициализация планировщика для механизма извлечения данных о сенсорах
        public void ShedulerRepeatStart(int CountSeconds)
        {
            IScheduler sched = schedFact.GetScheduler();
            if (sched.IsStarted == false) sched.Start();
            IJobDetail job = JobBuilder.Create<SimpleJob>()
                    .WithIdentity(ShedulerStatic.Name_job_meas_task_submit, ShedulerStatic.Name_group_meas_task_submit)
                    .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(ShedulerStatic.Name_trigger_meas_task_submit, ShedulerStatic.Name_group_meas_task_submit)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(CountSeconds).RepeatForever())
                .Build();
            TriggerState state = sched.GetTriggerState(trigger.Key);
            if (state == TriggerState.None) sched.ScheduleJob(job, trigger);
            else {
                sched.Clear(); sched = schedFact.GetScheduler();
                if (sched.IsStarted == false) sched.Start(); 
            }
        }

        // Инициализация основного обработчика 
        // здесь выполняется загрузка данных из БД о сенсорах с заданной периодичностью 1 минута
        public class SimpleJob : IJob
        {
            ClassesDBGetTasks cl = new ClassesDBGetTasks();
            //ClassConvertTasks ts = new ClassConvertTasks();
            void IJob.Execute(IJobExecutionContext context)
            {
                //foreach (IDisposable d in GlobalInit.Lds_Activity_MEAS_TASK_SDR_Main_List_SDR) d.SafeDispose();
                //Task stx = new Task(() =>
                //{
                try
                {
                     BusManager<List<MeasSdrTask>> busManager = new BusManager<List<MeasSdrTask>>();
                      foreach (Sensor s in GlobalInit.SensorListSDRNS.ToArray())
                        {
                            if (ClassStaticBus.bus.Advanced.IsConnected)
                            {
                            uint cnt = busManager.GetMessageCount(GlobalInit.Template_MEAS_TASK_SDR_Main_List_SDR + s.Name + s.Equipment.TechId);
                            for (int i = 0; i < cnt; i++)
                            {
                                var message = busManager.GetDataObject(GlobalInit.Template_MEAS_TASK_SDR_Main_List_SDR + s.Name + s.Equipment.TechId);
                                if (message != null)
                                {
                                        List<MeasSdrTask> fnd_s = message as List<MeasSdrTask>;
                                        if (fnd_s != null)
                                        {
                                            foreach (MeasSdrTask h in fnd_s)
                                            {
                                                if (h.MeasTaskId != null)
                                                {
                                                    MeasTask tsk = GlobalInit.LIST_MEAS_TASK.Find(t => t.Id.Value == h.MeasTaskId.Value);
                                                    if (tsk != null)
                                                    {
                                                        tsk.Status = h.status;
                                                        cl.SaveStatusTaskToDB(tsk);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                else break;
                            }
                                /*
                                    GlobalInit.Lds_Activity_MEAS_TASK_SDR_Main_List_SDR.Add(ClassStaticBus.bus.Receive(GlobalInit.Template_MEAS_TASK_SDR_Main_List_SDR + s.Name + s.Equipment.TechId, x => x
                                    .Add<List<MeasSdrTask>>(message =>
                                    {
                                        List<MeasSdrTask> fnd_s = message;
                                        if (fnd_s != null)
                                        {
                                            foreach (MeasSdrTask h in fnd_s)
                                            {
                                                if (h.MeasTaskId != null)
                                                {
                                                    MeasTask tsk = GlobalInit.LIST_MEAS_TASK.Find(t => t.Id.Value == h.MeasTaskId.Value);
                                                    if (tsk != null)
                                                    {
                                                        tsk.Status = h.status;
                                                        cl.SaveStatusTaskToDB(tsk);
                                                    }
                                                }
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
                        }
                        CoreICSM.Logs.CLogs.WriteInfo(CoreICSM.Logs.ELogsWhat.Unknown, "ShedulerGetMeasTask ");
                    }
                    catch (Exception ex)
                    {
                        CoreICSM.Logs.CLogs.WriteError(CoreICSM.Logs.ELogsWhat.Unknown, "ShedulerGetMeasTask " + ex.Message);
                    }
                    cl.Dispose();
                    //});
                    //stx.Start();
                    //stx.Wait();
                    System.GC.Collect();
            }
        }

        /// <summary>
        /// Dispose sheduler
        /// </summary>
        public static void DisposeSheduler()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler();
            if (sched.GetTriggersOfJob(new JobKey(ShedulerStatic.Name_job_meas_task_submit, ShedulerStatic.Name_group_meas_task_submit)).Count > 0) {
                sched.DeleteJob(new JobKey(ShedulerStatic.Name_job_meas_task_submit, ShedulerStatic.Name_group_meas_task_submit));
                sched.UnscheduleJob(new TriggerKey(ShedulerStatic.Name_trigger_meas_task_submit, ShedulerStatic.Name_group_meas_task_submit));
            }
            sched.Shutdown();
        }
    }
}
