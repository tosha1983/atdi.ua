using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeepServices
{
	internal static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.DeepServices";
    }

    internal static class Categories
    {
		public static readonly EventCategory MapPreparation = "MapPreparation";
		public static readonly EventCategory TaskRunning = "TaskRunning";
		public static readonly EventCategory Declaring = "Declaring";
        public static readonly EventCategory Registration = "Registration";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Subscribing = "Subscribing";
    }

    internal static class Events
    {
        public static readonly EventText DeepServiceTypeWasRegistered = "The deep service was registered: '{0}'";

	}

    internal static class TraceScopeNames
    {
        //public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    internal static class Exceptions
    {
        //public static readonly string ServiceHostWasNotInitialized = "The service host was not initialized";
    }
}
