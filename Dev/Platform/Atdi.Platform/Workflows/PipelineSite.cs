using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Workflows
{
    public class PipelineSite : IPipelineSite
    {
        private readonly ConcurrentDictionary<string, IPipeline> _pipelines;
        private readonly IPipelineHandlerFactory _handlerFactory;

        public PipelineSite(IPipelineHandlerFactory handlerFactory)
        {
            this._pipelines = new ConcurrentDictionary<string, IPipeline>();
            this._handlerFactory = handlerFactory;
        }

        public IPipeline<TData, TResult> Declare<TData, TResult>(string name, TResult @default = default(TResult))
        {
            if (!_pipelines.TryGetValue(name, out IPipeline pipeline))
            {
                pipeline = new Pipeline<TData, TResult>(name, @default, _handlerFactory);
                if (!_pipelines.TryAdd(name, pipeline))
                {
                    if (!_pipelines.TryGetValue(name, out pipeline))
                    {
                        throw new InvalidOperationException($"Failed to add new pipeline to cache by name '{name}'");
                    }
                }
            }
            return (IPipeline<TData, TResult>)pipeline;
        }

        public IPipeline<TData, TResult> GetByName<TData, TResult>(string name)
        {
            if (_pipelines.TryGetValue(name, out IPipeline pipeline))
            {
                return (IPipeline<TData, TResult>)pipeline;
            }
            throw new InvalidOperationException($"Not found a pipeline in cache by name '{name}'");
        }

        public bool TryGetByName<TData, TResult>(string name, out IPipeline<TData, TResult> pipeline)
        {
            if (_pipelines.TryGetValue(name, out IPipeline rawPipeline))
            {
                pipeline = (IPipeline<TData, TResult>)rawPipeline;
                return true;
            }

            pipeline = null;
            return false;
        }
    }
}
