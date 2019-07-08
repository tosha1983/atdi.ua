using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.BusMessages.Server;
using Atdi.Platform.Logging;
using System;
using Atdi.DataModels.EntityOrm;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendRegistrationResultHandler : MessageHandlerBase<DM.SensorRegistrationResult, SendRegistrationResultMessage>
    {
        private readonly IRepository<DM.Sensor, long?> _repositorySensor;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private readonly IRepository<LastUpdate, long?> _repositoryLastUpdateByInt;


        public SendRegistrationResultHandler(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            IRepository<DM.Sensor, long?> repositorySensor,
            ITaskStarter taskStarter,
            IRepository<LastUpdate, long?> repositoryLastUpdateByInt,
            ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._repositorySensor = repositorySensor;
            this._repositoryLastUpdateByInt = repositoryLastUpdateByInt;
        }

        public override void OnHandle(IReceivedMessage<DM.SensorRegistrationResult> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            try
            {

                DM.SensorRegistrationResult sensorRegistrationResult = message.Data;

                var loadSensors = this._repositorySensor.LoadAllObjects();
                //если в БД не обнаружено сведений о сенсоре, тогда:
                if ((loadSensors != null) && (loadSensors.Length >= 0))
                {
                    var sensorCheckConfirmed = loadSensors[0];

                    if ((sensorRegistrationResult.Status == "Success") && (sensorRegistrationResult.EquipmentTechId == sensorCheckConfirmed.Equipment.TechId) && (sensorRegistrationResult.SensorName == sensorCheckConfirmed.Name))
                    {
                        sensorCheckConfirmed.Status = "A";

                        var idSensor = this._repositorySensor.Update(sensorCheckConfirmed);

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

                        this._logger.Info(Contexts.ThisComponent, Events.ReceivedSensorRegistrationConfirmation);
                    }
                    else if ((sensorRegistrationResult.Status == "Reject") && (sensorRegistrationResult.EquipmentTechId == sensorCheckConfirmed.Equipment.TechId) && (sensorRegistrationResult.SensorName == sensorCheckConfirmed.Name))
                    {
                        this._logger.Error(Contexts.ThisComponent, Categories.Processing, Exceptions.DeviceServerCanNotBeStarted);
                    }
                    message.Result = MessageHandlingResult.Confirmed;
                }
            }
            catch (Exception e)
            {
                message.Result = MessageHandlingResult.Error;
                this._logger.Error(Contexts.ThisComponent, Exceptions.UnknownErrorsInSendRegistrationResultHandler, e.Message);
            }
        }
    }
}

