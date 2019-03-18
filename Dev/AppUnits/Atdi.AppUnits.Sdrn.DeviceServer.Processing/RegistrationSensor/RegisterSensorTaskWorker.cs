using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using Atdi.DataModels.EntityOrm;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using System.Collections.Generic;
using System.Linq;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// Воркер по обработке процедуры регистрации сенсора
    /// </summary>
    public class RegisterSensorTaskWorker : ITaskWorker<RegisterSensorTask, DispatchProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IRepository<DM.Sensor, int?> _repositorySensor;
        private readonly ILogger _logger;
        private readonly IBusGate _busGate;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly IDeviceServerConfig _deviceServerConfig;
        private readonly ConfigProcessing _config;
        private readonly IController _controller;
        private readonly IRepository<LastUpdate, int?> _repositoryLastUpdateByInt;


        public RegisterSensorTaskWorker(IRepository<DM.Sensor, int?> repositorySensor,
            ILogger logger,
            IBusGate busGate,
            ITimeService timeService,
            ITaskStarter taskStarter,
            ConfigProcessing config,
            IRepository<LastUpdate, int?> repositoryLastUpdateByInt,
            IController controller,
            IDeviceServerConfig deviceServerConfig
            )
        {
            this._logger = logger;
            this._repositorySensor = repositorySensor;
            this._busGate = busGate;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._deviceServerConfig = deviceServerConfig;
            this._config = config;
            this._controller = controller;
            this._repositoryLastUpdateByInt = repositoryLastUpdateByInt;
        }

        public void Run(ITaskContext<RegisterSensorTask, DispatchProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.RegisterSensorTaskWorker, Categories.Processing, Events.StartRegisterSensorTaskWorker.With(context.Task.Id));
                var allSensor = this._repositorySensor.LoadAllObjects();
                //если в БД не обнаружено сведений о сенсоре, тогда:
                if (((allSensor == null) || ((allSensor != null) && (allSensor.Length == 0))))
                {
                    bool isFindDeviceProps = false;
                    DM.Sensor sensor = null;
                    var deviceProperties = this._controller.GetDevicesProperties();
                    var listTraceDeviceProperties = deviceProperties.Values.ToArray();
                    for (int i = 0; i < listTraceDeviceProperties.Length; i++)
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
                        //сохранение сведений о сенсоре в БД
                        var idSensor = this._repositorySensor.Create(sensor);
                        if (idSensor > 0)
                        {
                            context.Process.activeSensor = sensor;

                            var lastUpdate = new LastUpdate()
                            {
                                TableName = "XBS_SENSOR",
                                LastDateTimeUpdate = DateTime.Now,
                                Status = "N"
                            };

                            var allTablesLastUpdated = this._repositoryLastUpdateByInt.LoadAllObjects();
                            if ((allTablesLastUpdated != null) && (allTablesLastUpdated.Length > 0))
                            {
                                var listAlTables = allTablesLastUpdated.ToList();
                                var findTableProperties = listAlTables.Find(z => z.TableName == "XBS_SENSOR");
                                if (findTableProperties != null)
                                {
                                    this._repositoryLastUpdateByInt.Update(lastUpdate);
                                }
                                else
                                {
                                    this._repositoryLastUpdateByInt.Create(lastUpdate);
                                }
                            }
                            else
                            {
                                this._repositoryLastUpdateByInt.Create(lastUpdate);
                            }


                            this._logger.Info(Contexts.RegisterSensorTaskWorker, Events.SensorInformationRecordedDB);

                            var publisher = this._busGate.CreatePublisher("main");
                            publisher.Send<DM.Sensor>("RegisterSensor", sensor);
                            publisher.Dispose();

                        }
                        else
                        {
                            this._logger.Info(Contexts.RegisterSensorTaskWorker, Events.SensorInformationNotRecordedDB);
                            throw new Exception(Events.SensorInformationNotRecordedDB.Text);
                        }
                      
                       
                        // ожидаем получение сообщения с подтверждением от SDRN сервера об успешной регистрации сенсора
                        while (true)
                        {

                            var loadSensors = this._repositorySensor.LoadAllObjects();
                            //если в БД не обнаружено сведений о сенсоре, тогда:
                            if ((loadSensors != null) && (loadSensors.Length >= 0))
                            {
                                var sensorCheckConfirmed = loadSensors[0];
                                if (sensorCheckConfirmed.Status=="A")
                                {
                                    this._logger.Info(Contexts.RegisterSensorTaskWorker, Events.ReceivedSensorRegistrationConfirmation);
                                    context.Process.activeSensor = sensorCheckConfirmed;
                                    break;
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(this._config.MaxTimeOutReceiveSensorRegistrationResult);
                                }
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
                    var sensor = allSensor[0];
                    if (sensor.Status == "A")
                    {
                        context.Process.activeSensor = allSensor[0];
                    }
                    if (sensor.Status == "NOT_CONFIRMED")
                    {
                        while (true)
                        {
                            var loadSensors = this._repositorySensor.LoadAllObjects();
                            //если в БД не обнаружено сведений о сенсоре, тогда:
                            if ((loadSensors != null) && (loadSensors.Length >= 0))
                            {
                                var sensorCheckConfirmed = loadSensors[0];
                                if (sensorCheckConfirmed.Status == "A")
                                {
                                    this._logger.Info(Contexts.RegisterSensorTaskWorker, Events.ReceivedSensorRegistrationConfirmation);
                                    context.Process.activeSensor = sensorCheckConfirmed;
                                    break;
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(this._config.MaxTimeOutReceiveSensorRegistrationResult);
                                }
                            }
                        }
                    }
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
