using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Atdi.SDNRS.AppServer;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.AppServer.Contracts.Sdrns;
using EasyNetQ;
using Atdi.AppServer;
using XMLLibrary;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class ShedulerGetMeasTask: InterfaceSheduler, IDisposable
    {
        public static ILogger logger;
        public ShedulerGetMeasTask(ILogger log)
        {
            if (logger == null) logger = log;
        }

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ShedulerGetMeasTask()
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
                    .WithIdentity(ShedulerStatic.Name_job_meas_task_submit, ShedulerStatic.Name_group_meas_task_submit)
                    .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(ShedulerStatic.Name_trigger_meas_task_submit, ShedulerStatic.Name_group_meas_task_submit)
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
                logger.Trace("Start job ShedulerGetMeasTask... ");
                context.Scheduler.PauseAll();
                try
                {
                    System.Threading.Thread thread = new System.Threading.Thread(() =>
                    {
                        ClassesDBGetTasks cl = new ClassesDBGetTasks(logger);
                        BusManager<List<MeasSdrTask>> busManager = new BusManager<List<MeasSdrTask>>();
                        ClassConvertTasks ts = new ClassConvertTasks(logger);
                        ClassDBGetSensor gsd = new ClassDBGetSensor(logger);
                        List<Sensor> SensorListSDRNS = gsd.LoadObjectAllSensor();
                        foreach (Sensor s in SensorListSDRNS.ToArray())
                        {
                            if (ClassStaticBus.bus.Advanced.IsConnected)
                            {
                                uint cnt = busManager.GetMessageCount(GlobalInit.Template_MEAS_TASK_SDR_Main_List_SDR + s.Name + s.Equipment.TechId);
                                for (int i = 0; i < cnt; i++)
                                {
                                    var message = busManager.GetDataObject(GlobalInit.Template_MEAS_TASK_SDR_Main_List_SDR + s.Name + s.Equipment.TechId);
                                    if (message != null)
                                    {
                                        List<MeasSdrTask> fnd_s = message as List<MeasSdrTask>;
                                        if (fnd_s != null)
                                        {
                                            foreach (MeasSdrTask h in fnd_s)
                                            {
                                                if (h.MeasTaskId != null)
                                                {
                                                    MeasTask[] ResMeasTasks = ts.ConvertToShortMeasTasks(cl.ShortReadTask(h.MeasTaskId.Value));
                                                    MeasTask tsk = ResMeasTasks.ToList().Find(t => t.Id.Value == h.MeasTaskId.Value);
                                                    if (tsk != null)
                                                    {
                                                        tsk.Status = h.status;
                                                        cl.SaveStatusTaskToDB(tsk);
                                                        //GlobalInit.blockingCollectionMeasTask.TryUpdate(h.Id, tsk, new MeasTask { CreatedBy = tsk.CreatedBy, DateCreated = tsk.DateCreated, ExecutionMode = tsk.ExecutionMode, Id = tsk.Id, MaxTimeBs = tsk.MaxTimeBs, MeasDtParam = tsk.MeasDtParam, MeasFreqParam = tsk.MeasFreqParam, MeasLocParams = tsk.MeasLocParams, MeasOther = tsk.MeasOther, MeasSubTasks = tsk.MeasSubTasks, MeasTimeParamList = tsk.MeasTimeParamList, Name = tsk.Name, OrderId = tsk.OrderId, Prio = tsk.Prio, ResultType = tsk.ResultType, Stations = tsk.Stations, StationsForMeasurements = tsk.StationsForMeasurements, Status = h.status, Task = tsk.Task, Type = tsk.Type });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ClassStaticBus.bus.Dispose();
                                GC.SuppressFinalize(ClassStaticBus.bus);
                                ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                            }
                        }
                        cl.Dispose();
                        System.GC.Collect();
                    });
                    thread.Start();
                    thread.Join();
                }
                catch (Exception ex)
                {
                    logger.Error("Error in job ShedulerGetMeasTask: " + ex.Message);
                }
                context.Scheduler.ResumeAll();
                logger.Trace("End job ShedulerGetMeasTask.");
            }
        }

        /// <summary>
        /// Dispose sheduler
        /// </summary>
        public static void DisposeSheduler()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler();
            if (sched.GetTriggersOfJob(new JobKey(ShedulerStatic.Name_job_meas_task_submit, ShedulerStatic.Name_group_meas_task_submit)).Count > 0) {
                sched.DeleteJob(new JobKey(ShedulerStatic.Name_job_meas_task_submit, ShedulerStatic.Name_group_meas_task_submit));
                sched.UnscheduleJob(new TriggerKey(ShedulerStatic.Name_trigger_meas_task_submit, ShedulerStatic.Name_group_meas_task_submit));
            }
            sched.Shutdown();
        }
    }
}
