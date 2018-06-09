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

    internal sealed class QuerySelectStatement<TModel> : LoggedObject, IQuerySelectStatement<TModel>
    {
        private static readonly Type ModelType = typeof(TModel);
        private readonly QuerySelectStatement _statement;


        public QuerySelectStatement Statement => _statement;

        public QuerySelectStatement(ILogger logger) : base(logger)
        {
            this._statement = new QuerySelectStatement(ModelType.Name, logger); 
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
            if (type == typeof(byte[]) || type == typeof(byte?[]))
            {
                return new BytesValuesOperand
                {
                    Values = values.Select(o => (byte?[])(object)o).ToArray()
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
            if (type == typeof(byte[]) || type == typeof(byte?[]))
            {
                return new BytesValueOperand
                {
                    Value = (byte?[])(object)value
                };
            }

            throw new InvalidOperationException(Exceptions.ValueTypeNotSupported.With(type));
        }

        private static Condition ParseCondition<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
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

        private static Condition ParseConditionExpression(Expression<Func<TModel, bool>> expression)
        {
            var body = expression.Body;
            return ParseExpression(body);
        }

        private static Condition ParseConditionExpression(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
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
                if (constantExpression.Type == typeof(byte[]) || constantExpression.Type == typeof(byte?[]))
                {
                    return new BytesValueOperand
                    {
                        Value = (byte?[])constantExpression.Value
                    };
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
                if (constantExpression.Type == typeof(byte[]) || constantExpression.Type == typeof(byte?[]))
                {
                    return new BytesValuesOperand
                    {
                        Values = constantExpressions.Select(o => (byte?[])o.Value).ToArray()
                    };
                }
                throw new InvalidOperationException(Exceptions.ExpressionTypeNotSupported.With(constantExpression.Type));
            }

            throw new InvalidOperationException(Exceptions.ExpressionNodeTypeNotSupported.With(expression.NodeType.ToString()));
        }

        private static string GetMemberName<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            //if (ModelType != expression.Body.Type)
            //{
            //    throw new ArgumentException(Exceptions.ExpresionRefersToMemberThatNotFromType.With(expression.ToString(), ModelType));
            //}

            return expression.Body.GetMemberName();
        }

        

        
    }
}
