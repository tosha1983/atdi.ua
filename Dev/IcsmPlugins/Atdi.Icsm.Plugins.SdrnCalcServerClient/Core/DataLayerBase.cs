using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.Core
{
	public class DataLayerBase
	{
		private readonly WebApiDataLayer _dataLayer;
		private readonly IQueryExecutor _executor;
		private readonly IMetadataSite _metadataSite;

		public DataLayerBase(WebApiEndpoint endpoint, WebApiDataContext dataContext)
		{
			this._dataLayer = new WebApiDataLayer(
				endpoint, dataContext);
			this._executor = this._dataLayer.Executor;
			this._metadataSite = this._dataLayer.MetadataSite;
		}

		public IMetadataSite MetadataSite => _metadataSite;

		public IQueryExecutor Executor => _executor;

		public IQueryBuilder<TEntity> GetBuilder<TEntity>()
		{
			return _dataLayer.GetBuilder<TEntity>();
		}

		public IQueryBuilder GetBuilder(string entityName)
		{
			return _dataLayer.GetBuilder(entityName);
		}

		public WebApiDataLayer Origin => _dataLayer;
	}
}
