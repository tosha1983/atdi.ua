using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using EasyNetQ.Consumer;
using EasyNetQ;
using EasyNetQ.Events;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using XMLLibrary;
using Atdi.AppServer;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class ShedulerCheckActivitySensor : InterfaceSheduler, IDisposable
    {
        public static ILogger logger;
        public ShedulerCheckActivitySensor(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ShedulerCheckActivitySensor()
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
                    .WithIdentity(ShedulerStatic.Name_job_CheckActivitySensor, ShedulerStatic.Name_group_CheckActivitySensor)
                    .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(ShedulerStatic.Name_trigger_CheckActivitySensor, ShedulerStatic.Name_group_CheckActivitySensor)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(CountSeconds).RepeatForever())
                .Build();
            TriggerState state = sched.GetTriggerState(trigger.Key);
            if (state == TriggerState.None) sched.ScheduleJob(job, trigger);
            else {
                sched.Clear(); sched = schedFact.GetScheduler();
                if (sched.IsStarted == false) sched.Start(); 
            }
        }

        // Инициализация обработчика 
        // здесь выполняется периодическое обновление статуса активности (доступности) сенсора
        public class SimpleJob : IJob
        {
            void IJob.Execute(IJobExecutionContext context)
            {
                //foreach (IDisposable d in GlobalInit.Lds_Activity_Sensor_Receiver) d.SafeDispose();
                logger.Trace("Start job ShedulerCheckActivitySensor...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
                    SensorListSDRNS senLst = new SensorListSDRNS(logger);
                    senLst.CheckActivitySensor();
                    senLst.Dispose();
                });
                tsk.Start();
                tsk.Join();

                System.GC.Collect();
                logger.Trace("End job ShedulerCheckActivitySensor.");
            }
          
        }
        /// <summary>
        /// Dispose sheduler
        /// </summary>
        public static void DisposeSheduler()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler();
            if (sched.GetTriggersOfJob(new JobKey(ShedulerStatic.Name_job_CheckActivitySensor, ShedulerStatic.Name_group_CheckActivitySensor)).Count > 0)
            {
                sched.DeleteJob(new JobKey(ShedulerStatic.Name_job_CheckActivitySensor, ShedulerStatic.Name_group_CheckActivitySensor));
                sched.UnscheduleJob(new TriggerKey(ShedulerStatic.Name_trigger_CheckActivitySensor, ShedulerStatic.Name_group_CheckActivitySensor));
            }
        }

    }

}
