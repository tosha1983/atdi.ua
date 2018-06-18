using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.Platform.Logging;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class IcsmOrmQueryBuilder : LoggedObject, IDisposable
    {
        private readonly IDataEngine _dataEngine;
        private readonly Orm.SchemasMetadata _schemasMetadata;

        public IcsmOrmQueryBuilder(IDataEngine dataEngine, Orm.SchemasMetadata schemasMetadata, ILogger logger) : base(logger)
        {
            this._dataEngine = dataEngine;
            this._schemasMetadata = schemasMetadata;

            logger.Debug(Contexts.LegacyServicesIcsm, Categories.CreatingInstance, Events.CreatedInstanceOfQueryBuilder);
        }

        public string BuildSelectStatement(QuerySelectStatement statement)
        {
            try
            {
                var sourceColumns = statement.Table.Columns.Values;
                var fieldPaths = new string[statement.Table.Columns.Count];
                int index = 0;
                foreach (var column in sourceColumns)
                {
                    if (string.IsNullOrEmpty(column.Alias))
                    {
                        column.Alias = column.Name; 
                    }
                    fieldPaths[index++] = column.Name;
                }

                var sql = this._schemasMetadata.BuildSelectStatement(_dataEngine.Syntax, statement.Table.Name, fieldPaths);
                return sql;
            }
            catch(Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
        }

        public void Dispose()
        {
        }
    }
}
