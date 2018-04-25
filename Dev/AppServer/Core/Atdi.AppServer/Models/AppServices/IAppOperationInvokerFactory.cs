using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public interface IAppOperationInvokerFactory
    {
        IAppOperationInvoker<TService, TOperation, TResult> Create<TService, TOperation, TResult>()
            where TService : class, IAppService, new()
            where TOperation : class, IAppOperation, new();
    }
}
