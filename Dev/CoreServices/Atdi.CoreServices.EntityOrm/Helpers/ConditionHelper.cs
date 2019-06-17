using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal static class ConditionHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOperatorMultiValues(ConditionOperator conditionOperator)
        {
            return
                conditionOperator == ConditionOperator.In ||
                conditionOperator == ConditionOperator.NotIn ||
                conditionOperator == ConditionOperator.Between ||
                conditionOperator == ConditionOperator.NotBetween;
        }

        public static Condition ParseCondition<TModel, TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
        {
            var condition = new ConditionExpression
            {
                LeftOperand = new ColumnOperand { ColumnName = columnExpression.Body.GetMemberName() },
                Operator = conditionOperator
            };

            if (IsOperatorMultiValues(conditionOperator))
            {
                if (values == null || values.Length == 0)
                {
                    throw new ArgumentException(nameof(values));
                }

                if ((conditionOperator == ConditionOperator.Between || conditionOperator == ConditionOperator.NotBetween) && values.Length != 2)
                {
                    throw new ArgumentException(nameof(values));
                }

                condition.RightOperand = ValuesOperand.Create<TValue>(values);
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
                condition.RightOperand = ValueOperand.Create<TValue>(value);
            }

            return condition;
        }

        public static Condition ParseConditionExpression<TModel>(Expression<Func<TModel, bool>> expression)
        {
            var body = expression.Body;
            return ParseExpression(body);
        }

        public static Condition ParseConditionExpression<TModel>(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
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
            if (type == ExpressionType.Equal)
            {
                return ConditionOperator.Equal;
            }
            if (type == ExpressionType.NotEqual)
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
                if (constantExpression.Type == typeof(byte[]))
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
                if (constantExpression.Type == typeof(byte[]))
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
    }
}
