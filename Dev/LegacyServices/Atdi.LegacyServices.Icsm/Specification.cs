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
    }

    static class Events
    {
        public static readonly EventText CreatedInstanceOfQueryExecutor = "Created instance of the ICSM ORM Query Executor";
    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName GetQueriesTree = "GetQueriesTree";
    }

    static class Exceptions
    {
        public static readonly ExceptionText ExpresionRefersToMemberThatNotFromType = "Expresion '{0}' refers to a member that is not from type {1}.";
        //public static readonly ExceptionText InvalidUserPassword = "Invalid password for user with name '{0}'";
    }

}
