using Atdi.Platform.Logging;
using Atdi.Platform.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers
{
    static class DataCaches
    {
        public static readonly IDataCacheDescriptor<string, long> AutoMeasTaskIdentity = DataCacheDefiner.Define<string, long>("SDRN.Server.AutoMeasTaskIdentity");
        public static readonly IDataCacheDescriptor<string, long> AutoSubTaskIdentity = DataCacheDefiner.Define<string, long>("SDRN.Server.AutoSubTaskIdentity");
        public static readonly IDataCacheDescriptor<string, long> AutoSubTaskSensorIdentity = DataCacheDefiner.Define<string, long>("SDRN.Server.AutoSubTaskSensorIdentity");
        public static readonly IDataCacheDescriptor<string, long> VerifiedSubTaskSensorIdentity = DataCacheDefiner.Define<string, long>("SDRN.Server.VerifiedSubTaskSensorIdentity");

        public static readonly IDataCacheDescriptor<string, long> MeasResultStationIdentity = DataCacheDefiner.Define<string, long>("SDRN.Server.MeasResultStationIdentity");
        public static readonly IDataCacheDescriptor<string, long> MeasResultIdentity = DataCacheDefiner.Define<string, long>("SDRN.Server.MeasResultIdentity");
        public static readonly IDataCacheDescriptor<string, long> SensorIdentity = DataCacheDefiner.Define<string, long>("SDRN.Server.SensorIdentity");
    }

    static class Monitoring
    {
        public static class Counters
        {
            public static readonly IStatisticCounterKey MessageProcessingHits = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.Hits");
            public static readonly IStatisticCounterKey SendMeasResultsHits = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendMeasResults.Hits");
            public static readonly IStatisticCounterKey SendMeasResultsSpectrumOccupation = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendMeasResults.SpectrumOccupation");
            public static readonly IStatisticCounterKey SendMeasResultsMonitoringStations = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendMeasResults.MonitoringStations");
            public static readonly IStatisticCounterKey SendMeasResultsSignaling = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendMeasResults.Signaling");
            public static readonly IStatisticCounterKey SendMeasResultsErrors = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendMeasResults.Errors");

            public static readonly IStatisticCounterKey RegisterSensorHits = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.RegisterSensor.Hits");
            public static readonly IStatisticCounterKey RegisterSensorErrors = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.RegisterSensor.Errors");

            public static readonly IStatisticCounterKey UpdateSensorHits = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.UpdateSensor.Hits");
            public static readonly IStatisticCounterKey UpdateSensorErrors = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.UpdateSensor.Errors");

            public static readonly IStatisticCounterKey SendCommandResultHits = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendCommandResult.Hits");
            public static readonly IStatisticCounterKey SendCommandResultErrors = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendCommandResult.Errors");

            public static readonly IStatisticCounterKey SendEntityHits = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendEntity.Hits");
            public static readonly IStatisticCounterKey SendEntityErrors = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendEntity.Errors");

            public static readonly IStatisticCounterKey SendEntityPartHits = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendEntityPart.Hits");
            public static readonly IStatisticCounterKey SendEntityPartErrors = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.SendEntityPart.Errors");

            public static readonly IStatisticCounterKey OnlineMeasurementResponseHits = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.OnlineMeasurementResponse.Hits");
            public static readonly IStatisticCounterKey OnlineMeasurementResponseErrors = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.OnlineMeasurementResponse.Errors");

            public static readonly IStatisticCounterKey OnlineMeasurementStatusHits = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.OnlineMeasurementStatus.Hits");
            public static readonly IStatisticCounterKey OnlineMeasurementStatusErrors = STS.DefineCounterKey("SDRN.Server.DeviceBus.MessageProcessing.OnlineMeasurementStatus.Errors");
        }
        

    }
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.EventSubscribers";
    }

    static class Categories
    {
        public static readonly EventCategory MessageProcessing = "Message processing";
        public static readonly EventCategory EventProcessing = "Processing";
        public static readonly EventCategory Notify = "Notify";
        public static readonly EventCategory RegisterAggregationServerPipelineHandler = "RegisterAggregationServerPipelineHandler";
    }

    static class Events
    {
        //public static readonly EventText UnableToCreateHost = "Unable to create the service host: {0}";
        //public static readonly EventText UnableToOpenHost = "Unable to open the service host: {0}";
        //public static readonly EventText UnableToCloseHost = "Unable to close the service host: {0}";
        //public static readonly EventText UnableToDisposeHost = "Unable to dispose the service host: {0}";
        //public static readonly EventText ServiceHostDescriptor = "{0}";
        public static readonly EventText StartOperationWriting = "Start of validation operation and writing to main tables";
        public static readonly EventText EndOperationWriting = "End of validation operation and writing to main tables";
        public static readonly EventText IsAlreadySaveResults = "ResultId = {0} is already recorded in the XBS_RESMEASRAW table with Id = {1}, repeated recording is canceled";

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
