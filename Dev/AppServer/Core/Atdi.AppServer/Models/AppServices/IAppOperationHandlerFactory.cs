using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public interface IAppOperationHandlerFactory
    {
        IAppOperationHandler<TService, TOperation, TOptions, TResult> Create<TService, TOperation, TOptions, TResult>()
            where TService : class, IAppService, new()
            where TOperation : class, IAppOperation, new()
            where TOptions : class, IAppOperationOptions, new();

        void Release<TService, TOperation, TOptions, TResult>(IAppOperationHandler<TService, TOperation, TOptions, TResult> handler)
            where TService : class, IAppService, new()
            where TOperation : class, IAppOperation, new()
            where TOptions : class, IAppOperationOptions, new();
    }
}
