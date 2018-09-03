
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
    public sealed class GetQueryGroups : LoggedObject
    {
        private readonly QueriesRepository _repository;
        private readonly IUserTokenProvider _tokenProvider;

        public GetQueryGroups(QueriesRepository repository, IUserTokenProvider tokenProvider, ILogger logger) : base(logger)
        {
            this._repository = repository;
            this._tokenProvider = tokenProvider;
        }

        public QueryGroups Handle(UserToken userToken)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.GetQueryGroups))
            {
                var tokenData = this._tokenProvider.UnpackUserToken(userToken);
                return new QueryGroups()
                {
                    Groups = this._repository.GetGroupsByUser(tokenData)
                };
            }
        }
    }
}
