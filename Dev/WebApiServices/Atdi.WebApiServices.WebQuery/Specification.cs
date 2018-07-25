using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebApiServices.WebQuery
{
    static class Contexts
    {
        public static readonly EventContext WebQueryWebApiServices = "WebQuery Web API Services";
        public static readonly EventContext WebQuery = "WebQuery Web API Service";
    }

    static class Categories
    {
        public static readonly EventCategory OperationCall = "Web API call";
    }

    static class Events
    {
        //public static readonly EventText UnableToCreateHost = "Unable to create the service host: {0}";
        //public static readonly EventText UnableToOpenHost = "Unable to open the service host: {0}";
        //public static readonly EventText UnableToCloseHost = "Unable to close the service host: {0}";
        //public static readonly EventText UnableToDisposeHost = "Unable to dispose the service host: {0}";
        //public static readonly EventText ServiceHostDescriptor = "{0}";
    }
    static class TraceScopeNames
    {
        public static readonly TraceScopeName GetQueryGroups = "Get query groups";
        public static readonly TraceScopeName GetQueryMetadata = "Get query metadata";
        public static readonly TraceScopeName GetQueryMetadataByCode = "Get query metadata by code";
        public static readonly TraceScopeName ExecuteQuery = "Execute query";
        public static readonly TraceScopeName SaveChanges = "Save changes";
    }

    static class Exceptions
    {
        //public static readonly string ServiceHostWasNotInitialized = "The service host was not initialized";
    }
}
