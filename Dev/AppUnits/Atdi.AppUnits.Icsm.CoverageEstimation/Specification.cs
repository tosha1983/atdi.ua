using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Icsm.CoverageEstimation
{
    internal static class Contexts
    {
        public static readonly EventContext ThisComponent = "CoverageEstimation";
    }

    internal static class Categories
    {
        public static readonly EventCategory Disposing = "Disposing";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Executing = "Executing";
        public static readonly EventCategory Running = "Running";
        public static readonly EventCategory Registering = "Registering";
        public static readonly EventCategory Finalizing = "Finalizing";
        public static readonly EventCategory Handling = "Handling";
        public static readonly EventCategory Converting = "Converting";
        public static readonly EventCategory Creating = "Creating";
        public static readonly EventCategory Finishing = "Finishing";
        public static readonly EventCategory Initializing = "Initializing";
    }

}
