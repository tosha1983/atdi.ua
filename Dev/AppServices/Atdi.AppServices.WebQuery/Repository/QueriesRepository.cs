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

        private readonly Dictionary<string, GroupDescriptor[]> _groupsCache;

        public QueriesRepository(ILogger logger, IDataLayer<IcsmDataOrm> dataLayer) : base(logger)
        {
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._groupsCache = new Dictionary<string, GroupDescriptor[]>();
        }

        private GroupDescriptor[] LoadGroupsByUser(UserTokenData userToken)
        {
            var groupsQuery = _dataLayer.Builder
                .From<TSKF_MEMBER>()
                .Select(
                    c => c.Taskforce.ID,
                    c => c.Taskforce.SHORT_NAME,
                    c => c.Taskforce.FULL_NAME,
                    c => c.Taskforce.DESCRIPTION)
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
                                Description = reader.GetValue(c => c.Taskforce.DESCRIPTION)
                            }
                        };

                        groups.Add(group);
                    }
                    return groups.ToArray();
                });

            // load queries token
            var tokensQuery = _dataLayer.Builder
                .From<XWebQuery>()
                .Where(c => c.TaskForceGroup, ConditionOperator.In, result.Select(g => g.Group.Name).ToArray())
                .Select(
                    c => c.Id,
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
                                Id = reader.GetValue(c => c.Id),
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryId"></param>
        /// <returns></returns>
        private QueryDescriptor LoadQueryDescriptor(int queryId)
        {
            return new QueryDescriptor();
        }

        public QueryDescriptor GetQueryDescriptorByToken(UserTokenData userToken, QueryToken queryToken)
        {
            return null;
        }

        public QueryDescriptor GetQueryDescriptorByCode(UserTokenData userToken, string queryCode)
        {
            return null;
        }

        public QueryGroup[] GetGroupsByUser(UserTokenData userToken)
        {
            if (!this._groupsCache.TryGetValue(userToken.UserCode, out GroupDescriptor[] groups))
            {
                groups = this.LoadGroupsByUser(userToken);
                this._groupsCache[userToken.UserCode] = groups;
            }

            return groups.Select(g => g.Group).ToArray();
        }
    }
}
