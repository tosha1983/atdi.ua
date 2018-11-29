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

        public async Task<object> ExecuteQueryAsync(UserToken userToken, QueryToken token, string[] columns, Filter filter, OrderExpression[] orders, DataLimit limit)
        {
            var options = new Options.ExecuteQueryOptions
            {
                UserToken = userToken,
                QueryToken = token,
                ResultStructure = DataSetStructure.ObjectCells,
                Columns = columns,
                Filter = filter,
                Orders = orders,
                Limit = limit
            };

            var uri = this.CreateRequestUri("/api/WebQuery/ExecuteQuery");
            return await this.PostAsync<object, Options.ExecuteQueryOptions>(uri, options);
        }

        public async Task<ActionResult> ExecuteActionAsync(UserToken userToken, QueryToken token, DataChangeAction action)
        {
            var options = new Options.SaveChangesOptions
            {
                UserToken = userToken,
                QueryToken = token,
                Changeset = new DataChangeset
                {
                    ChangesetId = Guid.NewGuid(),
                    Actions = new DataChangeAction[] { action }
                }
            };

            var uri = this.CreateRequestUri("/api/WebQuery/SaveChanges");
            var result = await this.PostAsync<ChangesResult, Options.SaveChangesOptions>(uri, options);

            return result.Actions[0];
        }
    }
}
