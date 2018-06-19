using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.DataModels;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class QueryExecutor : LoggedObject, IQueryExecutor
    {
        private sealed class DbFieldDescriptor
        {
            public DataSetColumn Column { get; set; }
            public Type DbType { get; set; }
            public int Ordinal { get; set; }
        }

        private readonly IDataEngine _dataEngine;
        private readonly IEngineSyntax _syntax;
        private readonly ConditionParser _conditionParser;
        private readonly IcsmOrmQueryBuilder _icsmOrmQueryBuilder;
        

        public QueryExecutor(IDataEngine dataEngine, IcsmOrmQueryBuilder icsmOrmQueryBuilder, ILogger logger) : base(logger)
        {
            this._dataEngine = dataEngine;
            this._syntax = dataEngine.Syntax;
            this._conditionParser = new ConditionParser(dataEngine.Syntax);
            this._icsmOrmQueryBuilder = icsmOrmQueryBuilder;

            logger.Debug(Contexts.LegacyServicesIcsm, Categories.CreatingInstance, Events.CreatedInstanceOfQueryExecutor);
        }

        public DataModels.DataSet Fetch(IQuerySelectStatement statement, DataSetColumn[] columns, DataSetStructure structure)
        {
            var dataSet =
                this.Fetch(statement, reader =>
                {
                    switch (structure)
                    {
                        case DataSetStructure.TypedCells:
                            return (DataModels.DataSet)ReadToTypedCellsDataSet(reader, columns);
                        case DataSetStructure.StringCells:
                            return (DataModels.DataSet)ReadToStringCellsDataSet(reader, columns);
                        case DataSetStructure.ObjectCells:
                            return (DataModels.DataSet)ReadToObjectCellsDataSet(reader, columns);
                        case DataSetStructure.TypedRows:
                            return (DataModels.DataSet)ReadToTypedRowsDataSet(reader, columns);
                        case DataSetStructure.StringRows:
                            return (DataModels.DataSet)ReadToStringRowsDataSet(reader, columns);
                        case DataSetStructure.ObjectRows:
                            return (DataModels.DataSet)ReadToObjectRowsDataSet(reader, columns);
                        default:
                            throw new InvalidOperationException(Exceptions.DataSetStructureNotSupported.With(structure));
                    }
                });

            return dataSet;
        }

        private TypedCellsDataSet ReadToTypedCellsDataSet(IDataReader dataReader, DataSetColumn[] columns)
        {
            var dataSet = new DataModels.TypedCellsDataSet
            {
                Columns = columns
            };

            var fields = new DbFieldDescriptor[columns.Length];
            int indexForString = -1;
            int indexForBoolean = -1;
            int indexForInteger = -1;
            int indexForDateTime = -1;
            int indexForDouble = -1;
            int indexForFloat = -1;
            int indexForDecimal = -1;
            int indexForByte = -1;
            int indexForBytes = -1;

            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                var dbField = new DbFieldDescriptor
                {
                    Column = column,
                    Ordinal = dataReader.GetOrdinal(column.Name)
                };
                dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
                fields[i] = dbField;

                switch (column.Type)
                {
                    case DataType.String:
                        column.Index = ++indexForString;
                        break;
                    case DataType.Boolean:
                        column.Index = ++indexForBoolean;
                        break;
                    case DataType.Integer:
                        column.Index = ++indexForInteger;
                        break;
                    case DataType.DateTime:
                        column.Index = ++indexForDateTime;
                        break;
                    case DataType.Double:
                        column.Index = ++indexForDouble;
                        break;
                    case DataType.Float:
                        column.Index = ++indexForFloat;
                        break;
                    case DataType.Decimal:
                        column.Index = ++indexForDecimal;
                        break;
                    case DataType.Byte:
                        column.Index = ++indexForByte;
                        break;
                    case DataType.Bytes:
                        column.Index = ++indexForBytes;
                        break;
                    default:
                        column.Index = -1;
                        break;
                }
            }

            List<string>[] columnsAsString = null;
            if (indexForString >= 0)
            {
                columnsAsString = new List<string>[indexForString + 1].Select(o => new List<string>()).ToArray();
            }
            List<bool?>[] columnsAsBoolean = null;
            if (indexForBoolean >= 0)
            {
                columnsAsBoolean = new List<bool?>[indexForBoolean + 1].Select(o => new List<bool?>()).ToArray();
            }
            List<int?>[] columnsAsInteger = null;
            if (indexForInteger >= 0)
            {
                columnsAsInteger = new List<int?>[indexForInteger + 1].Select(o => new List<int?>()).ToArray();
            }
            List<DateTime?>[] columnsAsDateTime = null;
            if (indexForDateTime >= 0)
            {
                columnsAsDateTime = new List<DateTime?>[indexForDateTime + 1].Select(o => new List<DateTime?>()).ToArray();
            }
            List<double?>[] columnsAsDouble = null;
            if (indexForDouble >= 0)
            {
                columnsAsDouble = new List<double?>[indexForDouble + 1].Select(o => new List<double?>()).ToArray();
            }
            List<float?>[] columnsAsFloat = null;
            if (indexForFloat >= 0)
            {
                columnsAsFloat = new List<float?>[indexForFloat + 1].Select(o => new List<float?>()).ToArray();
            }
            List<decimal?>[] columnsAsDecimal = null;
            if (indexForDecimal >= 0)
            {
                columnsAsDecimal = new List<decimal?>[indexForDecimal + 1].Select(o => new List<decimal?>()).ToArray();
            }
            List<byte?>[] columnsAsByte = null;
            if (indexForByte >= 0)
            {
                columnsAsByte = new List<byte?>[indexForByte + 1].Select(o => new List<byte?>()).ToArray();
            }
            List<byte[]>[] columnsAsBytes = null;
            if (indexForBytes >= 0)
            {
                columnsAsBytes = new List<byte[]>[indexForBytes + 1].Select(o => new List<byte[]>()).ToArray();
            }
            int rowCount = 0;
            while (dataReader.Read())
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    switch (field.Column.Type)
                    {
                        case DataType.String:
                            var valueAsString = dataReader.GetNullableValueAsString(field.DbType, field.Ordinal);
                            columnsAsString[field.Column.Index].Add(valueAsString);
                            break;
                        case DataType.Boolean:
                            var valueAsBoolean = dataReader.GetNullableValueAsBoolean(field.DbType, field.Ordinal);
                            columnsAsBoolean[field.Column.Index].Add(valueAsBoolean);
                            break;
                        case DataType.Integer:
                            var valueAsInteger = dataReader.GetNullableValueAsInt32(field.DbType, field.Ordinal);
                            columnsAsInteger[field.Column.Index].Add(valueAsInteger);
                            break;
                        case DataType.DateTime:
                            var valueAsDateTime = dataReader.GetNullableValueAsDateTime(field.DbType, field.Ordinal);
                            columnsAsDateTime[field.Column.Index].Add(valueAsDateTime);
                            break;
                        case DataType.Double:
                            var valueAsDouble = dataReader.GetNullableValueAsDouble(field.DbType, field.Ordinal);
                            columnsAsDouble[field.Column.Index].Add(valueAsDouble);
                            break;
                        case DataType.Float:
                            var valueAsFloat = dataReader.GetNullableValueAsFloat(field.DbType, field.Ordinal);
                            columnsAsFloat[field.Column.Index].Add(valueAsFloat);
                            break;
                        case DataType.Decimal:
                            var valueAsDecimal = dataReader.GetNullableValueAsDecimal(field.DbType, field.Ordinal);
                            columnsAsDecimal[field.Column.Index].Add(valueAsDecimal);
                            break;
                        case DataType.Byte:
                            var valueAsByte = dataReader.GetNullableValueAsByte(field.DbType, field.Ordinal);
                            columnsAsByte[field.Column.Index].Add(valueAsByte);
                            break;
                        case DataType.Bytes:
                            var valueAsBytes = dataReader.GetNullableValueAsBytes(field.DbType, field.Ordinal);
                            columnsAsBytes[field.Column.Index].Add(valueAsBytes);
                            break;
                        default:
                            break;
                    }
                }

                ++rowCount;
            }

            if (indexForString >= 0)
            {
                dataSet.StringCells = columnsAsString.Select(c => c.ToArray()).ToArray();
            }
            if (indexForBoolean >= 0)
            {
                dataSet.BooleanCells = columnsAsBoolean.Select(c => c.ToArray()).ToArray();
            }
            if (indexForInteger >= 0)
            {
                dataSet.IntegerCells = columnsAsInteger.Select(c => c.ToArray()).ToArray();
            }
            if (indexForDateTime >= 0)
            {
                dataSet.DateTimeCells = columnsAsDateTime.Select(c => c.ToArray()).ToArray();
            }
            if (indexForDouble >= 0)
            {
                dataSet.DoubleCells = columnsAsDouble.Select(c => c.ToArray()).ToArray();
            }
            if (indexForFloat >= 0)
            {
                dataSet.FloatCells = columnsAsFloat.Select(c => c.ToArray()).ToArray();
            }
            if (indexForDecimal >= 0)
            {
                dataSet.DecimalCells = columnsAsDecimal.Select(c => c.ToArray()).ToArray();
            }
            if (indexForByte >= 0)
            {
                dataSet.ByteCells = columnsAsByte.Select(c => c.ToArray()).ToArray();
            }
            if (indexForBytes >= 0)
            {
                dataSet.BytesCells = columnsAsBytes.Select(c => c.ToArray()).ToArray();
            }


            dataSet.RowCount = rowCount;
            return dataSet;
        }

        private StringCellsDataSet ReadToStringCellsDataSet(IDataReader dataReader, DataSetColumn[] columns)
        {
            var dataSet = new DataModels.StringCellsDataSet
            {
                Columns = columns
            };

            var fields = new DbFieldDescriptor[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                var dbField = new DbFieldDescriptor
                {
                    Column = column,
                    Ordinal = dataReader.GetOrdinal(column.Name)
                };
                dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
                fields[i] = dbField;
                column.Index = i;
            }

            var rows = new List<string[]>();
            while (dataReader.Read())
            {
                var cells = new string[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    cells[i] = dataReader.GetValueAsString(field.Column.Type, field.DbType, field.Ordinal);
                }
                rows.Add(cells);
            }
            dataSet.Cells = rows.ToArray();
            dataSet.RowCount = dataSet.Cells.Length;
            return dataSet;
        }

        private ObjectCellsDataSet ReadToObjectCellsDataSet(IDataReader dataReader, DataSetColumn[] columns)
        {
            var dataSet = new DataModels.ObjectCellsDataSet
            {
                Columns = columns
            };

            var fields = new DbFieldDescriptor[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                var dbField = new DbFieldDescriptor
                {
                    Column = column,
                    Ordinal = dataReader.GetOrdinal(column.Name)
                };
                dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
                fields[i] = dbField;
                column.Index = i;
            }

            var rows = new List<object[]>();
            while (dataReader.Read())
            {
                var cells = new object[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    cells[i] = dataReader.GetValueAsObject(field.Column.Type, field.DbType, field.Ordinal);
                }
                rows.Add(cells);
            }
            dataSet.Cells = rows.ToArray();
            dataSet.RowCount = dataSet.Cells.Length;
            return dataSet;
        }

        private TypedRowsDataSet ReadToTypedRowsDataSet(IDataReader dataReader, DataSetColumn[] columns)
        {
            var dataSet = new DataModels.TypedRowsDataSet
            {
                Columns = columns
            };

            var fields = new DbFieldDescriptor[columns.Length];
            int indexForString = -1;
            int indexForBoolean = -1;
            int indexForInteger = -1;
            int indexForDateTime = -1;
            int indexForDouble = -1;
            int indexForFloat = -1;
            int indexForDecimal = -1;
            int indexForByte = -1;
            int indexForBytes = -1;

            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                var dbField = new DbFieldDescriptor
                {
                    Column = column,
                    Ordinal = dataReader.GetOrdinal(column.Name)
                };
                dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
                fields[i] = dbField;

                switch (column.Type)
                {
                    case DataType.String:
                        column.Index = ++indexForString;
                        break;
                    case DataType.Boolean:
                        column.Index = ++indexForBoolean;
                        break;
                    case DataType.Integer:
                        column.Index = ++indexForInteger;
                        break;
                    case DataType.DateTime:
                        column.Index = ++indexForDateTime;
                        break;
                    case DataType.Double:
                        column.Index = ++indexForDouble;
                        break;
                    case DataType.Float:
                        column.Index = ++indexForFloat;
                        break;
                    case DataType.Decimal:
                        column.Index = ++indexForDecimal;
                        break;
                    case DataType.Byte:
                        column.Index = ++indexForByte;
                        break;
                    case DataType.Bytes:
                        column.Index = ++indexForBytes;
                        break;
                    default:
                        column.Index = -1;
                        break;
                }
            }

            var rows = new List<TypedDataRow>();
            while (dataReader.Read())
            {
                var row = new TypedDataRow();
                if (indexForString >= 0)
                {
                    row.StringCells = new string[indexForString + 1];
                }
                if (indexForBoolean >= 0)
                {
                    row.BooleanCells = new bool?[indexForBoolean + 1];
                }
                if (indexForInteger >= 0)
                {
                    row.IntegerCells = new int?[indexForInteger + 1];
                }
                if (indexForDateTime >= 0)
                {
                    row.DateTimeCells = new DateTime?[indexForDateTime + 1];
                }
                if (indexForDouble >= 0)
                {
                    row.DoubleCells = new double?[indexForDouble + 1];
                }
                if (indexForFloat >= 0)
                {
                    row.FloatCells = new float?[indexForFloat + 1];
                }
                if (indexForDecimal >= 0)
                {
                    row.DecimalCells = new decimal?[indexForDecimal + 1];
                }
                if (indexForByte >= 0)
                {
                    row.ByteCells = new byte?[indexForByte + 1];
                }
                if (indexForBytes >= 0)
                {
                    row.BytesCells = new byte[indexForBytes + 1][];
                }
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    switch (field.Column.Type)
                    {
                        case DataType.String:
                            if (!dataReader.IsDBNull(field.Ordinal))
                                row.StringCells[field.Column.Index] = dataReader.GetValueAsString(field.DbType, field.Ordinal);
                            break;
                        case DataType.Boolean:
                            if(!dataReader.IsDBNull(field.Ordinal))
                                row.BooleanCells[field.Column.Index] = dataReader.GetValueAsBoolean(field.DbType, field.Ordinal);
                            break;
                        case DataType.Integer:
                            if(!dataReader.IsDBNull(field.Ordinal))
                                row.IntegerCells[field.Column.Index] = dataReader.GetValueAsInt32(field.DbType, field.Ordinal);
                            break;
                        case DataType.DateTime:
                            if(!dataReader.IsDBNull(field.Ordinal))
                                row.DateTimeCells[field.Column.Index] = dataReader.GetValueAsDateTime(field.DbType, field.Ordinal);
                            break;
                        case DataType.Double:
                            if (!dataReader.IsDBNull(field.Ordinal))
                                row.DoubleCells[field.Column.Index] = dataReader.GetValueAsDouble(field.DbType, field.Ordinal);
                            break;
                        case DataType.Float:
                            if (!dataReader.IsDBNull(field.Ordinal))
                                row.FloatCells[field.Column.Index] = dataReader.GetValueAsFloat(field.DbType, field.Ordinal);
                            break;
                        case DataType.Decimal:
                            if (!dataReader.IsDBNull(field.Ordinal))
                                row.DecimalCells[field.Column.Index] = dataReader.GetValueAsDecimal(field.DbType, field.Ordinal);
                            break;
                        case DataType.Byte:
                            if (!dataReader.IsDBNull(field.Ordinal))
                                row.ByteCells [field.Column.Index] = dataReader.GetValueAsByte(field.DbType, field.Ordinal);
                            break;
                        case DataType.Bytes:
                            if (!dataReader.IsDBNull(field.Ordinal))
                                row.BytesCells[field.Column.Index] = dataReader.GetValueAsBytes(field.DbType, field.Ordinal);
                            break;
                        default:
                            break;
                    }
                }
                rows.Add(row);
            }

            dataSet.Rows = rows.ToArray();
            dataSet.RowCount = dataSet.Rows.Length;

            return dataSet;
        }

        private StringRowsDataSet ReadToStringRowsDataSet(IDataReader dataReader, DataSetColumn[] columns)
        {
            var dataSet = new DataModels.StringRowsDataSet
            {
                Columns = columns
            };

            var fields = new DbFieldDescriptor[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                var dbField = new DbFieldDescriptor
                {
                    Column = column,
                    Ordinal = dataReader.GetOrdinal(column.Name)
                };
                dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
                fields[i] = dbField;
                column.Index = i;
            }

            var rows = new List<StringDataRow>();
            while (dataReader.Read())
            {
                var row = new StringDataRow();
                var cells = new string[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    cells[i] = dataReader.GetValueAsString(field.Column.Type, field.DbType, field.Ordinal);
                }
                row.Cells = cells;
                rows.Add(row);
            }
            dataSet.Rows = rows.ToArray();
            dataSet.RowCount = dataSet.Rows.Length;
            return dataSet;
        }

        private ObjectRowsDataSet ReadToObjectRowsDataSet(IDataReader dataReader, DataSetColumn[] columns)
        {
            var dataSet = new DataModels.ObjectRowsDataSet
            {
                Columns = columns
            };

            var fields = new DbFieldDescriptor[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                var dbField = new DbFieldDescriptor
                {
                    Column = column,
                    Ordinal = dataReader.GetOrdinal(column.Name)
                };
                dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
                fields[i] = dbField;
                column.Index = i;
            }

            var rows = new List<ObjectDataRow>();
            while(dataReader.Read())
            {
                var row = new ObjectDataRow();
                var cells = new object[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    cells[i] = dataReader.GetValueAsObject(field.Column.Type, field.DbType, field.Ordinal);
                }
                row.Cells = cells;
                rows.Add(row);
            }
            dataSet.Rows = rows.ToArray();
            dataSet.RowCount = dataSet.Rows.Length;
            return dataSet;
        }

        public TResult Fetch<TResult>(IQuerySelectStatement statement, Func<IDataReader, TResult> handler)
        {
            try
            {
                var command = this.BuildSelectCommand(statement as QuerySelectStatement);

                var result = default(TResult);
                _dataEngine.Execute(command, reader =>
                {
                    result = handler(reader);
                });
                return result;
            }
            catch(Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.FetchingData, e);
                throw;
            }
        }

        public TResult Fetch<TModel, TResult>(IQuerySelectStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler)
        {
            try
            {
                var objectStatment = statement as QuerySelectStatement<TModel>;
                var command = this.BuildSelectCommand(objectStatment.Statement);

                var result = default(TResult);
                _dataEngine.Execute(command, reader =>
                {
                    var typedReader = new QueryDataReader<TModel>(reader);
                    result = handler(typedReader);
                });
                return result;
            }
            catch(Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.FetchingData, e);
                throw;
            }
        }

        

        private EngineCommand BuildSelectCommand(QuerySelectStatement statement)
        {
            var command = new EngineCommand();

            var rootStatement = this._icsmOrmQueryBuilder.BuildSelectStatement(statement);

            var fromExpression = this._dataEngine.Syntax.FromExpression(rootStatement, "A");

            var selectColumns = new string[statement.Table.SelectColumns.Count];
            var index = 0;
            foreach (var column in statement.Table.SelectColumns.Values)
            {
                /// todo alias
                selectColumns[index++] = this._syntax.ColumnExpression(this._syntax.EncodeFieldName("A", column.Name), column.Name);
            }

            // add conditions
            var whereExpression = this.CreateWhereExpression(statement.Conditions, command.Parameters);

            // add order by
            string[] orderByColumns = null;
            var sortColumns = statement.Orders;
            if (sortColumns != null && sortColumns.Count > 0)
            {
                orderByColumns = new string[sortColumns.Count];
                index = 0;
                foreach (var sortColumn in sortColumns)
                {
                    var encodeColumn = this._syntax.EncodeFieldName("A", sortColumn.Column.Alias);
                    orderByColumns[index++] = _syntax.SortedColumn(encodeColumn, sortColumn.Direction);
                }
            }

            // add on top (n)
            var limit = statement.Limit;
            
            // add group by

            command.Text = this._syntax.SelectExpression(selectColumns, fromExpression, whereExpression, orderByColumns, limit);
            return command;
        }

        private string CreateWhereExpression(List<Condition> conditions, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return null;
            }

            var expressions = conditions.Select(condition => this._conditionParser.Parse(condition, parameters)).ToArray();

            var result = this._syntax.Constraint.JoinExpressions(LogicalOperator.And, expressions);

            return result;
        }
    }
}

