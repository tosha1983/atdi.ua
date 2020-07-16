using Atdi.Common;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.Sqlite
{
    internal sealed class SqliteExecutor : LoggedObject
    {
        private SqliteConnection _connection;
        //private SqlTransaction _transaction;

        public SqliteExecutor(SqliteConnection connection, ILogger logger) : base(logger)
        {
            this._connection = connection;
            //this._transaction = transaction;
            logger.Verbouse(Contexts.SqliteEngine, Categories.Creation, Events.ObjectWasCreated.With("SqliteExecutor"));
        }

        private SqliteCommand CreatCommand(EngineCommand engineCommand)
        {
            using (var traceScope = this.Logger.StartTrace(Contexts.SqliteEngine, Categories.CommandCreation, this))
            {
                var sqlCommand = _connection.CreateCommand();
                sqlCommand.CommandText = engineCommand.Text;
                sqlCommand.CommandType = CommandType.Text;
                //if (_transaction != null)
                //{
                //    sqlCommand.Transaction = _transaction;
                //}
                this.PrepareParameters(sqlCommand, engineCommand.Parameters.Values);

                traceScope.SetData("Parameters:", sqlCommand.Parameters.Count);
                var parameters = engineCommand.Parameters.Values.ToArray();
                for (int i = 0; i < parameters.Length; i++)
                {
                    try
                    {
                        traceScope.SetData($"Parameter {i}:", parameters[i]);
                    }
                    catch (Exception) { traceScope.SetData($"Parameter {i}:", "Not visible"); }
                }

                return sqlCommand;
            }
        }

        public int ExecuteNonQuery(EngineCommand engineCommand)
        {
            using (var sqlCommand = this.CreatCommand(engineCommand))
            {
                try
                {
                    using (var trace = StartTraceExecuting(sqlCommand))
                    {
                        var result = sqlCommand.ExecuteNonQuery();
                        this.EnsureOutputParameters(sqlCommand, engineCommand);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.SqliteEngine, Categories.Processing, e, this);
                    throw new InvalidOperationException(Exceptions.FailedToExecuteNonQuery, e);
                }
            }
        }

        private void EnsureOutputParameters(SqliteCommand sqlCommand, EngineCommand engineCommand)
        {
            for (int i = 0; i < sqlCommand.Parameters.Count; i++)
            {
                var sqlParameter = sqlCommand.Parameters[i];
                if (sqlParameter.Direction == ParameterDirection.Output
                    || sqlParameter.Direction == ParameterDirection.InputOutput)
                {
                    var key = sqlParameter.ParameterName.Substring(1, sqlParameter.ParameterName.Length - 1);
                    engineCommand.Parameters[key].Value = sqlParameter.Value;
                }
            }
        }

        public object ExecuteScalar(EngineCommand engineCommand)
        {
            using (var sqlCommand = this.CreatCommand(engineCommand))
            {
                try
                {
                    using (var trace = StartTraceExecuting(sqlCommand))
                    {
                        var result = sqlCommand.ExecuteScalar();
                        this.EnsureOutputParameters(sqlCommand, engineCommand);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.SqliteEngine, Categories.Executing, e, this);
                    throw new InvalidOperationException(Exceptions.FailedToExecuteScalarQuery, e);
                }
            }
        }

        public T ExecuteReader<T>(EngineCommand engineCommand, Func<SqliteDataReader, T> handler)
        {
            using (var sqlCommand = this.CreatCommand(engineCommand))
            {
                T result = default(T);
                SqliteDataReader reader = null;
                try
                {
                   
                    using (var trace = StartTraceExecuting(sqlCommand))
                    {
                        reader = sqlCommand.ExecuteReader();
                        this.EnsureOutputParameters(sqlCommand, engineCommand);
                    }
                    //this.EnsureOutputParameters();
                    using (var trace = this.Logger.StartTrace(Contexts.SqliteEngine, Categories.ResultHandling, this))
                    {
                        result = handler(reader);
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.SqliteEngine, Categories.Executing, e, this);
                    throw new InvalidOperationException(Exceptions.FailedToExecuteReaderQuery, e);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Dispose();
                    }
                }
                return result;
            }
        }

        public void ExecuteReader(EngineCommand engineCommand, Action<SqliteDataReader> handler)
        {
            using (var sqlCommand = this.CreatCommand(engineCommand))
            {
                SqliteDataReader reader = null;
                try
                {

                    using (var trace = StartTraceExecuting(sqlCommand))
                    {
                        reader = sqlCommand.ExecuteReader();
                        this.EnsureOutputParameters(sqlCommand, engineCommand);
                    }
                    //this.EnsureOutputParameters();
                    using (var trace = this.Logger.StartTrace(Contexts.SqliteEngine, Categories.ResultHandling, this))
                    {
                        handler(reader);
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.SqliteEngine, Categories.Executing, e, this);
                    throw new InvalidOperationException(Exceptions.FailedToExecuteReaderQuery, e);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Dispose();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ITraceScope StartTraceExecuting(SqliteCommand sqlCommand)
        {
            var traceScope = this.Logger.StartTrace(Contexts.SqliteEngine, Categories.CommandExecuting, this);
            traceScope.SetData("SQL Statement", Environment.NewLine + sqlCommand.CommandText);
            return traceScope;
        }


        private void PrepareParameters(SqliteCommand sqlCommand, ICollection<EngineCommandParameter> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    sqlCommand.Parameters.Add(this.CreateSqlParameter(parameter));
                }
            }
        }

        private SqliteParameter CreateSqlParameter(EngineCommandParameter parameter)
        {
            // все типы из DataType должныт мапинться или конвертироваться
            var sqlParameter = new SqliteParameter
            {
                ParameterName = "@" + parameter.Name,
                SqliteType = ToSqlDbType(parameter.DataType),
                Value = parameter.Value ?? DBNull.Value,
                Direction = parameter.Direction.CopyTo<ParameterDirection>()
            };

            // мелкая конвертация для знакового байта и безнаковіх целых
            if (parameter.Value != null)
            {
                if (parameter.DataType == DataType.SignedByte)
                {
                    sqlParameter.Value = Convert.ToInt16(parameter.Value);
                }
                else if (parameter.DataType == DataType.UnsignedShort)
                {
                    sqlParameter.Value = Convert.ToInt32(parameter.Value);
                }
                else if (parameter.DataType == DataType.UnsignedInteger)
                {
                    sqlParameter.Value = Convert.ToInt64(parameter.Value);
                }
                else if (parameter.DataType == DataType.UnsignedLong)
                {
                    sqlParameter.Value = Convert.ToDecimal(parameter.Value);
                }
            }

            if (parameter.DataType == DataType.String && parameter.Value != null)
            {
                sqlParameter.Size = Convert.ToString(parameter.Value).Length;
            }
            if (parameter.DataType == DataType.Bytes && parameter.Value != null)
            {
                sqlParameter.Size = ((byte[])parameter.Value).Length;
            }
            if (parameter.DataType == DataType.Chars && parameter.Value != null)
            {
                sqlParameter.Size = ((char[])parameter.Value).Length;
            }
            //if (sqlParameter.SqlDbType == SqlDbType.Decimal)
            //{
            //    sqlParameter.Precision = 28;
            //    sqlParameter.Scale = 10;
            //}
            //else if (sqlParameter.SqlDbType == SqlDbType.Money)
            //{
            //    sqlParameter.Precision = 28;
            //    sqlParameter.Scale = 2;
            //}
            //else if (sqlParameter.SqlDbType == SqlDbType.SmallMoney)
            //{
            //    sqlParameter.Precision = 28;
            //    sqlParameter.Scale = 2;
            //}


            return sqlParameter;
        }

        private static SqliteType ToSqlDbType(DataType dataType)
        {
            switch (dataType)
            {
	            case DataType.Json:
				case DataType.String:
                    return SqliteType.Text;
                case DataType.Boolean:
                    return SqliteType.Integer;
                case DataType.Integer:
                    return SqliteType.Integer;
                case DataType.DateTime:
                    return SqliteType.Text;
                case DataType.Double:
                    return SqliteType.Real;
                case DataType.Float:
                    return SqliteType.Real;
                case DataType.Decimal:
                    return SqliteType.Real;
                case DataType.Byte:
                    return SqliteType.Integer;
                case DataType.Bytes:
                    return SqliteType.Blob;
                case DataType.Guid:
                    return SqliteType.Text;
                case DataType.DateTimeOffset:
                    return SqliteType.Text;
                case DataType.Time:
                    return SqliteType.Text;
                case DataType.Date:
                    return SqliteType.Text;
                case DataType.Long:
                    return SqliteType.Integer;
                case DataType.Short:
                    return SqliteType.Integer;
                case DataType.Char:
                    return SqliteType.Text ;
                case DataType.Chars:
                    return SqliteType.Blob;
                case DataType.Xml:
                    return SqliteType.Text;
                case DataType.ClrEnum:
                    return SqliteType.Integer;
                case DataType.SignedByte:
                    return SqliteType.Integer;
                case DataType.UnsignedShort:
                    return SqliteType.Integer;
                case DataType.UnsignedInteger:
                    return SqliteType.Integer;
                case DataType.UnsignedLong:
                    return SqliteType.Integer;
                case DataType.ClrType:
                    return SqliteType.Blob;
	            case DataType.Undefined:
                default:
                    throw new InvalidCastException($"Unsupported DataType with name '{dataType}'");
            }
            
        }

    }
}
