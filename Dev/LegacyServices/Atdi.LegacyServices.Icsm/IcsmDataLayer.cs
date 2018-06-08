using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServices.WebQuery;

namespace Atdi.LegacyServices.Icsm
{
    public sealed class IcsmDataLayer : LoggedObject, IDataLayer<IcsmDataOrm>
    {
        private readonly IDataLayer _dataLayer;
        private readonly IQueryBuilder _queryBuilder;
        private readonly IParserQuery _parserQuery;

        private readonly Dictionary<Type, QueryExecutor> _contextExecutors;
        public IcsmDataLayer(IDataLayer dataLayer, ILogger logger) :  base(logger)
        {
            this._dataLayer = dataLayer;
            this._queryBuilder = new QueryBuilder(logger);
            this._contextExecutors = new Dictionary<Type, QueryExecutor>();
            this._parserQuery = new ParseQuery();
        }

        public IQueryBuilder Builder => _queryBuilder;
        public IParserQuery ParserQuery => _parserQuery;

        public IQueryExecutor Executor<TContext>() where TContext : IDataContext, new()
        {
            var contextType = typeof(TContext);
            if (this._contextExecutors.ContainsKey(contextType))
            {
                return this._contextExecutors[contextType];
            }
            var engine = this._dataLayer.GetDataEngine<TContext>();
            var icsmOrm = new IcsmOrmQueryBuilder(engine, IcsmComponent.IcsmSchemaPath);
            var executor = new QueryExecutor(engine, icsmOrm, this.Logger);
            this._contextExecutors[contextType] = executor;
            return executor;
        }

        public IDataEngine GetDataEngine<TContext>() where TContext : IDataContext, new()
        {
            return _dataLayer.GetDataEngine<TContext>();
        }
    }
}
