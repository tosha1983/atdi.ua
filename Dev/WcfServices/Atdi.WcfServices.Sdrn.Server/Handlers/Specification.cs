using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices.Sdrn.Server
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "Atdi.WcfServices.Sdrn.Server";
    }

    static class Categories
    {
        public static readonly EventCategory Declaring = "Declaring";
        public static readonly EventCategory Registration = "Registration";
        public static readonly EventCategory Processing = "Processing";
    }

    static class Events
    {
        public static readonly EventText HandlerTypeWasRegistred = "The event subscriber type was registered successfully: '{0}'";
        
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
