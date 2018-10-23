using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System.Data;

namespace Atdi.CoreServices.DataLayer
{
    internal sealed class OracleDataEngine : LoggedObject, IDataEngine
    {
        private readonly IDataEngineConfig _engineConfig;
        private readonly IEngineSyntax _syntax;
       

        public OracleDataEngine(IDataEngineConfig engineConfig, ILogger logger) : base(logger)
        {
            this._engineConfig = engineConfig;
            this._syntax = new  OracleEngineSyntax();
        }

        public IDataEngineConfig Config => this._engineConfig;
        public IEngineSyntax Syntax => this._syntax;

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
