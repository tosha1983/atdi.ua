
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
    public class SendEntityHandler : ISdrnMessageHandler<MSG.Device.SendEntityMessage, Entity>
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public SendEntityHandler(
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

        public void Handle(ISdrnIncomingEnvelope<Entity> incomingEnvelope, ISdrnMessageHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.MessageProcessing, this))
            {
                string idEntity = "";
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                try
                {
                    result.Status = SdrnMessageHandlingStatus.Trash;
                    var entityObject = incomingEnvelope.DeliveryObject;

                    queryExecuter.BeginTransaction();
                    var builderInsertIEntity = this._dataLayer.GetBuilder<MD.IEntity>().Insert();
                    builderInsertIEntity.SetValue(c => c.ContentType, entityObject.ContentType);
                    builderInsertIEntity.SetValue(c => c.Description, entityObject.Description);
                    builderInsertIEntity.SetValue(c => c.HashAlgoritm, entityObject.HashAlgorithm);
                    builderInsertIEntity.SetValue(c => c.HashCode, entityObject.HashCode);
                    builderInsertIEntity.SetValue(c => c.Name, entityObject.Name);
                    builderInsertIEntity.SetValue(c => c.ParentId, entityObject.ParentId);
                    builderInsertIEntity.SetValue(c => c.ParentType, entityObject.ParentType);

                    builderInsertIEntity.Select(c => c.Id);
                    queryExecuter
                     .ExecuteAndFetch(builderInsertIEntity, reader =>
                     {
                         var resultValue = reader.Read();
                         if (resultValue)
                         {
                             idEntity = reader.GetValue(c => c.Id);
                         }
                         return resultValue;
                     });

                    queryExecuter.CommitTransaction();
                    // с этого момента нужно считать что сообщение удачно обработано
                    result.Status = SdrnMessageHandlingStatus.Confirmed;
                    this._eventEmitter.Emit("OnSendEntity", "SendEntityProccesing");
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
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    var deviceCommandResult = new DeviceCommand
                    {
                        EquipmentTechId = incomingEnvelope.SensorTechId,
                        SensorName = incomingEnvelope.SensorName,
                        SdrnServer = this._environment.ServerInstance,
                        Command = "SendEntityResult",
                        CommandId = "SendCommand",
                        CustTxt1 = "Success"
                    };

                    if (result.Status == SdrnMessageHandlingStatus.Error)
                    {
                        deviceCommandResult.CustTxt1 = "Error";
                    }
                    else if (!string.IsNullOrEmpty(idEntity))
                    {
                        deviceCommandResult.CustTxt1 = "Success";
                    }
                    else
                    {
                        deviceCommandResult.CustTxt1 = "Error";
                    }
                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DeviceCommand>();
                    envelop.SensorName = incomingEnvelope.SensorName;
                    envelop.SensorTechId = incomingEnvelope.SensorTechId;
                    envelop.DeliveryObject = deviceCommandResult;
                    _messagePublisher.Send(envelop);
                }

            }
        
        }

    }
}


