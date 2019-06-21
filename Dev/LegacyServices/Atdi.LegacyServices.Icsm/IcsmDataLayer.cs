using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.AppServices.WebQuery;

namespace Atdi.LegacyServices.Icsm
{
    public sealed class IcsmDataLayer : LoggedObject, IDataLayer<IcsmDataOrm>
    {
        private readonly Orm.SchemasMetadata _schemasMetadata;
        private readonly IDataLayer _dataLayer;
        private readonly IQueryBuilder _queryBuilder;
        private readonly Dictionary<string, QueryExecutor> _contextExecutors;

        public IcsmDataLayer(IDataLayer dataLayer, Orm.SchemasMetadata schemasMetadata, ILogger logger) :  base(logger)
        {
            this._dataLayer = dataLayer;
            this._schemasMetadata = schemasMetadata;
            this._queryBuilder = new QueryBuilder(logger);
            this._contextExecutors = new Dictionary<string, QueryExecutor>();

            logger.Debug(Contexts.LegacyServicesIcsm, Categories.CreatingInstance, Events.CreatedInstanceOfDataLayer);
        }

        public IQueryBuilder Builder => _queryBuilder;

        public IDataLayerScope<TContext> BeginScope<TContext>() where TContext : IDataContext, new()
        {
            throw new NotImplementedException();
        }

        public IQueryExecutor Executor<TContext>() where TContext : IDataContext, new()
        {
            var contextType = typeof(TContext);
            var key = contextType.AssemblyQualifiedName;
            if (this._contextExecutors.ContainsKey(key))
            {
                return this._contextExecutors[key];
            }

            var engine = this._dataLayer.GetDataEngine<TContext>();
            var icsmOrm = new IcsmOrmQueryBuilder(engine, this._schemasMetadata, this.Logger);
            var executor = new QueryExecutor(engine, icsmOrm, this.Logger);
            this._contextExecutors.Add(key, executor);
            return executor;
        }


        public IQueryBuilder<TModel> GetBuilder<TModel>()
        {
            return new QueryBuilder<TModel>(this.Logger);
        }

        //public IDataEngine GetDataEngine<TContext>() where TContext : IDataContext, new()
        //{
        //    return _dataLayer.GetDataEngine<TContext>();
        //}
    }
}
