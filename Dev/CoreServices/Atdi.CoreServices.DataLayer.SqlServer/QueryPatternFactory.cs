using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.CoreServices.DataLayer.SqlServer
{
    internal sealed class QueryPatternFactory : LoggedObject
    {
        private readonly Dictionary<Type, IQueryPatternHandler> _handlers;

        public QueryPatternFactory(ILogger logger) : base(logger)
        {
            this._handlers = new Dictionary<Type, IQueryPatternHandler>();
            this.FindHandlersInCurrentAssembly();
            logger.Verbouse(Contexts.SqlServerEngine, Categories.Creation, Events.ObjectWasCreated.With("QueryPatternFactory"));
        }

        private void FindHandlersInCurrentAssembly()
        {
            _handlers.Add(typeof(PS.EngineCommandPattern), new PatternHandlers.EngineCommandPatternHandler(this.Logger));
            _handlers.Add(typeof(PS.InsertPattern), new PatternHandlers.InsertPatternHandler(this.Logger));
            _handlers.Add(typeof(PS.SelectPattern), new PatternHandlers.SelectPatternHandler(this.Logger));
        }

        public IQueryPatternHandler GetHandler(Type handlerType)
        {
            return _handlers[handlerType];
        }
    }
}
