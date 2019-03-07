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
using Atdi.Platform.DependencyInjection;
using Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// Воркер, выполняющий прием уведомлений типа DM.DeviceCommand и TaskParameters и их обработку
    /// </summary>
    public class QueueEventTaskWorker : ITaskWorker<QueueEventTask, BaseContext, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ConfigProcessing _config;
        private readonly IWorkScheduler _workScheduler;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParametersByInt;
        private readonly IRepository<TaskParameters, string> _repositoryTaskParametersByString;

        public QueueEventTaskWorker(
            ILogger logger,
            IWorkScheduler workScheduler,
            IServicesResolver resolver,
            IServicesContainer servicesContainer,
            ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            IRepository<TaskParameters, int?> repositoryTaskParametersByInt,
            IRepository<TaskParameters, string> repositoryTaskParametersBystring,
            ConfigProcessing config,
            ITaskStarter taskStarter
            )
        {
            this._logger = logger;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._config = config;
            this._repositoryTaskParametersByInt = repositoryTaskParametersByInt;
            this._repositoryTaskParametersByString = repositoryTaskParametersBystring;
            this._workScheduler = workScheduler;
        }

        public void Run(ITaskContext<QueueEventTask, BaseContext> context)
        {
            try
            {
                _logger.Verbouse(Contexts.QueueEventTaskWorker, Categories.Processing, Events.StartQueueEventTaskWorker.With(context.Task.Id));
                // получаем объект глобального процесса с контейнера
                this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;
                baseContext.contextQueueEventTask = context;
                var taskParameters = new TaskParameters();
                var deviceCommand = new DM.DeviceCommand();
                bool isDatataskParameters = false;
                bool isDatadeviceCommand = false;
                DM.Sensor activeSensor = baseContext.activeSensor;

                Action action = new Action(() =>
                {
                    if (context.Task.taskParameters.StartTime.Value > DateTime.Now)
                    {
                        TimeSpan timeSpan = context.Task.taskParameters.StartTime.Value - DateTime.Now;
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
                            if (context.Task.taskParameters.MeasurementType == MeasType.SpectrumOccupation)
                            {
                                if (activeSensor != null)
                                {
                                    // Старт процесса SpectrumOccupationProcess
                                    var measProcess = this._processingDispatcher.Start<SpectrumOccupationProcess>();

                                    var soTask = new SOTask()
                                    {
                                        TimeStamp = _timeService.TimeStamp.Milliseconds, // фиксируем текущий момент, или берем заранее снятый
                                        Options = TaskExecutionOption.Default,
                                    };

                                    soTask.sensorParameters = activeSensor.Convert();

                                    soTask.durationForSendResult = this._config.DurationForSendResult; // файл конфигурации (с него надо брать)

                                    soTask.maximumTimeForWaitingResultSO = this._config.maximumTimeForWaitingResultSO;

                                    soTask.SOKoeffWaitingDevice = this._config.SOKoeffWaitingDevice;

                                    soTask.LastTimeSend = DateTime.Now;

                                    soTask.taskParameters = context.Task.taskParameters;

                                    soTask.mesureTraceParameter = soTask.taskParameters.Convert();

                                    _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.StartTaskQueueEventTaskWorker.With(soTask.Id));

                                    _taskStarter.Run(soTask, measProcess);

                                    _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.EndTaskQueueEventTaskWorker.With(soTask.Id));
                                }
                            }
                            else
                            {
                                _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(context.Task.taskParameters.MeasurementType));
                                throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(context.Task.taskParameters.MeasurementType));
                            }
                        }
                        else
                        {
                            // здесь необходимо добавлять в список отложенных задач
                            if (!baseContext.listDeferredTasks.Contains(context.Task.taskParameters))
                            {
                                baseContext.listDeferredTasks.Add(context.Task.taskParameters);
                            }
                        }
                    }
                });


                while (true)
                {
                    // ожидаем сообщения DeviceCommand  - "Run, Del, Stop task"
                    isDatadeviceCommand = context.WaitEvent<DM.DeviceCommand>(out deviceCommand, 10); //миллисекунд
                    if (isDatadeviceCommand)
                    {
                        if (deviceCommand != null)
                        {
                            if (deviceCommand.CustNbr1 != null)
                            {
                                int idTask = (int)deviceCommand.CustNbr1;
                                if (idTask > 0)
                                {
                                    var taskParams = this._repositoryTaskParametersByString.LoadObject(idTask.ToString());
                                    if (taskParams != null)
                                    {
                                        if (deviceCommand.Command == TypeMeasTask.RunMeasTask.ToString())
                                        {
                                            taskParams.status = StatusTask.A.ToString();
                                            if (taskParams.MeasurementType == MeasType.SpectrumOccupation)
                                            {
                                                var findSOTask = baseContext.contextSOTasks.Find(z => z.Task.taskParameters.SDRTaskId == taskParams.SDRTaskId);
                                                if (findSOTask != null)
                                                {
                                                    findSOTask.Task.status = StatusTask.A;
                                                }
                                                else
                                                {
                                                    //_workScheduler.Run($"Run scheduler measTask", () => { RunTask(taskParams, baseContext); });
                                                    context.Task.taskParameters = taskParams;
                                                    _workScheduler.Run($"Run scheduler measTask", () => {
                                                        if (context.Task.taskParameters.MeasurementType == MeasType.SpectrumOccupation)
                                                        {
                                                            if (activeSensor != null)
                                                            {
                                                                // Старт процесса SpectrumOccupationProcess
                                                                var measProcess = this._processingDispatcher.Start<SpectrumOccupationProcess>();

                                                                var soTask = new SOTask()
                                                                {
                                                                    TimeStamp = _timeService.TimeStamp.Milliseconds, // фиксируем текущий момент, или берем заранее снятый
                                                                    Options = TaskExecutionOption.Default,
                                                                };

                                                                soTask.sensorParameters = activeSensor.Convert();

                                                                soTask.durationForSendResult = this._config.DurationForSendResult; // файл конфигурации (с него надо брать)

                                                                soTask.maximumTimeForWaitingResultSO = this._config.maximumTimeForWaitingResultSO;

                                                                soTask.SOKoeffWaitingDevice = this._config.SOKoeffWaitingDevice;

                                                                soTask.LastTimeSend = DateTime.Now;

                                                                soTask.taskParameters = context.Task.taskParameters;

                                                                soTask.mesureTraceParameter = soTask.taskParameters.Convert();

                                                                _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.StartTaskQueueEventTaskWorker.With(soTask.Id));

                                                                _taskStarter.Run(soTask, measProcess);

                                                                _logger.Info(Contexts.QueueEventTaskWorker, Categories.Processing, Events.EndTaskQueueEventTaskWorker.With(soTask.Id));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(context.Task.taskParameters.MeasurementType));
                                                            throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(context.Task.taskParameters.MeasurementType));
                                                        }
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(taskParams.MeasurementType));
                                                throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(taskParams.MeasurementType));
                                            }
                                        }
                                        else if (deviceCommand.Command == TypeMeasTask.DelMeasTask.ToString())
                                        {
                                            taskParams.status = StatusTask.Z.ToString();
                                            if (taskParams.MeasurementType == MeasType.SpectrumOccupation)
                                            {
                                                var findSOTask = baseContext.contextSOTasks.Find(z => z.Task.taskParameters.SDRTaskId == taskParams.SDRTaskId);
                                                if (findSOTask != null)
                                                {
                                                    findSOTask.Task.status = StatusTask.Z;
                                                }
                                            }
                                            else
                                            {
                                                _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(taskParams.MeasurementType));
                                                throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(taskParams.MeasurementType));
                                            }
                                        }
                                        else if (deviceCommand.Command == TypeMeasTask.StopMeasTask.ToString())
                                        {
                                            taskParams.status = StatusTask.F.ToString();
                                            if (taskParams.MeasurementType == MeasType.SpectrumOccupation)
                                            {
                                                var findSOTask = baseContext.contextSOTasks.Find(z => z.Task.taskParameters.SDRTaskId == taskParams.SDRTaskId);
                                                if (findSOTask != null)
                                                {
                                                    findSOTask.Task.status = StatusTask.F;
                                                }
                                            }
                                            else
                                            {
                                                _logger.Error(Contexts.QueueEventTaskWorker, Categories.Processing, Exceptions.MeasurementTypeNotsupported.With(taskParams.MeasurementType));
                                                throw new NotImplementedException(Exceptions.MeasurementTypeNotsupported.With(taskParams.MeasurementType));
                                            }
                                        }
                                        // обновление TaskParameters в БД
                                        this._repositoryTaskParametersByInt.Update(taskParams);
                                    }
                                }
                            }
                        }
                    }
                    // ожидаем сообщения - "новый таск"
                    isDatataskParameters = context.WaitEvent<TaskParameters>(out taskParameters, 10); //миллисекунд
                    if (isDatataskParameters)
                    {
                        context.Task.taskParameters = taskParameters;
                        _workScheduler.Run($"Run scheduler measTask", () => { action.Invoke(); /*RunTask(taskParameters, baseContext);*/ });
                    }
                    //удаление всех контекстов задач, которые имеют статус Done, Cancelled, Aborted, Rejected
                    baseContext.contextSOTasks.RemoveAll(z => z.Task.status == StatusTask.Z || z.Task.status == StatusTask.C);
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
