using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Test.WebApi.RestOrm.ORM.DTO;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	internal class CreateQuery : ICreateQuery, IWebApiRequestCreator
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

		public EntityRequest Create()
		{
			var count = _setValues.Count;

			if (count == 0)
			{
				throw new InvalidOperationException($"Not defined fields to set");
			}

			var fields = new string[count];
			var values = new object[count];

			var request = new RecordCreationRequest
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

	internal class CreateQuery<TEntity> : ICreateQuery<TEntity>, IWebApiRequestCreator
	{
		private readonly CreateQuery _createQuery;

		public CreateQuery(CreateQuery createQuery)
		{
			_createQuery = createQuery;
		}

		public WebApiQueryType QueryType => WebApiQueryType.Create;

		public EntityRequest Create()
		{
			return _createQuery.Create();
		}

		public ICreateQuery<TEntity> SetValue<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> pathExpression, TValue value)
		{
			_createQuery.SetValue(pathExpression.Body.GetMemberName(), value);
			return this;
		}
	}
}
