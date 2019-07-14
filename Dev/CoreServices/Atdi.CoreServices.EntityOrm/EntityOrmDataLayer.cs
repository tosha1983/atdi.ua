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
    internal sealed class EntityOrmDataLayer : LoggedObject, IDataLayer<EntityDataOrm>
    {
        private readonly IDataLayer _dataLayer;        
        private readonly IEntityOrm _entityOrm;
        private readonly PatternBuilderFactory _builderFactory;
        private IQueryBuilder _queryBuilder;

        public EntityOrmDataLayer(IDataLayer dataLayer, IEntityOrm entityOrm, PatternBuilderFactory builderFactory, ILogger logger) : base(logger)
        {
            this._dataLayer = dataLayer;
            this._entityOrm = entityOrm;
            this._builderFactory = builderFactory;
        }

        public IQueryBuilder Builder
        {
            get
            {
                if (this._queryBuilder == null)
                {
                    this._queryBuilder = new QueryBuilder(this._entityOrm, this.Logger);
                }
                return this._queryBuilder;
            }
        }

        public IDataLayerScope CreateScope<TContext>() 
            where TContext : IDataContext, new()
        {
            var dataEngine = this._dataLayer.GetDataEngine<TContext>();
            var scope = new DataLayerScope(dataEngine, this._builderFactory, this.Logger);
            return scope;
        }

        public IDataLayerScope CreateScope(IDataContext dataContext)
        {
            var dataEngine = this._dataLayer.GetDataEngine(dataContext);
            var scope = new DataLayerScope(dataEngine, this._builderFactory, this.Logger);
            return scope;
        }

        public IQueryExecutor Executor<TContext>() where TContext : IDataContext, new()
        {
            var engine = this._dataLayer.GetDataEngine<TContext>();
            var executor = new QueryExecutor(engine, null, this._builderFactory, this.Logger);
            return executor;
        }

        public IQueryBuilder<TModel> GetBuilder<TModel>()
        {
            return new QueryBuilder<TModel>(this._entityOrm, this.Logger);
        }

    }
}
