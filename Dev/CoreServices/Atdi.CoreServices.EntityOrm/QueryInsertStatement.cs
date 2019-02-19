using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Contracts.CoreServices.EntityOrm;
using System.Linq.Expressions;
using Atdi.DataModels.DataConstraint;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryInsertStatement<TModel> : IQueryInsertStatement<TModel>
    {
        private readonly Type ModelType = typeof(TModel);
        private readonly string _tableName;
        private readonly List<ColumnValue> _columnsValues;
        private readonly QueryInsertStatement _statement;
        private readonly List<string> _selectColumns;
        private readonly QuerySelectStatement _selectStatement;


        public QueryInsertStatement Statement => _statement;
        public QuerySelectStatement SelectStatement => _selectStatement;



        public QueryInsertStatement()
        {
            this._tableName = (ModelType.Name[0] == 'I' ? ModelType.Name.Substring(1, ModelType.Name.Length - 1) : ModelType.Name);
            this._columnsValues = new List<ColumnValue>();
            this._statement = new QueryInsertStatement(this._tableName);
            this._selectColumns = new List<string>();
            this._selectStatement = new QuerySelectStatement(TableName);
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
            this._columnsValues.Add(columnValue);
            this._statement.ColumnsValues.Add(columnValue);
            return this;
        }

        public IQueryInsertStatement<TModel> SetValue<TValue>(System.Linq.Expressions.Expression<Func<TModel, TValue>> columnsExpression, TValue value)
        {
            var memberName = QuerySelectStatement<TModel>.GetMemberName(columnsExpression);
            this._columnsValues.Add(QuerySelectStatement<TModel>.GetColumnValue(value, memberName));
            this._statement.ColumnsValues.Add(QuerySelectStatement<TModel>.GetColumnValue(value, memberName));
            return this;
        }



        public IQueryInsertStatement<TModel> SetValues(ColumnValue[] columnsValues)
        {
            this._columnsValues.AddRange(columnsValues);
            this._statement.ColumnsValues.AddRange(columnsValues);
            return this;
        }

        public IQueryInsertStatement<TModel> Select(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            if (columnsExpressions == null)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }
            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var memberName = QuerySelectStatement<TModel>.GetMemberName(columnsExpressions[i]);
                if (!string.IsNullOrEmpty(memberName))
                {
                    _selectColumns.Add(memberName);
                }
            }
            _selectStatement.Select(_selectColumns.ToArray());
            return this;
        }

    }
    internal sealed class QueryInsertStatement : IQueryInsertStatement
    {
        private readonly string _tableName;
        private readonly List<ColumnValue> _columnsValues;
        private readonly List<string> _selectColumns;
        private readonly QuerySelectStatement _selectStatement;

        public QueryInsertStatement(string tableName)
        {
            this._tableName = tableName;
            this._columnsValues = new List<ColumnValue>();
            this._selectColumns = new List<string>();
            this._selectStatement = new QuerySelectStatement(TableName);

        }

        public string TableName => this._tableName;

        public List<ColumnValue> ColumnsValues => this._columnsValues;
        public QuerySelectStatement SelectStatement => _selectStatement;

        public IQueryInsertStatement Select(params string[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }
            if (columns != null)
            {
                _selectColumns.AddRange(columns.ToList());
            }
            _selectStatement.Select(_selectColumns.ToArray());
            return this;
        }

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
