﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Test.WebApi.RestOrm.ORM.DTO;
using Atdi.Test.WebApi.RestOrm.ORM.Methods;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	internal class DeleteQuery : IDeleteQuery, IWebApiRequestCreator
	{
		private readonly List<string> _filters;
		private readonly string _entityNamespace;
		private readonly string _entityName;

		public DeleteQuery(string entityNamespace, string entityName)
		{
			_entityNamespace = entityNamespace;
			_entityName = entityName;
			_filters = new List<string>();
		}

		public WebApiQueryType QueryType => WebApiQueryType.Delete;

		public EntityRequest Create()
		{
			
			var request = new RecordDeleteRequest()
			{
				Namespace = _entityNamespace,
				Entity = _entityName,
				Filter = _filters.ToArray(),
			};

			return request;
		}

		public IDeleteQuery Filter(string condition)
		{
			_filters.Add(condition);
			return this;
		}
	}

	internal class DeleteQuery<TEntity> : IDeleteQuery<TEntity>, IWebApiRequestCreator, IFilterSite
	{
		private readonly DeleteQuery _updateQuery;

		public DeleteQuery(DeleteQuery updateQuery)
		{
			_updateQuery = updateQuery;
		}

		public WebApiQueryType QueryType => WebApiQueryType.Delete;

		public IQueryFilter<TEntity, IDeleteQuery<TEntity>> BeginFilter()
		{
			return new QueryFilter<TEntity, IDeleteQuery<TEntity>>(this, this);
		}

		public EntityRequest Create()
		{
			return _updateQuery.Create();
		}

		public IDeleteQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, TValue value)
		{
			_updateQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, value));
			return this;
		}

		public IDeleteQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params TValue[] values)
		{
			_updateQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, filterOperator, values));
			return this;
		}

		public IDeleteQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, System.Linq.Expressions.Expression<Func<TEntity, TValue>> rightOperandPathExpression)
		{
			_updateQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, rightOperandPathExpression));
			return this;
		}

		public IDeleteQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params System.Linq.Expressions.Expression<Func<TEntity, TValue>>[] rightOperandPathExpressions)
		{
			_updateQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, filterOperator, rightOperandPathExpressions));
			return this;
		}

		public void SetFilter(string condition)
		{
			_updateQuery.Filter(condition);
		}
	}
}