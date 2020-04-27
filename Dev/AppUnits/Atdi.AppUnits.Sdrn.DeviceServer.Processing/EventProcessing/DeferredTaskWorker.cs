using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.DataModels.EntityOrm;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class DeferredTaskWorker : ITaskWorker<DeferredTasks, DispatchProcess, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITaskStarter _taskStarter;
        private readonly ITimeService _timeService;
        private readonly ConfigProcessing _config;
        private readonly IWorkScheduler _workScheduler;
        private readonly IRepository<TaskParameters, string> _repositoryTaskParametersByString;


        public DeferredTaskWorker(ILogger logger,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            IWorkScheduler workScheduler,
            ConfigProcessing config,
            ITimeService timeService,
            IRepository<TaskParameters, string> repositoryTaskParametersByString)
        {
            this._logger = logger;
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._timeService = timeService;
            this._config = config;
            this._workScheduler = workScheduler;
            this._repositoryTaskParametersByString = repositoryTaskParametersByString;
        }
        /// <summary>
        /// Обработка отложенных задач
        /// </summary>
        /// <param name="context"></param>
        public void Run(ITaskContext<DeferredTasks, DispatchProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.DeferredTaskWorker, Categories.Processing, Events.StartDeferredTaskWorker.With(context.Task.Id));
                while (true)
                {
                    // ожидаем заданный в конфигурации промежуток времени
                    Thread.Sleep(this._config.DurationWaitingEventWithTask);
                    int i = 0;
                    if (context.Process.listDeferredTasks != null)
                    {
                        //если в списке появилась отложенная задача
                        while (i < context.Process.listDeferredTasks.Count)
                        {
                            var arr = context.Process.listDeferredTasks.ToArray();
                            var taskParameters = arr[i];

                            TimeSpan timeSpan = taskParameters.StartTime.Value - DateTime.Now;
                            //запускаем задачу в случае, если время 
                            if (timeSpan.TotalMinutes < this._config.MaxDurationBeforeStartTimeTask)
                            {
                                /////////////////////////////////////////////////////////////////
                                //
                                //
                                // Запуск задач для типа измерения 'SpectrumOccupation'
                                //
                                //
                                /////////////////////////////////////////////////////////////////
                                if (taskParameters.MeasurementType == MeasType.SpectrumOccupation)
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
                                        soTask.taskParameters = taskParameters;
                                        //soTask.taskParameters.status = StatusTask.A.ToString();
                                        this._repositoryTaskParametersByString.Update(soTask.taskParameters);
                                        soTask.mesureTraceParameter = soTask.taskParameters.ConvertForSO();
                                        _logger.Info(Contexts.DeferredTaskWorker, Categories.Processing, Events.StartDeferredTask.With(soTask.Id));
                                        _taskStarter.RunParallel(soTask, process, context);
                                        _logger.Info(Contexts.DeferredTaskWorker, Categories.Processing, Events.EndDeferredTask.With(soTask.Id));
                                        for (int m = 0; m < arr.Length; m++)
                                        {
                                            if (arr[m] != null)
                                            {
                                                if (arr[m].SDRTaskId== taskParameters.SDRTaskId)
                                                {
                                                    context.Process.listDeferredTasks = context.Process.listDeferredTasks.Remove(arr[m]);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (taskParameters.MeasurementType == MeasType.Signaling)
                                {
                                    var signalProcess = _processingDispatcher.Start<SignalizationProcess>(context.Process);
                                    var signalTask = new SignalizationTask();
                                    signalTask.durationForSendResultSignaling = this._config.durationForSendResultSignaling; // файл конфигурации (с него надо брать)
                                    signalTask.maximumTimeForWaitingResultSignalization = this._config.maximumTimeForWaitingResultSO;
                                    signalTask.SleepTimePeriodForWaitingStartingMeas = this._config.SleepTimePeriodForWaitingStartingMeas_ms;
                                    signalTask.KoeffWaitingDevice = this._config.KoeffWaitingDevice;
                                    signalTask.LastTimeSend = DateTime.Now;
                                    signalTask.taskParameters = taskParameters;
                                    //signalTask.taskParameters.status = StatusTask.A.ToString();
                                    this._repositoryTaskParametersByString.Update(signalTask.taskParameters);
                                    signalTask.mesureTraceParameter = signalTask.taskParameters.ConvertForSignaling();
                                    signalTask.actionConvertBW = ConvertTaskParametersToMesureTraceParameterForBandWidth.ConvertForBW;
                                    signalTask.actionConvertSysInfo = ConvertTaskParametersToMesureSystemInfoParameterForSysInfo.ConvertForMesureSystemInfoParameter;
                                    _logger.Info(Contexts.DeferredTaskWorker, Categories.Processing, Events.StartDeferredTask.With(signalTask.Id));
                                    _taskStarter.RunParallel(signalTask, signalProcess, context);
                                    _logger.Info(Contexts.DeferredTaskWorker, Categories.Processing, Events.EndDeferredTask.With(signalTask.Id));
                                    for (int m = 0; m < arr.Length; m++)
                                    {
                                        if (arr[m] != null)
                                        {
                                            if (arr[m].SDRTaskId == taskParameters.SDRTaskId)
                                            {
                                                context.Process.listDeferredTasks = context.Process.listDeferredTasks.Remove(arr[m]);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (taskParameters.MeasurementType == MeasType.BandwidthMeas)
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
                                    bandWidtTask.taskParameters = taskParameters;
                                    //bandWidtTask.taskParameters.status = StatusTask.A.ToString();
                                    this._repositoryTaskParametersByString.Update(bandWidtTask.taskParameters);
                                    bandWidtTask.mesureTraceParameter = bandWidtTask.taskParameters.ConvertForBW();
                                    _logger.Info(Contexts.DeferredTaskWorker, Categories.Processing, Events.StartDeferredTask.With(bandWidtTask.Id));
                                    _taskStarter.RunParallel(bandWidtTask, bandWidthProcess, context);
                                    _logger.Info(Contexts.DeferredTaskWorker, Categories.Processing, Events.EndDeferredTask.With(bandWidtTask.Id));
                                    for (int m = 0; m < arr.Length; m++)
                                    {
                                        if (arr[m] != null)
                                        {
                                            if (arr[m].SDRTaskId == taskParameters.SDRTaskId)
                                            {
                                                context.Process.listDeferredTasks = context.Process.listDeferredTasks.Remove(arr[m]);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.Error(Contexts.DeferredTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(taskParameters.MeasurementType));
                                    throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(taskParameters.MeasurementType));
                                }
                            }
                            i++;
                        }
                    }
                }
                // контекст никогда не выгружается т.к. эта задача должна постоянно работать, периодически проверяя отложеннные задачи на возможность выполнения
                //context.Finish();
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.DeferredTaskWorker, Categories.Processing, Exceptions.UnknownErrorDeferredTaskWorker, e);
                context.Abort(e);
            }
        }
    }
}
