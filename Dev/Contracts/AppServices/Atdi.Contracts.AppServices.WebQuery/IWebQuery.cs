using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;

namespace Atdi.Contracts.AppServices.WebQuery
{
    public interface IWebQuery
    {
        QueriesTree GetQueriesTree(UserToken userToken);

        QueryMetadata GetQueryMetadata(UserToken userToken, QueryToken queryToken);

        QueryResult ExecuteQuery(UserToken userToken, QueryToken queryToken, FetchOptions fetchOptions);

        ChangesResult SaveChanges(UserToken userToken, QueryToken queryToken, Changeset changeset);
    }
}
