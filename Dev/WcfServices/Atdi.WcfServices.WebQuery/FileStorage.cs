using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;
using AppServices = Atdi.Contracts.AppServices.WebQuery;

namespace Atdi.WcfServices.WebQuery
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FileStorage : WcfServiceBase<IFileStorage>, IFileStorage
    {
        private readonly AppServices.IFileStorage _fileStorageAppServices;
        private readonly ILogger _logger;

        public FileStorage(AppServices.IFileStorage fileStorageAppServices, ILogger logger)
        {
            this._fileStorageAppServices = fileStorageAppServices;
            this._logger = logger;
        }

        public Result<FileInfo> GetFileInfo(UserToken userToken, int id)
        {
            try
            {
                var resultData = this._fileStorageAppServices.GetFileInfo(userToken, id);

                return new Result<FileInfo>
                {
                    State = OperationState.Success,
                    Data = resultData
                };
            }
            catch (Exception e)
            {
                _logger.Exception("FileStorage", "GetFileInfo", e, this);

                return new Result<FileInfo>()
                {
                    State = OperationState.Fault,
                    FaultCause = e.Message
                };
            }
        }

        public Result<FileInfo[]> GetFilesInfo(UserToken userToken, int[] ids)
        {
            try
            {
                var resultData = this._fileStorageAppServices.GetFilesInfo(userToken, ids);

                return new Result<FileInfo[]>
                {
                    State = OperationState.Success,
                    Data = resultData
                };
            }
            catch (Exception e)
            {
                _logger.Exception("FileStorage", "GetFilesInfo", e, this);

                return new Result<FileInfo[]>()
                {
                    State = OperationState.Fault,
                    FaultCause = e.Message
                };
            }
        }

        public Result<FileContent> GetFile(UserToken userToken, int id)
        {
            try
            {
                var resultData = this._fileStorageAppServices.GetFile(userToken, id);

                return new Result<FileContent>
                {
                    State = OperationState.Success,
                    Data = resultData
                };
            }
            catch (Exception e)
            {
                _logger.Exception("FileStorage", "GetFile", e, this);

                return new Result<FileContent>()
                {
                    State = OperationState.Fault,
                    FaultCause = e.Message
                };
            }
        }

        public Result<byte[]> GeFilePart(UserToken userToken, int id, int from, int to)
        {
            try
            {
                var resultData = this._fileStorageAppServices.GeFilePart(userToken, id, from, to);

                return new Result<byte[]>
                {
                    State = OperationState.Success,
                    Data = resultData
                };
            }
            catch (Exception e)
            {
                _logger.Exception("FileStorage", "GeFilePart", e, this);

                return new Result<byte[]>()
                {
                    State = OperationState.Fault,
                    FaultCause = e.Message
                };
            }
        }
    }
}
