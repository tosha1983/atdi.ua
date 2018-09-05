using System;
using Quartz;
using Quartz.Impl;
using Atdi.SDNRS.AppServer.BusManager;
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
                logger.Trace("Start job ShedulerCheckActivitySensor...");
                context.Scheduler.PauseAll();
                try
                {
                    SensorListSDRNS senLst = new SensorListSDRNS(logger);
                    senLst.CheckActivitySensor();
                    System.GC.Collect();
                }
                catch (Exception ex)
                {
                    logger.Error("Error in job ShedulerCheckActivitySensor: " + ex.Message);
                }
                context.Scheduler.ResumeAll();
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
