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
using Atdi.Platform;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendEntityDeviceBusEvent", SubscriberName = "SendEntitySubscriber")]
    public class SendEntitySubscriber : SubscriberBase<DM.Entity>
    {

        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IStatistics _statistics;

        private readonly IStatisticCounter _messageProcessingHitsCounter;
        private readonly IStatisticCounter _sendEntityHitsCounter;
        private readonly IStatisticCounter _sendEntityErrorsCounter;

        public SendEntitySubscriber(
            ISdrnMessagePublisher messagePublisher, 
            IMessagesSite messagesSite, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment,
            IStatistics statistics,
            ILogger logger) 
            : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._statistics = statistics;
            if (this._statistics != null)
            {
                this._messageProcessingHitsCounter = _statistics.Counter(Monitoring.Counters.MessageProcessingHits);
                this._sendEntityHitsCounter = _statistics.Counter(Monitoring.Counters.SendEntityHits);
                this._sendEntityErrorsCounter = _statistics.Counter(Monitoring.Counters.SendEntityErrors);
            }
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.Entity deliveryObject, long messageId)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                this._messageProcessingHitsCounter?.Increment();
                this._sendEntityHitsCounter?.Increment();

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

                        scope.Executor.Execute(builderInsertIEntity);

                        scope.Commit();
                        
                        // с этого момента нужно считать что сообщение удачно обработано
                        status = SdrnMessageHandlingStatus.Confirmed;

                    }
                }
                catch (Exception e)
                {
                    this._sendEntityErrorsCounter?.Increment();
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
