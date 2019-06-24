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
    internal class QueryDeleteStatement : IQueryDeleteStatement
    {
        private readonly DeleteQueryDescriptor _queryDescriptor;

        public QueryDeleteStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._queryDescriptor = new DeleteQueryDescriptor(entityOrm, entityMetadata);
        }

        public DeleteQueryDescriptor DeleteDecriptor => this._queryDescriptor;

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

    internal sealed class QueryDeleteStatement<TModel> : QueryDeleteStatement, IQueryDeleteStatement<TModel>
    {
        public QueryDeleteStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
            : base(entityOrm, entityMetadata)
        {
        }

        IQueryDeleteStatement<TModel> IQueryDeleteStatement<TModel>.Where(Expression<Func<TModel, bool>> expression)
        {
            this.Where(ConditionHelper.ParseConditionExpression(expression));
            return this;
        }

        IQueryDeleteStatement<TModel> IQueryDeleteStatement<TModel>.Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
        {
            this.Where(ConditionHelper.ParseConditionExpression(expression));
            return this;
        }

        IQueryDeleteStatement<TModel> IQueryDeleteStatement<TModel>.Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
        {
            this.Where(ConditionHelper.ParseCondition(columnExpression, conditionOperator, values));
            return this;
        }
    }
    
}
