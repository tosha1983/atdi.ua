﻿using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;

namespace Atdi.CoreServices.EntityOrm
{
    static class Contexts
    {
        public static readonly EventContext DataLayerCoreServices = "DataLayer Core Services";
        public static readonly EventContext SqlServerEngine = "SQL Server Engine";
        public static readonly EventContext OracleEngine = "Oracle Engine";
        public static readonly EventContext LegacyServicesIcsm = "EntityOrm Legacy Services";
    }





    static class Categories
    {
        public static readonly EventCategory CreatingInstance = "Creating instance";
        public static readonly EventCategory FetchingData = "Fetching data";
        public static readonly EventCategory BuildingStatement = "Building SQL query";
        public static readonly EventCategory DataProcessing = "Processing data";
        public static readonly EventCategory CommandExecuting = "Executing command";
        public static readonly EventCategory ResultHandling = "Handling result";
        public static readonly EventCategory OpeningConnection = "Opening connection";
    }

    static class Events
    {
        public static readonly EventText CreatedInstanceOfQueryBuilder = "Created instance of the ICSM ORM Query Builder";
        public static readonly EventText CreatedInstanceOfQueryExecutor = "Created instance of the ICSM ORM Query Executor";
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
        public static readonly ExceptionText EngineTypeNotSupported = "The data engine with type '{0}' is not supported";
        public static readonly ExceptionText DataLimitTypeNotSupported = "The data limit with type '{0}' is not supported";
        public static readonly ExceptionText SortDirectionNotSupported = "The sort direction with name '{0}' is not supported";
        public static readonly ExceptionText LogicalOperatorNotSupported = "The logical operator with name '{0}' is not supported";
        public static readonly ExceptionText NotRecognizeAlias = "The alias of main table not found {0}";
        public static readonly ExceptionText ValueTypeNotSupported = "The value type {0} is not supported.";
        public static readonly ExceptionText ExpresionRefersToMemberThatNotFromType = "Expresion '{0}' refers to a member that is not from type {1}.";
        public static readonly ExceptionText ColumnValueTypeNotSupported = "The value type {0} is not supported for column with name '{1}'.";
        public static readonly ExceptionText ExpressionNotSupported = "The expression '{0}' is not supported.";
        public static readonly ExceptionText ExpressionTypeNotSupported = "The expression type {0} is not supported.";
        public static readonly ExceptionText ExpressionNodeTypeNotSupported = "The expression node type {0} is not supported.";
        public static readonly ExceptionText ExpressionCallMethodNotSupported = "The expression call method {0} is not supported.";
        public static readonly ExceptionText MemberNameIsNotDefined = "A member name in the expression '{0}' is not defined.";
        public static readonly ExceptionText DataSetStructureNotSupported = "The data set structure '{0}' is not supported.";
        public static readonly ExceptionText ParsingIRPFile = "Error while parsing the IRP file";
        public static readonly ExceptionText InvalideInitializeIcsmEnvironment = "Invalide initialize the environment of ICSM ORM";
        public static readonly ExceptionText AbortedBuildSelectStatement = "Aborted sql query building for data selection";
        public static readonly ExceptionText AbortedBuildDeleteStatement = "Aborted sql query building for data deletion";
        public static readonly ExceptionText AbortedBuildUpdateStatement = "Aborted sql query building for data updateion";
        public static readonly ExceptionText AbortedBuildInsertStatement = "Aborted sql query building for data creation";
        public static readonly ExceptionText NotFoundOrmField = "Not found ORM field with path '{0}' into table with name '{1}'";
        public static readonly ExceptionText UndefinedParameter = "Undefined parameter with name '{0}'";
        public static readonly ExceptionText QueryStatementNotSupported = "The type of query statement with name '{0}' is not supported.";
        public static readonly ExceptionText NotRecognizeTypeField = "The type filed {0} not found";
    }

    static class ConfigParameters
    {
        public static readonly string DataContexts = "DataContexts";
    }
}
