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
    [Route("portal/api/[controller]/[action]")]
    [Route("portal/index/api/[controller]/[action]")]
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

            public string[] Columns { get; set; }

            public Filter Filter { get; set; }

            public OrderExpression[] Orders { get; set; }

            public DataLimit Limit { get; set; }
        }

        public class UpdateRecordOptions
        {
            public QueryToken Token { get; set; }

            public DataSetColumn[] Columns { get; set; }

            public Filter Filter { get; set; }

            public object[] Cells { get; set; }
        }

        public class CreateRecordOptions
        {
            public QueryToken Token { get; set; }

            public DataSetColumn[] Columns { get; set; }

            public object[] Cells { get; set; }
        }

        public class DeleteRecordOptions
        {
            public QueryToken Token { get; set; }

            public Filter Filter { get; set; }
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
                await _webQueryClient.ExecuteQueryAsync(userToken, options.Token, options.Columns, options.Filter, options.Orders, options.Limit);
        }

        [HttpPost]
        [Authorize]
        public async Task<WebApiModels.ActionResult> CreateRecord(CreateRecordOptions options)
        {
            var claim = this.HttpContext.User.FindFirst(c => "WebQueryUserTokenData".Equals(c.Type, StringComparison.OrdinalIgnoreCase));
            var userToken = new UserToken
            {
                Data = Convert.FromBase64String(claim.Value)
            };

            var action = new DataChangeAction
            {
                ActionId = Guid.NewGuid(),
                Columns = options.Columns,
                Type = ActionType.Create,
                RowType = DataRowType.ObjectCell,
                ObjectRow = new ObjectDataRow
                {
                    Cells = options.Cells
                }
            };

            return
                await _webQueryClient.ExecuteActionAsync(userToken, options.Token, action);
        }

        [HttpPost]
        [Authorize]
        public async Task<WebApiModels.ActionResult> UpdateRecord(UpdateRecordOptions options)
        {
            var claim = this.HttpContext.User.FindFirst(c => "WebQueryUserTokenData".Equals(c.Type, StringComparison.OrdinalIgnoreCase));
            var userToken = new UserToken
            {
                Data = Convert.FromBase64String(claim.Value)
            };

            var action = new DataChangeAction
            {
                ActionId = Guid.NewGuid(),
                Columns = options.Columns,
                Filter = options.Filter,
                Type = ActionType.Update,
                RowType = DataRowType.ObjectCell,
                ObjectRow = new ObjectDataRow
                {
                    Cells = options.Cells
                }
            };

            return
                await _webQueryClient.ExecuteActionAsync(userToken, options.Token, action);
        }

        [HttpPost]
        [Authorize]
        public async Task<WebApiModels.ActionResult> DeleteRecord(DeleteRecordOptions options)
        {
            var claim = this.HttpContext.User.FindFirst(c => "WebQueryUserTokenData".Equals(c.Type, StringComparison.OrdinalIgnoreCase));
            var userToken = new UserToken
            {
                Data = Convert.FromBase64String(claim.Value)
            };

            var action = new DataChangeAction
            {
                ActionId = Guid.NewGuid(),
                Filter = options.Filter,
                Type = ActionType.Delete
            };

            return
                await _webQueryClient.ExecuteActionAsync(userToken, options.Token, action);
        }
    }
}