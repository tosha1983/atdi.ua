using Atdi.DataModels.Api.EventSystem;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendEntityDeviceBusEvent", SubscriberName = "SendEntitySubscriber")]
    public class SendEntitySubscriber : SubscriberBase<DM.Entity>
    {

        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        public SendEntitySubscriber(ISdrnMessagePublisher messagePublisher, IMessagesSite messagesSite, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger) : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.Entity deliveryObject)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                var cntNewRecords = 0;
                var status = SdrnMessageHandlingStatus.Unprocessed;
                try
                {
                    var entityObject = deliveryObject;
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        scope.BeginTran();

                        var builderInsertIEntity = this._dataLayer.GetBuilder<MD.IEntity>().Insert();
                        builderInsertIEntity.SetValue(c => c.ContentType, entityObject.ContentType);
                        builderInsertIEntity.SetValue(c => c.Description, entityObject.Description);
                        builderInsertIEntity.SetValue(c => c.HashAlgoritm, entityObject.HashAlgorithm);
                        builderInsertIEntity.SetValue(c => c.HashCode, entityObject.HashCode);
                        builderInsertIEntity.SetValue(c => c.Name, entityObject.Name);
                        builderInsertIEntity.SetValue(c => c.ParentId, entityObject.ParentId);
                        builderInsertIEntity.SetValue(c => c.ParentType, entityObject.ParentType);
                        builderInsertIEntity.SetValue(c => c.Id, entityObject.EntityId);

                        cntNewRecords = scope.Executor.Execute(builderInsertIEntity);

                        scope.Commit();
                        // с этого момента нужно считать что сообщение удачно обработано
                        status = SdrnMessageHandlingStatus.Confirmed;
                        //this._eventEmitter.Emit("OnSendEntity", "SendEntityProccesing");
                    }
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    status = SdrnMessageHandlingStatus.Error;
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    var deviceCommandResult = new DeviceCommand
                    {
                        EquipmentTechId = sensorTechId,
                        SensorName = sensorName,
                        SdrnServer = this._environment.ServerInstance,
                        Command = "SendEntityResult",
                        CommandId = "SendCommand",
                        CustTxt1 = "Success"
                    };

                    if (status == SdrnMessageHandlingStatus.Error)
                    {
                        deviceCommandResult.CustTxt1 = "Error";
                    }
                    else if (cntNewRecords>0)
                    {
                        deviceCommandResult.CustTxt1 = "Success";
                    }
                    else
                    {
                        deviceCommandResult.CustTxt1 = "Error";
                    }
                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DeviceCommand>();
                    envelop.SensorName = sensorName;
                    envelop.SensorTechId = sensorTechId;
                    envelop.DeliveryObject = deviceCommandResult;
                    _messagePublisher.Send(envelop);
                }

            }
        }
    }
}
