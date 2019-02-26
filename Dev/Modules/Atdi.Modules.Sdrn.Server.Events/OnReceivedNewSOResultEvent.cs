using Atdi.Contracts.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.Server.Events
{
    public class OnReceivedNewSOResultEvent : Event
    {
        public OnReceivedNewSOResultEvent()
            : base("OnReceivedNewSOResult")
        {
        }

        public OnReceivedNewSOResultEvent(string source) 
            : base("OnReceivedNewSOResult", source)
        {
        }

        public int ResultId { get; set; }
    }
}
