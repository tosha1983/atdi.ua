using Atdi.Common;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Atdi.CoreServices.DataLayer.Oracle
{
    internal sealed class OracleExecuter : LoggedObject
    {
        private OracleConnection _connection;
        private OracleTransaction _transaction;

        public OracleExecuter(OracleConnection connection, OracleTransaction transaction, ILogger logger) : base(logger)
        {
            this._connection = connection;
            this._transaction = transaction;
            logger.Verbouse(Contexts.OracleEngine, Categories.Creation, Events.ObjectWasCreated.With("OracleExecuter"));
        }

        private OracleCommand CreatCommand(EngineCommand engineCommand)
        {
            using (var traceScope = this.Logger.StartTrace(Contexts.OracleEngine, Categories.CommandCreation, this))
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
                        this.EnsureOutputParameters(sqlCommand, engineCommand);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.OracleEngine, Categories.Processing, e, this);
                    throw new InvalidOperationException(Exceptions.FailedToExecuteNonQuery, e);
                }
            }
        }

        private void EnsureOutputParameters(OracleCommand sqlCommand, EngineCommand engineCommand)
        {
            for (int i = 0; i < sqlCommand.Parameters.Count; i++)
            {
                var sqlParameter = sqlCommand.Parameters[i];
                if (sqlParameter.Direction == ParameterDirection.Output
                    || sqlParameter.Direction == ParameterDirection.InputOutput)
                {
                    var key = sqlParameter.ParameterName.Substring(1, sqlParameter.ParameterName.Length - 1);
                    if (sqlParameter.OracleDbType == OracleDbType.Int64)
                    {
                        Int64 valInt64 = Convert.ToInt64(sqlParameter.Value.ToString());
                        sqlParameter.Value = valInt64;
                    }
                    else if (sqlParameter.OracleDbType == OracleDbType.Int32)
                    {
                        Int32 valInt32 = Convert.ToInt32(sqlParameter.Value.ToString());
                        sqlParameter.Value = valInt32;
                    }
                    else if (sqlParameter.OracleDbType == OracleDbType.Int16)
                    {
                        Int32 valInt16 = Convert.ToInt16(sqlParameter.Value.ToString());
                        sqlParameter.Value = valInt16;
                    }
                    else if (sqlParameter.OracleDbType == OracleDbType.Blob)
                    {
                        var bt = sqlParameter.Value;
                        var conv = (OracleBlob)bt;
                        Guid valGuid = new Guid(conv.Value);
                        sqlParameter.Value = valGuid;
                    }
                    else if(sqlParameter.OracleDbType == OracleDbType.TimeStampTZ)
                    {
                        var val = (OracleTimeStampTZ)sqlParameter.Value;
                        var oracleTimeStamp = val.ToOracleTimeStamp();
                        sqlParameter.Value = new DateTimeOffset(oracleTimeStamp.Value, val.GetTimeZoneOffset());
                    }
                    //else if (sqlParameter.OracleDbType == OracleDbType.gu)
                    //{
                    //Guid valGuid = new Guid(sqlParameter.Value.ToString());
                    //sqlParameter.Value = valGuid;
                    //}
                    engineCommand.Parameters[key].Value = sqlParameter.Value;
                    if (sqlParameter.OracleDbType== OracleDbType.RefCursor)
                    {
                        sqlParameter.Dispose();
                    }
                    
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
                    this.Logger.Exception(Contexts.OracleEngine, Categories.Executing, e, this);
                    throw new InvalidOperationException(Exceptions.FailedToExecuteScalarQuery, e);
                }
            }
        }

        public T ExecuteReader<T>(EngineCommand engineCommand, Func<OracleDataReader, T> handler)
        {
            using (var sqlCommand = this.CreatCommand(engineCommand))
            {
                T result = default(T);
                OracleDataReader reader = null;
                try
                {

                    using (var trace = StartTraceExecuting(sqlCommand))
                    {
                        reader = sqlCommand.ExecuteReader();
                        this.EnsureOutputParameters(sqlCommand, engineCommand);
                    }
                    //this.EnsureOutputParameters();
                    using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.ResultHandling, this))
                    {
                        result = handler(reader);
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.OracleEngine, Categories.Executing, e, this);
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

        public void ExecuteReader(EngineCommand engineCommand, Action<OracleDataReader> handler)
        {
            using (var sqlCommand = this.CreatCommand(engineCommand))
            {
                OracleDataReader reader = null;
                try
                {

                    using (var trace = StartTraceExecuting(sqlCommand))
                    {
                        reader = sqlCommand.ExecuteReader();
                        this.EnsureOutputParameters(sqlCommand, engineCommand);
                    }

                    using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.ResultHandling, this))
                    {
                        handler(reader);
                    }
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.OracleEngine, Categories.Executing, e, this);
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
        private ITraceScope StartTraceExecuting(OracleCommand sqlCommand)
        {
            var traceScope = this.Logger.StartTrace(Contexts.OracleEngine, Categories.CommandExecuting, this);
            traceScope.SetData("SQL Statement", Environment.NewLine + sqlCommand.CommandText);
            return traceScope;
        }


        private void PrepareParameters(OracleCommand sqlCommand, ICollection<EngineCommandParameter> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    sqlCommand.Parameters.Add(this.CreateSqlParameter(parameter));
                }
            }
        }

        private OracleParameter CreateSqlParameter(EngineCommandParameter parameter)
        {
           
            // все типы из DataType должныт мапинться или конвертироваться
            var sqlParameter = new OracleParameter
            {
                ParameterName = ":" + parameter.Name,
                OracleDbType = parameter.Name.Contains("_CURSOR_REF")==false ?  ToSqlDbType(parameter.DataType) : OracleDbType.RefCursor,
                Value = parameter.Value ?? DBNull.Value,
                Direction = parameter.Direction.CopyTo<ParameterDirection>()
            };

            if (parameter.DataType == DataType.Guid)
            {
                sqlParameter.OracleDbType = OracleDbType.Blob;
            }
            

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

            return sqlParameter;
        }

        private static OracleDbType ToSqlDbType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.String:
                    return OracleDbType.NVarchar2;
                case DataType.Boolean:
                    return OracleDbType.Int16;
                case DataType.Integer:
                    return OracleDbType.Int32;
                case DataType.DateTime:
                    return OracleDbType.Date;
                case DataType.Double:
                    return OracleDbType.BinaryDouble;
                case DataType.Float:
                    return OracleDbType.BinaryFloat;
                case DataType.Decimal:
                    return OracleDbType.Decimal;
                case DataType.Byte:
                    return OracleDbType.Byte;
                case DataType.Bytes:
                    return OracleDbType.Blob;
                case DataType.Guid:
                    return OracleDbType.Raw;
                case DataType.DateTimeOffset:
                    return OracleDbType.TimeStampTZ;
                case DataType.Time:
                    return OracleDbType.IntervalDS;
                case DataType.Date:
                    return OracleDbType.Date;
                case DataType.Long:
                    return OracleDbType.Int64;
                case DataType.Short:
                    return OracleDbType.Int16;
                case DataType.Char:
                    return OracleDbType.NChar;
                case DataType.Chars:
                    return OracleDbType.NChar;
                case DataType.Xml:
                    return OracleDbType.NClob;
                case DataType.ClrEnum:
                    return OracleDbType.Int32;
                case DataType.SignedByte:
                    return OracleDbType.Int16;
                case DataType.UnsignedShort:
                    return OracleDbType.Int32;
                case DataType.UnsignedInteger:
                    return OracleDbType.Int64;
                case DataType.UnsignedLong:
                    return OracleDbType.Decimal;
                case DataType.ClrType:
                    return OracleDbType.Blob;
                case DataType.Undefined:
                case DataType.Json:
                default:
                    throw new InvalidCastException($"Unsupported DataType with name '{dataType}'");
            }

        }

    }
}
