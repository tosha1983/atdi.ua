using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Workflows
{
    public interface IPipelineHandlerFactory
    {
        IPipelineHandler<TData, TResult> Create<TData, TResult>(Type handlerType);
    }
}
