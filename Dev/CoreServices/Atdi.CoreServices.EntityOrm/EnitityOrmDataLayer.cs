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
    public sealed class EnitityOrmDataLayer : LoggedObject, IDataLayer<EntityDataOrm>
    {
        private readonly IDataLayer _dataLayer;
        private readonly IQueryBuilder _queryBuilder;
        private ILogger _logger;
        private readonly IEntityOrm _entityOrm;

        public EnitityOrmDataLayer(IDataLayer dataLayer, IEntityOrm entityOrm, ILogger logger) : base(logger)
        {
            this._dataLayer = dataLayer;
            this._entityOrm = entityOrm;
            this._logger = logger;
            this._queryBuilder = new QueryBuilder(logger);
        }

        public IQueryBuilder Builder => _queryBuilder;

        public IQueryExecutor Executor<TContext>() where TContext : IDataContext, new()
        {
            var engine = this._dataLayer.GetDataEngine<TContext>();
            var entiryOrm = new EntityOrmQueryBuilder(engine, this._entityOrm, this.Logger);
            var executor = new QueryExecutor(engine, entiryOrm, this.Logger);
            return executor;
        }

        public IQueryBuilder<TModel> GetBuilder<TModel>()
        {
            return new QueryBuilder<TModel>(this.Logger);
        }

        public IDataEngine GetDataEngine<TContext>() where TContext : IDataContext, new()
        {
            return _dataLayer.GetDataEngine<TContext>();
        }
    }
}
