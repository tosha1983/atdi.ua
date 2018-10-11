using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.EventSystem
{
    [Flags]
    public enum EventEmittingRule
    {
        Default = 0,
        Broadcast = 1
    }

    public class EventEmittingOptions
    {
        public EventEmittingRule Rule { get; set; }

        public string[] Destination { get; set; }
    }
}
