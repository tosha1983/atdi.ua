using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Contracts.Sdrn.Server;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnUpdateMeasTaskEvent", SubscriberName = "SubscriberOnUpdateMeasTaskEvent")]
    public class OnReceivedUpdateMeasTaskEvent : IEventSubscriber<OnMeasTaskEvent>
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public OnReceivedUpdateMeasTaskEvent(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        public void Notify(OnMeasTaskEvent @event)
        {
            // Механизм обновления таска и перерасчета результатов
        }
    }
}
