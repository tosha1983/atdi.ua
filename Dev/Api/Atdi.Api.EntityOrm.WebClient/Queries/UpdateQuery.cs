﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EntityOrm.WebClient.DTO;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Newtonsoft.Json;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal sealed class UpdateQuery : IUpdateQuery, IWebApiRequestCreator
	{
		private readonly List<string> _filters;
		private readonly Dictionary<string, object> _setValues;
		private readonly string _entityNamespace;
		private readonly string _entityName;

		public UpdateQuery(string entityNamespace, string entityName)
		{
			_entityNamespace = entityNamespace;
			_entityName = entityName;
			_setValues = new Dictionary<string, object>();
			_filters = new List<string>();
		}

		public WebApiQueryType QueryType => WebApiQueryType.Update;

		public EntityQueryRequest Create()
		{
			var count = _setValues.Count;

			if (count == 0)
			{
				throw new InvalidOperationException($"Not defined fields to set");
			}

			var fields = new string[count];
			var values = new object[count];

			var request = new UpdateQueryRequest()
			{
				Namespace = _entityNamespace,
				Entity = _entityName,
				Fields = fields,
				Values = values,
				Filter = _filters.ToArray(),
			};

			var index = 0;
			foreach (var item in _setValues)
			{
				fields[index] = item.Key;
				values[index++] = item.Value;
			}

			return request;
		}

		public IUpdateQuery Filter(string condition)
		{
			_filters.Add(condition);
			return this;
		}

		public IUpdateQuery SetValue<TValue>(string path, TValue value)
		{
			_setValues[path] = value;
			return this;
		}


	}

	internal sealed class UpdateQuery<TEntity> : IUpdateQuery<TEntity>, IWebApiRequestCreator, IFilterSite
	{
		private readonly UpdateQuery _updateQuery;

		public UpdateQuery(UpdateQuery updateQuery)
		{
			_updateQuery = updateQuery;
		}

		public WebApiQueryType QueryType => WebApiQueryType.Update;

		public IQueryFilter<TEntity, IUpdateQuery<TEntity>> BeginFilter()
		{
			return new QueryFilter<TEntity, IUpdateQuery<TEntity>>(this, this);
		}

		public EntityQueryRequest Create()
		{
			return _updateQuery.Create();
		}

		public IUpdateQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, TValue value)
		{
			_updateQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, value));
			return this;
		}

		public IUpdateQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params TValue[] values)
		{
			_updateQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, filterOperator, values));
			return this;
		}

		public IUpdateQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, System.Linq.Expressions.Expression<Func<TEntity, TValue>> rightOperandPathExpression)
		{
			_updateQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, rightOperandPathExpression));
			return this;
		}

		public IUpdateQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params System.Linq.Expressions.Expression<Func<TEntity, TValue>>[] rightOperandPathExpressions)
		{
			_updateQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, filterOperator, rightOperandPathExpressions));
			return this;
		}

		public IUpdateQuery<TEntity> SetValue<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> pathExpression, TValue value)
		{
			_updateQuery.SetValue(pathExpression.Body.GetMemberName(), value);
			return this;
		}

		public IUpdateQuery<TEntity> SetValueAsJson<TValue>(Expression<Func<TEntity, string>> pathExpression, TValue value)
		{
			var json = JsonConvert.SerializeObject(value);
			_updateQuery.SetValue(pathExpression.Body.GetMemberName(), json);
			return this;
		}

		public void SetFilter(string condition)
		{
			_updateQuery.Filter(condition);
		}
	}
}
