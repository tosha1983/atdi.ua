using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;

namespace Atdi.Contracts.AppServices.WebQuery
{
    public interface IWebQuery
    {
        QueryGroups GetQueryGroups(UserToken userToken);

        QueryMetadata GetQueryMetadata(UserToken userToken, QueryToken queryToken);

        QueryMetadata GetQueryMetadataByCode(UserToken userToken, string queryCode);

        QueryResult ExecuteQuery(UserToken userToken, QueryToken queryToken, FetchOptions fetchOptions);

        ChangesResult SaveChanges(UserToken userToken, QueryToken queryToken, Changeset changeset);
    }
}
