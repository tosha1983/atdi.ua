using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IQueryUpdateStatement : IQueryStatement
    {
        IQueryUpdateStatement Where(Condition condition);

        IQueryUpdateStatement SetValue(ColumnValue columnValue);

        IQueryUpdateStatement SetValues(ColumnValue[] columnsValues);

    }

    public interface IQueryUpdateStatement<TModel> : IQueryStatement
    {
        IQueryUpdateStatement<TModel> Where(Expression<Func<TModel, bool>> expression);

        IQueryUpdateStatement<TModel> Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression);

        IQueryUpdateStatement<TModel> Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values);

        IQueryUpdateStatement<TModel> SetValue<TValue>(Expression<Func<TModel, TValue>> columnsExpression, TValue value);
    }

    public static class QueryUpdateStatementExtensitons
    {
        public static IQueryUpdateStatement SetValue(this IQueryUpdateStatement statement, string column, ValueOperand value)
        {
            ColumnValue result = null;
            switch (value.DataType)
            {
                case DataType.String:
                    result = new StringColumnValue
                    {
                        Value = ((StringValueOperand)value).Value
                    };
                    break;
                case DataType.Boolean:
                    result = new BooleanColumnValue
                    {
                        Value = ((BooleanValueOperand)value).Value
                    };
                    break;
                case DataType.Integer:
                    result = new IntegerColumnValue
                    {
                        Value = ((IntegerValueOperand)value).Value
                    };
                    break;
                case DataType.Long:
	                result = new LongColumnValue
	                {
		                Value = ((LongValueOperand)value).Value
	                };
	                break;
				case DataType.DateTime:
                    result = new DateTimeColumnValue
                    {
                        Value = ((DateTimeValueOperand)value).Value
                    };
                    break;
                case DataType.DateTimeOffset:
	                result = new DateTimeOffsetColumnValue
					{
		                Value = ((DateTimeOffsetValueOperand)value).Value
	                };
	                break;
				case DataType.Double:
                    result = new DoubleColumnValue
                    {
                        Value = ((DoubleValueOperand)value).Value
                    };
                    break;
                case DataType.Float:
                    result = new FloatColumnValue
                    {
                        Value = ((FloatValueOperand)value).Value
                    };
                    break;
                case DataType.Decimal:
                    result = new DecimalColumnValue
                    {
                        Value = ((DecimalValueOperand)value).Value
                    };
                    break;
                case DataType.Byte:
                    result = new ByteColumnValue
                    {
                        Value = ((ByteValueOperand)value).Value
                    };
                    break;
                case DataType.Bytes:
                    result = new BytesColumnValue
                    {
                        Value = ((BytesValueOperand)value).Value
                    };
                    break;
                case DataType.Guid:
                    result = new GuidColumnValue
                    {
                        Value = ((GuidValueOperand)value).Value
                    };
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{value.DataType}'");
            }
            result.Name = column;
            return statement.SetValue(result);
        }

        public static IQueryUpdateStatement SetValues(this IQueryUpdateStatement statement, DataSetColumn[] columns, TypedDataRow row)
        {
            var results = new ColumnValue[columns.Length];

            for (int i = 0; i < columns.Length; i++)
            {
                results[i] = row.GetColumnValue(columns[i]);
            }
            return statement.SetValues(results);
        }

        public static IQueryUpdateStatement Where(this IQueryUpdateStatement query, Condition[] conditions)
        {
            if (conditions == null)
            {
                throw new ArgumentNullException(nameof(conditions));
            }
            for (int i = 0; i < conditions.Length; i++)
            {
                query.Where(conditions[i]);
            }
            return query;
        }
    }
}
