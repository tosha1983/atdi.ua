using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class DeferredTaskWorker : ITaskWorker<DeferredTasks, BaseContext, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITaskStarter _taskStarter;
        private readonly ITimeService _timeService;
        private readonly ConfigProcessing _config;



        public DeferredTaskWorker(ILogger logger,
            IServicesResolver resolver,
            IServicesContainer servicesContainer,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ConfigProcessing config,
            ITimeService timeService)
        {
            this._logger = logger;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._timeService = timeService;
            this._config = config;
        }
        /// <summary>
        /// Обработка отложенных задач
        /// </summary>
        /// <param name="context"></param>
        public void Run(ITaskContext<DeferredTasks, BaseContext> context)
        {
            try
            {
                _logger.Verbouse(Contexts.DeferredTaskWorker, Categories.Processing, Events.StartDeferredTaskWorker.With(context.Task.Id));
                this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;
                while (true)
                {
                    // ожидаем заданный в конфигурации промежуток времени
                    Thread.Sleep(this._config.DurationWaitingEventWithTask);
                    int i= 0;
                    if (baseContext.listDeferredTasks != null)
                    {
                        //если в списке появилась отложенная задача
                        while (i< baseContext.listDeferredTasks.Count)
                        {
                            var taskParameters = baseContext.listDeferredTasks[i];

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
                                    if (baseContext.activeSensor != null)
                                    {
                                        var measProcess = this._processingDispatcher.Start<SpectrumOccupationProcess>();
                                        var soTask = new SOTask()
                                        {
                                            TimeStamp = _timeService.TimeStamp.Milliseconds, // фиксируем текущий момент, или берем заранее снятый
                                            Options = TaskExecutionOption.Default,
                                        };

                                        soTask.sensorParameters = baseContext.activeSensor.Convert();

                                        soTask.durationForSendResult = this._config.DurationForSendResult; // файл конфигурации (с него надо брать)

                                        soTask.maximumTimeForWaitingResultSO = this._config.maximumTimeForWaitingResultSO;

                                        soTask.SOKoeffWaitingDevice = this._config.SOKoeffWaitingDevice;

                                        soTask.LastTimeSend = DateTime.Now;

                                        soTask.taskParameters = taskParameters;

                                        soTask.mesureTraceParameter = soTask.taskParameters.Convert();

                                        _logger.Info(Contexts.DeferredTaskWorker, Categories.Processing, Events.StartDeferredTask.With(soTask.Id));

                                        _taskStarter.Run(soTask, measProcess);

                                        _logger.Info(Contexts.DeferredTaskWorker, Categories.Processing, Events.EndDeferredTask.With(soTask.Id));

                                        if (baseContext.listDeferredTasks.Contains(taskParameters))
                                        {
                                            baseContext.listDeferredTasks.Remove(taskParameters);
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
