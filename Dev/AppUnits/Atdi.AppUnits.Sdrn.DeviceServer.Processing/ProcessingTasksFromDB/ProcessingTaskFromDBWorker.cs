using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using Atdi.DataModels.EntityOrm;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor;
using Atdi.Platform.DependencyInjection;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// Воркер по обработке тасков из БД
    /// </summary>
    public class ProcessingTaskFromDBWorker : ITaskWorker<ProcessingFromDBTask, BaseContext, SingletonTaskWorkerLifetime>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParameters;
        private readonly ILogger _logger;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;
        private readonly ConfigProcessing _config;

        public ProcessingTaskFromDBWorker(IProcessingDispatcher processingDispatcher,
            IRepository<TaskParameters, int?> repositoryTaskParameters,
            ITaskStarter taskStarter,
            IServicesResolver resolver,
            IServicesContainer servicesContainer,
            ConfigProcessing config,
            ITimeService timeService, ILogger logger)
        {
            this._logger = logger;
            this._timeService = timeService;
            this._processingDispatcher = processingDispatcher;
            this._repositoryTaskParameters = repositoryTaskParameters;
            this._taskStarter = taskStarter;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
            this._config = config;
        }

        public void Run(ITaskContext<ProcessingFromDBTask, BaseContext> context)
        {
            try
            {
                _logger.Verbouse(Contexts.ProcessingTaskFromDBWorker, Categories.Processing, Events.StartProcessingTaskFromDBWorker.With(context.Task.Id));
                this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;
                DM.Sensor sensorActive = baseContext.activeSensor;
                if (sensorActive != null)
                {
                    // загрузка из БД;
                    var taskParams = this._repositoryTaskParameters.LoadAllObjects();
                    for (int i = 0; i < taskParams.Length; i++)
                    {
                        if (((taskParams[i].status == "A") || (taskParams[i].status == "N")) && ((taskParams[i].StartTime <= DateTime.Now) && (taskParams[i].StopTime >= DateTime.Now)))
                        {
                            /////////////////////////////////////////////////////////////////
                            //
                            //
                            // Запуск задач для типа измерения 'SpectrumOccupation'
                            //
                            //
                            /////////////////////////////////////////////////////////////////
                            if (taskParams[i].MeasurementType == MeasType.SpectrumOccupation)
                            {
                                // Старт процесса SpectrumOccupationProcess
                                var measProcess = this._processingDispatcher.Start<SpectrumOccupationProcess>();

                                var soTask = new SOTask()
                                {
                                    TimeStamp = _timeService.TimeStamp.Milliseconds, // фиксируем текущий момент, или берем заранее снятый
                                    Options = TaskExecutionOption.Default,
                                };

                                soTask.sensorParameters = sensorActive.Convert();

                                soTask.durationForSendResult = this._config.DurationForSendResult; // файл конфигурации (с него надо брать)

                                soTask.maximumTimeForWaitingResultSO = this._config.maximumTimeForWaitingResultSO;

                                soTask.SOKoeffWaitingDevice = this._config.SOKoeffWaitingDevice;

                                soTask.LastTimeSend = DateTime.Now;

                                soTask.taskParameters = taskParams[i];

                                soTask.mesureTraceParameter = soTask.taskParameters.Convert();

                                _logger.Info(Contexts.ProcessingTaskFromDBWorker, Categories.Processing, Events.StartProcessingTaskFromDBWorker.With(soTask.Id));

                                _taskStarter.Run(soTask, measProcess);

                                _logger.Info(Contexts.ProcessingTaskFromDBWorker, Categories.Processing, Events.EndTaskProcessingTaskFromDBWorker.With(soTask.Id));

                            }
                            else
                            {
                                _logger.Error(Contexts.ProcessingTaskFromDBWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(taskParams[i].MeasurementType));
                                throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(taskParams[i].MeasurementType));
                            }
                        }
                    }
                }
                context.Finish();
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.ProcessingTaskFromDBWorker, Categories.Processing, Exceptions.UnknownErrorGPSWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
