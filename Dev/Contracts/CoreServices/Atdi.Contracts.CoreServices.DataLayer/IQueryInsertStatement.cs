﻿using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IQueryInsertStatement : IQueryStatement
    {
        IQueryInsertStatement SetValue(ColumnValue columnValue);

        IQueryInsertStatement SetValues(ColumnValue[] columnsValues);
    }

    public static class QueryInsertStatementExtensitons
    {
        public static IQueryInsertStatement SetValue(this IQueryInsertStatement statement, string column, ValueOperand value)
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
                case DataType.DateTime:
                    result = new DateTimeColumnValue
                    {
                        Value = ((DateTimeValueOperand)value).Value
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

        public static IQueryInsertStatement SetValues(this IQueryInsertStatement statement, DataSetColumn[] columns, TypedDataRow row)
        {
            var results = new ColumnValue[columns.Length];

            for (int i = 0; i < columns.Length; i++)
            {
                results[i] = row.GetColumnValue(columns[i]);
            }
            return statement.SetValues(results);
        }
    }
}