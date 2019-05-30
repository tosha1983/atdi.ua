using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN Server Bus Messages Primary Handlers Component";
        public static readonly EventContext PrimaryHandler = "SDRN.PrimaryHandler";
    }

    static class Categories
    {
        public static readonly EventCategory MessageProcessing = "Processing";
        public static readonly EventCategory Notify = "Notify";
        public static readonly EventCategory OnReceivedNewSOResultEvent = "OnReceivedNewSOResultEvent";
        
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
