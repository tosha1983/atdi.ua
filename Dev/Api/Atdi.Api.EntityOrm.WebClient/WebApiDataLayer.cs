using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Atdi.Api.EntityOrm.WebClient
{
	public class WebApiDataLayer
	{
		private readonly ProxyInstanceFactory _proxyInstanceFactory;
		private readonly Dictionary<WebApiQueryType, IWebApiMethodHandler> _methodHandlers;
		private readonly ConcurrentDictionary<string, IQueryBuilder> _queryBuilders;
		private readonly ConcurrentDictionary<Type, object> _typedQueryBuilders;
		private readonly IQueryExecutor _executor;

		public WebApiDataLayer()
		{
			_queryBuilders = new ConcurrentDictionary<string, IQueryBuilder>();
			_typedQueryBuilders = new ConcurrentDictionary<Type, object>();
			_proxyInstanceFactory = new ProxyInstanceFactory();
			_methodHandlers = new Dictionary<WebApiQueryType, IWebApiMethodHandler>
			{
				[WebApiQueryType.Create] = new CreateMethodHandler(_proxyInstanceFactory),
				[WebApiQueryType.Read] = new ReadMethodHandler(_proxyInstanceFactory),
				[WebApiQueryType.Update] = new UpdateMethodHandler(_proxyInstanceFactory),
				[WebApiQueryType.Delete] = new DeleteMethodHandler(_proxyInstanceFactory),
				[WebApiQueryType.Apply] = new ApplyMethodHandler(_proxyInstanceFactory)
			};
		}

		internal Dictionary<WebApiQueryType, IWebApiMethodHandler> MethodHandlers => _methodHandlers;

		public ProxyInstanceFactory ProxyInstanceFactory => _proxyInstanceFactory;

		public WebApiDataLayer(WebApiEndpoint endpoint, WebApiDataContext dataContext)
			: this()
		{
			_executor = new WebApiQueryExecutor(this, endpoint, dataContext);
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
		
	}
}