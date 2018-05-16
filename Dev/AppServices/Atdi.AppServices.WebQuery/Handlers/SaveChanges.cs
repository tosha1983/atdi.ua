
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
    public sealed class SaveChanges : LoggedObject
    {
        public SaveChanges(ILogger logger) : base(logger)
        {
        }

        public ChangesResult Handle(UserToken userToken, QueryToken queryToken, Changeset changeset)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.SaveChanges))
            {
                return new ChangesResult() { };
            }
        }
    }
}
