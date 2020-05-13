using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Atdi.Test.WebApi.RestOrm.ORM.DTO;
using Atdi.Test.WebApi.RestOrm.ORM.Methods;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	internal class ReadQuery : IReadQuery, IWebApiRequestCreator
	{
		private readonly Dictionary<string, string> _select;
		private readonly List<string> _filters;
		private readonly List<string> _orderBy;
		private int _top;

		private readonly string _entityNamespace;
		private readonly string _entityName;

		public ReadQuery(string entityNamespace, string entityName)
		{
			_entityNamespace = entityNamespace;
			_entityName = entityName;
			_select = new Dictionary<string, string>();
			_filters = new List<string>();
			_orderBy = new List<string>();
		}

		public WebApiQueryType QueryType => WebApiQueryType.Read;

		public EntityRequest Create()
		{
			var count = _select.Count;

			if (count == 0)
			{
				throw new InvalidOperationException($"Not defined fields to select");
			}

			var select = new string[count];

			var request = new RecordReadRequest
			{
				Namespace = _entityNamespace,
				Entity = _entityName,
				Select = select,
				OrderBy = _orderBy.ToArray(),
				Filter = _filters.ToArray(),
				Top = _top
			};

			var index = 0;
			foreach (var path in _select.Values)
			{
				select[index++] = path;
			}

			return request;
		}

		public IReadQuery Filter(string condition)
		{
			_filters.Add(condition);
			return this;
		}

		public IReadQuery OnTop(int count)
		{
			_top = count;
			return this;
		}

		public IReadQuery OrderByAsc(string path)
		{
			_orderBy.Add($"{path} asc");
			return this;
		}

		public IReadQuery OrderByDesc(string path)
		{
			_orderBy.Add($"{path} desc");
			return this;
		}

		public IReadQuery Select(string path)
		{
			_select[path] = path;
			return this;
		}
	}

	internal class ReadQuery<TEntity> : IReadQuery<TEntity>, IWebApiRequestCreator, IFilterSite
	{
		private readonly ReadQuery _readQuery;

		public ReadQuery(ReadQuery readQuery)
		{
			_readQuery = readQuery;
		}

		public WebApiQueryType QueryType => WebApiQueryType.Read;

		public IQueryFilter<TEntity, IReadQuery<TEntity>> BeginFilter()
		{
			return new QueryFilter<TEntity, IReadQuery<TEntity>>(this, this);
		}

		public EntityRequest Create()
		{
			return _readQuery.Create();
		}

		public IReadQuery<TEntity> Filter<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, TValue value)
		{
			_readQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, value));
			return this;
		}

		public IReadQuery<TEntity> Filter<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params TValue[] values)
		{
			_readQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, filterOperator, values));
			return this;
		}

		public IReadQuery<TEntity> Filter<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, Expression<Func<TEntity, TValue>> rightOperandPathExpression)
		{
			_readQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, rightOperandPathExpression));
			return this;
		}

		public IReadQuery<TEntity> Filter<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params Expression<Func<TEntity, TValue>>[] rightOperandPathExpressions)
		{
			_readQuery.Filter(ConditionBuilder.Build(leftOperandPathExpression, filterOperator, rightOperandPathExpressions));
			return this;
		}

		public IReadQuery<TEntity> OnTop(int count)
		{
			_readQuery.OnTop(count);
			return this;
		}

		public IReadQuery<TEntity> OrderByAsc(params Expression<Func<TEntity, object>>[] pathExpressions)
		{
			for (var i = 0; i < pathExpressions.Length; i++)
			{
				_readQuery.OrderByAsc(pathExpressions[i].Body.GetMemberName());
			}
			return this;
		}

		public IReadQuery<TEntity> OrderByDesc(params Expression<Func<TEntity, object>>[] pathExpressions)
		{
			for(var i = 0; i < pathExpressions.Length; i++)
			{
				_readQuery.OrderByDesc(pathExpressions[i].Body.GetMemberName());
			}
			return this;
		}

		public IReadQuery<TEntity> Select(params Expression<Func<TEntity, object>>[] pathExpressions)
		{
			for (var i = 0; i < pathExpressions.Length; i++)
			{
				_readQuery.Select(pathExpressions[i].Body.GetMemberName());
			}
			return this;
		}

		public void SetFilter(string condition)
		{
			_readQuery.Filter(condition);
		}
	}
}
