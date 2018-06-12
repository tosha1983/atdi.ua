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
        private readonly UserGroupDescriptorsCache _userGroupDescriptorsCache;
        private readonly QueryDescriptorsCache _queryDescriptorsCache;

        public QueriesRepository(ILogger logger, IDataLayer<IcsmDataOrm> dataLayer, IIrpParser irpparser, UserGroupDescriptorsCache userGroupDescriptorsCache, QueryDescriptorsCache queryDescriptorsCache) : base(logger)
        {
            this._userGroupDescriptorsCache = userGroupDescriptorsCache;
            this._queryDescriptorsCache = queryDescriptorsCache;
        }

        public QueryDescriptor GetQueryDescriptorByToken(UserTokenData userToken, QueryToken queryToken)
        {
            var userGroupDescriptors = this._userGroupDescriptorsCache.GetDecriptors(userToken);
            if (!userGroupDescriptors.HasQuery(queryToken, out QueryTokenDescriptor queryTokenDescriptor))
            {
                throw new InvalidOperationException(Exceptions.QueryIsNotAvailable);
            }

            var queryDescriptor = _queryDescriptorsCache.GetDecriptor(queryTokenDescriptor);
            return queryDescriptor;
        }

        public QueryDescriptor GetQueryDescriptorByCode(UserTokenData userToken, string queryCode)
        {
            var userGroupDescriptors = this._userGroupDescriptorsCache.GetDecriptors(userToken);
            if (!userGroupDescriptors.HasQuery(queryCode, out QueryTokenDescriptor queryTokenDescriptor))
            {
                throw new InvalidOperationException(Exceptions.QueryIsNotAvailable);
            }

            var queryDescriptor = _queryDescriptorsCache.GetDecriptor(queryTokenDescriptor);
            return queryDescriptor;
        }

        public QueryGroup[] GetGroupsByUser(UserTokenData userToken)
        {
            var descriptors = this._userGroupDescriptorsCache.GetDecriptors(userToken);
            return descriptors.Descriptors.Select(g => g.Group).ToArray();
        }
    }
}
