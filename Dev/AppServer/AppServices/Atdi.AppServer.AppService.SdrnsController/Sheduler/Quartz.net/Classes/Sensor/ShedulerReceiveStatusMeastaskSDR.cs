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
           void IJob.Execute(IJobExecutionContext context)  {
                try
                {
                    ClassesDBGetResult DbGetRes = new ClassesDBGetResult(logger);
                    /*
                    logger.Trace("Start job ShedulerReceiveStatusMeastaskSDR...");
                    if (GlobalInit.MEAS_SDR_RESULTS.Count > 0)
                    {
                        ClassesDBGetResult DbGetRes = new ClassesDBGetResult(logger);
                        ClassConvertToSDRResults conv = new ClassConvertToSDRResults(logger);
                        System.Threading.Thread ge = new System.Threading.Thread(() =>
                        {
                        for (int i = 0; i < GlobalInit.MEAS_SDR_RESULTS.Count; i++)
                        {
                           
                                if (GlobalInit.MEAS_SDR_RESULTS.Count > 0)
                                {
                                    if (GlobalInit.MEAS_SDR_RESULTS[0] != null)
                                    {
                                        int ID = -1;
                                        string Status_Original = GlobalInit.MEAS_SDR_RESULTS[0].status;
                                        MeasurementResults msReslts = ClassConvertToSDRResults.GenerateMeasResults(GlobalInit.MEAS_SDR_RESULTS[0]);
                                        if (msReslts.TypeMeasurements == MeasurementType.SpectrumOccupation) msReslts.Status = Status_Original;
                                        if (msReslts.MeasurementsResults != null)
                                        {
                                            if (msReslts.MeasurementsResults.Count() > 0)
                                            {
                                                if (msReslts.MeasurementsResults[0] is LevelMeasurementOnlineResult)
                                                {
                                                    // Здесь в базу ничего не пишем (только в память)
                                                    msReslts.Status = "O";
                                                    GlobalInit.LST_MeasurementResults.Add(msReslts);
                                                }
                                                else
                                                {
                                                    logger.Trace(string.Format("Start save results..."));
                                                    ID = DbGetRes.SaveResultToDB(msReslts);
                                                    if (ID > 0)
                                                    {
                                                        GlobalInit.LST_MeasurementResults.Add(msReslts);
                                                        if (GlobalInit.MEAS_SDR_RESULTS.Count > 0) GlobalInit.MEAS_SDR_RESULTS.Remove(GlobalInit.MEAS_SDR_RESULTS[0]);
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
                                                GlobalInit.LST_MeasurementResults.Add(msReslts);
                                                GlobalInit.MEAS_SDR_RESULTS.Remove(GlobalInit.MEAS_SDR_RESULTS[0]);
                                                logger.Trace(string.Format("Success save results..."));
                                            }
                                        }
                                    }
                                }
                          
                            logger.Trace(string.Format("LST_MeasurementResults count: {0}", GlobalInit.LST_MeasurementResults.Count()));
                            logger.Trace(string.Format("MEAS_SDR_RESULTS count: {0}", GlobalInit.MEAS_SDR_RESULTS.Count()));
                            DbGetRes.Dispose();
                            conv.Dispose();
                            //CoreICSM.Logs.CLogs.WriteInfo(CoreICSM.Logs.ELogsWhat.Unknown, "ShedulerReceiveStatusMeastaskSDR ");
                        }
                        });
                        ge.Start();
                        ge.Join();
                    }
                    logger.Trace("End job ShedulerReceiveStatusMeastaskSDR.");
                    */
                    context.Scheduler.PauseTrigger(context.Trigger.Key);
                    context.Scheduler.PauseJob(context.JobDetail.Key);
                    context.Scheduler.PauseAll();
                    DbGetRes.SaveAllResultsToDB();
                    //context.Scheduler.ResumeAll();
                }
                catch (Exception ex)
                {
                    logger.Error("Error in job ShedulerReceiveStatusMeastaskSDR: "+ex.Message);
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
            if (sched.GetTriggersOfJob(new JobKey(ShedulerStatic.Name_job_submit_list_queus_submit, ShedulerStatic.Name_group_submit_list_queus_submit)).Count > 0)
            {
                sched.DeleteJob(new JobKey(ShedulerStatic.Name_job_submit_list_queus_submit, ShedulerStatic.Name_group_submit_list_queus_submit));
                sched.UnscheduleJob(new TriggerKey(ShedulerStatic.Name_trigger_submit_list_queus_submit, ShedulerStatic.Name_group_submit_list_queus_submit));
            }
            sched.Shutdown();
        }

    }
}
