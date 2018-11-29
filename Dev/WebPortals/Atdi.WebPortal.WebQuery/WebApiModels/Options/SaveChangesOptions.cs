using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels.Options
{
    public class SaveChangesOptions
    {
        public UserToken UserToken { get; set; }

        public QueryToken QueryToken { get; set; }

        public DataChangeset Changeset { get; set; }

    }
}
