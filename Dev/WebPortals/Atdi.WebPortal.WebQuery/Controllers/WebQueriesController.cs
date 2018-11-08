using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atdi.WebPortal.WebQuery.WebApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Atdi.WebPortal.WebQuery.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WebQueriesController : ControllerBase
    {
        public class GetQueriesMetadataByTokensOptions
        {
            public QueryToken[] Tokens { get; set; }
        }

        public class ExecuteOptions
        {
            public QueryToken Token { get; set; }
        }

        private readonly PortalSettings _portalSettings;
        private readonly WebQueryClient _webQueryClient;

        public WebQueriesController(IOptions<PortalSettings> options, WebQueryClient webQueryClient)
        {
            this._portalSettings = options.Value;
            this._webQueryClient = webQueryClient;
        }

        // GET api/environment
        [HttpPost]
        [Authorize]
        public async Task<QueryMetadata[]> Get(GetQueriesMetadataByTokensOptions options)
        {
            var claim = this.HttpContext.User.FindFirst(c => "WebQueryUserTokenData".Equals(c.Type, StringComparison.OrdinalIgnoreCase));
            var userToken = new UserToken
            {
                Data = Convert.FromBase64String(claim.Value)
            };

            return
                await _webQueryClient.GetQueriesMetadataAsync(userToken, options.Tokens);
        }

        [HttpPost]
        [Authorize]
        public async Task<object> Execute(ExecuteOptions options)
        {
            var claim = this.HttpContext.User.FindFirst(c => "WebQueryUserTokenData".Equals(c.Type, StringComparison.OrdinalIgnoreCase));
            var userToken = new UserToken
            {
                Data = Convert.FromBase64String(claim.Value)
            };

            return
                await _webQueryClient.ExecuteQueryAsync(userToken, options.Token);
        }
    }
}