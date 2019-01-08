using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.ControlA
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN Device Control A";
    }

    static class Categories
    {
        //public static readonly EventCategory MessageProcessing = "Processing";
        //public static readonly EventCategory Notify = "Notify";
    }

    static class Events
    {
        //public static readonly EventText UnableToCreateHost = "Unable to create the service host: {0}";
        //public static readonly EventText UnableToOpenHost = "Unable to open the service host: {0}";
        //public static readonly EventText UnableToCloseHost = "Unable to close the service host: {0}";
        //public static readonly EventText UnableToDisposeHost = "Unable to dispose the service host: {0}";
        //public static readonly EventText ServiceHostDescriptor = "{0}";
    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {
        //public static readonly string ServiceHostWasNotInitialized = "The service host was not initialized";
    }
}
