using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
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
