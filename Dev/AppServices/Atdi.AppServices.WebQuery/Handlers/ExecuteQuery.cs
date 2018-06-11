using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.Identity;

namespace Atdi.AppServices.WebQuery.Handlers
{
    public sealed class ExecuteQuery : LoggedObject
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly QueriesRepository _repository;
        private readonly IUserTokenProvider _tokenProvider;


        public ExecuteQuery(QueriesRepository repository, IUserTokenProvider tokenProvider, IDataLayer<IcsmDataOrm> dataLayer, ILogger logger) : base(logger)
        {
            this._repository = repository;
            this._tokenProvider = tokenProvider;
            this._dataLayer = dataLayer;
        }

        public QueryResult Handle(UserToken userToken, QueryToken queryToken, FetchOptions fetchOptions)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.ExecuteQuery))
            {
                var tokenData = this._tokenProvider.UnpackUserToken(userToken);
                var queryDescriptor = this._repository.GetQueryDescriptorByToken(tokenData, queryToken);

                
                return null;
            }
        }

        
    }
}
