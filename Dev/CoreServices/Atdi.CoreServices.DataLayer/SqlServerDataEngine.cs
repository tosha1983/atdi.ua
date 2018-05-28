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

namespace Atdi.CoreServices.DataLayer
{
    class SqlServerDataEngine : LoggedObject, IDataEngine
    {
        private readonly IDataEngineConfig _engineConfig;
        private readonly IEngineSyntax _syntax;
        public SqlServerDataEngine(IDataEngineConfig engineConfig, ILogger logger) : base(logger)
        {
            this._engineConfig = engineConfig;
            this._syntax = new SqlServerEngineSyntax();
        }

        public IDataEngineConfig Config => this._engineConfig;

        public IEngineSyntax Syntax => this._syntax;

        public int Execute(EngineCommand command)
        {
            using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.DataProcessing, this))
            {
                using (var executor = new SqlServerCommandExecuter(_engineConfig, command, this.Logger))
                {
                    return executor.Execute();
                }
            }
        }

        public void Execute(EngineCommand command, Action<IDataReader> handler)
        {
            using (var trace = this.Logger.StartTrace(Contexts.SqlServerEngine, Categories.DataProcessing, this))
            {
                using (var executor = new SqlServerCommandExecuter(_engineConfig, command, this.Logger))
                {
                    executor.Execute(handler);
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
