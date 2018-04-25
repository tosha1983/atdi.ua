using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public interface IAppOperationHandler<TService, TOperation, TOptions, TResult>
        where TService : class, IAppService, new()
        where TOperation : class, IAppOperation, new()
        where TOptions : class, IAppOperationOptions, new()
    {
        TService Service { get; }

        TOperation Operation { get; }

        TResult Handle(TOptions options, IAppOperationContext operationContext);
    }
}
