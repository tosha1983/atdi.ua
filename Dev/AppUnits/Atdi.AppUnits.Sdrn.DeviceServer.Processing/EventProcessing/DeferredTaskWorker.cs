using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor;


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


        public DeferredTaskWorker(ILogger logger,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            IWorkScheduler workScheduler,
            ConfigProcessing config,
            ITimeService timeService)
        {
            this._logger = logger;
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._timeService = timeService;
            this._config = config;
            this._workScheduler = workScheduler;
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
                    int i= 0;
                    if (context.Process.listDeferredTasks != null)
                    {
                        //если в списке появилась отложенная задача
                        while (i< context.Process.listDeferredTasks.Count)
                        {
                            var taskParameters = context.Process.listDeferredTasks[i];

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

                                        var soTask = new SOTask()
                                        {
                                            TimeStamp = _timeService.TimeStamp.Milliseconds, // фиксируем текущий момент, или берем заранее снятый
                                            Options = TaskExecutionOption.Default,
                                        };

                                        soTask.sensorParameters = context.Process.activeSensor.Convert();

                                        soTask.durationForSendResult = this._config.DurationForSendResult; // файл конфигурации (с него надо брать)

                                        soTask.maximumTimeForWaitingResultSO = this._config.maximumTimeForWaitingResultSO;

                                        soTask.SleepTimePeriodForWaitingStartingMeas = this._config.maximumTimeForWaitingResultSO;

                                        soTask.SOKoeffWaitingDevice = this._config.SOKoeffWaitingDevice;

                                        soTask.LastTimeSend = DateTime.Now;

                                        soTask.taskParameters = taskParameters;

                                        soTask.mesureTraceParameter = soTask.taskParameters.Convert();

                                        _logger.Info(Contexts.DeferredTaskWorker, Categories.Processing, Events.StartDeferredTask.With(soTask.Id));

                                        _taskStarter.RunParallel(soTask, process, context);

                                        _logger.Info(Contexts.DeferredTaskWorker, Categories.Processing, Events.EndDeferredTask.With(soTask.Id));

                                        if (context.Process.listDeferredTasks.Contains(taskParameters))
                                        {
                                            context.Process.listDeferredTasks.Remove(taskParameters);
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
                _logger.Error(Contexts.DeferredTaskWorker, Categories.Processing, Exceptions.UnknownErrorDeferredTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
