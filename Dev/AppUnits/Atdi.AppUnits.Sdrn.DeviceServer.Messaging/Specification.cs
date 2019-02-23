using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.Messaging";
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
        public static readonly EventCategory Initilazing = "Initilazing";

    }

    static class Events
    {
        public static readonly EventText MessageIsBeingHandled = "The message is being handled: Type = '{0}'";
    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName HandlingResult = "Id = '{0}', CommandType = '{1}', ResultType = '{2}', PartIndex = '{3}', Status = '{4}'";
        
    }

    static class Exceptions
    {
       // public static readonly string ServiceHostWasNotInitialized = "Failed to finish processing part of results: CommandType = '{0}', ResultType = '{1}', PartIndex = '{2}', Status = '{3}'";
    }
}
