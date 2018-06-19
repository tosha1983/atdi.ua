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
    internal sealed class QueryUpdateStatement : LoggedObject, IQueryUpdateStatement
    {
        public QueryUpdateStatement(ILogger logger) : base(logger)
        {
        }

        
        public IQueryUpdateStatement Where(Condition condition)
        {
            throw new NotImplementedException();
        }

        IQueryUpdateStatement IQueryUpdateStatement.SetValue(ColumnValue columnValue)
        {
            throw new NotImplementedException();
        }

        IQueryUpdateStatement IQueryUpdateStatement.SetValues(ColumnValue[] columnValue)
        {
            throw new NotImplementedException();
        }
    }
}
