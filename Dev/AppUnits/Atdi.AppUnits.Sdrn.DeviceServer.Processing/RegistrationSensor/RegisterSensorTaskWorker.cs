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
using System.Collections.Generic;
using System.Linq;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// Воркер по обработке процедуры регистрации сенсора
    /// </summary>
    public class RegisterSensorTaskWorker : ITaskWorker<RegisterSensorTask, BaseContext, SingletonTaskWorkerLifetime>
    {
        private readonly IRepository<DM.Sensor, int?> _repositorySensor;
        private readonly ILogger _logger;
        private readonly IBusGate _busGate;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly IDeviceServerConfig _deviceServerConfig;
        private readonly ConfigProcessing _config;
        private readonly IController _controller;

        public RegisterSensorTaskWorker(IRepository<DM.Sensor, int?> repositorySensor,
            ILogger logger,
            IBusGate busGate,
            IServicesResolver resolver,
            IServicesContainer servicesContainer,
            ITimeService timeService,
            ITaskStarter taskStarter,
            ConfigProcessing config,
            IController controller,
            IDeviceServerConfig deviceServerConfig
            )
        {
            this._logger = logger;
            this._repositorySensor = repositorySensor;
            this._busGate = busGate;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._deviceServerConfig = deviceServerConfig;
            this._config = config;
            this._controller = controller;
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
                //если в БД не обнаружено сведений о сенсоре, тогда:
                if (((allSensor == null) || ((allSensor != null) && (allSensor.Length == 0))))
                {
                    bool isFindDeviceProps = false;
                    DM.Sensor sensor = null;
                    var deviceProperties = this._controller.GetDevicesProperties();
                    var listTraceDeviceProperties = deviceProperties.Values.ToList();
                    for (int i = 0; i < listTraceDeviceProperties.Count; i++)
                    {
                        var traceDeviceProperties = listTraceDeviceProperties[i];
                        for (int j = 0; j < traceDeviceProperties.Length; j++)
                        {
                            var trace = traceDeviceProperties[j];
                            if (trace is MesureTraceDeviceProperties)
                            {
                                var mesureTraceDeviceProperties = (trace as MesureTraceDeviceProperties);
                                var standardDeviceProperties = mesureTraceDeviceProperties.StandardDeviceProperties;
                                if (standardDeviceProperties != null)
                                {
                                    var equipmentInfo = standardDeviceProperties.EquipmentInfo;
                                    if (equipmentInfo != null)
                                    {
                                        //преобразование в DM.Sensor
                                        //передаем наименование сенсора из лицензии this._deviceServerConfig.SensorName
                                        sensor = mesureTraceDeviceProperties.Convert(this._deviceServerConfig.SensorName, this._deviceServerConfig.SensorTechId);
                                        isFindDeviceProps = true;
                                        break;
                                    }
                                }
                            }
                            else if (trace is MesureZeroSpanDeviceProperties)
                            {
                                var mesureZeroSpanDeviceProperties = (trace as MesureZeroSpanDeviceProperties);
                                //поиск сведений о девайсе по значению this._deviceServerConfig.SensorTechId
                                //если сведения найдены, тогда:
                                //преобразование в DM.Sensor
                                sensor = mesureZeroSpanDeviceProperties.Convert(this._deviceServerConfig.SensorName, this._deviceServerConfig.SensorTechId);
                                isFindDeviceProps = true;
                            }
                            else if (trace is MesureSignalParametersDeviceProperties)
                            {
                                var mesureSignalParametersDeviceProperties = (trace as MesureSignalParametersDeviceProperties);
                                //поиск сведений о девайсе по значению this._deviceServerConfig.SensorTechId
                                //если сведения найдены, тогда:
                                //преобразование в DM.Sensor
                                sensor = mesureSignalParametersDeviceProperties.Convert(this._deviceServerConfig.SensorName, this._deviceServerConfig.SensorTechId);
                                isFindDeviceProps = true;
                            }
                            else if (trace is MesureSysInfoDeviceProperties)
                            {
                                var mesureSysInfoDeviceProperties = (trace as MesureSysInfoDeviceProperties);
                                //поиск сведений о девайсе по значению this._deviceServerConfig.SensorTechId
                                //если сведения найдены, тогда:
                                //преобразование в DM.Sensor
                                sensor = mesureSysInfoDeviceProperties.Convert(this._deviceServerConfig.SensorName, this._deviceServerConfig.SensorTechId);
                                isFindDeviceProps = true;
                            }
                            else if (trace is MesureRealTimeDeviceProperties)
                            {
                                var mesureRealTimeDeviceProperties = (trace as MesureRealTimeDeviceProperties);
                                //поиск сведений о девайсе по значению this._deviceServerConfig.SensorTechId
                                //если сведения найдены, тогда:
                                //преобразование в DM.Sensor
                                sensor = mesureRealTimeDeviceProperties.Convert(this._deviceServerConfig.SensorName, this._deviceServerConfig.SensorTechId);
                                isFindDeviceProps = true;
                            }
                            else if (trace is MesureIQStreamDeviceProperties)
                            {
                                var mesureIQStreamDeviceProperties = (trace as MesureIQStreamDeviceProperties);
                                //поиск сведений о девайсе по значению this._deviceServerConfig.SensorTechId
                                //если сведения найдены, тогда:
                                //преобразование в DM.Sensor
                                sensor = mesureIQStreamDeviceProperties.Convert(this._deviceServerConfig.SensorName, this._deviceServerConfig.SensorTechId);
                                isFindDeviceProps = true;
                            }
                            else if (trace is MesureGpsLocationProperties)
                            {
                                var mesureGpsLocationProperties = (trace as MesureGpsLocationProperties);
                                //поиск сведений о девайсе по значению this._deviceServerConfig.SensorTechId
                                //если сведения найдены, тогда:
                                //преобразование в DM.Sensor
                                sensor = mesureGpsLocationProperties.Convert(this._deviceServerConfig.SensorName, this._deviceServerConfig.SensorTechId);
                                isFindDeviceProps = true;
                            }
                            else if (trace is MesureAudioDeviceProperties)
                            {
                                var mesureAudioDeviceProperties = (trace as MesureAudioDeviceProperties);
                                //поиск сведений о девайсе по значению this._deviceServerConfig.SensorTechId
                                //если сведения найдены, тогда:
                                //преобразование в DM.Sensor
                                sensor = mesureAudioDeviceProperties.Convert(this._deviceServerConfig.SensorName, this._deviceServerConfig.SensorTechId);
                                isFindDeviceProps = true;
                            }
                            else if (trace is MesureDFDeviceProperties)
                            {
                                var mesureDFDeviceProperties = (trace as MesureDFDeviceProperties);
                                //поиск сведений о девайсе по значению this._deviceServerConfig.SensorTechId
                                //если сведения найдены, тогда:
                                //преобразование в DM.Sensor
                                sensor = mesureDFDeviceProperties.Convert(this._deviceServerConfig.SensorName, this._deviceServerConfig.SensorTechId);
                                isFindDeviceProps = true;
                            }
                            else
                            {
                                throw new Exception($"Type {trace.GetType()} is not supported");
                            }
                        }
                        if (isFindDeviceProps)
                        {
                            break;
                        }
                    }
                    if (sensor != null)
                    {
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
                                    if (idSensor > 0)
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
                    else
                    {
                        _logger.Error(Contexts.RegisterSensorTaskWorker, Categories.Processing, Exceptions.NotFoundInformationWithGetDevicesProperties);
                        throw new Exception(Exceptions.NotFoundInformationWithGetDevicesProperties);
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
