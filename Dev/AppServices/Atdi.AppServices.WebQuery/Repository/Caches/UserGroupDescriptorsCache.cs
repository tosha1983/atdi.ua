using Atdi.AppServices.WebQuery.DTO;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.Identity;
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
    public sealed class UserGroupDescriptorsCache
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly IQueryExecutor _queryExecutor;
        private readonly GroupDescriptorsCache _groupDescriptorsCache;
        private Dictionary<string, UserGroupDescriptors> _descriptorsCache;
        private DateTime? _actualyCacheDataDate;

        public UserGroupDescriptorsCache(IDataLayer<IcsmDataOrm> dataLayer, GroupDescriptorsCache groupDescriptorsCache)
        {
            this._actualyCacheDataDate = DateTime.Now;
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._descriptorsCache = new Dictionary<string, UserGroupDescriptors>();
            this._groupDescriptorsCache = groupDescriptorsCache;

        }
        public UserGroupDescriptors GetDecriptors(UserTokenData userToken)
        {
            this.TryToReloadCache();

            if (!this._descriptorsCache.TryGetValue(userToken.UserCode, out UserGroupDescriptors descriptors))
            {
                descriptors = this.LoadGroupsByUser(userToken);
                this._descriptorsCache[userToken.UserCode] = descriptors;
            }

            return descriptors;
        }

        private void TryToReloadCache()
        {
            var checkQuery = _dataLayer.Builder
                .From<XUPDATEOBJECTS>()
                .Select(
                        c => c.DATEMODIFIED
                    )
                .Where(c => c.OBJTABLE, ConditionOperator.In, "TSKF_MEMBER", "TASKFORCE", "XWEBQUERY")
                .Where(c => c.DATEMODIFIED, ConditionOperator.GreaterThan, this._actualyCacheDataDate);

            var isDirty = this._queryExecutor
                .Fetch(checkQuery, reader =>
                {
                    var result = false;
                    while(reader.Read())
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
                this._descriptorsCache = new Dictionary<string, UserGroupDescriptors>();
            }
        }

        private UserGroupDescriptors LoadGroupsByUser(UserTokenData userToken)
        {
            var groupsQuery = _dataLayer.Builder
                .From<TSKF_MEMBER>()
                .Select(
                    c => c.TSKF_ID
                   )
                .Where(c => c.APP_USER, ConditionOperator.Equal, userToken.UserCode);

            var result = this._queryExecutor
                .Fetch(groupsQuery, reader =>
                {
                    var groups = new List<int>();

                    while (reader.Read())
                    {
                        groups.Add(reader.GetValue(c => c.TSKF_ID));
                    }
                    return groups.ToArray();
                });

            var descriptors = _groupDescriptorsCache.GetDecriptors(result);
            return new UserGroupDescriptors(userToken, descriptors);

        }
    }
}
