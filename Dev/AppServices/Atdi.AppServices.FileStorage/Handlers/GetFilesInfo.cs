using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;


namespace Atdi.AppServices.FileStorage.Handlers
{
    public sealed class GetFilesInfo : LoggedObject
    {
        private readonly IUserTokenProvider _tokenProvider;
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private ILogger _logger { get; set; }
        private ConfigFileStorage _configFileStorage { get; set; }


        public GetFilesInfo(IUserTokenProvider tokenProvider, IDataLayer<IcsmDataOrm> dataLayer, ConfigFileStorage configFileStorage, ILogger logger) : base(logger)
        {
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._dataLayer = dataLayer;
            this._configFileStorage = configFileStorage;
        }


        public FileInfo[] Handle(UserToken userToken, int[] ids)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.GetFilesInfo))
            {
                if (userToken == null) throw new ArgumentNullException(nameof(userToken));
                if (ids == null) throw new ArgumentNullException(nameof(ids));
                if (ids.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(ids));

                var tokenData = this._tokenProvider.UnpackUserToken(userToken);
                var getFileInfo = new GetFileInfo(this._tokenProvider, this._dataLayer, this._configFileStorage, this._logger);
                var result = new FileInfo[ids.Length];

                for (var i = 0; i < ids.Length; i++)
                {
                    // Here put code of read file information from ICSM DB by Id
                    result[i] = new FileInfo();
                    var fileInfo = getFileInfo.Handle(userToken, ids[i]);
                    result[i].FileExtension = fileInfo.FileExtension;
                    result[i].Id = ids[i];
                    result[i].FileName = fileInfo.FileName;
                    result[i].Size = fileInfo.Size;
                    result[i].DateCreated = fileInfo.DateCreated;
                    result[i].TypeDocument = fileInfo.TypeDocument;
                    result[i].CreatedBy = fileInfo.CreatedBy;
                    result[i].DocMemo = fileInfo.DocMemo;
                    result[i].DocName = fileInfo.DocName;
                    result[i].PathDocument = fileInfo.PathDocument;
                }
                return result;
            }
        }
    }
}