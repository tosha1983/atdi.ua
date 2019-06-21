using Atdi.Common;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.SqlServer
{
    internal sealed class SqlServerExecuter : LoggedObject
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public SqlServerExecuter(SqlConnection connection, SqlTransaction transaction, ILogger logger) : base(logger)
        {
            this._connection = connection;
            this._transaction = transaction;
            logger.Verbouse(Contexts.SqlServerEngine, Categories.Creation, Events.ObjectWasCreated.With("SqlServerExecuter"));
        }

        private SqlCommand CreatCommand(EngineCommand engineCommand)
        {
            using (var traceScope = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.CommandCreation, this))
            {
                var sqlCommand = _connection.CreateCommand();
                sqlCommand.CommandText = engineCommand.Text;
                sqlCommand.CommandType = CommandType.Text;
                if (_transaction != null)
                {
                    sqlCommand.Transaction = _transaction;
                }
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
                        //this.EnsureOutputParameters();
                        return result;
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.SqlServerEngine, Categories.Processing, e, this);
                    throw new InvalidOperationException(Exceptions.FailedToExecuteNonQuery, e);
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
                        //this.EnsureOutputParameters();
                        return result;
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.SqlServerEngine, Categories.Executing, e, this);
                    throw new InvalidOperationException(Exceptions.FailedToExecuteScalarQuery, e);
                }
            }
        }

        public T ExecuteReader<T>(EngineCommand engineCommand, Func<SqlDataReader, T> handler)
        {
            using (var sqlCommand = this.CreatCommand(engineCommand))
            {
                T result = default(T);
                SqlDataReader reader = null;
                try
                {
                   
                    using (var trace = StartTraceExecuting(sqlCommand))
                    {
                        reader = sqlCommand.ExecuteReader();
                    }
                    //this.EnsureOutputParameters();
                    using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.ResultHandling, this))
                    {
                        result = handler(reader);
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.SqlServerEngine, Categories.Executing, e, this);
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

        public void ExecuteReader(EngineCommand engineCommand, Action<SqlDataReader> handler)
        {
            using (var sqlCommand = this.CreatCommand(engineCommand))
            {
                SqlDataReader reader = null;
                try
                {

                    using (var trace = StartTraceExecuting(sqlCommand))
                    {
                        reader = sqlCommand.ExecuteReader();
                    }
                    //this.EnsureOutputParameters();
                    using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.ResultHandling, this))
                    {
                        handler(reader);
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.SqlServerEngine, Categories.Executing, e, this);
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
        private ITraceScope StartTraceExecuting(SqlCommand sqlCommand)
        {
            var traceScope = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.CommandExecuting, this);
            traceScope.SetData("SQL Statement", Environment.NewLine + sqlCommand.CommandText);
            return traceScope;
        }


        private void PrepareParameters(SqlCommand sqlCommand, ICollection<EngineCommandParameter> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    sqlCommand.Parameters.Add(this.CreateSqlParameter(parameter));
                }
            }
        }

        private SqlParameter CreateSqlParameter(EngineCommandParameter parameter)
        {
            // все типы из DataType должныт мапинться или конвертироваться
            var sqlParameter = new SqlParameter
            {
                ParameterName = "@" + parameter.Name,
                SqlDbType = ToSqlDbType(parameter.DataType),
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

        private static SqlDbType ToSqlDbType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.String:
                    return SqlDbType.NVarChar;
                case DataType.Boolean:
                    return SqlDbType.Bit;
                case DataType.Integer:
                    return SqlDbType.Int;
                case DataType.DateTime:
                    return SqlDbType.DateTime;
                case DataType.Double:
                    return SqlDbType.Float;
                case DataType.Float:
                    return SqlDbType.Float;
                case DataType.Decimal:
                    return SqlDbType.Decimal;
                case DataType.Byte:
                    return SqlDbType.TinyInt;
                case DataType.Bytes:
                    return SqlDbType.VarBinary;
                case DataType.Guid:
                    return SqlDbType.UniqueIdentifier;
                case DataType.DateTimeOffset:
                    return SqlDbType.DateTimeOffset;
                case DataType.Time:
                    return SqlDbType.Time;
                case DataType.Date:
                    return SqlDbType.Date;
                case DataType.Long:
                    return SqlDbType.BigInt;
                case DataType.Short:
                    return SqlDbType.SmallInt;
                case DataType.Char:
                    return SqlDbType.NChar;
                case DataType.Chars:
                    return SqlDbType.NChar;
                case DataType.Xml:
                    return SqlDbType.Xml;
                case DataType.ClrEnum:
                    return SqlDbType.Int;
                case DataType.SignedByte:
                    return SqlDbType.SmallInt;
                case DataType.UnsignedShort:
                    return SqlDbType.Int;
                case DataType.UnsignedInteger:
                    return SqlDbType.BigInt;
                case DataType.UnsignedLong:
                    return SqlDbType.Decimal;
                case DataType.ClrType:
                    return SqlDbType.VarBinary;
                case DataType.Json:
                case DataType.Undefined:
                default:
                    throw new InvalidCastException($"Unsupported DataType with name '{dataType}'");
            }
            
        }

    }
}
