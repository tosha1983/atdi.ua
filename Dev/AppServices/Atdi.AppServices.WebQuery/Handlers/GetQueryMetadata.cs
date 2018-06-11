
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;

namespace Atdi.AppServices.WebQuery.Handlers
{
    public sealed class GetQueryMetadata : LoggedObject
    {
        private readonly QueriesRepository _repository;
        private readonly IUserTokenProvider _tokenProvider;

        public GetQueryMetadata(QueriesRepository repository, IUserTokenProvider tokenProvider, ILogger logger) : base(logger)
        {
            this._repository = repository;
            this._tokenProvider = tokenProvider;
        }

        public QueryMetadata Handle(UserToken userToken, QueryToken queryToken)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.GetQueryMetadata))
            {
                var tokenData = this._tokenProvider.UnpackUserToken(userToken);
                var queryDescriptor = this._repository.GetQueryDescriptorByToken(tokenData, queryToken);
                return queryDescriptor.Metadata;
            }
        }
    }
}
