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
    public sealed class GeFilePart : LoggedObject
    {

        private readonly IUserTokenProvider _tokenProvider;
        private readonly IQueryExecutor _queryExecutor;
        private ILogger _logger { get; set; }
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private ConfigFileStorage _configFileStorage { get; set; }

        public GeFilePart(IUserTokenProvider tokenProvider, IDataLayer<IcsmDataOrm> dataLayer, ConfigFileStorage configFileStorage, ILogger logger) : base(logger)
        {
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._configFileStorage = configFileStorage;
        }

        public byte[] Handle(UserToken userToken, int id, int from, int to)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.GeFilePart))
            {
                var retByte = new byte[to - from + 1];
                if (userToken == null) throw new ArgumentNullException(nameof(userToken));

                var tokenData = this._tokenProvider.UnpackUserToken(userToken);

                // Here put code of read part file from ICSM DB by Id

                var getFileInfo = new GetFileInfo(this._tokenProvider, this._dataLayer, this._configFileStorage, this._logger);
                var fileInfo = getFileInfo.Handle(userToken, id);

                if ((from >= 0) && (to< fileInfo.Size))
                {
                    var fullPathFile = fileInfo.PathDocument + "\\" + fileInfo.FileName + fileInfo.FileExtension;
                    var stream = System.IO.File.OpenRead(fullPathFile);
                    stream.Seek(0, System.IO.SeekOrigin.Begin); 
                    stream.Read(retByte, 0, to - from + 1);
                }
                else
                {
                    throw new InvalidOperationException($"Condition '((from >= 0) && (to < {fileInfo.Size}))' not satisfied");
                }
                return retByte;
            }
        }
    }
}