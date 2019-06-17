using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;


namespace Atdi.CoreServices.EntityOrm
{
    public sealed class EntityOrmDataLayer : LoggedObject, IDataLayer<EntityDataOrm>
    {
        private readonly IDataLayer _dataLayer;
        private readonly IQueryBuilder _queryBuilder;
        private readonly IEntityOrm _entityOrm;
        private readonly DataTypeSystem _dataTypeSystem;

        public EntityOrmDataLayer(IDataLayer dataLayer, IEntityOrm entityOrm, DataTypeSystem dataTypeSystem, ILogger logger) : base(logger)
        {
            this._dataLayer = dataLayer;
            this._entityOrm = entityOrm;
            this._dataTypeSystem = dataTypeSystem;
            this._queryBuilder = new QueryBuilder(entityOrm, logger);
        }

        public IQueryBuilder Builder => _queryBuilder;

        public IQueryExecutor Executor<TContext>() where TContext : IDataContext, new()
        {
            var engine = this._dataLayer.GetDataEngine<TContext>();
            var entiryOrm = new EntityOrmQueryBuilder(engine, this._entityOrm, this._dataTypeSystem, this.Logger);
            var executor = new QueryExecutor(engine, entiryOrm, this._dataTypeSystem, this.Logger);
            return executor;
        }

        public IQueryBuilder<TModel> GetBuilder<TModel>()
        {
            return new QueryBuilder<TModel>(this._entityOrm, this.Logger);
        }

        public IDataEngine GetDataEngine<TContext>() where TContext : IDataContext, new()
        {
            return _dataLayer.GetDataEngine<TContext>();
        }
    }
}
