using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.SDNRS.AppServer.ManageDB;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using EasyNetQ.Consumer;
using EasyNetQ;
using Atdi.AppServer.Contracts.Sdrns;


namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class Sheduler_Send_MeasSdr : InterfaceShedulerMeasTask, IDisposable
    {
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~Sheduler_Send_MeasSdr()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ISchedulerFactory schedFact = new StdSchedulerFactory();
        public void ShedulerRepeatStart(int CountSeconds, object val_MeasTask, List<int> SensorIDS, string ActionType, bool isOnline)
        {
            IScheduler sched = schedFact.GetScheduler();
            if (sched.IsStarted == false) sched.Start();
            IJobDetail job = JobBuilder.Create<SimpleJob>()
                    .WithIdentity(ShedulerStatic.Name_job_meas_task_scan, ShedulerStatic.Name_group_meas_task_scan)
                    .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(ShedulerStatic.Name_trigger_meas_task_scan, ShedulerStatic.Name_group_meas_task_scan)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(CountSeconds).RepeatForever())
                .Build();
            sched.Context.Add(new KeyValuePair<string, object>("val_MeasTask", val_MeasTask));
            sched.Context.Add(new KeyValuePair<string, object>("val_SensorIds", SensorIDS));
            sched.Context.Add(new KeyValuePair<string, object>("val_ActionType", ActionType));
            sched.Context.Add(new KeyValuePair<string, object>("val_isOnline", isOnline));
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
            void IJob.Execute(IJobExecutionContext context)
            {
                try
                {
                    bool isScuccess = false;
                    object val_MeasTask = context.Scheduler.Context["val_MeasTask"];
                    object SensorIDS = context.Scheduler.Context["val_SensorIds"];
                    object val_ActionType = context.Scheduler.Context["val_ActionType"];
                    object val_isOnline = context.Scheduler.Context["val_isOnline"];
                    if ((val_MeasTask != null) && (SensorIDS != null) && (val_ActionType != null) && (val_isOnline != null))
                    {
                        Task tsk = new Task(() =>
                        {
                            isScuccess = WorkFlowProcessManageTasks.Process_Multy_Meas((MeasTask)val_MeasTask, (List<int>)SensorIDS, (string)val_ActionType, (bool)val_isOnline);
                        }).ContinueWith(x=>
                        {
                            if (isScuccess) DisposeSheduler();
                        });
                        System.GC.Collect();
                    }
                    CoreICSM.Logs.CLogs.WriteInfo(CoreICSM.Logs.ELogsWhat.Unknown, "Sheduler_Send_MeasSdr ");
                }
                catch (Exception ex) {
                    CoreICSM.Logs.CLogs.WriteError(CoreICSM.Logs.ELogsWhat.Unknown, "Sheduler_Send_MeasSdr "+ex.Message);
                }
            }
        }
        /// <summary>
        /// Dispose sheduler
        /// </summary>
        public static void DisposeSheduler()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler();
            if (sched.GetTriggersOfJob(new JobKey(ShedulerStatic.Name_job_meas_task_scan, ShedulerStatic.Name_group_meas_task_scan)).Count > 0)
            {
                sched.DeleteJob(new JobKey(ShedulerStatic.Name_job_meas_task_scan, ShedulerStatic.Name_group_meas_task_scan));
                sched.UnscheduleJob(new TriggerKey(ShedulerStatic.Name_trigger_meas_task_scan, ShedulerStatic.Name_group_meas_task_scan));
            }
        }

    }
}
