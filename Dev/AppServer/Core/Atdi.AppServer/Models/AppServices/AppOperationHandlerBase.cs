using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public abstract class AppOperationHandlerBase<TService, TOperation, TOptions, TResult> : LoggedObject, IAppOperationHandler<TService, TOperation, TOptions, TResult>
        where TService : class, IAppService, new()
        where TOperation : class, IAppOperation, new()
        where TOptions : class, IAppOperationOptions, new()
    {
        private readonly IAppServerContext _serverContext;
        private readonly Lazy<TService> _lazyService;
        private readonly Lazy<TOperation> _lazyOperation;

        public AppOperationHandlerBase(IAppServerContext serverContext, ILogger logger) : base(logger)
        {
            this._lazyService = new Lazy<TService>(() => new TService());
            this._lazyOperation = new Lazy<TOperation>(() => new TOperation());
            this._serverContext = serverContext;
        }

        protected IAppServerContext ServerContext => this._serverContext;

        public TService Service => this._lazyService.Value;

        public TOperation Operation => this._lazyOperation.Value;

        public abstract TResult Handle(TOptions options, IAppOperationContext operationContext);

    }
}
