using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public class AppServiceInvoker<TService> : IAppServiceInvoker<TService>
        where TService : class, IAppService, new()
    {
        private readonly ILogger _logger;
        private readonly IAppOperationInvokerFactory _invokerFactory;

        public AppServiceInvoker(IAppOperationInvokerFactory invokerFactory, ILogger loger)
        {
            this._invokerFactory = invokerFactory;
            this._logger = loger;
        }

        public IAppOperationInvoker<TService, TOperation, TResult> Operation<TOperation, TResult>()
            where TOperation : class, IAppOperation, new()
        {
            return this._invokerFactory.Create<TService, TOperation, TResult>();
        }
    }
}
