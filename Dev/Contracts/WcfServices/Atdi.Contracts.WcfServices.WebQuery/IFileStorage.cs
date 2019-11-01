using System.ServiceModel;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;

namespace Atdi.Contracts.WcfServices.WebQuery
{
    /// <summary>
    /// The public contract of the WCF-service of the file storage
    /// </summary>
    [ServiceContract(Namespace = Specification.Namespace)]
    public interface IFileStorage
    {
        /// <summary>
        /// Gets the detailed file information
        /// </summary>
        /// <param name="userToken">The user auth token</param>
        /// <param name="id">The ID of the file descriptor obtained from ICSM</param>
        [OperationContract]
        Result<FileInfo> GetFileInfo(UserToken userToken, int id);

        /// <summary>
        /// Gets the detailed files information
        /// </summary>
        /// <param name="userToken">The user auth token</param>
        /// <param name="ids">The array of  ID of the file descriptor obtained from ICSM</param>
        [OperationContract]
        Result<FileInfo[]> GetFilesInfo(UserToken userToken, int[] ids);

        /// <summary>
        /// Gets the detailed file information
        /// </summary>
        /// <param name="userToken">The user auth token</param>
        /// <param name="id">The ID of the file descriptor obtained from ICSM</param>
        [OperationContract]
        Result<FileContent> GetFile(UserToken userToken, int id);

        /// <summary>
        /// Gets the part file
        /// </summary>
        /// <param name="userToken">The user auth token</param>
        /// <param name="id">The ID of the file descriptor obtained from ICSM</param>
        /// <param name="from">The starting position of reading from the file</param>
        /// <param name="to">The end position of reading from the file</param>
        [OperationContract]
        Result<byte[]> GeFilePart(UserToken userToken, int id, int from, int to);
    }
}
