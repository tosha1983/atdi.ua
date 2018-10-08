using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Atdi.DataModels;
using System.Runtime.CompilerServices;


namespace Atdi.CoreServices.DataLayer
{
    internal sealed class OracleCommandExecuter : LoggedObject, IDisposable
    {
        private DbConnection _connection;
        private DbCommand _command;

        public OracleCommandExecuter(IDataEngineConfig _engineConfig, EngineCommand engineCommand, ILogger logger) 
            : base(logger)
        {
            DbProviderFactory _dbProviderFactory = DbProviderFactories.GetFactory("Oracle.ManagedDataAccess.dll");
            this._connection =  _dbProviderFactory.CreateConnection();
            this._connection.ConnectionString = _engineConfig.ConnectionString;
            this._connection.Open();
            this._command = this._connection.CreateCommand();
            this._command.Connection = this._connection;
            this._command.CommandText = engineCommand.Text;
            this._command.CommandType = CommandType.Text;
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

        private DbParameter CreateSqlParameter(EngineCommandParameter parameter)
        {
            DbParameter sqlParameter = this._command.CreateParameter();
            sqlParameter.ParameterName = ":" + parameter.Name;
            sqlParameter.DbType = ToSqlDbType(parameter.DataType);
            sqlParameter.Direction = ParameterDirection.Input;
            sqlParameter.Value = parameter.Value ?? DBNull.Value;

            if (parameter.DataType == DataType.String && parameter.Value != null) {
                sqlParameter.Size = Convert.ToString(parameter.Value).Length;
            }
            if (parameter.DataType == DataType.Bytes && parameter.Value != null) {
                sqlParameter.Size = ((byte[])parameter.Value).Length;
            }
            if (parameter.DataType == DataType.Decimal && parameter.Value != null) {
                sqlParameter.Precision = 28;
                sqlParameter.Scale = 10;
            }

            return sqlParameter;
        }

        private static DbType ToSqlDbType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Guid:
                    return DbType.String;
                case DataType.String:
                    return DbType.String;
                case DataType.Boolean:
                    return DbType.Int32;
                case DataType.Integer:
                    return DbType.Int32;
                case DataType.DateTime:
                    return DbType.DateTime;
                case DataType.Double:
                    return DbType.Double;
                case DataType.Float:
                    return DbType.Single;
                case DataType.Decimal:
                    return DbType.Decimal;
                case DataType.Byte:
                    return DbType.String;
                case DataType.Bytes:
                    return DbType.Binary;
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
                    return this._command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.OracleEngine, Categories.DataProcessing, e, this);
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

                using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.ResultHandling, this))
                {
                    handler(reader);
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.OracleEngine, Categories.DataProcessing, e, this);
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
                    return this._command.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.OracleEngine, Categories.DataProcessing, e, this);
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
                using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.OpeningConnection, this))
                {
                    trace.SetData("Connection string", _connection.ConnectionString);
                    _connection.Open();
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.OracleEngine, Categories.DataProcessing, e, this);
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ITraceScope StartTraceExecuting()
        {
            return this.Logger.StartTrace(Contexts.OracleEngine, Categories.CommandExecuting, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TraceCommand(ITraceScope trace)
        {
            trace.SetData("SQL Statement", Environment.NewLine + _command.CommandText);
        }
    }
}
