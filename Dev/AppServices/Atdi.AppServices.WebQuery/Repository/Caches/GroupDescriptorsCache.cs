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
    public sealed class GroupDescriptorsCache
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly IQueryExecutor _queryExecutor;
        private Dictionary<int, GroupDescriptor> _descriptorsCache;
        private DateTime? _actualyCacheDataDate;
        private int _queryTokenVersion;
        private int _threadId;

        public GroupDescriptorsCache(IDataLayer<IcsmDataOrm> dataLayer)
        {
            this._actualyCacheDataDate = DateTime.Now;
            this._queryTokenVersion = 0;
            this._threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._descriptorsCache = new Dictionary<int, GroupDescriptor>();
        }

        public GroupDescriptor[] GetDecriptors(int[] identifiers)
        {
            if (identifiers == null)
            {
                throw new ArgumentNullException(nameof(identifiers));
            }

            this.TryToReloadCache();

            var loading = new List<int>();
            var result = new List<GroupDescriptor>();

            for (int i = 0; i < identifiers.Length; i++)
            {
                var id = identifiers[i];
                if (this._descriptorsCache.TryGetValue(id, out GroupDescriptor descriptor))
                {
                    result.Add(descriptor);
                }
                else
                {
                    loading.Add(id);
                }
            }

            if (loading.Count > 0)
            {
                var descriptors = LoadDescriptors(loading);
                result.AddRange(descriptors);
            }

            return result.ToArray();
        }

        private void TryToReloadCache()
        {
            var checkQuery = _dataLayer.Builder
                .From<XUPDATEOBJECTS>()
                .Select(
                        c => c.DATEMODIFIED
                    )
                .Where(c => c.OBJTABLE, ConditionOperator.In, "TASKFORCE", "XWEBQUERY", "XWEBCONSTRAINT")
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
                this._descriptorsCache = new Dictionary<int, GroupDescriptor>();
                ++this._queryTokenVersion;
            }
        }

        private static bool ToBool(int? value)
        {
            if (value == null)
            {
                return false;
            }

            return value > 0;
        }

        private GroupDescriptor[] LoadDescriptors(List<int> identifiers)
        {
            var groupsQuery = _dataLayer.Builder
                .From<TASKFORCE>()
                .Select(
                    c => c.ID,
                    c => c.CODE,
                    c => c.SHORT_NAME,
                    c => c.FULL_NAME,
                    c => c.DESCRIPTION,
                    c => c.CUST_CHB1,
                    c => c.CUST_CHB2
                   )
                .Where(c => c.ID, ConditionOperator.In, identifiers.ToArray());

            var groupDescriptors = this._queryExecutor
                .Fetch(groupsQuery, reader =>
                {
                    var groups = new List<GroupDescriptor>();

                    while (reader.Read())
                    {
                        var group = new GroupDescriptor
                        {
                            Id = reader.GetValue(c => c.ID),
                            Group = new QueryGroup
                            {
                                Code = reader.GetValue(c => c.CODE),
                                Name = reader.GetValue(c => c.SHORT_NAME),
                                Title = reader.GetValue(c => c.FULL_NAME),
                                Description = reader.GetValue(c => c.DESCRIPTION),
                                CanCreate = ToBool(reader.GetValue(c => c.CUST_CHB1)),
                                CanModify = ToBool(reader.GetValue(c => c.CUST_CHB1)),
                                CanDelete = ToBool(reader.GetValue(c => c.CUST_CHB2))
                            }
                        };

                        groups.Add(group);
                    }
                    return groups.ToArray();
                });

            if (groupDescriptors.Length == 0)
            {
                return groupDescriptors;
            }

            // load queries token
            var tokensQuery = _dataLayer.Builder
                .From<XWEBQUERY>()
                .Where(c => c.TASKFORCEGROUP, ConditionOperator.In, groupDescriptors.Select(g => g.Group.Name).ToArray())
                .Select(
                    c => c.ID,
                    c => c.CODE,
                    c => c.TASKFORCEGROUP);

            var tokensByGroup = new Dictionary<string, List<QueryTokenDescriptor>>();

            this._queryExecutor
                .Fetch(tokensQuery, reader =>
                {
                    while (reader.Read())
                    {
                        var group = reader.GetValue(c => c.TASKFORCEGROUP);
                        var token = new QueryTokenDescriptor
                        {
                            Code = reader.GetValue(c => c.CODE),
                            Token = new QueryToken
                            {
                                Id = reader.GetValue(c => c.ID),
                                Version = $"1.0.{this._threadId.ToString()}.{this._queryTokenVersion.ToString()}",
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

            foreach (var item in groupDescriptors)
            {
                if (tokensByGroup.TryGetValue(item.Group.Name, out List<QueryTokenDescriptor> tokens))
                {
                    item.Group.QueryTokens = tokens.Select(t => t.Token).ToArray();
                    item.QueryTokens = tokens.ToArray();
                    tokens.ForEach(t => t.GroupDescriptor = item);
                }
                else
                {
                    item.QueryTokens = new QueryTokenDescriptor[] { };
                }
                this._descriptorsCache[item.Id] = item;
            }

            return groupDescriptors;
        }

    }
}
