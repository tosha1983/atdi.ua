
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using MSG = Atdi.DataModels.Sdrns.BusMessages;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers
{
    public class SendCommandResultHandler : ISdrnMessageHandler<MSG.Device.SendCommandResultMessage, DeviceCommandResult>
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public SendCommandResultHandler(
            ISdrnMessagePublisher messagePublisher, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment, 
            IEventEmitter eventEmitter, ILogger logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }

        public void Handle(ISdrnIncomingEnvelope<DeviceCommandResult> incomingEnvelope, ISdrnMessageHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.MessageProcessing, this))
            {
                this._eventEmitter.Emit("OnEvent2", "SendCommandResultProcess");
                result.Status = SdrnMessageHandlingStatus.Trash;
                var sensorExistsInDb = false;
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                try
                {
                    queryExecuter.BeginTransaction();
                    switch (incomingEnvelope.DeliveryObject.CommandId)
                    {
                        case "SendActivitySensor":
                            var query = this._dataLayer.GetBuilder<MD.ISensor>()
                       .From()
                       .Select(c => c.Name)
                       .Select(c => c.Id)
                       .Where(c => c.Name, ConditionOperator.Equal, incomingEnvelope.SensorName)
                       .Where(c => c.TechId, ConditionOperator.Equal, incomingEnvelope.SensorTechId)
                       .OrderByAsc(c => c.Id);
                            sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
                            .Execute(query) == 1;

                            if (sensorExistsInDb == true)
                            {
                                var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensor>().Update();
                                builderUpdateSensor.Where(c => c.TechId, ConditionOperator.Equal, incomingEnvelope.SensorTechId);
                                builderUpdateSensor.Where(c => c.Name, ConditionOperator.Equal, incomingEnvelope.SensorName);
                                builderUpdateSensor.SetValue(c => c.Status, "A");
                                var cnt = queryExecuter
                                 .Execute(builderUpdateSensor);
                                if (cnt == 1)
                                {
                                    queryExecuter.CommitTransaction();
                                    result.Status = SdrnMessageHandlingStatus.Confirmed;
                                    this._eventEmitter.Emit("OnSensorActivity", "ActivitySensorProccesing");
                                }
                            }
                            break;
                        case "UpdateStatusMeasTask":
                            var queryMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, (int?)incomingEnvelope.DeliveryObject.CustNbr1)
                                .SetValue(c => c.Status, incomingEnvelope.DeliveryObject.Status);
                            var updated = queryExecuter
                            .Execute(queryMeasTask) == 1;
                            if (updated == true)
                            {
                                queryExecuter.CommitTransaction();
                                result.Status = SdrnMessageHandlingStatus.Confirmed;
                                this._eventEmitter.Emit("OnUpdateStatusMeasTask", "UpdateStatusMeasTaskProccesing");
                            }
                            break;
                        case "UpdateSensorLocation":
                            throw new NotImplementedException($"Handle for CommandId {incomingEnvelope.DeliveryObject.CommandId} not implemented");
                        default:
                            throw new NotImplementedException($"Handle for CommandId {incomingEnvelope.DeliveryObject.CommandId} not implemented");
                    }
                }
                catch (Exception e)
                {
                    queryExecuter.RollbackTransaction();
                    this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
                    if (result.Status == SdrnMessageHandlingStatus.Unprocessed)
                    {
                        result.Status = SdrnMessageHandlingStatus.Error;
                        result.ReasonFailure = e.ToString();
                    }
                }

            }
        }

    }
}


