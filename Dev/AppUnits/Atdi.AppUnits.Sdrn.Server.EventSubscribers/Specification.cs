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
        public static readonly IDataCacheDescriptor<string, int> MeasTaskIdentity = DataCacheDefiner.Define<string, int>("SDRN.Server.MeasTaskIdentity");
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
