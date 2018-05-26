using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;

namespace Atdi.CoreServices.DataLayer
{
    static class Contexts
    {
        public static readonly EventContext DataLayerCoreServices = "DataLayer Core Services";
        public static readonly EventContext SqlServerEngine = "SQL Server Engine";
        public static readonly EventContext OracleEngine = "Oracle Engine";
    }

    static class Categories
    {
        public static readonly EventCategory DataProcessing = "Processing data";
        public static readonly EventCategory CommandExecuting = "Executing command";
        public static readonly EventCategory ResultHandling = "Handling result";
        public static readonly EventCategory OpeningConnection = "Opening connection";
    }

    static class Events
    {
        public static readonly EventText UnableToCreateHost = "Unable to create the service host: {0}";
        public static readonly EventText UnableToOpenHost = "Unable to open the service host: {0}";
        public static readonly EventText UnableToCloseHost = "Unable to close the service host: {0}";
        public static readonly EventText UnableToDisposeHost = "Unable to dispose the service host: {0}";
        public static readonly EventText ServiceHostDescriptor = "{0}";
    }
    static class TraceScopeNames
    {
        public static readonly TraceScopeName GetQueriesTree = "GetQueriesTree";
        public static readonly TraceScopeName GetQueryMetadata = "GetQueryMetadata";
        public static readonly TraceScopeName ExecuteQuery = "ExecuteQuery";
        public static readonly TraceScopeName SaveChanges = "SaveChanges";
    }

    static class Exceptions
    {
        public static readonly ExceptionText NotSupportedEngineType = "Not supported the data engine with type '{0}'";
    }

    static class ConfigParameters
    {
        public static readonly string DataContexts = "DataContexts";
    }
}
