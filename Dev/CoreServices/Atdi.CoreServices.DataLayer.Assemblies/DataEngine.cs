using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.DataLayer.DataEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.Assemblies
{
    class DataEngine : IAssembliesDataEngine
    {
        private readonly EngineExecuter _engineExecuter;

        public DataEngine(EngineExecuter engineExecuter)
        {
            this._engineExecuter = engineExecuter;
        }

        public IDataEngineConfig Config { get; set; }

        public IEngineSyntax Syntax => throw new NotImplementedException();

        public IEngineExecuter CreateExecuter()
        {
            return _engineExecuter;
        }

        public void SetConfig(IDataEngineConfig engineConfig)
        {
            this.Config = engineConfig;
        }
    }
}
