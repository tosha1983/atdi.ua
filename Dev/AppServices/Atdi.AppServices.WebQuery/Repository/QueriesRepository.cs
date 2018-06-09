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
        private readonly IIrpParser _parserQuery;


        private readonly Dictionary<string, GroupDescriptor[]> _groupsCache;
        private readonly Dictionary<int, XWebQuery> _CacheWebQueryById;
        private readonly Dictionary<int, XWebConstraint[]> _CacheXWebConstraint;


        public QueriesRepository(ILogger logger, IDataLayer<IcsmDataOrm> dataLayer, IIrpParser irpparser) : base(logger)
        {
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._groupsCache = new Dictionary<string, GroupDescriptor[]>();
            this._CacheWebQueryById = new Dictionary<int, XWebQuery>();
            this._CacheXWebConstraint = new Dictionary<int, XWebConstraint[]>();
            this._parserQuery = irpparser;
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
                                CanCreateAndModify = reader.GetValue(c => c.Taskforce.CUST_CHB1),
                                CanDelete = reader.GetValue(c => c.Taskforce.CUST_CHB1)
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
                .From<XWebQuery>()
                .Where(c => c.TaskForceGroup, ConditionOperator.In, result.Select(g => g.Group.Name).ToArray())
                .Select(
                    c => c.ID,
                    c => c.Code,
                    c => c.TaskForceGroup);

            var tokensByGroup = new Dictionary<string, List<QueryTokenDescriptor>>();

            this._queryExecutor
                .Fetch(tokensQuery, reader =>
                {
                    while(reader.Read())
                    {
                        var group = reader.GetValue(c => c.TaskForceGroup);
                        var token = new QueryTokenDescriptor
                        {
                            Code = reader.GetValue(c => c.Code),
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

            return result;
        }


        public XWebConstraint[] LoadXWebConstraintByQueryID(int queryId)
        {
            var ConstraintQueryWeb = _dataLayer.Builder
               .From<XWebConstraint>()
               .Select(
                   c => c.ID,
                   c => c.WebQueryId,
                   c => c.Name,
                   c => c.Path,
                   c => c.StrValue,
                   c => c.Min,
                   c => c.Max,
                   c => c.Include,
                   c => c.DateValueMin,
                   c => c.DateValueMax
                 )
                .Where(c => c.WebQueryId, ConditionOperator.Equal, queryId);
            var QueryDescriptorValue = this._queryExecutor
           .Fetch(ConstraintQueryWeb, reader => {
               var groups_XWebConstraint = new List<XWebConstraint>();
               if (reader.Read()) {
                   var token = new XWebConstraint
                   {
                       ID = reader.GetValue(x => x.ID),
                       WebQueryId = reader.GetValue(x => x.WebQueryId),
                       StrValue = reader.GetValue(x => x.StrValue),
                       Path = reader.GetValue(x => x.Path),
                       Name = reader.GetValue(x => x.Name),
                       Min = reader.GetValue(x => x.Min),
                       Max = reader.GetValue(x => x.Max),
                       Include = reader.GetValue(x => x.Include),
                       DateValueMin = reader.GetValue(x => x.DateValueMin),
                       DateValueMax = reader.GetValue(x => x.DateValueMax)
                   };

                   groups_XWebConstraint.Add(token);
               }
               return groups_XWebConstraint.ToArray();
           });
            return QueryDescriptorValue;
        }

        public XWebQuery LoadXWebQueryByID(int queryId)
        {
            var query_web = _dataLayer.Builder
               .From<XWebQuery>()
               .Where(c => c.ID, ConditionOperator.Equal, queryId)
               .Select(
                   c => c.ID,
                   c => c.Name,
                   c => c.Code,
                   c => c.Query,
                   c => c.Comments,
                   c => c.IdentUser,
                   c => c.TaskForceGroup);
                
            var QueryDescriptorValue = this._queryExecutor
           .Fetch(query_web, reader => {
               var groups_XWebQuery = new List<XWebQuery>();
               if (reader.Read())
               {
                   var token = new XWebQuery
                   {
                       ID = reader.GetValue(x => x.ID),
                       Code = reader.GetValue(x => x.Code),
                       Comments = reader.GetValue(x => x.Comments),
                       IdentUser = reader.GetValue(x => x.IdentUser),
                       Name = reader.GetValue(x => x.Name),
                       Query = reader.GetValue(x => x.Query),
                       TaskForceGroup = reader.GetValue(x => x.TaskForceGroup)
                   };
                   groups_XWebQuery.Add(token);
               }
               return groups_XWebQuery.ToArray();
           });
            if (QueryDescriptorValue.Count() > 0)
                return QueryDescriptorValue[0];
            else return null;
        }


        public QueryDescriptor GetQueryDescriptorByToken(UserTokenData userToken, QueryToken queryToken)
        {
            QueryGroup[] groups = GetGroupsByUser(userToken);
            QueryDescriptor Q = null;
            foreach (QueryGroup qp in groups) {
                if (qp.QueryTokens != null) {
                    foreach (QueryToken l_g in qp.QueryTokens) {
                        if (l_g.Id == queryToken.Id)  {
                            XWebQuery xWeb = new XWebQuery();
                            if (!this._CacheWebQueryById.TryGetValue(queryToken.Id, out xWeb)) {
                                xWeb = LoadXWebQueryByID(queryToken.Id);
                                this._CacheWebQueryById[queryToken.Id] = xWeb;
                            }
                            if (xWeb != null) {
                                XWebConstraint[] xWebConstraint = new List<XWebConstraint>().ToArray();
                                if (!this._CacheXWebConstraint.TryGetValue(xWeb.ID, out xWebConstraint))
                                {
                                    xWebConstraint = LoadXWebConstraintByQueryID(xWeb.ID);
                                    this._CacheXWebConstraint[xWeb.ID] = xWebConstraint;
                                }
                                Q = new QueryDescriptor(qp, xWebConstraint, xWeb);
                                Q.Metadata = new QueryMetadata();
                                Q.Metadata.Name = xWeb.Name;
                                Q.Metadata.Code = xWeb.Code;
                                Q.Metadata.Token = new QueryToken();
                                Q.Metadata.Token.Id = xWeb.ID;
                                Q.Metadata.Token.Stamp = Guid.NewGuid().ToByteArray();
                                Q.Metadata.Token.Version = "1.0";
                                if (xWeb.Query != null) Q.Metadata.Columns = this._parserQuery.ExecuteParseQuery(xWeb.Query);
                                Q.Metadata.Description = xWeb.Comments;
                                Q.Metadata.Title = xWeb.Name;
                            }
                            break;
                        }
                    }
                }
            }
            return Q;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="queryCode"></param>
        /// <returns></returns>
        public QueryDescriptor GetQueryDescriptorByCode(UserTokenData userToken, string queryCode)
        {
            QueryGroup[] groups = GetGroupsByUser(userToken);
            QueryDescriptor Q = null;
            foreach (QueryGroup qp in groups) {
                if (qp.QueryTokens != null)
                {
                    foreach (QueryToken token in qp.QueryTokens)
                    {
                        XWebQuery xWeb = new XWebQuery();
                        if (!this._CacheWebQueryById.TryGetValue(token.Id, out xWeb))
                        {
                            xWeb = LoadXWebQueryByID(token.Id);
                            this._CacheWebQueryById[token.Id] = xWeb;
                        }
                        if (xWeb != null)
                        {
                            if (xWeb.Code == queryCode)
                            {
                                XWebConstraint[] xWebConstraint = new List<XWebConstraint>().ToArray();
                                if (!this._CacheXWebConstraint.TryGetValue(xWeb.ID, out xWebConstraint))
                                {
                                    xWebConstraint = LoadXWebConstraintByQueryID(xWeb.ID);
                                    this._CacheXWebConstraint[xWeb.ID] = xWebConstraint;
                                }
                                Q = new QueryDescriptor(qp, xWebConstraint, xWeb);
                                Q.Metadata = new QueryMetadata();
                                Q.Metadata.Name = xWeb.Name;
                                Q.Metadata.Code = xWeb.Code;
                                Q.Metadata.Token = new QueryToken();
                                Q.Metadata.Token.Id = xWeb.ID;
                                Q.Metadata.Token.Stamp = Guid.NewGuid().ToByteArray();
                                Q.Metadata.Token.Version = "1.0";
                                if (xWeb.Query != null) Q.Metadata.Columns = this._parserQuery.ExecuteParseQuery(xWeb.Query);
                                Q.Metadata.Description = xWeb.Comments;
                                Q.Metadata.Title = xWeb.Name;
                                break;
                            }
                        }
                    }
                }
            }
            return Q;
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
