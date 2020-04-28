using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EntityOrm.WebClient.DTO;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal class ApplyQuery : IApplyQuery, IWebApiRequestCreator
	{
		private readonly CreateQuery _createQuery;
		private readonly UpdateQuery _updateQuery;

		private readonly string _entityNamespace;
		private readonly string _entityName;

		private bool _useCreateQuery;
		private bool _useUpdateQuery;

		public ApplyQuery(string entityNamespace, string entityName)
		{
			_entityNamespace = entityNamespace;
			_entityName = entityName;
			_createQuery = new CreateQuery(entityNamespace, entityName);
			_updateQuery = new UpdateQuery(entityNamespace, entityName);
			_useCreateQuery = true;
			_useUpdateQuery = true;
		}

		public CreateQuery CreateQuery => _createQuery;

		public UpdateQuery UpdateQuery => _updateQuery;

		public WebApiQueryType QueryType => WebApiQueryType.Apply;

		public EntityRequest Create()
		{
			var createRequest = _createQuery.Create() as RecordCreateRequest;
			var updateRequest = _updateQuery.Create() as RecordUpdateRequest;

			var request = new RecordApplyRequest
			{
				Namespace = _entityNamespace,
				Entity = _entityName,
				FieldsToCreate = createRequest.Fields,
				FieldsToUpdate = updateRequest.Fields,
				ValuesToCreate = createRequest.Values,
				ValuesToUpdate = updateRequest.Values,
				Filter = updateRequest.Filter
			};

			return request;
		}

		public IApplyQuery CreateIfNotExists()
		{
			_useCreateQuery = true;
			_useUpdateQuery = false;
			return this;
		}

		public IApplyQuery Filter(string condition)
		{
			_updateQuery.Filter(condition);
			return this;
		}

		
		public IApplyQuery SetValue<TValue>(string path, TValue value)
		{
			if (_useCreateQuery)
			{
				_createQuery.SetValue(path, value);
			}
			if (_useUpdateQuery)
			{
				_updateQuery.SetValue(path, value);
			}
			return this;
		}

		public IApplyQuery UpdateIfExists()
		{
			_useCreateQuery = false;
			_useUpdateQuery = true;
			return this;
		}
	}

	internal class ApplyQuery<TEntity> : IApplyQuery<TEntity>, IWebApiRequestCreator, IFilterSite
	{
		private readonly ApplyQuery _applyQuery;

		public ApplyQuery(ApplyQuery applyQuery)
		{
			_applyQuery = applyQuery;
		}

		public WebApiQueryType QueryType => WebApiQueryType.Apply;

		public IQueryFilter<TEntity, IApplyQuery<TEntity>> BeginFilter()
		{
			return new QueryFilter<TEntity, IApplyQuery<TEntity>>(this, this);
		}

		public EntityRequest Create()
		{
			return _applyQuery.Create();
		}

		public IApplyQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, TValue value)
		{
			_applyQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, value));
			return this;
		}

		public IApplyQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params TValue[] values)
		{
			_applyQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, filterOperator, values));
			return this;
		}

		public IApplyQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, System.Linq.Expressions.Expression<Func<TEntity, TValue>> rightOperandPathExpression)
		{
			_applyQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, rightOperandPathExpression));
			return this;
		}

		public IApplyQuery<TEntity> Filter<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params System.Linq.Expressions.Expression<Func<TEntity, TValue>>[] rightOperandPathExpressions)
		{
			_applyQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, filterOperator, rightOperandPathExpressions));
			return this;
		}

		public IApplyQuery<TEntity> SetValue<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> pathExpression, TValue value)
		{
			_applyQuery.SetValue(pathExpression.Body.GetMemberName(), value);
			return this;
		}

		public void SetFilter(string condition)
		{
			_applyQuery.Filter(condition);
		}

		public IApplyQuery<TEntity> UpdateIfExists()
		{
			_applyQuery.UpdateIfExists();
			return this;
		}

		public IApplyQuery<TEntity> CreateIfNotExists()
		{
			_applyQuery.CreateIfNotExists();
			return this;
		}
	}
}
