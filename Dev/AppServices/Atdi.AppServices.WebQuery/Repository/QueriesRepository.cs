using Atdi.AppServices.WebQuery.DTO;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Atdi.AppServices.WebQuery
{ 
    public sealed class QueriesRepository : LoggedObject
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IIrpParser _irpParser;


        private readonly Dictionary<string, GroupDescriptor[]> _groupsCache;


        public QueriesRepository(ILogger logger, IDataLayer<IcsmDataOrm> dataLayer, IIrpParser irpparser) : base(logger)
        {
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._groupsCache = new Dictionary<string, GroupDescriptor[]>();
            this._irpParser = irpparser;

        }

        private GroupDescriptor[] LoadGroupsByUser(UserTokenData userToken)
        {
            var groupsQuery = _dataLayer.Builder
                .From<TSKF_MEMBER>()
                .Select(
                    c => c.Taskforce.ID,
                    c => c.Taskforce.CODE,
                    c => c.Taskforce.SHORT_NAME,
                    c => c.Taskforce.FULL_NAME,
                    c => c.Taskforce.DESCRIPTION,
                    c => c.Taskforce.CUST_CHB1,
                    c => c.Taskforce.CUST_CHB2
                   )
                .Where(c => c.APP_USER, ConditionOperator.Equal, userToken.UserCode);

            var result = this._queryExecutor
                .Fetch(groupsQuery, reader =>
                {
                    var groups = new List<GroupDescriptor>();

                    while(reader.Read())
                    {
                        var group = new GroupDescriptor
                        {
                            Id = reader.GetValue(c => c.Taskforce.ID),
                            Group = new QueryGroup
                            {
                                Code = reader.GetValue(c => c.Taskforce.CODE),
                                Name = reader.GetValue(c => c.Taskforce.SHORT_NAME),
                                Title = reader.GetValue(c => c.Taskforce.FULL_NAME),
                                Description = reader.GetValue(c => c.Taskforce.DESCRIPTION),
                                CanCreate = reader.GetValue(c => c.Taskforce.CUST_CHB1),
                                CanModify = reader.GetValue(c => c.Taskforce.CUST_CHB1),
                                CanDelete = reader.GetValue(c => c.Taskforce.CUST_CHB2)
                            }
                        };

                        groups.Add(group);
                    }
                    return groups.ToArray();
                });

            if (result.Length == 0)
            {
                return result;
            }

            // load queries token
            var tokensQuery = _dataLayer.Builder
                .From<XWEBQUERY>()
                .Where(c => c.TASKFORCEGROUP, ConditionOperator.In, result.Select(g => g.Group.Name).ToArray())
                .Select(
                    c => c.ID,
                    c => c.CODE,
                    c => c.TASKFORCEGROUP);

            var tokensByGroup = new Dictionary<string, List<QueryTokenDescriptor>>();

            this._queryExecutor
                .Fetch(tokensQuery, reader =>
                {
                    while(reader.Read())
                    {
                        var group = reader.GetValue(c => c.TASKFORCEGROUP);
                        var token = new QueryTokenDescriptor
                        {
                            Code = reader.GetValue(c => c.CODE),
                            Token = new QueryToken
                            {
                                Id = reader.GetValue(c => c.ID),
                                Version = "1.0.0.0",
                                Stamp = Guid.NewGuid().ToByteArray()
                            }
                        };

                        if (!tokensByGroup.TryGetValue(group, out List<QueryTokenDescriptor> tokens))
                        {
                            tokens = new List<QueryTokenDescriptor>();
                            tokensByGroup[group] = tokens;
                        }
                        tokens.Add(token);
                    }
                    return (object)null;
                });

            foreach (var item in result)
            {
                if (tokensByGroup.TryGetValue(item.Group.Name, out List<QueryTokenDescriptor> tokens))
                {
                    item.Group.QueryTokens = tokens.Select(t => t.Token).ToArray();
                }
            }
            QueryDescriptor x=  LoadQueryDescriptor(2);
            return result;
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

         

        public QueryGroup[] GetGroupsByUser(UserTokenData userToken)
        {
            if (!this._groupsCache.TryGetValue(userToken.UserCode, out GroupDescriptor[] groups)) {
                groups = this.LoadGroupsByUser(userToken);
                this._groupsCache[userToken.UserCode] = groups;
            }
            return groups.Select(g => g.Group).ToArray();
        }


    }
}
