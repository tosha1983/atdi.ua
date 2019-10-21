using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class DataLayerScope : LoggedObject, IDataLayerScope
    {
        private readonly Orm.SchemasMetadata _schemasMetadata;
        private readonly IDataEngine _dataEngine;
        private QueryExecutor _queryExecutor;
        private IEngineExecuter _engineExecuter;

        public DataLayerScope(IDataEngine dataEngine, Orm.SchemasMetadata schemasMetadata, ILogger logger) 
            : base(logger)
        {
            this._schemasMetadata = schemasMetadata;
            this._dataEngine = dataEngine;
            this._engineExecuter = this._dataEngine.CreateExecuter();
            var icsmOrm = new IcsmOrmQueryBuilder(this._dataEngine, this._schemasMetadata, this.Logger);
            this._queryExecutor = new QueryExecutor(this._dataEngine, icsmOrm, logger);

        }


        public TransactionIsolationLevel IsolationLevel => this._engineExecuter.IsolationLevel;

        public bool HasTransaction => this._engineExecuter.HasTransaction;

        public IQueryExecutor Executor => this._queryExecutor;

        public void BeginTran(TransactionIsolationLevel isoLevel = TransactionIsolationLevel.Default)
        {
            this._engineExecuter.BeginTran(isoLevel);
        }

        public void Commit()
        {
            this._engineExecuter.Commit();
        }

        public void Dispose()
        {
            this._queryExecutor = null;
            if (_engineExecuter != null)
            {
                _engineExecuter.Dispose();
                _queryExecutor = null;
            }
        }

        public void Rollback()
        {
            this._engineExecuter.Rollback();
        }

    }
}
