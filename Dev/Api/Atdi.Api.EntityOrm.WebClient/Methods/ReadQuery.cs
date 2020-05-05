using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.Api.EntityOrm.WebClient.DTO;
using Atdi.DataModels.Api.EntityOrm.WebClient;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal class ReadQuery : IReadQuery, IWebApiRequestCreator, IPagingReadQuery
	{
		private readonly Dictionary<string, string> _select;
		private readonly List<string> _filters;
		private readonly List<string> _orderBy;
		private long _fetchRows;
		private long _offsetRows;
		private bool _distinct;

		private readonly string _entityNamespace;
		private readonly string _entityName;

		public ReadQuery(string entityNamespace, string entityName)
		{
			_entityNamespace = entityNamespace;
			_entityName = entityName;
			_select = new Dictionary<string, string>();
			_filters = new List<string>();
			_orderBy = new List<string>();
			_offsetRows = -1;
			_fetchRows = -1;
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
				Distinct = _distinct
			};
			if (_fetchRows >= 0)
			{
				request.Fetch = _fetchRows;
			}
			if (_offsetRows >= 0)
			{
				if (_fetchRows == 0)
				{
					throw new InvalidOperationException($"Invalid fetch rows of zero' for the paging section");
				}
				if (request.OrderBy == null || request.OrderBy.Length == 0)
				{
					throw new InvalidOperationException($"Not defined fields to order by with defined offset");
				}
				request.Offset = _offsetRows;
			}

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
			_fetchRows = count;
			return this;
		}

		public IPagingReadQuery OrderByAsc(string path)
		{
			_orderBy.Add($"{path} asc");
			return this;
		}

		public IPagingReadQuery OrderByDesc(string path)
		{
			_orderBy.Add($"{path} desc");
			return this;
		}

		public IReadQuery Select(string path)
		{
			_select[path] = path;
			return this;
		}

		public IReadQuery FetchRows(long count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			_fetchRows = count;
			return this;
		}

		public IReadQuery Distinct()
		{
			_distinct = true;
			return this;
		}

		public IReadQuery OffsetRows(long count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			_offsetRows = count;
			return this;
		}

		public IReadQuery Paginate(long offsetRows, long fetchRows)
		{
			if (offsetRows < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(offsetRows));
			}
			if (fetchRows <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(fetchRows));
			}

			_offsetRows = offsetRows;
			_fetchRows = fetchRows;
			return this;
		}
	}

	internal class ReadQuery<TEntity> : IReadQuery<TEntity>, IWebApiRequestCreator, IFilterSite, IPagingReadQuery<TEntity>
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

		public IReadQuery<TEntity> Distinct()
		{
			_readQuery.Distinct();
			return this;
		}

		public IReadQuery<TEntity> FetchRows(long count)
		{
			_readQuery.FetchRows(count);
			return this;
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

		public IReadQuery<TEntity> OffsetRows(long count)
		{
			_readQuery.OffsetRows(count);
			return this;
		}

		public IReadQuery<TEntity> OnTop(int count)
		{
			_readQuery.OnTop(count);
			return this;
		}

		public IPagingReadQuery<TEntity> OrderByAsc(params Expression<Func<TEntity, object>>[] pathExpressions)
		{
			for (var i = 0; i < pathExpressions.Length; i++)
			{
				_readQuery.OrderByAsc(pathExpressions[i].Body.GetMemberName());
			}
			return this;
		}

		public IPagingReadQuery<TEntity> OrderByDesc(params Expression<Func<TEntity, object>>[] pathExpressions)
		{
			for(var i = 0; i < pathExpressions.Length; i++)
			{
				_readQuery.OrderByDesc(pathExpressions[i].Body.GetMemberName());
			}
			return this;
		}

		public IReadQuery<TEntity> Paginate(long offsetRows, long fetchRows)
		{
			_readQuery.Paginate(offsetRows, fetchRows);
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
