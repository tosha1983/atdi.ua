using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels;

namespace Atdi.CoreServices.EntityOrm
{

    public sealed class QuerySelectStatement : IQuerySelectStatement
    {
       
        public sealed class ColumnDescriptor
        {
            public string Table { get; set; }

            public string DBTable { get; set; }

            public string Name { get; set; }

            public string Alias { get; set; }

            public int Ordinal { get; set; }

            public string Expression { get; set; }


        }

        public sealed class TableDescriptor
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
                        Name = column,
                        Expression = ""
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
                    else return;
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

        public sealed class OrderByColumnDescriptor
        {
            public ColumnDescriptor Column { get; set; }

            public SortDirection Direction { get; set; }
        }

        private readonly TableDescriptor _table;
        private readonly List<Condition> _conditions;
        private readonly List<OrderByColumnDescriptor> _orderByColumns;
        private DataLimit _limit;
        private bool _isDistinct;

        public QuerySelectStatement(string tableName)
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

    internal sealed class QuerySelectStatement<TModel> : IQuerySelectStatement<TModel>
    {
        private readonly Type ModelType = typeof(TModel);
        private readonly QuerySelectStatement _statement;
        private static IFormatProvider CultureEnUs = new System.Globalization.CultureInfo("en-US");

        public QuerySelectStatement Statement => _statement;

        public QuerySelectStatement()
        {
            string modelTypeName = (ModelType.Name[0] == 'I' ? ModelType.Name.Substring(1, ModelType.Name.Length - 1) : ModelType.Name);
            this._statement = new QuerySelectStatement(modelTypeName); 
        }

        public IQuerySelectStatement<TModel> Distinct()
        {
            this._statement.Distinct();
            return this;
        }

        public IQuerySelectStatement<TModel> OnPercentTop(int percent)
        {
            this._statement.OnPercentTop(percent);
            return this;
        }

        public IQuerySelectStatement<TModel> OnTop(int count)
        {
            this._statement.OnTop(count);
            return this;
        }

        public IQuerySelectStatement<TModel> OrderByAsc(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            if (columnsExpressions == null || columnsExpressions.Length == 0)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }

            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var columnName = GetMemberName(columnsExpressions[i]);
                _statement.OrderByAsc(columnName);
            }

            return this;
        }

