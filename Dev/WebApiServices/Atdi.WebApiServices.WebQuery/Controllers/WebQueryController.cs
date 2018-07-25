using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Atdi.Contracts.AppServices.WebQuery;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;

namespace Atdi.WebApiServices.WebQuery.Controllers
{
    public class WebQueryController : WebApiController
    {
        public class GetQueryGroupsOptions
        {
            public UserToken UserToken { get; set; }
        }

        public class GetQueryMetadataOptions
        {
            public UserToken UserToken { get; set; }

            public QueryToken QueryToken { get; set; }
        }

        public class GetQueryMetadataByCodeOptions
        {
            public UserToken UserToken { get; set; }

            public string QueryCode { get; set; }
        }

        public class ExecuteQueryOptions
        {
            public UserToken UserToken { get; set; }

            public QueryToken QueryToken { get; set; }

            public Guid OptionId { get; set; }

            public DataSetStructure ResultStructure { get; set; }

            public string[] Columns { get; set; }

            public Filter Filter { get; set; }

            public OrderExpression[] Orders { get; set; }

            public DataLimit Limit { get; set; }
        }

        public class SaveChangesOptions
        {
            public UserToken UserToken { get; set; }

            public QueryToken QueryToken { get; set; }

            public DataChangeset Changeset { get; set; }
        }

        private readonly IWebQuery _webQueryAppServices;

        public WebQueryController(IWebQuery webQueryAppServices, ILogger logger) : base(logger)
        {
            this._webQueryAppServices = webQueryAppServices;
        }

        [HttpPost]
        public QueryGroups GetQueryGroups(GetQueryGroupsOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.UserToken == null)
            {
                throw new ArgumentNullException(nameof(options.UserToken));
            }

            using (this.Logger.StartTrace(Contexts.WebQuery, Categories.OperationCall, TraceScopeNames.GetQueryGroups))
            {
                var resultData = this._webQueryAppServices.GetQueryGroups(options.UserToken);
                return resultData;
            }
        }

        [HttpPost]
        public QueryMetadata GetQueryMetadata(GetQueryMetadataOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.UserToken == null)
            {
                throw new ArgumentNullException(nameof(options.UserToken));
            }
            if (options.QueryToken == null)
            {
                throw new ArgumentNullException(nameof(options.QueryToken));
            }
            using (this.Logger.StartTrace(Contexts.WebQuery, Categories.OperationCall, TraceScopeNames.GetQueryMetadata))
            {
                var resultData = this._webQueryAppServices.GetQueryMetadata(options.UserToken, options.QueryToken);
                return resultData;
            }
        }

        [HttpPost]
        public QueryMetadata GetQueryMetadataByCode(GetQueryMetadataByCodeOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.UserToken == null)
            {
                throw new ArgumentNullException(nameof(options.UserToken));
            }
            if (string.IsNullOrEmpty(options.QueryCode))
            {
                throw new ArgumentNullException(nameof(options.QueryCode));
            }

            using (this.Logger.StartTrace(Contexts.WebQuery, Categories.OperationCall, TraceScopeNames.GetQueryMetadataByCode))
            {
                var resultData = this._webQueryAppServices.GetQueryMetadataByCode(options.UserToken, options.QueryCode);
                return resultData;
            }
            
        }

        [HttpPost]
        public QueryResult ExecuteQuery(ExecuteQueryOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.UserToken == null)
            {
                throw new ArgumentNullException(nameof(options.UserToken));
            }
            if (options.QueryToken == null)
            {
                throw new ArgumentNullException(nameof(options.QueryToken));
            }
            using (this.Logger.StartTrace(Contexts.WebQuery, Categories.OperationCall, TraceScopeNames.ExecuteQuery))
            {
                var fetchOptions = new FetchOptions
                {
                    Id = options.OptionId,
                    Columns = options.Columns,
                    Condition = (Condition)options.Filter,
                    ResultStructure = options.ResultStructure,
                    Orders = options.Orders,
                    Limit = options.Limit
                };
                var resultData = this._webQueryAppServices.ExecuteQuery(options.UserToken, options.QueryToken, fetchOptions);
                return resultData;
            }

        }

        [HttpPost]
        public ChangesResult SaveChanges(SaveChangesOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.UserToken == null)
            {
                throw new ArgumentNullException(nameof(options.UserToken));
            }
            if (options.QueryToken == null)
            {
                throw new ArgumentNullException(nameof(options.QueryToken));
            }
            if (options.Changeset == null)
            {
                throw new ArgumentNullException(nameof(options.Changeset));
            }

            using (this.Logger.StartTrace(Contexts.WebQuery, Categories.OperationCall, TraceScopeNames.ExecuteQuery))
            {
                var changeset = (Changeset)options.Changeset;
                var resultData = this._webQueryAppServices.SaveChanges(options.UserToken, options.QueryToken, changeset);
                return resultData;

            }
        }
    }
}
