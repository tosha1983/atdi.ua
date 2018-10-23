using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryInsertStatement<TModel> : IQueryInsertStatement<TModel>
    {
        private readonly Type ModelType = typeof(TModel);
        private readonly string _tableName;
        private readonly List<ColumnValue> _columnsValues;

        public QueryInsertStatement()
        {
            this._tableName = (ModelType.Name[0] == 'I' ? ModelType.Name.Substring(1, ModelType.Name.Length - 1) : ModelType.Name);
            this._columnsValues = new List<ColumnValue>();
        }

        public QueryInsertStatement(string tableName)
        {
            this._tableName = tableName;
            this._columnsValues = new List<ColumnValue>();
        }

        public string TableName => this._tableName;

        public List<ColumnValue> ColumnsValues => this._columnsValues;

        public IQueryInsertStatement<TModel> SetValue(ColumnValue columnValue)
        {
            if (columnValue == null)
            {
                throw new ArgumentNullException(nameof(columnValue));
            }
            this._columnsValues.Add(columnValue);
            return this;
        }

        public IQueryInsertStatement<TModel> SetValue<TValue>(System.Linq.Expressions.Expression<Func<TModel, TValue>> columnsExpression, TValue value)
        {
            var memberName = QuerySelectStatement<TModel>.GetMemberName(columnsExpression);
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            this._columnsValues.Add(QuerySelectStatement<TModel>.GetColumnValue(value, memberName));
            return this;
        }

      
        public IQueryInsertStatement<TModel> SetValues(ColumnValue[] columnsValues)
        {
            if (columnsValues == null)
            {
                throw new ArgumentNullException(nameof(columnsValues));
            }
            this._columnsValues.AddRange(columnsValues);
            return this;
        }
    }
    internal sealed class QueryInsertStatement : IQueryInsertStatement
    {
        private readonly string _tableName;
        private readonly List<ColumnValue> _columnsValues;

        public QueryInsertStatement(string tableName)
        {
            this._tableName = tableName;
            this._columnsValues = new List<ColumnValue>();
        }

        public string TableName => this._tableName;

        public List<ColumnValue> ColumnsValues => this._columnsValues;

        public IQueryInsertStatement SetValue(ColumnValue columnValue)
        {
            if (columnValue == null)
            {
                throw new ArgumentNullException(nameof(columnValue));
            }

            this._columnsValues.Add(columnValue);
            return this;
        }

        public IQueryInsertStatement SetValues(ColumnValue[] columnsValues)
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
