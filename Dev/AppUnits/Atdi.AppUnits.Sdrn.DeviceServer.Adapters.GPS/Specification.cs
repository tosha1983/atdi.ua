using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.GPS
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "DeviceServer.GPS";
    }

    static class Categories
    {
        public static readonly EventCategory Processing = "GPSProcess";
    }

    static class Events
    {
        public static readonly EventText OpenDevice = "GPS device is running";
        public static readonly EventText CloseDevice = "GPS device is closed";
        public static readonly EventText FromConfigurationFileNotRecognized = "from configuration file not recognized";
        public static readonly EventText FromConfigurationFileIsNullOrEmpty = "from configuration file is null or empty";
        


    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName HandlingResult = "Id = '{0}', CommandType = '{1}', ResultType = '{2}', PartIndex = '{3}', Status = '{4}'";
        
    }

    static class Exceptions
    {
        public static readonly string UnknownError = "Unknown Error {0}";
        public static readonly string LogEventError = "LogEvent Error {0}";
        

    }
}
