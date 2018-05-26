using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class QueryExecutor : LoggedObject, IQueryExecutor
    {
        private readonly IDataEngine _dataEngine;
        private readonly IcsmOrmQueryBuilder _icsmOrmQueryBuilder;

        public QueryExecutor(IDataEngine dataEngine, IcsmOrmQueryBuilder icsmOrmQueryBuilder, ILogger logger) : base(logger)
        {
            
            this._dataEngine = dataEngine;
            this._icsmOrmQueryBuilder = icsmOrmQueryBuilder;

            logger.Debug(Contexts.LegacyServicesIcsm, Categories.CreatingInstance, Events.CreatedInstanceOfQueryExecutor);
        }

        public TResult Fetch<TResult>(IQuerySelectStatement statement, Func<IDataReader, TResult> handler)
        {
            var command = this.BuildSelectCommand(statement as QuerySelectStatement);

            var result = default(TResult);
            _dataEngine.Execute(command, reader => 
            {
                result = handler(reader);
            });
            return result;
        }

        private EngineCommand BuildSelectCommand(QuerySelectStatement statement)
        {
            var rootStatement = this._icsmOrmQueryBuilder.BuildSelectStatement(statement);

            // add on top (n)
            // add conditions
            // add group by
            // add conditions
            // add order by

            return new EngineCommand
            {
                Text = rootStatement
            };
        }
    }
}
