using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.BusMessages.Server;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.EntityOrm;
using Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Contracts.Api.Sdrn;
using Atdi.Platform.DependencyInjection;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendMeasTaskHandler : MessageHandlerBase<DM.MeasTask, SendMeasTaskMessage>
    {
        private readonly ILogger _logger;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITaskStarter _taskStarter;
        private readonly IRepository<DM.MeasTask,int?> _repositoryMeasTask;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParameters;
        private readonly IRepository<DM.Sensor, int?> _repositorySensor;
        private readonly ITimeService _timeService;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;



        public SendMeasTaskHandler(
           ITimeService timeService,
           IProcessingDispatcher processingDispatcher,
           IRepository<DM.MeasTask, int?> repositoryMeasTask,
           IRepository<TaskParameters, int?> repositoryTaskParameters,
           IRepository<DM.Sensor, int?> repositorySensor,
           ITaskStarter taskStarter,
           IServicesResolver resolver, 
           IServicesContainer servicesContainer,
           ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._repositoryMeasTask = repositoryMeasTask;
            this._repositoryTaskParameters = repositoryTaskParameters;
            this._repositorySensor = repositorySensor;
            this._timeService = timeService;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
        }


        public override void OnHandle(IReceivedMessage<DM.MeasTask> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            if ((message.Data != null) && (message.Data.SdrnServer != null) && (message.Data.SensorName != null) && (message.Data.EquipmentTechId != null))
            {
                this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;
                //здесь ожидаем, пока не пройдет этап регистрации сенсора и не запустится таск QueuEventTask
                if ((baseContext.activeSensor == null) && (baseContext.contextQueueEventTask == null))
                {
                    while (true)
                    {
                        if ((baseContext.activeSensor != null) && (baseContext.contextQueueEventTask != null))
                        {
                            break;
                        }
                    }
                }
                // здесь предварительная проверка(валидация) таска на возможность физической обработки
                if (Validation(message.Data, baseContext.activeSensor)) // пока заглушка
                {
                    if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.SpectrumOccupation)
                    {
                        this._logger.Info(Contexts.ThisComponent, Categories.SendMeasTaskHandlerStart, Events.StartProcessSendMeasTask);
                        var taskParameters = message.Data.Convert();
                        var idTaskParameters = this._repositoryTaskParameters.Create(taskParameters);

                        var process = this._processingDispatcher.Start<BaseContext>();
                        var eventTask = new EventTask()
                        {
                            TimeStamp = _timeService.TimeStamp.Milliseconds,
                            Options = TaskExecutionOption.Default,
                        };

                        eventTask.taskParameters = taskParameters;

                        _taskStarter.Run(eventTask, process, baseContext.contextQueueEventTask);

                        this._logger.Info(Contexts.ThisComponent, Events.StartedEventTask.With(eventTask.Id));

                        message.Result = MessageHandlingResult.Confirmed;
                    }
                    else if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.Level)
                    {
                        message.Result = MessageHandlingResult.Trash;
                        throw new NotImplementedException("Not supported MeasurementType  'Level'");
                    }
                    else if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.MonitoringStations)
                    {
                        message.Result = MessageHandlingResult.Trash;
                        throw new NotImplementedException("Not supported MeasurementType 'MonitoringStations'");
                    }
                    else if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.Signaling)
                    {
                        message.Result = MessageHandlingResult.Trash;
                        throw new NotImplementedException("Not supported MeasurementType 'Signaling'");
                    }
                    else if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.BandwidthMeas)
                    {
                        message.Result = MessageHandlingResult.Trash;
                        throw new NotImplementedException("Not supported MeasurementType 'BandwidthMeas'");
                    }
                    else
                    {
                        message.Result = MessageHandlingResult.Trash;
                        throw new NotImplementedException("Not supported MeasurementType");
                    }
                }
            }
            else
            {
                message.Result = MessageHandlingResult.Trash;
                this._logger.Error(Contexts.ThisComponent, Exceptions.IncorrectMessageParams);
            }
        }

        /// <summary>
        /// Предварительная валидация measTask
        /// </summary>
        /// <param name="measTask"></param>
        /// <returns></returns>
        public bool Validation(DM.MeasTask measTask, DM.Sensor sensor)
        {
            bool isSuccessValidation = false;
            if ((measTask.SensorName == sensor.Name) && (measTask.EquipmentTechId == sensor.Equipment.TechId))
            {
                isSuccessValidation = true;
            }
            return isSuccessValidation;
        }
    }
}
