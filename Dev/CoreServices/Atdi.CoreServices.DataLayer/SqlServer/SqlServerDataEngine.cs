using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System.Data.SqlClient;

namespace Atdi.CoreServices.DataLayer
{
    internal sealed class SqlServerDataEngine : LoggedObject, IDataEngine
    {
        private readonly IDataEngineConfig _engineConfig;
        private readonly IEngineSyntax _syntax;
        private SqlTransaction _dbTransaction;
        private SqlConnection _sqlConnect;


        public SqlServerDataEngine(IDataEngineConfig engineConfig, ILogger logger) : base(logger)
        {
            this._engineConfig = engineConfig;
            this._syntax = new SqlServerEngineSyntax();
        }

        public IDataEngineConfig Config => this._engineConfig;

        public IEngineSyntax Syntax => this._syntax;

        public void BeginTransaction()
        {
            using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.DataProcessing, this))
            {
                using (var executor = new SqlServerCommandExecuter(_engineConfig,  this.Logger))
                {
                    _sqlConnect = executor.GetConnection();
                    this._dbTransaction = _sqlConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                }
            }
        }

        public void CommitTransaction()
        {
            this._dbTransaction.Commit();
            this._dbTransaction.Dispose();
            this._dbTransaction = null;
            if (_sqlConnect != null)
            {
                _sqlConnect.Close();
                _sqlConnect = null;
            }
        }

        public void RollbackTransaction()
        {
            this._dbTransaction.Rollback();
            this._dbTransaction.Dispose();
            this._dbTransaction = null;
            if (_sqlConnect != null)
            {
                _sqlConnect.Close();
                _sqlConnect = null;
            }
        }



        public int Execute(EngineCommand command)
        {
            if (this._dbTransaction == null)
            {
                using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.DataProcessing, this))
                {
                    using (var executor = new SqlServerCommandExecuter(_engineConfig, command, this.Logger))
                    {
                        return executor.Execute();
                    }
                }
            }
            else
            {
                using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.DataProcessing, this))
                {
                    using (var executor = new SqlServerCommandExecuter(_engineConfig, command, this.Logger, _sqlConnect))
                    {
                        if (this._dbTransaction != null)
                        {
                            executor.SetTransactionToDbCommand(this._dbTransaction);
                        }
                        return executor.Execute();
                    }
                }
            }
        }


        public void Execute(EngineCommand command, Action<System.Data.IDataReader> handler)
        {
            if (this._dbTransaction == null)
            {
                using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.DataProcessing, this))
                {
                    using (var executor = new SqlServerCommandExecuter(_engineConfig, command, this.Logger))
                    {
                        executor.Execute(handler);
                    }
                }
            }
            else
            {
                using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.DataProcessing, this))
                {
                    using (var executor = new SqlServerCommandExecuter(_engineConfig, command, this.Logger, _sqlConnect))
                    {
                        if (this._dbTransaction != null)
                        {
                            executor.SetTransactionToDbCommand(this._dbTransaction);
                        }
                        executor.Execute(handler);
                    }
                }
            }
        }

        public object ExecuteScalar(EngineCommand command)
        {
            using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.DataProcessing, this))
            {
                using (var executor = new SqlServerCommandExecuter(_engineConfig, command, this.Logger))
                {
                    return executor.ExecuteScalar();
                }
            }
        }
    }
}
