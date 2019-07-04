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
        private readonly OracleQueryPatternFactory _patternFactory;
        public OracleDataEngine(OracleQueryPatternFactory patternFactory, OracleEngineSyntax engineSyntax, ILogger logger) : base(logger)
        {
            this._patternFactory = patternFactory;
            this.Syntax = engineSyntax;
            logger.Verbouse(Contexts.OracleEngine, Categories.Creation, Events.ObjectWasCreated.With("OracleDataEngine"));
        }

        public IDataEngineConfig Config { get; set; }

        public IEngineSyntax Syntax { get; set; }

        public IEngineExecuter CreateExecuter()
        {
            try
            {
                var executer = new EngineExecuter(this._patternFactory, this.Config, this.Logger);
                return executer;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.OracleEngine, Categories.Processing, e, this);
                throw new InvalidOperationException(Exceptions.FailedToCreateEngineExecuter, e);
            }

        }

        public void SetConfig(IDataEngineConfig engineConfig)
        {
            this.Config = engineConfig;
        }
    }
}
