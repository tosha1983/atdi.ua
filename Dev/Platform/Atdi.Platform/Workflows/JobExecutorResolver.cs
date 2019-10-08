using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;

namespace Atdi.Platform.Workflows
{
    public sealed class JobExecutorResolver : IJobExecutorResolver
    {
        private readonly IServicesResolver _servicesResolver;

        public JobExecutorResolver(IServicesResolver servicesResolver)
        {
            this._servicesResolver = servicesResolver;
        }

        public IJobExecutor Resolve<TExecutor>() where TExecutor : IJobExecutor
        {
            var instance = _servicesResolver.Resolve<TExecutor>();
            return instance;
        }

        public IJobExecutor<TState> Resolve<TExecutor, TState>() where TExecutor : IJobExecutor<TState>
        {
            var instance = _servicesResolver.Resolve<TExecutor>();
            return instance;
        }
    }
}
