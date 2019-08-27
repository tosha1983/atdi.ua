using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Common;
using Atdi.Platform;
using Atdi.Platform.Caching;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers
{
    [SubscriptionEvent(EventName = "OnMSMeasResultAppeared", SubscriberName = "MSMeasResultSubscriber")]
    public class OnMSMeasResultAppeared : IEventSubscriber<MSMeasResultAppeared>
    {
        private readonly ILogger _logger;
        private readonly IEventEmitter _eventEmitter;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;

        public OnMSMeasResultAppeared(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger, ISdrnServerEnvironment environment)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();
        }
        public void Notify(MSMeasResultAppeared @event)
        {
            try
            {
                this.Handle(@event.MeasResultId);
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.EventProcessing, e, (object)this);
            }
        }
        private void Handle(long measResultId)
        {
            // Some aggregaton code

            var busEvent = new MSMeasResultAggregated($"MSMeasResultAggregated", "OnMSMeasResultAppeared")
            {
                MeasResultId = measResultId
            };
            _eventEmitter.Emit(busEvent);
        }
    }
}
