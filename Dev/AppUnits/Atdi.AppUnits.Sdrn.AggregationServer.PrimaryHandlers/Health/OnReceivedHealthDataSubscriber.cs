using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Atdi.DataModels.Sdrns.Server;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Sdrns.Server.DataBus;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers
{
    [SubscriptionEvent(EventName = "OnReceivedHealthData", SubscriberName = "OnReceivedHealthDataSubscriber")]
    public class OnReceivedHealthDataSubscriber : IEventSubscriber<ReceivedHealthDataEvent>
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IPublisher _publisher;

        public OnReceivedHealthDataSubscriber(
	        IDataLayer<EntityDataOrm> dataLayer, 
	        ILogger logger, 
	        ISdrnServerEnvironment environment, 
	        IPublisher publisher)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._publisher = publisher;
        }
        public void Notify(ReceivedHealthDataEvent @event)
        {
            try
            {
	            using (var dbScope = this._dataLayer.CreateScope<SdrnServerDataContext>())
	            {
		            this.Handle(dbScope, @event);
				}
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.EventProcessing, e, (object)this);
                throw;
            }
        }
        private void Handle(IDataLayerScope dbScope, ReceivedHealthDataEvent @event)
        {
	        var healthId = @event.HealthId;
			var detailQuery = this._dataLayer.GetBuilder<MD.IHealthLogDetail>()
					.Insert()
					.SetValue(c => c.HEALTH.Id, healthId)
					.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
					.SetValue(c => c.Message, "Received health data from Event System")
					.SetValue(c => c.Note, $"EventId='{@event.Id}'; EventName='{@event.Name}'; Source='{@event.Source}'")
					.SetValue(c => c.SiteTypeCode, (byte)MD.EnvironmentElementTypeCode.AggregationServer)
					.SetValue(c => c.SiteTypeName, MD.EnvironmentElementTypeCode.AggregationServer.ToString())
					.SetValue(c => c.SiteInstance, this._environment.ServerInstance)
					.SetValue(c => c.ThreadId, System.Threading.Thread.CurrentThread.ManagedThreadId)
					.SetValue(c => c.Source, "OnReceivedHealthDataSubscriber")
					.SetValue(c => c.SiteHost, Environment.MachineName)
				;
			dbScope.Executor.Execute(detailQuery);

			var selQuery = this._dataLayer.GetBuilder<MD.IHealthLogData>()
				.From()
				.Select(
					c => c.Id,
					c => c.EventCode,
					c => c.EventName,
					c => c.EventNote,
					c => c.SourceTypeCode,
					c => c.SourceTypeName,
					c => c.SourceInstance,
					c => c.SourceTechId,
					c => c.JsonData,
					c => c.DispatchTime,
					c => c.ReceivedTime,
					c => c.SourceHost
				)
				.Where(c => c.Id, ConditionOperator.Equal, healthId);

			var healthData = dbScope.Executor.Fetch(selQuery, reader =>
			{
				if (!reader.Read())
				{
					throw new InvalidOperationException($"Could not find health record by id #{healthId}");
				}

				

				return new HealthData
				{
					SenderLogId = reader.GetValue(c => c.Id),
					SenderTypeCode = (byte)EnvironmentElementType.AggregationServer,
					SenderTypeName = EnvironmentElementType.AggregationServer.ToString(),
					SenderInstance = this._environment.ServerInstance,
					SenderHost = Environment.MachineName,
					EventCode = reader.GetValue(c => c.EventCode),
					EventName = reader.GetValue(c => c.EventName),
					EventNote = reader.GetValue(c => c.EventNote),
					DispatchTime = reader.GetValue(c => c.DispatchTime),
					ReceivedTime = reader.GetValue(c => c.ReceivedTime),
					JsonData = reader.GetValue(c => c.JsonData),
					SourceTypeCode = reader.GetValue(c => c.SourceTypeCode),
					SourceTypeName = reader.GetValue(c => c.SourceTypeName),
					SourceInstance = reader.GetValue(c => c.SourceInstance),
					SourceTechId = reader.GetValue(c => c.SourceTechId),
					SourceHost = reader.GetValue(c => c.SourceHost)
				};
			});

			var selDetailQuery = this._dataLayer.GetBuilder<MD.IHealthLogDetail>()
				.From()
				.Select(
					c => c.CreatedDate,
					c => c.Message,
					c => c.Note,
					c => c.ThreadId,
					c => c.Source,
					c => c.SiteTypeCode,
					c => c.SiteTypeName,
					c => c.SiteInstance,
					c => c.SiteHost
				)
				.Where(c => c.HEALTH.Id, ConditionOperator.Equal, healthId)
				.OrderByAsc(c => c.Id);

			healthData.Detail = dbScope.Executor.Fetch(selDetailQuery, reader =>
			{
				var result = new List<HealthDataDetail>();
				while (reader.Read())
				{
					result.Add(new HealthDataDetail()
					{
						CreatedDate = reader.GetValue(c => c.CreatedDate),
						Source = reader.GetValue(c => c.Source),
						Message = reader.GetValue(c => c.Message),
						Note = reader.GetValue(c => c.Note),
						ThreadId = reader.GetValue(c => c.ThreadId),
						SiteTypeCode = reader.GetValue(c => c.SiteTypeCode),
						SiteTypeName = reader.GetValue(c => c.SiteTypeName),
						SiteInstance = reader.GetValue(c => c.SiteInstance),
						SiteHost = reader.GetValue(c => c.SiteHost)
					});
				}

				return result.ToArray();
			});

			try
			{
				var envelope = this._publisher.CreateEnvelope<HealthDataMessage, HealthData>();
				envelope.To = this._environment.MasterServerInstance;
				envelope.DeliveryObject = healthData;
				this._publisher.Send(envelope);
			}
			catch (Exception e)
			{
				detailQuery = this._dataLayer.GetBuilder<MD.IHealthLogDetail>()
						.Insert()
						.SetValue(c => c.HEALTH.Id, healthId)
						.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
						.SetValue(c => c.Message, "Failed to send health data to Master Server")
						.SetValue(c => c.Note, e.ToString())
						.SetValue(c => c.SiteTypeCode, (byte)MD.EnvironmentElementTypeCode.AggregationServer)
						.SetValue(c => c.SiteTypeName, MD.EnvironmentElementTypeCode.AggregationServer.ToString())
						.SetValue(c => c.SiteInstance, this._environment.ServerInstance)
						.SetValue(c => c.ThreadId, System.Threading.Thread.CurrentThread.ManagedThreadId)
						.SetValue(c => c.Source, "OnReceivedHealthDataSubscriber")
						.SetValue(c => c.SiteHost, Environment.MachineName)
					;
				dbScope.Executor.Execute(detailQuery);
				throw;
			}

			var forwardedTime = DateTimeOffset.Now;
			var updateQuery = this._dataLayer.GetBuilder<MD.IHealthLog>()
				.Update()
				.SetValue(c => c.ForwardedTime, forwardedTime)
				.Where(c => c.Id, ConditionOperator.Equal, healthId);
			dbScope.Executor.Execute(updateQuery);

			detailQuery = this._dataLayer.GetBuilder<MD.IHealthLogDetail>()
					.Insert()
					.SetValue(c => c.HEALTH.Id, healthId)
					.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
					.SetValue(c => c.Message, "Forwarded health data to Master Server")
					.SetValue(c => c.Note, $"MasterServerInstance='{this._environment.MasterServerInstance}'")
					.SetValue(c => c.SiteTypeCode, (byte)MD.EnvironmentElementTypeCode.AggregationServer)
					.SetValue(c => c.SiteTypeName, MD.EnvironmentElementTypeCode.AggregationServer.ToString())
					.SetValue(c => c.SiteInstance, this._environment.ServerInstance)
					.SetValue(c => c.ThreadId, System.Threading.Thread.CurrentThread.ManagedThreadId)
					.SetValue(c => c.Source, "OnReceivedHealthDataSubscriber")
					.SetValue(c => c.SiteHost, Environment.MachineName)
				;
			dbScope.Executor.Execute(detailQuery);

			
		}
    }
}
