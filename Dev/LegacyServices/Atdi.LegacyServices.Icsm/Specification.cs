using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;

namespace Atdi.LegacyServices.Icsm
{
    static class Contexts
    {
        public static readonly EventContext LegacyServicesIcsm = "ICSM Legacy Services";
    }

    static class Categories
    {
        public static readonly EventCategory CreatingInstance = "Creating instance";
        public static readonly EventCategory FetchingData = "Fetching data";
        public static readonly EventCategory BuildingStatement = "Building SQL query";
        public static readonly EventCategory ParseIRP = "Parsing IRP";
    }

    static class Events
    {
        public static readonly EventText CreatedInstanceOfQueryExecutor = "Created instance of the ICSM ORM Query Executor";
        public static readonly EventText CreatedInstanceOfDataLayer = "Created instance of the ICSM ORM Data Layer";
        public static readonly EventText CreatedInstanceOfQueryBuilder = "Created instance of the ICSM ORM Query Builder";
    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName GetQueriesTree = "GetQueriesTree";
    }

    static class Exceptions
    {
        public static readonly ExceptionText ExpresionRefersToMemberThatNotFromType = "Expresion '{0}' refers to a member that is not from type {1}.";
        public static readonly ExceptionText ValueTypeNotSupported = "The value type {0} is not supported.";
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
        public static readonly ExceptionText NotRecognizeAlias = "The alias of main table not found {0}";
    }

    static class Parameters
    {
        public static readonly string SchemasPath = "SchemasPath";
        public static readonly string SchemaPrefix = "SchemaPrefix";
        public static readonly string Edition = "Edition";
        public static readonly string Schemas = "Schemas";
        public static readonly string Modules = "Modules";
    }
}
