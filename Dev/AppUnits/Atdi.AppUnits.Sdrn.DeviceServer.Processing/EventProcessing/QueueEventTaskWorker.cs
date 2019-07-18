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
using System.Collections.Generic;

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
        private readonly IController _controller;
        private readonly IRepository<TaskParameters, long?> _repositoryTaskParametersByInt;
        private readonly IRepository<TaskParameters, string> _repositoryTaskParametersByString;
        private readonly IRepository<LastUpdate, long?> _repositoryLastUpdateByInt;
        

        public QueueEventTaskWorker(
            ILogger logger,
            IWorkScheduler workScheduler,
            ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            IRepository<TaskParameters, long?> repositoryTaskParametersByInt,
            IRepository<TaskParameters, string> repositoryTaskParametersBystring,
            IRepository<LastUpdate, long?> repositoryLastUpdateByInt,
            ConfigProcessing config,
            IController controller,
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
            this._controller = controller;
        }

        public void Run(ITaskContext<QueueEventTask, DispatchProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.QueueEventTaskWorker, Categories.Processing, Events.StartQueueEventTaskWorker.With(context.Task.Id));
                var activeSensor = context.Process.activeSensor;

                List<string> SetValueSdrTaskId = new List<string>();
                bool isFirstStart = false;
                var taskParamsAll = new List<TaskParameters>();
                var cntActiveTaskParameters = this._repositoryTaskParametersByInt.GetCountObjectsWithRestrict();
                taskParamsAll = this._repositoryTaskParametersByInt.LoadObjectsWithRestrict().ToList();
                if (activeSensor != null)
                {
                    while (true)
                    {
                        // приостановка потока на время DurationWaitingCheckNewTasks
                        System.Threading.Thread.Sleep(this._config.DurationWaitingCheckNewTasks);
                        // проверка признака поступления новых тасков в БД

                        LastUpdate lastUpdateTaskParameter = null;
                        var allTablesLastUpdated = this._repositoryLastUpdateByInt.LoadAllObjects();
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
                                    soTask.durationForSendResultSO = this._config.durationForSendResultSO; // файл конфигурации (с него надо брать)
                                    soTask.maximumTimeForWaitingResultSO = this._config.maximumTimeForWaitingResultSO;
                                    soTask.SleepTimePeriodForWaitingStartingMeas = this._config.SleepTimePeriodForWaitingStartingMeas_ms;
                                    soTask.KoeffWaitingDevice = this._config.KoeffWaitingDevice;
                                    soTask.LastTimeSend = DateTime.Now;
                                    soTask.taskParameters = context.Task.taskParameters;
                                    soTask.mesureTraceParameter = soTask.taskParameters.ConvertForSO();
                                    _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.StartTaskQueueEventTaskWorker.With(soTask.Id));
                                    _taskStarter.RunParallel(soTask, process, context);
                                    _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.EndTaskQueueEventTaskWorker.With(soTask.Id));
                                }
                            }
                            else if (context.Task.taskParameters.MeasurementType == MeasType.Signaling)
                            {
                                var signalProcess = _processingDispatcher.Start<SignalizationProcess>(context.Process);
                                var signalTask = new SignalizationTask();
                                signalTask.durationForSendResultSignaling = this._config.durationForSendResultSignaling; // файл конфигурации (с него надо брать)
                                signalTask.maximumTimeForWaitingResultSignalization = this._config.maximumTimeForWaitingResultSignalization;
                                signalTask.SleepTimePeriodForWaitingStartingMeas = this._config.SleepTimePeriodForWaitingStartingMeas_ms;
                                signalTask.KoeffWaitingDevice = this._config.KoeffWaitingDevice;
                                signalTask.LastTimeSend = DateTime.Now;
                                signalTask.taskParameters = context.Task.taskParameters;
                                signalTask.mesureTraceParameter = signalTask.taskParameters.ConvertForSignaling();
                                signalTask.actionConvertBW = ConvertTaskParametersToMesureTraceParameterForBandWidth.ConvertForBW;
                                signalTask.actionConvertSysInfo = ConvertTaskParametersToMesureSystemInfoParameterForSysInfo.ConvertForMesureSystemInfoParameter;
                                var deviceProperties = this._controller.GetDevicesProperties();
                                var listTraceDeviceProperties = deviceProperties.Values.ToArray();
                                for (int i = 0; i < listTraceDeviceProperties.Length; i++)
                                {
                                    bool isFindProperties = false;
                                    var traceDeviceProperties = listTraceDeviceProperties[i];
                                    for (int j = 0; j < traceDeviceProperties.Length; j++)
                                    {
                                        var trace = traceDeviceProperties[j];
                                        if (trace is MesureTraceDeviceProperties)
                                        {
                                            signalTask.mesureTraceDeviceProperties = (trace as MesureTraceDeviceProperties);
                                            break;
                                        }
                                    }
                                    if (isFindProperties)
                                    {
                                        break;
                                    }
                                }
                                _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.StartTaskQueueEventTaskWorker.With(signalTask.Id));
                                _taskStarter.RunParallel(signalTask, signalProcess, context);
                                _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.EndTaskQueueEventTaskWorker.With(signalTask.Id));
                            }
                            else if (context.Task.taskParameters.MeasurementType == MeasType.BandwidthMeas)
                            {
                                var bandWidthProcess = _processingDispatcher.Start<BandWidthProcess>(context.Process);
                                var bandWidtTask = new BandWidthTask();
                                bandWidtTask.BandwidthEstimationType = this._config.BandwidthEstimationType;
                                bandWidtTask.Smooth = this._config.Smooth;
                                bandWidtTask.X_Beta = this._config.X_Beta;
                                bandWidtTask.MaximumIgnorPoint = this._config.MaximumIgnorPoint;
                                bandWidtTask.durationForMeasBW_ms = this._config.durationForMeasBW_ms;
                                bandWidtTask.durationForSendResultBandWidth = this._config.durationForSendResultBandWidth; // файл конфигурации (с него надо брать)
                                bandWidtTask.maximumTimeForWaitingResultBandWidth = this._config.maximumTimeForWaitingResultBandWidth;
                                bandWidtTask.SleepTimePeriodForWaitingStartingMeas = this._config.SleepTimePeriodForWaitingStartingMeas_ms;
                                bandWidtTask.KoeffWaitingDevice = this._config.KoeffWaitingDevice;
                                bandWidtTask.LastTimeSend = DateTime.Now;
                                bandWidtTask.taskParameters = context.Task.taskParameters;
                                bandWidtTask.mesureTraceParameter = bandWidtTask.taskParameters.ConvertForBW();
                                _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.StartTaskQueueEventTaskWorker.With(bandWidtTask.Id));
                                //_logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, "Check time start");
                                _taskStarter.RunParallel(bandWidtTask, bandWidthProcess, context);
                                _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.EndTaskQueueEventTaskWorker.With(bandWidtTask.Id));
                            }
                            else
                            {
                                _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(context.Task.taskParameters.MeasurementType));
                                throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(context.Task.taskParameters.MeasurementType));
                            }
                        });

                        if (isFirstStart == false)
                        {
                            lastUpdateTaskParameter.Status = "N";
                            this._repositoryLastUpdateByInt.Update(lastUpdateTaskParameter);
                            isFirstStart = true;
                        }

                        cntActiveTaskParameters = this._repositoryTaskParametersByInt.GetCountObjectsWithRestrict();
                        if (taskParamsAll.Count == 0)
                        {
                            taskParamsAll = this._repositoryTaskParametersByInt.LoadObjectsWithRestrict().ToList();
                        }
                        else  if (taskParamsAll.Count > 0)
                        {
                            if (cntActiveTaskParameters != taskParamsAll.Count)
                            {
                                lastUpdateTaskParameter.Status = "N";
                                this._repositoryLastUpdateByInt.Update(lastUpdateTaskParameter);
                            }

                            var dictionaryStatusObjects = this._repositoryTaskParametersByInt.GetDictionaryStatusObjects();
                            for (int i = 0; i < dictionaryStatusObjects.Count; i++)
                            {
                                if (taskParamsAll.Find(x => x.SDRTaskId == dictionaryStatusObjects.ElementAt(i).Key) == null)
                                {
                                    var taskParametersLoaded = this._repositoryTaskParametersByString.LoadObject(dictionaryStatusObjects.ElementAt(i).Key);
                                    if (taskParametersLoaded != null)
                                    {
                                        taskParamsAll.RemoveAll(z => z.SDRTaskId == dictionaryStatusObjects.ElementAt(i).Key);
                                        taskParamsAll.Add(taskParametersLoaded);
                                    }
                                }

                                if (taskParamsAll.Find(x => x.SDRTaskId == dictionaryStatusObjects.ElementAt(i).Key && x.status == dictionaryStatusObjects.ElementAt(i).Value) == null)
                                {
                                    var tskFnd = taskParamsAll.Find(x => x.SDRTaskId == dictionaryStatusObjects.ElementAt(i).Key);
                                    if (tskFnd != null)
                                    {
                                        lastUpdateTaskParameter.Status = StatusTask.C.ToString();
                                        this._repositoryLastUpdateByInt.Update(lastUpdateTaskParameter);

                                        tskFnd.status = dictionaryStatusObjects.ElementAt(i).Value;
                                        var tskParam = tskFnd;
                                        context.Task.taskParameters = tskParam;
                                        if ((tskParam.status == StatusTask.N.ToString()) || (tskParam.status == StatusTask.A.ToString()) || (tskParam.status == StatusTask.F.ToString()) || (tskParam.status == StatusTask.Z.ToString()))
                                        {
                                            if (tskParam.MeasurementType == MeasType.SpectrumOccupation)
                                            {
                                                var eventCommand = new EventCommand<SOTask, SpectrumOccupationProcess>(this._logger, this._repositoryTaskParametersByInt, this._config);
                                                var listDeferredTasksTemp = new List<TaskParameters>();
                                                var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextSOTasks, action, ref listDeferredTasksTemp, cntActiveTaskParameters);
                                                if (listDeferredTasksTemp.Count > 0)
                                                {
                                                    if (!context.Process.listDeferredTasks.Contains(tskParam))
                                                    {
                                                        context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                                                    }
                                                }
                                            }
                                            else if (tskParam.MeasurementType == MeasType.Signaling)
                                            {
                                                var eventCommand = new EventCommand<SignalizationTask, SignalizationProcess>(this._logger, this._repositoryTaskParametersByInt, this._config);
                                                var listDeferredTasksTemp = new List<TaskParameters>();
                                                var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextSignalizationTasks, action, ref listDeferredTasksTemp, cntActiveTaskParameters);
                                                if (listDeferredTasksTemp.Count > 0)
                                                {
                                                    if (!context.Process.listDeferredTasks.Contains(tskParam))
                                                    {
                                                        context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                                                    }
                                                }
                                            }
                                            else if (tskParam.MeasurementType == MeasType.BandwidthMeas)
                                            {
                                                var eventCommand = new EventCommand<BandWidthTask, BandWidthProcess>(this._logger, this._repositoryTaskParametersByInt, this._config);
                                                var listDeferredTasksTemp = new List<TaskParameters>();
                                                var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextBandWidthTasks, action, ref listDeferredTasksTemp, cntActiveTaskParameters);
                                                if (listDeferredTasksTemp.Count > 0)
                                                {
                                                    if (!context.Process.listDeferredTasks.Contains(tskParam))
                                                    {
                                                        context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                                throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                            }

                                            if (tskParam.status == StatusTask.Z.ToString())
                                            {
                                                SetValueSdrTaskId.Remove(tskParam.SDRTaskId);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var tskParam = taskParamsAll.Find(x => x.SDRTaskId == dictionaryStatusObjects.ElementAt(i).Key);
                                    if (tskParam != null)
                                    {
                                        if (!SetValueSdrTaskId.Contains(tskParam.SDRTaskId))
                                        {
                                            context.Task.taskParameters = tskParam;
                                            if ((tskParam.status == StatusTask.N.ToString()) || (tskParam.status == StatusTask.A.ToString()) || (tskParam.status == StatusTask.F.ToString()))
                                            {
                                                if (tskParam.MeasurementType == MeasType.SpectrumOccupation)
                                                {
                                                    var eventCommand = new EventCommand<SOTask, SpectrumOccupationProcess>(this._logger, this._repositoryTaskParametersByInt, this._config);
                                                    var listDeferredTasksTemp = new List<TaskParameters>();
                                                    var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextSOTasks, action, ref listDeferredTasksTemp, cntActiveTaskParameters);
                                                    if (listDeferredTasksTemp.Count > 0)
                                                    {
                                                        if (!context.Process.listDeferredTasks.Contains(tskParam))
                                                        {
                                                            context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                                                        }
                                                    }
                                                    SetValueSdrTaskId.Add(tskParam.SDRTaskId);
                                                }
                                                else if (tskParam.MeasurementType == MeasType.Signaling)
                                                {
                                                    var eventCommand = new EventCommand<SignalizationTask, SignalizationProcess>(this._logger, this._repositoryTaskParametersByInt, this._config);
                                                    var listDeferredTasksTemp = new List<TaskParameters>();
                                                    var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextSignalizationTasks, action, ref listDeferredTasksTemp, cntActiveTaskParameters);
                                                    if (listDeferredTasksTemp.Count > 0)
                                                    {
                                                        if (!context.Process.listDeferredTasks.Contains(tskParam))
                                                        {
                                                            context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                                                        }
                                                    }
                                                    SetValueSdrTaskId.Add(tskParam.SDRTaskId);
                                                }
                                                else if (tskParam.MeasurementType == MeasType.BandwidthMeas)
                                                {
                                                    var eventCommand = new EventCommand<BandWidthTask, BandWidthProcess>(this._logger, this._repositoryTaskParametersByInt, this._config);
                                                    var listDeferredTasksTemp = new List<TaskParameters>();
                                                    var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextBandWidthTasks, action, ref listDeferredTasksTemp, cntActiveTaskParameters);
                                                    if (listDeferredTasksTemp.Count > 0)
                                                    {
                                                        if (!context.Process.listDeferredTasks.Contains(tskParam))
                                                        {
                                                            context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                                                        }
                                                    }
                                                    SetValueSdrTaskId.Add(tskParam.SDRTaskId);
                                                }
                                                else
                                                {
                                                    _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                                    throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
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
