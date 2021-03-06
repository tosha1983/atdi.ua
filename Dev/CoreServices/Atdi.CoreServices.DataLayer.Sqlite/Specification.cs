﻿using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;

namespace Atdi.CoreServices.DataLayer.Sqlite
{
	internal static class Monitoring
    {
        public static readonly IStatisticCounterKey ExecutorHitsCounterKey = STS.DefineCounterKey("DataLayer.SQLite.Executor.Hits");
        public static readonly IStatisticCounterKey ExecutorCountCounterKey = STS.DefineCounterKey("DataLayer.SQLite.Executor.Count");
        public static readonly IStatisticCounterKey TranHitsCounterKey = STS.DefineCounterKey("DataLayer.SQLite.Transactions.Hits");
        public static readonly IStatisticCounterKey TranCountCounterKey = STS.DefineCounterKey("DataLayer.SQLite.Transactions.Count");
        public static readonly IStatisticCounterKey TranCommitCounterKey = STS.DefineCounterKey("DataLayer.SQLite.Transactions.Commit");
        public static readonly IStatisticCounterKey TranRollbackCounterKey = STS.DefineCounterKey("DataLayer.SQLite.Transactions.Rollback");
        public static readonly IStatisticCounterKey ErrorsCounterKey = STS.DefineCounterKey("DataLayer.SQLite.Errors");
        public static readonly IStatisticCounterKey ConnectionsCountCounterKey = STS.DefineCounterKey("DataLayer.SQLite.Connections.Count");
        public static readonly IStatisticCounterKey ConnectionsHitsCounterKey = STS.DefineCounterKey("DataLayer.SQLite.Connections.Hits");
    }

    static class Contexts
    {

        public static readonly EventContext SqliteEngine = "SQLiteEngine";

    }

    static class Categories
    {
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Executing = "Executing";
        public static readonly EventCategory Creation = "Creation";
        public static readonly EventCategory ResultHandling = "Reading";
        public static readonly EventCategory Disposing = "Disposing";
        public static readonly EventCategory CommandExecuting = "Executing";
        public static readonly EventCategory OpeningConnection = "Connection";
        public static readonly EventCategory CommandCreation = "Preparation";

    }

    static class Events
    {
        public static readonly EventText UnableToCreateHost = "Unable to create the service host: {0}";
        public static readonly EventText UnableToOpenHost = "Unable to open the service host: {0}";
        public static readonly EventText UnableToCloseHost = "Unable to close the service host: {0}";
        public static readonly EventText UnableToDisposeHost = "Unable to dispose the service host: {0}";
        public static readonly EventText ServiceHostDescriptor = "{0}";

        public static readonly EventText ObjectWasCreated = "The object of {0} was created";
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
        public static readonly ExceptionText TransactionHasNotBegan = "Transaction has not began";
        public static readonly ExceptionText TransactionHasBegan = "Transaction has began";
        public static readonly ExceptionText ConnectionHasNotOpened = "Connection has not opened";
        public static readonly ExceptionText ConnectionHasWrongState = "Connection has the wrong state '{0}'";
        public static readonly ExceptionText EngineTypeNotSupported = "The data engine with type '{0}' is not supported";
        public static readonly ExceptionText DataLimitTypeNotSupported = "The data limit with type '{0}' is not supported";
        public static readonly ExceptionText SortDirectionNotSupported = "The sort direction with name '{0}' is not supported";
        public static readonly ExceptionText LogicalOperatorNotSupported = "The logical operator with name '{0}' is not supported";
        public static readonly ExceptionText NotRecognizeAlias = "The alias of main table not found {0}";


        public static readonly ExceptionText FailedToOpenConnection = "Failed to open the connection to the database server";
        public static readonly ExceptionText FailedToCreateEngineExecuter = "Failed to create the Engine Executer Object";
        public static readonly ExceptionText FailedToBeginTransaction = "Failed to begin the transaction";
        public static readonly ExceptionText FailedToCommitTransaction = "Failed to commit the transaction";
        public static readonly ExceptionText FailedToRollbackTransaction = "Failed to rollback the transaction";
        public static readonly ExceptionText FailedToExecuteQuery = "Failed to execute the query";
        public static readonly ExceptionText FailedToExecuteQueryPattern = "Failed to execute the query patern '{0}'";

        public static readonly ExceptionText FailedToExecuteReaderQuery = "Failed to execute command with the result as reader";
        public static readonly ExceptionText FailedToExecuteNonQuery = "Failed to execute command without any result";
        public static readonly ExceptionText FailedToExecuteScalarQuery = "Failed to execute command with the result as scalar value";

        public static readonly ExceptionText NotSupportedFieldType = "Not supported field type '{0}'";
        public static readonly ExceptionText NotSupportedMethod = "Not supported method '{0}'";
    }

    static class ConfigParameters
    {
        public static readonly string DataContexts = "DataContexts";
    }
}
