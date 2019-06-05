using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.EventSystem
{
    [Flags]
    public enum EventEmittingRule
    {
        Default = 0,
        Broadcast = 1
    }
}
