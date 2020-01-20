using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.Server";
    }

    static class Categories
    {
        public static readonly EventCategory Declaring = "Declaring";
        public static readonly EventCategory Registration = "Registration";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Subscribing = "Subscribing";
    }

    static class Events
    {
        public static readonly EventText HandlerTypeWasRegistered = "The event subscriber was registered: '{0}'";
        public static readonly EventText HandlerTypeWasConnected = "The event subscriber was connected: '{0}'";

    }
    static class TraceScopeNames
    {
        public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {
        //public static readonly string ServiceHostWasNotInitialized = "The service host was not initialized";
    }
}
