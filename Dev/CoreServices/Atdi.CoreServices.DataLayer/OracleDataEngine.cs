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
    class OracleDataEngine : LoggedObject, IDataEngine
    {
        private readonly IDataEngineConfig _engineConfig;

        public OracleDataEngine(IDataEngineConfig engineConfig, ILogger logger) : base(logger)
        {
            this._engineConfig = engineConfig;
        }

        public IDataEngineConfig Config => this._engineConfig;

        public void Execute(EngineCommand command, Action<IDataReader> handler)
        {
            throw new NotImplementedException();
        }

        public int Execute(EngineCommand command)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(EngineCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
