﻿using Atdi.Platform;
using Atdi.Platform.Logging;

namespace Atdi.AppServices.WebQuery
{
    internal static class Contexts
    {
        public static readonly EventContext WebQueryAppServices = "WebQuery AppServices";
        public static readonly EventContext ErrorUpdateOperation = "Error occurred while updating data";
        public static readonly EventContext ErrorInsertOperation = "Error occurred while insert data";
        //public static readonly EventContext ErrorDeleteOperation = "Error occurred while delete data";

    }

    internal static class Categories
    {
        public static readonly EventCategory Handling = "Handling";
        public static readonly EventCategory Init = "Init";
		//public static readonly EventCategory HasColumnn = "HasColumnn";
	}

    internal static class Events
    {
        //public static readonly EventText UnableToCreateHost = "Unable to create the service host: {0}";
        //public static readonly EventText UnableToOpenHost = "Unable to open the service host: {0}";
        //public static readonly EventText UnableToCloseHost = "Unable to close the service host: {0}";
        //public static readonly EventText UnableToDisposeHost = "Unable to dispose the service host: {0}";
        //public static readonly EventText ServiceHostDescriptor = "{0}";
    }

    internal static class TraceScopeNames
    {
        public static readonly TraceScopeName GetQueryGroups = "GetQueryGroups";
        public static readonly TraceScopeName GetQueryMetadata = "GetQueryMetadata";
        public static readonly TraceScopeName GetQueryMetadataByCode = "GetQueryMetadataByCode";
        public static readonly TraceScopeName ExecuteQuery = "ExecuteQuery";
        public static readonly TraceScopeName SaveChanges = "SaveChanges";

    }

    internal static class Exceptions
    {
        //public static readonly ExceptionText ServiceHostWasNotInitialized = "The service host was not initialized";
        public static readonly ExceptionText QueryIsNotAvailable = "The query is not available";
        //public static readonly ExceptionText ColumnIsNotAvailable = "The column(s) {0} is not available";
        //public static readonly ExceptionText FetchOptionsNull = "Fetch options is NULL";
        public static readonly ExceptionText ActionTypeNotSupported = "The action type {0} is not supported.";
        //public static readonly ExceptionText AccessToActionDenied = "Access to action '{0}' is denied";
        public static readonly ExceptionText DataRowTypeNotSupported = "The data row type {0} is not supported.";
        public static readonly ExceptionText OperationBetweenNotSupportedfoString = "Operations 'Between' and 'NotBetween' not supported for type 'String'.";
        public static readonly ExceptionText HandlerOperationNotImplemented = "The handler for this operation is not implemented";
        //public static readonly ExceptionText HandlerOperationNotImplemented2 = "The handler for operations 'Between', 'NotBetween','In','NotIn' is not implemented";
        public static readonly ExceptionText FieldDefaultValueNull = "Field 'DEFAULTVALUE' the table 'XWEBCONSTRAINT' is NULL or Empty;";
    }
}
