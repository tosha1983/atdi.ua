using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.WebQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery
{
    public sealed class QueryDescriptorsCache
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IIrpParser _irpParser;
        private readonly Dictionary<int, QueryDescriptor> _queryDescriptors;

        public QueryDescriptorsCache(IDataLayer<IcsmDataOrm> dataLayer, IIrpParser irpParser)
        {
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._irpParser = irpParser;
            this._queryDescriptors = new Dictionary<int, QueryDescriptor>();
        }

        public QueryDescriptor GetDecriptor(QueryTokenDescriptor queryToken)
        {
            var queryId = queryToken.Token.Id;
            if (!this._queryDescriptors.TryGetValue(queryId, out QueryDescriptor descriptor))
            {
                descriptor = this.LoadQueryDescriptor(queryId);
                this._queryDescriptors[queryId] = descriptor;
            }
            return descriptor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryId"></param>
        /// <returns></returns>
        public QueryDescriptor LoadQueryDescriptor(int queryId)
        {
            QueryDescriptor description = null;
            var query_web_constraint = _dataLayer.Builder
              .From<XWEBCONSTRAINT>()
              .Where(c => c.WEBQUERYID, ConditionOperator.Equal, queryId)
              .Select(
                  c => c.ID,
                  c => c.INCLUDE,
                  c => c.MAX,
                  c => c.MIN,
                  c => c.NAME,
                  c => c.PATH,
                  c => c.STRVALUE,
                  c => c.WEBQUERYID,
                  c => c.DATEVALUEMIN,
                  c => c.DATEVALUEMAX
                 );

            var ConstraintsValue = this._queryExecutor
            .Fetch(query_web_constraint, reader =>
            {
                var groups_constr = new List<XWEBCONSTRAINT>();
                while (reader.Read())
                {
                    var token = new XWEBCONSTRAINT
                    {
                        ID = reader.GetValue(x => x.ID),
                        NAME = reader.GetValue(x => x.NAME),
                        STRVALUE = reader.GetValue(x => x.STRVALUE),
                        WEBQUERYID = reader.GetValue(x => x.WEBQUERYID),
                        INCLUDE = reader.GetValue(x => x.INCLUDE),
                        MIN = reader.GetValue(x => x.MIN),
                        MAX = reader.GetValue(x => x.MAX),
                        DATEVALUEMIN = reader.GetValue(x => x.DATEVALUEMIN),
                        DATEVALUEMAX = reader.GetValue(x => x.DATEVALUEMAX),
                        PATH = reader.GetValue(x => x.PATH)
                    };
                    groups_constr.Add(token);
                }
                return groups_constr.ToArray();
            });

            var queryweb = _dataLayer.Builder
               .From<XWEBQUERY>()
               .Where(c => c.ID, ConditionOperator.Equal, queryId)
               .Select(
                   c => c.ID,
                   c => c.CODE,
                   c => c.COMMENTS,
                   c => c.IDENTUSER,
                   c => c.NAME,
                   c => c.QUERY,
                   c => c.TASKFORCEGROUP
                  );

            var QueryValue = this._queryExecutor
           .Fetch(queryweb, reader => {
               var Value = new XWEBQUERY();
               if (reader.Read())
               {
                   Value.ID = reader.GetValue(x => x.ID);
                   Value.CODE = reader.GetValue(x => x.CODE);
                   Value.COMMENTS = reader.GetValue(x => x.COMMENTS);
                   Value.IDENTUSER = reader.GetValue(x => x.IDENTUSER);
                   Value.NAME = reader.GetValue(x => x.NAME);
                   Value.QUERY = reader.GetValue(x => x.QUERY);
                   Value.TASKFORCEGROUP = reader.GetValue(x => x.TASKFORCEGROUP);
               }
               return Value;
           });
            description = new QueryDescriptor(QueryValue, ConstraintsValue);
            description.Metadata = new QueryMetadata();
            if (QueryValue.QUERY != null) description.Metadata.Columns = this._irpParser.ExecuteParseQuery(QueryValue.QUERY);
            description.Metadata.Name = QueryValue.NAME;
            description.Metadata.Code = QueryValue.CODE;
            description.Metadata.Token = new QueryToken();
            description.Metadata.Token.Id = QueryValue.ID;
            description.Metadata.Token.Stamp = Guid.NewGuid().ToByteArray();
            description.Metadata.Token.Version = "1.0";
            description.Metadata.Description = QueryValue.COMMENTS;
            description.Metadata.Title = QueryValue.NAME;
            return description;
        }
    }
}
