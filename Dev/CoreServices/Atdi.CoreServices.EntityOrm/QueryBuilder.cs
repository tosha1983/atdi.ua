using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryBuilder<TModel> : LoggedObject, IQueryBuilder<TModel>
    {
        private readonly IEntityOrm _entityOrm;
        private readonly IEntityMetadata _entityMetadata;

        public QueryBuilder(IEntityOrm entityOrm, ILogger logger) : base(logger)
        {
            this._entityOrm = entityOrm;
            this._entityMetadata = entityOrm.GetEntityMetadata<TModel>();
        }

        public IQuerySelectStatement<TModel> From()
        {
            IQuerySelectStatement<TModel> querySelectStatement = new QuerySelectStatement<TModel>(_entityOrm, _entityMetadata);
            return querySelectStatement;
        }

        public IQueryInsertStatement<TModel> Insert()
        {
            IQueryInsertStatement<TModel> queryInsertStatement =  new QueryInsertStatement<TModel>(_entityOrm, _entityMetadata);
            return queryInsertStatement;
        }

        public IQueryUpdateStatement<TModel> Update()
        {
            IQueryUpdateStatement<TModel> queryUpdateStatement = new QueryUpdateStatement<TModel>(_entityOrm, _entityMetadata);
            return queryUpdateStatement;
        }

        public IQueryDeleteStatement<TModel> Delete()
        {
            IQueryDeleteStatement<TModel> queryDeleteStatement = new QueryDeleteStatement<TModel>(_entityOrm, _entityMetadata);
            return queryDeleteStatement;
        }
    }

    internal sealed class QueryBuilder : LoggedObject, IQueryBuilder
    {
        private readonly IEntityOrm _entityOrm;

        public QueryBuilder(IEntityOrm entityOrm, ILogger logger) : base(logger)
        {
            this._entityOrm = entityOrm;
        }

        public IQuerySelectStatement From(string entityName)
        {
            var entityMetadata = this._entityOrm.GetEntityMetadata(entityName);
            return new QuerySelectStatement(_entityOrm, entityMetadata);
        }

        public IQuerySelectStatement<TModel> From<TModel>()
        {
            var entityMetadata = this._entityOrm.GetEntityMetadata<TModel>();
            return new QuerySelectStatement<TModel>(this._entityOrm, entityMetadata);
        }

        public IQueryInsertStatement Insert(string entityName)
        {
            var entityMetadata = this._entityOrm.GetEntityMetadata(entityName);
            return new QueryInsertStatement(this._entityOrm, entityMetadata);
        }

        public IQueryUpdateStatement Update(string entityName)
        {
            var entityMetadata = this._entityOrm.GetEntityMetadata(entityName);
            return new QueryUpdateStatement(this._entityOrm, entityMetadata);
        }

        public IQueryDeleteStatement Delete(string entityName)
        {
            var entityMetadata = this._entityOrm.GetEntityMetadata(entityName);
            return new QueryDeleteStatement(this._entityOrm, entityMetadata);
        }
    }
}
