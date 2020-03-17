using Atdi.Contracts.Api.DataBus;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Api.DataBus;
using Atdi.DataModels.Sdrns.Server;
using Atdi.DataModels.Sdrns.Server.DataBus;
using Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.Health
{
	public sealed class HealthDataMessageHandler : IMessageHandler<HealthDataMessage, HealthData>
	{
		private readonly ISdrnServerEnvironment _environment;
		private readonly IDataLayer<EntityDataOrm> _dataLayer;
		private readonly ILogger _logger;

		public HealthDataMessageHandler(
			ISdrnServerEnvironment environment,
			IDataLayer<EntityDataOrm> dataLayer, 
			ILogger logger)
		{
			_environment = environment;
			_dataLayer = dataLayer;
			_logger = logger;
		}
		public void Handle(IIncomingEnvelope<HealthDataMessage, HealthData> envelope, IHandlingResult result)
		{
			var receivedTime = DateTimeOffset.Now;
			try
			{
				var healthData= envelope.DeliveryObject;
				using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
				{
					var insQuery = this._dataLayer.GetBuilder<IHealthLogData>().Insert();
					insQuery.SetValue(c => c.DispatchTime, healthData.DispatchTime);
					insQuery.SetValue(c => c.ReceivedTime, receivedTime);
					insQuery.SetValue(c => c.SenderLogId, healthData.SenderLogId);
					insQuery.SetValue(c => c.SenderTypeCode, healthData.SenderTypeCode);
					insQuery.SetValue(c => c.SenderTypeName, healthData.SenderTypeName);
					insQuery.SetValue(c => c.SenderInstance, healthData.SenderInstance);
					insQuery.SetValue(c => c.SenderHost, healthData.SenderHost);
					insQuery.SetValue(c => c.SourceTypeCode, healthData.SourceTypeCode);
					insQuery.SetValue(c => c.SourceTypeName, healthData.SourceTypeName);
					insQuery.SetValue(c => c.SourceInstance, healthData.SourceInstance);
					insQuery.SetValue(c => c.SourceTechId, healthData.SourceTechId);
					insQuery.SetValue(c => c.SourceHost, healthData.SourceHost);
					insQuery.SetValue(c => c.EventCode, healthData.EventCode);
					insQuery.SetValue(c => c.EventName, healthData.EventName);
					insQuery.SetValue(c => c.EventNote, healthData.EventNote);
					insQuery.SetValue(c => c.JsonData, healthData.JsonData);

					var health = scope.Executor.Execute<IHealthLogData_PK>(insQuery);

					for (int i = 0; i < healthData.Detail.Length; i++)
					{
						var detail = healthData.Detail[i];
						var sourceDetailQuery = this._dataLayer.GetBuilder<IHealthLogDetail>()
								.Insert()
								.SetValue(c => c.HEALTH.Id, health.Id)
								.SetValue(c => c.CreatedDate, detail.CreatedDate)
								.SetValue(c => c.Message, detail.Message)
								.SetValue(c => c.Note, detail.Note )
								.SetValue(c => c.SiteTypeCode, detail.SiteTypeCode)
								.SetValue(c => c.SiteTypeName, detail.SiteTypeName)
								.SetValue(c => c.SiteInstance, detail.SiteInstance)
								.SetValue(c => c.ThreadId, detail.ThreadId)
								.SetValue(c => c.Source, detail.Source)
								.SetValue(c => c.SiteHost, detail.SiteHost)
							;
						scope.Executor.Execute(sourceDetailQuery);
					}
					var detailQuery = this._dataLayer.GetBuilder<IHealthLogDetail>()
							.Insert()
							.SetValue(c => c.HEALTH.Id, health.Id)
							.SetValue(c => c.CreatedDate, receivedTime)
							.SetValue(c => c.Message, $"Received health data from Data Bus")
							.SetValue(c => c.Note, $"From='{envelope.From}'; Token='{envelope.Token}'; Created='{envelope.Created}'")
							.SetValue(c => c.SiteTypeCode, (byte)EnvironmentElementTypeCode.MasterServer)
							.SetValue(c => c.SiteTypeName, EnvironmentElementTypeCode.MasterServer.ToString())
							.SetValue(c => c.SiteInstance, this._environment.ServerInstance)
							.SetValue(c => c.ThreadId, System.Threading.Thread.CurrentThread.ManagedThreadId)
							.SetValue(c => c.Source, "HealthDataMessageHandler")
							.SetValue(c => c.SiteHost, Environment.MachineName)
						;
					scope.Executor.Execute(detailQuery);
					
				}
			}
			catch (Exception e)
			{
				this._logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
			}
		}
	}
}
