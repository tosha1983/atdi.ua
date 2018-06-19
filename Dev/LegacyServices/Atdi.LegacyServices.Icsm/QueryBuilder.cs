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

        public IQuerySelectStatement<TModel> From<TModel>()
        {

            return new QuerySelectStatement<TModel>(this.Logger);
        }

        public IQueryInsertStatement Insert(string tableName)
        {
            throw new NotImplementedException();
        }

        public IQueryUpdateStatement Update(string tableName)
        {
            throw new NotImplementedException();
        }

        public IQueryDeleteStatement Delete(string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
