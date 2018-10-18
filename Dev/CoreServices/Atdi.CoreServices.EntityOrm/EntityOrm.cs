using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Platform.AppComponent;
using System.Xml.Linq;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using Atdi.DataModels.DataConstraint;

namespace Atdi.CoreServices.EntityOrm
{
    public class EntityOrm : IEntityOrm
    {
        public readonly Dictionary<IRelationFieldMetadata, string> _relationFieldMetadata;
        public readonly Dictionary<IReferenceFieldMetadata, string> _referenceFieldMetadata;
        public readonly Dictionary<IExtensionFieldMetadata, string> _extensionFieldMetadata;
        public readonly Dictionary<IFieldMetadata, string> _columnFieldMetadata;
        private readonly IEntityOrmConfig _config;
        private readonly Dictionary<string, IEntityMetadata> _cache;
        private readonly List<string> _cashecontainerEntity;
        private readonly List<IEntityMetadata> _cashecontainerEntityList;


        public IReadOnlyDictionary<IRelationFieldMetadata, string> RelationFieldMetadata => this._relationFieldMetadata;

        public IReadOnlyDictionary<IReferenceFieldMetadata, string> ReferenceFieldMetadata => this._referenceFieldMetadata;

        public IReadOnlyDictionary<IExtensionFieldMetadata, string> ExtensionFieldMetadata => this._extensionFieldMetadata;

        public IReadOnlyDictionary<IFieldMetadata, string> ColumnFieldMetadata => this._columnFieldMetadata;

