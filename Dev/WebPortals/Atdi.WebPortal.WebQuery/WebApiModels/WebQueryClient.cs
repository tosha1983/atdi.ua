using Atdi.WebPortal.WebQuery.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    public class WebQueryClient : WebApiClient
    {
        public WebQueryClient(IOptions<PortalSettings> options) : base(new Uri(options.Value.WebQueryApiUrl))
        {
        }

        public async Task<UserIdentity> AuthenticateUserAsync(UserCredential userCredential)
        {
            var options = new Options.AuthenticateUserOptions
            {
                Credential = userCredential
            };

            var uri = this.CreateRequestUri("/api/AuthenticationManager/AuthenticateUser");
            return await this.PostAsync<UserIdentity, Options.AuthenticateUserOptions>(uri, options);
        }

        public async Task<QueryGroups> GetQueryGroupsAsync(UserToken userToken)
        {
            var options = new Options.GetQueryGroupsOptions
            {
                UserToken = userToken
            };

            var uri = this.CreateRequestUri("/api/WebQuery/GetQueryGroups");
            return await this.PostAsync<QueryGroups, Options.GetQueryGroupsOptions>(uri, options);
        }

        public async Task<QueryMetadata[]> GetQueriesMetadataAsync(UserToken userToken, QueryToken[] tokens)
        {
            var options = new Options.GetQueriesMetadataOptions
            {
                UserToken = userToken,
                QueryTokens = tokens
            };

            var uri = this.CreateRequestUri("/api/WebQuery/GetQueriesMetadata");
            return await this.PostAsync<QueryMetadata[], Options.GetQueriesMetadataOptions>(uri, options);
        }

        public async Task<object> ExecuteQueryAsync(UserToken userToken, QueryToken token)
        {
            var options = new Options.ExecuteQueryOptions
            {
                UserToken = userToken,
                QueryToken = token,
                ResultStructure = DataSetStructure.StringCells
            };

            var uri = this.CreateRequestUri("/api/WebQuery/ExecuteQuery");
            return await this.PostAsync<object, Options.ExecuteQueryOptions>(uri, options);
        }
    }
}
