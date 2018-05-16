using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery
{
    static class Contexts
    {
        public static readonly EventContext WebQueryAppServices = "WebQuery AppServices";
    }

    static class Categories
    {
        public static readonly EventCategory Handling = "Handling";
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
        public static readonly string ServiceHostWasNotInitialized = "The service host was not initialized";
    }
}
