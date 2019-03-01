using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using Atdi.DataModels.EntityOrm;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class RegisterSensorTaskWorker : ITaskWorker<RegisterSensorTask, BaseContext, SingletonTaskWorkerLifetime>
    {
        private readonly IRepository<DM.Sensor, int?> _repositorySensor;
        private readonly ILogger _logger;
        private readonly IBusGate _busGate;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly IDeviceServerConfig  _deviceServerConfig;
        private readonly ConfigProcessing _config;

        public RegisterSensorTaskWorker(IRepository<DM.Sensor, int?> repositorySensor,
            ILogger logger,
            IBusGate busGate,
            IServicesResolver resolver,
            IServicesContainer servicesContainer,
            ITimeService timeService,
            IProcessingDispatcher processingDispatcher, 
            ITaskStarter taskStarter,
            ConfigProcessing config,
            IDeviceServerConfig deviceServerConfig
            )
        {
            this._logger = logger;
            this._repositorySensor = repositorySensor;
            this._busGate = busGate;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._deviceServerConfig = deviceServerConfig;
            this._config = config;
        }

        public void Run(ITaskContext<RegisterSensorTask, BaseContext> context)
        {
            try
            {
                _logger.Verbouse(Contexts.RegisterSensorTaskWorker, Categories.Processing, Events.StartRegisterSensorTaskWorker.With(context.Task.Id));
                // получаем объект глобального процесса с контейнера
                this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;

                var allSensor = this._repositorySensor.LoadAllObjects();
                if (((allSensor == null) || ((allSensor != null) && (allSensor.Length == 0))))
                {
                    //здесь  получаем информацию о сенсоре с контроллера (пока не реализовано)
                    //this._controller
                    // и пишем в БД
                    DM.Sensor sensor = new DM.Sensor()
                    {
                        Status = "A",
                        Name = this._deviceServerConfig.SensorName,
                        Equipment = new DM.SensorEquipment()
                        {
                            TechId = this._deviceServerConfig.SensorTechId
                        }
                    };
                    
                    var publisher = this._busGate.CreatePublisher("main");
                    publisher.Send<DM.Sensor>("RegisterSensor", sensor);
                    publisher.Dispose();

                    // присваиваем текущий контекст ITaskContext<RegisterSensorTask, BaseContext> для дальнейшей передачи через него SensorRegistrationResult 
                    baseContext.contextRegisterSensorTask = context;
                    // ожидаем получение сообщения с подтверждением от SDRN сервера об успешной регистрации сенсора
                    var sensorRegResult = new DM.SensorRegistrationResult();
                    while (true)
                    {
                        // ожидаем сообщения 5 мин, так пока не получим подтверждение о регистрации
                        var isData = context.WaitEvent<DM.SensorRegistrationResult>(out sensorRegResult, this._config.MaxTimeOutReceiveSensorRegistrationResult);
                        //успешная регистрация
                        if (isData == true)
                        {
                            this._logger.Info(Contexts.RegisterSensorTaskWorker, Events.ReceivedSensorRegistrationConfirmation);
                            if ((sensorRegResult.Status == "Success") && (sensorRegResult.EquipmentTechId == sensor.Equipment.TechId) && (sensorRegResult.SensorName == sensor.Name))
                            {
                                //сохранение сведений о сенсоре в БД
                                var idSensor = this._repositorySensor.Create(sensor);
                                if (idSensor>0)
                                {
                                    baseContext.activeSensor = sensor;
                                    this._logger.Info(Contexts.RegisterSensorTaskWorker, Events.SensorInformationRecordedDB);
                                    break;
                                }
                                else
                                {
                                    this._logger.Info(Contexts.RegisterSensorTaskWorker, Events.SensorInformationNotRecordedDB);
                                    throw new Exception(Events.SensorInformationNotRecordedDB.Text);
                                }
                            }
                            else if ((sensorRegResult.Status == "Reject") && (sensorRegResult.EquipmentTechId == sensor.Equipment.TechId) && (sensorRegResult.SensorName == sensor.Name))
                            {
                                this._logger.Info(Contexts.RegisterSensorTaskWorker, Events.SensorAlreadyExists.With(sensorRegResult.SensorName, sensorRegResult.EquipmentTechId));
                                this._logger.Error(Contexts.RegisterSensorTaskWorker, Categories.Processing, Exceptions.DeviceServerCanNotBeStarted);
                            }
                        }
                        else
                        {
                            this._logger.Info(Contexts.RegisterSensorTaskWorker, Events.MessageTimedOut);
                        }
                    }
                }
                else if ((allSensor != null) && (allSensor.Length > 0))
                {
                    baseContext.activeSensor = allSensor[0];
                }
                context.Finish();
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.RegisterSensorTaskWorker, Categories.Processing, Exceptions.UnknownErrorRegisterSensorTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
