using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ICSM_DL = DatalayerCs;
using ICSM_ORM = OrmCs;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class IcsmOrmQueryBuilder : IDisposable
    {
        private readonly IDataEngine _dataEngine;
        private readonly string _initIcsmSchemaPath;
        private readonly ICSM_DL.ANetDb _icsmDb;
        private readonly ICSM_ORM.DbLinker _ormLinker;
        private readonly MethodInfo _ormEndInitMethod;
        private readonly MethodInfo _ormGetSQLTablesMethod;
        private readonly FieldInfo _ormInitDoneField;

        public IcsmOrmQueryBuilder(IDataEngine dataEngine, string initIcsmSchemaPath)
        {
            this._dataEngine = dataEngine;
            this._initIcsmSchemaPath = initIcsmSchemaPath;

            ICSM_DL.DotnetProvider provider = ICSM_DL.DotnetProvider.None;
            string schemaPrefix = string.Empty;
            if (dataEngine.Config.Type == DataEngineType.SqlServer)
            {
                provider = ICSM_DL.DotnetProvider.SqlClient;
                schemaPrefix = "dbo.";
            }
            else if (dataEngine.Config.Type == DataEngineType.Oracle)
            {
                provider = ICSM_DL.DotnetProvider.OracleClient;
            }

            this._icsmDb = ICSM_DL.ANetDb.New(provider);
            this._icsmDb.ConnectionString = dataEngine.Config.ConnectionString;
            this._icsmDb.Open();
            ICSM_ORM.OrmSchema.InitIcsmSchema(this._icsmDb, schemaPrefix, initIcsmSchemaPath);
            this._ormLinker = new ICSM_ORM.DbLinker(this._icsmDb, schemaPrefix);

            var ormType = typeof(ICSM_ORM.OrmRs);
            this._ormEndInitMethod = ormType.GetMethod("EndInit", BindingFlags.NonPublic | BindingFlags.Instance);
            this._ormGetSQLTablesMethod = ormType.GetMethod("GetSQLTables", BindingFlags.NonPublic | BindingFlags.Instance);
            this._ormInitDoneField = ormType.GetField("initDone", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string BuildSelectStatement(QuerySelectStatement statement)
        {
            var icsmOrm = new ICSM_ORM.OrmRs();
            icsmOrm.Init(this._ormLinker);

            var sourceColumns = statement.Table.Columns.Values;
            var icsmColumns = new ICSM_ORM.OrmItem[statement.Table.Columns.Count];
            int index = 0;
            foreach (var column in statement.Table.Columns.Values)
            {
                var icsmColumn = icsmOrm.AddFld(statement.Table.Name, column.Name, null, true);
                if (string.IsNullOrEmpty(column.Alias))
                {
                    column.Alias = column.Name; //"col_" + index.ToString();
                }
                icsmColumn.m_alias = column.Alias;
                icsmColumns[index++] = icsmColumn;
            }

            this._ormEndInitMethod.Invoke(icsmOrm, new object[] { true });

            var sql = new StringBuilder();
   
            if (!statement.IsDistinct)
            {
                sql.AppendLine("SELECT ");
            }
            else
            {
                sql.AppendLine("SELECT DISTINCT ");
            }

            var columnsSql = new string[icsmColumns.Length];
            for (int i = 0; i < icsmColumns.Length; i++)
            {
                var icsmColumn = icsmColumns[i];
                var sqlColumn = icsmColumn.GetDataName();
                columnsSql[i] = "    " + sqlColumn + " AS [" + icsmColumn.m_alias + "]";
            }

            sql.AppendLine(string.Join("," + Environment.NewLine, columnsSql));

            var tablesSql = (string)this._ormGetSQLTablesMethod.Invoke(icsmOrm, new object[] { });
            sql.AppendLine("FROM " + tablesSql);

            return sql.ToString();
        }

        public void Dispose()
        {
            this._ormLinker.Dispose();
            if (this._icsmDb.IsOpen())
            {
                this._icsmDb.Close();
            }
            this._icsmDb.Dispose();
        }
    }
}
