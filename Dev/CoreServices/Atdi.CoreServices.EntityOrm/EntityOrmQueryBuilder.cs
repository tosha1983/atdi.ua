using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Contracts.CoreServices.EntityOrm;


namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class EntityOrmQueryBuilder : LoggedObject, IDisposable
    {
        private readonly IDataEngine _dataEngine;
        private readonly IEngineSyntax _syntax;
        private readonly ConditionParser _conditionParser;
        private readonly IEntityOrm _entityMetadata;

        public EntityOrmQueryBuilder(IDataEngine dataEngine, IEntityOrm entityMetadata, ILogger logger) : base(logger)
        {
            this._entityMetadata = entityMetadata;
            this._dataEngine = dataEngine;
            //this._syntax = dataEngine.Syntax;
            //this._conditionParser = new ConditionParser(dataEngine.Syntax);
            logger.Debug(Contexts.LegacyServicesIcsm, Categories.CreatingInstance, Events.CreatedInstanceOfQueryBuilder);
        }

        /// <summary>
        /// Формирование JOIN для связей типа "Entity-BaseEntity"
        /// </summary>
        /// <param name="TableName1"></param>
        /// <param name="TableName2"></param>
        /// <returns></returns>
        private string BuildJoinBase(KeyValuePair<string,string> TableName1, KeyValuePair<string, string> TableName2)
        {
            string resultJoin = "";
            var entityMetadata1 = _entityMetadata.GetEntityMetadata(TableName1.Key);
            var entityMetadata2 = _entityMetadata.GetEntityMetadata(TableName2.Key);
            if ((entityMetadata1!=null) && (entityMetadata2!=null))
            {
                if (entityMetadata1.BaseEntity!=null)
                {
                    if (entityMetadata1.BaseEntity.Name == TableName2.Key)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var c in entityMetadata1.PrimaryKey.FieldRefs)
                        {
                            string PrimaryKeyTable1 = c.Key;
                            KeyValuePair<string, IPrimaryKeyFieldRefMetadata> keyValuePairKeyFieldRefMetadata = entityMetadata2.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == PrimaryKeyTable1);
                            if (keyValuePairKeyFieldRefMetadata.Key != null)
                            {
                                stringBuilder.Append(string.Format(" ({0}.{1} = {2}.{3}) ", TableName1.Value, entityMetadata1.Fields.Values.ToList().Find(z => z.Name == PrimaryKeyTable1).SourceName, TableName2.Value, entityMetadata2.Fields.Values.ToList().Find(z => z.Name == keyValuePairKeyFieldRefMetadata.Key).SourceName));
                            }
                        }
                        if (!string.IsNullOrEmpty(stringBuilder.ToString()))
                        {
                            resultJoin = string.Format(" ( INNER JOIN {0}  {1}  ON ({2}) ",  entityMetadata2.DataSource.Schema + "." + entityMetadata2.DataSource.Name, TableName2.Value, string.Join(" AND ", stringBuilder).ToString());
                        }
                    }
                }
                else if (entityMetadata2.BaseEntity != null)
                {
                    if (entityMetadata2.BaseEntity.Name == TableName1.Key)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var c in entityMetadata2.PrimaryKey.FieldRefs)
                        {
                            string PrimaryKeyTable1 = c.Key;
                            KeyValuePair<string, IPrimaryKeyFieldRefMetadata> keyValuePairKeyFieldRefMetadata = entityMetadata1.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == PrimaryKeyTable1);
                            if (keyValuePairKeyFieldRefMetadata.Key != null)
                            {
                                stringBuilder.Append(string.Format(" ({0}.{1} = {2}.{3}) ", TableName1.Value, entityMetadata1.Fields.Values.ToList().Find(z=>z.Name==PrimaryKeyTable1).SourceName , TableName2.Value, entityMetadata2.Fields.Values.ToList().Find(z => z.Name == keyValuePairKeyFieldRefMetadata.Key).SourceName ));
                            }
                        }
                        if (!string.IsNullOrEmpty(stringBuilder.ToString()))
                        {
                            resultJoin = string.Format(" ( INNER JOIN {0} {1}  ON ({2}) ", entityMetadata2.DataSource.Schema + "." + entityMetadata2.DataSource.Name, TableName2.Value, string.Join(" AND ", stringBuilder).ToString());
                        }
                    }
                }
            }
            return resultJoin;
         }

        /// <summary>
        /// Формирование JOIN для связей типа "Entity-Extrension"
        /// </summary>
        /// <param name="TableName1"></param>
        /// <param name="TableName2"></param>
        /// <returns></returns>
        private string BuildJoinExtension(KeyValuePair<string, string> TableName1, KeyValuePair<string, string> TableName2)
        {
            string resultJoin = "";
            var entityMetadata1 = _entityMetadata.GetEntityMetadata(TableName1.Key);
            var entityMetadata2 = _entityMetadata.GetEntityMetadata(TableName2.Key);
            if ((entityMetadata1 != null) && (entityMetadata2 != null))
            {
                if (entityMetadata1.ExtendEntity != null)
                {
                    if (entityMetadata1.ExtendEntity.Name == TableName2.Key)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var c in entityMetadata1.ExtendEntity.PrimaryKey.FieldRefs)
                        {
                            string PrimaryKeyTable1 = c.Key;
                            stringBuilder.Append(string.Format(" ({0}.{1} = {2}.{3}) ", TableName1.Value, entityMetadata1.Fields.Values.ToList().Find(z => z.Name == PrimaryKeyTable1).SourceName, TableName2.Value, entityMetadata2.ExtendEntity.Fields.Values.ToList().Find(z => z.Name == PrimaryKeyTable1).SourceName));
                        }
                        if (!string.IsNullOrEmpty(stringBuilder.ToString()))
                        {
                            resultJoin = string.Format(" ( INNER JOIN {0} {1}  ON ({2}) ",  entityMetadata1.DataSource.Schema + "." + entityMetadata1.DataSource.Name, TableName1.Value, string.Join(" AND ", stringBuilder).ToString());
                        }
                    }
                }
                else if (entityMetadata2.ExtendEntity != null)
                {
                    if (entityMetadata2.ExtendEntity.Name == TableName1.Key)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var c in entityMetadata2.ExtendEntity.PrimaryKey.FieldRefs)
                        {
                            string PrimaryKeyTable1 = c.Key;
                            stringBuilder.Append(string.Format(" ({0}.{1} = {2}.{3}) ", TableName1.Value, entityMetadata1.Fields.Values.ToList().Find(z => z.Name == PrimaryKeyTable1).SourceName, TableName2.Value, entityMetadata2.ExtendEntity.Fields.Values.ToList().Find(z => z.Name == PrimaryKeyTable1).SourceName));
                            
                        }
                        if (!string.IsNullOrEmpty(stringBuilder.ToString()))
                        {
                            resultJoin = string.Format(" (INNER JOIN {0} {1}  ON ({2}) ",  entityMetadata2.DataSource.Schema + "." + entityMetadata2.DataSource.Name, TableName2.Value, string.Join(" AND ", stringBuilder).ToString());
                        }
                    }
                }
            }
            return resultJoin;
        }

        private string BuidJoinRelation(KeyValuePair<string, string> TableName1, KeyValuePair<string, string> TableName2)
        {
            string resultJoin = "";
            return resultJoin;
        }

       

        private string BuidJoinReference(KeyValuePair<string, string> TableName1, KeyValuePair<string, string> TableName2)
        {
            string resultJoin = "";
            var entityMetadata1 = _entityMetadata.GetEntityMetadata(TableName1.Key);
            var entityMetadata2 = _entityMetadata.GetEntityMetadata(TableName2.Key);
            if ((entityMetadata1 != null) && (entityMetadata2 != null))
            {
                var dictionaryFieldMetadata = entityMetadata1.Fields;
                foreach (var c in dictionaryFieldMetadata)
                {
                    if (c.Value is ReferenceFieldMetadata)
                    {
                        IEntityMetadata refMetaData = ((ReferenceFieldMetadata)(c.Value)).RefEntity;
                        if (refMetaData != null)
                        {
                            if (refMetaData.Name != null)
                            {
                                if (((ReferenceFieldMetadata)(c.Value)).Mapping != null)
                                {
                                    StringBuilder stringBuilder = new StringBuilder();
                                    foreach (var FieldRef in ((ReferenceFieldMetadata)(c.Value)).Mapping.Fields)
                                    {
                                        if (FieldRef.Value != null)
                                        {
                                            if (FieldRef.Value is ValuePrimaryKeyFieldMappedMetadata)
                                            {
                                                ValuePrimaryKeyFieldMappedMetadata valuefieldMetaDataRef = (FieldRef.Value as ValuePrimaryKeyFieldMappedMetadata);
                                                if (valuefieldMetaDataRef != null)
                                                {
                                                    PrimaryKeyMappedMatchWith primaryKeyMappedMatchWith = valuefieldMetaDataRef.MatchWith;
                                                    IFieldMetadata FieldKeyMetadata = valuefieldMetaDataRef.KeyField;
                                                    object value = valuefieldMetaDataRef.Value;
                                                    string NameBase = FieldRef.Key;
                                                }
                                            }
                                            else if (FieldRef.Value is FieldPrimaryKeyFieldMappedMetadata)
                                            {
                                                FieldPrimaryKeyFieldMappedMetadata valuefieldMetaDataRef = (FieldRef.Value as FieldPrimaryKeyFieldMappedMetadata);
                                                if (valuefieldMetaDataRef != null)
                                                {
                                                    PrimaryKeyMappedMatchWith primaryKeyMappedMatchWith = valuefieldMetaDataRef.MatchWith;
                                                    IFieldMetadata FieldKeyMetadata = valuefieldMetaDataRef.KeyField;
                                                    IFieldMetadata FieldEntityMetadata = valuefieldMetaDataRef.EntityField;
                                                    string NameBase = FieldRef.Key;
                                                }
                                            }
                                            else if (FieldRef.Value is SourceNamePrimaryKeyFieldMappedMetadata)
                                            {
                                                SourceNamePrimaryKeyFieldMappedMetadata valuefieldMetaDataRef = (FieldRef.Value as SourceNamePrimaryKeyFieldMappedMetadata);
                                                if (valuefieldMetaDataRef != null)
                                                {
                                                    PrimaryKeyMappedMatchWith primaryKeyMappedMatchWith = valuefieldMetaDataRef.MatchWith;
                                                    string Refsourcename = valuefieldMetaDataRef.SourceName;
                                                    string NameBase = FieldRef.Key;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (c.Value is RelationFieldMetadata)
                    {

                    }
                }
            }
            return resultJoin;
        }



        private void BuildSelectStatement<TModel>(QuerySelectStatement<TModel> statement, string[] fields, out List<QuerySelectStatement.ColumnDescriptor> listColumnDescriptor, out List<IFieldMetadata> listFieldMetadata, out List<IFieldMetadata> listFieldMissingMetadata, FieldSourceType fieldSourceTypeFilter = FieldSourceType.All, bool isPrimaryKeyOnly = false)
        {
            const string ExtendedName = "EXTENDED";
            var entityMetadata = _entityMetadata.GetEntityMetadata(statement.Statement.Table.Name);
            var oldentityMetadata = entityMetadata;
            listColumnDescriptor = new List<QuerySelectStatement.ColumnDescriptor>();
            listFieldMetadata = new List<IFieldMetadata>();
            listFieldMissingMetadata = new List<IFieldMetadata>();

            for (int i = 0; i < fields.Length; i++)
            {
                entityMetadata = oldentityMetadata;
                var fieldName = new KeyValuePair<string, IFieldMetadata>();
                string[] nextNames = fields[i].Split(new char[] { '.' });
                for (int j = 0; j < nextNames.Length; j++)
                {
                    if (nextNames[j] == ExtendedName)
                    {
                        if (entityMetadata.ExtendEntity != null)
                        {
                            bool LastObject = false;
                            entityMetadata = entityMetadata.ExtendEntity;
                            if (j < nextNames.Length - 1)
                            {
                                j++;
                            }
                            else
                            {
                                LastObject = true;
                            }
                            var fieldMetadataFind = entityMetadata.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                            if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                            {
                                var fieldMetadataFindx1 = entityMetadata.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                if (fieldMetadataFindx1 == null)
                                {
                                    bool isAddedField = false;
                                    var oldEntityMetadataPrev = entityMetadata;
                                    entityMetadata = entityMetadata.BaseEntity;
                                    if (entityMetadata != null)
                                    {
                                        var fieldMetadataFindx = entityMetadata.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                        if (fieldMetadataFindx == null)
                                        {
                                            for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                            {
                                                var column = new QuerySelectStatement.ColumnDescriptor();
                                                if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Column) && ((fieldSourceTypeFilter == FieldSourceType.Column) || (fieldSourceTypeFilter== FieldSourceType.All)))
                                                {
                                                    IFieldMetadata fieldMetadataColumn = entityMetadata.Fields.Values.ToList().ElementAt(l);
                                                    string SourceName = fieldMetadataColumn.SourceName;
                                                    string Name = fieldMetadataColumn.Name;
                                                    
                                                    column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceName, Table = entityMetadata.Name };
                                                    if (string.IsNullOrEmpty(column.Alias))
                                                    {
                                                        var Alias = new StringBuilder();
                                                        for (int t = 0; t < j - 1; t++)
                                                        {
                                                            Alias.Append(nextNames[t]).Append(".");
                                                        }
                                                        Alias.Append(ExtendedName).Append(".");
                                                        column.Alias = Alias.ToString() + Name;
                                                    }
                                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                    {
                                                        listColumnDescriptor.Add(column);
                                                        listFieldMetadata.Add(fieldMetadataColumn);
                                                        isAddedField = true;
                                                    }
                                                }
                                                else if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Reference)  && ((fieldSourceTypeFilter == FieldSourceType.Reference) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                                {
                                                    var extensionMetaData = ((ExtensionFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).ExtensionEntity;
                                                    if (extensionMetaData != null)
                                                    {
                                                        entityMetadata = extensionMetaData;
                                                        IFieldMetadata fieldMetadataFindExt = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                        if (fieldMetadataFindExt != null)
                                                        {
                                                            var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFindExt.SourceName, Table = entityMetadata.Name };
                                                            if (string.IsNullOrEmpty(columnExt.Alias))
                                                            {
                                                                StringBuilder Alias = new StringBuilder();
                                                                for (int t = 0; t < j - 1; t++)
                                                                {
                                                                    Alias.Append(nextNames[t]).Append(".");
                                                                }
                                                                Alias.Append(ExtendedName).Append(".");
                                                                columnExt.Alias = Alias.ToString() + entityMetadata.Name;
                                                            }
                                                            if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                            {
                                                                listColumnDescriptor.Add(columnExt);
                                                                listFieldMetadata.Add(fieldMetadataFindExt);
                                                                isAddedField = true;
                                                            }
                                                        }
                                                        else if ((fieldMetadataFindExt == null) && (j == nextNames.Length - 1))
                                                        {
                                                            for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                            {
                                                                if ((entityMetadata.Fields.Values.ToList().ElementAt(lx).SourceType == FieldSourceType.Reference) && ((fieldSourceTypeFilter == FieldSourceType.Reference) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                                                {
                                                                    var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(lx);
                                                                    var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + SourceField.Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(SourceField);
                                                                        isAddedField = true;
                                                                    }
                                                                }
                                                            }
                                                            entityMetadata = oldentityMetadata;
                                                        }
                                                    }
                                                }
                                                else if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Reference)  && ((fieldSourceTypeFilter == FieldSourceType.Reference) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                                {
                                                    var refMetaData = ((ReferenceFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RefEntity;
                                                    if (refMetaData != null)
                                                    {
                                                        if (refMetaData.Name != null)
                                                        {
                                                            entityMetadata = refMetaData;
                                                            var fieldMetadataFindRef = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                            if ((fieldMetadataFindRef == null) && (j == nextNames.Length - 1))
                                                            {
                                                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                                {
                                                                    var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(lx);
                                                                    var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + SourceField.Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(SourceField);
                                                                        isAddedField = true;
                                                                    }
                                                                }
                                                                entityMetadata = oldentityMetadata;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Relation)  && ((fieldSourceTypeFilter == FieldSourceType.Relation) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                                {
                                                    var relMetaData = ((RelationFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RelatedEntity;
                                                    if (relMetaData != null)
                                                    {
                                                        if (relMetaData.Name != null)
                                                        {
                                                            entityMetadata = relMetaData;
                                                            var fieldMetadataFindRel = relMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                            if ((fieldMetadataFindRel == null) && (j == nextNames.Length - 1))
                                                            {
                                                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                                {
                                                                    var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(lx);
                                                                    var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + SourceField.Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(SourceField);
                                                                        isAddedField = true;
                                                                    }
                                                                }
                                                                entityMetadata = oldentityMetadata;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (fieldMetadataFindx.SourceType == FieldSourceType.Column && ((fieldSourceTypeFilter == FieldSourceType.Column) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                            {
                                                var column = new QuerySelectStatement.ColumnDescriptor();
                                                column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFindx.SourceName, Table = entityMetadata.Name };
                                                if (string.IsNullOrEmpty(column.Alias))
                                                {
                                                    StringBuilder Alias = new StringBuilder();
                                                    for (int t = 0; t < j - 1; t++)
                                                    {
                                                        Alias.Append(nextNames[t]).Append(".");
                                                    }
                                                    Alias.Append(ExtendedName).Append(".");
                                                    column.Alias = Alias.ToString() + fieldMetadataFindx.Name;
                                                }
                                                if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                {
                                                    listColumnDescriptor.Add(column);
                                                    listFieldMetadata.Add(fieldMetadataFindx);
                                                    isAddedField = true;
                                                }
                                            }
                                        }
                                        if ((LastObject == true) || (isAddedField == false))
                                        {
                                            entityMetadata = oldEntityMetadataPrev;
                                            for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                            {
                                                var column = new QuerySelectStatement.ColumnDescriptor();
                                                if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Column) && ((fieldSourceTypeFilter == FieldSourceType.Column) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                                {
                                                    var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(l);
                                                    column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                    if (string.IsNullOrEmpty(column.Alias))
                                                    {
                                                        StringBuilder Alias = new StringBuilder();
                                                        for (int t = 0; t < j - 1; t++)
                                                        {
                                                            Alias.Append(nextNames[t]).Append(".");
                                                        }
                                                        Alias.Append(ExtendedName).Append(".");
                                                        column.Alias = Alias.ToString() + SourceField.Name;
                                                    }
                                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                    {
                                                        listColumnDescriptor.Add(column);
                                                        listFieldMetadata.Add(SourceField);
                                                    }
                                                }
                                                else if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Extension)  && ((fieldSourceTypeFilter == FieldSourceType.Extension) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                                {
                                                    var extensionMetaData = ((ExtensionFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).ExtensionEntity;
                                                    if (extensionMetaData != null)
                                                    {
                                                        if (extensionMetaData.Name != null)
                                                        {
                                                            entityMetadata = extensionMetaData;
                                                            var fieldMetadataFindExt = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                            if ((fieldMetadataFindExt == null) && (j == nextNames.Length - 1))
                                                            {
                                                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                                {
                                                                    var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(lx);
                                                                    var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + SourceField.Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(SourceField);
                                                                    }
                                                                }
                                                                entityMetadata = oldentityMetadata;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Reference) && ((fieldSourceTypeFilter == FieldSourceType.Reference) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                                {
                                                    var refMetaData = ((ReferenceFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RefEntity;
                                                    if (refMetaData != null)
                                                    {
                                                        if (refMetaData.Name != null)
                                                        {
                                                            entityMetadata = refMetaData;
                                                            var fieldMetadataFindRef = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                            if ((fieldMetadataFindRef == null) && (j == nextNames.Length - 1))
                                                            {
                                                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                                {
                                                                    var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(lx);
                                                                    var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + SourceField.Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(SourceField);
                                                                    }
                                                                }
                                                                entityMetadata = oldentityMetadata;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Relation) && ((fieldSourceTypeFilter == FieldSourceType.Relation) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                                {
                                                    var relMetaData = ((RelationFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RelatedEntity;
                                                    if (relMetaData != null)
                                                    {
                                                        if (relMetaData.Name != null)
                                                        {
                                                            entityMetadata = relMetaData;
                                                            var fieldMetadataFindRel = relMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                            if ((fieldMetadataFindRel == null) && (j == nextNames.Length - 1))
                                                            {
                                                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                                {
                                                                   var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(lx);
                                                                   var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + SourceField.Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(SourceField);
                                                                    }
                                                                }
                                                                entityMetadata = oldentityMetadata;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        entityMetadata = oldEntityMetadataPrev;
                                        for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                        {
                                            var column = new QuerySelectStatement.ColumnDescriptor();
                                            if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Column)  && ((fieldSourceTypeFilter == FieldSourceType.Column) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                            {
                                                var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(l);
                                                column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                if (string.IsNullOrEmpty(column.Alias))
                                                {
                                                    StringBuilder Alias = new StringBuilder();
                                                    for (int t = 0; t < j - 1; t++)
                                                    {
                                                        Alias.Append(nextNames[t]).Append(".");
                                                    }
                                                    Alias.Append(ExtendedName).Append(".");
                                                    column.Alias = Alias.ToString() + SourceField.Name;
                                                }
                                                if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                {
                                                    listColumnDescriptor.Add(column);
                                                    listFieldMetadata.Add(SourceField);
                                                }
                                            }
                                            else if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Extension)  && ((fieldSourceTypeFilter == FieldSourceType.Extension) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                            {
                                                var extensionMetaData = ((ExtensionFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).ExtensionEntity;
                                                if (extensionMetaData != null)
                                                {
                                                    if (extensionMetaData.Name != null)
                                                    {
                                                        entityMetadata = extensionMetaData;
                                                        var fieldMetadataFindExt = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                        if ((fieldMetadataFindExt == null) && (j == nextNames.Length - 1))
                                                        {
                                                            for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                            {
                                                                var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(lx);
                                                                var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                                if (string.IsNullOrEmpty(columnExt.Alias))
                                                                {
                                                                    StringBuilder Alias = new StringBuilder();
                                                                    for (int t = 0; t < j - 1; t++)
                                                                    {
                                                                        Alias.Append(nextNames[t]).Append(".");
                                                                    }
                                                                    Alias.Append(ExtendedName).Append(".");
                                                                    columnExt.Alias = Alias.ToString() + SourceField.Name;
                                                                }
                                                                if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                {
                                                                    listColumnDescriptor.Add(columnExt);
                                                                    listFieldMetadata.Add(SourceField);
                                                                }
                                                            }
                                                            entityMetadata = oldentityMetadata;
                                                        }
                                                    }
                                                }
                                            }
                                            else if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Reference) && ((fieldSourceTypeFilter == FieldSourceType.Reference) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                            {
                                                var refMetaData = ((ReferenceFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RefEntity;
                                                if (refMetaData != null)
                                                {
                                                    if (refMetaData.Name != null)
                                                    {
                                                        entityMetadata = refMetaData;
                                                        var fieldMetadataFindRef = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                        if ((fieldMetadataFindRef == null) && (j == nextNames.Length - 1))
                                                        {
                                                            for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                            {
                                                                var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(lx);
                                                                var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                                if (string.IsNullOrEmpty(columnExt.Alias))
                                                                {
                                                                    StringBuilder Alias = new StringBuilder();
                                                                    for (int t = 0; t < j - 1; t++)
                                                                    {
                                                                        Alias.Append(nextNames[t]).Append(".");
                                                                    }
                                                                    Alias.Append(ExtendedName).Append(".");
                                                                    columnExt.Alias = Alias.ToString() + SourceField.Name;
                                                                }
                                                                if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                {
                                                                    listColumnDescriptor.Add(columnExt);
                                                                    listFieldMetadata.Add(SourceField);
                                                                }
                                                            }
                                                            entityMetadata = oldentityMetadata;
                                                        }
                                                    }
                                                }
                                            }
                                            else if ((entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Relation) && ((fieldSourceTypeFilter == FieldSourceType.Relation) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                            {
                                                var relMetaData = ((RelationFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RelatedEntity;
                                                if (relMetaData != null)
                                                {
                                                    if (relMetaData.Name != null)
                                                    {
                                                        entityMetadata = relMetaData;
                                                        var fieldMetadataFindRel = relMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                        if ((fieldMetadataFindRel == null) && (j == nextNames.Length - 1))
                                                        {
                                                            for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                            {
                                                                var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(lx);
                                                                var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                                if (string.IsNullOrEmpty(columnExt.Alias))
                                                                {
                                                                    StringBuilder Alias = new StringBuilder();
                                                                    for (int t = 0; t < j - 1; t++)
                                                                    {
                                                                        Alias.Append(nextNames[t]).Append(".");
                                                                    }
                                                                    Alias.Append(ExtendedName).Append(".");
                                                                    columnExt.Alias = Alias.ToString() + SourceField.Name;
                                                                }
                                                                if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                {
                                                                    listColumnDescriptor.Add(columnExt);
                                                                    listFieldMetadata.Add(SourceField);
                                                                }
                                                            }
                                                            entityMetadata = oldentityMetadata;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if ((fieldMetadataFindx1.SourceType == FieldSourceType.Column) && ((fieldSourceTypeFilter == FieldSourceType.Column) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                    {
                                        var column = new QuerySelectStatement.ColumnDescriptor();
                                        column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFindx1.SourceName, Table = entityMetadata.Name };

                                        if (string.IsNullOrEmpty(column.Alias))
                                        {
                                            StringBuilder Alias = new StringBuilder();
                                            for (int t = 0; t < j - 1; t++)
                                            {
                                                Alias.Append(nextNames[t]).Append(".");
                                            }
                                            Alias.Append(ExtendedName).Append(".");
                                            column.Alias = Alias.ToString() + entityMetadata.Name;
                                        }
                                        if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                        {
                                            listColumnDescriptor.Add(column);
                                            listFieldMetadata.Add(fieldMetadataFindx1);
                                        }
                                    }
                                }
                                entityMetadata = oldentityMetadata;
                            }
                        }
                    }
                    fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == nextNames[j]);
                    if (fieldName.Value != null)
                    {
                        if ((fieldName.Value.SourceType == FieldSourceType.Column) && ((fieldSourceTypeFilter == FieldSourceType.Column) || (fieldSourceTypeFilter == FieldSourceType.All)))
                        {
                            if (isPrimaryKeyOnly)
                            {
                                if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == fieldName.Value.Name).Value == null)
                                {
                                    continue;
                                }
                            }

                            var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldName.Value.SourceName, Table = entityMetadata.Name };
                            if (string.IsNullOrEmpty(column.Alias))
                            {
                                column.Alias = fields[i];
                            }
                            if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                            {
                                listColumnDescriptor.Add(column);
                                listFieldMetadata.Add(fieldName.Value);
                            }
                            entityMetadata = oldentityMetadata;
                        }
                        else if ((fieldName.Value.SourceType == FieldSourceType.Extension) && ((fieldSourceTypeFilter == FieldSourceType.Extension) || (fieldSourceTypeFilter == FieldSourceType.All)))
                        {
                            var extensionMetaData = ((ExtensionFieldMetadata)(fieldName.Value)).ExtensionEntity;
                            if (extensionMetaData != null)
                            {
                                if (extensionMetaData.Name != null)
                                {
                                    entityMetadata = extensionMetaData;
                                    var fieldMetadataFind = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                    if (fieldMetadataFind != null)
                                    {
                                        if (isPrimaryKeyOnly)
                                        {
                                            if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == fieldMetadataFind.Name).Value == null)
                                            {
                                                continue;
                                            }
                                        }

                                        var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                        if (string.IsNullOrEmpty(column.Alias))
                                        {
                                            column.Alias = fields[i];
                                        }
                                        if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                        {
                                            listColumnDescriptor.Add(column);
                                            listFieldMetadata.Add(fieldMetadataFind);
                                        }
                                    }
                                    else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                    {
                                        for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                        {
                                            var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(l);
                                            if (isPrimaryKeyOnly)
                                            {
                                                if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == SourceField.Name).Value == null)
                                                {
                                                    continue;
                                                }
                                            }

                                            var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                            if (string.IsNullOrEmpty(column.Alias))
                                            {
                                                column.Alias = fields[i] + "." + SourceField.Name;
                                            }
                                            if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                            {
                                                listColumnDescriptor.Add(column);
                                                listFieldMetadata.Add(SourceField);
                                            }
                                        }
                                        entityMetadata = oldentityMetadata;
                                    }
                                }
                            }
                        }
                        else if ((fieldName.Value.SourceType == FieldSourceType.Reference) && ((fieldSourceTypeFilter == FieldSourceType.Reference) || (fieldSourceTypeFilter == FieldSourceType.All)))
                        {
                            IEntityMetadata refMetaData = ((ReferenceFieldMetadata)(fieldName.Value)).RefEntity;
                            if (refMetaData != null)
                            {
                                if (refMetaData.Name != null)
                                {
                                    entityMetadata = refMetaData;
                                    var fieldMetadataFind = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                    if (fieldMetadataFind != null)
                                    {
                                        if (isPrimaryKeyOnly)
                                        {
                                            if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == fieldMetadataFind.Name).Value == null)
                                            {
                                                continue;
                                            }
                                        }
                                        var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                        if (string.IsNullOrEmpty(column.Alias))
                                        {
                                            column.Alias = fields[i];
                                        }
                                        if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                        {
                                            listColumnDescriptor.Add(column);
                                            listFieldMetadata.Add(fieldMetadataFind);
                                        }
                                    }
                                    else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                    {
                                        for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                        {
                                            var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(l);
                                            if (isPrimaryKeyOnly)
                                            {
                                                if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == SourceField.Name).Value == null)
                                                {
                                                    continue;
                                                }
                                            }


                                            var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                            if (string.IsNullOrEmpty(column.Alias))
                                            {
                                                column.Alias = fields[i] + "." + SourceField.Name;
                                            }
                                            if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                            {
                                                listColumnDescriptor.Add(column);
                                                listFieldMetadata.Add(SourceField);
                                            }
                                        }
                                        entityMetadata = oldentityMetadata;
                                    }
                                }
                            }
                        }
                        else if ((fieldName.Value.SourceType == FieldSourceType.Relation) && ((fieldSourceTypeFilter == FieldSourceType.Relation) || (fieldSourceTypeFilter == FieldSourceType.All)))
                        {
                            var relatedMetaData = ((RelationFieldMetadata)(fieldName.Value)).RelatedEntity;
                            if (relatedMetaData != null)
                            {
                                if (relatedMetaData.Name != null)
                                {
                                    entityMetadata = relatedMetaData;
                                    var fieldMetadataFind = relatedMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                    if (fieldMetadataFind != null)
                                    {
                                        if (isPrimaryKeyOnly)
                                        {
                                            if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == fieldMetadataFind.Name).Value == null)
                                            {
                                                continue;
                                            }
                                        }

                                        var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                        if (string.IsNullOrEmpty(column.Alias))
                                        {
                                            column.Alias = fields[i];
                                        }
                                        if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                        {
                                            listColumnDescriptor.Add(column);
                                            listFieldMetadata.Add(fieldMetadataFind);
                                        }
                                    }
                                    else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                    {
                                        for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                        {
                                            var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(l);
                                            if (isPrimaryKeyOnly)
                                            {
                                                if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == SourceField.Name).Value == null)
                                                {
                                                    continue;
                                                }
                                            }
                                            var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                            if (string.IsNullOrEmpty(column.Alias))
                                            {
                                                column.Alias = fields[i] + "." + SourceField.Name;
                                            }
                                            if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                            {
                                                listColumnDescriptor.Add(column);
                                                listFieldMetadata.Add(SourceField);
                                            }
                                        }
                                        entityMetadata = oldentityMetadata;
                                    }
                                }
                            }
                        }
                        else if ((fieldName.Value.SourceType == FieldSourceType.Expression) && ((fieldSourceTypeFilter == FieldSourceType.Expression) || (fieldSourceTypeFilter == FieldSourceType.All)))
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        var oldEntityMetadataPrev = entityMetadata;
                        //entityMetadata = oldentityMetadata;
                        entityMetadata = entityMetadata.BaseEntity;
                        if (entityMetadata != null)
                        {
                            fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == nextNames[j]);
                            if (fieldName.Value != null)
                            {
                                if ((fieldName.Value.SourceType == FieldSourceType.Column) && ((fieldSourceTypeFilter == FieldSourceType.Column) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                {

                                    if (isPrimaryKeyOnly)
                                    {
                                        if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == fieldName.Value.Name).Value == null)
                                        {
                                            continue;
                                        }
                                    }

                                    var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldName.Value.SourceName, Table = entityMetadata.Name };
                                    if (string.IsNullOrEmpty(column.Alias))
                                    {
                                        column.Alias = fields[i];
                                    }
                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                    {
                                        listColumnDescriptor.Add(column);
                                        listFieldMetadata.Add(fieldName.Value);
                                    }
                                    entityMetadata = oldentityMetadata;
                                }
                                else if ((fieldName.Value.SourceType == FieldSourceType.Extension) && ((fieldSourceTypeFilter == FieldSourceType.Extension) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                {
                                    var extensionMetaData = ((ExtensionFieldMetadata)(fieldName.Value)).ExtensionEntity;
                                    if (extensionMetaData != null)
                                    {
                                        if (extensionMetaData.Name != null)
                                        {
                                            entityMetadata = extensionMetaData;
                                            var fieldMetadataFind = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                            if (fieldMetadataFind != null)
                                            {
                                                if (isPrimaryKeyOnly)
                                                {
                                                    if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == fieldMetadataFind.Name).Value == null)
                                                    {
                                                        continue;
                                                    }
                                                }


                                                var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                                if (string.IsNullOrEmpty(column.Alias))
                                                {
                                                    column.Alias = fields[i];
                                                }
                                                if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                {
                                                    listColumnDescriptor.Add(column);
                                                    listFieldMetadata.Add(fieldMetadataFind);
                                                }
                                            }
                                            else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                            {
                                                for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                                {
                                                    var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(l);
                                                    if (isPrimaryKeyOnly)
                                                    {
                                                        if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == SourceField.Name).Value == null)
                                                        {
                                                            continue;
                                                        }
                                                    }

                                                    var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                    if (string.IsNullOrEmpty(column.Alias))
                                                    {
                                                        column.Alias = fields[i] + "." + SourceField.Name;
                                                    }
                                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                    {
                                                        listColumnDescriptor.Add(column);
                                                        listFieldMetadata.Add(SourceField);
                                                    }
                                                }
                                                entityMetadata = oldentityMetadata;
                                            }
                                        }
                                    }
                                }
                                else if ((fieldName.Value.SourceType == FieldSourceType.Reference) && ((fieldSourceTypeFilter == FieldSourceType.Reference) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                {
                                    IEntityMetadata refMetaData = ((ReferenceFieldMetadata)(fieldName.Value)).RefEntity;
                                    if (refMetaData != null)
                                    {
                                        if (refMetaData.Name != null)
                                        {
                                            entityMetadata = refMetaData;
                                            var fieldMetadataFind = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                            if (fieldMetadataFind != null)
                                            {
                                                if (isPrimaryKeyOnly)
                                                {
                                                    if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == fieldMetadataFind.Name).Value == null)
                                                    {
                                                        continue;
                                                    }
                                                }
                                                var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                                if (string.IsNullOrEmpty(column.Alias))
                                                {
                                                    column.Alias = fields[i];
                                                }
                                                if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                {
                                                    listColumnDescriptor.Add(column);
                                                    listFieldMetadata.Add(fieldMetadataFind);
                                                }
                                            }
                                            else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                            {
                                                for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                                {
                                                    var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(l);
                                                    if (isPrimaryKeyOnly)
                                                    {
                                                        if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == SourceField.Name).Value == null)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                    if (string.IsNullOrEmpty(column.Alias))
                                                    {
                                                        column.Alias = fields[i] + "." + SourceField.Name;
                                                    }
                                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                    {
                                                        listColumnDescriptor.Add(column);
                                                        listFieldMetadata.Add(SourceField);
                                                    }
                                                }
                                                entityMetadata = oldentityMetadata;
                                            }
                                        }
                                    }
                                }
                                else if ((fieldName.Value.SourceType == FieldSourceType.Relation) && ((fieldSourceTypeFilter == FieldSourceType.Relation) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                {
                                    var relatedMetaData = ((RelationFieldMetadata)(fieldName.Value)).RelatedEntity;
                                    if (relatedMetaData != null)
                                    {
                                        if (relatedMetaData.Name != null)
                                        {
                                            entityMetadata = relatedMetaData;
                                            var fieldMetadataFind = relatedMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                            if (fieldMetadataFind != null)
                                            {
                                                if (isPrimaryKeyOnly)
                                                {
                                                    if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == fieldMetadataFind.Name).Value == null)
                                                    {
                                                        continue;
                                                    }
                                                }
                                                var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                                if (string.IsNullOrEmpty(column.Alias))
                                                {
                                                    column.Alias = fields[i];
                                                }
                                                if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                {
                                                    listColumnDescriptor.Add(column);
                                                    listFieldMetadata.Add(fieldMetadataFind);
                                                }
                                            }
                                            else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                            {
                                                for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                                {
                                                    var SourceField = entityMetadata.Fields.Values.ToList().ElementAt(l);
                                                    if (isPrimaryKeyOnly)
                                                    {
                                                        if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == SourceField.Name).Value == null)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = SourceField.SourceName, Table = entityMetadata.Name };
                                                    if (string.IsNullOrEmpty(column.Alias))
                                                    {
                                                        column.Alias = fields[i] + "." + SourceField.Name;
                                                    }
                                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                    {
                                                        listColumnDescriptor.Add(column);
                                                        listFieldMetadata.Add(SourceField);
                                                    }
                                                }
                                                entityMetadata = oldentityMetadata;
                                            }
                                        }
                                    }
                                }
                                else if ((fieldName.Value.SourceType == FieldSourceType.Expression) && ((fieldSourceTypeFilter == FieldSourceType.Expression) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                {
                                    throw new NotImplementedException();
                                }
                            }
                        }
                        entityMetadata = oldEntityMetadataPrev;
                    }
                }
            }

        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string BuildSelectStatement<TModel>(QuerySelectStatement<TModel> statement, IDictionary<string, EngineCommandParameter> parameters) 
         {
            try
            {
                const string PrefixTableName = "Tcaz_";
                var dicPrefixTables = new Dictionary<string, string>();
                var dicLinksSourceNameTables = new Dictionary<string, string>();
                var entityMetadata = _entityMetadata.GetEntityMetadata(statement.Statement.Table.Name);
                string schemaTable = entityMetadata.DataSource.Schema;

                var schemasMetadata = new SchemasMetadata<TModel>(entityMetadata);
                var selectedColumns = statement.Statement.Table.Columns.Values.ToArray();
                var conditionsColumns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Statement.Conditions, conditionsColumns);
                var whereColumns = conditionsColumns.ToArray();
                var sortColumns = statement.Statement.Orders == null ? new QuerySelectStatement.OrderByColumnDescriptor[] { } : statement.Statement.Orders.ToArray();
                var fieldCount = whereColumns.Length + sortColumns.Length;

                var listColumnDescriptorSelected = new List<QuerySelectStatement.ColumnDescriptor>();
                var listFieldMetadataSelected = new List<IFieldMetadata>();
                var listFieldMissingMetadataSelected = new List<IFieldMetadata>();

                var listColumnDescriptorWhere = new List<QuerySelectStatement.ColumnDescriptor>();
                var listFieldMetadataWhere = new List<IFieldMetadata>();
                var listFieldMissingMetadataWhere = new List<IFieldMetadata>();

                var listColumnDescriptorSort = new List<QuerySelectStatement.ColumnDescriptor>();
                var listFieldMetadataSort = new List<IFieldMetadata>();
                var listFieldMissingMetadataSort = new List<IFieldMetadata>();

                BuildSelectStatement<TModel>(statement, selectedColumns.Select(t => t.Name).ToArray(), out listColumnDescriptorSelected, out listFieldMetadataSelected, out listFieldMissingMetadataSelected);


                List<string> distinctTableNames = new List<string>();
                fieldCount += listColumnDescriptorSelected.Count;
                var fieldPaths = new string[fieldCount];
                int index = 0;
                for (int i = 0; i < listColumnDescriptorSelected.Count; i++)
                {
                    fieldPaths[index++] = listColumnDescriptorSelected[i].Name;
                    var dbTableName = listColumnDescriptorSelected[i].Name;
                    if (!dicPrefixTables.ContainsKey(listColumnDescriptorSelected[i].Table))
                    {
                        dicPrefixTables.Add(listColumnDescriptorSelected[i].Table, PrefixTableName + (dicPrefixTables.Count + 1).ToString());
                    }
                    if (!dicLinksSourceNameTables.ContainsKey(listColumnDescriptorSelected[i].Table))
                    {
                        dicLinksSourceNameTables.Add(listColumnDescriptorSelected[i].Table, listColumnDescriptorSelected[i].DBTable);
                    }
                }


                string[] valSelectedFlds = selectedColumns.Select(t => t.Name).ToArray();
                List<string> valSpecialFlds = new List<string>();
                foreach (string val in valSelectedFlds)
                {
                    if (val.Contains("."))
                    {
                        if (val.LastIndexOf(".")>0)
                        {
                            int EndIndex = val.LastIndexOf(".");
                            string valResStr = val.Substring(0, EndIndex);
                            if (!valSpecialFlds.Contains(valResStr))
                                valSpecialFlds.Add(valResStr);
                        }
                    }
                }

                string resultJoin = "";
                for (int i=0; i< valSpecialFlds.Count; i++)
                {
                    
                    string RelTableName = _entityMetadata.RelationFieldMetadata.ToList().Find(z => z.Key.Name == valSpecialFlds[i]).Value;
                    BuildSelectStatement<TModel>(statement, new string[] { valSpecialFlds[i] }, out listColumnDescriptorSelected, out listFieldMetadataSelected, out listFieldMissingMetadataSelected, FieldSourceType.Relation, true);
                    IRelationFieldMetadata relationFieldMetadataFnd = _entityMetadata.RelationFieldMetadata.ToList().Find(z => z.Key.Name == valSpecialFlds[i]).Key;
                    if (relationFieldMetadataFnd != null)
                    {
                        
                    }

                    string RefTableName = _entityMetadata.ReferenceFieldMetadata.ToList().Find(z => z.Key.Name == valSpecialFlds[i]).Value;
                    BuildSelectStatement<TModel>(statement, new string[] { valSpecialFlds[i] }, out listColumnDescriptorSelected, out listFieldMetadataSelected, out listFieldMissingMetadataSelected, FieldSourceType.Reference, true);
                    IReferenceFieldMetadata referenceFieldMetadataFnd = _entityMetadata.ReferenceFieldMetadata.ToList().Find(z => z.Key.Name == valSpecialFlds[i]).Key; 
                    if (referenceFieldMetadataFnd != null)
                    {
                        /*
                        if (referenceFieldMetadataFnd.Mapping==null)
                        {
                            IEntityMetadata metadataRefEntity = referenceFieldMetadataFnd.RefEntity;
                            if (metadataRefEntity != null)
                            {
                                if (metadataRefEntity.PrimaryKey != null)
                                {
                                    StringBuilder stringBuilder = new StringBuilder();
                                    foreach (var FieldRef in metadataRefEntity.PrimaryKey.FieldRefs)
                                    {
                                        if (FieldRef.Key != null)
                                        {
                                           IFieldMetadata columnFrom = metadataRefEntity.Fields.ToList().Find(z => z.Value.Name == FieldRef.Key).Value;
                                           IFieldMetadata columnTo = listFieldMetadataSelected.Find(z => z.Name == FieldRef.Key);
                                        }
                                    }
                                }
                            }       
                        }
                        */
                        //else
                        {
                            if (referenceFieldMetadataFnd.Mapping != null)
                            {
                                string DataSource = referenceFieldMetadataFnd.RefEntity.DataSource.Name;
                                List<string> stringBuilderRef = new List<string>();
                                foreach (var FieldRef in referenceFieldMetadataFnd.Mapping.Fields)
                                {
                                    if (FieldRef.Value != null)
                                    {
                                        if (FieldRef.Value is ValuePrimaryKeyFieldMappedMetadata)
                                        {
                                            ValuePrimaryKeyFieldMappedMetadata valuefieldMetaDataRef = (FieldRef.Value as ValuePrimaryKeyFieldMappedMetadata);
                                            if (valuefieldMetaDataRef != null)
                                            {
                                                IFieldMetadata FieldKeyMetadata = valuefieldMetaDataRef.KeyField;
                                                if (FieldKeyMetadata != null)
                                                {
                                                    QuerySelectStatement.ColumnDescriptor columnDescriptor = listColumnDescriptorSelected.Find(z => z.Name == FieldKeyMetadata.SourceName);
                                                    if (columnDescriptor != null)
                                                    {
                                                        string TableNameFrom = columnDescriptor.DBTable;
                                                        string FieldNameFrom = columnDescriptor.Name;
                                                        if (valuefieldMetaDataRef.Value.GetType()== typeof(System.String))
                                                        {
                                                            stringBuilderRef.Add(string.Format(" ({0}.{1} = '{2}') ", valSpecialFlds[i], FieldNameFrom, valuefieldMetaDataRef.Value));
                                                        }
                                                        else
                                                        {
                                                            stringBuilderRef.Add(string.Format(" ({0}.{1} = {2}) ", valSpecialFlds[i], FieldNameFrom, valuefieldMetaDataRef.Value));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (FieldRef.Value is FieldPrimaryKeyFieldMappedMetadata)
                                        {
                                            FieldPrimaryKeyFieldMappedMetadata valuefieldMetaDataRef = (FieldRef.Value as FieldPrimaryKeyFieldMappedMetadata);
                                            if (valuefieldMetaDataRef != null)
                                            {
                                                if (valuefieldMetaDataRef.KeyField != null)
                                                {
                                                    IFieldMetadata FieldKeyMetadata = valuefieldMetaDataRef.KeyField;
                                                    string FieldName = FieldRef.Key;
                                                    QuerySelectStatement.ColumnDescriptor columnDescriptor = listColumnDescriptorSelected.Find(z => z.Name == FieldKeyMetadata.SourceName);
                                                    if (columnDescriptor != null)
                                                    {
                                                        string FieldNameFrom = columnDescriptor.Name;
                                                        BuildSelectStatement<TModel>(statement, new string[] { FieldName }, out listColumnDescriptorSelected, out listFieldMetadataSelected, out listFieldMissingMetadataSelected);
                                                        if (listColumnDescriptorSelected.Count > 0)
                                                        {
                                                            
                                                            stringBuilderRef.Add(string.Format(" ({0}.{1} = {2}.{3}) ", valSpecialFlds[i], FieldNameFrom, dicPrefixTables.ToList().Find(p => p.Key == dicLinksSourceNameTables.ToList().Find(v => v.Value == RefTableName).Key).Value, listColumnDescriptorSelected[0].Name));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (FieldRef.Value is SourceNamePrimaryKeyFieldMappedMetadata)
                                        {
                                            SourceNamePrimaryKeyFieldMappedMetadata valuefieldMetaDataRef = (FieldRef.Value as SourceNamePrimaryKeyFieldMappedMetadata);
                                            if (valuefieldMetaDataRef != null)
                                            {
                                                string Refsourcename = valuefieldMetaDataRef.SourceName;
                                                string NameBase = FieldRef.Key;
                                                QuerySelectStatement.ColumnDescriptor columnDescriptor = listColumnDescriptorSelected.Find(z => z.Name == NameBase);
                                                if (columnDescriptor != null)
                                                {
                                                    string FieldNameFrom = columnDescriptor.Name;
                                                    stringBuilderRef.Add(string.Format(" ({0}.{1} = {2}.{3}) ", valSpecialFlds[i], FieldNameFrom, dicPrefixTables.ToList().Find(p => p.Key == dicLinksSourceNameTables.ToList().Find(v => v.Value == RefTableName).Key).Value, Refsourcename));
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(stringBuilderRef.ToString()))
                                {
                                   resultJoin += string.Format(" LEFT JOIN {0} {1}  ON ({2}) ", schemaTable+"." + DataSource, valSpecialFlds[i], string.Join(" AND ", stringBuilderRef).ToString());
                                }
                            }
                        }
                    }
                    string ExtTableName = _entityMetadata.ExtensionFieldMetadata.ToList().Find(z => z.Key.Name == valSpecialFlds[i]).Value;
                    IExtensionFieldMetadata extensionFieldMetadataFnd = _entityMetadata.ExtensionFieldMetadata.ToList().Find(z => z.Key.Name == valSpecialFlds[i]).Key;
                    BuildSelectStatement<TModel>(statement, new string[] { valSpecialFlds[i] }, out listColumnDescriptorSelected, out listFieldMetadataSelected, out listFieldMissingMetadataSelected, FieldSourceType.Extension, true);
                    if (extensionFieldMetadataFnd != null)
                    {

                    }
                }

                                         

                ///Формирование JOIN для отношений типа "Entity - BaseEntity"
                ///Перебор всех воможных комбинаций
                string resJoinBaseTable = "";
                for (int i = 0; i < dicPrefixTables.Count - 1; i++)
                {
                    for (int j = 1; j < dicPrefixTables.Count; j++)
                    {
                        resJoinBaseTable+= BuildJoinBase(dicPrefixTables.ElementAt(i), dicPrefixTables.ElementAt(j));
                        resJoinBaseTable += BuildJoinExtension(dicPrefixTables.ElementAt(i), dicPrefixTables.ElementAt(j));
                        resJoinBaseTable += BuidJoinReference(dicPrefixTables.ElementAt(i), dicPrefixTables.ElementAt(j));
                        resJoinBaseTable += BuidJoinRelation(dicPrefixTables.ElementAt(i), dicPrefixTables.ElementAt(j));
                    }
                }
                                            


                BuildSelectStatement<TModel>(statement, whereColumns.Select(t => t.ColumnName).ToArray(), out listColumnDescriptorWhere, out listFieldMetadataWhere, out listFieldMissingMetadataWhere);
                for (int i = 0; i < listColumnDescriptorWhere.Count; i++)
                {
                    fieldPaths[index++] = listColumnDescriptorWhere[i].Name;
                    var dbTableName = entityMetadata.DataSource.Schema + "." + listColumnDescriptorWhere[i].DBTable;
                    if (!dicPrefixTables.ContainsKey(dbTableName))
                    {
                        dicPrefixTables.Add(dbTableName, PrefixTableName + (dicPrefixTables.Count + 1).ToString());
                    }
                }
               

                BuildSelectStatement<TModel>(statement, sortColumns.Select(t => t.Column.Name).ToArray(), out listColumnDescriptorSort, out listFieldMetadataSort, out listFieldMissingMetadataSort);
                for (int i = 0; i < listColumnDescriptorSort.Count; i++)
                {
                    fieldPaths[index++] = listColumnDescriptorSort[i].Name;
                    var dbTableName = entityMetadata.DataSource.Schema + "." + listColumnDescriptorSort[i].DBTable;
                    if (!dicPrefixTables.ContainsKey(dbTableName))
                    {
                        dicPrefixTables.Add(dbTableName, PrefixTableName + (dicPrefixTables.Count+1).ToString());
                    }
                }
               
                //Генератор JOIN's
                var fromExpression = schemasMetadata.BuildJoinStatement(this._dataEngine, fieldPaths, out IFieldMetadata[] dbFields);
                index = 0;

                var columnExpressions = new string[listColumnDescriptorSelected.Count];
                for (int i = 0; i < listColumnDescriptorSelected.Count; i++)
                {
                    var dbField = dbFields[index++];
                    columnExpressions[i] = this._syntax.ColumnExpression(this._syntax.EncodeFieldName(dbField.SourceName), listColumnDescriptorSelected[i].Alias);
                }

                // to build the where section
                for (int i = 0; i < whereColumns.Length; i++)
                {
                    var column = whereColumns[i];
                    var dbField = dbFields[index++];
                    column.ColumnName = dbField.Name;
                }
                var whereExpression = this.BuildWhereExpression(statement.Statement.Conditions, parameters);

                // to build the order by section
                var orderByColumns = new string[sortColumns.Length];
                for (int i = 0; i < sortColumns.Length; i++)
                {
                    var column = sortColumns[i];
                    var dbField = dbFields[index++];
                    var encodeColumn = "";
                    encodeColumn = this._syntax.EncodeFieldName(dbField.Name);
                    orderByColumns[i] = _syntax.SortedColumn(encodeColumn, column.Direction);
                }

                // add on top (n)
                var limit = statement.Statement.Limit;

                // add group by

                var selectStatement = this._syntax.SelectExpression(columnExpressions, fromExpression, whereExpression, orderByColumns, limit);
                return selectStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
         }

       

        public string BuildWhereExpression(List<Condition> conditions, IDictionary<string, EngineCommandParameter> parameters)
         {
             if (conditions == null || conditions.Count == 0)
             {
                 return null;
             }
             var expressions = conditions.Select(condition => this._conditionParser.Parse(condition, parameters)).ToArray();
             var result = this._syntax.Constraint.JoinExpressions(LogicalOperator.And, expressions);
             return result;
         }

         public string BuildSetValuesExpression(List<ColumnValue> columns, IDictionary<string, EngineCommandParameter> parameters)
         {
             if (columns == null || columns.Count == 0)
             {
                 return null;
             }
             var values = new string[columns.Count];
             for (int i = 0; i < columns.Count; i++)
             {
                 var column = columns[i];
                 var parameter = new EngineCommandParameter
                 {
                     DataType = column.DataType,
                     Name = "v_" + column.Name,
                     Value = column.GetValue()

                 };
                parameters.Add(parameter.Name, parameter);
                 values[i] = this._syntax.SetColumnValueExpression(this._syntax.EncodeFieldName(column.Name), this._syntax.EncodeParameterName(parameter.Name));
             }
             var result = string.Join("," + Environment.NewLine, values);
             return result;
         }

         public string BuildDeleteStatement<TModel>(QueryDeleteStatement<TModel> statement, IDictionary<string, EngineCommandParameter> parameters)
         {
             try
             {
                var entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, entityMetadata.DataSource.Name);
                var columns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Conditions, columns);
                var columnsArray = columns.ToArray();
                var schemasMetadata = new SchemasMetadata<TModel>(entityMetadata);
                var fromStatement = schemasMetadata.BuildJoinStatement(this._syntax, entityMetadata.DataSource.Name, columnsArray.Select(c => c.ColumnName).ToArray(), out IFieldMetadata[] dbFields);
                for (int i = 0; i < columnsArray.Length; i++)
                {
                     var column = columnsArray[i];
                     var dbField = dbFields[i];
                     column.ColumnName = dbField.Name;
                 }
                 var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);
                 var deleteStatement = this._syntax.DeleteExpression(sourceExpression, fromStatement, whereExpression);
                 return deleteStatement;
             }
             catch (Exception e)
             {
                 this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                 throw new InvalidOperationException(Exceptions.AbortedBuildDeleteStatement, e);
             }
         }

      
        public string BuildInsertStatement<TModel>(QueryInsertStatement<TModel> statement, IDictionary<string, EngineCommandParameter> parameters)
         {
             try
             {
                 var entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                 var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, entityMetadata.DataSource.Name);
                 var changedColumns = new string[statement.ColumnsValues.Count];
                 var selectedParameters = new string[statement.ColumnsValues.Count];
                 for (int i = 0; i < statement.ColumnsValues.Count; i++)
                 {
                    KeyValuePair<string, IFieldMetadata> fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == statement.ColumnsValues[i].Name);
                    if (fieldName.Value != null)
                    {
                        var column = statement.ColumnsValues[i];
                        column.Name = fieldName.Value.SourceName;
                        var parameter = new EngineCommandParameter
                        {
                            DataType = column.DataType,
                            Name = "v_" + column.Name,
                            Value = column.GetValue()
                        };
                        parameters.Add(parameter.Name, parameter);
                        selectedParameters[i] = this._syntax.EncodeParameterName(parameter.Name);
                        changedColumns[i] = this._syntax.EncodeFieldName(column.Name);
                    }
                 }

                 var columnsExpression = string.Join(", ", changedColumns);
                 var valuesExpression = string.Join(", ", selectedParameters);
                 var insertStatement = this._syntax.InsertExpression(sourceExpression, columnsExpression, valuesExpression);
                 return insertStatement;
             }
             catch (Exception e)
             {
                 this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                 throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
             }
         }

         public string BuildUpdateStatement<TModel>(QueryUpdateStatement<TModel> statement, IDictionary<string, EngineCommandParameter> parameters)
         {
            try
            {
                var columns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Conditions, columns);
                var columnsArray = columns.ToArray();
                IEntityMetadata entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, entityMetadata.DataSource.Name);
                var valuesExpression = this.BuildSetValuesExpression(statement.ColumnsValues, parameters);
                var schemasMetadata = new SchemasMetadata<TModel>(entityMetadata);
                var fromStatement = schemasMetadata.BuildJoinStatement(this._syntax, entityMetadata.DataSource.Name, columnsArray.Select(c => c.ColumnName).ToArray(), out IFieldMetadata[] dbFields);
                for (int i = 0; i < columnsArray.Length; i++)
                {
                    var column = columnsArray[i];
                    var dbField = dbFields[i];
                    column.ColumnName = dbField.Name;
                }
                var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);
                var updateStatement = this._syntax.UpdateExpression(sourceExpression, valuesExpression, fromStatement, whereExpression);
                return updateStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
            }
         }

         private void AppendColumnsFromConditions(List<Condition> conditions, List<ColumnOperand> columns)
         {
             if (conditions == null || conditions.Count == 0)
             {
                 return;
             }

             for (int i = 0; i < conditions.Count; i++)
             {
                 AppendColumnsFromCondition(conditions[i], columns);
             }
         }

         private void AppendColumnsFromCondition(Condition condition, List<ColumnOperand> columns)
         {
             if (condition.Type == ConditionType.Complex)
             {
                 var complexCondition = condition as ComplexCondition;
                 var conditions = complexCondition.Conditions;
                 if (conditions != null && conditions.Length > 0)
                 {
                     for (int i = 0; i < conditions.Length; i++)
                     {
                         AppendColumnsFromCondition(conditions[i], columns);
                     }
                 }
                 return;
             }
             else if (condition.Type == ConditionType.Expression)
             {
                 var conditionExpression = condition as ConditionExpression;
                 AppendColumnFromOperand(conditionExpression.LeftOperand, columns);
                 AppendColumnFromOperand(conditionExpression.RightOperand, columns);
             }
         }

         private void AppendColumnFromOperand(Operand operand, List<ColumnOperand> columns)
         {
             if (operand == null)
             {
                 return;
             }
             if (operand.Type == OperandType.Column)
             {
                 var columnOperand = operand as ColumnOperand;
                 columns.Add(columnOperand);
             }
         }


        public void Dispose()
        {
        }

    }
}
