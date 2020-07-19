using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;

namespace Atdi.CoreServices.AuthService.IcsmViisp
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "IcsmViispAuthService";
    }

    static class Categories
    {
        public static readonly EventCategory Handling = "Handling";
    }
       
    static class Events
    {
        public static readonly EventText RedirectionQuery = "Redirection query: {0}";
        public static readonly EventText InformationAboutAuthenticatedUser = "Information about authenticated user: RegistNum = {0}, Name = {1}, Email = {2} ";
        public static readonly EventText CreatedNewUser = "Created new user: Id = {0}, RegistNum = {1}, Name = {2}, Email = {3} ";
    }
    static class TraceScopeNames
    {

    }

    static class Exceptions
    {
        public static readonly ExceptionText ResponseInformationDataAuthenticationAttributeZero = "'ResponseInformationData.AuthenticationAttribute' length = 0";
        public static readonly ExceptionText ResponseInformationDataIsNull = "'ResponseInformationData' is null";
        public static readonly ExceptionText GetResponseAuthenticationDataFaultString = "Method 'GetResponseAuthenticationData' return fault string '{0}'";
        public static readonly ExceptionText ResponseUrlNotContainsParametersTicketOrCustomData = "Response Url not contains parameters 'ticket' or 'customData'";
        public static readonly ExceptionText UnexpectedErrorOccurredDuringMethodExecution = "Unexpected error occurred during method execution (UserTokenData is null)";
        public static readonly ExceptionText PrepareAuthRedirectionFaultString = "Method 'PrepareAuthRedirection' return fault string '{0}'";
    }

}
