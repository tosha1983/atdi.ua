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
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class ShedulerCheckStatusF : InterfaceShedulerWithParams, IDisposable
    {
        public static ILogger logger;
        public ShedulerCheckStatusF(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ShedulerCheckStatusF()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ISchedulerFactory schedFact = new StdSchedulerFactory();
        public void ShedulerRepeatStart(int CountSeconds, object sens)
        {
            IScheduler sched = schedFact.GetScheduler();
            if (sched.IsStarted == false) sched.Start();
            IJobDetail job = JobBuilder.Create<SimpleJob>()
                    .WithIdentity(ShedulerStatic.Name_job_CheckStatusF, ShedulerStatic.Name_group_CheckStatusF)
                    .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(ShedulerStatic.Name_trigger_CheckStatusF, ShedulerStatic.Name_group_CheckStatusF)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(CountSeconds).RepeatForever())
                .Build();
            sched.Context.Add(new KeyValuePair<string, object>("val_check", sens));
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
                logger.Trace("Start job ShedulerCheckStatusF...");
                object val = context.Scheduler.Context["val_check"];
                    if (val != null)
                    {
                        if ((val as Sensor).Status == AllStatusSensor.F.ToString())
                        {
                            if (GlobalInit.SensorListSDRNS.Count > 0)
                            {
                                Sensor fnd = GlobalInit.SensorListSDRNS.Find(t => t.Name == (val as Sensor).Name && t.Equipment.TechId == (val as Sensor).Equipment.TechId);
                                if (fnd != null)
                                    GlobalInit.SensorListSDRNS.ReplaceAll<Sensor>(fnd, (val as Sensor));
                                else GlobalInit.SensorListSDRNS.Add((val as Sensor));
                                //GlobalInit.SensorListSDRNS.RemoveAll(t => t.Name == (val as Sensor).Name && t.Equipment.TechId == (val as Sensor).Equipment.TechId);
                                (val as Sensor).Status = AllStatusSensor.Z.ToString();
                                //GlobalInit.SensorListSDRNS.Add((val as Sensor));
                                ClassDBGetSensor sens_db = new ClassDBGetSensor(logger);
                                sens_db.CreateNewObjectSensor((val as Sensor));
                                sens_db.Dispose();
                            }
                        }
                    }
                    System.GC.Collect();
                logger.Trace("End job ShedulerCheckStatusF.");
            }
            /// <summary>
            /// Dispose sheduler
            /// </summary>
            public static void DisposeSheduler()
            {
                ISchedulerFactory schedFact = new StdSchedulerFactory();
                IScheduler sched = schedFact.GetScheduler();
                if (sched.GetTriggersOfJob(new JobKey(ShedulerStatic.Name_job_CheckStatusF, ShedulerStatic.Name_group_CheckStatusF)).Count > 0)
                {
                    sched.DeleteJob(new JobKey(ShedulerStatic.Name_job_CheckStatusF, ShedulerStatic.Name_group_CheckStatusF));
                    sched.UnscheduleJob(new TriggerKey(ShedulerStatic.Name_trigger_CheckStatusF, ShedulerStatic.Name_group_CheckStatusF));
                }
            }

        }
    }
}
