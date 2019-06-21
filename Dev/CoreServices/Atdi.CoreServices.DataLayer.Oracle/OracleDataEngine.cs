using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System.Data.Common;
using Atdi.Contracts.CoreServices.DataLayer.DataEngines;

namespace Atdi.CoreServices.DataLayer.Oracle
{
    internal sealed class OracleDataEngine : LoggedObject, IOracleDataEngine
    {
        public OracleDataEngine(ILogger logger) : base(logger)
        {
        }

        //private IDataEngineConfig _engineConfig;
        //private readonly IEngineSyntax _syntax;
        //private  DbTransaction _dbTransaction;
        //private  DbConnection _sqlConnect;


        //public OracleDataEngine(ILogger logger) : base(logger)
        //{
        //    this._syntax = new  OracleEngineSyntax();
        //}

        //public IDataEngineConfig Config => this._engineConfig;

        //public IEngineSyntax Syntax => this._syntax;


        //public void BeginTransaction()
        //{
        //    using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.DataProcessing, this))
        //    {
        //        using (var executor = new OracleCommandExecuter(_engineConfig, this.Logger))
        //        {
        //            _sqlConnect = executor.GetConnection();
        //            this._dbTransaction = _sqlConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        //        }
        //    }
        //}

        //public void CommitTransaction()
        //{
        //    this._dbTransaction.Commit();
        //    this._dbTransaction.Dispose();
        //    this._dbTransaction = null;
        //    if (_sqlConnect != null)
        //    {
        //        _sqlConnect.Close();
        //        _sqlConnect = null;
        //    }
        //}

        //public void RollbackTransaction()
        //{
        //    this._dbTransaction.Rollback();
        //    this._dbTransaction.Dispose();
        //    this._dbTransaction = null;
        //    if (_sqlConnect != null)
        //    {
        //        _sqlConnect.Close();
        //        _sqlConnect = null;
        //    }
        //}



        //public int Execute(EngineCommand command)
        //{
        //    using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.DataProcessing, this))
        //    {
        //        if (this._dbTransaction == null)
        //        {
        //            using (var executor = new OracleCommandExecuter(_engineConfig, command, this.Logger))
        //            {
        //                return executor.Execute();
        //            }
        //        }
        //        else
        //        {
        //            using (var executor = new OracleCommandExecuter(_engineConfig, command, this.Logger, _sqlConnect))
        //            {
        //                if (this._dbTransaction != null)
        //                {
        //                    executor.SetTransactionToDbCommand(this._dbTransaction);
        //                }
        //                return executor.Execute();
        //            }
        //        }
        //    }
        //}

        //public void Execute(EngineCommand command, Action<System.Data.IDataReader> handler)
        //{
        //    using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.DataProcessing, this))
        //    {
        //        if (this._dbTransaction == null)
        //        {
        //            using (var executor = new OracleCommandExecuter(_engineConfig, command, this.Logger))
        //            {
        //                executor.Execute(handler);
        //            }
        //        }
        //        else
        //        {
        //            using (var executor = new OracleCommandExecuter(_engineConfig, command, this.Logger, _sqlConnect))
        //            {
        //                if (this._dbTransaction != null)
        //                {
        //                    executor.SetTransactionToDbCommand(this._dbTransaction);
        //                }
        //                executor.Execute(handler);
        //            }
        //        }
        //    }
        //}

        //public object ExecuteScalar(EngineCommand command)
        //{
        //    using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.DataProcessing, this))
        //    {
        //        using (var executor = new OracleCommandExecuter(_engineConfig, command, this.Logger))
        //        {
        //            return executor.ExecuteScalar();
        //        }
        //    }
        //}

        //public void Execute(IEngineQueryPattern queryPattern, Action<IEngineDataReader> handler)
        //{

        //}

        //public int Execute(IEngineQueryPattern queryPattern)
        //{
        //    return 0;
        //}

        //public object ExecuteScalar(IEngineQueryPattern queryPattern)
        //{
        //    return null;
        //}

        //public void SetConfig(IDataEngineConfig engineConfig)
        //{
        //    this._engineConfig = engineConfig;
        //}

        public IDataEngineConfig Config => throw new NotImplementedException();

        public IEngineSyntax Syntax => throw new NotImplementedException();

        public IEngineExecuter CreateExecuter()
        {
            throw new NotImplementedException();
        }

        public void SetConfig(IDataEngineConfig engineConfig)
        {
            throw new NotImplementedException();
        }
    }
}
