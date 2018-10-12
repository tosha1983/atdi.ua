using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryExecutor: LoggedObject, IQueryExecutor
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
        private readonly EntityOrmQueryBuilder _icsmOrmQueryBuilder;
        

        public QueryExecutor(IDataEngine dataEngine, EntityOrmQueryBuilder icsmOrmQueryBuilder, ILogger logger) : base(logger) 
        {
            this._dataEngine = dataEngine;
            //this._syntax = dataEngine.Syntax;
            //this._conditionParser = new ConditionParser(dataEngine.Syntax);
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

        private TypedCellsDataSet ReadToTypedCellsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
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
            int indexForGuid = -1;

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
                    case DataType.Guid:
                        column.Index = ++indexForGuid;
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
            List<Guid?>[] columnsAsGuid = null;
            if (indexForGuid >= 0)
            {
                columnsAsGuid = new List<byte?>[indexForGuid + 1].Select(o => new List<Guid?>()).ToArray();
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
                        case DataType.Guid:
                            var valueAsGuid = dataReader.GetNullableValueAsGuid(field.DbType, field.Ordinal);
                            columnsAsGuid[field.Column.Index].Add(valueAsGuid);
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
            if (indexForGuid >= 0)
            {
                dataSet.GuidCells = columnsAsGuid.Select(c => c.ToArray()).ToArray();
            }

            dataSet.RowCount = rowCount;
            return dataSet;
        }

        private StringCellsDataSet ReadToStringCellsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
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

        private ObjectCellsDataSet ReadToObjectCellsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
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

        private TypedRowsDataSet ReadToTypedRowsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
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
            int indexForGuid = -1;

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
                    case DataType.Guid:
                        column.Index = ++indexForGuid;
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
                if (indexForGuid >= 0)
                {
                    row.GuidCells = new Guid?[indexForGuid + 1];
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
                        case DataType.Guid:
                            if (!dataReader.IsDBNull(field.Ordinal))
                                row.GuidCells[field.Column.Index] = dataReader.GetValueAsGuid(field.DbType, field.Ordinal);
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

        private StringRowsDataSet ReadToStringRowsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
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

        private ObjectRowsDataSet ReadToObjectRowsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
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

        public TResult Fetch<TResult>(IQuerySelectStatement statement, Func<Atdi.Contracts.CoreServices.DataLayer.IDataReader, TResult> handler)
        {
            try
            {
                var objectStatment = statement as QuerySelectStatement;
                var command = this.BuildSelectCommand(objectStatment);

                var result = default(TResult);
                _dataEngine.Execute(command, reader =>
                {
                    var columnsMapper = objectStatment.Table.SelectColumns.ToDictionary(k => k.Value.Name, e => e.Value.Alias);
                    var typedReader = new QueryDataReader(reader, columnsMapper);
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

        public TResult Fetch<TModel, TResult>(IQuerySelectStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler)
        {
            try
            {
                var objectStatment = statement as QuerySelectStatement<TModel>;
                var command = this.BuildSelectCommand(objectStatment.Statement);

                var result = default(TResult);
                _dataEngine.Execute(command, reader =>
                {
                    var columnsMapper = objectStatment.Statement.Table.SelectColumns.ToDictionary(k => k.Value.Name, e => e.Value.Alias);
                    var typedReader = new QueryDataReader<TModel>(reader, columnsMapper);
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


        public int Execute(IQueryStatement statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            var command = new EngineCommand();
            if (statement is QueryInsertStatement queryInsertStatement)
            {
                command.Text = this._icsmOrmQueryBuilder.BuildInsertStatement(queryInsertStatement, command.Parameters);
            }
            else if (statement is QueryUpdateStatement queryUpdateStatement)
            {
                command.Text = this._icsmOrmQueryBuilder.BuildUpdateStatement(queryUpdateStatement, command.Parameters);
            }
            else if (statement is QueryDeleteStatement queryDeleteStatement)
            {
                command.Text = this._icsmOrmQueryBuilder.BuildDeleteStatement(queryDeleteStatement, command.Parameters);
            }
            else if (statement is QuerySelectStatement querySelectStatement)
            {
                command.Text = this._icsmOrmQueryBuilder.BuildSelectStatement(querySelectStatement, command.Parameters);
            }



            if (command == null)
            {
                throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statement.GetType().Name));
            }

            var recordsAffected = this._dataEngine.Execute(command);
            return recordsAffected;
        }
        public int Execute<TModel>(IQueryStatement statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            var command = new EngineCommand();
            if (statement is QueryInsertStatement<TModel> queryInsertStatement)
            {
                command.Text = this._icsmOrmQueryBuilder.BuildInsertStatement(queryInsertStatement, command.Parameters);
            }
            else if (statement is QueryUpdateStatement<TModel> queryUpdateStatement)
            {
                command.Text = this._icsmOrmQueryBuilder.BuildUpdateStatement(queryUpdateStatement, command.Parameters);
            }
            else if (statement is QueryDeleteStatement<TModel> queryDeleteStatement)
            {
                command.Text = this._icsmOrmQueryBuilder.BuildDeleteStatement(queryDeleteStatement, command.Parameters);
            }
            else if (statement is QuerySelectStatement<TModel> querySelectStatement)
            {
                command.Text = this._icsmOrmQueryBuilder.BuildSelectStatement<TModel>(querySelectStatement, command.Parameters);
            }



            if (command == null)
            {
                throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statement.GetType().Name));
            }

            var recordsAffected = this._dataEngine.Execute(command);
            return recordsAffected;
        }

        private EngineCommand BuildSelectCommand(QuerySelectStatement statement)
        {
            var command = new EngineCommand();
            command.Text = this._icsmOrmQueryBuilder.BuildSelectStatement(statement, command.Parameters);
            return command;
        }

    }
}

