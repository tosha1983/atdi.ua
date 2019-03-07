using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;




namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// Основной воркер (автозапуск)
    /// Выполняет последовательный запуск тасков по регистрации, проверка и старт незавершенных задач с БД, старт GPS, 
    /// таск по приему и обработке уведомлений о поступленни новых тасков из входящих сообщений (шины),
    /// таск по обработке отложенных задач (задач, время до выполнения которых > 20 мин)
    /// </summary>
    public class DispatcherWorker : ITaskWorker<AutoTaskBase , DeviceServerBackgroundProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;

        public DispatcherWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger,
            IServicesResolver resolver,
            IServicesContainer servicesContainer)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
        }

        public void Run(ITaskContext<AutoTaskBase, DeviceServerBackgroundProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.DispatcherWorker, Categories.Processing, Events.StartDispatcherWorker.With(context.Task.Id));
                //перед запуском- приостановка потока на 3 сек, для инициализации всех объектов,
                //которые не успели инициализироваться к этому моменту
                //System.Threading.Thread.Sleep(3000);

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

                if (baseContext.activeSensor != null)
                {


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
                    _taskStarter.Run(processingFromDBTask, baseProcessingFromDBTask);

                 
                }
                else
                {
                    _logger.Error(Contexts.DispatcherWorker, Categories.Processing, Exceptions.NotFoundInformationAboutSensor);
                    context.Finish();
                }
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.DispatcherWorker, Categories.Processing, Exceptions.UnknownErrorDispatcherWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
