using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryDeleteStatement<TModel> : IQueryDeleteStatement<TModel>
    {
        private readonly Type ModelType = typeof(TModel);
        private readonly List<Condition> _conditions;
        private readonly string _tableName;

        public QueryDeleteStatement()
        {
            this._tableName = ModelType.Name;
            this._conditions = new List<Condition>();
        }

        public QueryDeleteStatement(string tableName)
        {
            this._tableName = tableName;
            this._conditions = new List<Condition>();
        }
        public List<Condition> Conditions => this._conditions;
        public string TableName => this._tableName;

        public IQueryDeleteStatement<TModel> Where(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            this._conditions.Add(condition);
            return this;
        }

        public IQueryDeleteStatement<TModel> Where(Expression<Func<TModel, bool>> expression)
        {
            this.Where(Atdi.CoreServices.EntityOrm.QuerySelectStatement<TModel>.ParseConditionExpression(expression));
            return this;
        }

        public IQueryDeleteStatement<TModel> Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
        {
            this.Where(Atdi.CoreServices.EntityOrm.QuerySelectStatement<TModel>.ParseConditionExpression(expression));
            return this;
        }

        public IQueryDeleteStatement<TModel> Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
        {
            this.Where(Atdi.CoreServices.EntityOrm.QuerySelectStatement<TModel>.ParseCondition(columnExpression, conditionOperator, values));
            return this;
        }
    }
    internal sealed class QueryDeleteStatement : IQueryDeleteStatement
    {
        private readonly List<Condition> _conditions;
        private readonly string _tableName;

        public QueryDeleteStatement(string tableName)
        {
            this._tableName = tableName;
            this._conditions = new List<Condition>();
        }
        public List<Condition> Conditions => this._conditions;
        public string TableName => this._tableName;

        public IQueryDeleteStatement Where(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            this._conditions.Add(condition);
            return this;
        }
    }
}
