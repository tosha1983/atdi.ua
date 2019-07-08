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
    [SubscriptionEvent(EventName = "OnSendEntityPartDeviceBusEvent", SubscriberName = "SendEntityPartSubscriber")]
    public class SendEntityPartSubscriber : SubscriberBase<DM.EntityPart>
    {

        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        public SendEntityPartSubscriber(ISdrnMessagePublisher messagePublisher, IMessagesSite messagesSite, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger) : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
        }



        protected override void Handle(string sensorName, string sensorTechId, DM.EntityPart deliveryObject)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                var status = SdrnMessageHandlingStatus.Unprocessed;
                try
                {
                    var entityObject = deliveryObject;
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        scope.BeginTran();

                        var builderInsertIEntityPart = this._dataLayer.GetBuilder<MD.IEntityPart>().Insert();
                        builderInsertIEntityPart.SetValue(c => c.Content, entityObject.Content);
                        builderInsertIEntityPart.SetValue(c => c.ENTITY.Id, entityObject.EntityId);
                        builderInsertIEntityPart.SetValue(c => c.Eof, entityObject.EOF);
                        builderInsertIEntityPart.SetValue(c => c.PartIndex, entityObject.PartIndex);

                        scope.Executor
                            .Execute(builderInsertIEntityPart);

                        scope.Commit();
                        
                        // с этого момента нужно считать что сообщение удачно обработано
                        status = SdrnMessageHandlingStatus.Confirmed;
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
                        Command = "SendEntityPartResult",
                        CommandId = "SendCommand",
                        CustTxt1 = "Success"
                    };

                    if (status == SdrnMessageHandlingStatus.Error)
                    {
                        deviceCommandResult.CustTxt1 = "Error";
                    }
                    else if (status == SdrnMessageHandlingStatus.Confirmed)
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
