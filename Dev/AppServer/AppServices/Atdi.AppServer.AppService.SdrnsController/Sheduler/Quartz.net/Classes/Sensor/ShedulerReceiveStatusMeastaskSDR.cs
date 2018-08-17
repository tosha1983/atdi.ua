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
using Atdi.SDNRS.AppServer.ManageDB;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.SDR.Server.Utils;
using Atdi.AppServer;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class ShedulerReceiveStatusMeastaskSDR : InterfaceSheduler, IDisposable
    {
        public static ILogger logger;
        public ShedulerReceiveStatusMeastaskSDR(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ShedulerReceiveStatusMeastaskSDR()
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
                    .WithIdentity(ShedulerStatic.Name_job_submit_list_queus_submit, ShedulerStatic.Name_group_submit_list_queus_submit)
                    .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(ShedulerStatic.Name_trigger_submit_list_queus_submit, ShedulerStatic.Name_group_submit_list_queus_submit)
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
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            void IJob.Execute(IJobExecutionContext context)  {
                logger.Trace("Start job ShedulerReceiveStatusMeastaskSDR... ");
                context.Scheduler.PauseAll();
                try
                {
                    ClassesDBGetResult DbGetRes = new ClassesDBGetResult(logger);
                    if (GlobalInit.MEAS_SDR_RESULTS.Count > 0)
                    {
                        context.Scheduler.PauseAll();
                        DbGetRes.SaveAllResultsToDB();
                        context.Scheduler.ResumeAll();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error in job ShedulerReceiveStatusMeastaskSDR: " + ex.Message);
                }
                context.Scheduler.ResumeAll();
                logger.Trace("End job ShedulerReceiveStatusMeastaskSDR.");
            }
        }
        /// <summary>
        /// Dispose sheduler
        /// </summary>
        public static void DisposeSheduler()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler();
            if (sched.GetTriggersOfJob(new JobKey(ShedulerStatic.Name_job_submit_list_queus_submit, ShedulerStatic.Name_group_submit_list_queus_submit)).Count > 0)
            {
                sched.DeleteJob(new JobKey(ShedulerStatic.Name_job_submit_list_queus_submit, ShedulerStatic.Name_group_submit_list_queus_submit));
                sched.UnscheduleJob(new TriggerKey(ShedulerStatic.Name_trigger_submit_list_queus_submit, ShedulerStatic.Name_group_submit_list_queus_submit));
            }
            sched.Shutdown();
        }

    }
}
