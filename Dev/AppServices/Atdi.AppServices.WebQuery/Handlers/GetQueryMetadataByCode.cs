
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
    public sealed class GetQueryMetadataByCode : LoggedObject
    {
        public GetQueryMetadataByCode(ILogger logger) : base(logger)
        {
        }

        public QueryMetadata Handle(UserToken userToken, string queryCode)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.GetQueryMetadataByCode))
            {
                return new QueryMetadata() { Token = new QueryToken { Id = 0, Version = "1111.44" } };
            }
        }
    }
}
