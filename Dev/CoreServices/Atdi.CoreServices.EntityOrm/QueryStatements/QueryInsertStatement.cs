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
    
    internal class QueryInsertStatement : IQueryInsertStatement
    {
        private readonly InsertQueryDescriptor _queryDescriptor;
        private readonly QuerySelectStatement _selectStatement;

        public QueryInsertStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._queryDescriptor = new InsertQueryDescriptor(entityOrm, entityMetadata);
            this._selectStatement = new QuerySelectStatement(entityOrm, entityMetadata);
        }

        public InsertQueryDescriptor InsertDecriptor => this._queryDescriptor;

        public SelectQueryDescriptor SelectDecriptor => _selectStatement.SelectDecriptor;

        public IQueryInsertStatement Select(params string[] columns)
        {
            _selectStatement.Select(columns);
            return this;
        }

        public IQueryInsertStatement SetValue(ColumnValue columnValue)
        {
            if (columnValue == null)
            {
                throw new ArgumentNullException(nameof(columnValue));
            }

            this._queryDescriptor.AppendValue(columnValue);
            return this;
        }

        public IQueryInsertStatement SetValues(ColumnValue[] columnsValues)
        {
            if (columnsValues == null)
            {
                throw new ArgumentNullException(nameof(columnsValues));
            }
            for (int i = 0; i < columnsValues.Length; i++)
            {
                this.SetValue(columnsValues[i]);
            }
            return this;
        }

    }

    internal sealed class QueryInsertStatement<TModel> : QueryInsertStatement, IQueryInsertStatement<TModel>
    {
        public QueryInsertStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
            : base(entityOrm, entityMetadata)
        {
        }

        //public IQueryInsertStatement<TModel> SetValue(ColumnValue columnValue)
        //{
        //    this.SetValue(columnValue); 
        //    return this;
        //}

        public IQueryInsertStatement<TModel> SetValue<TValue>(Expression<Func<TModel, TValue>> columnsExpression, TValue value)
        {
            var memberName = columnsExpression.Body.GetMemberName();
            var columnValue = ColumnValue.Create(value, memberName);
            this.SetValue(columnValue);
            return this;
        }

        //public IQueryInsertStatement<TModel> SetValues(ColumnValue[] columnsValues)
        //{
        //    this.SetValues(columnsValues);
        //    return this;
        //}

        public IQueryInsertStatement<TModel> Select(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            if (columnsExpressions == null)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }

            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var memberName = columnsExpressions[i].Body.GetMemberName();
                this.Select(memberName);
            }
            return this;
        }

    }

}
