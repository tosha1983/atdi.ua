﻿using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels;

namespace Atdi.CoreServices.EntityOrm
{
    public class QuerySelectStatement : IQuerySelectStatement, IQueryPagingStatement
	{
        private readonly SelectQueryDescriptor _queryDescriptor;

        public QuerySelectStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._queryDescriptor = new SelectQueryDescriptor(entityOrm, entityMetadata);
        }

        public SelectQueryDescriptor SelectDescriptor => this._queryDescriptor;

        public IQuerySelectStatement OnTop(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            this._queryDescriptor.SetLimit(count, LimitValueType.Records);
            return this;
        }

        public IQueryPagingStatement OrderByAsc(params string[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }
            if (columns.Length == 0)
            {
                throw new ArgumentException(nameof(columns));
            }

            foreach (var path in columns)
            {
                this._queryDescriptor.AppendSortableField(path, SortDirection.Ascending);
            }
            return this;
        }

        public IQueryPagingStatement OrderByDesc(params string[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }
            if (columns.Length == 0)
            {
                throw new ArgumentException(nameof(columns));
            }

            foreach (var path in columns)
            {
                this._queryDescriptor.AppendSortableField(path, SortDirection.Descending);
            }
            return this;
        }

        public IQuerySelectStatement Select(params string[] columns)
        {
            if (columns == null || columns.Length == 0)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            _queryDescriptor.AppendSelectableFields(columns);
            return this;
        }

        public IQuerySelectStatement Where(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            _queryDescriptor.AppendCondition(condition);
            return this;
        }

        public IQuerySelectStatement OnPercentTop(int percent)
        {
            this._queryDescriptor.SetLimit(percent, LimitValueType.Percent);
            return this;
        }

        public IQuerySelectStatement Distinct()
        {
            this._queryDescriptor.Distinct();
            return this;
        }

		public IQuerySelectStatement FetchRows(long count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			this._queryDescriptor.SetLimit(count, LimitValueType.Records);
			return this;
		}

		public IQuerySelectStatement OffsetRows(long count)
		{
			if (count < -1)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			this._queryDescriptor.SetOffsetRows(count);
			return this;
		}

		public IQuerySelectStatement Paginate(long offsetRows, long fetchRows)
		{
			if (offsetRows < -1)
			{
				throw new ArgumentOutOfRangeException(nameof(offsetRows));
			}
			this._queryDescriptor.SetOffsetRows(offsetRows);

			if (fetchRows < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(fetchRows));
			}
			this._queryDescriptor.SetLimit(fetchRows, LimitValueType.Records);

			return this;
		}
	}

    internal sealed class QuerySelectStatement<TModel> : QuerySelectStatement, IQuerySelectStatement<TModel>, IQueryPagingStatement<TModel>
	{
        public QuerySelectStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata) 
            : base(entityOrm, entityMetadata)
        {
        }

        IQuerySelectStatement<TModel> IQuerySelectStatement<TModel>.Distinct()
        {
            this.Distinct();
            return this;
        }

		IQuerySelectStatement<TModel> IQuerySelectStatement<TModel>.FetchRows(long count)
		{
			this.FetchRows(count);
			return this;
		}

		IQuerySelectStatement<TModel> IQueryPagingStatement<TModel>.OffsetRows(long count)
		{
			this.OffsetRows(count);
			return this;
		}

		IQuerySelectStatement<TModel> IQuerySelectStatement<TModel>.OnPercentTop(int percent)
        {
            this.OnPercentTop(percent);
            return this;
        }

        IQuerySelectStatement<TModel> IQuerySelectStatement<TModel>.OnTop(int count)
        {
            this.OnTop(count);
            return this;
        }

        IQueryPagingStatement<TModel> IQuerySelectStatement<TModel>.OrderByAsc(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            if (columnsExpressions == null || columnsExpressions.Length == 0)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }

            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var columnName = columnsExpressions[i].Body.GetMemberName();
                this.OrderByAsc(columnName);
            }

            return this;
        }

        IQueryPagingStatement<TModel> IQuerySelectStatement<TModel>.OrderByDesc(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            
            if (columnsExpressions == null || columnsExpressions.Length == 0)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }

            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var columnName = columnsExpressions[i].Body.GetMemberName();
                this.OrderByDesc(columnName);
            }

            return this;
        }

		IQuerySelectStatement<TModel> IQueryPagingStatement<TModel>.Paginate(long offsetRows, long fetchRows)
		{
			this.Paginate(offsetRows, fetchRows);
			return this;
		}

		IQuerySelectStatement<TModel> IQuerySelectStatement<TModel>.Select(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            if (columnsExpressions == null || columnsExpressions.Length == 0)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }

            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var columnName = columnsExpressions[i].Body.GetMemberName();
                this.Select(columnName);
            }

            return this;
        }

        IQuerySelectStatement<TModel> IQuerySelectStatement<TModel>.Where(Expression<Func<TModel, bool>> expression)
        {
            this.Where(ConditionHelper.ParseConditionExpression(expression));
            return this;
        }

        IQuerySelectStatement<TModel> IQuerySelectStatement<TModel>.Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
        {
            this.Where(ConditionHelper.ParseConditionExpression(expression));
            return this;
        }

        IQuerySelectStatement<TModel> IQuerySelectStatement<TModel>.Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
        {
            this.Where(ConditionHelper.ParseCondition(columnExpression, conditionOperator, values));
            return this;
        }
    }
}
