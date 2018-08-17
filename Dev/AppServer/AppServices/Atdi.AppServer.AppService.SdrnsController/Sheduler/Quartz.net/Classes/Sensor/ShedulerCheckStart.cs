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
using Atdi.AppServer;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class ShedulerCheckStart : InterfaceSheduler, IDisposable
    {
        public static ILogger logger;
        public ShedulerCheckStart(ILogger log)
        {
            if (logger == null) logger = log;
        }
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
                logger.Trace("Start job ShedulerCheckStart...");
                context.Scheduler.PauseAll();
                try
                {
                    ClassReceiveAllSensors rec = new ClassReceiveAllSensors(logger);
                    rec.ReceiveAllSensorList();
                    rec.Dispose();
                    System.GC.Collect();
                }
                catch (Exception ex)
                {
                    logger.Error("Error in job ShedulerCheckStart: " + ex.Message);
                }
                context.Scheduler.ResumeAll();
                logger.Trace("End job ShedulerCheckStart.");
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
