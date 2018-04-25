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
using Atdi.SDR.Server.Utils;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class Sheduler_ArchiveSDRResults : InterfaceSheduler, IDisposable
    {
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~Sheduler_ArchiveSDRResults()
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
                    .WithIdentity(ShedulerStatic.Name_job_meas_sdr_results_arch, ShedulerStatic.Name_group_meas_sdr_results_arch)
                    .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(ShedulerStatic.Name_trigger_meas_sdr_results_arch, ShedulerStatic.Name_group_meas_sdr_results_arch)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(CountSeconds).RepeatForever())
                .Build();
            TriggerState state = sched.GetTriggerState(trigger.Key);
            if (state == TriggerState.None) sched.ScheduleJob(job, trigger);
            else {
                sched.Clear(); sched = schedFact.GetScheduler();
                if (sched.IsStarted == false) sched.Start(); 
            }
        }



        public class SimpleJob : IJob
        {
            void IJob.Execute(IJobExecutionContext context)
            {
                //Task stx = new Task(() =>
                //{
                    //ClassesDBGetResult DbGetRes = new ClassesDBGetResult();
                    //ClassConvertToSDRResults conv = new ClassConvertToSDRResults();
                    //MeasurementResults[] msResltConv = conv.ConvertTo_SDRObjects(DbGetRes.ReadlAllResultFromDB());
                    //if (msResltConv != null)
                    //{
                        //foreach (MeasurementResults it in msResltConv.ToArray())
                        //{
                            //все MeasSDRResults для которых прошло время = 2 * TimeArchiveResult, и которые имеют статус C надо архивировать
                            //if (it.Status == "C")
                            //{
                                //if (it.FindAndDestroyObject(2 * XMLLibrary.BaseXMLConfiguration.xml_conf._TimeArchiveResult, "C"))
                                    //DbGetRes.DeleteResultFromDB(it, "Z");
                            //}
                        //}
                    //}
                //});
                ///stx.Start();
            }
        }
            
        
        /// <summary>
        /// Dispose sheduler
        /// </summary>
        public static void DisposeSheduler()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler();
            if (sched.GetTriggersOfJob(new JobKey(ShedulerStatic.Name_job_meas_sdr_results_arch, ShedulerStatic.Name_group_meas_sdr_results_arch)).Count > 0)
            {
                sched.DeleteJob(new JobKey(ShedulerStatic.Name_job_meas_sdr_results_arch, ShedulerStatic.Name_group_meas_sdr_results_arch));
                sched.UnscheduleJob(new TriggerKey(ShedulerStatic.Name_trigger_meas_sdr_results_arch, ShedulerStatic.Name_group_meas_sdr_results_arch));
            }
        }

    }
}
