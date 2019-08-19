using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Workflows
{
    public interface IPipeline
    {
        string Name { get; }
    }

    public enum PipelineHandlerRegistrationOptions
    {
        First,
        Last
    }

    public interface IPipeline<TData, TResult> : IPipeline
    {
        TResult Execute(TData data);

        IPipeline<TData, TResult> Register(IPipelineHandler<TData, TResult> handler, PipelineHandlerRegistrationOptions options = PipelineHandlerRegistrationOptions.First);

        IPipeline<TData, TResult> Register(Type type, PipelineHandlerRegistrationOptions options = PipelineHandlerRegistrationOptions.First);
    }
}
