using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public interface IAppOperationInvoker<TService, TOperation, TResult>
        where TService : class, IAppService, new()
        where TOperation : class, IAppOperation, new()
        
    {
        TResult Invoke<TOptions>(TOptions options, IAppOperationContext operationContext) 
            where TOptions : class, IAppOperationOptions, new() ;
    }
}
