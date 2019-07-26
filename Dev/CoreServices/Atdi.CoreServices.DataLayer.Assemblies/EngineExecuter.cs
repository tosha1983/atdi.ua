using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.Assemblies
{
    class EngineExecuter : LoggedObject, IEngineExecuter
    {
        private readonly ServiceObjectResolver _resolver;
        private readonly QueryPatternFactory _patternFactory;

        public EngineExecuter(ServiceObjectResolver resolver, QueryPatternFactory patternFactory, ILogger logger) : base(logger)
        {
            this._resolver = resolver;
            this._patternFactory = patternFactory;
            logger.Verbouse(Contexts.AssembliesEngine, Categories.Creation, Events.ObjectWasCreated.With("EngineExecuter"));
        }

        public TransactionIsolationLevel IsolationLevel => throw new NotImplementedException();

        public bool HasTransaction => throw new NotImplementedException();

        public void BeginTran(TransactionIsolationLevel isoLevel = TransactionIsolationLevel.Default)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            ;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        void IEngineExecuter.Execute<TPattern>(TPattern queryPattern)
        {
            try
            {
                var patternHandler = this._patternFactory.GetHandler(typeof(TPattern));
                patternHandler.Handle(queryPattern, this._resolver);
            }
            catch (Exception e)
            {
                Logger.Exception(Contexts.AssembliesEngine, Categories.Processing, e, this);
                throw new InvalidOperationException(Exceptions.FailedToExecuteQueryPattern.With(typeof(TPattern).FullName), e);
            }
        }
    }
}
