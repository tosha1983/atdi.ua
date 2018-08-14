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
using Atdi.AppServer;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class Sheduler_Up_Meas_SDR_Results : InterfaceSheduler, IDisposable
    {
        public static ILogger logger;
        public Sheduler_Up_Meas_SDR_Results(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~Sheduler_Up_Meas_SDR_Results()
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
                    .WithIdentity(ShedulerStatic.Name_job_meas_sdr_results, ShedulerStatic.Name_group_meas_sdr_results)
                    .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(ShedulerStatic.Name_trigger_meas_sdr_results, ShedulerStatic.Name_group_meas_sdr_results)
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
            public static List<MeasSdrResults> ConfirmRes = new List<MeasSdrResults>();
            void IJob.Execute(IJobExecutionContext context)
            {
                try
                {
                    logger.Trace("Start job Sheduler_Up_Meas_SDR_Results...");
                    BusManager<List<MeasSdrResults>> BS = new BusManager<List<MeasSdrResults>>();
                    foreach (Sensor s in GlobalInit.SensorListSDRNS.ToArray()) {
                        if (ClassStaticBus.bus.Advanced.IsConnected) {
                            uint MessCount = BS.GetMessageCount(GlobalInit.Template_MEAS_SDR_RESULTS_Main_List_APPServer + s.Name + s.Equipment.TechId);
                            for (int i=0; i< MessCount; i++) {
                                var rs = BS.GetDataObject(GlobalInit.Template_MEAS_SDR_RESULTS_Main_List_APPServer + s.Name + s.Equipment.TechId);
                                if (rs != null)
                                    GlobalInit.MEAS_SDR_RESULTS.AddRange(rs as List<MeasSdrResults>);
                                else {
                                    break;
                                }
                            }
                        }
                        else {
                            ClassStaticBus.bus.Dispose();
                            GC.SuppressFinalize(ClassStaticBus.bus);
                            ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                        }

                    }
                    logger.Trace("End job Sheduler_Up_Meas_SDR_Results.");
                 }
                catch (Exception ex)
                {
                    logger.Error("Error in job  Sheduler_Up_Meas_SDR_Results: "+ex.Message);
                    //CoreICSM.Logs.CLogs.WriteError(CoreICSM.Logs.ELogsWhat.Unknown, "Sheduler_Up_Meas_SDR_Results " +ex.Message);
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
            if (sched.GetTriggersOfJob(new JobKey(ShedulerStatic.Name_job_meas_sdr_results, ShedulerStatic.Name_group_meas_sdr_results)).Count > 0)
            {
                sched.DeleteJob(new JobKey(ShedulerStatic.Name_job_meas_sdr_results, ShedulerStatic.Name_group_meas_sdr_results));
                sched.UnscheduleJob(new TriggerKey(ShedulerStatic.Name_trigger_meas_sdr_results, ShedulerStatic.Name_group_meas_sdr_results));
            }
        }

    }
}
