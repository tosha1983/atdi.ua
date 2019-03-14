using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using System.Linq;
using Atdi.DataModels.EntityOrm;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// Воркер, выполняющий прием уведомлений типа DM.DeviceCommand и TaskParameters и их обработку
    /// </summary>
    public class QueueEventTaskWorker : ITaskWorker<QueueEventTask, DispatchProcess, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ConfigProcessing _config;
        private readonly IWorkScheduler _workScheduler;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParametersByInt;
        private readonly IRepository<TaskParameters, string> _repositoryTaskParametersByString;
        private readonly IRepository<LastUpdate, int?> _repositoryLastUpdateByInt;
        

        public QueueEventTaskWorker(
            ILogger logger,
            IWorkScheduler workScheduler,
            ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            IRepository<TaskParameters, int?> repositoryTaskParametersByInt,
            IRepository<TaskParameters, string> repositoryTaskParametersBystring,
            IRepository<LastUpdate, int?> repositoryLastUpdateByInt,
            ConfigProcessing config,
            ITaskStarter taskStarter
            )
        {
            this._logger = logger;
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._config = config;
            this._repositoryTaskParametersByInt = repositoryTaskParametersByInt;
            this._repositoryTaskParametersByString = repositoryTaskParametersBystring;
            this._repositoryLastUpdateByInt = repositoryLastUpdateByInt;
            this._workScheduler = workScheduler;
        }

        public void Run(ITaskContext<QueueEventTask, DispatchProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.QueueEventTaskWorker, Categories.Processing, Events.StartQueueEventTaskWorker.With(context.Task.Id));
                var activeSensor = context.Process.activeSensor;

                var cntActiveTaskParameters = 0;
                var taskParams = this._repositoryTaskParametersByInt.LoadObjectsWithRestrict();
                if ((taskParams != null) && (taskParams.Length > 0))
                {
                    var listTaskParameters = taskParams.ToList();
                    var allTaksWithStatusActive = listTaskParameters.FindAll(z => z.status == StatusTask.N.ToString() || z.status == StatusTask.A.ToString() || z.status == StatusTask.F.ToString());
                    if (allTaksWithStatusActive != null)
                    {
                        cntActiveTaskParameters = allTaksWithStatusActive.Count;
                    }
                }
                if (activeSensor != null)
                {
                    while (true)
                    {
                        // приостановка потока на время DurationWaitingCheckNewTasks
                        System.Threading.Thread.Sleep(this._config.DurationWaitingCheckNewTasks);

                        // проверка признака поступления новых тасков в БД
                        var allTablesLastUpdated = this._repositoryLastUpdateByInt.LoadAllObjects();
                        LastUpdate lastUpdateTaskParameter = null;
                        if ((allTablesLastUpdated != null) && (allTablesLastUpdated.Length > 0))
                        {
                            var listAlTables = allTablesLastUpdated.ToList();
                            lastUpdateTaskParameter = listAlTables.Find(z => z.TableName == "XBS_TASKPARAMETERS");
                        }

                        Action action = new Action(() =>
                        {
                            /////////////////////////////////////////////////////////////////
                            //
                            //
                            // Запуск задач для типа измерения 'SpectrumOccupation'
                            //
                            //
                            /////////////////////////////////////////////////////////////////
                            if (context.Task.taskParameters.MeasurementType == MeasType.SpectrumOccupation)
                            {
                                if (activeSensor != null)
                                {

                                    var process = _processingDispatcher.Start<SpectrumOccupationProcess>(context.Process);

                                    var soTask = new SOTask();

                                    soTask.sensorParameters = activeSensor.Convert();

                                    soTask.durationForSendResult = this._config.DurationForSendResult; // файл конфигурации (с него надо брать)

                                    soTask.maximumTimeForWaitingResultSO = this._config.maximumTimeForWaitingResultSO;

                                    soTask.SleepTimePeriodForWaitingStartingMeas = this._config.maximumTimeForWaitingResultSO;

                                    soTask.SOKoeffWaitingDevice = this._config.SOKoeffWaitingDevice;

                                    soTask.LastTimeSend = DateTime.Now;

                                    soTask.taskParameters = context.Task.taskParameters;

                                    soTask.mesureTraceParameter = soTask.taskParameters.Convert();

                                    _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.StartTaskQueueEventTaskWorker.With(soTask.Id));

                                    _taskStarter.RunParallel(soTask, process, context);

                                    _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.EndTaskQueueEventTaskWorker.With(soTask.Id));
                                }
                            }
                            else
                            {
                                _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(context.Task.taskParameters.MeasurementType));
                                throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(context.Task.taskParameters.MeasurementType));
                            }
                        });


                        if (((lastUpdateTaskParameter != null) && (lastUpdateTaskParameter.Status == "N")) || (lastUpdateTaskParameter == null) || (cntActiveTaskParameters > 0))
                        {
                            taskParams = this._repositoryTaskParametersByInt.LoadObjectsWithRestrict();

                            for (int i = 0; i < taskParams.Length; i++)
                            {
                                var tskParam = taskParams[i];

                                ITaskContext<SOTask, SpectrumOccupationProcess> findSOTask = null;

                                context.Task.taskParameters = tskParam;

                                if (tskParam.status == StatusTask.N.ToString())
                                {
                                    findSOTask = context.Process.contextSOTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId && z.Task.taskParameters.status == StatusTask.N.ToString());
                                    if (findSOTask != null)
                                    {
                                        continue;
                                    }
                                    if (tskParam.MeasurementType == MeasType.SpectrumOccupation)
                                    {

                                        if (context.Task.taskParameters.StartTime.Value > DateTime.Now)
                                        {
                                            TimeSpan timeSpan = context.Task.taskParameters.StartTime.Value - DateTime.Now;
                                            //запускаем задачу в случае, если время 
                                            if (timeSpan.TotalMinutes < this._config.MaxDurationBeforeStartTimeTask)
                                            {
                                                action.Invoke();
                                            }
                                            else
                                            {
                                                // здесь необходимо добавлять в список отложенных задач
                                                if (!context.Process.listDeferredTasks.Contains(context.Task.taskParameters))
                                                {
                                                    context.Process.listDeferredTasks.Add(context.Task.taskParameters);
                                                }
                                            }
                                        }
                                        else if ((context.Task.taskParameters.StartTime.Value <= DateTime.Now) && (context.Task.taskParameters.StopTime.Value >= DateTime.Now))
                                        {
                                            action.Invoke();
                                        }
                                        else
                                        {
                                            tskParam.status = StatusTask.C.ToString();
                                            this._repositoryTaskParametersByInt.Update(tskParam);
                                        }
                                    }
                                    else
                                    {
                                        _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                        throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                    }
                                }
                                else if (tskParam.status == StatusTask.A.ToString())
                                {
                                    findSOTask = context.Process.contextSOTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId && z.Task.taskParameters.status == StatusTask.A.ToString());
                                    if (findSOTask != null)
                                    {
                                        continue;
                                    }
                                    if (tskParam.MeasurementType == MeasType.SpectrumOccupation)
                                    {
                                        findSOTask = context.Process.contextSOTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId);
                                        if (findSOTask != null)
                                        {
                                            findSOTask.Task.taskParameters.status = StatusTask.A.ToString();
                                        }
                                        else
                                        {

                                            if (context.Task.taskParameters.StartTime.Value > DateTime.Now)
                                            {
                                                TimeSpan timeSpan = context.Task.taskParameters.StartTime.Value - DateTime.Now;
                                                //запускаем задачу в случае, если время 
                                                if (timeSpan.TotalMinutes < this._config.MaxDurationBeforeStartTimeTask)
                                                {
                                                    action.Invoke();
                                                }
                                                else
                                                {
                                                    // здесь необходимо добавлять в список отложенных задач
                                                    if (!context.Process.listDeferredTasks.Contains(context.Task.taskParameters))
                                                    {
                                                        context.Process.listDeferredTasks.Add(context.Task.taskParameters);
                                                    }
                                                }
                                            }
                                            else if ((context.Task.taskParameters.StartTime.Value <= DateTime.Now) && (context.Task.taskParameters.StopTime.Value >= DateTime.Now))
                                            {
                                                action.Invoke();
                                            }
                                            else
                                            {
                                                tskParam.status = StatusTask.C.ToString();
                                                this._repositoryTaskParametersByInt.Update(tskParam);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                        throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                    }
                                }
                                else if (tskParam.status == StatusTask.F.ToString())
                                {
                                    if (tskParam.MeasurementType == MeasType.SpectrumOccupation)
                                    {
                                        findSOTask = context.Process.contextSOTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId);
                                        if (findSOTask != null)
                                        {
                                            findSOTask.Task.taskParameters.status = StatusTask.F.ToString();
                                        }
                                        else
                                        {
                                            if (cntActiveTaskParameters > 0)
                                            {
                                                if ((context.Task.taskParameters.StartTime.Value <= DateTime.Now) && (context.Task.taskParameters.StopTime.Value >= DateTime.Now))
                                                {
                                                    tskParam.status = StatusTask.A.ToString();
                                                    this._repositoryTaskParametersByInt.Update(tskParam);
                                                    action.Invoke();
                                                    System.Threading.Thread.Sleep(this._config.SleepTimeForUpdateContextSOTask_ms);
                                                    findSOTask = context.Process.contextSOTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId);
                                                    if (findSOTask != null)
                                                    {
                                                        findSOTask.Task.taskParameters.status = StatusTask.F.ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    tskParam.status = StatusTask.C.ToString();
                                                    this._repositoryTaskParametersByInt.Update(tskParam);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                        throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                    }
                                }
                                else if (tskParam.status == StatusTask.Z.ToString())
                                {
                                    if (tskParam.MeasurementType == MeasType.SpectrumOccupation)
                                    {
                                        findSOTask = context.Process.contextSOTasks.Find(z => z.Task.taskParameters.SDRTaskId == tskParam.SDRTaskId);
                                        if (findSOTask != null)
                                        {
                                            findSOTask.Task.taskParameters.status = StatusTask.Z.ToString();
                                        }
                                    }
                                    else
                                    {
                                        _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                        throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                    }
                                }
                            }
                            context.Process.contextSOTasks.RemoveAll(z => z.Task.taskParameters.status == StatusTask.Z.ToString() || z.Task.taskParameters.status == StatusTask.C.ToString());
                            if (lastUpdateTaskParameter != null)
                            {
                                lastUpdateTaskParameter.Status = StatusTask.C.ToString();
                                this._repositoryLastUpdateByInt.Update(lastUpdateTaskParameter);
                            }
                            else
                            {
                                var lastUpdate = new LastUpdate()
                                {
                                    TableName = "XBS_TASKPARAMETERS",
                                    LastDateTimeUpdate = DateTime.Now,
                                    Status = StatusTask.C.ToString()
                                };
                                this._repositoryLastUpdateByInt.Create(lastUpdate);
                            }
                            cntActiveTaskParameters = 0;
                        }
                    }
                }

                // контекст никогда не выгружается т.к. в этом воркере происходит процесс постоянного ожидания для обработки сообщений типа DeviceCommand и TaskParameters
                //context.Finish();
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.UnknownErrorQueueEventTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
