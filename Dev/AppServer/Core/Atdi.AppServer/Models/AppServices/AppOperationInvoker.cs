using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public sealed class AppOperationInvoker<TService, TOperation, TResult> : IAppOperationInvoker<TService, TOperation, TResult>
        where TService : class, IAppService, new()
        where TOperation : class, IAppOperation, new()
    {
        private readonly ILogger _logger;
        private readonly IAppOperationHandlerFactory _handlerFactory;

        public AppOperationInvoker(IAppOperationHandlerFactory handlerFactory, ILogger logger)
        {
            this._handlerFactory = handlerFactory;
            this._logger = logger;
        }

        public TResult Invoke<TOptions>(TOptions options, IAppOperationContext operationContext)
            where TOptions : class, IAppOperationOptions, new()
        {
            var handler = _handlerFactory.Create<TService, TOperation, TOptions, TResult>();
            try
            {
                return handler.Handle(options, operationContext);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _handlerFactory.Release(handler);
            }
        }
    }
}
