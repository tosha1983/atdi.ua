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
            private readonly Dictionary<string, ColumnDescriptor> _selectColumns;
            private readonly Dictionary<string, ColumnDescriptor> _sortColumns;
            private readonly Dictionary<string, ColumnDescriptor> _conditionColumns;

            public TableDescriptor(string tableName)
            {
                this.Name = tableName;
                this._columns = new Dictionary<string, ColumnDescriptor>();
                this._selectColumns = new Dictionary<string, ColumnDescriptor>();
                this._sortColumns = new Dictionary<string, ColumnDescriptor>();
                this._conditionColumns = new Dictionary<string, ColumnDescriptor>();
            }

            public string Name { get; private set; }

            public IReadOnlyDictionary<string, ColumnDescriptor> Columns => this._columns;

            public IReadOnlyDictionary<string, ColumnDescriptor> SelectColumns => this._selectColumns;

            public IReadOnlyDictionary<string, ColumnDescriptor> SortColumns => this._sortColumns;

            public IReadOnlyDictionary<string, ColumnDescriptor> ConditionColumns => this._conditionColumns;

            public void AppendSelectColumns(string[] columns)
            {
                foreach (var column in columns)
                {
                    var descriptor = new ColumnDescriptor
                    {
                        Table = this.Name,
                        Name = column
                    };

                    this.AppendSelectColumn(descriptor);
                }
            }

            public void AppendSelectColumn(ColumnDescriptor column)
            {
                if (column.Table != this.Name)
                {
                    throw new InvalidOperationException("Not match table name");
                }

                if (this.Columns.ContainsKey(column.Name))
                {
                    if (!this._selectColumns.ContainsKey(column.Name))
                    {
                        this._selectColumns[column.Name] = column;
                    }
                    return;
                }
                this._columns[column.Name] = column;
                this._selectColumns[column.Name] = column;
            }

            public ColumnDescriptor AppendSortColumn(string columnName)
            {
                if (this.Columns.ContainsKey(columnName))
                {
                    var descriptor = this.Columns[columnName];
                    if (!this._sortColumns.ContainsKey(columnName))
                    {
                        this._sortColumns[columnName] = descriptor;
                    }
                    return descriptor;
                }
                else
                {
                    var descriptor = new ColumnDescriptor
                    {
                        Table = this.Name,
                        Name = columnName
                    };
                    this._columns[columnName] = descriptor;
                    this._sortColumns[columnName] = descriptor;
                    return descriptor;
                }
            }

            public ColumnDescriptor AppendConditionColumn(string columnName)
            {
                if (this._columns.ContainsKey(columnName))
                {
                    var descriptor = this._columns[columnName];
                    if (!this._conditionColumns.ContainsKey(columnName))
                    {
                        this._conditionColumns[columnName] = descriptor;
                    }
                    return descriptor;
                }
                else
                {
                    var descriptor = new ColumnDescriptor
                    {
                        Table = this.Name,
                        Name = columnName
                    };
                    this._columns[columnName] = descriptor;
                    this._conditionColumns[columnName] = descriptor;
                    return descriptor;
                }
            }
        }

        internal sealed class OrderByColumnDescriptor
        {
            public ColumnDescriptor Column { get; set; }

            public SortDirection Direction { get; set; }
        }

        private readonly TableDescriptor _table;
        private readonly List<Condition> _conditions;
        private readonly List<OrderByColumnDescriptor> _orderByColumns;
        private DataLimit _limit;
        private bool _isDistinct;

        public QuerySelectStatement(string tableName, ILogger logger) : base(logger)
        {
            this._table = new TableDescriptor(tableName);
            this._conditions = new List<Condition>();
            this._orderByColumns = new List<OrderByColumnDescriptor>();
            this._limit = null;
            this._isDistinct = false;
        }

        public TableDescriptor Table => this._table;

        public List<Condition> Conditions => this._conditions;

        public List<OrderByColumnDescriptor> Orders => this._orderByColumns;

        public bool IsDistinct => this._isDistinct;

        public DataLimit Limit => this._limit;

        public IQuerySelectStatement OnTop(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (this._limit == null)
            {
                this._limit = new DataLimit();
            }
            this._limit.Value  = count;
            this._limit.Type = LimitValueType.Records;

            return this;
        }

        public IQuerySelectStatement OrderByAsc(params string[] columns)
        {
            foreach (var column in columns)
            {
                var columnDdescriptor = this._table.AppendSortColumn(column);
                var orderByDscriptor = new OrderByColumnDescriptor
                {
                    Column = columnDdescriptor,
                    Direction = SortDirection.Ascending
                };
                this._orderByColumns.Add(orderByDscriptor);
            }
            return this;
        }

        public IQuerySelectStatement OrderByDesc(params string[] columns)
        {
            foreach (var column in columns)
            {
                var columnDdescriptor = this._table.AppendSortColumn(column);
                var orderByDscriptor = new OrderByColumnDescriptor
                {
                    Column = columnDdescriptor,
                    Direction = SortDirection.Descending 
                };
                this._orderByColumns.Add(orderByDscriptor);
            }
            return this;
        }

        public IQuerySelectStatement Select(params string[] columns)
        {
            if (columns == null || columns.Length == 0)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            _table.AppendSelectColumns(columns);
            return this;
        }

        public IQuerySelectStatement Where(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            this._conditions.Add(condition);
            AppendColumnsFromCondition(condition);
            return this;
        }


        private void AppendColumnsFromCondition(Condition condition)
        {
            if (condition.Type == ConditionType.Complex)
            {
                var complexCondition = condition as ComplexCondition;
                var conditions = complexCondition.Conditions;
                if (conditions != null && conditions.Length > 0)
                {
                    for (int i = 0; i < conditions.Length; i++)
                    {
                        AppendColumnsFromCondition(conditions[i]);
                    }
                }
                return;
            }
            else if (condition.Type == ConditionType.Expression)
            {
                var conditionExpression = condition as ConditionExpression;
                AppendColumnFromOperand(conditionExpression.LeftOperand);
                AppendColumnFromOperand(conditionExpression.RightOperand);
            }
        }

        private void AppendColumnFromOperand(Operand operand)
        {
            if (operand == null)
            {
                return;
            }
            if (operand.Type == OperandType.Column)
            {
                var columnOperand = operand as ColumnOperand;
                this._table.AppendConditionColumn(columnOperand.ColumnName);
            }
        }

        public IQuerySelectStatement OnPercentTop(int percent)
        {
            if (this._limit == null)
            {
                this._limit = new DataLimit();
            }
            this._limit.Value = percent;
            this._limit.Type = LimitValueType.Percent;

            return this;
        }

        public IQuerySelectStatement Distinct()
        {
            this._isDistinct = true;
            return this;
        }
    }
}
