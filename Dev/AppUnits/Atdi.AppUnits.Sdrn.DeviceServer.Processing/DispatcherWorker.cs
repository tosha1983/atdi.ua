using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;




namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class DispatcherWorker : ITaskWorker<AutoTaskBase , DeviceServerBackgroundProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;
        private readonly ConfigProcessing _config;

        public DispatcherWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IServicesResolver resolver,
            ConfigProcessing config,
            IServicesContainer servicesContainer)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
            this._config = config;
        }

        public void Run(ITaskContext<AutoTaskBase, DeviceServerBackgroundProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.DispatcherWorker, Categories.Processing, Events.StartDispatcherWorker.With(context.Task.Id));

                ////////////////////////////////////////////////////////////////////////
                // 
                //
                // получение с DI - контейнера экземпляра глобального процесса MainProcess
                //
                ////////////////////////////////////////////////////////////////////////
                this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;


                ////////////////////////////////////////////////////////////////////////
                // 
                //
                // запуск задачи регистрации сенсора
                //
                ////////////////////////////////////////////////////////////////////////
                var baseProcessRegisterSensor = this._processingDispatcher.Start<BaseContext>(baseContext);
                var regSensorTask = new RegisterSensorTask()
                {
                    TimeStamp = _timeService.TimeStamp.Milliseconds,
                    Options = TaskExecutionOption.Default
                };
                _taskStarter.Run(regSensorTask, baseProcessRegisterSensor);


                ////////////////////////////////////////////////////////////////////////
                // 
                //
                // запуск задачи включения GPS
                //
                ////////////////////////////////////////////////////////////////////////
                var baseGPS = this._processingDispatcher.Start<BaseContext>(baseContext);
                var gpsTask = new GPSTask()
                {
                    TimeStamp = _timeService.TimeStamp.Milliseconds,
                    Options = TaskExecutionOption.Default
                };
                _taskStarter.RunParallel(gpsTask, baseGPS);

                ////////////////////////////////////////////////////////////////////////
                // 
                //
                // запуск задач c БД
                //
                ////////////////////////////////////////////////////////////////////////
                var baseProcessingFromDBTask = this._processingDispatcher.Start<BaseContext>(baseContext);
                var processingFromDBTask = new ProcessingFromDBTask()
                 {
                   TimeStamp = _timeService.TimeStamp.Milliseconds,
                   Options = TaskExecutionOption.Default
                };
                _taskStarter.RunParallel(processingFromDBTask, baseProcessingFromDBTask);
                
                ////////////////////////////////////////////////////////////////////////
                // 
                //
                // запуск задач по событиям, которые приходят с шины, время до выполнения, которых меньше 20 мин
                //
                ////////////////////////////////////////////////////////////////////////
                var baseQueueEventTask = this._processingDispatcher.Start<BaseContext>(baseContext);
                var queueEventTask = new QueueEventTask()
                {
                    TimeStamp = _timeService.TimeStamp.Milliseconds,
                    Options = TaskExecutionOption.Default
                };
                _taskStarter.RunParallel(queueEventTask, baseQueueEventTask);


                ////////////////////////////////////////////////////////////////////////
                // 
                //
                // запуск отложенных задач, которые были получены с шины но время до выполнения, которых превышает 20 мин
                //
                ////////////////////////////////////////////////////////////////////////
                var baseDeferredTasks = this._processingDispatcher.Start<BaseContext>(baseContext);
                var deferredTasks = new DeferredTasks()
                {
                    TimeStamp = _timeService.TimeStamp.Milliseconds,
                    Options = TaskExecutionOption.Default
                };
                _taskStarter.RunParallel(deferredTasks, baseDeferredTasks);
             
                
                //context.Finish();
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.DispatcherWorker, Categories.Processing, Exceptions.UnknownErrorDispatcherWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
