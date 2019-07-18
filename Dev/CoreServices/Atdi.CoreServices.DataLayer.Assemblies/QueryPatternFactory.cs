using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.DependencyInjection;

namespace Atdi.CoreServices.DataLayer.Assemblies
{
    internal sealed class QueryPatternFactory : LoggedObject
    {
        private readonly Dictionary<Type, IQueryPatternHandler> _handlers;
        

        public QueryPatternFactory(IServicesResolver servicesResolver, ILogger logger) : base(logger)
        {
            
            this._handlers = new Dictionary<Type, IQueryPatternHandler>();
            this.FindHandlersInCurrentAssembly();
            logger.Verbouse(Contexts.AssembliesEngine, Categories.Creation, Events.ObjectWasCreated.With("QueryPatternFactory"));
        }

        private void FindHandlersInCurrentAssembly()
        {

            _handlers.Add(typeof(PS.SelectPattern), new PatternHandlers.SelectPatternHandler(this.Logger));
            
        }

        public IQueryPatternHandler GetHandler(Type handlerType)
        {
            return _handlers[handlerType];
        }
    }
}
