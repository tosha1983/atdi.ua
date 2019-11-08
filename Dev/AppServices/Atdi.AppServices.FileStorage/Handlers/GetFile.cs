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
    public sealed class GetFile : LoggedObject
    {

        private readonly IUserTokenProvider _tokenProvider;
        private ILogger _logger { get; set; }
        private IDataLayer<IcsmDataOrm> _dataLayer { get; set; }
        private ConfigFileStorage _configFileStorage { get; set; }

        public GetFile(IUserTokenProvider tokenProvider, IDataLayer<IcsmDataOrm> dataLayer, ConfigFileStorage configFileStorage, ILogger logger) : base(logger)
        {
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._dataLayer = dataLayer;
            this._configFileStorage = configFileStorage;
        }


        public FileContent Handle(UserToken userToken, int id)
        {
            var fileContent = new FileContent();
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.GetFile))
            {
                if (userToken == null) throw new ArgumentNullException(nameof(userToken));

                var tokenData = this._tokenProvider.UnpackUserToken(userToken);

                // Here put code of read whole file from ICSM DB by Id

                var getFileInfo = new GetFileInfo(this._tokenProvider, this._dataLayer, this._configFileStorage, this._logger);

                var fileInfo = getFileInfo.Handle(userToken, id);
                fileContent.FileExtension = fileInfo.FileExtension;
                fileContent.Id = id;
                fileContent.FileName = fileInfo.FileName;
                fileContent.Size = fileInfo.Size;
                fileContent.DateCreated = fileInfo.DateCreated;
                fileContent.TypeDocument = fileInfo.TypeDocument;
                fileContent.CreatedBy = fileInfo.CreatedBy;
                fileContent.DocMemo = fileInfo.DocMemo;
                fileContent.DocName = fileInfo.DocName;
                fileContent.PathDocument = fileInfo.PathDocument;
                var fullPathFile = fileContent.PathDocument + "\\" + fileContent.FileName + fileContent.FileExtension;
                fileContent.Body = System.IO.File.ReadAllBytes(fullPathFile);
                return fileContent;
            }
        }
    }
}