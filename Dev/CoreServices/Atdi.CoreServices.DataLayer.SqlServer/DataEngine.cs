using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.DataLayer.DataEngines;
using Atdi.Platform;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.SqlServer
{
    internal sealed class DataEngine : LoggedObject, ISqlServerDataEngine
    {
        private readonly QueryPatternFactory _patternFactory;
        private readonly IStatistics _statistics;

        public DataEngine(QueryPatternFactory patternFactory, EngineSyntax engineSyntax, IStatistics statistics, ILogger logger) : base(logger)
        {
            this._patternFactory = patternFactory;
            this.Syntax = engineSyntax;
            this._statistics = statistics;
            logger.Verbouse(Contexts.SqlServerEngine, Categories.Creation, Events.ObjectWasCreated.With("DataEngine"));
        }

        public IDataEngineConfig Config { get; set; }

        public IEngineSyntax Syntax { get; set; }

        public IEngineExecuter CreateExecuter()
        {
            try
            {
                var executer = new EngineExecuter(this._patternFactory, this.Config, this._statistics, this.Logger);
                return executer;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.SqlServerEngine, Categories.Processing, e, this);
                throw new InvalidOperationException(Exceptions.FailedToCreateEngineExecuter, e);
            }
            
        }

        public void SetConfig(IDataEngineConfig engineConfig)
        {
            this.Config = engineConfig;
        }
    }
}