        public EntityOrm(IEntityOrmConfig config)
        {
            this._config = config;
            this._columnFieldMetadata = new Dictionary<IFieldMetadata, string>();
            this._extensionFieldMetadata = new Dictionary<IExtensionFieldMetadata, string>();
            this._relationFieldMetadata = new Dictionary<IRelationFieldMetadata, string>();
            this._referenceFieldMetadata = new Dictionary<IReferenceFieldMetadata, string>();
            _cache = new Dictionary<string, IEntityMetadata>();
            _cashecontainerEntity = new List<string>();
            _cashecontainerEntityList = new List<IEntityMetadata>();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTypeName"></param>
        /// <param name="dataSourceType"></param>
        /// <returns></returns>
        public IDataTypeMetadata GetDataTypeMetadata(string dataTypeName, Atdi.Contracts.CoreServices.EntityOrm.Metadata.DataSourceType dataSourceType)
        {
            var dataTypeMetadata = new DataTypeMetadata();
            if (!string.IsNullOrEmpty(_config.DataTypesPath))
            {
                bool isFinded = false;
                var di = new System.IO.DirectoryInfo(_config.DataTypesPath+ @"\" + dataSourceType.ToString());
                var list = di.GetFiles();
                for (int i = 0; i < list.Length; i++)
                {
                    var serializer = new XmlSerializer(typeof(Atdi.CoreServices.EntityOrm.Metadata.DataTypeDef));
                    var reader = new StreamReader(list[i].FullName);
                    object resDataTypeDef = serializer.Deserialize(reader);
                    if (resDataTypeDef is Atdi.CoreServices.EntityOrm.Metadata.DataTypeDef)
                    {
                        var dataTypeDef  = resDataTypeDef as Atdi.CoreServices.EntityOrm.Metadata.DataTypeDef;
                        if (dataTypeDef != null)
                        {
                            if (dataTypeDef.Name == dataTypeName)
                            {
                                var autonumMetadata = new AutonumMetadata();
                                dataTypeMetadata.Name = dataTypeDef.Name;
                                if (dataTypeDef.DataSourceType != null)
                                {
                                    dataTypeMetadata.DataSourceType = (DataSourceType)Enum.Parse(typeof(DataSourceType), dataTypeDef.DataSourceType.ToString());
                                }
                                if (dataTypeDef.Autonum != null)
                                {
                                    autonumMetadata.Start = (int)dataTypeDef.Autonum.Start;
                                    autonumMetadata.Step = (int)dataTypeDef.Autonum.Step;
                                }
                                dataTypeMetadata.Autonum = autonumMetadata;
                                if (dataTypeDef.CodeVarType != null)
                                {
                                    if (!string.IsNullOrEmpty(dataTypeDef.CodeVarType.ClrType))
                                    {
                                        dataTypeMetadata.CodeVarClrType = Type.GetType(dataTypeDef.CodeVarType.ClrType);
                                    }
                                }
                                if (dataTypeDef.CodeVarType!=null)
                                {
                                    dataTypeMetadata.CodeVarType = (DataModels.DataType)Enum.Parse(typeof(DataModels.DataType), dataTypeDef.CodeVarType.Value.ToString());
                                }
                                if (dataTypeDef.DataSourceType != null)
                                {
                                    dataTypeMetadata.DataSourceType = (DataSourceType)Enum.Parse(typeof(DataSourceType), dataTypeDef.DataSourceType.ToString());
                                }
                                if (dataTypeDef.Length!=null)
                                {
                                    dataTypeMetadata.Length = (int)dataTypeDef.Length.Value;
                                }
                                dataTypeMetadata.Multiple = dataTypeDef.Multiple;
                                if (dataTypeDef.Precision != null)
                                {
                                    if (dataTypeDef.Precision.Value != null)
                                    {
                                        if (!string.IsNullOrEmpty(dataTypeDef.Precision.Value))
                                        {
                                            dataTypeMetadata.Precision = Convert.ToInt32(dataTypeDef.Precision.Value);
                                        }
                                    }
                                }
                                if (dataTypeDef.Scale != null)
                                {
                                    if (dataTypeDef.Scale.Value != null)
                                    {
                                        if (!string.IsNullOrEmpty(dataTypeDef.Scale.Value))
                                        {
                                            dataTypeMetadata.Scale = Convert.ToInt32(dataTypeDef.Scale.Value);
                                        }
                                    }
                                }
                                if (dataTypeDef.SourceVarType != null)
                                {
                                    dataTypeMetadata.SourceVarType = (DataSourceVarType)Enum.Parse(typeof(DataSourceVarType), dataTypeDef.SourceVarType.Value.ToString());
                                }
                                isFinded = true;
                            }
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                    if (isFinded)
                    {
                        break;
                    }
                }
            }
            return dataTypeMetadata;
        }

       

        /// <summary>
        /// Добавление сведений об "отсутствующих" первичных ключах из расширяемой сущности в сущность расширения
        /// </summary>
        /// <param name="entityMetadata"></param>
        private void AddedMissingPrimaryKeyExtension(IEntityMetadata entityMetadata)
        {
            if (entityMetadata.PrimaryKey != null)
            {
                if (entityMetadata.ExtendEntity != null)
                {
                    if (entityMetadata.ExtendEntity.PrimaryKey != null)
                    {
                        (entityMetadata.PrimaryKey as PrimaryKeyMetadata).FieldRefs = (entityMetadata.ExtendEntity.PrimaryKey as PrimaryKeyMetadata).FieldRefs;
                    }
                }
                
                if (entityMetadata.Fields != null)
                {
                    var dicMetaData = new Dictionary<string, IFieldMetadata>();
                    foreach (var x in entityMetadata.Fields.Values)
                    {
                        if (x is ExtensionFieldMetadata)
                        {
                            if ((x as ExtensionFieldMetadata).ExtensionEntity != null)
                            {
                                ((x as ExtensionFieldMetadata).ExtensionEntity.PrimaryKey as PrimaryKeyMetadata).FieldRefs = entityMetadata.PrimaryKey.FieldRefs;
                            }
                        }
                    }
                }
            }
        }
       
    
        private bool AddMissingPropertyValues(Type type, object source, object destination)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var readableNonIndexers = properties.Where(p => p.CanWrite && p.GetIndexParameters().Length == 0);
            foreach (var propertyInfo in readableNonIndexers)
            {
                var a = propertyInfo.GetValue(source, null);
                var b = propertyInfo.GetValue(destination, null);
                if ((b == null) || (!b.Equals(a)))
                {
                    if (a != null)
                    {
                        propertyInfo.SetValue(destination, a);
                    }
                }
            }
            return true;
        }


      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitName"></param>
        /// <returns></returns>
        public IUnitMetadata GetUnitMetadata(string unitName)
        {
            var unitMetadata = new UnitMetadata();
            if (!string.IsNullOrEmpty(_config.UnitsPath))
            {
                bool isFinded = false;
                var di = new System.IO.DirectoryInfo(_config.UnitsPath);
                var list = di.GetFiles();
                for (int i = 0; i < list.Length; i++)
                {
                    var serializer = new XmlSerializer(typeof(Atdi.CoreServices.EntityOrm.Metadata.UnitDef));
                    var reader = new StreamReader(list[i].FullName);
                    object resEntity = serializer.Deserialize(reader);
                    if (resEntity is Atdi.CoreServices.EntityOrm.Metadata.UnitDef)
                    {
                        var unitObject = resEntity as Atdi.CoreServices.EntityOrm.Metadata.UnitDef;
                        if (unitObject != null)
                        {
                            if (unitObject.Name == unitName)
                            {
                                unitMetadata.Name = unitObject.Name;
                                unitMetadata.Dimension = unitObject.Dimension.Value;
                                unitMetadata.Category = unitObject.Category.Value;
                                isFinded = true;
                            }
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                    if (isFinded)
                    {
                        break;
                    }
                }
            }
            return unitMetadata;
        }


        public IEntityMetadata GetEntityMetadata(string entityName)
        {
            var entityMetadata = new EntityMetadata();
            if (_cache.ContainsKey(entityName))
            {
                return _cache[entityName];
            }
            if (!string.IsNullOrEmpty(_config.EntitiesPath))
            {
                bool isFinded = false;
                var di = new System.IO.DirectoryInfo(_config.EntitiesPath);
                var list = di.GetFiles();
                for (int i = 0; i < list.Length; i++)
                {
                    var serializer = new XmlSerializer(typeof(Atdi.CoreServices.EntityOrm.Metadata.EntityDef));
                    var reader = new StreamReader(list[i].FullName);
                    object resEntity = serializer.Deserialize(reader);
                    if (resEntity is Atdi.CoreServices.EntityOrm.Metadata.EntityDef)
                    {
                        var entityObject = resEntity as Atdi.CoreServices.EntityOrm.Metadata.EntityDef;
                        if (entityObject != null)
                        {
                            if (entityObject.Name == entityName)
                            {
                                if (!string.IsNullOrEmpty(entityObject.BaseEntity))
                                {
                                    if (!_cashecontainerEntity.Contains(entityObject.BaseEntity))
                                    {
                                        _cashecontainerEntity.Add(entityObject.BaseEntity);
                                        entityMetadata.BaseEntity = GetEntityMetadata(entityObject.BaseEntity);
                                        _cashecontainerEntityList.Add(entityMetadata.BaseEntity);
                                    }
                                    else
                                    {
                                        entityMetadata.BaseEntity = _cashecontainerEntityList.Find(t => t.Name == entityObject.BaseEntity);
                                    }
                                }
                                if (!string.IsNullOrEmpty(entityObject.ExtendEntity))
                                {
                                    if (!_cashecontainerEntity.Contains(entityObject.ExtendEntity))
                                    {
                                        _cashecontainerEntity.Add(entityObject.ExtendEntity);
                                        entityMetadata.ExtendEntity = GetEntityMetadata(entityObject.ExtendEntity);
                                        _cashecontainerEntityList.Add(entityMetadata.ExtendEntity);
                                    }
                                    else
                                    {
                                        entityMetadata.ExtendEntity = _cashecontainerEntityList.Find(t => t.Name == entityObject.ExtendEntity);
                                    }
                                }
                                var dataSourceMetadata = new DataSourceMetadata();
                                dataSourceMetadata.Name = entityObject.DataSource.Name;
                                dataSourceMetadata.Schema = entityObject.DataSource.Schema;
                                dataSourceMetadata.Object = (DataSourceObject)Enum.Parse(typeof(DataSourceObject), entityObject.DataSource.Object.ToString());
                                dataSourceMetadata.Type = (DataSourceType)Enum.Parse(typeof(DataSourceType), entityObject.DataSource.Type.ToString());
                                entityMetadata.DataSource = dataSourceMetadata;
                                var primaryKeyMetadata = new PrimaryKeyMetadata();
                                var dictionaryFields = new Dictionary<string, IFieldMetadata>();
                                entityMetadata.Desc = entityObject.Desc;


                                var fieldDefs = entityObject.Fields;
                                foreach (var fieldDef in fieldDefs)
                                {
                                    if (fieldDef.SourceType == Metadata.FieldSourceType.Column)
                                    {
                                        var fieldMetadata = new FieldMetadata();
                                        if (fieldDef.DataType != null)
                                        {
                                            fieldMetadata.DataType = GetDataTypeMetadata(fieldDef.DataType, dataSourceMetadata.Type);
                                        }

                                        fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                                        fieldMetadata.Title = fieldDef.Title;
                                        fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                                        fieldMetadata.SourceName = fieldDef.SourceName;
                                        fieldMetadata.Required = fieldDef.Required;
                                        fieldMetadata.Name = fieldDef.Name;
                                        fieldMetadata.Desc = fieldDef.Desc;
                                        dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);

                                        if (this._columnFieldMetadata.ToList().Find(c => c.Key.Name == fieldMetadata.Name).Key == null)
                                        {
                                            this._columnFieldMetadata.Add(fieldMetadata, dataSourceMetadata.Name);
                                        }
                                        else
                                        {
                                            this._columnFieldMetadata.Remove(this._columnFieldMetadata.ToList().Find(c => c.Key.Name == fieldMetadata.Name).Key);
                                            this._columnFieldMetadata.Add(fieldMetadata, dataSourceMetadata.Name);
                                        }
                                    }
                                    else if (fieldDef.SourceType == Metadata.FieldSourceType.Reference)
                                    {
                                        var fieldMetadata = new ReferenceFieldMetadata();
                                        if (fieldDef.DataType != null)
                                        {
                                            fieldMetadata.DataType = GetDataTypeMetadata(fieldDef.DataType, dataSourceMetadata.Type);
                                        }

                                        if (!string.IsNullOrEmpty(fieldDef.SourceName))
                                        {
                                            if (!_cashecontainerEntity.Contains(fieldDef.SourceName))
                                            {
                                                _cashecontainerEntity.Add(fieldDef.SourceName);
                                                fieldMetadata.RefEntity = GetEntityMetadata(fieldDef.SourceName);
                                                _cashecontainerEntityList.Add(fieldMetadata.RefEntity);

                                                if (_cache.ContainsKey(fieldDef.SourceName))
                                                {
                                                    _cache.Remove(fieldDef.SourceName);
                                                    _cache.Add(fieldDef.SourceName, fieldMetadata.RefEntity);
                                                }

                                            }
                                            else
                                            {
                                                fieldMetadata.RefEntity = _cashecontainerEntityList.Find(t => t.Name == fieldDef.SourceName);
                                            }
                                        }
                                        if ((fieldMetadata.RefEntity != null) && (fieldDef.PrimaryKeyMapping != null))
                                        {
                                            var primaryKeyFieldMappingMetadata = new PrimaryKeyMappingMetadata();
                                            Dictionary<string, IPrimaryKeyFieldMappedMetadata> dictionary = new Dictionary<string, IPrimaryKeyFieldMappedMetadata>();
                                            foreach (var ch in fieldDef.PrimaryKeyMapping)
                                            {
                                                if (fieldMetadata.RefEntity.Fields != null)
                                                {
                                                    if (ch.MatchWith == Metadata.PrimaryKeyMappedMatchWith.Value)
                                                    {
                                                        var valueprimaryKeyFieldMappedMetadata = new ValuePrimaryKeyFieldMappedMetadata();
                                                        IFieldMetadata fieldMetadataf = new FieldMetadata();
                                                        if (fieldMetadata.RefEntity.Fields.TryGetValue(ch.KeyFieldName, out fieldMetadataf))
                                                        {
                                                            valueprimaryKeyFieldMappedMetadata.KeyField = fieldMetadataf;
                                                            (valueprimaryKeyFieldMappedMetadata as ValuePrimaryKeyFieldMappedMetadata).Value = ch.Value;
                                                            IFieldMetadata fieldEntityMetadataf = new FieldMetadata();
                                                            if (_cache.ContainsKey(entityObject.Name))
                                                            {
                                                                if (!_cache[entityObject.Name].Fields.TryGetValue(ch.KeyFieldName, out fieldEntityMetadataf))
                                                                {
                                                                    EntityMetadata entityMetadataOverride = _cache[entityObject.Name] as EntityMetadata;
                                                                    if (entityMetadataOverride.BaseEntity != null)
                                                                    {
                                                                        var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
                                                                        var savedOldFields = _cache[entityObject.Name].Fields.Values;
                                                                        foreach (var v in savedOldFields)
                                                                        {
                                                                            dictionaryFieldsMissing.Add(v.Name, v);
                                                                        }
                                                                        IFieldMetadata fieldEntityMetadataBase = new FieldMetadata();
                                                                        if (entityMetadataOverride.BaseEntity.Fields.TryGetValue(ch.KeyFieldName, out fieldEntityMetadataBase))
                                                                        {

                                                                            var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                                                            if (primaryKeyMetadata.FieldRefs != null)
                                                                            {
                                                                                var FieldRefs = primaryKeyMetadata.FieldRefs;
                                                                                foreach (var v in FieldRefs)
                                                                                {
                                                                                    dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
                                                                                }
                                                                            }

                                                                            PrimaryKeyMetadata primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                                            primaryKeyMetadataF.Clustered = entityMetadataOverride.BaseEntity.PrimaryKey.Clustered;
                                                                            dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, entityMetadataOverride.BaseEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                                            primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                                            dictionaryFieldsMissing.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);

                                                                            entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                                            primaryKeyMetadata = primaryKeyMetadataF;
                                                                            if (entityMetadataOverride != null)
                                                                            {
                                                                                EntityMetadata entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
                                                                                entityMetadataOverridex.Fields = dictionaryFieldsMissing;
                                                                                _cache.Remove(entityObject.Name);
                                                                                _cache.Add(entityObject.Name, entityMetadataOverridex);
                                                                                dictionaryFields.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
                                                                            savedOldFields = _cache[entityObject.Name].Fields.Values;
                                                                            foreach (var v in savedOldFields)
                                                                            {
                                                                                dictionaryFieldsMissing.Add(v.Name, v);
                                                                            }
                                                                            IFieldMetadata fieldMetadataMiss = fieldMetadataf;

                                                                            var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                                                            if (primaryKeyMetadata.FieldRefs != null)
                                                                            {
                                                                                var FieldRefs = primaryKeyMetadata.FieldRefs;
                                                                                foreach (var v in FieldRefs)
                                                                                {
                                                                                    dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
                                                                                }
                                                                            }


                                                                            PrimaryKeyMetadata primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                                            primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                                                            dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                                            primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                                            dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);

                                                                            entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                                            primaryKeyMetadata = primaryKeyMetadataF;
                                                                            if (entityMetadataOverride != null)
                                                                            {
                                                                                EntityMetadata entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
                                                                                entityMetadataOverridex.Fields = dictionaryFieldsMissing;
                                                                                _cache.Remove(entityObject.Name);
                                                                                _cache.Add(entityObject.Name, entityMetadataOverridex);
                                                                                dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
                                                                        var savedOldFields = _cache[entityObject.Name].Fields.Values;
                                                                        foreach (var v in savedOldFields)
                                                                        {
                                                                            dictionaryFieldsMissing.Add(v.Name, v);
                                                                        }
                                                                        IFieldMetadata fieldMetadataMiss = fieldMetadataf;


                                                                        var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                                                        if (primaryKeyMetadata.FieldRefs != null)
                                                                        {
                                                                            var FieldRefs = primaryKeyMetadata.FieldRefs;
                                                                            foreach (var v in FieldRefs)
                                                                            {
                                                                                dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
                                                                            }
                                                                        }

                                                                        PrimaryKeyMetadata primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                                        primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                                                        dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                                        primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                                        dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);

                                                                        entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                                        primaryKeyMetadata = primaryKeyMetadataF;
                                                                        if (entityMetadataOverride != null)
                                                                        {
                                                                            EntityMetadata entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
                                                                            entityMetadataOverridex.Fields = dictionaryFieldsMissing;
                                                                            _cache.Remove(entityObject.Name);
                                                                            _cache.Add(entityObject.Name, entityMetadataOverridex);
                                                                            dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        valueprimaryKeyFieldMappedMetadata.MatchWith = (PrimaryKeyMappedMatchWith)Enum.Parse(typeof(PrimaryKeyMappedMatchWith), ch.MatchWith.ToString());
                                                        //dictionary.Add(valueprimaryKeyFieldMappedMetadata.KeyField.SourceName, valueprimaryKeyFieldMappedMetadata);
                                                        dictionary.Add(ch.Value, valueprimaryKeyFieldMappedMetadata);
                                                        
                                                    }
                                                    else if (ch.MatchWith == Metadata.PrimaryKeyMappedMatchWith.Field)
                                                    {
                                                        var fieldprimaryKeyFieldMappedMetadata = new FieldPrimaryKeyFieldMappedMetadata();
                                                        IFieldMetadata fieldMetadataf = new FieldMetadata();
                                                        if (fieldMetadata.RefEntity.Fields.TryGetValue(ch.KeyFieldName, out fieldMetadataf))
                                                        {
                                                            fieldprimaryKeyFieldMappedMetadata.KeyField = fieldMetadataf;
                                                            (fieldprimaryKeyFieldMappedMetadata as FieldPrimaryKeyFieldMappedMetadata).KeyField = fieldMetadataf;
                                                            IFieldMetadata fieldEntityMetadataf = new FieldMetadata();


                                                            if (_cache.ContainsKey(entityObject.Name))
                                                            {
                                                                if (_cache[entityObject.Name].Fields.TryGetValue(ch.Value, out fieldEntityMetadataf))
                                                                {
                                                                    (fieldprimaryKeyFieldMappedMetadata as FieldPrimaryKeyFieldMappedMetadata).EntityField = fieldEntityMetadataf;
                                                                }
                                                                else
                                                                {
                                                                    EntityMetadata entityMetadataOverride = _cache[entityObject.Name] as EntityMetadata;
                                                                    if (entityMetadataOverride.BaseEntity != null)
                                                                    {
                                                                        var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
                                                                        var savedOldFields = _cache[entityObject.Name].Fields.Values;
                                                                        foreach (var v in savedOldFields)
                                                                        {
                                                                            dictionaryFieldsMissing.Add(v.Name, v);
                                                                        }
                                                                        IFieldMetadata fieldEntityMetadataBase = new FieldMetadata();
                                                                        if (entityMetadataOverride.BaseEntity.Fields.TryGetValue(ch.Value, out fieldEntityMetadataBase))
                                                                        {

                                                                            var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                                                            if (primaryKeyMetadata.FieldRefs != null)
                                                                            {
                                                                                var FieldRefs = primaryKeyMetadata.FieldRefs;
                                                                                foreach (var v in FieldRefs)
                                                                                {
                                                                                    dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
                                                                                }
                                                                            }

                                                                            PrimaryKeyMetadata primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                                            primaryKeyMetadataF.Clustered = entityMetadataOverride.BaseEntity.PrimaryKey.Clustered;
                                                                            dictionaryPrimaryKeyFieldRefMetadata.Add(ch.Value, entityMetadataOverride.BaseEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.Value).Value);
                                                                            primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                                            dictionaryFieldsMissing.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);

                                                                            entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                                            primaryKeyMetadata = primaryKeyMetadataF;
                                                                            if (entityMetadataOverride != null)
                                                                            {
                                                                                EntityMetadata entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
                                                                                entityMetadataOverridex.Fields = dictionaryFieldsMissing;
                                                                                _cache.Remove(entityObject.Name);
                                                                                _cache.Add(entityObject.Name, entityMetadataOverridex);
                                                                                dictionaryFields.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
                                                                            savedOldFields = _cache[entityObject.Name].Fields.Values;
                                                                            foreach (var v in savedOldFields)
                                                                            {
                                                                                dictionaryFieldsMissing.Add(v.Name, v);
                                                                            }
                                                                            IFieldMetadata fieldMetadataMiss = fieldMetadataf;

                                                                            var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                                                            if (primaryKeyMetadata.FieldRefs != null)
                                                                            {
                                                                                var FieldRefs = primaryKeyMetadata.FieldRefs;
                                                                                foreach (var v in FieldRefs)
                                                                                {
                                                                                    dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
                                                                                }
                                                                            }

                                                                            PrimaryKeyMetadata primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                                            primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                                                            dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                                            primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                                            dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);

                                                                            entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                                            primaryKeyMetadata = primaryKeyMetadataF;
                                                                            if (entityMetadataOverride != null)
                                                                            {
                                                                                EntityMetadata entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
                                                                                entityMetadataOverridex.Fields = dictionaryFieldsMissing;
                                                                                _cache.Remove(entityObject.Name);
                                                                                _cache.Add(entityObject.Name, entityMetadataOverridex);
                                                                                dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
                                                                        var savedOldFields = _cache[entityObject.Name].Fields.Values;
                                                                        foreach (var v in savedOldFields)
                                                                        {
                                                                            dictionaryFieldsMissing.Add(v.Name, v);
                                                                        }
                                                                        IFieldMetadata fieldMetadataMiss = fieldMetadataf;


                                                                        var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                                                        if (primaryKeyMetadata.FieldRefs != null)
                                                                        {
                                                                            var FieldRefs = primaryKeyMetadata.FieldRefs;
                                                                            foreach (var v in FieldRefs)
                                                                            {
                                                                                dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
                                                                            }
                                                                        }

                                                                        PrimaryKeyMetadata primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                                        primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                                                        dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                                        primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                                        dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);

                                                                        entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                                        primaryKeyMetadata = primaryKeyMetadataF;
                                                                        if (entityMetadataOverride != null)
                                                                        {
                                                                            EntityMetadata entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
                                                                            entityMetadataOverridex.Fields = dictionaryFieldsMissing;
                                                                            _cache.Remove(entityObject.Name);
                                                                            _cache.Add(entityObject.Name, entityMetadataOverridex);
                                                                            dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                        fieldprimaryKeyFieldMappedMetadata.MatchWith = (PrimaryKeyMappedMatchWith)Enum.Parse(typeof(PrimaryKeyMappedMatchWith), ch.MatchWith.ToString());
                                                        //dictionary.Add(fieldprimaryKeyFieldMappedMetadata.KeyField.SourceName, fieldprimaryKeyFieldMappedMetadata);
                                                        dictionary.Add(ch.Value, fieldprimaryKeyFieldMappedMetadata);
                                                        
                                                    }
                                                    else if (ch.MatchWith == Metadata.PrimaryKeyMappedMatchWith.SourceName)
                                                    {
                                                        var sourceprimaryKeyFieldMappedMetadata = new SourceNamePrimaryKeyFieldMappedMetadata();
                                                        IFieldMetadata fieldMetadataf = new FieldMetadata();
                                                        if (fieldMetadata.RefEntity.Fields.TryGetValue(ch.KeyFieldName, out fieldMetadataf))
                                                        {
                                                            sourceprimaryKeyFieldMappedMetadata.KeyField = fieldMetadataf;
                                                            (sourceprimaryKeyFieldMappedMetadata as SourceNamePrimaryKeyFieldMappedMetadata).SourceName = ch.Value;
                                                            IFieldMetadata fieldEntityMetadataf = new FieldMetadata();
                                                            if (_cache.ContainsKey(entityObject.Name))
                                                            {
                                                                fieldEntityMetadataf = _cache[entityObject.Name].Fields.ToList().Find(z => z.Value.SourceName == ch.Value).Value;
                                                                if (fieldEntityMetadataf == null)
                                                                {
                                                                    EntityMetadata entityMetadataOverride = _cache[entityObject.Name] as EntityMetadata;
                                                                    if (entityMetadataOverride.BaseEntity != null)
                                                                    {
                                                                        var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
                                                                        var savedOldFields = _cache[entityObject.Name].Fields.Values;
                                                                        foreach (var v in savedOldFields)
                                                                        {
                                                                            dictionaryFieldsMissing.Add(v.Name, v);
                                                                        }
                                                                        IFieldMetadata fieldEntityMetadataBase = new FieldMetadata();
                                                                        fieldEntityMetadataBase = entityMetadataOverride.BaseEntity.Fields.ToList().Find(z => z.Value.SourceName == ch.Value).Value;
                                                                        if (fieldEntityMetadataBase != null)
                                                                        {
                                                                            var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                                                            if (primaryKeyMetadata.FieldRefs != null)
                                                                            {
                                                                                var FieldRefs = primaryKeyMetadata.FieldRefs;
                                                                                foreach (var v in FieldRefs)
                                                                                {
                                                                                    dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
                                                                                }
                                                                            }


                                                                            PrimaryKeyMetadata primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                                            primaryKeyMetadataF.Clustered = entityMetadataOverride.BaseEntity.PrimaryKey.Clustered;
                                                                            dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, entityMetadataOverride.BaseEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                                            primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                                            dictionaryFieldsMissing.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);

                                                                            entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                                            primaryKeyMetadata = primaryKeyMetadataF;
                                                                            if (entityMetadataOverride != null)
                                                                            {
                                                                                EntityMetadata entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
                                                                                entityMetadataOverridex.Fields = dictionaryFieldsMissing;
                                                                                _cache.Remove(entityObject.Name);
                                                                                _cache.Add(entityObject.Name, entityMetadataOverridex);
                                                                                dictionaryFields.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
                                                                            savedOldFields = _cache[entityObject.Name].Fields.Values;
                                                                            foreach (var v in savedOldFields)
                                                                            {
                                                                                dictionaryFieldsMissing.Add(v.Name, v);
                                                                            }

                                                                            if (fieldMetadataf != null)
                                                                            {
                                                                                FieldMetadata fieldMetadataMiss = fieldMetadataf as FieldMetadata;
                                                                                fieldMetadataMiss.SourceName = ch.Value;

                                                                                var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                                                                if (primaryKeyMetadata.FieldRefs != null)
                                                                                {
                                                                                    var FieldRefs = primaryKeyMetadata.FieldRefs;
                                                                                    foreach (var v in FieldRefs)
                                                                                    {
                                                                                        dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
                                                                                    }
                                                                                }

                                                                                PrimaryKeyMetadata primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                                                primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                                                                dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                                                primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                                                dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);


                                                                                entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                                                primaryKeyMetadata = primaryKeyMetadataF;
                                                                                if (entityMetadataOverride != null)
                                                                                {
                                                                                    EntityMetadata entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
                                                                                    entityMetadataOverridex.Fields = dictionaryFieldsMissing;
                                                                                    _cache.Remove(entityObject.Name);
                                                                                    _cache.Add(entityObject.Name, entityMetadataOverridex);
                                                                                    dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
                                                                        var savedOldFields = _cache[entityObject.Name].Fields.Values;
                                                                        foreach (var v in savedOldFields)
                                                                        {
                                                                            dictionaryFieldsMissing.Add(v.Name, v);
                                                                        }
                                                                        IFieldMetadata fieldMetadataMiss = fieldMetadataf;


                                                                        var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                                                        if (primaryKeyMetadata.FieldRefs != null)
                                                                        {
                                                                            var FieldRefs = primaryKeyMetadata.FieldRefs;
                                                                            foreach (var v in FieldRefs)
                                                                            {
                                                                                dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
                                                                            }
                                                                        }

                                                                        PrimaryKeyMetadata primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                                        primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                                                        dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                                        primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                                        dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);

                                                                        entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                                        primaryKeyMetadata = primaryKeyMetadataF;
                                                                        if (entityMetadataOverride != null)
                                                                        {
                                                                            EntityMetadata entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
                                                                            entityMetadataOverridex.Fields = dictionaryFieldsMissing;
                                                                            _cache.Remove(entityObject.Name);
                                                                            _cache.Add(entityObject.Name, entityMetadataOverridex);
                                                                            dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        sourceprimaryKeyFieldMappedMetadata.MatchWith = (PrimaryKeyMappedMatchWith)Enum.Parse(typeof(PrimaryKeyMappedMatchWith), ch.MatchWith.ToString());
                                                        //dictionary.Add(sourceprimaryKeyFieldMappedMetadata.KeyField.SourceName, sourceprimaryKeyFieldMappedMetadata);
                                                        dictionary.Add(ch.Value, sourceprimaryKeyFieldMappedMetadata);
                                                        
                                                    }
                                                }
                                            }
                                            primaryKeyFieldMappingMetadata.Fields = dictionary;
                                            fieldMetadata.Mapping = primaryKeyFieldMappingMetadata;
                                        }

                                        fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                                        fieldMetadata.Title = fieldDef.Title;
                                        fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                                        fieldMetadata.SourceName = fieldDef.SourceName;
                                        fieldMetadata.Required = fieldDef.Required;
                                        fieldMetadata.Name = fieldDef.Name;
                                        fieldMetadata.Desc = fieldDef.Desc;
                                        dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);


                                        if (this._referenceFieldMetadata.ToList().Find(c => c.Key.Name == fieldMetadata.Name).Key == null)
                                        {
                                            this._referenceFieldMetadata.Add(fieldMetadata, dataSourceMetadata.Name);
                                        }
                                        else
                                        {
                                            this._referenceFieldMetadata.Remove(this._referenceFieldMetadata.ToList().Find(c => c.Key.Name == fieldMetadata.Name).Key);
                                            this._referenceFieldMetadata.Add(fieldMetadata, dataSourceMetadata.Name);
                                        }
                                    }
                                    else if (fieldDef.SourceType == Metadata.FieldSourceType.Extension)
                                    {
                                        var fieldMetadata = new ExtensionFieldMetadata();
                                        if (fieldDef.DataType != null)
                                        {
                                            fieldMetadata.DataType = GetDataTypeMetadata(fieldDef.DataType, dataSourceMetadata.Type);
                                        }

                                        var primaryKeyFieldMappedMetadata = new PrimaryKeyFieldMappedMetadata();
                                        if (!string.IsNullOrEmpty(fieldDef.SourceName))
                                        {
                                            if (!_cashecontainerEntity.Contains(fieldDef.SourceName))
                                            {
                                                _cashecontainerEntity.Add(fieldDef.SourceName);
                                                fieldMetadata.ExtensionEntity = GetEntityMetadata(fieldDef.SourceName);
                                                _cashecontainerEntityList.Add(fieldMetadata.ExtensionEntity);

                                                if (_cache.ContainsKey(fieldDef.SourceName))
                                                {
                                                    _cache.Remove(fieldDef.SourceName);
                                                    _cache.Add(fieldDef.SourceName, fieldMetadata.ExtensionEntity);
                                                }
                                            }
                                            else
                                            {
                                                fieldMetadata.ExtensionEntity = _cashecontainerEntityList.Find(t => t.Name == fieldDef.SourceName);
                                            }
                                        }
                                       
                                        //в расширяемую сущность переносим все данные по первичному ключу с сущности, которую расширяем
                                        /*
                                        if (entityMetadata.PrimaryKey != null)
                                        {
                                            if (fieldMetadata.ExtensionEntity != null)
                                            {
                                                if (fieldMetadata.ExtensionEntity.PrimaryKey != null)
                                                {
                                                    (fieldMetadata.ExtensionEntity.PrimaryKey as PrimaryKeyMetadata).FieldRefs = entityMetadata.PrimaryKey.FieldRefs;
                                                }
                                            }
                                        }
                                        */

                                        fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                                        fieldMetadata.Title = fieldDef.Title;
                                        fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                                        fieldMetadata.SourceName = fieldDef.SourceName;
                                        fieldMetadata.Required = fieldDef.Required;
                                        fieldMetadata.Name = fieldDef.Name;
                                        fieldMetadata.Desc = fieldDef.Desc;
                                        dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);


                                        if (this._extensionFieldMetadata.ToList().Find(c => c.Key.Name == fieldMetadata.Name).Key == null)
                                        {
                                            this._extensionFieldMetadata.Add(fieldMetadata, dataSourceMetadata.Name);
                                        }
                                        else
                                        {
                                            this._extensionFieldMetadata.Remove(this._extensionFieldMetadata.ToList().Find(c => c.Key.Name == fieldMetadata.Name).Key);
                                            this._extensionFieldMetadata.Add(fieldMetadata, dataSourceMetadata.Name);
                                        }

                                    }
                                    else if (fieldDef.SourceType == Metadata.FieldSourceType.Relation)
                                    {
                                        var fieldMetadata = new RelationFieldMetadata();
                                        if (fieldDef.DataType != null)
                                        {
                                            fieldMetadata.DataType = GetDataTypeMetadata(fieldDef.DataType, dataSourceMetadata.Type);
                                        }
                                        var primaryKeyFieldMappedMetadata = new PrimaryKeyFieldMappedMetadata();
                                        if (!string.IsNullOrEmpty(fieldDef.SourceName))
                                        {
                                            if (!_cashecontainerEntity.Contains(fieldDef.SourceName))
                                            {
                                                _cashecontainerEntity.Add(fieldDef.SourceName);
                                                fieldMetadata.RelatedEntity = GetEntityMetadata(fieldDef.SourceName);
                                                _cashecontainerEntityList.Add(fieldMetadata.RelatedEntity);

                                                if (_cache.ContainsKey(fieldDef.SourceName))
                                                {
                                                    _cache.Remove(fieldDef.SourceName);
                                                    _cache.Add(fieldDef.SourceName, fieldMetadata.RelatedEntity);
                                                }
                                            }
                                            else
                                            {
                                                fieldMetadata.RelatedEntity = _cashecontainerEntityList.Find(t => t.Name == fieldDef.SourceName);
                                            }
                                        }
                                        fieldMetadata.RelationCondition = new DataModels.DataConstraint.ComplexCondition();
                                        ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Operator = (DataModels.DataConstraint.LogicalOperator)Enum.Parse(typeof(DataModels.DataConstraint.LogicalOperator), fieldDef.RelationCondition.ItemElementName.ToString());

                                        if (fieldDef.RelationCondition.Item is Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef)
                                        {
                                            object[] items = (fieldDef.RelationCondition.Item as Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef).Items;
                                            if (items != null)
                                            {
                                                ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions = new DataModels.DataConstraint.ComplexCondition[items.Length];
                                                for (int k = 0; k < items.Length; k++)
                                                {
                                                    ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k] = new DataModels.DataConstraint.ComplexCondition();
                                                    if (items[k] is Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef)
                                                    {
                                                        ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k] = new DataModels.DataConstraint.ComplexCondition();
                                                        var expr = items[k] as Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef;
                                                        if (expr != null)
                                                        {
                                                            if (expr.Items != null)
                                                            {
                                                                ((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions = new DataModels.DataConstraint.ConditionExpression[expr.Items.Length];
                                                                for (int z = 0; z < expr.Items.Length; z++)
                                                                {
                                                                    ((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z] = new DataModels.DataConstraint.ConditionExpression();
                                                                    if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.TwoOperandsOperationDef)
                                                                    {
                                                                        ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).Operator = (DataModels.DataConstraint.ConditionOperator)Enum.Parse(typeof(DataModels.DataConstraint.ConditionOperator), expr.ItemsElementName[z].ToString());

                                                                        var twoOperans = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.TwoOperandsOperationDef);
                                                                        if (twoOperans != null)
                                                                        {
                                                                            if (twoOperans.Item is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand = new DataModels.DataConstraint.ColumnOperand();
                                                                                string Name = (twoOperans.Item as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                                                ((DataModels.DataConstraint.ColumnOperand)((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand).ColumnName = Name;
                                                                            }
                                                                            if (twoOperans.Item is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand = new DataModels.DataConstraint.StringValueOperand();
                                                                                string Value = (twoOperans.Item as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
                                                                                ((DataModels.DataConstraint.StringValueOperand)(((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z])).LeftOperand).Value = Value;
                                                                            }

                                                                            if (twoOperans.Item1 is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).RightOperand = new DataModels.DataConstraint.ColumnOperand();
                                                                                string Name = (twoOperans.Item1 as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                                                ((DataModels.DataConstraint.ColumnOperand)((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).RightOperand).ColumnName = Name;

                                                                            }
                                                                            if (twoOperans.Item1 is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).RightOperand = new DataModels.DataConstraint.StringValueOperand();
                                                                                string Value = (twoOperans.Item1 as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
                                                                                ((DataModels.DataConstraint.StringValueOperand)(((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z])).RightOperand).Value = Value;
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.OneOperandOperationDef)
                                                                    {
                                                                        ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).Operator = (DataModels.DataConstraint.ConditionOperator)Enum.Parse(typeof(DataModels.DataConstraint.ConditionOperator), expr.ItemsElementName[z].ToString());
                                                                        var oneOperand = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.OneOperandOperationDef);
                                                                        if (oneOperand != null)
                                                                        {

                                                                            if (oneOperand.Item is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand = new DataModels.DataConstraint.ColumnOperand();
                                                                                string Name = (oneOperand.Item as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                                                ((DataModels.DataConstraint.ColumnOperand)((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand).ColumnName = Name;

                                                                            }
                                                                            if (oneOperand.Item is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).RightOperand = new DataModels.DataConstraint.StringValueOperand();
                                                                                string Value = (oneOperand.Item as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
                                                                                ((DataModels.DataConstraint.StringValueOperand)(((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z])).RightOperand).Value = Value;
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.InOperationDef)
                                                                    {
                                                                        ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).Operator = (DataModels.DataConstraint.ConditionOperator)Enum.Parse(typeof(DataModels.DataConstraint.ConditionOperator), expr.ItemsElementName[z].ToString());
                                                                        var oneOperand = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.InOperationDef);
                                                                        if (oneOperand != null)
                                                                        {
                                                                            if (oneOperand.Field is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand = new DataModels.DataConstraint.ColumnOperand();
                                                                                string Name = (oneOperand.Field as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                                                ((DataModels.DataConstraint.ColumnOperand)((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand).ColumnName = Name;
                                                                            }
                                                                            if (oneOperand.Values is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef[])
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).RightOperand = new DataModels.DataConstraint.StringValuesOperand();
                                                                                List<string> Values = new List<string>();
                                                                                foreach (var cx in oneOperand.Values)
                                                                                {
                                                                                    Values.Add(cx.Value);
                                                                                }
                                                                                ((DataModels.DataConstraint.StringValuesOperand)(((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z])).RightOperand).Values = Values.ToArray();
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        throw new NotImplementedException();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        fieldMetadata.RelationCondition.Type = DataModels.DataConstraint.ConditionType.Complex;
                                        fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                                        fieldMetadata.Title = fieldDef.Title;
                                        fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                                        fieldMetadata.SourceName = fieldDef.SourceName;
                                        fieldMetadata.Required = fieldDef.Required;
                                        fieldMetadata.Name = fieldDef.Name;
                                        fieldMetadata.Desc = fieldDef.Desc;
                                        dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);

                                        if (this._relationFieldMetadata.ToList().Find(c => c.Key.Name == fieldMetadata.Name).Key == null)
                                        {
                                            this._relationFieldMetadata.Add(fieldMetadata, dataSourceMetadata.Name);
                                        }
                                        else
                                        {
                                            this._relationFieldMetadata.Remove(this._relationFieldMetadata.ToList().Find(c => c.Key.Name == fieldMetadata.Name).Key);
                                            this._relationFieldMetadata.Add(fieldMetadata, dataSourceMetadata.Name);
                                        }
                                    }
                                    else if (fieldDef.SourceType == Metadata.FieldSourceType.Expression)
                                    {
                                        throw new NotImplementedException();
                                    }
                                    else
                                    {
                                        throw new NotImplementedException();
                                    }
                                }
                                entityMetadata.Fields = dictionaryFields;
                                entityMetadata.Inheritance = (InheritanceType)Enum.Parse(typeof(InheritanceType), entityObject.Inheritance.ToString());
                                entityMetadata.Name = entityObject.Name;

                                var dicIPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                if (entityObject.PrimaryKey != null)
                                {
                                    primaryKeyMetadata.Clustered = entityObject.PrimaryKey.Clustered;
                                    foreach (var fld in entityObject.PrimaryKey.FieldRef)
                                    {
                                        var primaryKeyFieldRefMetadata = new PrimaryKeyFieldRefMetadata();
                                        primaryKeyFieldRefMetadata.SortOrder = (DataModels.DataConstraint.SortDirection)Enum.Parse(typeof(DataModels.DataConstraint.SortDirection), fld.SortOrder.ToString());
                                        primaryKeyFieldRefMetadata.Field = primaryKeyFieldRefMetadata;
                                        dicIPrimaryKeyFieldRefMetadata.Add(fld.Name, primaryKeyFieldRefMetadata);
                                    }
                                }
                                if (primaryKeyMetadata.FieldRefs != null)
                                {
                                    foreach (var c in primaryKeyMetadata.FieldRefs)
                                    {
                                        dicIPrimaryKeyFieldRefMetadata.Add(c.Key, c.Value);
                                    }
                                }
                                primaryKeyMetadata.FieldRefs = dicIPrimaryKeyFieldRefMetadata;
                                entityMetadata.PrimaryKey = primaryKeyMetadata;

                                entityMetadata.Title = entityObject.Title;
                                entityMetadata.Type = (EntityType)Enum.Parse(typeof(EntityType), entityObject.Type.ToString());
                                isFinded = true;
                            }
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                    if (isFinded)
                    {
                        AddedMissingPrimaryKeyExtension(entityMetadata);

                        if (!_cache.ContainsKey(entityName))
                            _cache.Add(entityName, entityMetadata);
                        else
                        {
                            AddMissingPropertyValues(typeof(EntityMetadata), entityMetadata, _cache[entityName]);
                        }
                        break;
                    }
                }
            }

            _cashecontainerEntity.Clear();
            _cashecontainerEntityList.Clear();
            return entityMetadata;
        }
    }
}
