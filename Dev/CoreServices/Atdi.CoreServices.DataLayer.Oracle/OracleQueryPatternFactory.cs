using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.CoreServices.DataLayer.Oracle
{
    internal sealed class OracleQueryPatternFactory : LoggedObject
    {
        private readonly Dictionary<Type, IOracleQueryPatternHandler> _handlers;

        public OracleQueryPatternFactory(ILogger logger) : base(logger)
        {
            this._handlers = new Dictionary<Type, IOracleQueryPatternHandler>();
            this.FindHandlersInCurrentAssembly();
            logger.Verbouse(Contexts.OracleEngine, Categories.Creation, Events.ObjectWasCreated.With("OracleQueryPatternFactory"));
        }

        private void FindHandlersInCurrentAssembly()
        {
            _handlers.Add(typeof(PS.EngineCommandPattern), new PatternHandlers.EngineCommandPatternHandler(this.Logger));
            _handlers.Add(typeof(PS.InsertPattern), new PatternHandlers.OracleInsertPatternHandler(this.Logger));
            _handlers.Add(typeof(PS.SelectPattern), new PatternHandlers.OracleSelectPatternHandler(this.Logger));
        }

        public IOracleQueryPatternHandler GetHandler(Type handlerType)
        {
            return _handlers[handlerType];
        }
    }
}
