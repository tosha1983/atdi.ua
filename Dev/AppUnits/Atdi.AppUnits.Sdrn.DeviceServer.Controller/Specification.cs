using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.DeviceServer";
        public static readonly EventContext AdapterWorker = "SDRN.AdapterWorker";
        public static readonly EventContext DevicesHost = "SDRN.DevicesHost";
        public static readonly EventContext ResultWorker = "SDRN.ResultWorker";
        public static readonly EventContext ResultHandler = "SDRN.ResultHandler";
        public static readonly EventContext ResultConvertor = "SDRN.ResultConvertor";
    }

    static class Categories
    {
        public static readonly EventCategory Disposing = "Disposing";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Running = "Running";
        public static readonly EventCategory Registering = "Registering";
        public static readonly EventCategory Finalizing = "Finalizing";
        public static readonly EventCategory Handling = "Handling";
        public static readonly EventCategory Converting = "Converting";

        public static readonly EventCategory UpdateSensorStatus = "Update sensor";
        public static readonly EventCategory LoadSensor = "Load sensor";
        public static readonly EventCategory CreateNewObjectSensor = "Create sensor";
        

    }

    static class Events
    {


        public static readonly EventText DisconnectingAdapterError = "Error occurred while disconnecting the adapter: AdapterType = '{0}'";
        public static readonly EventText AbortAdapterThread = "Abort the thread of the adapter: AdapterType = '{0}'";
        public static readonly EventText ProcessingAdapterError = "Error occurred while processing the adapter: AdapterType = '{0}'";
        public static readonly EventText TookCommand = "The new command was took: AdapterType = '{0}', CommandType = '{1}', ParameterType = '{2}'";
        public static readonly EventText RejectedCommand = "The new command was rejected: AdapterType = '{0}', CommandType = '{1}', ParameterType = '{2}', Reasone = '{3}', ";
        public static readonly EventText TransferCommand = "Transfer command to adapter: AdapterType = '{0}', CommandType = '{1}', ParameterType = '{2}'";
        public static readonly EventText FinalizedCommand = "The execution command was finalized: AdapterType = '{0}', CommandType = '{1}', ParameterType = '{2}'";

        public static readonly EventText CreatedAdapter = "The adapter object was created: AdapterType = '{0}'";
        public static readonly EventText ConnectedAdapter = "The adapter object was connected: AdapterType = '{0}'";

        public static readonly EventText RanAdapterThread = "The adapter thread was ran: AdapterType = '{0}'";

        public static readonly EventText RegisteredAdapter = "The adapter was registered: AdapterType = '{0}'";

        public static readonly EventText ProcessingResultError = "Error occurred while processing the result: AdapterType = '{0}', CommandType = '{1}'";

        public static readonly EventText HandlingResultError = "Error occurred while processing the results: CommandType = '{0}', ResultType = '{1}', PartIndex = '{2}', Status = '{3}'";
        public static readonly EventText ConvertingResultError = "Error occurred while converting the results: FromType = '{0}', ResultType = '{1}', PartIndex = '{2}', Status = '{3}'";
    }
    static class TraceScopeNames
    {
        public static readonly TraceScopeName HandlingResult = "Id = '{0}', CommandType = '{1}', ResultType = '{2}', PartIndex = '{3}', Status = '{4}'";
        public static readonly TraceScopeName ConvertingResult = "Id = '{0}', FromType = '{1}', ResultType = '{2}', PartIndex = '{3}', Status = '{4}'";
    }

    static class Exceptions
    {
       // public static readonly string ServiceHostWasNotInitialized = "Failed to finish processing part of results: CommandType = '{0}', ResultType = '{1}', PartIndex = '{2}', Status = '{3}'";
    }
}
