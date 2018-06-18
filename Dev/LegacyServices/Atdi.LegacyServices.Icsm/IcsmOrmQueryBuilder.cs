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

        //private Tuple<ICSM_DL.ANetDb, ICSM_ORM.DbLinker> InitializeIcsmEnvironment()
        //{
        //    try
        //    {
        //        var provider = ICSM_DL.DotnetProvider.None;
        //        string schemaPrefix = string.Empty;
        //        if (_dataEngine.Config.Type == DataEngineType.SqlServer)
        //        {
        //            provider = ICSM_DL.DotnetProvider.SqlClient;
        //            schemaPrefix = "dbo.";
        //        }
        //        else if (_dataEngine.Config.Type == DataEngineType.Oracle)
        //        {
        //            provider = ICSM_DL.DotnetProvider.OracleClient;
        //            schemaPrefix = "ICSM.";
        //        }

        //        var icsmDb = ICSM_DL.ANetDb.New(provider);
        //        icsmDb.ConnectionString = _dataEngine.Config.ConnectionString;
        //        icsmDb.Open();
        //        ICSM_ORM.OrmSchema.InitIcsmSchema(icsmDb, schemaPrefix, _initIcsmSchemaPath);
        //        ICSM_ORM.OrmSchema.ParseSchema(_initIcsmSchemaPath, "WebQuery", "XICSM_WebQuery.dll", out string outErr);
        //        var ormLinker = new ICSM_ORM.DbLinker(icsmDb, schemaPrefix);

        //        return new Tuple<ICSM_DL.ANetDb, ICSM_ORM.DbLinker>(icsmDb, ormLinker);
        //    }
        //    catch (Exception e)
        //    {
        //        this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.CreatingInstance, e, this);
        //        throw new InvalidOperationException(Exceptions.InvalideInitializeIcsmEnvironment, e);
        //    }
        //}

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

                var dbms = Orm.DBMS.MsSql;
                var schemaPrefix = "dbo.";
                var quoteColumn = "[]";

                if (this._dataEngine.Config.Type == DataEngineType.Oracle)
                {
                    dbms = Orm.DBMS.Oracle;
                    schemaPrefix = "ICSM.";
                    quoteColumn = "\"\"";
                }

                var sql = this._schemasMetadata.BuildSelectStatement(statement.Table.Name, fieldPaths, schemaPrefix, dbms, quoteColumn);
                return sql;
            }
            catch(Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
        }

        //private string FormatFromStatement(string expression)
        //{
        //    int ident = -1;
        //    string identValue = "";

        //    var sql = new StringBuilder();

        //    foreach (var symbol in expression)
        //    {
        //        if(symbol == '(')
        //        {
        //            ++ident;
        //            if (ident > 0)
        //            {
        //                //identValue += "    ";
        //                sql.Append(Environment.NewLine);
        //                sql.Append(identValue);
                        
        //            }
        //            sql.Append(symbol);
        //            ++ident;
        //            identValue += "    ";
        //            sql.Append(Environment.NewLine);
        //            sql.Append(identValue);
        //        }
        //        else if (symbol == ')')
        //        {
        //            --ident;
        //            identValue = identValue.Substring(0, identValue.Length - 4);
        //            sql.Append(Environment.NewLine);
        //            sql.Append(identValue);
        //            sql.Append(symbol);
        //        }
        //        else
        //        {
        //            sql.Append(symbol);
        //        }
                
        //    }
        //    return sql.ToString();
        //}
        public void Dispose()
        {
        }
    }
}
