using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Workflows
{
    class PipelineContext<TData, TResult> : IPipelineContext<TData, TResult>
    {
        private readonly Pipeline<TData, TResult> _pipeline;

        public PipelineContext(Pipeline<TData, TResult> pipeline, IPipelineHandler<TData, TResult> handler)
        {
            this._pipeline = pipeline;
            this.Handler = handler;
        }
        public PipelineContext(Pipeline<TData, TResult> pipeline, Type handlerType)
        {
            this._pipeline = pipeline;
            this.HandlerType = handlerType;
        }

        public TResult Default => this._pipeline.Default;

        public IPipeline<TData, TResult> Pipeline => this._pipeline;

        public PipelineContext<TData, TResult> Next { get; set; }

        public IPipelineHandler<TData, TResult> Handler { get; set; }

        public Type HandlerType { get; }

        public TResult GoAhead(TData data)
        {
            if (this.Next != null)
            {
                return this.Next.Execute(data);
            }
            return this._pipeline.Default;
        }

        public TResult Execute(TData data)
        {
            if (this.Handler != null)
            {
                return this.Handler.Handle(data, this);
            }
            if (this.HandlerType != null)
            {
                this.Handler = _pipeline.HandlerFactory.Create<TData, TResult>(this.HandlerType);
                return this.Handler.Handle(data, this);
            }

            return this._pipeline.Default;
        }

        public override string ToString()
        {
            return $"Handler = '{Handler?.GetType().Name}', Type = '{HandlerType?.Name}'";
        }
    }
}
