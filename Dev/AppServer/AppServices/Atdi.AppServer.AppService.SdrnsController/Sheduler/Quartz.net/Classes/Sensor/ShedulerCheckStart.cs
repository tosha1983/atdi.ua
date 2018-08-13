using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using EasyNetQ.Consumer;
using EasyNetQ;
using Atdi.SDNRS.AppServer.BusManager;


namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class ShedulerCheckStart : InterfaceSheduler, IDisposable
    {
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ShedulerCheckStart()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ISchedulerFactory schedFact = new StdSchedulerFactory();
        public void ShedulerRepeatStart(int CountSeconds)
        {
            IScheduler sched = schedFact.GetScheduler();
            if (sched.IsStarted == false) sched.Start();
            IJobDetail job = JobBuilder.Create<SimpleJob>()
                    .WithIdentity(ShedulerStatic.Name_job_device_submit, ShedulerStatic.Name_group_device_submit)
                    .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(ShedulerStatic.Name_trigger_device_submit, ShedulerStatic.Name_group_device_submit)
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
            void IJob.Execute(IJobExecutionContext context)
            {
                try
                {
                        System.Threading.Thread tsk = new System.Threading.Thread(() =>
                        {
                        //CoreICSM.Logs.CLogs.WriteInfo(CoreICSM.Logs.ELogsWhat.Unknown, "New check list sensors ");
                        Console.WriteLine("--> New check list sensors");
                        ClassReceiveAllSensors rec = new ClassReceiveAllSensors();
                        rec.ReceiveAllSensorList();
                        rec.Dispose();
                        });
                        tsk.Start();
                        //tsk.Join();
                        System.GC.Collect();
                }
                catch (Exception ex)
                {
                    //CoreICSM.Logs.CLogs.WriteError(CoreICSM.Logs.ELogsWhat.Unknown, "ShedulerCheckStart "+ex.Message);
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
            if (sched.GetTriggersOfJob(new JobKey(ShedulerStatic.Name_job_device_submit, ShedulerStatic.Name_group_device_submit)).Count > 0) {
                sched.DeleteJob(new JobKey(ShedulerStatic.Name_job_device_submit, ShedulerStatic.Name_group_device_submit));
                sched.UnscheduleJob(new TriggerKey(ShedulerStatic.Name_trigger_device_submit, ShedulerStatic.Name_group_device_submit));
            }
        }

    }
}
