using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Atdi.DataModels;
using Atdi.DataModels.WebQuery;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Identity;

namespace Atdi.Contracts.WcfServices.WebQuery
{
    /// <summary>
    /// The public contract of the WCF-service of the web query
    /// </summary>
    [ServiceContract(Namespace = Specification.Namespace)]
    public interface IWebQuery
    {
        /// <summary>
        /// Gets the tree of the queries by current user
        /// </summary>
        /// <param name="userToken">The token of the authenticated user</param>
        /// <returns>The tree of the web queries</returns>
        [OperationContract]
        Result<QueryGroups> GetQueryGroups(UserToken userToken);

        /// <summary>
        /// Gets the metadata of the result of the web query
        /// </summary>
        /// <param name="userToken">The token of the authenticated user</param>
        /// <param name="queryToken">The token of the web query</param>
        /// <returns>The metadata of the result of the web query</returns>
        [OperationContract]
        Result<QueryMetadata> GetQueryMetadata(UserToken userToken, QueryToken queryToken);

        /// <summary>
        /// Gets the metadata of the result of the web query
        /// </summary>
        /// <param name="userToken">The token of the authenticated user</param>
        /// <param name="queryCode">The code of the web query</param>
        /// <returns>The metadata of the result of the web query</returns>
        [OperationContract]
        Result<QueryMetadata> GetQueryMetadataByCode(UserToken userToken, string queryCode);

        /// <summary>
        /// Executes the web query
        /// </summary>
        /// <param name="userToken">The token of the authenticated user</param>
        /// <param name="queryToken">The token of the web query</param>
        /// <param name="fetchOptions">The options of the executable query</param>
        /// <returns>The result of the web query</returns>
        [OperationContract]
        Result<QueryResult> ExecuteQuery(UserToken userToken, QueryToken queryToken, FetchOptions fetchOptions);

        /// <summary>
        /// Saves changes
        /// </summary>
        /// <param name="userToken">The token of the authenticated user</param>
        /// <param name="queryToken">The token of the web query</param>
        /// <param name="changeset">The changeset of the web query</param>
        /// <returns></returns>
        [OperationContract]
        Result<ChangesResult> SaveChanges(UserToken userToken, QueryToken queryToken, Changeset changeset);
    }
}
