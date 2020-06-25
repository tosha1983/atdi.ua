using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Contracts.CoreServices.EntityOrm;
using Newtonsoft.Json;

namespace Atdi.CoreServices.EntityOrm
{
    internal class QueryUpdateStatement : IQueryUpdateStatement
    {
        private readonly UpdateQueryDescriptor _queryDescriptor;

        public QueryUpdateStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._queryDescriptor = new UpdateQueryDescriptor(entityOrm, entityMetadata);
        }

        public UpdateQueryDescriptor UpdateDecriptor => this._queryDescriptor;

        public IQueryUpdateStatement Where(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            _queryDescriptor.AppendCondition(condition);
            return this;
        }

        public IQueryUpdateStatement SetValue(ColumnValue columnValue)
        {
            if (columnValue == null)
            {
                throw new ArgumentNullException(nameof(columnValue));
            }

            this._queryDescriptor.AppendValue(columnValue);
            return this;
        }

        public IQueryUpdateStatement SetValues(ColumnValue[] columnsValues)
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

    internal sealed class QueryUpdateStatement<TModel> : QueryUpdateStatement, IQueryUpdateStatement<TModel>
    {
        public QueryUpdateStatement(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
            : base(entityOrm, entityMetadata)
        {
        }

        IQueryUpdateStatement<TModel> IQueryUpdateStatement<TModel>.SetValue<TValue>(Expression<Func<TModel, TValue>> columnsExpression, TValue value)
        {
            var memberName = columnsExpression.Body.GetMemberName();
            var columnValue = ColumnValue.Create(value, memberName);
            this.SetValue(columnValue);
            return this;
        }

        public IQueryUpdateStatement<TModel> SetValueAsJson<TValue>(Expression<Func<TModel, string>> columnsExpression, TValue value)
        {
			var memberName = columnsExpression.Body.GetMemberName();
			var json = JsonConvert.SerializeObject(value);
			var columnValue = ColumnValue.Create<string>(json, memberName);
			this.SetValue(columnValue);
			return this;
		}

        IQueryUpdateStatement<TModel> IQueryUpdateStatement<TModel>.Where(Expression<Func<TModel, bool>> expression)
        {
            this.Where(ConditionHelper.ParseConditionExpression(expression));
            return this;
        }

        IQueryUpdateStatement<TModel> IQueryUpdateStatement<TModel>.Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
        {
            this.Where(ConditionHelper.ParseConditionExpression(expression));
            return this;
        }

        IQueryUpdateStatement<TModel> IQueryUpdateStatement<TModel>.Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
        {
            this.Where(ConditionHelper.ParseCondition(columnExpression, conditionOperator, values));
            return this;
        }

        //private static readonly IFormatProvider CultureEnUs = new System.Globalization.CultureInfo("en-US");

        //public static ColumnValue GetColumnValue(object value, string nameColumn, DataTypeMetadata dataTypeMetadata)
        //{
        //    ColumnValue result = null;
        //    switch (dataTypeMetadata.SourceVarType.ToString())
        //    {
        //        case "BOOL":
        //            result = Convesrion.ConversionBool(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "BIT":
        //            result = Convesrion.ConversionBit(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "BYTE":
        //            result = Convesrion.ConversionByte(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "BYTES":
        //            result = Convesrion.ConversionBytes(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "BLOB":
        //            result = Convesrion.ConversionBlob(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "INT08":
        //            result = Convesrion.ConversionInt08(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "INT16":
        //            result = Convesrion.ConversionInt16(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "INT32":
        //            result = Convesrion.ConversionInt32(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "INT64":
        //            result = Convesrion.ConversionInt64(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "NCHAR":
        //            result = Convesrion.ConversionNchar(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "NVARCHAR":
        //            result = Convesrion.ConversionNvarChar(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "NTEXT":
        //            result = Convesrion.ConversionNText(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "CHAR":
        //            result = Convesrion.ConversionChar(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "VARCHAR":
        //            result = Convesrion.ConversionVarChar(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "TEXT":
        //            result = Convesrion.ConversionText(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "TIME":
        //            result = Convesrion.ConversionTime(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "DATE":
        //            result = Convesrion.ConversionDate(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "DATETIME":
        //            result = Convesrion.ConversionDateTime(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "DATETIMEOFFSET":
        //            result = Convesrion.ConversionDateTimeOffset(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "MONEY":
        //            result = Convesrion.ConversionMoney(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "FLOAT":
        //            result = Convesrion.ConversionFloat(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "DOUBLE":
        //            result = Convesrion.ConversionDouble(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "DECIMAL":
        //            result = Convesrion.ConversionDecimal(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "GUID":
        //            result = Convesrion.ConversionGuid(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "XML":
        //            result = Convesrion.ConversionXml(value, nameColumn, dataTypeMetadata);
        //            break;
        //        case "JSON":
        //            result = Convesrion.ConversionJson(value, nameColumn, dataTypeMetadata);
        //            break;
        //        default:
        //            throw new InvalidOperationException($"Unsupported data type with name '{dataTypeMetadata.SourceVarType.ToString()}'");
        //    }
        //    return result;
        //}

        //public static ColumnValue GetColumnValue(object value, string nameColumn)
        //{
        //    ColumnValue result = null;
        //    if (value == null)
        //    {
        //        value = System.DBNull.Value;
        //    }
        //    switch (value.GetType().ToString())
        //    {
        //        case "System.DBNull":
        //        case "System.String":
        //            result = new StringColumnValue
        //            {
        //                Value = (value.GetType() != typeof(System.DBNull)) ? (value.ToString() ?? null) : null,
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.Boolean":
        //            bool valueBool = false; int valueInt = 0;
        //            if (bool.TryParse(value.ToString(), out valueBool))
        //            {
        //                result = new BooleanColumnValue
        //                {
        //                    Value = (value == null) ? (bool?)null : valueBool as bool?,
        //                    Name = nameColumn
        //                };
        //            }
        //            else if (int.TryParse(value.ToString(), out valueInt))
        //            {
        //                result = new BooleanColumnValue
        //                {
        //                    Value = value.ToString() == "1" ? (bool?)true : (bool?)false,
        //                    Name = nameColumn
        //                };
        //            }
        //            break;
        //        case "System.Integer":
        //        case "System.Int32":
        //            result = new IntegerColumnValue
        //            {
        //                Value = (value == null) ? (int?)null : (int.Parse(value.ToString(), CultureEnUs) as int?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.DateTime":
        //            result = new DateTimeColumnValue
        //            {
        //                Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.Double":
        //            result = new DoubleColumnValue
        //            {
        //                Value = (value == null) ? (double?)null : (double.Parse(value.ToString(), CultureEnUs) as double?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.Float":
        //        case "System.Single":
        //            result = new FloatColumnValue
        //            {
        //                Value = (value == null) ? (float?)null : (float.Parse(value.ToString(), CultureEnUs) as float?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.Decimal":
        //            result = new DecimalColumnValue
        //            {
        //                Value = (value == null) ? (decimal?)null : (decimal.Parse(value.ToString(), CultureEnUs) as decimal?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.Byte":
        //            result = new ByteColumnValue
        //            {
        //                Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?,
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.Byte[]":
        //            result = new BytesColumnValue
        //            {
        //                Value = (value == null) ? (byte[])null : value as byte[],
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.Guid":
        //            result = new GuidColumnValue
        //            {
        //                Value = (value == null) ? (Guid?)null : (Guid.Parse(value.ToString()) as Guid?),
        //                Name = nameColumn
        //            };
        //            break;

        //        case "System.Char":
        //            result = new CharColumnValue
        //            {
        //                Value = (value == null) ? (char?)null : (char.Parse(value.ToString()) as char?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.Int16":
        //            result = new ShortColumnValue
        //            {
        //                Value = (value == null) ? (Int16?)null : (Int16.Parse(value.ToString()) as Int16?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.UInt16":
        //            result = new UnsignedShortColumnValue
        //            {
        //                Value = (value == null) ? (UInt16?)null : (UInt16.Parse(value.ToString()) as UInt16?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.UInt32":
        //            result = new UnsignedIntegerColumnValue
        //            {
        //                Value = (value == null) ? (UInt32?)null : (UInt32.Parse(value.ToString()) as UInt32?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.Int64":
        //            result = new LongColumnValue
        //            {
        //                Value = (value == null) ? (Int64?)null : (Int64.Parse(value.ToString()) as Int64?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.UInt64":
        //            result = new UnsignedLongColumnValue
        //            {
        //                Value = (value == null) ? (UInt64?)null : (UInt64.Parse(value.ToString()) as UInt64?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.SByte":
        //            result = new SignedByteColumnValue
        //            {
        //                Value = (value == null) ? (sbyte?)null : (System.SByte.Parse(value.ToString()) as sbyte?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.TimeSpan":
        //            result = new TimeColumnValue
        //            {
        //                Value = (value == null) ? (TimeSpan?)null : (System.TimeSpan.Parse(value.ToString()) as TimeSpan?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.DateTimeOffset":
        //            result = new DateTimeOffsetColumnValue
        //            {
        //                Value = (value == null) ? (DateTimeOffset?)null : (System.DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?),
        //                Name = nameColumn
        //            };
        //            break;
        //        case "System.Object":
        //            throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
        //        case "System.Enum":
        //            throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
        //        default:
        //            throw new InvalidOperationException($"Unsupported data type with name '{value.GetType().ToString()}'");
        //    }
        //    return result;
        //}
    }


    
}
