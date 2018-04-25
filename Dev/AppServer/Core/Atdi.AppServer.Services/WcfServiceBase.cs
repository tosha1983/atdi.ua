using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.TechServices;

namespace Atdi.AppServer.Services
{
    public abstract class WcfServiceBase<TAppService, TContract> : LoggedObject, ITechService<TContract>
        where TAppService : class, IAppService, new()
        where TContract : class
    {
        private readonly IAppServiceInvokerFactory _factory;
        private readonly IAppServiceInvoker<TAppService> _serviceInvoker;

        public WcfServiceBase(IAppServiceInvokerFactory factory, ILogger logger) : base(logger)
        {
            this._factory = factory;
            this._serviceInvoker = factory.Create<TAppService>();
        }

        private IAppOperationContext PreparedOperationContext()
        {
            return null;
        }

        public IAppOperationContext OperationContext => this.PreparedOperationContext();

        public IAppServiceInvoker<TService> Service<TService>()
            where TService : class, IAppService, new()
        {
            return _factory.Create<TService>();
        }

        public IAppOperationInvoker<TAppService, TOperation, TResult> Operation<TOperation, TResult>()
            where TOperation : class, IAppOperation, new()
        {
            return this._serviceInvoker.Operation<TOperation, TResult>();
        }
    }
}
