using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System.Data.Common;

namespace Atdi.CoreServices.DataLayer
{
    internal sealed class OracleDataEngine : LoggedObject, IDataEngine
    {
        private readonly IDataEngineConfig _engineConfig;
        private readonly IEngineSyntax _syntax;
        private DbTransaction _dbTransaction;
        private  DbConnection _sqlConnect;


        public OracleDataEngine(IDataEngineConfig engineConfig, ILogger logger) : base(logger)
        {
            this._engineConfig = engineConfig;
            this._syntax = new  OracleEngineSyntax();
        }

        public IDataEngineConfig Config => this._engineConfig;
        public IEngineSyntax Syntax => this._syntax;



        public void BeginTransaction()
        {
            using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.DataProcessing, this))
            {
                using (var executor = new OracleCommandExecuter(_engineConfig, this.Logger))
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




        public int ExecuteTransaction(EngineCommand command)
        {
            using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.DataProcessing, this))
            {
                using (var executor = new OracleCommandExecuter(_engineConfig, command, this.Logger, _sqlConnect))
                {
                    if (this._dbTransaction != null)
                    {
                        executor.SetTransactionToDbCommand(this._dbTransaction);
                    }
                    return executor.Execute();
                }
            }
        }

        public int Execute(EngineCommand command)
        {
            using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.DataProcessing, this))
            {
                using (var executor = new OracleCommandExecuter(_engineConfig, command, this.Logger))
                {
                    return executor.Execute();
                }
            }
        }

        public void Execute(EngineCommand command, Action<System.Data.IDataReader> handler)
        {
            using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.DataProcessing, this))
            {
                using (var executor = new OracleCommandExecuter(_engineConfig, command, this.Logger))
                {
                    executor.Execute(handler);
                }
            }
        }

        public object ExecuteScalar(EngineCommand command)
        {
            using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.DataProcessing, this))
            {
                using (var executor = new OracleCommandExecuter(_engineConfig, command, this.Logger))
                {
                    return executor.ExecuteScalar();
                }
            }
        }
     
    }
}
