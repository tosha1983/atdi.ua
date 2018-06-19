using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class QueryDeleteStatement : LoggedObject, IQueryDeleteStatement
    {
        public QueryDeleteStatement(ILogger logger) : base(logger)
        {
        }

        public IQueryDeleteStatement Where(Condition condition)
        {
            throw new NotImplementedException();
        }
    }
}
