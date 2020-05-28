using Atdi.Contracts.Sdrn.Infocenter;
using Atdi.DataModels.Sdrn.Infocenter.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common.Extensions;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.Infocenter
{
	internal class IntegrationService : IIntegrationService
	{
		private readonly AppServerComponentConfig _config;
		private readonly IDataLayer<EntityDataOrm> _dataLayer;
		private readonly ILogger _logger;
		private readonly IQueryBuilder<IIntegrationObject> _integrationObjectQueryBuilder;
		private readonly IQueryBuilder<IIntegrationLog> _integrationLogQueryBuilder;
		public IntegrationService(AppServerComponentConfig config, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
		{
			_config = config;
			_dataLayer = dataLayer;
			_logger = logger;
			_integrationObjectQueryBuilder = _dataLayer.GetBuilder<IIntegrationObject>();
			_integrationLogQueryBuilder = _dataLayer.GetBuilder<IIntegrationLog>();
		}

		public void Finish(long token, IntegrationStatusCode status, string statusNote, string total)
		{
			using (var dbScope = this._dataLayer.CreateScope<InfocenterDataContext>())
			{
				var updQuery = _integrationLogQueryBuilder
					.Update()
					.SetValue(c => c.FinishTime, DateTimeOffset.Now)
					.SetValue(c => c.StatusCode, (byte) status)
					.SetValue(c => c.StatusName, status.ToString())
					.SetValue(c => c.StatusNote, statusNote)
					.SetValue(c => c.SyncTotal, total)
					.Where(c => c.Id, ConditionOperator.Equal, token);

				dbScope.Executor.Execute(updQuery);
			}
		}

		public void Finish<TSyncKey>(long token, IntegrationStatusCode status, string statusNote, string total, TSyncKey syncKey)
		{
			using (var dbScope = this._dataLayer.CreateScope<InfocenterDataContext>())
			{
				var updQuery = _integrationLogQueryBuilder
					.Update()
					.SetValue(c => c.FinishTime, DateTimeOffset.Now)
					.SetValue(c => c.StatusCode, (byte)status)
					.SetValue(c => c.StatusName, status.ToString())
					.SetValue(c => c.StatusNote, statusNote)
					.SetValue(c => c.SyncTotal, total)
					.Where(c => c.Id, ConditionOperator.Equal, token);

				dbScope.Executor.Execute(updQuery);

				var findQuery = _integrationLogQueryBuilder
					.From()
					.Select(c => c.OBJECT.Id)
					.Where(c => c.Id, ConditionOperator.Equal, token);
				var objectId = dbScope.Executor.ExecuteAndFetch(findQuery, reader =>
				{
					if (reader.Read())
					{
						return reader.GetValue(c => c.OBJECT.Id);
					}

					return 0;
				});

				if (objectId == 0)
				{
					throw new InvalidOperationException($"Incorrect log token #{token}");
				}

				var updObjectQuery = _integrationObjectQueryBuilder
					.Update()
					.SetValue(c => c.SyncKeyType, typeof(TSyncKey).ToString())
					.SetValue(c => c.SyncKeyNote, Convert.ToString(syncKey))
					.SetValue(c => c.SyncKeyContent, syncKey?.Serialize())
					.Where(c => c.Id, ConditionOperator.Equal, objectId);
				dbScope.Executor.Execute(updObjectQuery);
			}
		}

		public long Start(string source, string objectName)
		{
			using (var dbScope = this._dataLayer.CreateScope<InfocenterDataContext>())
			{
				var objectId = this.GetObjectId(dbScope, source, objectName);
				var insQuery = _integrationLogQueryBuilder
					.Insert()
					.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
					.SetValue(c => c.StartTime, DateTimeOffset.Now)
					.SetValue(c => c.StatusCode, (byte) IntegrationStatusCode.Processing)
					.SetValue(c => c.StatusName, "Processing")
					.SetValue(c => c.OBJECT.Id, objectId);

				var pk = dbScope.Executor.Execute<IIntegrationLog_PK>(insQuery);

				return pk.Id;
			}
		}

		private long GetObjectId(IDataLayerScope dbScope, string dataSource, string objectName)
		{
			var findQuery = _integrationObjectQueryBuilder
				.From()
				.Select(c => c.Id)
				.Where(c => c.DataSource, ConditionOperator.Equal, dataSource)
				.Where(c => c.ObjectName, ConditionOperator.Equal, objectName);

			var id = dbScope.Executor.ExecuteAndFetch(findQuery, reader =>
			{
				if (reader.Read())
				{
					return reader.GetValue(c => c.Id);
				}

				return 0;
			});

			if (id == 0)
			{
				var insQuery = _integrationObjectQueryBuilder
					.Insert()
					.SetValue(c => c.DataSource, dataSource)
					.SetValue(c => c.ObjectName, objectName)
					.SetValue(c => c.CreatedDate, DateTimeOffset.Now);
				var pk = dbScope.Executor.Execute<IIntegrationObject_PK>(insQuery);
				id = pk.Id;
			}

			return id;
		}
	}
}
