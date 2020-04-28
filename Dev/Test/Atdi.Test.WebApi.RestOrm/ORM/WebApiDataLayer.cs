using System;
using System.Collections.Concurrent;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public class WebApiDataLayer
	{
		private readonly ConcurrentDictionary<string, IQueryBuilder> _queryBuilders;
		private readonly ConcurrentDictionary<Type, object> _typedQueryBuilders;

		public WebApiDataLayer()
		{
			_queryBuilders = new ConcurrentDictionary<string, IQueryBuilder>();
			_typedQueryBuilders = new ConcurrentDictionary<Type, object>();
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
			return new WebApiQueryExecutor(endpoint, dataContext);
		}
	}
}