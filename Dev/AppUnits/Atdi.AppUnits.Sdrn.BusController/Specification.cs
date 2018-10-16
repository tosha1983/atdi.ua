using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.MessageBus";
    }

    static class Categories
    {
        public static readonly EventCategory Declaring = "Declaring";
        public static readonly EventCategory Registration = "Registration";
        public static readonly EventCategory Processing = "Processing";
    }

    static class Events
    {
        public static readonly EventText HandlerTypeWasRegistred = "The handler type was registered successfully: '{0}'";
        public static readonly EventText ReceivedMessage = "Received a message: Consumer: '{0}', RoutingKey: '{1}', Exchange: '{2}', DeliveryTag: #{3}, MessageType: '{4}', MessageId: '{5}'";
        //public static readonly EventText UnableToOpenHost = "Unable to open the service host: {0}";
        //public static readonly EventText UnableToCloseHost = "Unable to close the service host: {0}";
        //public static readonly EventText UnableToDisposeHost = "Unable to dispose the service host: {0}";
        //public static readonly EventText ServiceHostDescriptor = "{0}";
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
