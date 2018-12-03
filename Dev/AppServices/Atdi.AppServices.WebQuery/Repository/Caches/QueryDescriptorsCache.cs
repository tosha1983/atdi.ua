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
            var queryWebConstraint = _dataLayer.Builder
              .From<XWEBCONSTRAINT>()
              .Where(c => c.WEBQUERYID, ConditionOperator.Equal, queryId)
              .Select(
                  c => c.ID,
                  c => c.MAX,
                  c => c.MIN,
                  c => c.NAME,
                  c => c.INCLUDE,
                  c => c.PATH,
                  c => c.WEBQUERYID,
                  c => c.STRVALUE,
                  c => c.STRVALUETO,
                  c => c.DATEVALUEMIN,
                  c => c.DATEVALUEMAX,
                  c => c.TYPECONDITION,
                  c => c.OPERCONDITION,
                  c => c.MOMENTOFUSE,
                  c => c.MESSAGENOTVALID,
                  c => c.DESCRCONDITION,
                  c => c.DEFAULTVALUE
                 );

            var constraintsValue = this._queryExecutor
            .Fetch(queryWebConstraint, reader =>
            {
                var groupsConstr = new List<XWEBCONSTRAINT>();
                while (reader.Read())
                {
                    var token = new XWEBCONSTRAINT
                    {
                        ID = reader.GetValue(x => x.ID),
                        NAME = reader.GetValue(x => x.NAME),
                        INCLUDE = reader.GetValue(x => x.INCLUDE),
                        PATH = reader.GetValue(x => x.PATH),
                        STRVALUE = reader.GetValue(x => x.STRVALUE),
                        STRVALUETO = reader.GetValue(x => x.STRVALUETO),
                        WEBQUERYID = reader.GetValue(x => x.WEBQUERYID),
                        MIN = reader.GetValue(x => x.MIN),
                        MAX = reader.GetValue(x => x.MAX),
                        DATEVALUEMIN = reader.GetValue(x => x.DATEVALUEMIN),
                        DATEVALUEMAX = reader.GetValue(x => x.DATEVALUEMAX),
                        TYPECONDITION = reader.GetValue(x => x.TYPECONDITION),
                        OPERCONDITION = reader.GetValue(x => x.OPERCONDITION),
                        MOMENTOFUSE = reader.GetValue(x => x.MOMENTOFUSE),
                        MESSAGENOTVALID = reader.GetValue(x => x.MESSAGENOTVALID),
                        DESCRCONDITION = reader.GetValue(x => x.DESCRCONDITION),
                        DEFAULTVALUE = reader.GetValue(x => x.DEFAULTVALUE)
                    };
                    groupsConstr.Add(token);
                }
                return groupsConstr.ToArray();
            });


            var queryWebAttributes = _dataLayer.Builder
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

            var attributesValue = this._queryExecutor
            .Fetch(queryWebAttributes, reader =>
            {
                var groupsAttributes = new List<XWEBQUERYATTRIBUTES>();
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
                    groupsAttributes.Add(token);
                }
                return groupsAttributes.ToArray();
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

            var queryValue = this._queryExecutor
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
            IrpDescriptor irpDescriptor = this._irpParser.ExecuteParseQuery(queryValue.QUERY);
            description = new QueryDescriptor(queryValue, constraintsValue, attributesValue, irpDescriptor, queryToken);
            return description;
        }
    }
}
