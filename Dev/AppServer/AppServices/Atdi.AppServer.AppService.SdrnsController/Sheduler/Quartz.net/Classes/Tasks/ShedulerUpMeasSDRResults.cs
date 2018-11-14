using System;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using Quartz.Impl;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer;
using RabbitMQ.Client;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class ShedulerUpMeasSDRResults : InterfaceSheduler, IDisposable
    {
        public static ILogger logger;
        public ShedulerUpMeasSDRResults(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ShedulerUpMeasSDRResults()
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
            void IJob.Execute(IJobExecutionContext context)
            {
               
                logger.Trace("Start job Sheduler_Up_Meas_SDR_Results... ");
                context.Scheduler.PauseAll();
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        BusManager<List<MeasSdrResults>> BS = new BusManager<List<MeasSdrResults>>();
                        ClassDBGetSensor gsd = new ClassDBGetSensor(logger);
                        List<Sensor> SensorListSDRNS = gsd.LoadObjectAllSensor();
                        foreach (Sensor s in SensorListSDRNS.ToArray())
                        {
                            if (ClassStaticBus.factory != null)
                            {
                                uint MessCount = BS.GetMessageCount(GlobalInit.Template_MEAS_SDR_RESULTS_Main_List_APPServer + s.Name + s.Equipment.TechId);
                                for (int j = 0; j < MessCount; j++)
                                {
                                    var rs = BS.GetDataObject<List<MeasSdrResults>>(GlobalInit.Template_MEAS_SDR_RESULTS_Main_List_APPServer + s.Name + s.Equipment.TechId);
                                    if (rs != null)
                                    {
                                        List<MeasSdrResults> MEAS_SDR_RESULTS = rs as List<MeasSdrResults>;
                                        ClassesDBGetResult DbGetRes = new ClassesDBGetResult(logger);
                                        ClassConvertToSDRResults conv = new ClassConvertToSDRResults(logger);
                                        for (int i = 0; i < MEAS_SDR_RESULTS.Count; i++)
                                        {
                                            if (MEAS_SDR_RESULTS.Count > 0)
                                            {
                                                if (MEAS_SDR_RESULTS[0] != null)
                                                {
                                                    int? ID = -1;
                                                    string Status_Original = MEAS_SDR_RESULTS[0].status;
                                                    MeasurementResults msReslts = ClassConvertToSDRResults.GenerateMeasResults(MEAS_SDR_RESULTS[0]);
                                                    if (msReslts.TypeMeasurements == MeasurementType.SpectrumOccupation) msReslts.Status = Status_Original;
                                                    if (msReslts.MeasurementsResults != null)
                                                    {
                                                        if (msReslts.MeasurementsResults.Count() > 0)
                                                        {
                                                            if (msReslts.MeasurementsResults[0] is LevelMeasurementOnlineResult)
                                                            {
                                                                msReslts.Status = "O";
                                                                logger.Trace(string.Format("Start save online results..."));
                                                                ID = DbGetRes.SaveResultToDB(msReslts);
                                                                if (ID > 0)
                                                                {
                                                                    logger.Trace(string.Format("Success save online results..."));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                logger.Trace(string.Format("Start save results..."));
                                                                ID = DbGetRes.SaveResultToDB(msReslts);
                                                                if (ID > 0)
                                                                {
                                                                    logger.Trace(string.Format("Success save results..."));
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        logger.Trace(string.Format("Start save results..."));
                                                        ID = DbGetRes.SaveResultToDB(msReslts);
                                                        if (ID > 0)
                                                        {
                                                            logger.Trace(string.Format("Success save results..."));
                                                        }
                                                    }
                                                    //GlobalInit.blockingCollectionMeasurementResults.TryAdd(msReslts);
                                                }
                                            }
                                            DbGetRes.Dispose();
                                            conv.Dispose();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ClassStaticBus.factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword, VirtualHost = GlobalInit.RabbitVirtualHost, SocketReadTimeout = 2147000000, SocketWriteTimeout = 2147000000 };
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error in job  Sheduler_Up_Meas_SDR_Results: " + ex.Message);
                    }
                });
                thread.Start();
                thread.Join();
                logger.Trace("End job Sheduler_Up_Meas_SDR_Results.");
                context.Scheduler.ResumeAll();
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
