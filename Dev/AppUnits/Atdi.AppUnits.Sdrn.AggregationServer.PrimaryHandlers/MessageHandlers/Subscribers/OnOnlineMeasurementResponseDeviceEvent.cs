using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DM = Atdi.DataModels.Sdrns.Server.Entities;
using System.Threading.Tasks;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Sdrns.Server;
using Atdi.DataModels.Sdrns.Device;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.MessageHandlers
{
    [SubscriptionEvent(EventName = "OnOnlineMeasurementResponseDevice", SubscriberName = "SubscriberOnOnlineMeasurementResponseDeviceEvent")]
    public class OnOnlineMeasurementResponseDeviceEvent : IEventSubscriber<OnOnlineMeasurementResponseDevice>
    {
        
        private readonly ILogger _logger;
        private readonly IPublisher _publisher;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        

        public OnOnlineMeasurementResponseDeviceEvent(IDataLayer<EntityDataOrm> dataLayer,  ILogger logger, ISdrnServerEnvironment environment, IPublisher publisher)
        {
            this._logger = logger;
            this._environment = environment;
            this._dataLayer = dataLayer;
            this._logger = logger;
            this._publisher = publisher;
        }

        public void Notify(OnOnlineMeasurementResponseDevice @event)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnOnlineMeasurementResponseDeviceEvent, this))
            {
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        var linkOnlineMesurementQuery = _dataLayer.GetBuilder<DM.ILinkOnlineMesurement>()
                        .From()
                        .Select(c => c.ONLINE_MEAS.Id)
                        .Select(c => c.ONLINE_MEAS.StatusCode)
                        .Select(c => c.OnlineMesurementMasterId)
                        .Select(c => c.Id)
                        .Where(c => c.ONLINE_MEAS.Id, ConditionOperator.Equal, @event.OnlineMeasId);
                        var linkOnlineMesurementExists = scope.Executor.ExecuteAndFetch(linkOnlineMesurementQuery, reader =>
                        {
                            var exists = reader.Read();
                            if (exists)
                            {
                                exists = reader.GetValue(c => c.ONLINE_MEAS.Id) == @event.OnlineMeasId;
                                if (exists)
                                {
                                //Отправка сообщения в шину DataBus в MasterServer
                                var retEnvelope = this._publisher.CreateEnvelope<OnlineMeasResponseDeviceToMasterServer, OnlineMeasurementResponseData>();
                                    retEnvelope.To = this._environment.MasterServerInstance;
                                    retEnvelope.DeliveryObject = new OnlineMeasurementResponseData()
                                    {
                                        OnlineMeasId = reader.GetValue(c => c.OnlineMesurementMasterId),
                                        Conformed = @event.Conformed,
                                        Message = @event.Message,
                                        Token = @event.Token,
                                        WebSocketUrl = @event.WebSocketUrl,
                                        StatusCode = reader.GetValue(c => c.ONLINE_MEAS.StatusCode)
                                    };
                                    this._publisher.Send(retEnvelope);
                                }
                            }
                            return exists;
                        });
                    }
                }
                catch (Exception e)
                {
                    _logger.Exception(Contexts.ThisComponent, (EventCategory)"OnInitOnlineMeasurement", e, this);
                    throw;
                }
            }
        }
      
    }
}
