using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System.Data;
using System.Data.SqlClient;
using Atdi.DataModels;
using System.Runtime.CompilerServices;

namespace Atdi.CoreServices.DataLayer
{
    internal sealed class SqlServerCommandExecuter : LoggedObject, IDisposable
    {
        private SqlConnection _connection;
        private SqlCommand _command;

        public SqlServerCommandExecuter(IDataEngineConfig _engineConfig, EngineCommand engineCommand, ILogger logger) 
            : base(logger)
        {
            this._connection = new SqlConnection(_engineConfig.ConnectionString);
            this._command = new SqlCommand
            {
                Connection = this._connection,
                CommandType = CommandType.Text,
                CommandText = engineCommand.Text
            };

            this.PrepareCommandParameters(engineCommand.Parameters.Values);
        }

        private void PrepareCommandParameters(ICollection<EngineCommandParameter> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    this._command.Parameters.Add(this.CreateSqlParameter(parameter));
                }
            }
        }

        private SqlParameter CreateSqlParameter(EngineCommandParameter parameter)
        {
            var sqlParameter = new SqlParameter
            {
                ParameterName = "@" + parameter.Name,
                SqlDbType = ToSqlDbType(parameter.DataType),
                Value = parameter.Value ?? DBNull.Value
            };

            if (parameter.DataType == DataType.String && parameter.Value != null)
            {
                sqlParameter.Size = Convert.ToString(parameter.Value).Length;
            }
            if (parameter.DataType == DataType.Bytes && parameter.Value != null)
            {
                sqlParameter.Size = ((byte[])parameter.Value).Length;
            }
            if (parameter.DataType == DataType.Decimal && parameter.Value != null)
            {
                sqlParameter.Precision = 28;
                sqlParameter.Scale = 10;
            }

            return sqlParameter;
        }

        private static SqlDbType ToSqlDbType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Guid:
                    return SqlDbType.UniqueIdentifier;
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
                default:
                    throw new InvalidCastException();
            }
        }

        public void Dispose()
        {
            if (_command != null)
            {
                _command.Dispose();
                _command = null;
            }
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public int Execute()
        {
            this.OpenConnection();
            try
            {
                using (var trace = StartTraceExecuting())
                {
                    TraceCommand(trace);
                    return _command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.SqlServerEngine, Categories.DataProcessing, e, this);
                throw;
            }
        }

        

        public void Execute(Action<System.Data.IDataReader> handler)
        {
            this.OpenConnection();
            System.Data.IDataReader reader = null;
            try
            {

                using (var trace = StartTraceExecuting())
                {
                    TraceCommand(trace);
                    reader = this._command.ExecuteReader();
                }

                using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.ResultHandling, this))
                {
                    handler(reader);
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.SqlServerEngine, Categories.DataProcessing, e, this);
                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
        }

        public object ExecuteScalar()
        {
            this.OpenConnection();
            try
            {
                using (var trace = StartTraceExecuting())
                {
                    TraceCommand(trace);
                    return _command.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.SqlServerEngine, Categories.DataProcessing, e, this);
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OpenConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                return;
            }

            try
            {
                using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.OpeningConnection, this))
                {
                    trace.SetData("Connection string", _connection.ConnectionString);
                    _connection.Open();
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.SqlServerEngine, Categories.DataProcessing, e, this);
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ITraceScope StartTraceExecuting()
        {
            return this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.CommandExecuting, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TraceCommand(ITraceScope trace)
        {
            trace.SetData("SQL Statement", Environment.NewLine + _command.CommandText);
        }
    }
}
