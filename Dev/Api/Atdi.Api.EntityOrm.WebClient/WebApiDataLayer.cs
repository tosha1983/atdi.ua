using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Atdi.Api.EntityOrm.WebClient
{
	public sealed class WebApiDataLayer
	{
		private readonly ProxyInstanceFactory _proxyInstanceFactory;
		private readonly Dictionary<WebApiQueryType, IWebApiQueryHandler> _queryHandlers;
		private readonly ConcurrentDictionary<string, IQueryBuilder> _queryBuilders;
		private readonly ConcurrentDictionary<Type, object> _typedQueryBuilders;
		private readonly IQueryExecutor _executor;
		private readonly IMetadataSite _metadataSite;

		public WebApiDataLayer()
		{
			_queryBuilders = new ConcurrentDictionary<string, IQueryBuilder>();
			_typedQueryBuilders = new ConcurrentDictionary<Type, object>();
			_proxyInstanceFactory = new ProxyInstanceFactory();
			_queryHandlers = new Dictionary<WebApiQueryType, IWebApiQueryHandler>
			{
				[WebApiQueryType.Create] = new CreateQueryHandler(_proxyInstanceFactory),
				[WebApiQueryType.Read] = new ReadQueryHandler(_proxyInstanceFactory),
				[WebApiQueryType.Update] = new UpdateQueryHandler(_proxyInstanceFactory),
				[WebApiQueryType.Delete] = new DeleteQueryHandler(_proxyInstanceFactory),
				[WebApiQueryType.Apply] = new ApplyQueryHandler(_proxyInstanceFactory)
			};
		}

		internal Dictionary<WebApiQueryType, IWebApiQueryHandler> QueryHandlers => _queryHandlers;

		public ProxyInstanceFactory ProxyInstanceFactory => _proxyInstanceFactory;

		public WebApiDataLayer(WebApiEndpoint endpoint, WebApiDataContext dataContext)
			: this()
		{
			_executor = new WebApiQueryExecutor(this, endpoint, dataContext);
			_metadataSite = new WebApiMetadataSite(this, endpoint);
		}

		public IQueryBuilder<TEntity> GetBuilder<TEntity>()
		{
			var entityType = typeof(TEntity);
			if (_typedQueryBuilders.TryGetValue(entityType, out var result))
			{
				return (IQueryBuilder<TEntity>)result;
			}

			result = new WebApiQueryBuilder<TEntity>();

			if (_typedQueryBuilders.TryAdd(entityType, result))
			{
				return (IQueryBuilder<TEntity>)result;
			}
			if (_typedQueryBuilders.TryGetValue(entityType, out result))
			{
				return (IQueryBuilder<TEntity>)result;
			}
			throw new InvalidOperationException("Failed to add object of QueryBuilder to concurrent collection.");
		}


		public IQueryBuilder GetBuilder(string entityName)
		{
			if (_queryBuilders.TryGetValue(entityName, out var result))
			{
				return result;
			}
			result = new WebApiQueryBuilder(entityName);
			if (_queryBuilders.TryAdd(entityName, result))
			{
				return result;
			}
			if (_queryBuilders.TryGetValue(entityName, out result))
			{
				return result;
			}
			throw new InvalidOperationException("Failed to add object of QueryBuilder to concurrent collection.");
		}

		public IQueryExecutor GetExecutor(WebApiEndpoint endpoint, WebApiDataContext dataContext)
		{
			return new WebApiQueryExecutor(this, endpoint, dataContext);
		}

		public IQueryExecutor Executor => _executor;


		public IMetadataSite MetadataSite => _metadataSite;

		public IMetadataSite GetMetadataSite(WebApiEndpoint endpoint)
		{
			return new WebApiMetadataSite(this, endpoint);
		}


	}
}