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
        public  IParseQuery _parserQuery;

        private readonly Dictionary<Type, QueryExecutor> _contextExecutors;
        public IcsmDataLayer(IDataLayer dataLayer, ILogger logger) :  base(logger)
        {
            this._dataLayer = dataLayer;
            this._queryBuilder = new QueryBuilder(logger);
            this._contextExecutors = new Dictionary<Type, QueryExecutor>();
        }

        public IQueryBuilder Builder => _queryBuilder;
        public IParseQuery Parser => _parserQuery;

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
            _parserQuery = executor.GetParse();
            this._contextExecutors[contextType] = executor;
            return executor;
        }

        public IParseQuery GetQueryParser<TContext>() where TContext : IDataContext, new()
        {
            var engine = this._dataLayer.GetDataEngine<TContext>();
            var icsmOrm = new IcsmOrmQueryBuilder(engine, IcsmComponent.IcsmSchemaPath);
            //((ParseQuery)(this._parserQuery))._report = icsmOrm._icsmReport;
            return this._parserQuery;
        }

            public IDataEngine GetDataEngine<TContext>() where TContext : IDataContext, new()
        {
            return _dataLayer.GetDataEngine<TContext>();
        }
    }
}
