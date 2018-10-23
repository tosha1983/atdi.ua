using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
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
    internal sealed class QueryUpdateStatement<TModel> : IQueryUpdateStatement<TModel>
    {
       
        private readonly Type ModelType = typeof(TModel);
        private readonly List<Condition> _conditions;
        private readonly string _tableName;
        private readonly List<ColumnValue> _columnsValues;

        public QueryUpdateStatement()
        {
            this._tableName = (ModelType.Name[0] == 'I' ? ModelType.Name.Substring(1, ModelType.Name.Length - 1) : ModelType.Name);
            this._conditions = new List<Condition>();
            this._columnsValues = new List<ColumnValue>();
        }

        public QueryUpdateStatement(string tableName)
        {
            this._tableName = tableName;
            this._conditions = new List<Condition>();
            this._columnsValues = new List<ColumnValue>();
        }

        public List<Condition> Conditions => this._conditions;

        public string TableName => this._tableName;

        public List<ColumnValue> ColumnsValues => this._columnsValues;


      
        public IQueryUpdateStatement<TModel> SetValue<TValue>(Expression<Func<TModel, TValue>> columnsExpression, TValue value)
        {
            var memberName = QuerySelectStatement<TModel>.GetMemberName(columnsExpression);
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            this._columnsValues.Add(QuerySelectStatement<TModel>.GetColumnValue(value, memberName));
            return this;
        }

        public IQueryUpdateStatement<TModel> Where(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            this._conditions.Add(condition);
            return this;
        }

        public IQueryUpdateStatement<TModel> Where(Expression<Func<TModel, bool>> expression)
        {
            this.Where(Atdi.CoreServices.EntityOrm.QuerySelectStatement<TModel>.ParseConditionExpression(expression));
            return this;
        }

        public IQueryUpdateStatement<TModel> Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
        {
            this.Where(Atdi.CoreServices.EntityOrm.QuerySelectStatement<TModel>.ParseConditionExpression(expression));
            return this;
        }

        public IQueryUpdateStatement<TModel> Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
        {
            this.Where(Atdi.CoreServices.EntityOrm.QuerySelectStatement<TModel>.ParseCondition(columnExpression, conditionOperator, values));
            return this;
        }

        IQueryUpdateStatement<TModel> SetValue(ColumnValue columnValue)
        {
            if (columnValue == null)
            {
                throw new ArgumentNullException(nameof(columnValue));
            }

            this._columnsValues.Add(columnValue);
            return this;
        }

        IQueryUpdateStatement<TModel> SetValues(ColumnValue[] columnsValues)
        {
            if (columnsValues == null)
            {
                throw new ArgumentNullException(nameof(columnsValues));
            }

            this._columnsValues.AddRange(columnsValues);
            return this;
        }
    }


    internal sealed class QueryUpdateStatement : IQueryUpdateStatement
    {
        private readonly List<Condition> _conditions;
        private readonly string _tableName;
        private readonly List<ColumnValue> _columnsValues;

        public QueryUpdateStatement(string tableName)
        {
            this._tableName = tableName;
            this._conditions = new List<Condition>();
            this._columnsValues = new List<ColumnValue>();
        }

        public List<Condition> Conditions => this._conditions;

        public string TableName => this._tableName;

        public List<ColumnValue> ColumnsValues => this._columnsValues;
        
        public IQueryUpdateStatement Where(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            this._conditions.Add(condition);
            return this;
        }

        IQueryUpdateStatement IQueryUpdateStatement.SetValue(ColumnValue columnValue)
        {
            if (columnValue == null)
            {
                throw new ArgumentNullException(nameof(columnValue));
            }

            this._columnsValues.Add(columnValue);
            return this;
        }

        IQueryUpdateStatement IQueryUpdateStatement.SetValues(ColumnValue[] columnsValues)
        {
            if (columnsValues == null)
            {
                throw new ArgumentNullException(nameof(columnsValues));
            }

            this._columnsValues.AddRange(columnsValues);
            return this;
        }
    }
}
