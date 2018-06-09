using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;
using AppServices = Atdi.Contracts.AppServices.WebQuery;

namespace Atdi.WcfServices.WebQuery
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WebQuery : WcfServiceBase<IWebQuery>, IWebQuery
    {
        private readonly AppServices.IWebQuery _webQueryAppServices;
        public WebQuery(AppServices.IWebQuery webQueryAppServices)
        {
            this._webQueryAppServices = webQueryAppServices;
        }

        public Result<QueryResult> ExecuteQuery(UserToken userToken, QueryToken queryToken, FetchOptions fetchOptions)
        {
            var resultData = this._webQueryAppServices.ExecuteQuery(userToken, queryToken, fetchOptions);

            return new Result<QueryResult>
            {
                State = OperationState.Success,
                Data = resultData
            };
        }

        public Result<QueryGroups> GetQueryGroups(UserToken userToken)
        {
            var resultData = this._webQueryAppServices.GetQueryGroups(userToken);

            return new Result<QueryGroups>
            {
                State = OperationState.Success,
                Data = resultData
            };
        }
        
        public Result<QueryMetadata> GetQueryMetadata(UserToken userToken, QueryToken queryToken)
        {
            var resultData = this._webQueryAppServices.GetQueryMetadata(userToken, queryToken);

            return new Result<QueryMetadata>
            {
                State = OperationState.Success,
                Data = resultData
            };
        }

        public Result<QueryMetadata> GetQueryMetadataByCode(UserToken userToken, string queryCode)
        {
            var resultData = this._webQueryAppServices.GetQueryMetadataByCode(userToken, queryCode);

            return new Result<QueryMetadata>
            {
                State = OperationState.Success,
                Data = resultData
            };
        }

        public Result<ChangesResult> SaveChanges(UserToken userToken, QueryToken queryToken, Changeset changeset)
        {
            var resultData = this._webQueryAppServices.SaveChanges(userToken, queryToken, changeset);

            return new Result<ChangesResult>
            {
                State = OperationState.Success,
                Data = resultData
            };
        }
    }
}
