using Atdi.Contracts.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    public class EventSite : IEventSite
    {
        private readonly IEventSiteConfig _config;

        public EventSite(IEventSiteConfig config)
        {
            this._config = config;
        }

        public void Dispose()
        {
        
        }

        public IEventEmitter GetEmitter()
        {
            return new EventEmitter(this);
        }
    }
}
