using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.ExampleAdapter
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN Device Server ExampleAdapter";
        public static readonly EventContext Adapter1 = "SDRN.Adapter1";
        public static readonly EventContext Adapter2 = "SDRN.Adapter2";
        public static readonly EventContext Adapter3 = "SDRN.Adapter3";
        public static readonly EventContext TraceAdapter = "SDRN.TraceAdapter";
        public static readonly EventContext ResultConvertor1 = "SDRN.ResultConvertor1";
        public static readonly EventContext ResultConvertor2 = "SDRN.ResultConvertor2";
        public static readonly EventContext ResultConvertor3 = "SDRN.ResultConvertor3";
    }

    static class Categories
    {
        public static readonly EventCategory ConfigLoading = "Config loading";
        public static readonly EventCategory Ctor = ".ctor";
        public static readonly EventCategory Connect = "Connect";
        public static readonly EventCategory Disconnect = "disconnect";
        public static readonly EventCategory Handle = "Handle";
        public static readonly EventCategory Converting = "Converting";

    }

    static class Events
    {
        public static readonly EventText Call = "Call";
        public static readonly EventText HandleCommand = "Handling commnad: {0}";
        public static readonly EventText ConvertFromTo = "Convert from type '{0}' to type '{1}'";

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
