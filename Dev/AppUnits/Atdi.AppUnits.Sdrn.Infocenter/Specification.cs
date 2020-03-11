using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.Infocenter
{
	internal static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.Infocenter";
    }

    internal static class Categories
    {
        public static readonly EventCategory Declaring = "Declaring";
        public static readonly EventCategory Registration = "Registration";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Subscribing = "Subscribing";

        public static readonly EventCategory MapsImport = "MapsImport";
	}

    internal static class Events
    {
        public static readonly EventText HandlerTypeWasRegistered = "The event subscriber was registered: '{0}'";
        public static readonly EventText HandlerTypeWasConnected = "The event subscriber was connected: '{0}'";

        public static readonly EventText TaskHandlerTypeWasRegistered = "The task handler was registered: '{0}'";
        public static readonly EventText IterationHandlerTypeWasRegistered = "The iteration handler was registered: '{0}'";

	}

    internal static class TraceScopeNames
    {
        public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    internal static class Exceptions
    {
        //public static readonly string ServiceHostWasNotInitialized = "The service host was not initialized";
    }
}
