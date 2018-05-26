using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class QuerySelectStatement : LoggedObject, IQuerySelectStatement
    {
        internal sealed class ColumnDescriptor
        {
            public string Table { get; set; }

            public string Name { get; set; }

            public string Alias { get; set; }

            public int Ordinal { get; set; }
        }

        internal sealed class TableDescriptor
        {
            private readonly Dictionary<string, ColumnDescriptor> _columns;
            public TableDescriptor(string tableName)
            {
                this.Name = tableName;
                this._columns = new Dictionary<string, ColumnDescriptor>();
            }

            public string Name { get; private set; }

            public IReadOnlyDictionary<string, ColumnDescriptor> Columns => this._columns;

            public void AppendColumns(string[] columns)
            {
                foreach (var column in columns)
                {
                    var descriptor = new ColumnDescriptor
                    {
                        Table = this.Name,
                        Name = column
                    };

                    this.AppendColumn(descriptor);
                }
            }

            public void AppendColumn(ColumnDescriptor column)
            {
                if (column.Table != this.Name)
                {
                    throw new InvalidOperationException("Not match table name");
                }

                if (this.Columns.ContainsKey(column.Name))
                {
                    return;
                }
                this._columns[column.Name] = column;
            }

            public ColumnDescriptor AppendColumn(string columnName)
            {
                
                if (this.Columns.ContainsKey(columnName))
                {
                    return this.Columns[columnName];
                }
                var descriptor = new ColumnDescriptor
                {
                    Table = this.Name,
                    Name = columnName
                };
                this._columns[columnName] = descriptor;
                return descriptor;
            }
        }

        internal sealed class OrderByColumnDescriptor
        {
            public ColumnDescriptor Column { get; set; }

            public OrderType OrderType { get; set; }
        }

        private readonly TableDescriptor _table;
        private readonly List<Condition> _conditions;
        private readonly List<OrderByColumnDescriptor> _orderByColumns;
        private int _onTop;
        private bool _isDistinct;

        public QuerySelectStatement(string tableName, ILogger logger) : base(logger)
        {
            this._table = new TableDescriptor(tableName);
            this._conditions = new List<Condition>();
            this._orderByColumns = new List<OrderByColumnDescriptor>();
            this._onTop = -1;
            this._isDistinct = false;
        }

        public TableDescriptor Table => this._table;

        public List<Condition> Conditions => this._conditions;

        public List<OrderByColumnDescriptor> Orders => this._orderByColumns;

        public bool IsDistinct => this._isDistinct;

        public int Limit => this._onTop;

        public IQuerySelectStatement OnTop(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            this._onTop = count;
            return this;
        }

        public IQuerySelectStatement OrderByAsc(params string[] columns)
        {
            foreach (var column in columns)
            {
                var columnDdescriptor = this._table.AppendColumn(column);
                var orderByDscriptor = new OrderByColumnDescriptor
                {
                    Column = columnDdescriptor,
                    OrderType = OrderType.Ascending
                };
                this._orderByColumns.Add(orderByDscriptor);
            }
            return this;
        }

        public IQuerySelectStatement OrderByDesc(params string[] columns)
        {
            foreach (var column in columns)
            {
                var columnDdescriptor = this._table.AppendColumn(column);
                var orderByDscriptor = new OrderByColumnDescriptor
                {
                    Column = columnDdescriptor,
                    OrderType = OrderType.Descending 
                };
                this._orderByColumns.Add(orderByDscriptor);
            }
            return this;
        }

        public IQuerySelectStatement Select(params string[] columns)
        {
            _table.AppendColumns(columns);
            return this;
        }

        public IQuerySelectStatement Where(Condition condition)
        {
            this._conditions.Add(condition);
            return this;
        }
    }
}
