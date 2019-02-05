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
        public static readonly EventContext ThisComponent = "SDRN Device Server Controller";
    }

    static class Categories
    {
        public static readonly EventCategory MessageCheckLocation = "Check location";
        public static readonly EventCategory MeasSdrTaskUpdateStatus = "Update MeasSdrTask";
        public static readonly EventCategory UpdateSensorStatus = "Update sensor";
        public static readonly EventCategory LoadSensor = "Load sensor";
        public static readonly EventCategory CreateNewObjectSensor = "Create sensor";
        

    }

    static class Events
    {
        public static readonly EventText CheckLocation = "Check location coordinates";
        public static readonly EventText UpdateStatus = "Update status MeasSdrTask";
        public static readonly EventText UpdateSensorStatus = "Update status MeasSdrTask";
        public static readonly EventText LoadSensor = "Load sensor";
        public static readonly EventText CreateNewObjectSensor = "Create sensor";
    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {
        //public static readonly string ServiceHostWasNotInitialized = "The service host was not initialized";
    }
}
