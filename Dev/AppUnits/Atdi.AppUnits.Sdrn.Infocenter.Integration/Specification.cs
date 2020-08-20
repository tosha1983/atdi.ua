using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration
{
	internal static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.Infocenter.Integration";
    }

    internal static class Categories
    {
	    public static readonly EventCategory CsvImport = "CsvImport";
	    public static readonly EventCategory Synchronization = "Synchronization";

		public static readonly EventCategory Declaring = "Declaring";
        public static readonly EventCategory Registration = "Registration";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Subscribing = "Subscribing";

        public static readonly EventCategory MapsImport = "MapsImport";
        public static readonly EventCategory ClutterImport = "ClutterImport";
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

    internal static class Pipelines
	{
	    public static readonly string FilesImport = "FilesImportPipeline";
    }

    internal static class DataSource
    {
	    public static readonly string SdrnServer = "SDRN Server";
		public static readonly string CsvFile = "CSV File";
		public static readonly string CalcServer = "Calc Server";
	}
    internal static class IntegrationObjects
    {
	    public static readonly string GlobalIdentity = "GlobalIdentity";
	    public static readonly string StationMonitoring = "StationMonitoring";
	    public static readonly string Sensors = "Sensors";
	    public static readonly string SensorAntennas = "SensorAntennas";
	    public static readonly string SensorAntennaPatterns = "SensorAntennaPatterns";
	    public static readonly string SensorEquipment = "SensorEquipment";
	    public static readonly string SensorEquipmentSensitivities = "SensorEquipmentSensitivities";
	    public static readonly string SensorLocations = "SensorLocations";

	    public static readonly string ObservedTasks = "ObservedTasks";
	}
}
