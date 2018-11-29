using Atdi.AppServices.WebQuery.DTO;
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
        private Dictionary<int, QueryDescriptor> _queryDescriptors;
        private DateTime? _actualyCacheDataDate;

        public QueryDescriptorsCache(IDataLayer<IcsmDataOrm> dataLayer, IIrpParser irpParser)
        {
            this._actualyCacheDataDate = DateTime.Now;
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._irpParser = irpParser;
            this._queryDescriptors = new Dictionary<int, QueryDescriptor>();
        }

        public QueryDescriptor GetDecriptor(QueryTokenDescriptor queryToken)
        {
            this.TryToReloadCache();

            var queryId = queryToken.Token.Id;
            if (!this._queryDescriptors.TryGetValue(queryId, out QueryDescriptor descriptor))
            {
                descriptor = this.LoadQueryDescriptor(queryToken);
                this._queryDescriptors[queryId] = descriptor;
            }
            return descriptor;
        }

        private void TryToReloadCache()
        {
            var checkQuery = _dataLayer.Builder
                .From<XUPDATEOBJECTS>()
                .Select(
                        c => c.DATEMODIFIED
                    )
                .Where(c => c.OBJTABLE, ConditionOperator.In, "XWEBQUERY", "XWEBCONSTRAINT", "XWEBQUERYATTRIBUTES")
                .Where(c => c.DATEMODIFIED, ConditionOperator.GreaterThan, this._actualyCacheDataDate)
                .OnTop(1);

            var isDirty = this._queryExecutor
                .Fetch(checkQuery, reader =>
                {
                    var result = false;
                    while (reader.Read())
                    {
                        var modifiedDate = reader.GetValue(c => c.DATEMODIFIED);
                        if (modifiedDate > this._actualyCacheDataDate)
                        {
                            this._actualyCacheDataDate = modifiedDate;
                        }
                        result = true;
                    }
                    return result;
                });

            if (isDirty)
            {
                this._queryDescriptors = new Dictionary<int, QueryDescriptor>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryId"></param>
        /// <returns></returns>
        public QueryDescriptor LoadQueryDescriptor(QueryTokenDescriptor queryToken)
        {
            var queryId = queryToken.Token.Id;
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


            var query_web_attributes = _dataLayer.Builder
             .From<XWEBQUERYATTRIBUTES>()
             .Where(c => c.WEBQUERYID, ConditionOperator.Equal, queryId)
             .Select(
                 c => c.ID,
                 c => c.PATH,
                c => c.WEBQUERYID,
                c => c.READONLY,
                c => c.NOTCHANGEADD,
                c => c.NOTCHANGEEDIT
                );

            var AttributesValue = this._queryExecutor
            .Fetch(query_web_attributes, reader =>
            {
                var groups_attributes = new List<XWEBQUERYATTRIBUTES>();
                while (reader.Read())
                {
                    var token = new XWEBQUERYATTRIBUTES
                    {
                        ID = reader.GetValue(x => x.ID),
                        WEBQUERYID = reader.GetValue(x => x.WEBQUERYID),
                        READONLY = reader.GetValue(x => x.READONLY),
                        PATH = reader.GetValue(x => x.PATH),
                        NOTCHANGEADD  = reader.GetValue(x => x.NOTCHANGEADD),
                        NOTCHANGEEDIT = reader.GetValue(x => x.NOTCHANGEEDIT)
                    };
                    groups_attributes.Add(token);
                }
                return groups_attributes.ToArray();
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
                   c => c.TASKFORCEGROUP,
                   c => c.VIEWCOLUMNS,
                   c => c.ADDCOLUMNS,
                   c => c.EDITCOLUMNS,
                   c => c.TABLECOLUMNS
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
                   Value.VIEWCOLUMNS = reader.GetValue(x => x.VIEWCOLUMNS);
                   Value.ADDCOLUMNS = reader.GetValue(x => x.ADDCOLUMNS);
                   Value.EDITCOLUMNS = reader.GetValue(x => x.EDITCOLUMNS);
                   Value.TABLECOLUMNS = reader.GetValue(x => x.TABLECOLUMNS);
               }
               return Value;
           });
            IrpDescriptor irpDescriptor = this._irpParser.ExecuteParseQuery(QueryValue.QUERY);
            description = new QueryDescriptor(QueryValue, ConstraintsValue, AttributesValue, irpDescriptor, queryToken);
            return description;
        }
    }
}
