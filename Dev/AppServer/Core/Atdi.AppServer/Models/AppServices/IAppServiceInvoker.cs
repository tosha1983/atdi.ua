using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public interface IAppServiceInvoker<TService>
        where TService : class, IAppService, new()
    {
        IAppOperationInvoker<TService, TOperation, TResult> Operation<TOperation, TResult>()
            where TOperation : class, IAppOperation, new();
    }
}
