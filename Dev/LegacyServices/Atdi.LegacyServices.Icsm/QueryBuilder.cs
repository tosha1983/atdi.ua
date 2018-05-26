using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class QueryBuilder : LoggedObject, IQueryBuilder
    {
        public QueryBuilder(ILogger logger) : base(logger)
        {
        }

        public IQuerySelectStatement From(string tableName)
        {
            return new QuerySelectStatement(tableName, this.Logger);
        }
    }
}