        public IQuerySelectStatement<TModel> OrderByDesc(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            if (columnsExpressions == null || columnsExpressions.Length == 0)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }

            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var columnName = GetMemberName(columnsExpressions[i]);
                _statement.OrderByDesc(columnName);
            }

            return this;
        }

        public IQuerySelectStatement<TModel> Select(params Expression<Func<TModel, object>>[] columnsExpressions)
        {
            if (columnsExpressions == null || columnsExpressions.Length == 0)
            {
                throw new ArgumentNullException(nameof(columnsExpressions));
            }

            for (int i = 0; i < columnsExpressions.Length; i++)
            {
                var columnName = GetMemberName(columnsExpressions[i]);
                _statement.Select(columnName);
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOperatorMultiValues(ConditionOperator conditionOperator)
        {
            return
                conditionOperator == ConditionOperator.In ||
                conditionOperator == ConditionOperator.NotIn ||
                conditionOperator == ConditionOperator.Between ||
                conditionOperator == ConditionOperator.NotBetween;
        }

        public IQuerySelectStatement<TModel> Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
        {
            this._statement.Where(ParseCondition(columnExpression, conditionOperator, values));
            return this;
        }

        private static ValuesOperand ParseValuesOperand<TValue>(params TValue[] values)
        {
            var type = typeof(TValue);
            if (type == typeof(string))
            {
                return new StringValuesOperand
                {
                    Values = values.Select(o => (string)(object)o).ToArray()
                };
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                return new IntegerValuesOperand
                {
                    Values = values.Select(o => (int?)(object)o).ToArray()
                };
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return new BooleanValuesOperand
                {
                    Values = values.Select(o => (bool?)(object)o).ToArray()
                };
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new DateTimeValuesOperand
                {
                    Values = values.Select(o => (DateTime?)(object)o).ToArray()
                };
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                return new FloatValuesOperand
                {
                    Values = values.Select(o => (float?)(object)o).ToArray()
                };
            }
            if (type == typeof(double) || type == typeof(double?))
            {
                return new DoubleValuesOperand
                {
                    Values = values.Select(o => (double?)(object)o).ToArray()
                };
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return new DecimalValuesOperand
                {
                    Values = values.Select(o => (decimal?)(object)o).ToArray()
                };
            }
            if (type == typeof(byte) || type == typeof(byte?))
            {
                return new ByteValuesOperand
                {
                    Values = values.Select(o => (byte?)(object)o).ToArray()
                };
            }
            if (type == typeof(byte[]))
            {
                return new BytesValuesOperand
                {
                    Values = values.Select(o => (byte[])(object)o).ToArray()
                };
            }
            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return new GuidValuesOperand
                {
                    Values = values.Select(o => (Guid?)(object)o).ToArray()
                };
            }
            if (type == typeof(char) || type == typeof(char?))
            {
                return new CharValuesOperand
                {
                    Values = values.Select(o => (char?)(object)o).ToArray()
                };
            }
            if (type == typeof(short) || type == typeof(short?))
            {
                return new ShortValuesOperand
                {
                    Values = values.Select(o => (short?)(object)o).ToArray()
                };
            }
            if (type == typeof(UInt16) || type == typeof(UInt16?))
            {
                return new UnsignedShortValuesOperand
                {
                    Values = values.Select(o => (UInt16?)(object)o).ToArray()
                };
            }
            if (type == typeof(UInt32) || type == typeof(UInt32?))
            {
                return new UnsignedIntegerValuesOperand
                {
                    Values = values.Select(o => (UInt32?)(object)o).ToArray()
                };
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                return new LongValuesOperand
                {
                    Values = values.Select(o => (long?)(object)o).ToArray()
                };
            }
            if (type == typeof(UInt64) || type == typeof(UInt64?))
            {
                return new UnsignedLongValuesOperand
                {
                    Values = values.Select(o => (UInt64?)(object)o).ToArray()
                };
            }
            if (type == typeof(sbyte) || type == typeof(sbyte?))
            {
                return new SignedByteValuesOperand
                {
                    Values = values.Select(o => (sbyte?)(object)o).ToArray()
                };
            }
            if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
            {
                return new TimeValuesOperand
                {
                    Values = values.Select(o => (TimeSpan?)(object)o).ToArray()
                };
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new DateValuesOperand
                {
                    Values = values.Select(o => (DateTime?)(object)o).ToArray()
                };
            }
            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                return new DateTimeOffsetValuesOperand
                {
                    Values = values.Select(o => (DateTimeOffset?)(object)o).ToArray()
                };
            }
            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                return new DateTimeOffsetValuesOperand
                {
                    Values = values.Select(o => (DateTimeOffset?)(object)o).ToArray()
                };
            }
            if (type == typeof(Enum))
            {
                return new ClrEnumValuesOperand
                {
                    Values = values.Select(o => (Enum)(object)o).ToArray()
                };
            }
            if (type == typeof(Object))
            {
                return new ClrTypeValuesOperand
                {
                    Values = values.Select(o => (Object)(object)o).ToArray()
                };
            }

            throw new InvalidOperationException(Exceptions.ValueTypeNotSupported.With(type));
        }


        private static ValueOperand ParseValueOperand<TValue>(TValue value)
        {
            var type = typeof(TValue);
            if (type == typeof(string))
            {
                return new StringValueOperand
                {
                    Value = (string)(object)value
                };
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                return new IntegerValueOperand
                {
                    Value = (int?)(object)value
                };
            }
            if (type == typeof(Int32) || type == typeof(Int32?))
            {
                return new IntegerValueOperand
                {
                    Value = (Int32?)(object)value
                };
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return new BooleanValueOperand
                {
                    Value = (bool?)(object)value
                };
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new DateTimeValueOperand
                {
                    Value = (DateTime?)(object)value
                };
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                return new FloatValueOperand
                {
                    Value = (float?)(object)value
                };
            }
            if (type == typeof(double) || type == typeof(double?))
            {
                return new DoubleValueOperand
                {
                    Value = (double?)(object)value
                };
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return new DecimalValueOperand
                {
                    Value = (decimal?)(object)value
                };
            }
            if (type == typeof(byte) || type == typeof(byte?))
            {
                return new ByteValueOperand
                {
                    Value = (byte?)(object)value
                };
            }
            if (type == typeof(byte[]) )
            {
                return new BytesValueOperand
                {
                    Value = (byte[])(object)value
                };
            }
            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return new GuidValueOperand
                {
                    Value = (Guid?)(object)value
                };
            }
            if (type == typeof(char) || type == typeof(char?))
            {
                return new CharValueOperand
                {
                    Value = (char?)(object)value
                };
            }
            if (type == typeof(char) || type == typeof(char?))
            {
                return new CharValueOperand
                {
                    Value = (char?)(object)value
                };
            }
            if (type == typeof(short) || type == typeof(short?))
            {
                return new ShortValueOperand
                {
                    Value = (short?)(object)value
                };
            }
            if (type == typeof(UInt16) || type == typeof(UInt16?))
            {
                return new UnsignedShortValueOperand
                {
                    Value = (UInt16?)(object)value
                };
            }
            if (type == typeof(UInt32) || type == typeof(UInt32?))
            {
                return new UnsignedIntegerValueOperand
                {
                    Value = (UInt32?)(object)value
                };
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                return new LongValueOperand
                {
                    Value = (long?)(object)value
                };
            }
            if (type == typeof(UInt64) || type == typeof(UInt64?))
            {
                return new UnsignedLongValueOperand
                {
                    Value = (UInt64?)(object)value
                };
            }
            if (type == typeof(sbyte) || type == typeof(sbyte?))
            {
                return new SignedByteValueOperand
                {
                    Value = (sbyte?)(object)value
                };
            }
            if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
            {
                return new TimeValueOperand
                {
                    Value = (TimeSpan?)(object)value
                };
            }
            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                return new DateTimeOffsetValueOperand
                {
                    Value = (DateTimeOffset?)(object)value
                };
            }
            if (type == typeof(Enum) || type == typeof(Enum))
            {
                throw new InvalidOperationException(Exceptions.ValueTypeNotSupported.With(type));
            }
            if (type == typeof(Object) || type == typeof(Object))
            {
                throw new InvalidOperationException(Exceptions.ValueTypeNotSupported.With(type));
            }
            throw new InvalidOperationException(Exceptions.ValueTypeNotSupported.With(type));
        }

        public static Condition ParseCondition<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
        {
            var condition = new ConditionExpression
            {
                LeftOperand = new ColumnOperand { ColumnName = GetMemberName(columnExpression) },
                Operator = conditionOperator
            };

            if (IsOperatorMultiValues(conditionOperator))
            {
                if (values == null || values.Length == 0)
                {
                    throw new ArgumentException(nameof(values));
                }

                if ((conditionOperator == ConditionOperator.Between || conditionOperator == ConditionOperator.NotBetween) &&  values.Length != 2)
                {
                    throw new ArgumentException(nameof(values));
                }

                condition.RightOperand = ParseValuesOperand<TValue>(values);
            }
            else
            {
                if (conditionOperator == ConditionOperator.IsNull || conditionOperator == ConditionOperator.IsNotNull)
                {
                    return condition;
                }

                if (values == null || values.Length == 0)
                {
                    throw new ArgumentException(nameof(values));
                }

                var value = values[0];

                if (conditionOperator == ConditionOperator.Equal && value == null)
                {
                    condition.Operator = ConditionOperator.IsNull;
                    return condition;
                }
                if (conditionOperator == ConditionOperator.NotEqual && value == null)
                {
                    condition.Operator = ConditionOperator.IsNotNull;
                    return condition;
                }
                condition.RightOperand = ParseValueOperand<TValue>(value);
            }

            return condition;
        }

        public IQuerySelectStatement<TModel> Where(Expression<Func<TModel, bool>> expression)
        {
            this._statement.Where(ParseConditionExpression(expression));
            return this;
        }

        public IQuerySelectStatement<TModel> Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
        {
            this._statement.Where(ParseConditionExpression(expression));
            return this;
        }

        public static Condition ParseConditionExpression(Expression<Func<TModel, bool>> expression)
        {
            var body = expression.Body;
            return ParseExpression(body);
        }

        public static Condition ParseConditionExpression(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
        {
            var body = expression.Body;
            return ParseExpression(body);
        }

        private static Condition ParseExpression(Expression expression)
        {
            if (expression is MethodCallExpression methodCallExpression)
            {
                return ParseCallMethodExpression(methodCallExpression);
            }

            if (expression is BinaryExpression binaryExpression)
            {
                return ParseBinaryExpression(binaryExpression);
            }

            throw new InvalidOperationException(Exceptions.ExpressionNotSupported.With(expression.ToString()));
        }
        private static Condition ParseCallMethodExpression(MethodCallExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.Method.Name == "Like" && expression.Arguments.Count == 2)
            {
                return new ConditionExpression
                {
                    Operator = ConditionOperator.Like,
                    LeftOperand = ParseOperand(expression.Arguments[0]),
                    RightOperand = ParseOperand(expression.Arguments[1])
                };
            }

            if (expression.Method.Name == "NotLike" && expression.Arguments.Count == 2)
            {
                return new ConditionExpression
                {
                    Operator = ConditionOperator.NotLike,
                    LeftOperand = ParseOperand(expression.Arguments[0]),
                    RightOperand = ParseOperand(expression.Arguments[1])
                };
            }

            if (expression.Method.Name == "Between" && expression.Arguments.Count == 3)
            {
                return new ConditionExpression
                {
                    Operator = ConditionOperator.Between,
                    LeftOperand = ParseOperand(expression.Arguments[0]),
                    RightOperand = ParseValuesOperand(expression.Arguments[1], expression.Arguments[2])
                };
            }
            if (expression.Method.Name == "NotBetween" && expression.Arguments.Count == 3)
            {
                return new ConditionExpression
                {
                    Operator = ConditionOperator.NotBetween,
                    LeftOperand = ParseOperand(expression.Arguments[0]),
                    RightOperand = ParseValuesOperand(expression.Arguments[1], expression.Arguments[2])
                };
            }

            if (expression.Method.Name == "In" && expression.Arguments.Count >= 2)
            {
                var args = new Expression[expression.Arguments.Count - 1];
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = expression.Arguments[i + 1];
                }
                return new ConditionExpression
                {
                    Operator = ConditionOperator.In,
                    LeftOperand = ParseOperand(expression.Arguments[0]),
                    RightOperand = ParseValuesOperand(args)
                };
            }

            if (expression.Method.Name == "NotIn" && expression.Arguments.Count >= 2)
            {
                var args = new Expression[expression.Arguments.Count - 1];
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = expression.Arguments[i + 1];
                }
                return new ConditionExpression
                {
                    Operator = ConditionOperator.NotIn,
                    LeftOperand = ParseOperand(expression.Arguments[0]),
                    RightOperand = ParseValuesOperand(args)
                };
            }

            throw new InvalidOperationException(Exceptions.ExpressionCallMethodNotSupported.With(expression.Method.Name));
        }
        private static Condition ParseBinaryExpression(BinaryExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse)
            {
                var condition = new ComplexCondition
                {
                    Operator = expression.NodeType == ExpressionType.AndAlso ? LogicalOperator.And : LogicalOperator.Or
                };
                var left = ParseExpression(expression.Left);
                var right = ParseExpression(expression.Right);

                condition.Conditions = new Condition[] { left, right };
                return condition;
            }

            if (expression.NodeType == ExpressionType.Equal ||
                expression.NodeType == ExpressionType.NotEqual ||
                expression.NodeType == ExpressionType.GreaterThan ||
                expression.NodeType == ExpressionType.GreaterThanOrEqual ||
                expression.NodeType == ExpressionType.LessThan ||
                expression.NodeType == ExpressionType.LessThanOrEqual)
            {
                var condition = new ConditionExpression
                {
                    LeftOperand = ParseOperand(expression.Left),
                    Operator = ToConditionOperator(expression.NodeType),
                    RightOperand = ParseOperand(expression.Right),
                };

                return condition;
            }

            throw new InvalidOperationException(Exceptions.ExpressionNodeTypeNotSupported.With(expression.NodeType.ToString()));
        }

        private static ConditionOperator ToConditionOperator(ExpressionType type)
        {
            if (type == ExpressionType.Equal )
            {
                return ConditionOperator.Equal;
            }
            if (type == ExpressionType.NotEqual )
            {
                return ConditionOperator.NotEqual;
            }
            if (type == ExpressionType.GreaterThan)
            {
                return ConditionOperator.GreaterThan;
            }
            if (type == ExpressionType.GreaterThanOrEqual)
            {
                return ConditionOperator.GreaterEqual;
            }
            if (type == ExpressionType.LessThan)
            {
                return ConditionOperator.LessThan;
            }
            if (type == ExpressionType.LessThanOrEqual)
            {
                return ConditionOperator.LessEqual;
            }

            throw new InvalidOperationException(Exceptions.ExpressionTypeNotSupported.With(type));
        }
        private static Operand ParseOperand(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return new ColumnOperand
                {
                    ColumnName = expression.GetMemberName()
                };
            }

            if (expression.NodeType == ExpressionType.Constant)
            {
                var constantExpression = expression as ConstantExpression;
                if (constantExpression.Type == typeof(string))
                {
                    return new StringValueOperand
                    {
                        Value = (string)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(int) || constantExpression.Type == typeof(int?))
                {
                    return new IntegerValueOperand
                    {
                        Value = (int?)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(bool) || constantExpression.Type == typeof(bool?))
                {
                    return new BooleanValueOperand
                    {
                        Value = (bool?)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(DateTime) || constantExpression.Type == typeof(DateTime?))
                {
                    return new DateTimeValueOperand
                    {
                        Value = (DateTime?)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(float) || constantExpression.Type == typeof(float?))
                {
                    return new FloatValueOperand
                    {
                        Value = (float?)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(double) || constantExpression.Type == typeof(double?))
                {
                    return new DoubleValueOperand
                    {
                        Value = (double?)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(decimal) || constantExpression.Type == typeof(decimal?))
                {
                    return new DecimalValueOperand
                    {
                        Value = (decimal?)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(byte) || constantExpression.Type == typeof(byte?))
                {
                    return new ByteValueOperand
                    {
                        Value = (byte?)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(byte[]) )
                {
                    return new BytesValueOperand
                    {
                        Value = (byte[])constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(Guid) || constantExpression.Type == typeof(Guid?))
                {
                    return new GuidValueOperand
                    {
                        Value = (Guid?)constantExpression.Value
                    };
                }

                if (constantExpression.Type == typeof(char) || constantExpression.Type == typeof(char?))
                {
                    return new CharValueOperand
                    {
                        Value = (char?)(object)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(char) || constantExpression.Type == typeof(char?))
                {
                    return new CharValueOperand
                    {
                        Value = (char?)(object)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(short) || constantExpression.Type == typeof(short?))
                {
                    return new ShortValueOperand
                    {
                        Value = (short?)(object)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(UInt16) || constantExpression.Type == typeof(UInt16?))
                {
                    return new UnsignedShortValueOperand
                    {
                        Value = (UInt16?)(object)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(UInt32) || constantExpression.Type == typeof(UInt32?))
                {
                    return new UnsignedIntegerValueOperand
                    {
                        Value = (UInt32?)(object)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(long) || constantExpression.Type == typeof(long?))
                {
                    return new LongValueOperand
                    {
                        Value = (long?)(object)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(UInt64) || constantExpression.Type == typeof(UInt64?))
                {
                    return new UnsignedLongValueOperand
                    {
                        Value = (UInt64?)(object)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(sbyte) || constantExpression.Type == typeof(sbyte?))
                {
                    return new SignedByteValueOperand
                    {
                        Value = (sbyte?)(object)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(TimeSpan) || constantExpression.Type == typeof(TimeSpan?))
                {
                    return new TimeValueOperand
                    {
                        Value = (TimeSpan?)(object)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(DateTimeOffset) || constantExpression.Type == typeof(DateTimeOffset?))
                {
                    return new DateTimeOffsetValueOperand
                    {
                        Value = (DateTimeOffset?)(object)constantExpression.Value
                    };
                }
                if (constantExpression.Type == typeof(Enum) || constantExpression.Type == typeof(Enum))
                {
                    throw new InvalidOperationException(Exceptions.ValueTypeNotSupported.With(constantExpression.Type));
                }
                if (constantExpression.Type == typeof(Object) || constantExpression.Type == typeof(Object))
                {
                    throw new InvalidOperationException(Exceptions.ValueTypeNotSupported.With(constantExpression.Type));
                }

                throw new InvalidOperationException(Exceptions.ExpressionTypeNotSupported.With(constantExpression.Type));
            }

            throw new InvalidOperationException(Exceptions.ExpressionNodeTypeNotSupported.With(expression.NodeType.ToString()));
        }

        private static ValuesOperand ParseValuesOperand(params Expression[] expressions)
        {
            if (expressions == null || expressions.Length == 0)
            {
                throw new ArgumentNullException(nameof(expressions));
            }

            var expression = expressions[0];
            

            if (expression.NodeType == ExpressionType.Constant)
            {
                var constantExpressions = expressions as ConstantExpression[];
                var constantExpression = constantExpressions[0];
                if (constantExpression.Type == typeof(string))
                {
                    return new StringValuesOperand
                    {
                        Values = constantExpressions.Select(o => (string)o.Value).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(int) || constantExpression.Type == typeof(int?))
                {
                    return new IntegerValuesOperand
                    {
                        Values = constantExpressions.Select(o => (int?)o.Value).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(bool) || constantExpression.Type == typeof(bool?))
                {
                    return new BooleanValuesOperand
                    {
                        Values = constantExpressions.Select(o => (bool?)o.Value).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(DateTime) || constantExpression.Type == typeof(DateTime?))
                {
                    return new DateTimeValuesOperand
                    {
                        Values = constantExpressions.Select(o => (DateTime?)o.Value).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(float) || constantExpression.Type == typeof(float?))
                {
                    return new FloatValuesOperand
                    {
                        Values = constantExpressions.Select(o => (float?)o.Value).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(double) || constantExpression.Type == typeof(double?))
                {
                    return new DoubleValuesOperand
                    {
                        Values = constantExpressions.Select(o => (double?)o.Value).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(decimal) || constantExpression.Type == typeof(decimal?))
                {
                    return new DecimalValuesOperand
                    {
                        Values = constantExpressions.Select(o => (decimal?)o.Value).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(byte) || constantExpression.Type == typeof(byte?))
                {
                    return new ByteValuesOperand
                    {
                        Values = constantExpressions.Select(o => (byte?)o.Value).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(byte[]) )
                {
                    return new BytesValuesOperand
                    {
                        Values = constantExpressions.Select(o => (byte[])o.Value).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(Guid) || constantExpression.Type == typeof(Guid?))
                {
                    return new GuidValuesOperand
                    {
                        Values = constantExpressions.Select(o => (Guid?)o.Value).ToArray()
                    };
                }

                if (constantExpression.Type == typeof(char) || constantExpression.Type == typeof(char?))
                {
                    return new CharValuesOperand
                    {
                        Values = constantExpressions.Select(o => (char?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(short) || constantExpression.Type == typeof(short?))
                {
                    return new ShortValuesOperand
                    {
                        Values = constantExpressions.Select(o => (short?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(UInt16) || constantExpression.Type == typeof(UInt16?))
                {
                    return new UnsignedShortValuesOperand
                    {
                        Values = constantExpressions.Select(o => (UInt16?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(UInt32) || constantExpression.Type == typeof(UInt32?))
                {
                    return new UnsignedIntegerValuesOperand
                    {
                        Values = constantExpressions.Select(o => (UInt32?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(long) || constantExpression.Type == typeof(long?))
                {
                    return new LongValuesOperand
                    {
                        Values = constantExpressions.Select(o => (long?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(UInt64) || constantExpression.Type == typeof(UInt64?))
                {
                    return new UnsignedLongValuesOperand
                    {
                        Values = constantExpressions.Select(o => (UInt64?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(sbyte) || constantExpression.Type == typeof(sbyte?))
                {
                    return new SignedByteValuesOperand
                    {
                        Values = constantExpressions.Select(o => (sbyte?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(TimeSpan) || constantExpression.Type == typeof(TimeSpan?))
                {
                    return new TimeValuesOperand
                    {
                        Values = constantExpressions.Select(o => (TimeSpan?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(DateTime) || constantExpression.Type == typeof(DateTime?))
                {
                    return new DateValuesOperand
                    {
                        Values = constantExpressions.Select(o => (DateTime?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(DateTimeOffset) || constantExpression.Type == typeof(DateTimeOffset?))
                {
                    return new DateTimeOffsetValuesOperand
                    {
                        Values = constantExpressions.Select(o => (DateTimeOffset?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(DateTimeOffset) || constantExpression.Type == typeof(DateTimeOffset?))
                {
                    return new DateTimeOffsetValuesOperand
                    {
                        Values = constantExpressions.Select(o => (DateTimeOffset?)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(Enum))
                {
                    return new ClrEnumValuesOperand
                    {
                        Values = constantExpressions.Select(o => (Enum)(object)o).ToArray()
                    };
                }
                if (constantExpression.Type == typeof(Object))
                {
                    return new ClrTypeValuesOperand
                    {
                        Values = constantExpressions.Select(o => (Object)(object)o).ToArray()
                    };
                }


                throw new InvalidOperationException(Exceptions.ExpressionTypeNotSupported.With(constantExpression.Type));
            }

            throw new InvalidOperationException(Exceptions.ExpressionNodeTypeNotSupported.With(expression.NodeType.ToString()));
        }


        public static ColumnValue GetColumnValue(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        {
            ColumnValue result = null;
            switch (dataTypeMetadata.SourceVarType.ToString())
            {
                case "BOOL":
                    result = Convesrion.ConversionBool(value, nameColumn, dataTypeMetadata);
                    break;
                case "BIT":
                    result = Convesrion.ConversionBit(value, nameColumn, dataTypeMetadata);
                    break;
                case "BYTE":
                    result = Convesrion.ConversionByte(value, nameColumn, dataTypeMetadata);
                    break;
                case "BYTES":
                    result = Convesrion.ConversionBytes (value, nameColumn, dataTypeMetadata);
                    break;
                case "BLOB":
                    result = Convesrion.ConversionBlob(value, nameColumn, dataTypeMetadata);
                    break;
                case "INT08":
                    result = Convesrion.ConversionInt08(value, nameColumn, dataTypeMetadata);
                    break;
                case "INT16":
                    result = Convesrion.ConversionInt16(value, nameColumn, dataTypeMetadata);
                    break;
                case "INT32":
                    result = Convesrion.ConversionInt32(value, nameColumn, dataTypeMetadata);
                    break;
                case "INT64":
                    result = Convesrion.ConversionInt64(value, nameColumn, dataTypeMetadata);
                    break;
                case "NCHAR":
                    result = Convesrion.ConversionNchar(value, nameColumn, dataTypeMetadata);
                    break;
                case "NVARCHAR":
                    result = Convesrion.ConversionNvarChar(value, nameColumn, dataTypeMetadata);
                    break;
                case "NTEXT":
                    result = Convesrion.ConversionNText(value, nameColumn, dataTypeMetadata);
                    break;
                case "CHAR":
                    result = Convesrion.ConversionChar(value, nameColumn, dataTypeMetadata);
                    break;
                case "VARCHAR":
                    result = Convesrion.ConversionVarChar(value, nameColumn, dataTypeMetadata);
                    break;
                case "TEXT":
                    result = Convesrion.ConversionText(value, nameColumn, dataTypeMetadata);
                    break;
                case "TIME":
                    result = Convesrion.ConversionTime(value, nameColumn, dataTypeMetadata);
                    break;
                case "DATE":
                    result = Convesrion.ConversionDate(value, nameColumn, dataTypeMetadata);
                    break;
                case "DATETIME":
                    result = Convesrion.ConversionDateTime(value, nameColumn, dataTypeMetadata);
                    break;
                case "DATETIMEOFFSET":
                    result = Convesrion.ConversionDateTimeOffset(value, nameColumn, dataTypeMetadata);
                    break;
                case "MONEY":
                    result = Convesrion.ConversionMoney(value, nameColumn, dataTypeMetadata);
                    break;
                case "FLOAT":
                    result = Convesrion.ConversionFloat(value, nameColumn, dataTypeMetadata);
                    break;
                case "DOUBLE":
                    result = Convesrion.ConversionDouble(value, nameColumn, dataTypeMetadata);
                    break;
                case "DECIMAL":
                    result = Convesrion.ConversionDecimal(value, nameColumn, dataTypeMetadata);
                    break;
                case "GUID":
                    result = Convesrion.ConversionGuid(value, nameColumn, dataTypeMetadata);
                    break;
                case "XML":
                    result = Convesrion.ConversionXml(value, nameColumn, dataTypeMetadata);
                    break;
                case "JSON":
                    result = Convesrion.ConversionJson(value, nameColumn, dataTypeMetadata);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{dataTypeMetadata.SourceVarType.ToString()}'");
            }
            return result;
        }


       
        public static ColumnValue GetColumnValue(object value, string nameColumn)
        {
            ColumnValue result = null;
            switch (value.GetType().ToString())
            {
                case "System.String":
                    result = new StringColumnValue
                    {
                        Value = value.ToString() ?? null,
                        Name = nameColumn
                    };
                    break;
                case "System.Boolean":
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value.ToString(), out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?,
                            Name = nameColumn
                        };
                    }
                    else if (int.TryParse(value.ToString(), out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value.ToString() == "1" ? (bool?)true : (bool?)false,
                            Name = nameColumn
                        };
                    }
                    break;
                case "System.Integer":
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
                        Name = nameColumn
                    };
                    break;
                case "System.DateTime":
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        Name = nameColumn
                    };
                    break;
                case "System.Double":
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
                        Name = nameColumn
                    };
                    break;
                case "System.Float":
                case "System.Single":
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
                        Name = nameColumn
                    };
                    break;
                case "System.Decimal":
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
                        Name = nameColumn
                    };
                    break;
                case "System.Byte":
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
                        Name = nameColumn
                    };
                    break;
                case "System.Byte[]":
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : value as byte[],
                        Name = nameColumn
                    };
                    break;
                case "System.Guid":
                    result = new GuidColumnValue
                    {
                        Value = (value == null) ? (Guid?)null : (Guid.Parse(value.ToString()) as Guid?),
                        Name = nameColumn
                    };
                    break;
                   
                case "System.Char":
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
                        Name = nameColumn
                    };
                    break;
                case "System.Int16":
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
                        Name = nameColumn
                    };
                    break;
                case "System.UInt16":
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
                        Name = nameColumn
                    };
                    break;
                case "System.UInt32":
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
                        Name = nameColumn
                    };
                    break;
                case "System.Int64":
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
                        Name = nameColumn
                    };
                    break;
                case "System.UInt64":
                    result = new UnsignedLongColumnValue
                    {
                        Value = (value == null) ? (UInt64?)null : (UInt64.Parse(value.ToString()) as UInt64?),
                        Name = nameColumn
                    };
                    break;
                case "System.SByte":
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
                        Name = nameColumn
                    };
                    break;
                case "System.TimeSpan":
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
                        Name = nameColumn
                    };
                    break;
                case "System.DateTimeOffset":
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
                        Name = nameColumn
                    };
                    break;
                case "System.Object":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                case "System.Enum":
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
            }
            return result;
        }
       
        public static string GetMemberName<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            //if (ModelType != expression.Body.Type)
            //{
            //    throw new ArgumentException(Exceptions.ExpresionRefersToMemberThatNotFromType.With(expression.ToString(), ModelType));
            //}

            return expression.Body.GetMemberName();
        }

        

        
    }
}
