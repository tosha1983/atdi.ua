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
	internal sealed class CreateQuery : ICreateQuery, IWebApiRequestCreator
	{
		private readonly Dictionary<string, object> _setValues;
		private readonly string _entityNamespace;
		private readonly string _entityName;

		public CreateQuery(string entityNamespace, string entityName)
		{
			_entityNamespace = entityNamespace;
			_entityName = entityName;
			_setValues = new Dictionary<string, object>();
		}

		public WebApiQueryType QueryType => WebApiQueryType.Create;

		public EntityQueryRequest Create()
		{
			var count = _setValues.Count;

			if (count == 0)
			{
				throw new InvalidOperationException($"Not defined fields to set");
			}

			var fields = new string[count];
			var values = new object[count];

			var request = new CreateQueryRequest
			{
				Namespace = _entityNamespace,
				Entity = _entityName,
				Fields = fields,
				Values = values
			};

			var index = 0;
			foreach (var item in _setValues)
			{
				fields[index] = item.Key;
				values[index++] = item.Value;
			}

			return request;
		}

		public ICreateQuery SetValue<TValue>(string path, TValue value)
		{
			_setValues[path] = value;
			return this;
		}


	}

	internal sealed class CreateQuery<TEntity> : ICreateQuery<TEntity>, IWebApiRequestCreator
	{
		private readonly CreateQuery _createQuery;

		public CreateQuery(CreateQuery createQuery)
		{
			_createQuery = createQuery;
		}

		public WebApiQueryType QueryType => WebApiQueryType.Create;

		public EntityQueryRequest Create()
		{
			return _createQuery.Create();
		}

		public ICreateQuery<TEntity> SetValue<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> pathExpression, TValue value)
		{
			_createQuery.SetValue(pathExpression.Body.GetMemberName(), value);
			return this;
		}

		public ICreateQuery<TEntity> SetValueAsJson<TValue>(Expression<Func<TEntity, string>> pathExpression, TValue value)
		{
			var json = JsonConvert.SerializeObject(value);
			_createQuery.SetValue(pathExpression.Body.GetMemberName(), json);
			return this;
		}
	}
}
