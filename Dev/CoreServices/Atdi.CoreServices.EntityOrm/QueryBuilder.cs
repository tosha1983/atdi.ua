using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryBuilder : LoggedObject, IQueryBuilder
    {
        private readonly IEntityOrm _entityOrm;
        public QueryBuilder(IEntityOrm entityOrm, ILogger logger) : base(logger)
        {
            this._entityOrm = entityOrm;
        }

        public IQuerySelectStatement From(string tableName)
        {
            return new QuerySelectStatement(tableName);
        }

        public IQuerySelectStatement<TModel> From<TModel>()
        {
            return new QuerySelectStatement<TModel>();
        }

        public IQueryInsertStatement Insert(string tableName)
        {
            return new QueryInsertStatement(tableName);
        }

        public IQueryUpdateStatement Update(string tableName)
        {
            return new QueryUpdateStatement(tableName);
        }

        public IQueryDeleteStatement Delete(string tableName)
        {
            return new QueryDeleteStatement(tableName);
        }
    }
}
