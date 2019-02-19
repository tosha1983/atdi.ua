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
            this._syntax = dataEngine.Syntax;
            this._conditionParser = new ConditionParser(dataEngine.Syntax);
            logger.Debug(Contexts.LegacyServicesEntity, Categories.CreatingInstance, Events.CreatedInstanceOfQueryBuilder);
        }

        /// <summary>
        /// Формирование JOIN для связей типа "Entity-BaseEntity"
        /// </summary>
        /// <param name="TableName1"></param>
        /// <param name="TableName2"></param>
        /// <returns></returns>
        private string BuildJoinBase(AliasField TableName1, AliasField TableName2)
        {
            string resultJoin = "";
            try
            {
                var entityMetadata1 = _entityMetadata.GetEntityMetadata(TableName1.EntityName);
                var entityMetadata2 = _entityMetadata.GetEntityMetadata(TableName2.EntityName);
                if ((entityMetadata1 != null) && (entityMetadata2 != null))
                {
                    if (entityMetadata1.BaseEntity != null)
                    {
                        if (entityMetadata1.BaseEntity.Name == TableName2.EntityName)
                        {
                            var stringBuilder = new List<string>();
                            foreach (var c in entityMetadata1.PrimaryKey.FieldRefs)
                            {
                                string PrimaryKeyTable1 = c.Key;
                                var keyValuePairKeyFieldRefMetadata = entityMetadata1.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == PrimaryKeyTable1);
                                if (keyValuePairKeyFieldRefMetadata.Key != null)
                                {
                                    IFieldMetadata fieldMetadata1 = entityMetadata1.Fields.Values.ToList().Find(z => z.Name == PrimaryKeyTable1);
                                    IFieldMetadata fieldMetadata2 = entityMetadata2.Fields.Values.ToList().Find(z => z.Name == keyValuePairKeyFieldRefMetadata.Key);
                                    if ((fieldMetadata1 != null) && (fieldMetadata2 != null))
                                    {
                                        stringBuilder.Add(string.Format(" ({0}.{1} = {2}.{3}) ", this._syntax.EncodeFieldName(TableName1.Alias), this._syntax.EncodeFieldName(fieldMetadata1.SourceName), this._syntax.EncodeFieldName(TableName2.Alias), this._syntax.EncodeFieldName(fieldMetadata2.SourceName)));
                                    }
                                }
                            }
                            if (stringBuilder.Count > 0)
                            {
                                resultJoin += Environment.NewLine + string.Format(Templates.CommmentsBuildJoin, this._syntax.EncodeFieldName(TableName1.DBTableName), this._syntax.EncodeFieldName(TableName2.DBTableName)) + Environment.NewLine;
                                resultJoin += string.Format(" INNER JOIN {0}  {1}  ON ({2}) ", entityMetadata2.DataSource.Schema + "." + entityMetadata2.DataSource.Name, this._syntax.EncodeFieldName(TableName2.Alias), string.Join(" AND ", stringBuilder).ToString());
                            }
                        }
                    }
                    else if (entityMetadata2.BaseEntity != null)
                    {
                        if (entityMetadata2.BaseEntity.Name == TableName1.EntityName)
                        {
                            var stringBuilder = new List<string>();
                            foreach (var c in entityMetadata2.PrimaryKey.FieldRefs)
                            {
                                string PrimaryKeyTable1 = c.Key;
                                var keyValuePairKeyFieldRefMetadata = entityMetadata2.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == PrimaryKeyTable1);
                                if (keyValuePairKeyFieldRefMetadata.Key != null)
                                {
                                    IFieldMetadata fieldMetadata1 = entityMetadata1.Fields.Values.ToList().Find(z => z.Name == PrimaryKeyTable1);
                                    IFieldMetadata fieldMetadata2 = entityMetadata2.Fields.Values.ToList().Find(z => z.Name == keyValuePairKeyFieldRefMetadata.Key);
                                    if ((fieldMetadata1 != null) && (fieldMetadata2 != null))
                                    {
                                        stringBuilder.Add(string.Format(" ({0}.{1} = {2}.{3}) ", this._syntax.EncodeFieldName(TableName1.Alias), this._syntax.EncodeFieldName(fieldMetadata1.SourceName), this._syntax.EncodeFieldName(TableName2.Alias), this._syntax.EncodeFieldName(fieldMetadata2.SourceName)));
                                    }
                                }
                            }
                            if (stringBuilder.Count > 0)
                            {
                                resultJoin += Environment.NewLine + string.Format(Templates.CommmentsBuildJoin, this._syntax.EncodeFieldName(TableName1.DBTableName), this._syntax.EncodeFieldName(TableName2.DBTableName)) + Environment.NewLine;
                                resultJoin += string.Format(" INNER JOIN {0} {1}  ON ({2}) ", entityMetadata1.DataSource.Schema + "." + entityMetadata1.DataSource.Name, this._syntax.EncodeFieldName(TableName1.Alias), string.Join(" AND ", stringBuilder).ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
            return resultJoin;
        }

        /// <summary>
        /// Формирование JOIN для связей типа "Relation"
        /// </summary>
        /// <param name="TableName1"></param>
        /// <param name="TableName2"></param>
        /// <returns></returns>
        private string BuidJoinRelation(string EntityName, string SpecialFld, IDictionary<string, EngineCommandParameter> parameters, List<AliasField> aliasFields, string schemaTable, string resultJoinValue)
        {
            string resultJoin = "";
            try
            {
                var listFieldProperties =  BuildSelectStatement(EntityName, new string[] { SpecialFld }, FieldSourceType.Relation, true);
                var fieldProperties = listFieldProperties.Find(z => z.Alias == SpecialFld && z.SourceType == FieldSourceType.Relation);
                if (fieldProperties == null)
                {
                    fieldProperties = listFieldProperties.Find(z => isContainField(z.Alias, (SpecialFld + ".")) && z.SourceType == FieldSourceType.Relation);
                }
                if (fieldProperties != null)
                {
                    string RelTableNameTo = "";
                    string RelTableNameFrom = "";
                    IRelationFieldMetadata relationFieldMetadataFnd = null;
                    bool isSuccessBuildRelation = false;
                    RelTableNameTo = fieldProperties.DBTableName;
                    var aliasFieldFind = aliasFields.Find(z => z.DBTableName == RelTableNameTo /*&& z.IsReplaced == false*/);
                    if (aliasFieldFind != null)
                    {
                        if (SpecialFld.Length < this._syntax.MaxLengthAlias)
                        {
                            aliasFieldFind.Alias = SpecialFld;
                            aliasFieldFind.IsReplaced = true;
                        }
                    }

                    var entityObject = _entityMetadata.GetEntityMetadata(EntityName);
                    RelTableNameFrom = entityObject.DataSource.Name;
                    var findReferenceMetaData = entityObject.Fields;
                    var fndMeta = findReferenceMetaData.ToList().FindAll(z =>  z.Value.SourceType == FieldSourceType.Relation).Select(c=>c.Value);
                    foreach (var x in fndMeta)
                    {
                        if (x is RelationFieldMetadata rel)
                        {
                            if (rel.RelatedEntity != null)
                            {
                                if (rel.RelatedEntity.DataSource.Name == RelTableNameTo)
                                {
                                    relationFieldMetadataFnd = rel;
                                }
                            }
                        }
                    }

                    string reJoins = this.BuildWhereExpression(new List<Condition> { relationFieldMetadataFnd.RelationCondition }, parameters);
                    if (relationFieldMetadataFnd != null)
                    {
                        IEntityMetadata metadataRelEntity = relationFieldMetadataFnd.RelatedEntity;
                        if (metadataRelEntity != null)
                        {
                           
                            bool isFindedBlock = false;
                            do
                            {
                                if (reJoins.IndexOf("#source") > 0)
                                {
                                    isFindedBlock = true;
                                    int zeroIdx = reJoins.IndexOf("#source");
                                    int idxStart = zeroIdx + 7;
                                    reJoins = reJoins.Remove(zeroIdx, idxStart - zeroIdx);
                                    reJoins = reJoins.Insert(zeroIdx, SpecialFld + "\"");
                                    idxStart = zeroIdx + SpecialFld.Length + 2;
                                    string rsVal = reJoins.Substring(idxStart, reJoins.Length - idxStart);
                                    if (rsVal.IndexOf("\"") > 0)
                                    {
                                        int idxEnd = rsVal.IndexOf("\"");
                                        rsVal = reJoins.Substring(idxStart, idxEnd);
                                        var dataField = metadataRelEntity.Fields.ToList().Find(z => z.Value.Name == rsVal);
                                        IFieldMetadata fieldMeta = dataField.Value;
                                        if (fieldMeta != null)
                                        {
                                            reJoins = reJoins.Remove(idxStart, idxEnd);
                                            reJoins = reJoins.Insert(idxStart, "\"" + fieldMeta.SourceName);
                                        }
                                        else
                                        {
                                            isFindedBlock = false;
                                        }
                                    }
                                }
                                else if (reJoins.IndexOf("#this") > 0)
                                {
                                    isFindedBlock = true;
                                    int zeroIdx = reJoins.IndexOf("#this");
                                    int idxStart = zeroIdx + 5;
                                    reJoins = reJoins.Remove(zeroIdx, idxStart - zeroIdx);
                                    var aliasField = aliasFields.Find(p => p.DBTableName == RelTableNameFrom);
                                    string prefix = aliasField != null ? aliasField.Alias : throw new Exception(string.Format(Exceptions.NotFoundAlias, RelTableNameFrom));
                                    reJoins = reJoins.Insert(zeroIdx, prefix + "\"");
                                    idxStart = zeroIdx + prefix.Length + 2;
                                    string rsVal = reJoins.Substring(idxStart, reJoins.Length - idxStart);
                                    if (rsVal.IndexOf("\"") > 0)
                                    {
                                        int idxEnd = rsVal.IndexOf("\"");
                                        rsVal = reJoins.Substring(idxStart, idxEnd);
                                        var dataField = metadataRelEntity.Fields.ToList().Find(z => z.Value.Name == rsVal);
                                        IFieldMetadata fieldMeta = dataField.Value;
                                        if (fieldMeta != null)
                                        {
                                            reJoins = reJoins.Remove(idxStart, idxEnd);
                                            reJoins = reJoins.Insert(idxStart, "\"" + fieldMeta.SourceName);
                                        }
                                        else
                                        {
                                            string[] wrds = rsVal.Split(new char[] { '.' });
                                            List<string> concatString = new List<string>();
                                            if (wrds != null)
                                            {
                                                if (wrds.Length > 1)
                                                {
                                                    for (int x = 0; x < wrds.Length - 1; x++)
                                                    {
                                                        concatString.Add(wrds[x]);
                                                    }
                                                }
                                                if (wrds.Length > 0)
                                                {
                                                    var dataFieldBlock = metadataRelEntity.Fields.ToList().Find(z => z.Value.Name == wrds[wrds.Length - 1]);
                                                    fieldMeta = dataFieldBlock.Value;
                                                    if (fieldMeta != null)
                                                    {
                                                        reJoins = reJoins.Remove(idxStart, idxEnd);
                                                        reJoins = reJoins.Insert(idxStart, "\"" + fieldMeta.SourceName);
                                                        reJoins = reJoins.Remove(idxStart - prefix.Length - 2, prefix.Length);
                                                        reJoins = reJoins.Insert(idxStart - prefix.Length - 2, string.Join(".", concatString));
                                                    }
                                                    else
                                                    {
                                                        isFindedBlock = false;
                                                    }
                                                }
                                                else
                                                {
                                                    isFindedBlock = false;
                                                }
                                            }
                                            else
                                            {
                                                isFindedBlock = false;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    isFindedBlock = false;
                                }
                                isSuccessBuildRelation = true;
                            }
                            while (isFindedBlock == true);
                        }
                    }
                    if (isSuccessBuildRelation)
                    {
                        if (!string.IsNullOrEmpty(reJoins))
                        {
                            string joinVal = Environment.NewLine + string.Format(Templates.CommmentsBuildJoinRelation, RelTableNameTo, RelTableNameFrom) + Environment.NewLine;
                            joinVal+= string.Format(" LEFT JOIN {0} {1}  ON ({2}) ", schemaTable + "." + RelTableNameTo, this._syntax.EncodeFieldName(SpecialFld), reJoins);
                            if (!resultJoinValue.Contains(joinVal))
                            {
                                resultJoin += joinVal;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
            return resultJoin;
        }

        /// <summary>
        /// Переименование Alias 
        /// </summary>
        /// <param name="countSubLevels"></param>
        /// <param name="aliasFieldFind"></param>
        /// <param name="RefTableNameTo"></param>
        /// <param name="SpecialFld"></param>
        /// <param name="aliasFields"></param>
        private void RenameAliases(int countSubLevels, AliasField aliasFieldFind, string RefTableNameTo, string SpecialFld, ref  List<AliasField> aliasFields)
        {
            do
            {
                if (aliasFieldFind.EntityMetadataLinks != null)
                {
                    if (((countSubLevels == 0) && (aliasFieldFind.EntityMetadataLinks.Count == 0)) == false)
                    {
                        RefTableNameTo = aliasFieldFind.EntityMetadataLinks[countSubLevels].DataSource.Name;
                        if (countSubLevels < aliasFieldFind.EntityMetadataLinks.Count - 1)
                        {
                            string[] wrds = SpecialFld.Split(new char[] { '.' });
                            if (wrds != null)
                            {
                                if (wrds.Length > 0)
                                {
                                    if (aliasFieldFind != null)
                                    {
                                        SpecialFld = SpecialFld.Replace("." + wrds[wrds.Length - 1], "");
                                        var aliasFieldCheck = aliasFields.Find(z => z.DBTableName == RefTableNameTo /*&& z.IsReplaced == false*/);
                                        if (aliasFieldCheck != null)
                                        {
                                            if (SpecialFld.Length < this._syntax.MaxLengthAlias)
                                            {
                                                aliasFieldCheck.Alias = SpecialFld;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                --countSubLevels;
            }
            while (countSubLevels > 0);
        }

        private int GetIndexTable(List<IEntityMetadata> listEntityMetaData, string Name)
        {
           return listEntityMetaData.FindIndex(c => c.DataSource.Name == Name);
        }

        private bool isContainField(string inStr, string checkStr)
        {
            bool isContain = false;
            if (inStr.Contains(checkStr))
            {
                var countchInStr = inStr.Count(chr => chr == '.');
                var countcheckStr = checkStr.Count(chr => chr == '.');
                if (countchInStr== countcheckStr)
                {
                    isContain = true;
                }
            }
            return isContain;
        }

        private IFieldMetadata FindMetaDataValue(List<KeyValuePair<string,IFieldMetadata>> listFieldMetadata, FieldSourceType fieldSourceType, string TableName)
        {
            IFieldMetadata fndMeta = null;
            for (int i=0; i< listFieldMetadata.Count; i++)
            {
                if (listFieldMetadata[i].Value != null)
                {
                    if (listFieldMetadata[i].Value is ExtensionFieldMetadata extensionFieldMetadata)
                    {
                        if (extensionFieldMetadata != null)
                        {
                            var value = extensionFieldMetadata as ExtensionFieldMetadata;
                            if (value != null)
                            {
                                if (value.ExtensionEntity != null)
                                {
                                    if ((value.ExtensionEntity.DataSource.Name == TableName) && (value.SourceType == FieldSourceType.Extension))
                                    {
                                        fndMeta = extensionFieldMetadata;
                                    }
                                }
                            }

                        }
                    }
                    else if (listFieldMetadata[i].Value is ReferenceFieldMetadata RefrenceFieldMetadata)
                    {
                        if (RefrenceFieldMetadata != null)
                        {
                            var value = RefrenceFieldMetadata as ReferenceFieldMetadata;
                            if (value != null)
                            {
                                if (value.RefEntity != null)
                                {
                                    if ((value.RefEntity.DataSource.Name == TableName) && (value.SourceType == FieldSourceType.Reference))
                                    {
                                        fndMeta = RefrenceFieldMetadata;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return fndMeta;
        }

        private string FindMetaDataKey(List<KeyValuePair<string, IFieldMetadata>> listFieldMetadata, FieldSourceType fieldSourceType, string TableName)
        {
            string fndName = null;
            for (int i = 0; i < listFieldMetadata.Count; i++)
            {
                if (listFieldMetadata[i].Value != null)
                {
                    if (listFieldMetadata[i].Value is ExtensionFieldMetadata extensionFieldMetadata)
                    {
                        if (extensionFieldMetadata != null)
                        {
                            var value = extensionFieldMetadata as ExtensionFieldMetadata;
                            if (value != null)
                            {
                                if (value.ExtensionEntity != null)
                                {
                                    if ((value.ExtensionEntity.DataSource.Name == TableName) && (value.SourceType == FieldSourceType.Extension))
                                    {
                                        fndName = listFieldMetadata[i].Key;
                                    }
                                }
                            }
                        }
                    }
                    else if (listFieldMetadata[i].Value is ReferenceFieldMetadata referenceFieldMetadata)
                    {
                        if (referenceFieldMetadata != null)
                        {
                            var value = referenceFieldMetadata as ReferenceFieldMetadata;
                            if (value != null)
                            {
                                if (value.RefEntity != null)
                                {
                                    if ((value.RefEntity.DataSource.Name == TableName) && (value.SourceType == FieldSourceType.Reference))
                                    {
                                        fndName = listFieldMetadata[i].Key;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return fndName;
        }

       

        private string BuidJoinExtension(string EntityName, string SpecialFld, List<AliasField> aliasFields, string schemaTable, string resultJoinValue, List<FieldProperties> listFieldProperties)
        {
            string resultJoin = "";
            try
            {
                string RefTableNameTo = "";
                string RefTableNameFrom = "";
                IExtensionFieldMetadata referenceFieldMetadataFnd = null;
                var fieldProperties = listFieldProperties.Find(z => z.Alias== SpecialFld);
                if (fieldProperties == null)
                {
                    fieldProperties = listFieldProperties.Find(z => isContainField(z.Alias,(SpecialFld + ".")));
                }
                if (fieldProperties != null)
                {
                    RefTableNameTo = fieldProperties.DBTableName;
                    var aliasFieldFind = aliasFields.Find(z => z.DBTableName == RefTableNameTo /* && z.IsReplaced == false*/);
                    if (aliasFieldFind != null)
                    {
                        if (SpecialFld.Length < this._syntax.MaxLengthAlias)
                        {
                            aliasFieldFind.Alias = SpecialFld;
                            aliasFieldFind.IsReplaced = true;
                        }
                        else
                        {
                            aliasFieldFind.IsReplaced = true;
                        }
                        int countSubLevels = 0;
                        if (aliasFieldFind.EntityMetadataLinks != null)
                        {
                            if (aliasFieldFind.EntityMetadataLinks.Count > 0)
                            {
                                countSubLevels = aliasFieldFind.EntityMetadataLinks.Count - 1;
                            }
                        }
                        RenameAliases(countSubLevels, aliasFieldFind, RefTableNameTo, SpecialFld, ref aliasFields);
                        var stringBuilderRefGlobal = new List<string>();
                        if (aliasFieldFind.EntityMetadataLinks != null)
                        {
                            if (aliasFieldFind.EntityMetadataLinks.Count > 0)
                            {
                                countSubLevels = aliasFieldFind.EntityMetadataLinks.Count - 1;
                            }
                        }
                        RefTableNameTo = fieldProperties.DBTableName;
                        RefTableNameFrom = EntityName;
                        do
                        {
                            if ((countSubLevels == 0) && (aliasFieldFind.EntityMetadataLinks == null))
                            {
                                string name = RefTableNameFrom;
                                RefTableNameFrom = _entityMetadata.GetEntityMetadata(name).DataSource.Name;
                                var findReferenceMetaData = _entityMetadata.GetEntityMetadata(name).Fields;
                                IFieldMetadata fndMeta = FindMetaDataValue(findReferenceMetaData.ToList(), FieldSourceType.Extension, RefTableNameTo);
                                string fndMetaName = FindMetaDataKey(findReferenceMetaData.ToList(), FieldSourceType.Extension, RefTableNameTo);
                                if (fndMeta != null)
                                {
                                    referenceFieldMetadataFnd = fndMeta as ExtensionFieldMetadata;
                                }
                            }
                            else if (countSubLevels == 0)
                            {
                                if (aliasFieldFind.EntityMetadataLinks.Count == 0)
                                {
                                    string name = RefTableNameFrom;
                                    RefTableNameFrom = _entityMetadata.GetEntityMetadata(name).DataSource.Name;
                                    var findReferenceMetaData = _entityMetadata.GetEntityMetadata(name).Fields;
                                    IFieldMetadata fndMeta = FindMetaDataValue(findReferenceMetaData.ToList(), FieldSourceType.Extension, RefTableNameTo);
                                    string fndMetaName = FindMetaDataKey(findReferenceMetaData.ToList(), FieldSourceType.Extension, RefTableNameTo);
                                    if (fndMeta != null)
                                    {
                                        referenceFieldMetadataFnd = fndMeta as ExtensionFieldMetadata;
                                    }
                                }
                            }
                            else
                            {

                                RefTableNameTo = aliasFieldFind.EntityMetadataLinks[countSubLevels].DataSource.Name;

                                int index = GetIndexTable(aliasFieldFind.EntityMetadataLinks, RefTableNameTo);
                                index = index - 1;
                                if (index < 0)
                                {
                                    index = 0;
                                }

                                RefTableNameFrom = aliasFieldFind.EntityMetadataLinks[index].DataSource.Name;
                                string name = aliasFieldFind.EntityMetadataLinks[index].Name;
                                var findReferenceMetaData = _entityMetadata.GetEntityMetadata(name).Fields;
                                IFieldMetadata fndMeta = FindMetaDataValue(findReferenceMetaData.ToList(), FieldSourceType.Extension, RefTableNameTo);
                                string fndMetaName = FindMetaDataKey(findReferenceMetaData.ToList(), FieldSourceType.Extension, RefTableNameTo);
                                if (fndMeta != null)
                                {
                                    referenceFieldMetadataFnd = fndMeta as ExtensionFieldMetadata;

                                }

                                var aliasFieldCheck = aliasFields.Find(z => z.DBTableName == RefTableNameTo);
                                if (aliasFieldCheck != null)
                                {
                                    SpecialFld = aliasFieldCheck.Alias;
                                }

                            }
                            if (referenceFieldMetadataFnd != null)
                            {
                                string DataSource = "";
                                IEntityMetadata metadataExtEntity = referenceFieldMetadataFnd.ExtensionEntity;
                                if (metadataExtEntity != null)
                                {
                                    IEntityMetadata metadataExtendedEntity = metadataExtEntity.ExtendEntity;
                                    DataSource = referenceFieldMetadataFnd.ExtensionEntity.DataSource.Name;
                                    var stringBuilderRef = new List<string>();
                                    if (metadataExtEntity.PrimaryKey != null)
                                    {
                                        foreach (var FieldRef in metadataExtEntity.PrimaryKey.FieldRefs)
                                        {
                                            if (FieldRef.Key != null)
                                            {
                                                if (metadataExtendedEntity != null)
                                                {
                                                    IFieldMetadata fieldMetadataDb2 = metadataExtendedEntity.Fields.ToList().Find(z => z.Key == FieldRef.Key).Value;
                                                    var aliasFieldTo = aliasFields.Find(v => v.DBTableName == RefTableNameTo);
                                                    var aliasFieldFrom = aliasFields.Find(v => v.DBTableName == RefTableNameFrom);
                                                    var entityMetadataRefTableName = _entityMetadata.GetEntityMetadata(aliasFieldFrom != null ? aliasFieldFrom.EntityName : throw new Exception(string.Format(Exceptions.NotFoundAlias, RefTableNameTo)));
                                                    if ((entityMetadataRefTableName != null) && (fieldMetadataDb2 != null))
                                                    {
                                                        foreach (var FieldRefV in entityMetadataRefTableName.PrimaryKey.FieldRefs)
                                                        {
                                                            if (FieldRef.Key == FieldRefV.Key)
                                                            {
                                                                string columnFrom = FieldRefV.Key;
                                                                IFieldMetadata fieldMetadata = entityMetadataRefTableName.Fields.ToList().Find(v => v.Key == columnFrom).Value;
                                                                if (fieldMetadata != null)
                                                                {
                                                                    var Db1Val = aliasFields.Find(v => v.DBTableName == RefTableNameFrom);
                                                                    var Db2Val = aliasFields.Find(v => v.DBTableName == metadataExtEntity.DataSource.Name);
                                                                    string Db1 = Db1Val != null ? Db1Val.Alias : throw new Exception(string.Format(Exceptions.NotFoundAlias, RefTableNameFrom));
                                                                    string Db2 = Db2Val != null ? Db2Val.Alias : throw new Exception(string.Format(Exceptions.NotFoundAlias, metadataExtEntity.DataSource.Name));
                                                                    stringBuilderRef.Add(string.Format(" ({0}.{1} = {2}.{3}) ", this._syntax.EncodeFieldName(Db1), fieldMetadata.SourceName, this._syntax.EncodeFieldName(Db2), fieldMetadataDb2.SourceName));
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (stringBuilderRef.Count > 0)
                                    {
                                        stringBuilderRefGlobal.Add(Environment.NewLine + string.Format(Templates.CommmentsBuildJoinExtension, RefTableNameTo, RefTableNameFrom) + Environment.NewLine + string.Format(" LEFT JOIN {0} {1}  ON ({2}) ", schemaTable + "." + DataSource, this._syntax.EncodeFieldName(SpecialFld), string.Join(" AND ", stringBuilderRef).ToString()));
                                    }
                                }
                            }
                            --countSubLevels;
                        }
                        while (countSubLevels > 0);

                        stringBuilderRefGlobal.Reverse();
                        foreach (string d in stringBuilderRefGlobal)
                        {
                            resultJoin += d;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
            return resultJoin;
        }


        /// <summary>
        /// Формирование JOIN для связей типа "Reference"
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="EntityName"></param>
        /// <param name="SpecialFld"></param>
        /// <param name="aliasFields"></param>
        /// <param name="schemaTable"></param>
        /// <returns></returns>
        private string BuidJoinReference(string EntityName, string SpecialFld,  List<AliasField> aliasFields, string schemaTable, string resultJoinValue)
        {
            string resultJoin = "";
            try
            {
                string RefTableNameTo = "";
                string RefTableNameFrom = "";
                IReferenceFieldMetadata referenceFieldMetadataFnd = null;
                var listFieldProperties = BuildSelectStatement(EntityName, new string[] { SpecialFld }, FieldSourceType.Reference, true);
                var fieldProperties = listFieldProperties.Find(z => z.Alias == SpecialFld && z.SourceType == FieldSourceType.Reference);
                if (fieldProperties == null)
                {
                    fieldProperties = listFieldProperties.Find(z => isContainField(z.Alias, (SpecialFld + ".")) && z.SourceType == FieldSourceType.Reference);
                }
                if (fieldProperties != null)
                {
                    RefTableNameTo = fieldProperties.DBTableName;
                    var aliasFieldFind = aliasFields.Find(z => z.DBTableName == RefTableNameTo /* && z.IsReplaced == false */);
                    if (aliasFieldFind != null)
                    {
                        if (SpecialFld.Length < this._syntax.MaxLengthAlias)
                        {
                            aliasFieldFind.Alias = SpecialFld;
                            aliasFieldFind.IsReplaced = true;
                        }
                        else
                        {
                            aliasFieldFind.IsReplaced = true;
                        }
                        int countSubLevels = 0;
                        if (aliasFieldFind.EntityMetadataLinks != null)
                        {
                            if (aliasFieldFind.EntityMetadataLinks.Count > 0)
                            {
                                countSubLevels = aliasFieldFind.EntityMetadataLinks.Count - 1;
                            }
                        }
                        RenameAliases(countSubLevels, aliasFieldFind, RefTableNameTo, SpecialFld, ref aliasFields);
                        var stringBuilderRefGlobal = new List<string>();
                        if (aliasFieldFind.EntityMetadataLinks != null)
                        {
                            if (aliasFieldFind.EntityMetadataLinks.Count > 0)
                            {
                                countSubLevels = aliasFieldFind.EntityMetadataLinks.Count - 1;
                            }
                        }
                        RefTableNameTo = fieldProperties.DBTableName;
                        RefTableNameFrom = EntityName;
                        do
                        {
                            if ((countSubLevels == 0) && (aliasFieldFind.EntityMetadataLinks == null))
                            {
                                string name = RefTableNameFrom;
                                RefTableNameFrom = _entityMetadata.GetEntityMetadata(name).DataSource.Name;
                                var findReferenceMetaData = _entityMetadata.GetEntityMetadata(name).Fields;
                                IFieldMetadata fndMeta = FindMetaDataValue(findReferenceMetaData.ToList(), FieldSourceType.Reference, RefTableNameTo);
                                string fndMetaName = FindMetaDataKey(findReferenceMetaData.ToList(), FieldSourceType.Reference, RefTableNameTo);
                                if (fndMeta != null)
                                {
                                    referenceFieldMetadataFnd = fndMeta as ReferenceFieldMetadata;
                                }
                            }
                            else if (countSubLevels == 0)
                            {
                                if (aliasFieldFind.EntityMetadataLinks.Count == 0)
                                {
                                    string name = RefTableNameFrom;
                                    RefTableNameFrom = _entityMetadata.GetEntityMetadata(name).DataSource.Name;
                                    var findReferenceMetaData = _entityMetadata.GetEntityMetadata(name).Fields;
                                    IFieldMetadata fndMeta = FindMetaDataValue(findReferenceMetaData.ToList(), FieldSourceType.Reference, RefTableNameTo);
                                    string fndMetaName = FindMetaDataKey(findReferenceMetaData.ToList(), FieldSourceType.Reference, RefTableNameTo);
                                    if (fndMeta != null)
                                    {
                                        referenceFieldMetadataFnd = fndMeta as ReferenceFieldMetadata;
                                    }
                                }
                            }
                            else
                            {

                                RefTableNameTo = aliasFieldFind.EntityMetadataLinks[countSubLevels].DataSource.Name;


                                int index = GetIndexTable(aliasFieldFind.EntityMetadataLinks, RefTableNameTo);
                                index = index - 1;
                                if (index < 0)
                                {
                                    index = 0;
                                }
                               
                                RefTableNameFrom = aliasFieldFind.EntityMetadataLinks[index].DataSource.Name;
                                string name = aliasFieldFind.EntityMetadataLinks[index].Name;
                                var findReferenceMetaData = _entityMetadata.GetEntityMetadata(name).Fields;
                                IFieldMetadata fndMeta = FindMetaDataValue(findReferenceMetaData.ToList(), FieldSourceType.Reference, RefTableNameTo);
                                string fndMetaName = FindMetaDataKey(findReferenceMetaData.ToList(), FieldSourceType.Reference, RefTableNameTo);
                                if (fndMeta != null)
                                {
                                    referenceFieldMetadataFnd = fndMeta as ReferenceFieldMetadata;
                                }

                                var aliasFieldCheck = aliasFields.Find(z => z.DBTableName == RefTableNameTo);
                                if (aliasFieldCheck != null)
                                {
                                    SpecialFld = aliasFieldCheck.Alias;
                                }

                            }
                            if (referenceFieldMetadataFnd != null)
                            {
                                string DataSource = "";
                                if (referenceFieldMetadataFnd.Mapping == null)
                                {
                                    IEntityMetadata metadataRefEntity = referenceFieldMetadataFnd.RefEntity;
                                    if (metadataRefEntity != null)
                                    {
                                        DataSource = referenceFieldMetadataFnd.RefEntity.DataSource.Name;
                                        var stringBuilderRef = new List<string>();
                                        if (metadataRefEntity.PrimaryKey != null)
                                        {
                                            foreach (var FieldRef in metadataRefEntity.PrimaryKey.FieldRefs)
                                            {
                                                if (FieldRef.Key != null)
                                                {
                                                    IFieldMetadata fieldMetadataDb2 = metadataRefEntity.Fields.ToList().Find(z => z.Key == FieldRef.Key).Value;
                                                    var aliasFieldTo = aliasFields.Find(v => v.DBTableName == RefTableNameTo);
                                                    var aliasFieldFrom = aliasFields.Find(v => v.DBTableName == RefTableNameFrom);
                                                    var entityMetadataRefTableName = _entityMetadata.GetEntityMetadata(aliasFieldFrom != null ? aliasFieldFrom.EntityName : throw new Exception(string.Format(Exceptions.NotFoundAlias, RefTableNameTo)));
                                                    if ((entityMetadataRefTableName != null) && (fieldMetadataDb2 != null))
                                                    {
                                                        foreach (var FieldRefV in entityMetadataRefTableName.PrimaryKey.FieldRefs)
                                                        {
                                                            if (FieldRef.Key == FieldRefV.Key)
                                                            {
                                                                string columnFrom = FieldRefV.Key;
                                                                IFieldMetadata fieldMetadata = entityMetadataRefTableName.Fields.ToList().Find(v => v.Key == columnFrom).Value;
                                                                if (fieldMetadata != null)
                                                                {
                                                                    var Db1Val = aliasFields.Find(v => v.DBTableName == RefTableNameFrom);
                                                                    var Db2Val = aliasFields.Find(v => v.DBTableName == metadataRefEntity.DataSource.Name);

                                                                    string Db1 = Db1Val != null ? Db1Val.Alias : throw new Exception(string.Format(Exceptions.NotFoundAlias, RefTableNameFrom));
                                                                    string Db2 = Db2Val != null ? Db2Val.Alias : throw new Exception(string.Format(Exceptions.NotFoundAlias, metadataRefEntity.DataSource.Name));

                                                                    stringBuilderRef.Add(string.Format(" ({0}.{1} = {2}.{3}) ", this._syntax.EncodeFieldName(Db1), fieldMetadata.SourceName, this._syntax.EncodeFieldName(Db2), fieldMetadataDb2.SourceName));

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (stringBuilderRef.Count>0)
                                        {
                                            stringBuilderRefGlobal.Add(Environment.NewLine + string.Format(Templates.CommmentsBuildJoinReference, RefTableNameTo, RefTableNameFrom) + Environment.NewLine + string.Format(" LEFT JOIN {0} {1}  ON ({2}) ", schemaTable + "." + DataSource, this._syntax.EncodeFieldName(SpecialFld), string.Join(" AND ", stringBuilderRef).ToString()));
                                        }
                                    }
                                }
                                else
                                {
                                    if (referenceFieldMetadataFnd.Mapping != null)
                                    {
                                        DataSource = referenceFieldMetadataFnd.RefEntity.DataSource.Name;
                                        var stringBuilderRef = new List<string>();
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
                                                            string TableNameFrom = fieldProperties.DBTableName;
                                                            string FieldNameFrom = fieldProperties.FieldName;
                                                            if (valuefieldMetaDataRef.Value.GetType() == typeof(System.String))
                                                            {
                                                                stringBuilderRef.Add(string.Format(" ({0}.{1} = '{2}') ", this._syntax.EncodeFieldName(SpecialFld), FieldNameFrom, valuefieldMetaDataRef.Value));
                                                            }
                                                            else
                                                            {
                                                                stringBuilderRef.Add(string.Format(" ({0}.{1} = {2}) ", this._syntax.EncodeFieldName(SpecialFld), FieldNameFrom, valuefieldMetaDataRef.Value));
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
                                                            string FieldNameFrom = fieldProperties.DBFieldName;
                                                            var DbValx = aliasFields.Find(v => v.DBTableName == RefTableNameFrom);
                                                            var entityMetadataRefTableName = _entityMetadata.GetEntityMetadata(DbValx != null ? DbValx.EntityName : throw new Exception(string.Format(Exceptions.NotFoundAlias, RefTableNameFrom)));
                                                            if (entityMetadataRefTableName != null)
                                                            {
                                                                var valueMetaData = entityMetadataRefTableName.Fields.ToList().Find(z => z.Key == FieldName).Value;
                                                                if (valueMetaData != null)
                                                                {
                                                                    string columnFrom = valueMetaData.SourceName;
                                                                    stringBuilderRef.Add(string.Format(" ({0}.{1} = {2}.{3}) ", this._syntax.EncodeFieldName(SpecialFld), FieldNameFrom, this._syntax.EncodeFieldName((DbValx != null ? DbValx.Alias : throw new Exception(string.Format(Exceptions.NotFoundAlias, RefTableNameFrom)))), columnFrom));
                                                                }
                                                                else
                                                                {
                                                                    throw new Exception(string.Format(Exceptions.NotFoundDetailInformation, FieldName));
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

                                                        string FieldNameFrom = fieldProperties.FieldName;
                                                        var DbValx = aliasFields.Find(v => v.DBTableName == RefTableNameFrom);
                                                        stringBuilderRef.Add(string.Format(" ({0}.{1} = {2}.{3}) ", this._syntax.EncodeFieldName(SpecialFld), FieldNameFrom, this._syntax.EncodeFieldName((DbValx != null ? DbValx.Alias : throw new Exception(string.Format(Exceptions.NotFoundAlias, RefTableNameFrom)))), Refsourcename));

                                                    }
                                                }
                                            }
                                        }
                                        if (stringBuilderRef.Count > 0)
                                        {
                                            string joinVal = Environment.NewLine + string.Format(Templates.CommmentsBuildJoinReference, RefTableNameTo, RefTableNameFrom) + Environment.NewLine + string.Format(" LEFT JOIN {0} {1}  ON ({2}) ", schemaTable + "." + DataSource, this._syntax.EncodeFieldName(SpecialFld), string.Join(" AND ", stringBuilderRef).ToString());
                                            if (!stringBuilderRefGlobal.Contains(joinVal))
                                            {
                                                if (!resultJoinValue.Contains(joinVal))
                                                {
                                                    stringBuilderRefGlobal.Add(joinVal);
                                                }
                                               
                                            }
                                        }
                                    }
                                }
                            }
                            --countSubLevels;
                        }
                        while (countSubLevels > 0);

                        stringBuilderRefGlobal.Reverse();
                        foreach (string d in stringBuilderRefGlobal)
                        {
                            resultJoin += d;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
            return resultJoin;
        }

        /// <summary>
        /// Основной обработчик генерации JOIN'ов
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="EntityName"></param>
        /// <param name="fieldPath"></param>
        /// <param name="parameters"></param>
        /// <param name="aliasFields"></param>
        /// <returns></returns>
        private string BuildJoinStatement(string EntityName, IEnumerable<string> fieldPathValue, IDictionary<string, EngineCommandParameter> parameters, out List<AliasField> aliasFields, ref List<FieldProperties> listFieldPropertiesOut)
        {
            var resJoinBaseTable = "";

            listFieldPropertiesOut = new List<FieldProperties>();
            const string PrefixTableName = "Tcaz_";
            var entityMetadata = _entityMetadata.GetEntityMetadata(EntityName);
            var schemaTable = entityMetadata.DataSource.Schema;
            var selectedColumns = new List<string>(fieldPathValue);
            var listFieldProperties = BuildSelectStatement(EntityName, selectedColumns);
            listFieldPropertiesOut.AddRange(listFieldProperties);
            aliasFields = new List<AliasField>();
            for (int i = 0; i < listFieldProperties.Count; i++)
            {
                var dbTableName = listFieldProperties[i].DBTableName;
                if (aliasFields.Find(z => z.EntityName == listFieldProperties[i].TableName) == null)
                {
                    aliasFields.Add(new AliasField() { Alias = PrefixTableName + (aliasFields.Count + 1).ToString(), DBTableName = listFieldProperties[i].DBTableName, EntityName = listFieldProperties[i].TableName, EntityMetadataLinks = listFieldProperties[i].entityMetadataLinks });
                }
                if (listFieldProperties[i].entityMetadataLinks != null)
                {
                    for (int j = 0; j < listFieldProperties[i].entityMetadataLinks.Count; j++)
                    {
                        if (aliasFields.Find(z => z.EntityName == listFieldProperties[i].entityMetadataLinks[j].Name) == null)
                        {
                            aliasFields.Add(new AliasField() { Alias = PrefixTableName + (aliasFields.Count + 1).ToString(), DBTableName = listFieldProperties[i].entityMetadataLinks[j].DataSource.Name, EntityName = listFieldProperties[i].entityMetadataLinks[j].Name });
                        }
                    }
                }

                var aliasFieldFind1 = aliasFields.Find(z => z.DBTableName == listFieldProperties[i].DBTableName);
                var aliasFieldFind2 = aliasFields.Find(z => z.DBTableName == entityMetadata.DataSource.Name);
                if ((aliasFieldFind1 != null) && (aliasFieldFind2 != null))
                {
                    resJoinBaseTable += BuildJoinBase(aliasFieldFind1, aliasFieldFind2);
                }
            }
            var valSelectedFlds = selectedColumns;
            var valSpecialFlds = new List<string>();
            foreach (string val in valSelectedFlds)
            {
                if (val.Contains("."))
                {
                    if (val.LastIndexOf(".") > 0)
                    {
                        var EndIndex = val.LastIndexOf(".");
                        var valResStr = val.Substring(0, EndIndex);
                        if (!valSpecialFlds.Contains(valResStr))
                        {
                            valSpecialFlds.Add(valResStr);
                        }
                    }
                }
                else if ((val.ToCharArray().ToList().FindAll(z => char.IsUpper(z) || char.IsDigit(z)).Count == val.ToCharArray().Length) && (!val.Contains(".")))
                {
                    valSpecialFlds.Add(val);
                }
            }

            var resultJoin = "";
            for (int i = 0; i < valSpecialFlds.Count; i++)
            {
                bool isSuccess = false;
                var joinString = "";
                // Calc BASE entity
               
                var fieldProperties = listFieldProperties.Find(z => z.Alias == valSpecialFlds[i]);
                if (fieldProperties == null)
                {
                    fieldProperties = listFieldProperties.Find(z => isContainField(z.Alias, (valSpecialFlds[i] + ".")));
                    if (fieldProperties != null)
                    {
                        if (entityMetadata != null)
                        {
                            var aliasFieldFind1 = aliasFields.Find(z => z.DBTableName == fieldProperties.DBTableName);
                            var aliasFieldFind2 = aliasFields.Find(z => z.DBTableName == entityMetadata.DataSource.Name);
                            if ((aliasFieldFind1 != null) && (aliasFieldFind2 != null))
                            {
                                resJoinBaseTable += BuildJoinBase(aliasFieldFind1, aliasFieldFind2);
                            }
                        }
                    }
                }

               
               
                // Calc EXTENSION entity
                if (!isSuccess)
                {
                    joinString = BuidJoinExtension(EntityName, valSpecialFlds[i], aliasFields, schemaTable, resultJoin, listFieldPropertiesOut);
                    if (!string.IsNullOrEmpty(joinString))
                    {
                        if (!resultJoin.Contains(joinString))
                        {
                            resultJoin += joinString;
                        }
                    }
                }
                

                // Calc Relation entity
                joinString = BuidJoinRelation(EntityName, valSpecialFlds[i], parameters, aliasFields, schemaTable, resultJoin);
                if (!isSuccess)
                {
                    if (!string.IsNullOrEmpty(joinString))
                    {
                        if (!resultJoin.Contains(joinString))
                        {
                            resultJoin += joinString;
                            isSuccess = true;
                        }
                    }
                }

                // Calc Reference entity
                if (!isSuccess)
                {
                    joinString = BuidJoinReference(EntityName, valSpecialFlds[i], aliasFields, schemaTable, resultJoin);
                    if (!string.IsNullOrEmpty(joinString))
                    {
                        if (!resultJoin.Contains(joinString))
                        {
                            resultJoin += joinString;
                            isSuccess = true;
                        }
                    }
                }
               
            }

            resJoinBaseTable += resultJoin;
            var DbVal = aliasFields.Find(v => v.EntityName == entityMetadata.Name);
            return  string.Format(" {0}.{1}  {2} ", schemaTable,  entityMetadata.DataSource.Name, this._syntax.EncodeFieldName((DbVal != null ? DbVal.Alias : throw new Exception(string.Format(Exceptions.NotFoundAlias, entityMetadata.Name))))) + resJoinBaseTable;
        }

        /// <summary>
        /// Добавление данных о поле в кєш
        /// </summary>
        /// <param name="fieldMetadataColumn"></param>
        /// <param name="entityMetadata"></param>
        /// <param name="index"></param>
        /// <param name="nextNames"></param>
        /// <param name="fieldSourceTypeFilter"></param>
        /// <returns></returns>
        private bool AddNewFieldProperties(IFieldMetadata fieldMetadataColumn, IEntityMetadata entityMetadata, int index, string[] nextNames, FieldSourceType fieldSourceTypeFilter, ref List<FieldProperties> listFieldProperties)
        {
            const string ExtendedName = "EXTENDED";
            bool isAdded = false;
            if (fieldMetadataColumn != null)
            {
                var sourceName = fieldMetadataColumn.SourceName;
                var name = fieldMetadataColumn.Name;
                if (fieldMetadataColumn is FieldMetadata)
                {
                    var alias = new StringBuilder();
                    for (int t = 0; t < index - 1; t++)
                    {
                        alias.Append(nextNames[t]).Append(".");
                    }
                    var aliasValue = (alias.Append(ExtendedName).Append(".")).ToString() + name;
                    if (listFieldProperties.Find(b => (b.Alias == aliasValue && b.SourceType== fieldSourceTypeFilter) || (b.DBFieldName == sourceName && b.DBTableName == entityMetadata.DataSource.Name)) == null)
                    {
                        listFieldProperties.Add(new FieldProperties() { Alias = aliasValue, DBFieldName = sourceName, DBTableName = entityMetadata.DataSource.Name, FieldName = name, TableName = entityMetadata.Name, SourceType = fieldSourceTypeFilter });
                        isAdded = true;
                    }
                    else
                    {
                        isAdded = true;
                    }
                }
            }
            return isAdded;
        }

        /// <summary>
        /// Добавление данных о поле в кєш
        /// </summary>
        /// <param name="fieldMetadataColumn"></param>
        /// <param name="entityMetadata"></param>
        /// <param name="NameField"></param>
        /// <param name="fieldSourceTypeFilter"></param>
        private bool AddNewFieldProperties(IFieldMetadata fieldMetadataColumn, IEntityMetadata entityMetadata, string NameField, FieldSourceType  fieldSourceTypeFilter, ref List<IEntityMetadata> listEntityMetaDataLinks, ref List<FieldProperties> listFieldProperties)
        {
            bool isSuccess = false;
            if (fieldMetadataColumn != null)
            {
                string sourceName = fieldMetadataColumn.SourceName;
                string name = fieldMetadataColumn.Name;
                if (fieldMetadataColumn is FieldMetadata)
                {
                    if (listFieldProperties.Find(b => (b.Alias == NameField && b.SourceType == fieldSourceTypeFilter) || (b.DBFieldName== sourceName && b.DBTableName== entityMetadata.DataSource.Name)) == null)
                    {
                        listFieldProperties.Add(new FieldProperties() { Alias = NameField, DBFieldName = sourceName, DBTableName = entityMetadata.DataSource.Name, FieldName = name, TableName = entityMetadata.Name, SourceType = fieldSourceTypeFilter, entityMetadataLinks = listEntityMetaDataLinks });
                        isSuccess = true;
                    }
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// Поиск полей для добавления в кєш
        /// </summary>
        /// <param name="entityMetadata"></param>
        /// <param name="oldentityMetadata"></param>
        /// <param name="index"></param>
        /// <param name="nextNames"></param>
        /// <param name="fieldSourceTypeFilter"></param>
        /// <returns></returns>
        private bool ScanFields(ref IEntityMetadata entityMetadata, IEntityMetadata oldentityMetadata, int index, string[] nextNames, FieldSourceType fieldSourceTypeFilter, ref List<FieldProperties> listFieldProperties)
        {
            bool isAddedField = false;
            for (int l = 0; l < entityMetadata.Fields.Count; l++)
            {
                var valuesFieldMetaData = entityMetadata.Fields.Values;
                if ((valuesFieldMetaData.ElementAt(l).SourceType == FieldSourceType.Column) && ((fieldSourceTypeFilter == FieldSourceType.Column) || (fieldSourceTypeFilter == FieldSourceType.All)))
                {
                    IFieldMetadata fieldMetadataColumn = valuesFieldMetaData.ElementAt(l);
                    if (fieldMetadataColumn != null)
                    {
                        if (AddNewFieldProperties(fieldMetadataColumn, entityMetadata, index, nextNames, valuesFieldMetaData.ElementAt(l).SourceType, ref listFieldProperties))
                        {
                            isAddedField = true;
                        }
                    }
                }
                else if ((valuesFieldMetaData.ElementAt(l).SourceType == FieldSourceType.Extension) && ((fieldSourceTypeFilter == FieldSourceType.Extension) || (fieldSourceTypeFilter == FieldSourceType.All)))
                {
                    var extensionMetaData = ((ExtensionFieldMetadata)(valuesFieldMetaData.ElementAt(l))).ExtensionEntity;
                    if (extensionMetaData != null)
                    {
                        entityMetadata = extensionMetaData;
                        var fieldMetadataColumn = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[index]);
                        if (fieldMetadataColumn != null)
                        {
                            if (AddNewFieldProperties(fieldMetadataColumn, entityMetadata, index, nextNames, valuesFieldMetaData.ElementAt(l).SourceType, ref listFieldProperties))
                            {
                                isAddedField = true;
                            }
                        }
                        else if ((fieldMetadataColumn == null) && (index == nextNames.Length - 1))
                        {
                            for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                            {
                                if ((valuesFieldMetaData.ElementAt(lx).SourceType == FieldSourceType.Extension) && ((fieldSourceTypeFilter == FieldSourceType.Extension) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                {
                                    fieldMetadataColumn = valuesFieldMetaData.ElementAt(lx);
                                    if (fieldMetadataColumn != null)
                                    {
                                        if (AddNewFieldProperties(fieldMetadataColumn, entityMetadata, index, nextNames, valuesFieldMetaData.ElementAt(lx).SourceType, ref listFieldProperties))
                                        {
                                            isAddedField = true;
                                        }
                                    }
                                }
                            }
                            entityMetadata = oldentityMetadata;
                        }
                    }
                }
                else if ((valuesFieldMetaData.ElementAt(l).SourceType == FieldSourceType.Reference) && ((fieldSourceTypeFilter == FieldSourceType.Reference) || (fieldSourceTypeFilter == FieldSourceType.All)))
                {
                    var refMetaData = ((ReferenceFieldMetadata)(valuesFieldMetaData.ElementAt(l))).RefEntity;
                    if (refMetaData != null)
                    {
                        if (refMetaData.Name != null)
                        {
                            entityMetadata = refMetaData;
                            var fieldMetadataFindRef = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[index]);
                            if ((fieldMetadataFindRef == null) && (index == nextNames.Length - 1))
                            {
                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                {
                                    var fieldMetadataColumn = valuesFieldMetaData.ElementAt(lx);
                                    if (fieldMetadataColumn != null)
                                    {
                                        if (AddNewFieldProperties(fieldMetadataColumn, entityMetadata, index, nextNames, valuesFieldMetaData.ElementAt(l).SourceType, ref listFieldProperties))
                                        {
                                            isAddedField = true;
                                        }
                                    }
                                }
                                entityMetadata = oldentityMetadata;
                            }
                        }
                    }
                }
                else if ((valuesFieldMetaData.ElementAt(l).SourceType == FieldSourceType.Relation) && ((fieldSourceTypeFilter == FieldSourceType.Relation) || (fieldSourceTypeFilter == FieldSourceType.All)))
                {
                    var relMetaData = ((RelationFieldMetadata)(valuesFieldMetaData.ElementAt(l))).RelatedEntity;
                    if (relMetaData != null)
                    {
                        if (relMetaData.Name != null)
                        {
                            entityMetadata = relMetaData;
                            var fieldMetadataFindRel = relMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[index]);
                            if ((fieldMetadataFindRel == null) && (index == nextNames.Length - 1))
                            {
                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                {
                                    var fieldMetadataColumn = valuesFieldMetaData.ElementAt(lx);
                                    if (fieldMetadataColumn != null)
                                    {
                                        if (AddNewFieldProperties(fieldMetadataColumn, entityMetadata, index, nextNames, valuesFieldMetaData.ElementAt(l).SourceType, ref listFieldProperties))
                                        {
                                            isAddedField = true;
                                        }
                                    }
                                }
                                entityMetadata = oldentityMetadata;
                            }
                        }
                    }
                }
            }
            return isAddedField;
        }


        /// <summary>
        /// Поиск полей для добавления в кєш
        /// </summary>
        /// <param name="refMetaData"></param>
        /// <param name="fieldMetadata"></param>
        /// <param name="entityMetadata"></param>
        /// <param name="oldentityMetadata"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="fields"></param>
        /// <param name="nextNames"></param>
        /// <param name="isPrimaryKeyOnly"></param>
        /// <returns></returns>
        private bool RescanFieldsManul(IEntityMetadata refMetaData,  IFieldMetadata fieldMetadata, ref IEntityMetadata entityMetadata, IEntityMetadata oldentityMetadata, int i, int j, IEnumerable<string> fieldsValue, string[] nextNames,  bool isPrimaryKeyOnly, ref List<IEntityMetadata> listEntityMetaDataLinks, ref List<FieldProperties> listFieldProperties)
        {
            bool isAddedField = false;
            var fields = new List<string>(fieldsValue);
            if (refMetaData != null)
            {
                if (refMetaData.Name != null)
                {
                    entityMetadata = refMetaData;
                    var fieldMetadataFind = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                    if (fieldMetadataFind != null)
                    {
                        if ((fieldMetadataFind is FieldMetadata) || (fieldMetadataFind is RelationFieldMetadata) || (fieldMetadataFind is ExtensionFieldMetadata) || (fieldMetadataFind is ReferenceFieldMetadata))
                        {
                            if (isPrimaryKeyOnly)
                            {
                                if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == fieldMetadataFind.Name).Value == null)
                                {
                                    return false;
                                }
                            }
                            if (AddNewFieldProperties(fieldMetadataFind, entityMetadata, fields[i], fieldMetadata.SourceType, ref listEntityMetaDataLinks, ref listFieldProperties))
                            {
                                isAddedField = true;
                            }
                        }
                    }
                    else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                    {
                        for (int l = 0; l < entityMetadata.Fields.Count; l++)
                        {
                            var SourceField = entityMetadata.Fields.Values.ElementAt(l);
                            if ((SourceField is FieldMetadata) || (SourceField is RelationFieldMetadata) || (SourceField is ExtensionFieldMetadata) || (SourceField is ReferenceFieldMetadata))
                            {
                                if (isPrimaryKeyOnly)
                                {
                                    if (entityMetadata.PrimaryKey.FieldRefs.ToList().Find(v => v.Key == SourceField.Name).Value == null)
                                    {
                                        continue;
                                    }
                                }
                                if (AddNewFieldProperties(SourceField, entityMetadata, fields[i] + "." + SourceField.Name, fieldMetadata.SourceType, ref listEntityMetaDataLinks, ref listFieldProperties))
                                {
                                    isAddedField = true;
                                }
                            }
                        }
                        entityMetadata = oldentityMetadata;
                    }
                 

                    if (isAddedField == false)
                    {
                        if (listEntityMetaDataLinks.Count == 0)
                        {
                            listEntityMetaDataLinks.Add(oldentityMetadata);
                        }
                        var tempValue = entityMetadata;
                        if (listEntityMetaDataLinks.Find(x => x.Name == tempValue.Name) == null)
                        {
                            listEntityMetaDataLinks.Add(entityMetadata);
                        }
                    }

                }
            }
            return isAddedField;
        }

        /// <summary>
        /// Поиск полей для добавления в кєш
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="EntityName"></param>
        /// <param name="fields"></param>
        /// <param name="fieldSourceTypeFilter"></param>
        /// <param name="isPrimaryKeyOnly"></param>
        private List<FieldProperties> BuildSelectStatement(string EntityName, IEnumerable<string> fieldsValue, FieldSourceType fieldSourceTypeFilter = FieldSourceType.All, bool isPrimaryKeyOnly = false)
        {
            var listFieldProperties = new List<FieldProperties>();
            try
            {
                const string ExtendedName = "EXTENDED";
                var entityMetadata = _entityMetadata.GetEntityMetadata(EntityName);
                var oldentityMetadata = entityMetadata;
                var fields = new List<string>(fieldsValue);
                for (int i = 0; i < fields.Count; i++)
                {
                    var listEntityMetaDataLinks = new List<IEntityMetadata>();
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
                                var entityDataFields = entityMetadata.Fields.Values.ToList();
                                var fieldMetadataFind = entityDataFields.Find(u => u.Name == nextNames[j]);
                                if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                {
                                    var fieldMetadataFindx1 = entityDataFields.Find(u => u.Name == nextNames[j]);
                                    if (fieldMetadataFindx1 == null)
                                    {
                                        bool isAddedField = false;
                                        var oldEntityMetadataPrev = entityMetadata;
                                        entityMetadata = entityMetadata.BaseEntity;
                                        if (entityMetadata != null)
                                        {
                                            var fieldMetadataFindx = entityDataFields.Find(u => u.Name == nextNames[j]);
                                            if (fieldMetadataFindx == null)
                                            {
                                                if (ScanFields(ref entityMetadata, oldentityMetadata, j, nextNames, fieldSourceTypeFilter,ref listFieldProperties))
                                                {
                                                    isAddedField = true;
                                                }
                                            }
                                            else
                                            {
                                                if (fieldMetadataFindx.SourceType == FieldSourceType.Column && ((fieldSourceTypeFilter == FieldSourceType.Column) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                                {
                                                    if (AddNewFieldProperties(fieldMetadataFindx, entityMetadata, j, nextNames, fieldMetadataFindx.SourceType, ref listFieldProperties))
                                                    {
                                                        isAddedField = true;
                                                    }
                                                }
                                            }
                                            if ((LastObject == true) || (isAddedField == false))
                                            {
                                                entityMetadata = oldEntityMetadataPrev;
                                                if (ScanFields(ref entityMetadata, oldentityMetadata, j, nextNames, fieldSourceTypeFilter, ref listFieldProperties))
                                                {
                                                    isAddedField = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            entityMetadata = oldEntityMetadataPrev;
                                            if (ScanFields(ref entityMetadata, oldentityMetadata, j, nextNames, fieldSourceTypeFilter, ref listFieldProperties))
                                            {
                                                isAddedField = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if ((fieldMetadataFindx1.SourceType == FieldSourceType.Column) && ((fieldSourceTypeFilter == FieldSourceType.Column) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                        {
                                            AddNewFieldProperties(fieldMetadataFindx1, entityMetadata, j, nextNames, fieldMetadataFindx1.SourceType, ref listFieldProperties);
                                        }
                                    }
                                    entityMetadata = oldentityMetadata;
                                }
                            }
                        }
                        if (entityMetadata != null)
                        {
                            if (nextNames[j] == ExtendedName)
                            {
                                if ((j + 1) < nextNames.Length)
                                {
                                    j++;
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
                                    AddNewFieldProperties(fieldName.Value, entityMetadata, fields[i], fieldName.Value.SourceType, ref listEntityMetaDataLinks, ref listFieldProperties);
                                    entityMetadata = oldentityMetadata;
                                }
                                else if ((fieldName.Value.SourceType == FieldSourceType.Extension) && ((fieldSourceTypeFilter == FieldSourceType.Extension) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                {
                                    var extensionMetaData = ((ExtensionFieldMetadata)(fieldName.Value)).ExtensionEntity;
                                    IFieldMetadata fieldMetadata = fieldName.Value;
                                    RescanFieldsManul(extensionMetaData, fieldMetadata, ref entityMetadata, oldentityMetadata, i, j, fields, nextNames, isPrimaryKeyOnly, ref listEntityMetaDataLinks, ref listFieldProperties);
                                }
                                else if ((fieldName.Value.SourceType == FieldSourceType.Reference) && ((fieldSourceTypeFilter == FieldSourceType.Reference) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                {
                                    IEntityMetadata refMetaData = ((ReferenceFieldMetadata)(fieldName.Value)).RefEntity;
                                    IFieldMetadata fieldMetadata = fieldName.Value;
                                    RescanFieldsManul(refMetaData, fieldMetadata, ref entityMetadata, oldentityMetadata, i, j, fields, nextNames, isPrimaryKeyOnly, ref listEntityMetaDataLinks, ref listFieldProperties);
                                }
                                else if ((fieldName.Value.SourceType == FieldSourceType.Relation) && ((fieldSourceTypeFilter == FieldSourceType.Relation) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                {
                                    var relatedMetaData = ((RelationFieldMetadata)(fieldName.Value)).RelatedEntity;
                                    IFieldMetadata fieldMetadata = fieldName.Value;
                                    RescanFieldsManul(relatedMetaData, fieldMetadata, ref entityMetadata, oldentityMetadata, i, j, fields, nextNames, isPrimaryKeyOnly, ref listEntityMetaDataLinks, ref listFieldProperties);
                                }
                                else if ((fieldName.Value.SourceType == FieldSourceType.Expression) && ((fieldSourceTypeFilter == FieldSourceType.Expression) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                {
                                    throw new NotImplementedException(Exceptions.HandlerTypeExpressionNotSupported);
                                }
                            }
                            else
                            {
                                var oldEntityMetadataPrev = entityMetadata;
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
                                            AddNewFieldProperties(fieldName.Value, entityMetadata, fields[i], fieldName.Value.SourceType, ref listEntityMetaDataLinks, ref listFieldProperties);
                                            entityMetadata = oldentityMetadata;
                                        }
                                        else if ((fieldName.Value.SourceType == FieldSourceType.Extension) && ((fieldSourceTypeFilter == FieldSourceType.Extension) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                        {
                                            var extensionMetaData = ((ExtensionFieldMetadata)(fieldName.Value)).ExtensionEntity;
                                            IFieldMetadata fieldMetadata = fieldName.Value;
                                            RescanFieldsManul(extensionMetaData, fieldMetadata, ref entityMetadata, oldentityMetadata, i, j, fields, nextNames, isPrimaryKeyOnly, ref listEntityMetaDataLinks, ref listFieldProperties);
                                        }
                                        else if ((fieldName.Value.SourceType == FieldSourceType.Reference) && ((fieldSourceTypeFilter == FieldSourceType.Reference) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                        {
                                            IEntityMetadata refMetaData = ((ReferenceFieldMetadata)(fieldName.Value)).RefEntity;
                                            IFieldMetadata fieldMetadata = fieldName.Value;
                                            RescanFieldsManul(refMetaData, fieldMetadata, ref entityMetadata, oldentityMetadata, i, j, fields, nextNames, isPrimaryKeyOnly, ref listEntityMetaDataLinks, ref listFieldProperties);
                                        }
                                        else if ((fieldName.Value.SourceType == FieldSourceType.Relation) && ((fieldSourceTypeFilter == FieldSourceType.Relation) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                        {
                                            var relatedMetaData = ((RelationFieldMetadata)(fieldName.Value)).RelatedEntity;
                                            IFieldMetadata fieldMetadata = fieldName.Value;
                                            RescanFieldsManul(relatedMetaData, fieldMetadata, ref entityMetadata, oldentityMetadata, i, j, fields, nextNames, isPrimaryKeyOnly, ref listEntityMetaDataLinks, ref listFieldProperties);

                                        }
                                        else if ((fieldName.Value.SourceType == FieldSourceType.Expression) && ((fieldSourceTypeFilter == FieldSourceType.Expression) || (fieldSourceTypeFilter == FieldSourceType.All)))
                                        {
                                            throw new NotImplementedException(Exceptions.HandlerTypeExpressionNotSupported);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
            return listFieldProperties;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string BuildSelectStatement(QuerySelectStatement statement, IDictionary<string, EngineCommandParameter> parameters)
        {
            try
            {
                var selectColumns = statement.Table.SelectColumns;
                var selectedColumns = statement.Table.Columns.Values.ToArray();
                var listAlias = new List<AliasField>();
                var listFieldProperties = new List<FieldProperties>();
                var fromExpression = BuildJoinStatement(statement.Table.Name, selectedColumns.Select(t => t.Name), parameters, out listAlias, ref listFieldProperties);
                for (int i = 0; i < selectedColumns.Length; i++)
                {
                    var dbField = listFieldProperties.Find(z => z.Alias == selectedColumns[i].Name);
                    if (dbField != null)
                    {
                        var column = selectedColumns[i];
                        AliasField aliasField = listAlias.Find(z => z.DBTableName == dbField.DBTableName);
                        if (aliasField != null)
                        {
                            column.Alias = dbField.Alias;
                        }
                    }
                }
                var conditionsColumns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Conditions, conditionsColumns);
                var whereColumns = conditionsColumns.ToArray();
                var sortColumns = statement.Orders == null ? new QuerySelectStatement.OrderByColumnDescriptor[] { } : statement.Orders.ToArray();
                var fieldCount = whereColumns.Length + sortColumns.Length;
                listFieldProperties.AddRange(BuildSelectStatement(statement.Table.Name, sortColumns.Select(t => t.Column.Name)));
                listFieldProperties.AddRange(BuildSelectStatement(statement.Table.Name, selectedColumns.Select(t => t.Name)));
                var columnExpressions = new List<string>();
                for (int i = 0; i < selectedColumns.Length; i++)
                {
                    var dbField = listFieldProperties.Find(z => z.Alias == selectedColumns[i].Name);
                    if (dbField != null)
                    {
                        AliasField aliasField = listAlias.Find(z => z.DBTableName == dbField.DBTableName);
                        if (aliasField != null)
                        {
                            if (selectColumns.ContainsKey(dbField.Alias))
                            {
                                columnExpressions.Add(this._syntax.ColumnExpression(this._syntax.EncodeFieldName(aliasField.Alias) + "." + this._syntax.EncodeFieldName(dbField.DBFieldName), dbField.Alias));
                            }
                        }
                    }
                }
                // to build the where section
                for (int i = 0; i < whereColumns.Length; i++)
                {
                    var column = whereColumns[i];
                    var dbField = listFieldProperties.Find(z => z.Alias == whereColumns[i].ColumnName);
                    if (dbField != null)
                    {
                        AliasField aliasField = listAlias.Find(z => z.DBTableName == dbField.DBTableName);
                        if (aliasField != null)
                        {
                            column.ColumnName = dbField.DBFieldName;
                            if (this._dataEngine.Config.Type == DataEngineType.Oracle)
                            {
                                column.Source = this._syntax.EncodeFieldName(aliasField.Alias);
                            }
                            else
                            {
                                column.Source = aliasField.Alias;
                            }
                        }
                    }
                }

                var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);
                // to build the order by section
                var orderByColumns = new string[sortColumns.Length];
                for (int i = 0; i < sortColumns.Length; i++)
                {
                    var column = sortColumns[i];
                    var dbField = listFieldProperties.Find(z => z.Alias == sortColumns[i].Column.Name);
                    var encodeColumn = "";
                    encodeColumn = this._syntax.EncodeFieldName(dbField.DBFieldName);
                    AliasField aliasField = listAlias.Find(z => z.DBTableName == dbField.DBTableName);
                    if (aliasField != null)
                    {
                        //column.Column.Name = dbField.DBFieldName;
                        encodeColumn = this._syntax.EncodeFieldName(aliasField.Alias) + "." + encodeColumn;
                        orderByColumns[i] = _syntax.SortedColumn(encodeColumn, column.Direction);
                    }
                }
                // add on top (n)
                var limit = statement.Limit;

                // add group by
                var selectStatement = this._syntax.SelectExpression(columnExpressions.ToArray(), fromExpression, whereExpression, orderByColumns, limit);
                return selectStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string BuildSelectStatementWithAllocId(QuerySelectStatement statement, IDictionary<string, EngineCommandParameter> parameters)
        {
            try
            {
                var selectColumns = statement.Table.SelectColumns;
                var selectedColumns = statement.Table.Columns.Values.ToArray();
                var listAlias = new List<AliasField>();
                var listFieldProperties = new List<FieldProperties>();
                var tableName = "";
                var fromExpression = BuildJoinStatement(statement.Table.Name, selectedColumns.Select(t => t.Name), parameters, out listAlias, ref listFieldProperties);
                for (int i = 0; i < selectedColumns.Length; i++)
                {
                    var dbField = listFieldProperties.Find(z => z.Alias == selectedColumns[i].Name);
                    if (dbField != null)
                    {
                        var column = selectedColumns[i];
                        AliasField aliasField = listAlias.Find(z => z.DBTableName == dbField.DBTableName);
                        if (aliasField != null)
                        {
                            column.Alias = dbField.Alias;
                            tableName = dbField.DBTableName;
                        }
                    }
                }
                var conditionsColumns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Conditions, conditionsColumns);
                var whereColumns = conditionsColumns.ToArray();
                var sortColumns = statement.Orders == null ? new QuerySelectStatement.OrderByColumnDescriptor[] { } : statement.Orders.ToArray();
                var fieldCount = whereColumns.Length + sortColumns.Length;
                listFieldProperties.AddRange(BuildSelectStatement(statement.Table.Name, sortColumns.Select(t => t.Column.Name)));
                listFieldProperties.AddRange(BuildSelectStatement(statement.Table.Name, selectedColumns.Select(t => t.Name)));
                var columnExpressions = new List<string>();
                if (this._dataEngine.Config.Type == DataEngineType.Oracle)
                {
                    columnExpressions.Add($"GetID('{tableName}')");
                }
                for (int i = 0; i < selectedColumns.Length; i++)
                {
                    var dbField = listFieldProperties.Find(z => z.Alias == selectedColumns[i].Name);
                    if (dbField != null)
                    {
                        AliasField aliasField = listAlias.Find(z => z.DBTableName == dbField.DBTableName);
                        if (aliasField != null)
                        {
                            if (selectColumns.ContainsKey(dbField.FieldName))
                            {
                                columnExpressions.Add(this._syntax.ColumnExpression(this._syntax.EncodeFieldName(aliasField.Alias) + "." + this._syntax.EncodeFieldName(dbField.DBFieldName), dbField.Alias));
                            }
                        }
                    }
                }
                // to build the where section
                for (int i = 0; i < whereColumns.Length; i++)
                {
                    var column = whereColumns[i];
                    var dbField = listFieldProperties.Find(z => z.Alias == whereColumns[i].ColumnName);
                    if (dbField != null)
                    {
                        AliasField aliasField = listAlias.Find(z => z.DBTableName == dbField.DBTableName);
                        if (aliasField != null)
                        {
                            column.ColumnName = dbField.DBFieldName;
                            if (this._dataEngine.Config.Type == DataEngineType.Oracle)
                            {
                                column.Source = this._syntax.EncodeFieldName(aliasField.Alias);
                            }
                            else
                            {
                                column.Source = aliasField.Alias;
                            }
                        }
                    }
                }

                var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);
                // to build the order by section
                var orderByColumns = new string[sortColumns.Length];
                for (int i = 0; i < sortColumns.Length; i++)
                {
                    var column = sortColumns[i];
                    var dbField = listFieldProperties.Find(z => z.Alias == sortColumns[i].Column.Name);
                    var encodeColumn = "";
                    encodeColumn = this._syntax.EncodeFieldName(dbField.DBFieldName);
                    AliasField aliasField = listAlias.Find(z => z.DBTableName == dbField.DBTableName);
                    if (aliasField != null)
                    {
                        //column.Column.Name = dbField.DBFieldName;
                        encodeColumn = this._syntax.EncodeFieldName(aliasField.Alias) + "." + encodeColumn;
                        orderByColumns[i] = _syntax.SortedColumn(encodeColumn, column.Direction);
                    }
                }
                // add on top (n)
                var limit = statement.Limit;

                // add group by
                var selectStatement = this._syntax.SelectExpression(columnExpressions.ToArray(), fromExpression, whereExpression, orderByColumns, limit);
                return selectStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
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

        /// <summary>
        /// Генератор SQL запросов для Delete
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string BuildDeleteStatement(QueryDeleteStatement statement, IDictionary<string, EngineCommandParameter> parameters)
         {
             try
             {
                var listFieldProperties = new List<FieldProperties>();
                var entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, entityMetadata.DataSource.Name);
                var columns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Conditions, columns);
                var columnsArray = columns.ToArray();
                var listAlias = new List<AliasField>();
                var fromStatement = BuildJoinStatement(statement.TableName, columnsArray.Select(c => c.ColumnName), parameters, out listAlias, ref listFieldProperties);
                for (int i = 0; i < columnsArray.Length; i++)
                {
                     var column = columnsArray[i];
                     var dbField = entityMetadata.Fields.Values.ToList().Find(z => z.Name == column.ColumnName);
                     column.ColumnName = dbField.SourceName;
                }

                var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);
                var deleteStatement = this._syntax.DeleteExpression(sourceExpression, fromStatement, whereExpression);
                return deleteStatement;
             }
             catch (Exception e)
             {
                 this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                 throw new InvalidOperationException(Exceptions.AbortedBuildDeleteStatement, e);
             }
         }


        /// <summary>
        /// Генератор пакетного SQL запроса Insert
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string BuildInsertStatementExecuteAndFetch(QueryInsertStatement[] statements, IDictionary<string, EngineCommandParameter> parameters)
        {
            string insertStatement = null;
            try
            {
                int cntPrimaryKey = 0;
                string columnsExpression = null;
                string valuesExpression = null;
                string primaryKeyField = null;
                string sourceExpression = null;
                var listSelectedParameters = new List<string>();
                IEntityMetadata entityMetadata = null;
                for (int j = 0; j < statements.Length; j++)
                {
                    var statement = statements[j];
                    var listAlias = new List<AliasField>();
                    if (j == 0)
                    {
                        entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                    }
                    var changedColumns = new string[statement.ColumnsValues.Count];
                    var selectedParameters = new string[statement.ColumnsValues.Count];
                    for (int i = 0; i < statement.ColumnsValues.Count; i++)
                    {
                        KeyValuePair<string, IFieldMetadata> fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == statement.ColumnsValues[i].Name);
                        if (fieldName.Value != null)
                        {
                            var column = statement.ColumnsValues[i];
                            column.Name = fieldName.Value.SourceName;
                            var columnValueReplaced = QuerySelectStatement.GetColumnValue(column.GetValue(), column.Name, fieldName.Value.DataType as DataTypeMetadata);
                            column = columnValueReplaced;
                            var parameter = new EngineCommandParameter
                            {
                                DataType = column.DataType,
                                Name = "v_" + column.Name + j.ToString(),
                                Value = column.GetValue()
                            };

                            parameters.Add(parameter.Name, parameter);
                            selectedParameters[i] = this._syntax.EncodeParameterName(parameter.Name);
                            changedColumns[i] = this._syntax.EncodeFieldName(column.Name);
                        }
                        else
                        {
                            throw new Exception(string.Format(Exceptions.NotFoundDetailInformation, statement.TableName + "." + statement.ColumnsValues[i].Name));
                        }
                    }
                    listSelectedParameters.Add(string.Join(", ", selectedParameters));

                    if (j == 0)
                    {
                        sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, entityMetadata.DataSource.Name);
                        columnsExpression = string.Join(", ", changedColumns);
                        if (entityMetadata.PrimaryKey != null)
                        {
                            var primaryKeys = entityMetadata.PrimaryKey.FieldRefs;
                            if (primaryKeys != null)
                            {
                                cntPrimaryKey = primaryKeys.Count;
                                foreach (var item in primaryKeys)
                                {
                                    primaryKeyField = item.Key;
                                    KeyValuePair<string, IFieldMetadata> fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == primaryKeyField);
                                    if (fieldName.Value != null)
                                    {
                                        primaryKeyField = fieldName.Value.SourceName;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
               
                valuesExpression = string.Join("| ", listSelectedParameters);

                if (listSelectedParameters.Count == 1) valuesExpression += "|";



                if ((primaryKeyField != null) && (cntPrimaryKey == 1))
                {
                    insertStatement = this._syntax.InsertExpression(sourceExpression, columnsExpression, valuesExpression, null, null, primaryKeyField);
                }
                return insertStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
            }
        }

        /// <summary>
        /// Генератор SQL запросов для Insert
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string BuildInsertStatement(QueryInsertStatement statement, IDictionary<string, EngineCommandParameter> parameters)
         {
            try
            {
                var listAlias = new List<AliasField>();
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
                        var columnValueReplaced = QuerySelectStatement.GetColumnValue(column.GetValue(), column.Name, fieldName.Value.DataType as DataTypeMetadata);
                        column = columnValueReplaced;
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
                    else
                    {
                        throw new Exception(string.Format(Exceptions.NotFoundDetailInformation, statement.TableName + "." + statement.ColumnsValues[i].Name));
                    }
                }
              

                var columnsExpression = string.Join(", ", changedColumns);
                var valuesExpression = string.Join(", ", selectedParameters);
                var insertStatement = this._syntax.InsertExpression(sourceExpression, columnsExpression, valuesExpression);
                return insertStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
            }
         }

        /// <summary>
        /// Генератор SQL запросов для Insert
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string BuildInsertSelectStatement(QueryInsertStatement statementInsert, QuerySelectStatement statementSelect, IDictionary<string, EngineCommandParameter> parameters)
        {
            try
            {
               var selectCommandText = BuildSelectStatementWithAllocId(statementSelect, parameters);
               var entityMetadata = _entityMetadata.GetEntityMetadata(statementInsert.TableName);
               var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, entityMetadata.DataSource.Name);
               var selectedParameters = statementSelect.Table.SelectColumns;
               var changedColumns = new List<string>();
               foreach (var item in selectedParameters)
               {
                   KeyValuePair<string, IFieldMetadata> fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == item.Value.Name);
                   if (fieldName.Value != null)
                   {
                        changedColumns.Add(fieldName.Value.SourceName);
                   }
               }
               int cntPrimaryKey = 0;
               string primaryKeyField = null;
               if (entityMetadata.PrimaryKey != null)
               {
                   var primaryKeys = entityMetadata.PrimaryKey.FieldRefs;
                   if (primaryKeys != null)
                   {
                       cntPrimaryKey = primaryKeys.Count;
                       foreach (var item in primaryKeys)
                       {
                           primaryKeyField = item.Key;
                           KeyValuePair<string, IFieldMetadata> fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == primaryKeyField);
                           if (fieldName.Value != null)
                           {
                               primaryKeyField = fieldName.Value.SourceName;
                           }
                           break;
                       }
                   }
               }

               var columnsExpression = string.Join(", ", changedColumns);
               string insertStatement = null;
               if (cntPrimaryKey == 1)
               {
                   insertStatement = this._syntax.InsertExpression(sourceExpression, columnsExpression, null, null, selectCommandText, primaryKeyField);
               }
               return insertStatement;
               
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
            }
        }

        /// <summary>
        /// Генератор SQL запросов для Insert
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string BuildInsertStatementExecuteAndFetch(QueryInsertStatement statement, IDictionary<string, EngineCommandParameter> parameters)
        {
            try
            {
                var listAlias = new List<AliasField>();
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
                        var columnValueReplaced = QuerySelectStatement.GetColumnValue(column.GetValue(), column.Name, fieldName.Value.DataType as DataTypeMetadata);
                        column = columnValueReplaced;
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
                    else
                    {
                        throw new Exception(string.Format(Exceptions.NotFoundDetailInformation, statement.TableName + "." + statement.ColumnsValues[i].Name));
                    }
                }
                int cntPrimaryKey = 0;
                string primaryKeyField = null;
                if (entityMetadata.PrimaryKey != null)
                {
                    var primaryKeys = entityMetadata.PrimaryKey.FieldRefs;
                    if (primaryKeys != null)
                    {
                        cntPrimaryKey = primaryKeys.Count;
                        foreach (var item in primaryKeys)
                        {
                            primaryKeyField = item.Key;
                            KeyValuePair<string, IFieldMetadata> fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == primaryKeyField);
                            if (fieldName.Value != null)
                            {
                                primaryKeyField = fieldName.Value.SourceName;
                            }
                            break;
                        }
                    }
                }

                var columnsExpression = string.Join(", ", changedColumns);
                var valuesExpression = string.Join(", ", selectedParameters);
                string insertStatement = null;
                if (primaryKeyField == null)
                {
                    insertStatement = this._syntax.InsertExpression(sourceExpression, columnsExpression, valuesExpression);
                }
                else
                {
                    if (cntPrimaryKey == 1)
                    {
                        insertStatement = this._syntax.InsertExpression(sourceExpression, columnsExpression, valuesExpression, null, null, primaryKeyField);
                    }
                    else
                    {
                        insertStatement = this._syntax.InsertExpression(sourceExpression, columnsExpression, valuesExpression);
                    }
                }
                return insertStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
            }
        }
        public KeyValuePair<string, DataType> GetIdentFieldFromTable(QueryInsertStatement statement, IDictionary<string, EngineCommandParameter> parameters)
        {
            try
            {
                var entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                if (entityMetadata.PrimaryKey != null)
                {
                    var primaryKeys = entityMetadata.PrimaryKey.FieldRefs;
                    if (primaryKeys != null)
                    {
                        foreach (var item in primaryKeys)
                        {
                            string name = item.Key;
                            KeyValuePair<string, IFieldMetadata> fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == name);
                            if (fieldName.Value != null)
                            {
                                return new KeyValuePair<string, DataType>(fieldName.Value.SourceName, fieldName.Value.DataType.CodeVarType);
                            }
                            break;
                        }
                    }
                }
                return default(KeyValuePair<string, DataType>);
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
            }
        }

        public string GetIdentFieldFromTable(QueryUpdateStatement statement, IDictionary<string, EngineCommandParameter> parameters)
        {
            string identFieldName = null;
            try
            {
                var entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                if (entityMetadata.PrimaryKey != null)
                {
                    var primaryKeys = entityMetadata.PrimaryKey.FieldRefs;
                    if (primaryKeys != null)
                    {
                        foreach (var item in primaryKeys)
                        {
                            identFieldName = item.Key;
                            KeyValuePair<string, IFieldMetadata> fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == identFieldName);
                            if (fieldName.Value != null)
                            {
                                identFieldName = fieldName.Value.SourceName;
                            }
                            break;
                        }
                    }
                }
                return identFieldName;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
            }
        }
        /// <summary>
        ///  Генератор SQL запросов для Update
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string BuildUpdateStatement(QueryUpdateStatement statement, IDictionary<string, EngineCommandParameter> parameters)
         {
            try
            {
                var columns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Conditions, columns);
                var columnsArray = columns.ToArray();
                IEntityMetadata entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, entityMetadata.DataSource.Name);
                for (int i = 0; i < statement.ColumnsValues.Count; i++)
                {
                    KeyValuePair<string, IFieldMetadata> fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == statement.ColumnsValues[i].Name);
                    if (fieldName.Value != null)
                    {
                        var column = statement.ColumnsValues[i];
                        column.Name = fieldName.Value.SourceName;
                        var columnValueReplaced = QuerySelectStatement.GetColumnValue(column.GetValue(), column.Name, fieldName.Value.DataType as DataTypeMetadata);
                        column = columnValueReplaced;
                    }
                  
                }
                var valuesExpression = this.BuildSetValuesExpression(statement.ColumnsValues, parameters);
                var listAlias = new List<AliasField>();
                var listFieldProperties = new List<FieldProperties>();
                var fromStatement = BuildJoinStatement(statement.TableName, columnsArray.Select(c => c.ColumnName), parameters, out listAlias, ref listFieldProperties);
                for (int i = 0; i < columnsArray.Length; i++)
                {
                    var column = columnsArray[i];
                    var dbField = entityMetadata.Fields.Values.ToList().Find(z=>z.Name== column.ColumnName);
                    column.ColumnName = dbField.SourceName;
                }
                var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);
                var updateStatement = this._syntax.UpdateExpression(sourceExpression, valuesExpression, fromStatement, whereExpression);
                return updateStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesEntity, Categories.BuildingStatement, e, this);
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
