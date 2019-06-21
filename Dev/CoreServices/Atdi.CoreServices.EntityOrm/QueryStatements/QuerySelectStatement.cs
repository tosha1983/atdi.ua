using Atdi.Contracts.CoreServices.DataLayer;
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
    public sealed class QuerySelectStatement : IQuerySelectStatement
    {
        private readonly SelectQueryDescriptor _queryDescriptor;

        public QuerySelectStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._queryDescriptor = new SelectQueryDescriptor(entityOrm, entityMetadata);
        }

        public SelectQueryDescriptor QueryDecriptor => this._queryDescriptor;

        public IQuerySelectStatement OnTop(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            this._queryDescriptor.SetLimit(count, LimitValueType.Records);
            return this;
        }

        public IQuerySelectStatement OrderByAsc(params string[] columns)
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

        public IQuerySelectStatement OrderByDesc(params string[] columns)
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
    }

    internal sealed class QuerySelectStatement<TModel> : IQuerySelectStatement<TModel>
    {
        private readonly QuerySelectStatement _statement;

        public QuerySelectStatement Statement => _statement;

        public QuerySelectStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._statement = new QuerySelectStatement(entityOrm, entityMetadata); 
        }

        public IQuerySelectStatement<TModel> Distinct()
        {
            this._statement.Distinct();
            return this;
        }

        public IQuerySelectStatement<TModel> OnPercentTop(int percent)
        {
            this._statement.OnPercentTop(percent);
            return this;
        }

        public IQuerySelectStatement<TModel> OnTop(int count)
        {
            this._statement.OnTop(count);
            return this;
        }

        public IQuerySelectStatement<TModel> OrderByAsc(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            if (columnsExpressions == null || columnsExpressions.Length == 0)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }

            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var columnName = columnsExpressions[i].Body.GetMemberName();
                _statement.OrderByAsc(columnName);
            }

            return this;
        }

        public IQuerySelectStatement<TModel> OrderByDesc(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            if (columnsExpressions == null || columnsExpressions.Length == 0)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }

            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var columnName = columnsExpressions[i].Body.GetMemberName();
                _statement.OrderByDesc(columnName);
            }

            return this;
        }

        public IQuerySelectStatement<TModel> Select(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            if (columnsExpressions == null || columnsExpressions.Length == 0)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }

            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var columnName = columnsExpressions[i].Body.GetMemberName();
                _statement.Select(columnName);
            }

            return this;
        }

        public IQuerySelectStatement<TModel> Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
        {
            this._statement.Where(ConditionHelper.ParseCondition(columnExpression, conditionOperator, values));
            return this;
        }

        public IQuerySelectStatement<TModel> Where(Expression<Func<TModel, bool>> expression)
        {
            this._statement.Where(ConditionHelper.ParseConditionExpression(expression));
            return this;
        }

        public IQuerySelectStatement<TModel> Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
        {
            this._statement.Where(ConditionHelper.ParseConditionExpression(expression));
            return this;
        }
 
    }
}
