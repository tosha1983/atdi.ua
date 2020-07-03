using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
	internal static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.CalcServer.Tasks";
    }

    internal static class Categories
    {
		public static readonly EventCategory MapPreparation = "MapPreparation";
		public static readonly EventCategory ContextPreparation = "ContextPreparation";
		public static readonly EventCategory TaskRunning = "TaskRunning";
		public static readonly EventCategory Declaring = "Declaring";
        public static readonly EventCategory Registration = "Registration";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Subscribing = "Subscribing";
    }

    internal static class Events
    {
        public static readonly EventText HandlerTypeWasRegistered = "The event subscriber was registered: '{0}'";
        public static readonly EventText HandlerTypeWasConnected = "The event subscriber was connected: '{0}'";

        public static readonly EventText TaskHandlerTypeWasRegistered = "The task handler was registered: '{0}'";
        public static readonly EventText IterationHandlerTypeWasRegistered = "The iteration handler was registered: '{0}'";
        public static readonly EventText DeepServiceTypeWasRegistered = "The deep service was registered: '{0}'";

	}

    internal static class TraceScopeNames
    {
        public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    internal static class Exceptions
    {
        public static readonly string StationCalibration = "Station Calibration";
    }

    internal static class ObjectPools
    {
	    public static readonly string GisProfileIndexerArrayObjectPool = "CalcServer.Gis.Profile.IndexerArray";
	    public static readonly string GisProfileClutterArrayObjectPool = "CalcServer.Gis.Profile.ClutterArray";
	    public static readonly string GisProfileBuildingArrayObjectPool = "CalcServer.Gis.Profile.BuildingArray";
	    public static readonly string GisProfileReliefArrayObjectPool = "CalcServer.Gis.Profile.ReliefArray";
	    public static readonly string GisProfileHeightArrayObjectPool = "CalcServer.Gis.Profile.HeightArray";
        public static readonly string StationCalibrationDriveTestsResultArrayObjectPool = "CalcServer.StationCalibration.DriveTestsResult";
        public static readonly string StationCalibrationCalcPointArrayObjectPool = "CalcServer.StationCalibration.CalcPoint";
        public static readonly string StationCalibrationPointFSArrayObjectPool = "CalcServer.StationCalibration.PointFS";
        public static readonly string StationCalibrationResultObjectPool = "CalcServer.StationCalibration.StationCalibrationResultObjectPool";
        public static readonly string StationCalibrationListDriveTestsResultObjectPool = "CalcServer.StationCalibration.ListDriveTestsResultObjectPool";
        public static readonly string GE06PointEarthGeometricObjectPool = "CalcServer.GE06.PointEarthGeometricObjectPool";
        
    }
}
