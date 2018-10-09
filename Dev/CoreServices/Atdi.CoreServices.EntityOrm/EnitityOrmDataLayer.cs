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
        

        public EnitityOrmDataLayer(IDataLayer dataLayer, IEntityOrm entityOrm, ILogger logger) : base(logger)
        {
            this._dataLayer = dataLayer;
            this._queryBuilder = new QueryBuilder(entityOrm,  logger);
        }

        public IQueryBuilder Builder => _queryBuilder;

        public IQueryExecutor Executor<TContext>() where TContext : IDataContext, new()
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TModel> GetBuilder<TModel>()
        {
            throw new NotImplementedException();
        }

        public IDataEngine GetDataEngine<TContext>() where TContext : IDataContext, new()
        {
            return _dataLayer.GetDataEngine<TContext>();
        }
    }
}
