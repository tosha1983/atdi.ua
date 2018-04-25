using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public interface IAppServiceInvokerFactory
    {
        IAppServiceInvoker<TService> Create<TService>()
            where TService : class, IAppService, new();
    }
}
