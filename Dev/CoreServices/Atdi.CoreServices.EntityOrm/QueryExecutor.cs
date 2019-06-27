using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
//using System.Data;
using System.Linq;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class VoidModel
    {

    }

    internal class PatternExecutionContex<TResult, TModel>
    {
        public PatternExecutionContex(IEngineExecuter executer)
        {
            this.Executer = executer;
        }

        public IEngineExecuter Executer { get; }

        public IQueryStatement Statement { get; set; }

        public Type ResultType { get; set; }

        public EngineExecutionResultKind ResultKind { get; set; }

        public override string ToString()
        {
            return $"Statement ='{Statement?.GetType().FullName}', Kind = {ResultKind}, Result = {typeof(TResult).FullName}[{ResultType?.FullName}]";
        }
    }
    internal sealed class PatternExecutionContexWithHandler<TResult> : PatternExecutionContex<TResult, VoidModel>
    {
        public PatternExecutionContexWithHandler(IEngineExecuter executer) 
            : base(executer)
        {
        }

        public Func<IDataReader, TResult> Handler { get; set; }
    }

    internal sealed class PatternExecutionContexWithHandler<TResult, TModel> : PatternExecutionContex<TResult, TModel>
    {
        public PatternExecutionContexWithHandler(IEngineExecuter executer) 
            : base(executer)
        {
        }

        public Func<IDataReader<TModel>, TResult> Handler { get; set; }
    }

    internal sealed class QueryExecutor : LoggedObject, IQueryExecutor
    {
        private readonly IDataEngine _dataEngine;
        private readonly IEngineExecuter _engineExecuter;
        private readonly PatternBuilderFactory _builderFactory;

        public QueryExecutor(IDataEngine dataEngine, IEngineExecuter engineExecuter, PatternBuilderFactory builderFactory, ILogger logger) : base(logger) 
        {
            this._dataEngine = dataEngine;
            this._engineExecuter = engineExecuter;
            this._builderFactory = builderFactory;

            logger.Debug(Contexts.EntityOrm, Categories.CreatingInstance, Events.CreatedInstanceOfQueryExecutor);
        }

        public int Execute(IQueryStatement statement)
        {
            if (_engineExecuter != null)
            {
                return Execute(statement, _engineExecuter);
            }

            using (var engineExecuter = this._dataEngine.CreateExecuter())
            {
                return Execute(statement, engineExecuter);
            }
        }

        private int Execute(IQueryStatement statement, IEngineExecuter engineExecuter)
        {
            var context = new PatternExecutionContex<int, VoidModel>(engineExecuter)
            {
                ResultKind = EngineExecutionResultKind.RowsAffected,
                Statement = statement
            };
            var result = this._builderFactory.BuildAndExecute(context);
            return result;
        }

        public object Execute(IQueryStatement statement, Type resultType)
        {
            if (_engineExecuter != null)
            {
                return Execute(statement, resultType, _engineExecuter);
            }

            using (var engineExecuter = this._dataEngine.CreateExecuter())
            {
                return Execute(statement, resultType, engineExecuter);
            }
        }
        private object Execute(IQueryStatement statement, Type resultType, IEngineExecuter engineExecuter)
        {
            var context = new PatternExecutionContex<object, VoidModel>(engineExecuter)
            {
                ResultKind = EngineExecutionResultKind.Scalar,
                Statement = statement,
                ResultType = resultType
            };
            var result = this._builderFactory.BuildAndExecute(context);
            return result;
        }


        public TResult Execute<TResult>(IQueryStatement statement)
        {
            if (_engineExecuter != null)
            {
                return Execute<TResult>(statement, _engineExecuter);
            }

            using (var engineExecuter = this._dataEngine.CreateExecuter())
            {
                return Execute<TResult>(statement, engineExecuter);
            }
        }

        private TResult Execute<TResult>(IQueryStatement statement, IEngineExecuter engineExecuter)
        {
            var context = new PatternExecutionContex<TResult, VoidModel>(engineExecuter)
            {
                ResultKind = EngineExecutionResultKind.Scalar,
                Statement = statement
            };
            var result = this._builderFactory.BuildAndExecute(context);
            return result;
        }

        public TResult ExecuteAndFetch<TResult>(IQueryStatement statement, Func<IDataReader, TResult> handler)
        {
            if (_engineExecuter != null)
            {
                return ExecuteAndFetch(statement, handler, _engineExecuter);
            }

            using (var engineExecuter = this._dataEngine.CreateExecuter())
            {
                return ExecuteAndFetch(statement, handler, engineExecuter);
            }
        }

        private TResult ExecuteAndFetch<TResult>(IQueryStatement statement, Func<IDataReader, TResult> handler, IEngineExecuter engineExecuter)
        {
            var context = new PatternExecutionContexWithHandler<TResult>(engineExecuter)
            {
                ResultKind = EngineExecutionResultKind.Reader,
                Statement = statement,
                Handler = handler
            };
            var result = this._builderFactory.BuildAndExecute(context);
            return result;
        }

        public TResult ExecuteAndFetch<TModel, TResult>(IQueryStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler)
        {
            if (_engineExecuter != null)
            {
                return ExecuteAndFetch(statement, handler, _engineExecuter);
            }

            using (var engineExecuter = this._dataEngine.CreateExecuter())
            {
                return ExecuteAndFetch(statement, handler, engineExecuter);
            }
        }

        private TResult ExecuteAndFetch<TModel, TResult>(IQueryStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler, IEngineExecuter engineExecuter)
        {
            var context = new PatternExecutionContexWithHandler<TResult, TModel>(engineExecuter)
            {
                ResultKind = EngineExecutionResultKind.Reader,
                Statement = statement,
                Handler = handler
            };
            var result = this._builderFactory.BuildAndExecute(context);
            return result;
        }


        #region Not implemented

        //public TResult ExecuteAndFetch<TResult>(IQueryStatement[] statements, Func<IDataReader, TResult> handler)
        //{
        //    throw new NotImplementedException();
        //}

        //public TResult ExecuteAndFetch<TModel, TResult>(IQueryStatement<TModel>[] statements, Func<IDataReader<TModel>, TResult> handler)
        //{
        //    throw new NotImplementedException();
        //}



        public DataModels.DataSet Fetch(IQuerySelectStatement statement, DataSetColumn[] columns, DataSetStructure structure)
        {
            throw new NotImplementedException();
        }

        //public TResult Fetch<TResult>(IQuerySelectStatement statement, Func<IDataReader, TResult> handler)
        //{
        //    throw new NotImplementedException();
        //}

        //public TResult Fetch<TModel, TResult>(IQuerySelectStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion


        //public TResult Fetch<TModel, TResult>(IQuerySelectStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler)
        //{
        //    try
        //    {
        //        var objectStatment = statement as QuerySelectStatement<TModel>;
        //        var command = this.BuildSelectCommand(objectStatment.Statement);

        //        var result = default(TResult);
        //        _dataEngine.Execute(command, reader =>
        //        {
        //            var columnsMapper = objectStatment.Statement.QueryDecriptor.SelectableFields.ToDictionary(k => k.Value.Path, e => e.Value.Alias);
        //            var typedReader = new QueryDataReader<TModel>(reader, columnsMapper, null, _dataTypeSystem);
        //            result = handler(typedReader);
        //        });
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        this.Logger.Exception(Contexts.EntityOrm, Categories.FetchingData, e);
        //        throw;
        //    }
        //}

        ////private sealed class DbFieldDescriptor
        ////{
        ////    public DataSetColumn Column { get; set; }
        ////    public Type DbType { get; set; }
        ////    public int Ordinal { get; set; }
        ////}
        ////private TypedCellsDataSet ReadToTypedCellsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
        ////{
        ////    var dataSet = new DataModels.TypedCellsDataSet
        ////    {
        ////        Columns = columns
        ////    };

        ////    var fields = new DbFieldDescriptor[columns.Length];
        ////    int indexForString = -1;
        ////    int indexForBoolean = -1;
        ////    int indexForInteger = -1;
        ////    int indexForDateTime = -1;
        ////    int indexForDouble = -1;
        ////    int indexForFloat = -1;
        ////    int indexForDecimal = -1;
        ////    int indexForByte = -1;
        ////    int indexForBytes = -1;
        ////    int indexForGuid = -1;

        ////    for (int i = 0; i < columns.Length; i++)
        ////    {
        ////        var column = columns[i];
        ////        var dbField = new DbFieldDescriptor
        ////        {
        ////            Column = column,
        ////            Ordinal = dataReader.GetOrdinal(column.Name)
        ////        };
        ////        dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
        ////        fields[i] = dbField;

        ////        switch (column.Type)
        ////        {
        ////            case DataType.String:
        ////                column.Index = ++indexForString;
        ////                break;
        ////            case DataType.Boolean:
        ////                column.Index = ++indexForBoolean;
        ////                break;
        ////            case DataType.Integer:
        ////                column.Index = ++indexForInteger;
        ////                break;
        ////            case DataType.DateTime:
        ////                column.Index = ++indexForDateTime;
        ////                break;
        ////            case DataType.Double:
        ////                column.Index = ++indexForDouble;
        ////                break;
        ////            case DataType.Float:
        ////                column.Index = ++indexForFloat;
        ////                break;
        ////            case DataType.Decimal:
        ////                column.Index = ++indexForDecimal;
        ////                break;
        ////            case DataType.Byte:
        ////                column.Index = ++indexForByte;
        ////                break;
        ////            case DataType.Bytes:
        ////                column.Index = ++indexForBytes;
        ////                break;
        ////            case DataType.Guid:
        ////                column.Index = ++indexForGuid;
        ////                break;
        ////            default:
        ////                column.Index = -1;
        ////                break;
        ////        }
        ////    }

        ////    List<string>[] columnsAsString = null;
        ////    if (indexForString >= 0)
        ////    {
        ////        columnsAsString = new List<string>[indexForString + 1].Select(o => new List<string>()).ToArray();
        ////    }
        ////    List<bool?>[] columnsAsBoolean = null;
        ////    if (indexForBoolean >= 0)
        ////    {
        ////        columnsAsBoolean = new List<bool?>[indexForBoolean + 1].Select(o => new List<bool?>()).ToArray();
        ////    }
        ////    List<int?>[] columnsAsInteger = null;
        ////    if (indexForInteger >= 0)
        ////    {
        ////        columnsAsInteger = new List<int?>[indexForInteger + 1].Select(o => new List<int?>()).ToArray();
        ////    }
        ////    List<DateTime?>[] columnsAsDateTime = null;
        ////    if (indexForDateTime >= 0)
        ////    {
        ////        columnsAsDateTime = new List<DateTime?>[indexForDateTime + 1].Select(o => new List<DateTime?>()).ToArray();
        ////    }
        ////    List<double?>[] columnsAsDouble = null;
        ////    if (indexForDouble >= 0)
        ////    {
        ////        columnsAsDouble = new List<double?>[indexForDouble + 1].Select(o => new List<double?>()).ToArray();
        ////    }
        ////    List<float?>[] columnsAsFloat = null;
        ////    if (indexForFloat >= 0)
        ////    {
        ////        columnsAsFloat = new List<float?>[indexForFloat + 1].Select(o => new List<float?>()).ToArray();
        ////    }
        ////    List<decimal?>[] columnsAsDecimal = null;
        ////    if (indexForDecimal >= 0)
        ////    {
        ////        columnsAsDecimal = new List<decimal?>[indexForDecimal + 1].Select(o => new List<decimal?>()).ToArray();
        ////    }
        ////    List<byte?>[] columnsAsByte = null;
        ////    if (indexForByte >= 0)
        ////    {
        ////        columnsAsByte = new List<byte?>[indexForByte + 1].Select(o => new List<byte?>()).ToArray();
        ////    }
        ////    List<byte[]>[] columnsAsBytes = null;
        ////    if (indexForBytes >= 0)
        ////    {
        ////        columnsAsBytes = new List<byte[]>[indexForBytes + 1].Select(o => new List<byte[]>()).ToArray();
        ////    }
        ////    List<Guid?>[] columnsAsGuid = null;
        ////    if (indexForGuid >= 0)
        ////    {
        ////        columnsAsGuid = new List<byte?>[indexForGuid + 1].Select(o => new List<Guid?>()).ToArray();
        ////    }
        ////    int rowCount = 0;
        ////    while (dataReader.Read())
        ////    {
        ////        for (int i = 0; i < fields.Length; i++)
        ////        {
        ////            var field = fields[i];
        ////            switch (field.Column.Type)
        ////            {
        ////                case DataType.String:
        ////                    var valueAsString = dataReader.GetNullableValueAsString(field.DbType, field.Ordinal);
        ////                    columnsAsString[field.Column.Index].Add(valueAsString);
        ////                    break;
        ////                case DataType.Boolean:
        ////                    var valueAsBoolean = dataReader.GetNullableValueAsBoolean(field.DbType, field.Ordinal);
        ////                    columnsAsBoolean[field.Column.Index].Add(valueAsBoolean);
        ////                    break;
        ////                case DataType.Integer:
        ////                    var valueAsInteger = dataReader.GetNullableValueAsInt32(field.DbType, field.Ordinal);
        ////                    columnsAsInteger[field.Column.Index].Add(valueAsInteger);
        ////                    break;
        ////                case DataType.DateTime:
        ////                    var valueAsDateTime = dataReader.GetNullableValueAsDateTime(field.DbType, field.Ordinal);
        ////                    columnsAsDateTime[field.Column.Index].Add(valueAsDateTime);
        ////                    break;
        ////                case DataType.Double:
        ////                    var valueAsDouble = dataReader.GetNullableValueAsDouble(field.DbType, field.Ordinal);
        ////                    columnsAsDouble[field.Column.Index].Add(valueAsDouble);
        ////                    break;
        ////                case DataType.Float:
        ////                    var valueAsFloat = dataReader.GetNullableValueAsFloat(field.DbType, field.Ordinal);
        ////                    columnsAsFloat[field.Column.Index].Add(valueAsFloat);
        ////                    break;
        ////                case DataType.Decimal:
        ////                    var valueAsDecimal = dataReader.GetNullableValueAsDecimal(field.DbType, field.Ordinal);
        ////                    columnsAsDecimal[field.Column.Index].Add(valueAsDecimal);
        ////                    break;
        ////                case DataType.Byte:
        ////                    var valueAsByte = dataReader.GetNullableValueAsByte(field.DbType, field.Ordinal);
        ////                    columnsAsByte[field.Column.Index].Add(valueAsByte);
        ////                    break;
        ////                case DataType.Bytes:
        ////                    var valueAsBytes = dataReader.GetNullableValueAsBytes(field.DbType, field.Ordinal);
        ////                    columnsAsBytes[field.Column.Index].Add(valueAsBytes);
        ////                    break;
        ////                case DataType.Guid:
        ////                    var valueAsGuid = dataReader.GetNullableValueAsGuid(field.DbType, field.Ordinal);
        ////                    columnsAsGuid[field.Column.Index].Add(valueAsGuid);
        ////                    break;
        ////                default:
        ////                    break;
        ////            }
        ////        }

        ////        ++rowCount;
        ////    }

        ////    if (indexForString >= 0)
        ////    {
        ////        dataSet.StringCells = columnsAsString.Select(c => c.ToArray()).ToArray();
        ////    }
        ////    if (indexForBoolean >= 0)
        ////    {
        ////        dataSet.BooleanCells = columnsAsBoolean.Select(c => c.ToArray()).ToArray();
        ////    }
        ////    if (indexForInteger >= 0)
        ////    {
        ////        dataSet.IntegerCells = columnsAsInteger.Select(c => c.ToArray()).ToArray();
        ////    }
        ////    if (indexForDateTime >= 0)
        ////    {
        ////        dataSet.DateTimeCells = columnsAsDateTime.Select(c => c.ToArray()).ToArray();
        ////    }
        ////    if (indexForDouble >= 0)
        ////    {
        ////        dataSet.DoubleCells = columnsAsDouble.Select(c => c.ToArray()).ToArray();
        ////    }
        ////    if (indexForFloat >= 0)
        ////    {
        ////        dataSet.FloatCells = columnsAsFloat.Select(c => c.ToArray()).ToArray();
        ////    }
        ////    if (indexForDecimal >= 0)
        ////    {
        ////        dataSet.DecimalCells = columnsAsDecimal.Select(c => c.ToArray()).ToArray();
        ////    }
        ////    if (indexForByte >= 0)
        ////    {
        ////        dataSet.ByteCells = columnsAsByte.Select(c => c.ToArray()).ToArray();
        ////    }
        ////    if (indexForBytes >= 0)
        ////    {
        ////        dataSet.BytesCells = columnsAsBytes.Select(c => c.ToArray()).ToArray();
        ////    }
        ////    if (indexForGuid >= 0)
        ////    {
        ////        dataSet.GuidCells = columnsAsGuid.Select(c => c.ToArray()).ToArray();
        ////    }

        ////    dataSet.RowCount = rowCount;
        ////    return dataSet;
        ////}

        ////private StringCellsDataSet ReadToStringCellsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
        ////{
        ////    var dataSet = new DataModels.StringCellsDataSet
        ////    {
        ////        Columns = columns
        ////    };

        ////    var fields = new DbFieldDescriptor[columns.Length];
        ////    for (int i = 0; i < columns.Length; i++)
        ////    {
        ////        var column = columns[i];
        ////        var dbField = new DbFieldDescriptor
        ////        {
        ////            Column = column,
        ////            Ordinal = dataReader.GetOrdinal(column.Name)
        ////        };
        ////        dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
        ////        fields[i] = dbField;
        ////        column.Index = i;
        ////    }

        ////    var rows = new List<string[]>();
        ////    while (dataReader.Read())
        ////    {
        ////        var cells = new string[fields.Length];
        ////        for (int i = 0; i < fields.Length; i++)
        ////        {
        ////            var field = fields[i];
        ////            cells[i] = dataReader.GetValueAsString(field.Column.Type, field.DbType, field.Ordinal);
        ////        }
        ////        rows.Add(cells);
        ////    }
        ////    dataSet.Cells = rows.ToArray();
        ////    dataSet.RowCount = dataSet.Cells.Length;
        ////    return dataSet;
        ////}

        ////private ObjectCellsDataSet ReadToObjectCellsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
        ////{
        ////    var dataSet = new DataModels.ObjectCellsDataSet
        ////    {
        ////        Columns = columns
        ////    };

        ////    var fields = new DbFieldDescriptor[columns.Length];
        ////    for (int i = 0; i < columns.Length; i++)
        ////    {
        ////        var column = columns[i];
        ////        var dbField = new DbFieldDescriptor
        ////        {
        ////            Column = column,
        ////            Ordinal = dataReader.GetOrdinal(column.Name)
        ////        };
        ////        dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
        ////        fields[i] = dbField;
        ////        column.Index = i;
        ////    }

        ////    var rows = new List<object[]>();
        ////    while (dataReader.Read())
        ////    {
        ////        var cells = new object[fields.Length];
        ////        for (int i = 0; i < fields.Length; i++)
        ////        {
        ////            var field = fields[i];
        ////            cells[i] = dataReader.GetValueAsObject(field.Column.Type, field.DbType, field.Ordinal);
        ////        }
        ////        rows.Add(cells);
        ////    }
        ////    dataSet.Cells = rows.ToArray();
        ////    dataSet.RowCount = dataSet.Cells.Length;
        ////    return dataSet;
        ////}

        ////private TypedRowsDataSet ReadToTypedRowsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
        ////{
        ////    var dataSet = new DataModels.TypedRowsDataSet
        ////    {
        ////        Columns = columns
        ////    };

        ////    var fields = new DbFieldDescriptor[columns.Length];
        ////    int indexForString = -1;
        ////    int indexForBoolean = -1;
        ////    int indexForInteger = -1;
        ////    int indexForDateTime = -1;
        ////    int indexForDouble = -1;
        ////    int indexForFloat = -1;
        ////    int indexForDecimal = -1;
        ////    int indexForByte = -1;
        ////    int indexForBytes = -1;
        ////    int indexForGuid = -1;

        ////    for (int i = 0; i < columns.Length; i++)
        ////    {
        ////        var column = columns[i];
        ////        var dbField = new DbFieldDescriptor
        ////        {
        ////            Column = column,
        ////            Ordinal = dataReader.GetOrdinal(column.Name)
        ////        };
        ////        dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
        ////        fields[i] = dbField;

        ////        switch (column.Type)
        ////        {
        ////            case DataType.String:
        ////                column.Index = ++indexForString;
        ////                break;
        ////            case DataType.Boolean:
        ////                column.Index = ++indexForBoolean;
        ////                break;
        ////            case DataType.Integer:
        ////                column.Index = ++indexForInteger;
        ////                break;
        ////            case DataType.DateTime:
        ////                column.Index = ++indexForDateTime;
        ////                break;
        ////            case DataType.Double:
        ////                column.Index = ++indexForDouble;
        ////                break;
        ////            case DataType.Float:
        ////                column.Index = ++indexForFloat;
        ////                break;
        ////            case DataType.Decimal:
        ////                column.Index = ++indexForDecimal;
        ////                break;
        ////            case DataType.Byte:
        ////                column.Index = ++indexForByte;
        ////                break;
        ////            case DataType.Bytes:
        ////                column.Index = ++indexForBytes;
        ////                break;
        ////            case DataType.Guid:
        ////                column.Index = ++indexForGuid;
        ////                break;
        ////            default:
        ////                column.Index = -1;
        ////                break;
        ////        }
        ////    }

        ////    var rows = new List<TypedDataRow>();
        ////    while (dataReader.Read())
        ////    {
        ////        var row = new TypedDataRow();
        ////        if (indexForString >= 0)
        ////        {
        ////            row.StringCells = new string[indexForString + 1];
        ////        }
        ////        if (indexForBoolean >= 0)
        ////        {
        ////            row.BooleanCells = new bool?[indexForBoolean + 1];
        ////        }
        ////        if (indexForInteger >= 0)
        ////        {
        ////            row.IntegerCells = new int?[indexForInteger + 1];
        ////        }
        ////        if (indexForDateTime >= 0)
        ////        {
        ////            row.DateTimeCells = new DateTime?[indexForDateTime + 1];
        ////        }
        ////        if (indexForDouble >= 0)
        ////        {
        ////            row.DoubleCells = new double?[indexForDouble + 1];
        ////        }
        ////        if (indexForFloat >= 0)
        ////        {
        ////            row.FloatCells = new float?[indexForFloat + 1];
        ////        }
        ////        if (indexForDecimal >= 0)
        ////        {
        ////            row.DecimalCells = new decimal?[indexForDecimal + 1];
        ////        }
        ////        if (indexForByte >= 0)
        ////        {
        ////            row.ByteCells = new byte?[indexForByte + 1];
        ////        }
        ////        if (indexForBytes >= 0)
        ////        {
        ////            row.BytesCells = new byte[indexForBytes + 1][];
        ////        }
        ////        if (indexForGuid >= 0)
        ////        {
        ////            row.GuidCells = new Guid?[indexForGuid + 1];
        ////        }
        ////        for (int i = 0; i < fields.Length; i++)
        ////        {
        ////            var field = fields[i];
        ////            switch (field.Column.Type)
        ////            {
        ////                case DataType.String:
        ////                    if (!dataReader.IsDBNull(field.Ordinal))
        ////                        row.StringCells[field.Column.Index] = dataReader.GetValueAsString(field.DbType, field.Ordinal);
        ////                    break;
        ////                case DataType.Boolean:
        ////                    if(!dataReader.IsDBNull(field.Ordinal))
        ////                        row.BooleanCells[field.Column.Index] = dataReader.GetValueAsBoolean(field.DbType, field.Ordinal);
        ////                    break;
        ////                case DataType.Integer:
        ////                    if(!dataReader.IsDBNull(field.Ordinal))
        ////                        row.IntegerCells[field.Column.Index] = dataReader.GetValueAsInt32(field.DbType, field.Ordinal);
        ////                    break;
        ////                case DataType.DateTime:
        ////                    if(!dataReader.IsDBNull(field.Ordinal))
        ////                        row.DateTimeCells[field.Column.Index] = dataReader.GetValueAsDateTime(field.DbType, field.Ordinal);
        ////                    break;
        ////                case DataType.Double:
        ////                    if (!dataReader.IsDBNull(field.Ordinal))
        ////                        row.DoubleCells[field.Column.Index] = dataReader.GetValueAsDouble(field.DbType, field.Ordinal);
        ////                    break;
        ////                case DataType.Float:
        ////                    if (!dataReader.IsDBNull(field.Ordinal))
        ////                        row.FloatCells[field.Column.Index] = dataReader.GetValueAsFloat(field.DbType, field.Ordinal);
        ////                    break;
        ////                case DataType.Decimal:
        ////                    if (!dataReader.IsDBNull(field.Ordinal))
        ////                        row.DecimalCells[field.Column.Index] = dataReader.GetValueAsDecimal(field.DbType, field.Ordinal);
        ////                    break;
        ////                case DataType.Byte:
        ////                    if (!dataReader.IsDBNull(field.Ordinal))
        ////                        row.ByteCells [field.Column.Index] = dataReader.GetValueAsByte(field.DbType, field.Ordinal);
        ////                    break;
        ////                case DataType.Bytes:
        ////                    if (!dataReader.IsDBNull(field.Ordinal))
        ////                        row.BytesCells[field.Column.Index] = dataReader.GetValueAsBytes(field.DbType, field.Ordinal);
        ////                    break;
        ////                case DataType.Guid:
        ////                    if (!dataReader.IsDBNull(field.Ordinal))
        ////                        row.GuidCells[field.Column.Index] = dataReader.GetValueAsGuid(field.DbType, field.Ordinal);
        ////                    break;
        ////                default:
        ////                    break;
        ////            }
        ////        }
        ////        rows.Add(row);
        ////    }

        ////    dataSet.Rows = rows.ToArray();
        ////    dataSet.RowCount = dataSet.Rows.Length;

        ////    return dataSet;
        ////}

        ////private StringRowsDataSet ReadToStringRowsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
        ////{
        ////    var dataSet = new DataModels.StringRowsDataSet
        ////    {
        ////        Columns = columns
        ////    };

        ////    var fields = new DbFieldDescriptor[columns.Length];
        ////    for (int i = 0; i < columns.Length; i++)
        ////    {
        ////        var column = columns[i];
        ////        var dbField = new DbFieldDescriptor
        ////        {
        ////            Column = column,
        ////            Ordinal = dataReader.GetOrdinal(column.Name)
        ////        };
        ////        dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
        ////        fields[i] = dbField;
        ////        column.Index = i;
        ////    }

        ////    var rows = new List<StringDataRow>();
        ////    while (dataReader.Read())
        ////    {
        ////        var row = new StringDataRow();
        ////        var cells = new string[fields.Length];
        ////        for (int i = 0; i < fields.Length; i++)
        ////        {
        ////            var field = fields[i];
        ////            cells[i] = dataReader.GetValueAsString(field.Column.Type, field.DbType, field.Ordinal);
        ////        }
        ////        row.Cells = cells;
        ////        rows.Add(row);
        ////    }
        ////    dataSet.Rows = rows.ToArray();
        ////    dataSet.RowCount = dataSet.Rows.Length;
        ////    return dataSet;
        ////}

        ////private ObjectRowsDataSet ReadToObjectRowsDataSet(Atdi.Contracts.CoreServices.DataLayer.IDataReader dataReader, DataSetColumn[] columns)
        ////{
        ////    var dataSet = new DataModels.ObjectRowsDataSet
        ////    {
        ////        Columns = columns
        ////    };

        ////    var fields = new DbFieldDescriptor[columns.Length];
        ////    for (int i = 0; i < columns.Length; i++)
        ////    {
        ////        var column = columns[i];
        ////        var dbField = new DbFieldDescriptor
        ////        {
        ////            Column = column,
        ////            Ordinal = dataReader.GetOrdinal(column.Name)
        ////        };
        ////        dbField.DbType = dataReader.GetFieldType(dbField.Ordinal);
        ////        fields[i] = dbField;
        ////        column.Index = i;
        ////    }

        ////    var rows = new List<ObjectDataRow>();
        ////    while(dataReader.Read())
        ////    {
        ////        var row = new ObjectDataRow();
        ////        var cells = new object[fields.Length];
        ////        for (int i = 0; i < fields.Length; i++)
        ////        {
        ////            var field = fields[i];
        ////            cells[i] = dataReader.GetValueAsObject(field.Column.Type, field.DbType, field.Ordinal);
        ////        }
        ////        row.Cells = cells;
        ////        rows.Add(row);
        ////    }
        ////    dataSet.Rows = rows.ToArray();
        ////    dataSet.RowCount = dataSet.Rows.Length;
        ////    return dataSet;
        ////}


        //private EngineCommand BuildSelectCommand(QuerySelectStatement statement)
        //{
        //    var command = new EngineCommand();
        //    command.Text = this._icsmOrmQueryBuilder.BuildSelectStatement(statement, command.Parameters);
        //    return command;
        //}


        //public TResult Fetch<TResult>(IQuerySelectStatement statement, Func<Contracts.CoreServices.DataLayer.IDataReader, TResult> handler)
        //{
        //    try
        //    {
        //        var objectStatment = statement as QuerySelectStatement;
        //        var command = this.BuildSelectCommand(objectStatment);

        //        var result = default(TResult);
        //        _dataEngine.Execute(command, reader =>
        //        {
        //            var columnsMapper = objectStatment.QueryDecriptor.SelectableFields.ToDictionary(k => k.Value.Path, e => e.Value.Alias);
        //            var typedReader = new QueryDataReader(reader, columnsMapper, null, _dataTypeSystem);
        //            result = handler(typedReader);
        //        });
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        this.Logger.Exception(Contexts.EntityOrm, Categories.FetchingData, e);
        //        throw;
        //    }

        //}

        //public DataModels.DataSet Fetch(IQuerySelectStatement statement, DataSetColumn[] columns, DataSetStructure structure)
        //{
        //    //var dataSet =
        //    //   this.Fetch<DataModels.DataSet>(statement, reader =>
        //    //   {
        //    //       switch (structure)
        //    //       {
        //    //           case DataSetStructure.TypedCells:
        //    //               return (DataModels.DataSet)ReadToTypedCellsDataSet(reader, columns);
        //    //           case DataSetStructure.StringCells:
        //    //               return (DataModels.DataSet)ReadToStringCellsDataSet(reader, columns);
        //    //           case DataSetStructure.ObjectCells:
        //    //               return (DataModels.DataSet)ReadToObjectCellsDataSet(reader, columns);
        //    //           case DataSetStructure.TypedRows:
        //    //               return (DataModels.DataSet)ReadToTypedRowsDataSet(reader, columns);
        //    //           case DataSetStructure.StringRows:
        //    //               return (DataModels.DataSet)ReadToStringRowsDataSet(reader, columns);
        //    //           case DataSetStructure.ObjectRows:
        //    //               return (DataModels.DataSet)ReadToObjectRowsDataSet(reader, columns);
        //    //           default:
        //    //               throw new InvalidOperationException(Exceptions.DataSetStructureNotSupported.With(structure));
        //    //       }
        //    //   });

        //    //return dataSet;

        //    throw new NotImplementedException();
        //}


        //public int Execute(IQueryStatement statement)
        //{
        //    if (statement == null)
        //    {
        //        throw new ArgumentNullException(nameof(statement));
        //    }

        //    int recordsAffected = 0;

        //    try
        //    {
        //        var extractType = statement.GetType();

        //        if (extractType.Name.Contains(typeof(QuerySelectStatement).Name))
        //        {
        //            statement = extractType.GetProperty(_propertyName).GetValue(statement, null) as QuerySelectStatement;
        //        }
        //        else if (extractType.Name.Contains(typeof(QueryDeleteStatement).Name))
        //        {
        //            statement = extractType.GetProperty(_propertyName).GetValue(statement, null) as QueryDeleteStatement;
        //        }
        //        else if (extractType.Name.Contains(typeof(QueryInsertStatement).Name))
        //        {
        //            statement = extractType.GetProperty(_propertyName).GetValue(statement, null) as QueryInsertStatement;
        //        }
        //        else if (extractType.Name.Contains(typeof(QueryUpdateStatement).Name))
        //        {
        //            statement = extractType.GetProperty(_propertyName).GetValue(statement, null) as QueryUpdateStatement;
        //        }


        //        var command = new EngineCommand();


        //        if (statement is QueryUpdateStatement queryUpdateStatement)
        //        {
        //            command.Text = this._icsmOrmQueryBuilder.BuildUpdateStatement(queryUpdateStatement, command.Parameters);
        //        }
        //        else if (statement is QueryDeleteStatement queryDeleteStatement)
        //        {
        //            command.Text = this._icsmOrmQueryBuilder.BuildDeleteStatement(queryDeleteStatement, command.Parameters);
        //        }
        //        else if (statement is QuerySelectStatement querySelectStatement)
        //        {
        //            command.Text = this._icsmOrmQueryBuilder.BuildSelectStatement(querySelectStatement, command.Parameters);
        //        }
        //        else if (statement is QueryInsertStatement queryInsertStatement)
        //        {
        //            command.Text = this._icsmOrmQueryBuilder.BuildInsertStatement(queryInsertStatement, command.Parameters);

        //            // закрыл следующую логику, так как в этом контексте по ряду причин ей не место
        //            //var dicIdentField = new KeyValuePair<string, DataType>();
        //            //dicIdentField = this._icsmOrmQueryBuilder.GetIdentFieldFromTable(queryInsertStatement, command.Parameters);
        //            //if (!string.IsNullOrEmpty(dicIdentField.Key))
        //            //{
        //            //    if ((dicIdentField.Value != DataType.String) && (dicIdentField.Value != DataType.Guid))
        //            //    {
        //            //        command.Text = this._icsmOrmQueryBuilder.BuildInsertStatementExecuteAndFetch(queryInsertStatement, command.Parameters);
        //            //        EngineCommandParameter engineCommandParameter = new EngineCommandParameter();
        //            //        engineCommandParameter.Name = _nameIdentFieldParameter;
        //            //        engineCommandParameter.DataType = DataType.Integer;
        //            //        command.Parameters.Add(new KeyValuePair<string, EngineCommandParameter>(_nameIdentFieldParameter, engineCommandParameter));
        //            //    }
        //            //    else
        //            //    {
        //            //        command.Text = this._icsmOrmQueryBuilder.BuildInsertStatement(queryInsertStatement, command.Parameters);
        //            //    }
        //            //}
        //            //else
        //            //{
        //            //    throw new InvalidOperationException("Not found primary key description");
        //            //}
        //        }

        //        if (command == null)
        //        {
        //            throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statement.GetType().Name));
        //        }

        //        if (statement is QuerySelectStatement)
        //        {
        //            this._dataEngine.Execute(command, (System.Data.IDataReader reader) =>
        //            {
        //                while (reader.Read())
        //                {
        //                    recordsAffected++;
        //                }
        //            });
        //        }
        //        else
        //        {
        //            if (statement is QueryInsertStatement)
        //            {
        //                recordsAffected = this._dataEngine.Execute(command);
        //            }
        //            else if (statement is QueryUpdateStatement)
        //            {
        //                recordsAffected = this._dataEngine.Execute(command);
        //            }
        //            else if (statement is QueryDeleteStatement)
        //            {
        //                recordsAffected = this._dataEngine.Execute(command);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.Message != Exceptions.AbortedBuildSqlStatement)
        //        {
        //            throw new InvalidOperationException(e.Message, e);
        //        }
        //    }
        //    return recordsAffected;
        //}

        //public TResult ExecuteAndFetch<TResult>(IQueryStatement statement, Func<Contracts.CoreServices.DataLayer.IDataReader, TResult> handler)
        //{
        //    var addedIdentField = false;
        //    var dicIdentField = new KeyValuePair<string, DataType>();
        //    if (statement == null)
        //    {
        //        throw new ArgumentNullException(nameof(statement));
        //    }

        //    var extractType = statement.GetType();

        //    if (extractType.Name.Contains(typeof(QueryInsertStatement).Name))
        //    {
        //        statement = extractType.GetProperty(_propertyName).GetValue(statement, null) as QueryInsertStatement;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statement.GetType().Name));
        //    }


        //    var command = new EngineCommand();
        //    if (statement is QueryInsertStatement queryInsertStatement)
        //    {
        //        command.Text = this._icsmOrmQueryBuilder.BuildInsertStatementExecuteAndFetch(queryInsertStatement, command.Parameters);
        //        dicIdentField = this._icsmOrmQueryBuilder.GetIdentFieldFromTable(queryInsertStatement, command.Parameters);
        //        if (!string.IsNullOrEmpty(dicIdentField.Key))
        //        {
        //            EngineCommandParameter engineCommandParameter = new EngineCommandParameter();
        //            engineCommandParameter.Name = _nameIdentFieldParameter;
        //            engineCommandParameter.DataType = DataType.Integer;
        //            command.Parameters.Add(new KeyValuePair<string, EngineCommandParameter>(_nameIdentFieldParameter, engineCommandParameter));
        //            addedIdentField = true;
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("Not found primary key description");
        //        }
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statement.GetType().Name));
        //    }

        //    if (command == null)
        //    {
        //        throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statement.GetType().Name));
        //    }

        //    var result = default(TResult);
        //    if ((addedIdentField) && (statement is QueryInsertStatement))
        //    {
        //        int val = this._dataEngine.Execute(command);
        //        object resIdentObject = command.Parameters[_nameIdentFieldParameter].Value;
        //        if (resIdentObject != null)
        //        {
        //            var res = Int32.Parse(resIdentObject.ToString());
        //            var objectStatment = (statement as QueryInsertStatement).SelectStatement;
        //            if ((objectStatment != null) && (!string.IsNullOrEmpty(dicIdentField.Key)))
        //            {
        //                objectStatment.Where(dicIdentField.Key, res);
        //                var commandSelect = this.BuildSelectCommand(objectStatment);
        //                _dataEngine.Execute(commandSelect, reader =>
        //                {
        //                    var columnsMapper = objectStatment.QueryDecriptor.SelectableFields.ToDictionary(k => k.Value.Path, e => e.Value.Alias);
        //                    var typedReader = new QueryDataReader(reader, columnsMapper, null, _dataTypeSystem);
        //                    result = handler(typedReader);
        //                });
        //            }
        //        }
        //    }
        //    else
        //    { 
        //        throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statement.GetType().Name));
        //    }
        //    return result;

        //}

        //public TResult ExecuteAndFetch<TModel, TResult>(IQueryStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler)
        //{
        //    var addedIdentField = false;
        //    var dicIdentField = new KeyValuePair<string, DataType>();
        //    var extractType = statement.GetType();
        //    IQueryStatement statementQuery = null;

        //    if (extractType.Name.Contains(typeof(QueryInsertStatement).Name))
        //    {
        //        statementQuery = extractType.GetProperty(_propertyName).GetValue(statement, null) as QueryInsertStatement;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statement.GetType().Name));
        //    }


        //    var command = new EngineCommand();
        //    if (statementQuery is QueryInsertStatement queryInsertStatement)
        //    {
        //        command.Text = this._icsmOrmQueryBuilder.BuildInsertStatementExecuteAndFetch(queryInsertStatement, command.Parameters);
        //        dicIdentField = this._icsmOrmQueryBuilder.GetIdentFieldFromTable(queryInsertStatement, command.Parameters);
        //        if (!string.IsNullOrEmpty(dicIdentField.Key))
        //        {
        //            EngineCommandParameter engineCommandParameter = new EngineCommandParameter();
        //            engineCommandParameter.Name = _nameIdentFieldParameter;
        //            engineCommandParameter.DataType = DataType.Integer;
        //            command.Parameters.Add(new KeyValuePair<string, EngineCommandParameter>(_nameIdentFieldParameter, engineCommandParameter));
        //            addedIdentField = true;
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("Not found primary key description");
        //        }
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statementQuery.GetType().Name));
        //    }

        //    if (command == null)
        //    {
        //        throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statement.GetType().Name));
        //    }

        //    var result = default(TResult);
        //    if ((addedIdentField) && (statementQuery is QueryInsertStatement))
        //    {
        //        int val = this._dataEngine.Execute(command);
        //        object resIdentObject = command.Parameters[_nameIdentFieldParameter].Value;
        //        if (resIdentObject != null)
        //        {
        //            var res = Int32.Parse(resIdentObject.ToString());
        //            var objectStatment = (statement as QueryInsertStatement<TModel>).Statement.SelectStatement;
        //            if ((objectStatment != null) && (!string.IsNullOrEmpty(dicIdentField.Key)))
        //            {
        //                objectStatment.Where(dicIdentField.Key, res);
        //                var commandSelect = this.BuildSelectCommand(objectStatment);
        //                _dataEngine.Execute(commandSelect, reader =>
        //                {
        //                    var columnsMapper = objectStatment.QueryDecriptor.SelectableFields.ToDictionary(k => k.Value.Path, e => e.Value.Alias);
        //                    var typedReader = new QueryDataReader<TModel>(reader, columnsMapper, null, _dataTypeSystem);
        //                    result = handler(typedReader);
        //                });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statementQuery.GetType().Name));
        //    }
        //    return result;

        //}

        //public TResult ExecuteAndFetch<TResult>(IQueryStatement[] statements, Func<Contracts.CoreServices.DataLayer.IDataReader, TResult> handler)
        //{
        //    var listBulkQueryInsert = new QueryInsertStatement[statements.Length];
        //    for (int i = 0; i < statements.Length; i++)
        //    {
        //        var statement = statements[i];
        //        var extractType = statement.GetType();


        //        if (extractType.Name.Contains(typeof(QueryInsertStatement).Name))
        //        {
        //            statement = extractType.GetProperty(_propertyName).GetValue(statement, null) as QueryInsertStatement;
        //        }


        //        if (statement is QueryInsertStatement queryInsertStatement)
        //        {
        //            listBulkQueryInsert[i] = queryInsertStatement;
        //        }

        //    }

        //    int rowAffected = 0;
        //    var listTempBulkQueryInsert = new List<QueryInsertStatement>();
        //    int cnt = 0;
        //    for (int j = 0; j < listBulkQueryInsert.Length; j++)
        //    {
        //        cnt++;
        //        if (cnt == _syntax.MaxBatchSizeBuffer)
        //        {
        //            listTempBulkQueryInsert.Add(listBulkQueryInsert[j]);
        //            var command = new EngineCommand();
        //            command.Text = this._icsmOrmQueryBuilder.BuildInsertStatementExecuteAndFetch(listTempBulkQueryInsert.ToArray(), command.Parameters);
        //            var dicIdentField = this._icsmOrmQueryBuilder.GetIdentFieldFromTable(listTempBulkQueryInsert[0], command.Parameters);
        //            if (!string.IsNullOrEmpty(dicIdentField.Key))
        //            {
        //                EngineCommandParameter engineCommandParameter = new EngineCommandParameter();
        //                engineCommandParameter.Name = _nameIdentFieldParameter;
        //                engineCommandParameter.DataType = DataType.Integer;
        //                command.Parameters.Add(new KeyValuePair<string, EngineCommandParameter>(_nameIdentFieldParameter, engineCommandParameter));
        //            }
        //            else
        //            {
        //                throw new InvalidOperationException("Not found primary key description");
        //            }
        //            rowAffected += this._dataEngine.Execute(command);
        //            listTempBulkQueryInsert.Clear();
        //            cnt = 0;
        //        }
        //        else
        //        {
        //            listTempBulkQueryInsert.Add(listBulkQueryInsert[j]);
        //        }
        //    }

        //    if (listTempBulkQueryInsert.Count > 0)
        //    {
        //        var command = new EngineCommand();
        //        command.Text = this._icsmOrmQueryBuilder.BuildInsertStatementExecuteAndFetch(listTempBulkQueryInsert.ToArray(), command.Parameters);
        //        var dicIdentField = this._icsmOrmQueryBuilder.GetIdentFieldFromTable(listTempBulkQueryInsert[0], command.Parameters);
        //        if (!string.IsNullOrEmpty(dicIdentField.Key))
        //        {
        //            EngineCommandParameter engineCommandParameter = new EngineCommandParameter();
        //            engineCommandParameter.Name = _nameIdentFieldParameter;
        //            engineCommandParameter.DataType = DataType.Integer;
        //            command.Parameters.Add(new KeyValuePair<string, EngineCommandParameter>(_nameIdentFieldParameter, engineCommandParameter));
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("Not found primary key description");
        //        }
        //        rowAffected += this._dataEngine.Execute(command);
        //        listTempBulkQueryInsert.Clear();
        //    }


        //    var result = default(TResult);
        //    return result;
        //}

        //public TResult ExecuteAndFetch<TModel, TResult>(IQueryStatement<TModel>[] statements, Func<IDataReader<TModel>, TResult> handler)
        //{

        //    var listBulkQueryInsert = new QueryInsertStatement[statements.Length];
        //    for (int i = 0; i < statements.Length; i++)
        //    {
        //        var statement = statements[i];
        //        var extractType = statement.GetType();
        //        IQueryStatement statementQuery = null;
        //        if (extractType.Name.Contains(typeof(QueryInsertStatement).Name))
        //        {
        //            statementQuery = extractType.GetProperty(_propertyName).GetValue(statement, null) as QueryInsertStatement;
        //            if (statementQuery is QueryInsertStatement queryInsertStatement)
        //            {
        //                listBulkQueryInsert[i] = queryInsertStatement;
        //            }
        //        }
        //    }

        //    int rowAffected = 0;
        //    var listTempBulkQueryInsert = new  List<QueryInsertStatement>();
        //    int cnt = 0;
        //    for (int j = 0; j < listBulkQueryInsert.Length; j++)
        //    {
        //        cnt++;
        //        if (cnt == _syntax.MaxBatchSizeBuffer)
        //        {
        //            listTempBulkQueryInsert.Add(listBulkQueryInsert[j]);
        //            var command = new EngineCommand();
        //            command.Text = this._icsmOrmQueryBuilder.BuildInsertStatementExecuteAndFetch(listTempBulkQueryInsert.ToArray(), command.Parameters);
        //            var dicIdentField = this._icsmOrmQueryBuilder.GetIdentFieldFromTable(listTempBulkQueryInsert[0], command.Parameters);
        //            if (!string.IsNullOrEmpty(dicIdentField.Key))
        //            {
        //                EngineCommandParameter engineCommandParameter = new EngineCommandParameter();
        //                engineCommandParameter.Name = _nameIdentFieldParameter;
        //                engineCommandParameter.DataType = DataType.Integer;
        //                command.Parameters.Add(new KeyValuePair<string, EngineCommandParameter>(_nameIdentFieldParameter, engineCommandParameter));
        //            }
        //            else
        //            {
        //                throw new InvalidOperationException("Not found primary key description");
        //            }
        //            rowAffected += this._dataEngine.Execute(command);
        //            listTempBulkQueryInsert.Clear();
        //            cnt = 0;
        //        }
        //        else
        //        {
        //            listTempBulkQueryInsert.Add(listBulkQueryInsert[j]);
        //        }
        //    }

        //    if (listTempBulkQueryInsert.Count > 0)
        //    {
        //        var command = new EngineCommand();
        //        command.Text = this._icsmOrmQueryBuilder.BuildInsertStatementExecuteAndFetch(listTempBulkQueryInsert.ToArray(), command.Parameters);
        //        var dicIdentField = this._icsmOrmQueryBuilder.GetIdentFieldFromTable(listTempBulkQueryInsert[0], command.Parameters);
        //        if (!string.IsNullOrEmpty(dicIdentField.Key))
        //        {
        //            EngineCommandParameter engineCommandParameter = new EngineCommandParameter();
        //            engineCommandParameter.Name = _nameIdentFieldParameter;
        //            engineCommandParameter.DataType = DataType.Integer;
        //            command.Parameters.Add(new KeyValuePair<string, EngineCommandParameter>(_nameIdentFieldParameter, engineCommandParameter));
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("Not found primary key description");
        //        }
        //        rowAffected += this._dataEngine.Execute(command);
        //        listTempBulkQueryInsert.Clear();
        //    }


        //    var result = default(TResult);
        //    return result;
        //}

        //public object Execute(IQueryStatement statement, Type resultType)
        //{
        //    throw new NotImplementedException();
        //}

        //public TResult Execute<TResult>(IQueryStatement statement)
        //{
        //    throw new NotImplementedException();
        //}


        //public int InsertSelect<TModelInsert, TModelSelect>(IQueryStatement<TModelInsert> statementInsert, IQuerySelectStatement<TModelSelect> selectStatement)
        //{
        //    var rowAffected = 0;
        //    IQueryStatement statementQueryInsert = null;
        //    IQueryStatement statementQuerySelect = null;
        //    var extractTypeInsert = statementInsert.GetType();
        //    var extractTypeSelect = selectStatement.GetType();
        //    if (extractTypeSelect.Name.Contains(typeof(QuerySelectStatement).Name))
        //    {
        //        statementQuerySelect = extractTypeSelect.GetProperty(_propertyName).GetValue(selectStatement, null) as QuerySelectStatement;
        //    }
        //    if (extractTypeInsert.Name.Contains(typeof(QueryInsertStatement).Name))
        //    {
        //        statementQueryInsert = extractTypeInsert.GetProperty(_propertyName).GetValue(statementInsert, null) as QueryInsertStatement;
        //    }

        //    var command = new EngineCommand();

        //    if ((statementQueryInsert is QueryInsertStatement queryInsertStatement) && (statementQuerySelect is QuerySelectStatement querySelectStatement))
        //    {
        //        command.Text = this._icsmOrmQueryBuilder.BuildInsertSelectStatement(queryInsertStatement, querySelectStatement, command.Parameters);
        //        rowAffected += this._dataEngine.Execute(command);
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(Exceptions.QueryStatementNotSupported.With(statementInsert.GetType().Name) + " or " + Exceptions.QueryStatementNotSupported.With(selectStatement.GetType().Name));
        //    }
        //    return rowAffected;
        //}
    }
}

