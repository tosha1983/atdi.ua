using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Workflows
{
    public interface IJobBroker
    {
        IJobToken Run<TExecutor>(JobDefinition<TExecutor> definition)
            where TExecutor : IJobExecutor;

        IJobToken Run<TExecutor,TState>(JobDefinition<TExecutor, TState> definition, TState state)
            where TExecutor : IJobExecutor<TState>;
    }
}
