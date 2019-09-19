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
    public class CommandTaskWorker : ITaskWorker<EventTask, DispatchProcess, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ConfigProcessing _config;
        private readonly IWorkScheduler _workScheduler;
        private readonly IController _controller;
        private readonly IRepository<TaskParameters, string> _repositoryTaskParametersByString;
        private readonly IRepository<DM.DeviceCommand, string> _repositoryDeviceCommand;


        public CommandTaskWorker(
            ILogger logger,
            IWorkScheduler workScheduler,
            ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            IRepository<TaskParameters, string> repositoryTaskParametersBystring,
            IRepository<DM.DeviceCommand, string> repositoryDeviceCommand,
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
            this._repositoryTaskParametersByString = repositoryTaskParametersBystring;
            this._workScheduler = workScheduler;
            this._controller = controller;
            this._repositoryDeviceCommand = repositoryDeviceCommand;
        }

        private void StartSOTask(ITaskContext<EventTask, DispatchProcess> context, ref List<string> listRunTask)
        {
            if (context.Process.activeSensor != null)
            {
                var process = _processingDispatcher.Start<SpectrumOccupationProcess>(context.Process);
                var soTask = new SOTask();
                soTask.sensorParameters = context.Process.activeSensor.Convert();
                soTask.durationForSendResultSO = this._config.durationForSendResultSO; // файл конфигурации (с него надо брать)
                soTask.maximumTimeForWaitingResultSO = this._config.maximumTimeForWaitingResultSO;
                soTask.SleepTimePeriodForWaitingStartingMeas = this._config.SleepTimePeriodForWaitingStartingMeas_ms;
                soTask.KoeffWaitingDevice = this._config.KoeffWaitingDevice;
                soTask.LastTimeSend = DateTime.Now;
                soTask.taskParameters = context.Task.taskParameters;
                soTask.mesureTraceParameter = soTask.taskParameters.ConvertForSO();
                if (!listRunTask.Contains(soTask.taskParameters.SDRTaskId))
                {
                    _logger.Info(Contexts.CommandTaskWorker, Categories.Processing, Events.StartTaskQueueEventTaskWorker.With(soTask.Id));
                    _taskStarter.RunParallel(soTask, process, context);
                    listRunTask.Add(soTask.taskParameters.SDRTaskId);
                    _logger.Info(Contexts.CommandTaskWorker, Categories.Processing, Events.EndTaskQueueEventTaskWorker.With(soTask.Id));
                }
            }
        }

        private void StartSignalizationTask(ITaskContext<EventTask, DispatchProcess> context, ref List<string> listRunTask)
        {
            if (context.Process.activeSensor != null)
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
                if (!listRunTask.Contains(signalTask.taskParameters.SDRTaskId))
                {
                    _logger.Info(Contexts.CommandTaskWorker, Categories.Processing, Events.StartTaskQueueEventTaskWorker.With(signalTask.Id));
                    _taskStarter.RunParallel(signalTask, signalProcess, context);
                    listRunTask.Add(signalTask.taskParameters.SDRTaskId);
                    _logger.Info(Contexts.CommandTaskWorker, Categories.Processing, Events.EndTaskQueueEventTaskWorker.With(signalTask.Id));
                }
            }
        }
        private void StartBandWidthTask(ITaskContext<EventTask, DispatchProcess> context, ref List<string> listRunTask)
        {
            if (context.Process.activeSensor != null)
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
                if (!listRunTask.Contains(bandWidtTask.taskParameters.SDRTaskId))
                {
                    _logger.Info(Contexts.CommandTaskWorker, Categories.Processing, Events.StartTaskQueueEventTaskWorker.With(bandWidtTask.Id));
                    _taskStarter.RunParallel(bandWidtTask, bandWidthProcess, context);
                    listRunTask.Add(bandWidtTask.taskParameters.SDRTaskId);
                    _logger.Info(Contexts.CommandTaskWorker, Categories.Processing, Events.EndTaskQueueEventTaskWorker.With(bandWidtTask.Id));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void StartLoad(ITaskContext<EventTask, DispatchProcess> context, Action action, ref List<string> listRunTask)
        {
            listRunTask = new List<string>();
            var loadData = this._repositoryTaskParametersByString.LoadObjectsWithRestrict(ref listRunTask);

            for (int i = 0; i < loadData.Length; i++)
            {
                var tskParam = loadData[i];

                context.Task.taskParameters = tskParam;
                if ((tskParam.status == StatusTask.N.ToString()) || (tskParam.status == StatusTask.A.ToString()) || (tskParam.status == StatusTask.F.ToString()) || (tskParam.status == StatusTask.Z.ToString()))
                {
                    if (tskParam.MeasurementType == MeasType.SpectrumOccupation)
                    {
                        var eventCommand = new CommandHandler<SOTask, SpectrumOccupationProcess>(this._logger, this._repositoryTaskParametersByString, this._config);
                        var listDeferredTasksTemp = new List<TaskParameters>();
                        var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextSOTasks, action, ref listDeferredTasksTemp, ref listRunTask, loadData.Length);
                        if (listDeferredTasksTemp.Count > 0)
                        {
                            if (context.Process.listDeferredTasks.Find(x => x.SDRTaskId == tskParam.SDRTaskId) == null)
                            {
                                context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                            }
                        }
                    }
                    else if (tskParam.MeasurementType == MeasType.Signaling)
                    {
                        var eventCommand = new CommandHandler<SignalizationTask, SignalizationProcess>(this._logger, this._repositoryTaskParametersByString, this._config);
                        var listDeferredTasksTemp = new List<TaskParameters>();
                        var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextSignalizationTasks, action, ref listDeferredTasksTemp, ref listRunTask, loadData.Length);
                        if (listDeferredTasksTemp.Count > 0)
                        {
                            if (context.Process.listDeferredTasks.Find(x => x.SDRTaskId == tskParam.SDRTaskId) == null)
                            {
                                context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                            }
                        }
                    }
                    else if (tskParam.MeasurementType == MeasType.BandwidthMeas)
                    {
                        var eventCommand = new CommandHandler<BandWidthTask, BandWidthProcess>(this._logger, this._repositoryTaskParametersByString, this._config);
                        var listDeferredTasksTemp = new List<TaskParameters>();
                        var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextBandWidthTasks, action, ref listDeferredTasksTemp, ref listRunTask, loadData.Length);
                        if (listDeferredTasksTemp.Count > 0)
                        {
                            if (context.Process.listDeferredTasks.Find(x => x.SDRTaskId == tskParam.SDRTaskId) == null)
                            {
                                context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                            }
                        }
                    }
                    else
                    {
                        _logger.Error(Contexts.CommandTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                        throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                    }
                }

            }
        }

        public void Run(ITaskContext<EventTask, DispatchProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.CommandTaskWorker, Categories.Processing, Events.StartQueueEventTaskWorker.With(context.Task.Id));
                var activeSensor = context.Process.activeSensor;
                var listRunTask = new List<string>();
                Action action = new Action(() =>
                {
                    if (context.Task.taskParameters.MeasurementType == MeasType.SpectrumOccupation)
                    {
                        StartSOTask(context, ref listRunTask);
                    }
                    else if (context.Task.taskParameters.MeasurementType == MeasType.Signaling)
                    {
                        StartSignalizationTask(context, ref listRunTask);
                    }
                    else if (context.Task.taskParameters.MeasurementType == MeasType.BandwidthMeas)
                    {
                        StartBandWidthTask(context, ref listRunTask);
                    }
                    else
                    {
                        _logger.Error(Contexts.CommandTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(context.Task.taskParameters.MeasurementType));
                        throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(context.Task.taskParameters.MeasurementType));
                    }
                });

                StartLoad(context, action, ref listRunTask);

                if (activeSensor != null)
                {
                    while (true)
                    {
                        // приостановка потока на время DurationWaitingCheckNewTasks
                        System.Threading.Thread.Sleep(this._config.DurationWaitingCheckNewTasks);
                        // проверка признака поступления новых тасков в БД
                        var deviceCommands = this._repositoryDeviceCommand.LoadAllObjects();
                        if ((deviceCommands != null) && (deviceCommands.Length>0))
                        {
                            for (int i = 0; i < deviceCommands.Length; i++)
                            {
                                var devCommand = deviceCommands[i];
                                var tskParam = this._repositoryTaskParametersByString.LoadObject(devCommand.CustTxt1);
                                if (tskParam != null)
                                {
                                    context.Task.taskParameters = tskParam;
                                    if ((tskParam.status == StatusTask.N.ToString()) || (tskParam.status == StatusTask.A.ToString()) || (tskParam.status == StatusTask.F.ToString()) || (tskParam.status == StatusTask.Z.ToString()))
                                    {
                                        if (tskParam.MeasurementType == MeasType.SpectrumOccupation)
                                        {
                                            var eventCommand = new CommandHandler<SOTask, SpectrumOccupationProcess>(this._logger, this._repositoryTaskParametersByString, this._config);
                                            var listDeferredTasksTemp = new List<TaskParameters>();
                                            var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextSOTasks, action, ref listDeferredTasksTemp, ref listRunTask, 1);
                                            if (listDeferredTasksTemp.Count > 0)
                                            {
                                                if (context.Process.listDeferredTasks.Find(x => x.SDRTaskId == tskParam.SDRTaskId) == null)
                                                {
                                                    context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                                                }
                                            }
                                        }
                                        else if (tskParam.MeasurementType == MeasType.Signaling)
                                        {
                                            var eventCommand = new CommandHandler<SignalizationTask, SignalizationProcess>(this._logger, this._repositoryTaskParametersByString, this._config);
                                            var listDeferredTasksTemp = new List<TaskParameters>();
                                            var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextSignalizationTasks, action, ref listDeferredTasksTemp, ref listRunTask, 1);
                                            if (listDeferredTasksTemp.Count > 0)
                                            {
                                                if (context.Process.listDeferredTasks.Find(x => x.SDRTaskId == tskParam.SDRTaskId) == null)
                                                {
                                                    context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                                                }
                                            }
                                        }
                                        else if (tskParam.MeasurementType == MeasType.BandwidthMeas)
                                        {
                                            var eventCommand = new CommandHandler<BandWidthTask, BandWidthProcess>(this._logger, this._repositoryTaskParametersByString, this._config);
                                            var listDeferredTasksTemp = new List<TaskParameters>();
                                            var isSuccess = eventCommand.StartCommand(tskParam, context.Process.contextBandWidthTasks, action, ref listDeferredTasksTemp, ref listRunTask, 1);
                                            if (listDeferredTasksTemp.Count > 0)
                                            {
                                                if (context.Process.listDeferredTasks.Find(x => x.SDRTaskId == tskParam.SDRTaskId) == null)
                                                {
                                                    context.Process.listDeferredTasks.AddRange(listDeferredTasksTemp);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Error(Contexts.CommandTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                            throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(tskParam.MeasurementType));
                                        }
                                    }
                                }
                            }
                        }
                        this._repositoryTaskParametersByString.RemoveOldObjects();
                    }
                }
                // контекст никогда не выгружается т.к. в этом воркере происходит процесс постоянного ожидания для обработки сообщений типа DeviceCommand и TaskParameters
                //context.Finish();
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.CommandTaskWorker, Categories.Processing, Exceptions.UnknownErrorQueueEventTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
