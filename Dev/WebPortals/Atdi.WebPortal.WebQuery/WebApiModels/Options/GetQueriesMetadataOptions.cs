using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels.Options
{
    public class GetQueriesMetadataOptions
    {
        public UserToken UserToken { get; set; }

        public QueryToken[] QueryTokens { get; set; }
    }
}
