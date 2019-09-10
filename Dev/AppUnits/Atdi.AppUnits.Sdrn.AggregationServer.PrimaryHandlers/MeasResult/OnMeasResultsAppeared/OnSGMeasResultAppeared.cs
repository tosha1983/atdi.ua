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
    [SubscriptionEvent(EventName = "OnSGMeasResultAppeared", SubscriberName = "SGMeasResultSubscriber")]
    public class OnSGMeasResultAppeared : IEventSubscriber<SGMeasResultAppeared>
    {
        private readonly ILogger _logger;
        private readonly IEventEmitter _eventEmitter;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;

        public OnSGMeasResultAppeared(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger, ISdrnServerEnvironment environment)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();
        }
        public void Notify(SGMeasResultAppeared @event)
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
            using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
            {
                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeasSignaling>().Insert();
                builderInsertIResMeas.SetValue(c => c.RES_MEAS.Id, measResultId);
                builderInsertIResMeas.SetValue(c => c.IsSend, false);
                var resMeasSG = scope.Executor.Execute<MD.IResMeasSignaling_PK>(builderInsertIResMeas);
            }

            var builderResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().From();
            builderResMeas.Select(c => c.SUBTASK_SENSOR.SENSOR.Id, c => c.Status, c => c.TimeMeas);
            builderResMeas.Where(c => c.Id, ConditionOperator.Equal, measResultId);
            
            this._queryExecutor.Fetch(builderResMeas, readerResMeas =>
            {
                while (readerResMeas.Read())
                {
                    var status = readerResMeas.GetValue(c => c.Status).ToUpper();
                    var timeMeas = readerResMeas.GetValue(c => c.TimeMeas);

                    if (timeMeas.HasValue && (status == "C" || status == "COMPLETE" || status == "COMPLETED"))
                    {
                        var busEvent = new SGMeasResultAggregated($"OnSGMeasResultAggregated", "OnSGMeasResultAppeared") { MeasResultId = measResultId };
                        _eventEmitter.Emit(busEvent);

                        using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                        {
                            var builderUpdateIResMeas = this._dataLayer.GetBuilder<MD.IResMeasSignaling>().Update();
                            builderUpdateIResMeas.SetValue(c => c.IsSend, true);
                            builderUpdateIResMeas.Where(c => c.RES_MEAS.TimeMeas, ConditionOperator.Between, timeMeas.Value.Date, new DateTime(timeMeas.Value.Year, timeMeas.Value.Month, timeMeas.Value.Day, 23, 59, 59));
                            builderUpdateIResMeas.Where(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SENSOR.Id));
                            scope.Executor.Execute(builderUpdateIResMeas);
                        }
                    }
                }
                return true;
            });
        }
    }
}


