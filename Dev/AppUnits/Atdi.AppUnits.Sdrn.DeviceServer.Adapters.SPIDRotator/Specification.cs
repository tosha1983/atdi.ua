using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SPIDRotator
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN Device Server SPIDRotator";
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
