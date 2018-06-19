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
    internal sealed class QueryInsertStatement : LoggedObject, IQueryInsertStatement
    {
        public QueryInsertStatement(ILogger logger) : base(logger)
        {
        }

        public IQueryInsertStatement SetValue(ColumnValue columnValue)
        {
            throw new NotImplementedException();
        }

        public IQueryInsertStatement SetValues(ColumnValue[] columnValue)
        {
            throw new NotImplementedException();
        }

    }
}
