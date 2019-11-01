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
        private readonly IQueryExecutor _queryExecutor;

        public GetFilesInfo(IUserTokenProvider tokenProvider, IDataLayer<IcsmDataOrm> dataLayer, ILogger logger) : base(logger)
        {
            this._tokenProvider = tokenProvider;
            this._queryExecutor = dataLayer.Executor<IcsmDataContext>();
        }

        public FileInfo[] Handle(UserToken userToken, int[] ids)
        {
            
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.GetFilesInfo))
            {
                if (userToken == null) throw new ArgumentNullException(nameof(userToken));
                if (ids == null) throw new ArgumentNullException(nameof(ids));
                if (ids.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(ids));

                var tokenData = this._tokenProvider.UnpackUserToken(userToken);
                var result = new FileInfo[ids.Length];

                for (var i = 0; i < ids.Length; i++)
                {
                    // Here put code of read file information from ICSM DB by Id

                    result[i] = new FileInfo()
                    {
                        Id = ids[i],
                        Name = "FileName",
                        Extension = "ext",
                        Size = 1024
                    };
                }

                return result;
            }
        }
    }
}