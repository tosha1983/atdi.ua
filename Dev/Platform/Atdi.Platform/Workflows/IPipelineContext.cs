using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Workflows
{
    public interface IPipelineContext<TData, TResult>
    {
        TResult GoAhead(TData data);

        TResult Default { get; }

        IPipeline<TData, TResult> Pipeline { get; }
    }
}
