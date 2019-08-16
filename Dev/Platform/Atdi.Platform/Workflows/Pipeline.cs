using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Workflows
{
    class Pipeline<TData, TResult> : IPipeline<TData, TResult>
    {
        private readonly object _locker = new object();

        private readonly TResult _default;
        private readonly IPipelineHandlerFactory _handlerFactory;
        private PipelineContext<TData, TResult> _firstContext;
        private PipelineContext<TData, TResult> _lastContext;

        public Pipeline(string name, TResult @default, IPipelineHandlerFactory handlerFactory)
        {
            this.Name = name;
            this._default = @default;
            this._handlerFactory = handlerFactory;
        }

        public TResult Default => _default;

        public string Name { get; }

        public IPipelineHandlerFactory HandlerFactory => this._handlerFactory;

        public TResult Execute(TData data)
        {
            if (this._firstContext != null)
            {
                return _firstContext.Execute(data);
            }
            return _default;
        }

        public IPipeline<TData, TResult> Register(IPipelineHandler<TData, TResult> handler, PipelineHandlerRegistrationOptions options = PipelineHandlerRegistrationOptions.First)
        {
            lock(_locker)
            {
                var context = new PipelineContext<TData, TResult>(this, handler);
                switch (options)
                {
                    case PipelineHandlerRegistrationOptions.First:
                        context.Next = this._firstContext;
                        this._firstContext = context;
                        if (this._lastContext == null)
                        {
                            this._lastContext = _firstContext;
                        }
                        break;
                    case PipelineHandlerRegistrationOptions.Last:
                        if (this._lastContext != null)
                        {
                            this._lastContext.Next = context;
                        }
                        this._lastContext = context;
                        if (_firstContext == null)
                        {
                            _firstContext = _lastContext;
                        }
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported pipeline handler registration options '{options}'");
                }

                return this;
            }
        }

        public IPipeline<TData, TResult> Register(Type type, PipelineHandlerRegistrationOptions options = PipelineHandlerRegistrationOptions.First)
        {
            lock (_locker)
            {
                var context = new PipelineContext<TData, TResult>(this, type);
                switch (options)
                {
                    case PipelineHandlerRegistrationOptions.First:
                        context.Next = this._firstContext;
                        this._firstContext = context;
                        if (this._lastContext == null)
                        {
                            this._lastContext = _firstContext;
                        }
                        break;
                    case PipelineHandlerRegistrationOptions.Last:
                        if (this._lastContext != null)
                        {
                            this._lastContext.Next = context;
                        }
                        this._lastContext = context;
                        if (_firstContext == null)
                        {
                            _firstContext = _lastContext;
                        }
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported pipeline handler registration options '{options}'");
                }

                return this;
            }
        }

        public override string ToString()
        {
            return $"Name = '{Name}', Data = '{typeof(TData).Name}', Result = '{typeof(TResult).Name}'";
        }
    }
}
