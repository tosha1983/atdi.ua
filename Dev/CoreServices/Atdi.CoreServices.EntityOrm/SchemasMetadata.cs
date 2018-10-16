using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Atdi.Platform.Logging;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Contracts.CoreServices.EntityOrm;


namespace Atdi.CoreServices.EntityOrm
{
    public sealed class SchemasMetadata<TModel>
    {
        public  IDataEngine _configDataEngine;
        private IEntityMetadata _entityMetadata;

        public SchemasMetadata(IEntityMetadata entityMetadata)
        {
            this._entityMetadata = entityMetadata;
        }
        public string DbSchema => this._entityMetadata.DataSource.Schema;


        internal string BuildJoinStatement(IDataEngine config,  string[] fieldPaths, out IFieldMetadata[] selectedFieldsOut)
        {
            var schemaPrefix = DbSchema + ".";
            this._configDataEngine = config;
            var dbTables = new Dictionary<string, IEntityMetadata>();
            var dbFields = new List<IFieldMetadata>();
            var dbWorldFields = new Dictionary<string, IFieldMetadata>();
            selectedFieldsOut = new IFieldMetadata[fieldPaths.Length];
            for (int i = 0; i < fieldPaths.Length; i++)
            {
                var fieldPath = fieldPaths[i];
                //Здесь нужно пересмотреть работу функии this.AddField
                //var dbField = this.AddField(statement.Statement.Table.Name, fieldPath, schemaPrefix, dbTables, dbFields, dbWorldFields, expressColumns);
                //dbField.Path = fieldPath;
                //selectedFields[i] = dbField;
            }
            var joinSql = BuildJoinStatement(config.Syntax, this._entityMetadata.DataSource.Name, schemaPrefix, dbTables, dbFields, dbWorldFields);
            var formatedSql = FormatJoinStatement(joinSql);
            return formatedSql;
        }


        public string BuildJoinStatement(IEngineSyntax engineSyntax, string tableName, string[] fieldPaths, out IFieldMetadata[] selectedFieldsOut)
        {
            var schemaPrefix = DbSchema + ".";
            var dbTables = new Dictionary<string, IEntityMetadata>();
            var dbFields = new List<IFieldMetadata>();
            var dbWorldFields = new Dictionary<string, IFieldMetadata>();
            selectedFieldsOut = new IFieldMetadata[fieldPaths.Length];
            for (int i = 0; i < fieldPaths.Length; i++)
            {
                var fieldPath = fieldPaths[i];
                //Здесь нужно пересмотреть работу функии this.AddField
                //var dbField = this.AddField(tableName, fieldPath, schemaPrefix, dbTables,  dbFields, dbWorldFields);
                //dbField.Path = fieldPath;
                //selectedFields[i] = dbField;
            }

            var joinSql = BuildJoinStatement(engineSyntax, tableName, schemaPrefix, dbTables,  dbFields, dbWorldFields);
            var formatedSql = FormatJoinStatement(joinSql);

            return formatedSql;
        }

        private string BuildJoinStatement(IEngineSyntax engineSyntax, string tableName, string schemaPrefix, Dictionary<string, IEntityMetadata> dbTables, List<IFieldMetadata> dbFields, Dictionary<string, IFieldMetadata> dbWorldFields)
        {
            string joinSql = string.Empty;
        
            return joinSql;
        }

        private static string FormatJoinStatement(string expression)
        {
            int ident = -1;
            string identValue = "";
            var sql = new StringBuilder();
            foreach (var symbol in expression)
            {
                if (symbol == '(')
                {
                    ++ident;
                    if (ident > 0 && symbol != '(')
                    {
                        sql.Append(Environment.NewLine);
                        sql.Append(identValue);

                    }
                    sql.Append(symbol);
                    ++ident;
                    identValue += "    ";
                    sql.Append(Environment.NewLine);
                    sql.Append(identValue);
                }
                else if (symbol == ')')
                {
                    --ident;
                    identValue = identValue.Substring(0, identValue.Length - 4);
                    sql.Append(Environment.NewLine);
                    sql.Append(identValue);
                    sql.Append(symbol);
                }
                else
                {
                    sql.Append(symbol);
                }

            }
            return sql.ToString();
        }

    }
}
