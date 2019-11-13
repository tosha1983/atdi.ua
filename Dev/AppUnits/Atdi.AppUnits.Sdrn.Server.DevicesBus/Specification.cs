using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.Server.DevicesBus
{
    internal static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.DevicesBus";
    }

    internal static class Categories
    {
        public static readonly EventCategory Disposing = "Disposing";
        //public static readonly EventCategory Notify = "Notify";
        public static readonly EventCategory Processing = "Processing";
        //public static readonly EventCategory OnReceivedNewSOResultEvent = "OnReceivedNewSOResultEvent";
        
    }

    internal static class Events
    {
        //public static readonly EventText UnableToCreateHost = "Unable to create the service host: {0}";
        //public static readonly EventText UnableToOpenHost = "Unable to open the service host: {0}";
        //public static readonly EventText UnableToCloseHost = "Unable to close the service host: {0}";
        //public static readonly EventText UnableToDisposeHost = "Unable to dispose the service host: {0}";
        //public static readonly EventText ServiceHostDescriptor = "{0}";
        //public static readonly EventText StartOperationWriting = "Start of validation operation and writing to main tables";
        //public static readonly EventText EndOperationWriting = "End of validation operation and writing to main tables";
        //public static readonly EventText IsAlreadySaveResults = "ResultId = {0} is already recorded in the XBS_RESMEASRAW table with Id = {1}, repeated recording is canceled";

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
