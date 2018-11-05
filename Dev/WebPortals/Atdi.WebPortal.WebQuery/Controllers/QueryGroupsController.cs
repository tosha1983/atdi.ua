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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QueryGroupsController : ControllerBase
    {
        private readonly PortalSettings _portalSettings;
        private readonly WebQueryClient _webQueryClient;

        public QueryGroupsController(IOptions<PortalSettings> options, WebQueryClient webQueryClient)
        {
            this._portalSettings = options.Value;
            this._webQueryClient = webQueryClient;
        }

        // GET api/environment
        [HttpGet]
        [Authorize]
        public async Task<QueryGroups> Get()
        {
            var claim = this.HttpContext.User.FindFirst(c => "WebQueryUserTokenData".Equals(c.Type, StringComparison.OrdinalIgnoreCase));
            var token = new UserToken
            {
                Data = Convert.FromBase64String(claim.Value)
            };

            return
                await _webQueryClient.GetQueryGroupsAsync(token);
        }
    }
}