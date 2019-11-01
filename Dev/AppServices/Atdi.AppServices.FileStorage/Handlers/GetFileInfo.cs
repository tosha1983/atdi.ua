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
    public sealed class GetFileInfo : LoggedObject
    {
        private readonly IUserTokenProvider _tokenProvider;
        private readonly IQueryExecutor _queryExecutor;

        public GetFileInfo(IUserTokenProvider tokenProvider, IDataLayer<IcsmDataOrm> dataLayer, ILogger logger) : base(logger)
        {
            this._tokenProvider = tokenProvider;
            this._queryExecutor = dataLayer.Executor<IcsmDataContext>();
        }

        public FileInfo Handle(UserToken userToken, int id)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.GetFileInfo))
            {
                if (userToken == null) throw new ArgumentNullException(nameof(userToken));

                var tokenData = this._tokenProvider.UnpackUserToken(userToken);

                // Here put code of read file information from ICSM DB by Id

                return new FileInfo()
                {
                    Id = id,
                    Name = "FileName",
                    Extension = "ext",
                    Size = 1024
                };
            }
        }
    }
}
