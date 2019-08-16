using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Workflows
{
    public interface IPipelineSite
    {
        IPipeline<TData, TResult> Declare<TData, TResult>(string name, TResult @default = default(TResult));

        IPipeline<TData, TResult> GetByName<TData, TResult>(string name);

        bool TryGetByName<TData, TResult>(string name, out IPipeline<TData, TResult> pipeline);
    }
}
