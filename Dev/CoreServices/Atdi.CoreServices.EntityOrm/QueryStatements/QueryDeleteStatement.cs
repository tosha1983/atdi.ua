using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryDeleteStatement : IQueryDeleteStatement
    {
        private readonly DeleteQueryDescriptor _queryDescriptor;

        public QueryDeleteStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._queryDescriptor = new DeleteQueryDescriptor(entityOrm, entityMetadata);
        }

        public DeleteQueryDescriptor QueryDecriptor => this._queryDescriptor;

        public IQueryDeleteStatement Where(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            _queryDescriptor.AppendCondition(condition);
            return this;
        }
    }

    internal sealed class QueryDeleteStatement<TModel> : IQueryDeleteStatement<TModel>
    {
        private readonly QueryDeleteStatement _statement;

        public QueryDeleteStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._statement = new QueryDeleteStatement(entityOrm, entityMetadata);
        }

        public QueryDeleteStatement Statement => _statement;

        public IQueryDeleteStatement<TModel> Where(Expression<Func<TModel, bool>> expression)
        {
            this._statement.Where(ConditionHelper.ParseConditionExpression(expression));
            return this;
        }

        public IQueryDeleteStatement<TModel> Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
        {
            this._statement.Where(ConditionHelper.ParseConditionExpression(expression));
            return this;
        }

        public IQueryDeleteStatement<TModel> Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
        {
            this._statement.Where(ConditionHelper.ParseCondition(columnExpression, conditionOperator, values));
            return this;
        }
    }
    
}
