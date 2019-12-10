using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example
{
    static class Contexts
    {
        public static readonly EventContext Test1TaskWorker = "SDRN.Test1TaskWorker";
        public static readonly EventContext Test2TaskWorker = "SDRN.Test2TaskWorker";

        public static readonly EventContext TraceAutoTask = "SDRN.TraceAutoTask";
        public static readonly EventContext TraceTask = "SDRN.TraceTask";

        public static readonly EventContext TestCommand1ResultHandler = "SDRN.TestCommand1ResultHandler";
        public static readonly EventContext TestCommand2ResultHandler = "SDRN.TestCommand2ResultHandler";

        public static readonly EventContext TraceCommandResultHandler = "SDRN.TraceCommandResultHandler";

    }

    static class Categories
    {
        public static readonly EventCategory ConfigLoading = "Config loading";
        public static readonly EventCategory Ctor = ".ctor";
        public static readonly EventCategory Connect = "Connect";
        public static readonly EventCategory Disconnect = "disconnect";
        public static readonly EventCategory Handle = "Handle";
        public static readonly EventCategory Converting = "Converting";
        public static readonly EventCategory Run = "Run";

    }

    static class Events
    {
        public static readonly EventText Call = "Call";
        public static readonly EventText RunTask = "Run task: Id {0}";
        public static readonly EventText HandlingResult = "Handling result: Index = #{0}, Status = '{1}'";

    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {
        public static readonly string ConfigWasNotLoaded = "The config was not loaded";
    }
}
