using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.AppServer
{
    sealed class  ServerHostLoader : IServerHostLoader
    {
        private readonly Queue<(string Context, Action Callback)> _triggers;
        private readonly ILogger _logger;

        public ServerHostLoader(ILogger logger)
        {
            this._logger = logger;
            this._triggers = new Queue<(string Context, Action Callback)>();
        }

        public void RegisterTrigger(string context, Action triggerCallback)
        {
            this._triggers.Enqueue((context, triggerCallback));
        }

        public void ExecuteTriggers()
        {
            while(this._triggers.Count > 0)
            {
                var (context, callback) = this._triggers.Dequeue();
                try
                {
                    this._logger.Verbouse(Contexts.AppServerHost, Categories.Triggering, Events.TriggerExecuting.With(context));
                    callback();
                    this._logger.Verbouse(Contexts.AppServerHost, Categories.Triggering, Events.TriggerExecuted.With(context));
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.AppServerHost, Categories.Triggering, Events.ExecutingTriggerError.With(context), e);
                }
            }
        }
    }
}
