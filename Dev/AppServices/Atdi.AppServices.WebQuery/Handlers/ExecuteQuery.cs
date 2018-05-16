
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;

namespace Atdi.AppServices.WebQuery.Handlers
{
    public sealed class ExecuteQuery : LoggedObject
    {
        public ExecuteQuery(ILogger logger) : base(logger)
        {
        }

        public QueryResult Handle(UserToken userToken, QueryToken queryToken, FetchOptions fetchOptions)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.ExecuteQuery))
            {
                return new QueryResult() {   };
            }
        }
    }
}
