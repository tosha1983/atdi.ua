using Atdi.AppServices.WebQuery.DTO;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery
{ 
    internal sealed class QueriesRepository : LoggedObject
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;

        public QueriesRepository(ILogger logger, IDataLayer<IcsmDataOrm> dataLayer) : base(logger)
        {
            this._dataLayer = dataLayer;
        }


        public QueryDescriptor GetQueryDescriptor(QueryToken token)
        {
            return null;
        }

        public QueriesNode[] GetTreeNodesByUser(UserTokenData token)
        {
            _dataLayer.Builder.From<TSKF_MEMBER>()
                .Select(c => c.ID, c => c.APP_USER, c => c.Taskforce.FULL_NAME)
                .Where((c, o) => o.Like(c.APP_USER, "ss") && o.NotLike(c.Taskforce.FULL_NAME, "s"));

            return null;
        }
    }
}
