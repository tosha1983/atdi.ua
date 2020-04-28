using Atdi.Contracts.Api.EntityOrm.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal class WebApiQueryBuilder : IQueryBuilder
	{
		private readonly string _entityName;
		private readonly string _entityNamespace;

		public WebApiQueryBuilder(string entityFullName)
		{
			if (string.IsNullOrEmpty(entityFullName))
			{
				throw new ArgumentNullException(nameof(entityFullName));
			}

			var nameParts = entityFullName.Split('.');
			if (nameParts.Length == 1)
			{
				throw new InvalidOperationException($"Incorrect entity full name '{entityFullName}'. Undefined namespace or entity name");
			}

			_entityName = nameParts[nameParts.Length - 1];
			_entityNamespace = entityFullName.Substring(0, entityFullName.Length - 1 - _entityName.Length);

			if (string.IsNullOrEmpty(_entityName))
			{
				throw new InvalidOperationException($"Incorrect entity full name '{entityFullName}'. Undefined entity name");
			}
			if (string.IsNullOrEmpty(_entityNamespace))
			{
				throw new InvalidOperationException($"Incorrect entity full name '{entityFullName}'. Undefined entity namespace");
			}
		}

		public string EntityName => _entityName;

		public string EntityNamespace => _entityNamespace;

		public ICreateQuery Create()
		{
			return new CreateQuery(_entityNamespace, _entityName);
		}

		public IDeleteQuery Delete()
		{
			return new DeleteQuery(_entityNamespace, _entityName);
		}

		public IApplyQuery Apply()
		{
			return new ApplyQuery(_entityNamespace, _entityName);
		}

		public IReadQuery Read()
		{
			return new ReadQuery(_entityNamespace, _entityName);
		}

		public IUpdateQuery Update()
		{
			return new UpdateQuery(_entityNamespace, _entityName);
		}
	}

	internal class WebApiQueryBuilder<TEntity> : IQueryBuilder<TEntity>
	{
		private readonly string _entityName;
		private readonly string _entityNamespace;

		public WebApiQueryBuilder()
		{
			var entityType = typeof(TEntity);
			var entityTypeName = entityType.Name;

			_entityName = (entityTypeName[0] == 'I' ? entityTypeName.Substring(1, entityTypeName.Length - 1) : entityTypeName);
			_entityNamespace = entityType.Namespace;
		}

		public string EntityName => _entityName;

		public string EntityNamespace => _entityNamespace;

		public IApplyQuery<TEntity> Apply()
		{
			return new ApplyQuery<TEntity>(new ApplyQuery(_entityNamespace, _entityName));
		}

		public ICreateQuery<TEntity> Create()
		{
			return new CreateQuery<TEntity>(new CreateQuery(_entityNamespace, _entityName));
		}

		public IDeleteQuery<TEntity> Delete()
		{
			return new DeleteQuery<TEntity>(new DeleteQuery(_entityNamespace, _entityName));
		}

		public IReadQuery<TEntity> Read()
		{
			return new ReadQuery<TEntity>(new ReadQuery(_entityNamespace, _entityName));
		}

		public IUpdateQuery<TEntity> Update()
		{
			return new UpdateQuery<TEntity>(new UpdateQuery(_entityNamespace, _entityName));
		}
	}
}
