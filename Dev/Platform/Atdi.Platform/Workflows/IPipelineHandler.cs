using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Workflows
{
    public interface IPipelineHandler<TData, TResult>
    {
        TResult Handle(TData data, IPipelineContext<TData, TResult> context);
    }
}
