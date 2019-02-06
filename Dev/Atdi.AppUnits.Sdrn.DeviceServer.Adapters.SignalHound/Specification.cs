using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN Device Server ExampleAdapter";
    }

    static class Categories
    {
        public static readonly EventCategory ConfigLoading = "Config loading";


    }

    static class Events
    {
        //public static readonly EventText CheckLocation = "Check location coordinates";

    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {
        public static readonly string ConfigWasNotLoaded = "The config was not loaded";
    }
}
