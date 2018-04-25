using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// The public contract of the service of the web query manager
    /// </summary>
    [ServiceContract(Namespace = ServicesSpecification.Namespace)]
    public interface IWebQueryManager
    {
        /// <summary>
        /// Authentication of the current user by user name and password
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns>The user Id</returns>
        [OperationContract]
        CommonOperationDataResult<int> AuthenticateUser(string userName, string password);
        

        /// <summary>
        /// Gets the metadata of the query page by current user
        /// </summary>
        /// <returns>The metadata of query page</returns>
        [OperationContract]
        PageMetadata GetPageMetadata(CommonOperationArguments otherArgs);

        /// <summary>
        /// Gets the tree of the queries by current user
        /// </summary>
        /// <returns>The tree of the web queries</returns>
        [OperationContract]
        QueryTree GetQueryTree(CommonOperationArguments otherArgs);

        /// <summary>
        /// Gets the metadata of the result of the web query
        /// </summary>
        /// <param name="queryRef">The reference to metadata of the web query</param>
        /// <returns>The metadata of the result of the web query</returns>
        [OperationContract]
        QueryMetadata GetQueryMetadata(QueryReference queryRef, CommonOperationArguments otherArgs);

        /// <summary>
        /// Executes the web query
        /// </summary>
        /// <param name="options">The options of the executable query</param>
        /// <returns>The result of the web query</returns>
        [OperationContract]
        QueryResult ExecuteQuery(QueryOptions options, CommonOperationArguments otherArgs);

        /// <summary>
        /// Saves changes
        /// </summary>
        /// <param name="changeset">The changeset of the web query</param>
        /// <returns></returns>
        [OperationContract]
        QueryChangesResult SaveChanges(QueryChangeset changeset, CommonOperationArguments otherArgs);
    }
}
