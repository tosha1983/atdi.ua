using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
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
