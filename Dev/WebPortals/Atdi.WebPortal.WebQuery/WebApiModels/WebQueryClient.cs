using Atdi.WebPortal.WebQuery.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    public class WebQueryClient : WebApiClient
    {
        public WebQueryClient(Uri baseEndpoint) : base(baseEndpoint)
        {
        }

        public async Task<UserIdentity> AuthenticateUserAsync(UserCredential userCredential)
        {
            var options = new AuthenticateUserOptions
            {
                Credential = userCredential
            };

            var uri = this.CreateRequestUri("/api/AuthenticationManager/AuthenticateUser");
            return await this.PostAsync<UserIdentity, AuthenticateUserOptions>(uri, options);
        }
    }
}
