using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using Atdi.Contracts.Api.Sdrn.MessageBus;





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
        private readonly ConfigProcessing _configProcessing;


        public DispatcherWorker(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ConfigProcessing configProcessing,
            ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._configProcessing = configProcessing;
        }

        public void Run(ITaskContext<AutoTaskBase, DeviceServerBackgroundProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.DispatcherWorker, Categories.Processing, Events.StartDispatcherWorker.With(context.Task.Id));

                ////////////////////////////////////////////////////////////////////////
                // 
                //
                // запуск задачи регистрации сенсора
                //
                ////////////////////////////////////////////////////////////////////////

                var dispatchProcess = this._processingDispatcher.Start<DispatchProcess>(context.Process);

                _taskStarter.Run(new RegisterSensorTask(), dispatchProcess);

                if (dispatchProcess.activeSensor != null)
                {
                    ////////////////////////////////////////////////////////////////////////
                    // 
                    //
                    // запуск задач по событиям, которые приходят с шины, время до выполнения, которых меньше 20 мин
                    //
                    ////////////////////////////////////////////////////////////////////////
                    _taskStarter.RunParallel(new EventTask(), dispatchProcess);

                    ////////////////////////////////////////////////////////////////////////
                    // 
                    //
                    // запуск отложенных задач, которые были получены с шины но время до выполнения, которых превышает 20 мин
                    //
                    ////////////////////////////////////////////////////////////////////////
                    _taskStarter.RunParallel(new DeferredTasks(), dispatchProcess);

                    ////////////////////////////////////////////////////////////////////////
                    // 
                    //
                    // запуск задачи включения GPS
                    //
                    ////////////////////////////////////////////////////////////////////////
                    _taskStarter.RunParallel(new GPSTask(), dispatchProcess);
                    


                    ////////////////////////////////////////////////////////////////////////
                    // 
                    //
                    // запуск задачи по отправке уведомлений об активности сенсора
                    //
                    ////////////////////////////////////////////////////////////////////////
                    //_taskStarter.RunParallel(new ActiveSensorTask(), dispatchProcess);


                    ////////////////////////////////////////////////////////////////////////
                    // 
                    //
                    // запуск задачи по отправке результатов измерений в шину
                    //
                    ////////////////////////////////////////////////////////////////////////
                    _taskStarter.RunParallel(new SendResult(), dispatchProcess);

                }
                else
                {
                    _logger.Error(Contexts.DispatcherWorker, Categories.Processing, Exceptions.NotFoundInformationAboutSensor);
                    context.Finish();
                }

            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.DispatcherWorker, Categories.Processing, Exceptions.UnknownErrorDispatcherWorker, e);
                context.Abort(e);
            }
        }
    }
}
