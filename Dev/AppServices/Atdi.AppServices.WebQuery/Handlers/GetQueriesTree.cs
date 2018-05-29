
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;

namespace Atdi.AppServices.WebQuery.Handlers
{
    public sealed class GetQueriesTree : LoggedObject
    {
        public GetQueriesTree(ILogger logger) : base(logger)
        {
        }

        public QueriesTree Handle(UserToken userToken)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.GetQueriesTree))
            {
                return new QueriesTree() { Nodes = new QueriesTreeNode[] { } };
            }
        }
    }
}
