using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using Atdi.Common;
using RE = System.Reflection.Emit;

//using Atdi.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm
{
    public class EntityOrm : IEntityOrm
    {
        private readonly DataTypeSystem _dataTypeSystem;
        private readonly IEntityOrmConfig _config;
        private readonly ConcurrentDictionary<string, DataTypeMetadata> _dataTypeMetadataCache;
        private readonly ConcurrentDictionary<string, EntityMetadata> _entityMetadataCache;
        private readonly ConcurrentDictionary<Type, Type> _primaryKeyTypes;

        public EntityOrm(DataTypeSystem dataTypeSystem, IEntityOrmConfig config)
        {
            this._dataTypeSystem = dataTypeSystem;
            this._config = config;
            this._dataTypeMetadataCache = new ConcurrentDictionary<string, DataTypeMetadata>();
            this._entityMetadataCache = new ConcurrentDictionary<string, EntityMetadata>();
            this._primaryKeyTypes = new ConcurrentDictionary<Type, Type>();
        }


        /// <summary>
        /// Извлечение данных о типах
        /// </summary>
        /// <param name="dataTypeName"></param>
        /// <param name="dataSourceType"></param>
        /// <returns></returns>
        public IDataTypeMetadata GetDataTypeMetadata(string dataTypeName, Atdi.Contracts.CoreServices.EntityOrm.Metadata.DataSourceType dataSourceType)
        {
            if (string.IsNullOrWhiteSpace(dataTypeName))
            {
                throw new ArgumentException("Undefined a data type name", nameof(dataTypeName));
            }

            if (string.IsNullOrWhiteSpace(_config.DataTypesPath))
            {
                throw new ArgumentException("Undefined data type path in configuration");
            }

            var key = $"{dataSourceType}:{dataTypeName}";

            if (_dataTypeMetadataCache.TryGetValue(key, out DataTypeMetadata dataTypeMetadata))
            {
                return dataTypeMetadata;
            }
            dataTypeMetadata = DeserializeDataTypeMetadata(dataTypeName, dataSourceType);
            if (!_dataTypeMetadataCache.TryAdd(key, dataTypeMetadata))
            {
                if (!_dataTypeMetadataCache.TryGetValue(key, out dataTypeMetadata))
                {
                    throw new InvalidOperationException($"Failed to add the data type metadata information to cache with key '{key}'");
                }
            }
            return dataTypeMetadata;
        }

        private DataTypeMetadata DeserializeDataTypeMetadata(string dataTypeName, DataSourceType dataSourceType)
        {
            var dataTypeMetadata = new DataTypeMetadata();

            var filePath = GetFullPathFile(dataTypeName, _config.DataTypesPath, dataSourceType, "xml");

            using (var reader = new StreamReader(filePath))
            {
                var serializer = new XmlSerializer(typeof(Atdi.CoreServices.EntityOrm.Metadata.DataTypeDef));
                if (!(serializer.Deserialize(reader) is Atdi.CoreServices.EntityOrm.Metadata.DataTypeDef dataTypeDef))
                {
                    throw new InvalidOperationException($"Failed to deserialize data type metadata object from the file by path '{filePath}'");
                }

                dataTypeMetadata.Name = dataTypeDef.Name;
                dataTypeMetadata.DataSourceType = (DataSourceType)Enum.Parse(typeof(DataSourceType), dataTypeDef.DataSourceType.ToString());

                if (dataTypeDef.CodeVarType == null)
                {
                    throw new InvalidOperationException($"Undefined CodeVarType for Data Type Metadata with name '{dataTypeName}' and source '{dataSourceType}'");
                }
                dataTypeMetadata.CodeVarType = (DataModels.DataType)Enum.Parse(typeof(DataModels.DataType), dataTypeDef.CodeVarType.Value.ToString());

                if (dataTypeMetadata.CodeVarType == DataModels.DataType.ClrType)
                {
                    var clrType = dataTypeDef.CodeVarType.ClrType;
                    if (string.IsNullOrEmpty(clrType))
                    {
                        throw new InvalidOperationException($"Undefined a CLR Type for Data Type Metadata with name '{dataTypeName}' and source '{dataSourceType}'");
                    }
                    dataTypeMetadata.CodeVarClrType = Type.GetType(clrType);
                    if (dataTypeMetadata.CodeVarClrType == null)
                    {
                        throw new InvalidOperationException($"Undefined a CLR Type '{clrType}' for Data Type Metadata with name '{dataTypeName}' and source '{dataSourceType}'");
                    }
                }

                if (dataTypeDef.SourceVarType == null)
                {
                    throw new InvalidOperationException($"Undefined SourceVarType for Data Type Metadata with name '{dataTypeName}' and source '{dataSourceType}'");
                }
                dataTypeMetadata.SourceVarType = (DataSourceVarType)Enum.Parse(typeof(DataSourceVarType), dataTypeDef.SourceVarType.Value.ToString());
                dataTypeMetadata.SourceCodeVarType = _dataTypeSystem.GetSourceDataType(dataTypeMetadata.SourceVarType);
                if (dataTypeDef.Autonum != null)
                {
                    var autonumMetadata = new AutonumMetadata
                    {
                        Start = (int)dataTypeDef.Autonum.Start,
                        Step = (int)dataTypeDef.Autonum.Step
                    };
                    dataTypeMetadata.Autonum = autonumMetadata;
                }

                if (dataTypeDef.Length != null)
                {
                    dataTypeMetadata.Length = dataTypeDef.Length.Value;
                }
                if (dataTypeDef.Precision != null)
                {
                    if (!string.IsNullOrEmpty(dataTypeDef.Precision.Value))
                    {
                        dataTypeMetadata.Precision = Convert.ToInt32(dataTypeDef.Precision.Value);
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

                dataTypeMetadata.Multiple = dataTypeDef.Multiple;

                if (dataTypeDef.StoreContentType != null)
                {
                    dataTypeMetadata.ContentType = (StoreContentType)Enum.Parse(typeof(StoreContentType), dataTypeDef.StoreContentType.Value.ToString());
                }
            }

            return dataTypeMetadata;
        }


        /// НЕ нужно копировать первичный ключи из расширения или базового объекта, это береться с контекста
        /// <summary>
        /// Добавление сведений об "отсутствующих" первичных ключах из расширяемой сущности в сущность расширения
        /// </summary>
        /// <param name="entityMetadata"></param>
        //private void AddedMissingPrimaryKeyExtension(IEntityMetadata entityMetadata)
        //{
        //    if (entityMetadata.PrimaryKey != null)
        //    {
        //        if (entityMetadata.ExtendEntity != null)
        //        {
        //            if (entityMetadata.ExtendEntity.PrimaryKey != null)
        //            {
        //                (entityMetadata.PrimaryKey as PrimaryKeyMetadata).FieldRefs = (entityMetadata.ExtendEntity.PrimaryKey as PrimaryKeyMetadata).FieldRefs;
        //            }
        //        }

        //        if (entityMetadata.Fields != null)
        //        {
        //            var dicMetaData = new Dictionary<string, IFieldMetadata>();
        //            foreach (var x in entityMetadata.Fields.Values)
        //            {
        //                if (x is ExtensionFieldMetadata)
        //                {
        //                    if ((x as ExtensionFieldMetadata).ExtensionEntity != null)
        //                    {
        //                        ((x as ExtensionFieldMetadata).ExtensionEntity.PrimaryKey as PrimaryKeyMetadata).FieldRefs = entityMetadata.PrimaryKey.FieldRefs;
        //                    }
        //                }
        //            }
        //        }

        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
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
        /// Извлечение данных о единице измерения
        /// </summary>
        /// <param name="unitName"></param>
        /// <returns></returns>
        public IUnitMetadata GetUnitMetadata(string unitName)
        {
            var unitMetadata = new UnitMetadata();
            {
                var serializer = new XmlSerializer(typeof(Atdi.CoreServices.EntityOrm.Metadata.UnitDef));
                var reader = new StreamReader(GetFullPathFile(unitName, _config.UnitsPath, "xml"));
                object resEntity = serializer.Deserialize(reader);
                if (resEntity is Atdi.CoreServices.EntityOrm.Metadata.UnitDef)
                {
                    var unitObject = resEntity as Atdi.CoreServices.EntityOrm.Metadata.UnitDef;
                    if (unitObject != null)
                    {
                        unitMetadata.Name = unitObject.Name;
                        unitMetadata.Dimension = unitObject.Dimension.Value;
                        unitMetadata.Category = unitObject.Category.Value;
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            return unitMetadata;
        }

        /// <summary>
        /// Определение пути разещения xml - файла
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Path"></param>
        /// <param name="dataSourceType"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        private string GetFullPathFile(string Name, string Path, Atdi.Contracts.CoreServices.EntityOrm.Metadata.DataSourceType dataSourceType, string extension)
        {
            string fullPath = "";
            if ((!string.IsNullOrEmpty(Path)) && (!string.IsNullOrEmpty(Name)))
            {
                fullPath = Path + @"\" + dataSourceType.ToString() + @"\" + Name + "." + extension;
                if (!System.IO.File.Exists(fullPath))
                {
                    throw new Exception(string.Format(Exceptions.FileNotFound, fullPath));
                }
            }
            else
            {
                throw new Exception(Exceptions.NameOrPathisNotSpecified);
            }
            return fullPath;
        }

        /// <summary>
        /// Определение пути разещения xml - файла
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Path"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        private string GetFullPathFile(string Name, string Path, string extension)
        {
            string fullPath = "";
            if ((!string.IsNullOrEmpty(Path)) && (!string.IsNullOrEmpty(Name)))
            {
                fullPath = Path + @"\" + Name + "." + extension;
                if (!System.IO.File.Exists(fullPath))
                {
                    throw new Exception(string.Format(Exceptions.FileNotFound, fullPath));
                }
            }
            else
            {
                throw new Exception(Exceptions.NameOrPathisNotSpecified);
            }
            return fullPath;
        }

        /*
        /// <summary>
        /// Определение пути разещения xml - файла
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        private string GetFullPathFile(string Name, string Path)
        {
            string fullPath = "";
            if ((!string.IsNullOrEmpty(Path)) && (!string.IsNullOrEmpty(Name)))
            {
                fullPath = Path + @"\" + Name;
                if (!System.IO.File.Exists(fullPath))
                {
                    throw new Exception(string.Format(Exceptions.FileNotFound, fullPath));
                }
            }
            else
            {
                throw new Exception(Exceptions.NameOrPathisNotSpecified);
            }
            return fullPath;
        }
        */
        /// <summary>
        /// Добавление в структуру сущности типа Extension сведений о первичных ключах из расширяемой сущности
        /// </summary>
        /// <param name="fieldMetadata"></param>
        /// <param name="fieldDef"></param>
        /// <param name="entityObject"></param>
        /// <param name="primaryKeyMetadata"></param>
        /// <param name="dictionaryFields"></param>
        //private void CheckFieldsForReference(ref ReferenceFieldMetadata fieldMetadata, Metadata.FieldDef fieldDef, Metadata.EntityDef entityObject, ref PrimaryKeyMetadata primaryKeyMetadata, ref Dictionary<string, IFieldMetadata> dictionaryFields)
        //{
        //    //if ((fieldMetadata.RefEntity != null) && (fieldDef.PrimaryKeyMapping != null))
        //    //{
        //    //    var primaryKeyFieldMappingMetadata = new PrimaryKeyMappingMetadata();
        //    //    var dictionary = new Dictionary<string, IPrimaryKeyFieldMappedMetadata>();
        //    //    foreach (var ch in fieldDef.PrimaryKeyMapping)
        //    //    {
        //    //        if (fieldMetadata.RefEntity.Fields != null)
        //    //        {
        //    //            if (ch.MatchWith == Metadata.PrimaryKeyMappedMatchWith.Value)
        //    //            {
        //    //                var valueprimaryKeyFieldMappedMetadata = new ValuePrimaryKeyFieldMappedMetadata();
        //    //                IFieldMetadata fieldMetadataf = new FieldMetadata();
        //    //                if (fieldMetadata.RefEntity.Fields.TryGetValue(ch.KeyFieldName, out fieldMetadataf))
        //    //                {
        //    //                    valueprimaryKeyFieldMappedMetadata.KeyField = fieldMetadataf;
        //    //                    (valueprimaryKeyFieldMappedMetadata as ValuePrimaryKeyFieldMappedMetadata).Value = ch.Value;
        //    //                    IFieldMetadata fieldEntityMetadataf = new FieldMetadata();
        //    //                    if (_cache.ContainsKey(entityObject.Name))
        //    //                    {
        //    //                        if (!_cache[entityObject.Name].Fields.TryGetValue(ch.KeyFieldName, out fieldEntityMetadataf))
        //    //                        {
        //    //                            EntityMetadata entityMetadataOverride = _cache[entityObject.Name] as EntityMetadata;
        //    //                            if (entityMetadataOverride.BaseEntity != null)
        //    //                            {
        //    //                                var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
        //    //                                var savedOldFields = _cache[entityObject.Name].Fields.Values;
        //    //                                foreach (var v in savedOldFields)
        //    //                                {
        //    //                                    dictionaryFieldsMissing.Add(v.Name, v);
        //    //                                }
        //    //                                IFieldMetadata fieldEntityMetadataBase = new FieldMetadata();
        //    //                                if (entityMetadataOverride.BaseEntity.Fields.TryGetValue(ch.KeyFieldName, out fieldEntityMetadataBase))
        //    //                                {

        //    //                                    var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
        //    //                                    if (primaryKeyMetadata.FieldRefs != null)
        //    //                                    {
        //    //                                        var FieldRefs = primaryKeyMetadata.FieldRefs;
        //    //                                        foreach (var v in FieldRefs)
        //    //                                        {
        //    //                                            dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
        //    //                                        }
        //    //                                    }

        //    //                                    var primaryKeyMetadataF = new PrimaryKeyMetadata();
        //    //                                    primaryKeyMetadataF.Clustered = entityMetadataOverride.BaseEntity.PrimaryKey.Clustered;
        //    //                                    if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
        //    //                                    {
        //    //                                        dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, entityMetadataOverride.BaseEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
        //    //                                    }
        //    //                                    primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
        //    //                                    if (!dictionaryFieldsMissing.ContainsKey(fieldEntityMetadataBase.Name))
        //    //                                    {
        //    //                                        dictionaryFieldsMissing.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
        //    //                                    }

        //    //                                    entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
        //    //                                    primaryKeyMetadata = primaryKeyMetadataF;
        //    //                                    if (entityMetadataOverride != null)
        //    //                                    {
        //    //                                        var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
        //    //                                        entityMetadataOverridex.Fields = dictionaryFieldsMissing;
        //    //                                        _cache.Remove(entityObject.Name);
        //    //                                        _cache.Add(entityObject.Name, entityMetadataOverridex);
        //    //                                        dictionaryFields.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
        //    //                                    }
        //    //                                }
        //    //                                else
        //    //                                {
        //    //                                    dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
        //    //                                    savedOldFields = _cache[entityObject.Name].Fields.Values;
        //    //                                    foreach (var v in savedOldFields)
        //    //                                    {
        //    //                                        dictionaryFieldsMissing.Add(v.Name, v);
        //    //                                    }
        //    //                                    IFieldMetadata fieldMetadataMiss = fieldMetadataf;

        //    //                                    var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
        //    //                                    if (primaryKeyMetadata.FieldRefs != null)
        //    //                                    {
        //    //                                        var FieldRefs = primaryKeyMetadata.FieldRefs;
        //    //                                        foreach (var v in FieldRefs)
        //    //                                        {
        //    //                                            dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
        //    //                                        }
        //    //                                    }

        //    //                                    var primaryKeyMetadataF = new PrimaryKeyMetadata();
        //    //                                    primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
        //    //                                    if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
        //    //                                    {
        //    //                                        dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
        //    //                                    }
        //    //                                    primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
        //    //                                    if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
        //    //                                    {
        //    //                                        dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                    }

        //    //                                    entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
        //    //                                    primaryKeyMetadata = primaryKeyMetadataF;
        //    //                                    if (entityMetadataOverride != null)
        //    //                                    {
        //    //                                        var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
        //    //                                        entityMetadataOverridex.Fields = dictionaryFieldsMissing;
        //    //                                        _cache.Remove(entityObject.Name);
        //    //                                        _cache.Add(entityObject.Name, entityMetadataOverridex);
        //    //                                        dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                    }
        //    //                                }
        //    //                            }
        //    //                            else
        //    //                            {
        //    //                                var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
        //    //                                var savedOldFields = _cache[entityObject.Name].Fields.Values;
        //    //                                foreach (var v in savedOldFields)
        //    //                                {
        //    //                                    dictionaryFieldsMissing.Add(v.Name, v);
        //    //                                }
        //    //                                IFieldMetadata fieldMetadataMiss = fieldMetadataf;


        //    //                                var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
        //    //                                if (primaryKeyMetadata.FieldRefs != null)
        //    //                                {
        //    //                                    var FieldRefs = primaryKeyMetadata.FieldRefs;
        //    //                                    foreach (var v in FieldRefs)
        //    //                                    {
        //    //                                        dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
        //    //                                    }
        //    //                                }

        //    //                                var primaryKeyMetadataF = new PrimaryKeyMetadata();
        //    //                                primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
        //    //                                if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
        //    //                                {
        //    //                                    dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
        //    //                                }
        //    //                                primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
        //    //                                if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
        //    //                                {
        //    //                                    dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                }

        //    //                                entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
        //    //                                primaryKeyMetadata = primaryKeyMetadataF;
        //    //                                if (entityMetadataOverride != null)
        //    //                                {
        //    //                                    var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
        //    //                                    entityMetadataOverridex.Fields = dictionaryFieldsMissing;
        //    //                                    _cache.Remove(entityObject.Name);
        //    //                                    _cache.Add(entityObject.Name, entityMetadataOverridex);
        //    //                                    dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                }

        //    //                            }
        //    //                        }
        //    //                    }
        //    //                }
        //    //                valueprimaryKeyFieldMappedMetadata.MatchWith = (PrimaryKeyMappedMatchWith)Enum.Parse(typeof(PrimaryKeyMappedMatchWith), ch.MatchWith.ToString());
        //    //                dictionary.Add(ch.Value, valueprimaryKeyFieldMappedMetadata);
        //    //            }
        //    //            else if (ch.MatchWith == Metadata.PrimaryKeyMappedMatchWith.Field)
        //    //            {
        //    //                var fieldprimaryKeyFieldMappedMetadata = new FieldPrimaryKeyFieldMappedMetadata();
        //    //                IFieldMetadata fieldMetadataf = new FieldMetadata();
        //    //                if (fieldMetadata.RefEntity.Fields.TryGetValue(ch.KeyFieldName, out fieldMetadataf))
        //    //                {
        //    //                    fieldprimaryKeyFieldMappedMetadata.KeyField = fieldMetadataf;
        //    //                    (fieldprimaryKeyFieldMappedMetadata as FieldPrimaryKeyFieldMappedMetadata).KeyField = fieldMetadataf;
        //    //                    IFieldMetadata fieldEntityMetadataf = new FieldMetadata();
        //    //                    if (_cache.ContainsKey(entityObject.Name))
        //    //                    {
        //    //                        if (_cache[entityObject.Name].Fields.TryGetValue(ch.Value, out fieldEntityMetadataf))
        //    //                        {
        //    //                            (fieldprimaryKeyFieldMappedMetadata as FieldPrimaryKeyFieldMappedMetadata).EntityField = fieldEntityMetadataf;
        //    //                        }
        //    //                        else
        //    //                        {
        //    //                            var entityMetadataOverride = _cache[entityObject.Name] as EntityMetadata;
        //    //                            if (entityMetadataOverride.BaseEntity != null)
        //    //                            {
        //    //                                var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
        //    //                                var savedOldFields = _cache[entityObject.Name].Fields.Values;
        //    //                                foreach (var v in savedOldFields)
        //    //                                {
        //    //                                    dictionaryFieldsMissing.Add(v.Name, v);
        //    //                                }
        //    //                                IFieldMetadata fieldEntityMetadataBase = new FieldMetadata();
        //    //                                if (entityMetadataOverride.BaseEntity.Fields.TryGetValue(ch.Value, out fieldEntityMetadataBase))
        //    //                                {

        //    //                                    var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
        //    //                                    if (primaryKeyMetadata.FieldRefs != null)
        //    //                                    {
        //    //                                        var FieldRefs = primaryKeyMetadata.FieldRefs;
        //    //                                        foreach (var v in FieldRefs)
        //    //                                        {
        //    //                                            dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
        //    //                                        }
        //    //                                    }

        //    //                                    var primaryKeyMetadataF = new PrimaryKeyMetadata();
        //    //                                    primaryKeyMetadataF.Clustered = entityMetadataOverride.BaseEntity.PrimaryKey.Clustered;
        //    //                                    if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
        //    //                                    {
        //    //                                        dictionaryPrimaryKeyFieldRefMetadata.Add(ch.Value, entityMetadataOverride.BaseEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.Value).Value);
        //    //                                    }
        //    //                                    primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
        //    //                                    if (!dictionaryFieldsMissing.ContainsKey(fieldEntityMetadataBase.Name))
        //    //                                    {
        //    //                                        dictionaryFieldsMissing.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
        //    //                                    }

        //    //                                    entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
        //    //                                    primaryKeyMetadata = primaryKeyMetadataF;
        //    //                                    if (entityMetadataOverride != null)
        //    //                                    {
        //    //                                        var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
        //    //                                        entityMetadataOverridex.Fields = dictionaryFieldsMissing;
        //    //                                        _cache.Remove(entityObject.Name);
        //    //                                        _cache.Add(entityObject.Name, entityMetadataOverridex);
        //    //                                        dictionaryFields.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
        //    //                                    }
        //    //                                }
        //    //                                else
        //    //                                {
        //    //                                    dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
        //    //                                    savedOldFields = _cache[entityObject.Name].Fields.Values;
        //    //                                    foreach (var v in savedOldFields)
        //    //                                    {
        //    //                                        dictionaryFieldsMissing.Add(v.Name, v);
        //    //                                    }
        //    //                                    IFieldMetadata fieldMetadataMiss = fieldMetadataf;

        //    //                                    var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
        //    //                                    if (primaryKeyMetadata.FieldRefs != null)
        //    //                                    {
        //    //                                        var FieldRefs = primaryKeyMetadata.FieldRefs;
        //    //                                        foreach (var v in FieldRefs)
        //    //                                        {
        //    //                                            dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
        //    //                                        }
        //    //                                    }

        //    //                                    var primaryKeyMetadataF = new PrimaryKeyMetadata();
        //    //                                    primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
        //    //                                    if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
        //    //                                    {
        //    //                                        dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
        //    //                                    }
        //    //                                    primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
        //    //                                    if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
        //    //                                    {
        //    //                                        dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                    }

        //    //                                    entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
        //    //                                    primaryKeyMetadata = primaryKeyMetadataF;
        //    //                                    if (entityMetadataOverride != null)
        //    //                                    {
        //    //                                        var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
        //    //                                        entityMetadataOverridex.Fields = dictionaryFieldsMissing;
        //    //                                        _cache.Remove(entityObject.Name);
        //    //                                        _cache.Add(entityObject.Name, entityMetadataOverridex);
        //    //                                        dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                    }
        //    //                                }
        //    //                            }
        //    //                            else
        //    //                            {
        //    //                                var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
        //    //                                var savedOldFields = _cache[entityObject.Name].Fields.Values;
        //    //                                foreach (var v in savedOldFields)
        //    //                                {
        //    //                                    dictionaryFieldsMissing.Add(v.Name, v);
        //    //                                }
        //    //                                IFieldMetadata fieldMetadataMiss = fieldMetadataf;


        //    //                                var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
        //    //                                if (primaryKeyMetadata.FieldRefs != null)
        //    //                                {
        //    //                                    var FieldRefs = primaryKeyMetadata.FieldRefs;
        //    //                                    foreach (var v in FieldRefs)
        //    //                                    {
        //    //                                        dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
        //    //                                    }
        //    //                                }

        //    //                                var primaryKeyMetadataF = new PrimaryKeyMetadata();
        //    //                                primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
        //    //                                if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
        //    //                                {
        //    //                                    dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
        //    //                                }
        //    //                                primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
        //    //                                if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
        //    //                                {
        //    //                                    dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                }

        //    //                                entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
        //    //                                primaryKeyMetadata = primaryKeyMetadataF;
        //    //                                if (entityMetadataOverride != null)
        //    //                                {
        //    //                                    var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
        //    //                                    entityMetadataOverridex.Fields = dictionaryFieldsMissing;
        //    //                                    _cache.Remove(entityObject.Name);
        //    //                                    _cache.Add(entityObject.Name, entityMetadataOverridex);
        //    //                                    dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                }
        //    //                            }
        //    //                        }
        //    //                    }

        //    //                }
        //    //                fieldprimaryKeyFieldMappedMetadata.MatchWith = (PrimaryKeyMappedMatchWith)Enum.Parse(typeof(PrimaryKeyMappedMatchWith), ch.MatchWith.ToString());
        //    //                dictionary.Add(ch.Value, fieldprimaryKeyFieldMappedMetadata);

        //    //            }
        //    //            else if (ch.MatchWith == Metadata.PrimaryKeyMappedMatchWith.SourceName)
        //    //            {
        //    //                var sourceprimaryKeyFieldMappedMetadata = new SourceNamePrimaryKeyFieldMappedMetadata();
        //    //                IFieldMetadata fieldMetadataf = new FieldMetadata();
        //    //                if (fieldMetadata.RefEntity.Fields.TryGetValue(ch.KeyFieldName, out fieldMetadataf))
        //    //                {
        //    //                    sourceprimaryKeyFieldMappedMetadata.KeyField = fieldMetadataf;
        //    //                    (sourceprimaryKeyFieldMappedMetadata as SourceNamePrimaryKeyFieldMappedMetadata).SourceName = ch.Value;
        //    //                    IFieldMetadata fieldEntityMetadataf = new FieldMetadata();
        //    //                    if (_cache.ContainsKey(entityObject.Name))
        //    //                    {
        //    //                        fieldEntityMetadataf = _cache[entityObject.Name].Fields.ToList().Find(z => z.Value.SourceName == ch.Value).Value;
        //    //                        if (fieldEntityMetadataf == null)
        //    //                        {
        //    //                            var entityMetadataOverride = _cache[entityObject.Name] as EntityMetadata;
        //    //                            if (entityMetadataOverride.BaseEntity != null)
        //    //                            {
        //    //                                var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
        //    //                                var savedOldFields = _cache[entityObject.Name].Fields.Values;
        //    //                                foreach (var v in savedOldFields)
        //    //                                {
        //    //                                    dictionaryFieldsMissing.Add(v.Name, v);
        //    //                                }
        //    //                                IFieldMetadata fieldEntityMetadataBase = new FieldMetadata();
        //    //                                fieldEntityMetadataBase = entityMetadataOverride.BaseEntity.Fields.ToList().Find(z => z.Value.SourceName == ch.Value).Value;
        //    //                                if (fieldEntityMetadataBase != null)
        //    //                                {
        //    //                                    var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
        //    //                                    if (primaryKeyMetadata.FieldRefs != null)
        //    //                                    {
        //    //                                        var FieldRefs = primaryKeyMetadata.FieldRefs;
        //    //                                        foreach (var v in FieldRefs)
        //    //                                        {
        //    //                                            dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
        //    //                                        }
        //    //                                    }

        //    //                                    var primaryKeyMetadataF = new PrimaryKeyMetadata();
        //    //                                    primaryKeyMetadataF.Clustered = entityMetadataOverride.BaseEntity.PrimaryKey.Clustered;
        //    //                                    if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
        //    //                                    {
        //    //                                        dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, entityMetadataOverride.BaseEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
        //    //                                    }
        //    //                                    primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
        //    //                                    primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
        //    //                                    if (!dictionaryFieldsMissing.ContainsKey(fieldEntityMetadataBase.Name))
        //    //                                    {
        //    //                                        dictionaryFieldsMissing.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
        //    //                                    }

        //    //                                    entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
        //    //                                    primaryKeyMetadata = primaryKeyMetadataF;
        //    //                                    if (entityMetadataOverride != null)
        //    //                                    {
        //    //                                        var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
        //    //                                        entityMetadataOverridex.Fields = dictionaryFieldsMissing;
        //    //                                        _cache.Remove(entityObject.Name);
        //    //                                        _cache.Add(entityObject.Name, entityMetadataOverridex);
        //    //                                        dictionaryFields.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
        //    //                                    }
        //    //                                }
        //    //                                else
        //    //                                {
        //    //                                    dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
        //    //                                    savedOldFields = _cache[entityObject.Name].Fields.Values;
        //    //                                    foreach (var v in savedOldFields)
        //    //                                    {
        //    //                                        dictionaryFieldsMissing.Add(v.Name, v);
        //    //                                    }

        //    //                                    if (fieldMetadataf != null)
        //    //                                    {
        //    //                                        var fieldMetadataMiss = fieldMetadataf as FieldMetadata;
        //    //                                        fieldMetadataMiss.SourceName = ch.Value;

        //    //                                        var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
        //    //                                        if (primaryKeyMetadata.FieldRefs != null)
        //    //                                        {
        //    //                                            var FieldRefs = primaryKeyMetadata.FieldRefs;
        //    //                                            foreach (var v in FieldRefs)
        //    //                                            {
        //    //                                                dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
        //    //                                            }
        //    //                                        }

        //    //                                        var primaryKeyMetadataF = new PrimaryKeyMetadata();
        //    //                                        primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
        //    //                                        if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
        //    //                                        {
        //    //                                            dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
        //    //                                        }
        //    //                                        primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
        //    //                                        if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
        //    //                                        {
        //    //                                            dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                        }


        //    //                                        entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
        //    //                                        primaryKeyMetadata = primaryKeyMetadataF;
        //    //                                        if (entityMetadataOverride != null)
        //    //                                        {
        //    //                                            var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
        //    //                                            entityMetadataOverridex.Fields = dictionaryFieldsMissing;
        //    //                                            _cache.Remove(entityObject.Name);
        //    //                                            _cache.Add(entityObject.Name, entityMetadataOverridex);
        //    //                                            dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                        }
        //    //                                    }
        //    //                                }
        //    //                            }
        //    //                            else
        //    //                            {
        //    //                                var dictionaryFieldsMissing = new Dictionary<string, IFieldMetadata>();
        //    //                                var savedOldFields = _cache[entityObject.Name].Fields.Values;
        //    //                                foreach (var v in savedOldFields)
        //    //                                {
        //    //                                    dictionaryFieldsMissing.Add(v.Name, v);
        //    //                                }
        //    //                                IFieldMetadata fieldMetadataMiss = fieldMetadataf;

        //    //                                var dictionaryPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
        //    //                                if (primaryKeyMetadata.FieldRefs != null)
        //    //                                {
        //    //                                    var FieldRefs = primaryKeyMetadata.FieldRefs;
        //    //                                    foreach (var v in FieldRefs)
        //    //                                    {
        //    //                                        dictionaryPrimaryKeyFieldRefMetadata.Add(v.Key, v.Value);
        //    //                                    }
        //    //                                }

        //    //                                var primaryKeyMetadataF = new PrimaryKeyMetadata();
        //    //                                primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
        //    //                                if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
        //    //                                {
        //    //                                    dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
        //    //                                }
        //    //                                primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
        //    //                                if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
        //    //                                {
        //    //                                    dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                }

        //    //                                entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
        //    //                                primaryKeyMetadata = primaryKeyMetadataF;
        //    //                                if (entityMetadataOverride != null)
        //    //                                {
        //    //                                    var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
        //    //                                    entityMetadataOverridex.Fields = dictionaryFieldsMissing;
        //    //                                    _cache.Remove(entityObject.Name);
        //    //                                    _cache.Add(entityObject.Name, entityMetadataOverridex);
        //    //                                    dictionaryFields.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
        //    //                                }
        //    //                            }
        //    //                        }
        //    //                    }
        //    //                }
        //    //                sourceprimaryKeyFieldMappedMetadata.MatchWith = (PrimaryKeyMappedMatchWith)Enum.Parse(typeof(PrimaryKeyMappedMatchWith), ch.MatchWith.ToString());
        //    //                dictionary.Add(ch.Value, sourceprimaryKeyFieldMappedMetadata);
        //    //            }
        //    //        }
        //    //    }
        //    //    primaryKeyFieldMappingMetadata.Fields = dictionary;
        //    //    fieldMetadata.Mapping = primaryKeyFieldMappingMetadata;
        //    //}
        //}

        /// <summary>
        /// Преобразование данных из типа FieldDef в RelationFieldMetadata для сущности типа Relation
        /// </summary>
        /// <param name="fieldMetadata"></param>
        /// <param name="fieldDef"></param>
        //private void BuildConditionForRelation(ref RelationFieldMetadata fieldMetadata, Metadata.FieldDef fieldDef)
        //{
        //    fieldMetadata.RelationCondition = new DataModels.DataConstraint.ComplexCondition();
        //    var complexConditionDataConstraint = ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition));
        //    complexConditionDataConstraint.Operator = (DataModels.DataConstraint.LogicalOperator)Enum.Parse(typeof(DataModels.DataConstraint.LogicalOperator), fieldDef.RelationCondition.ItemElementName.ToString());
        //    if (fieldDef.RelationCondition.Item is Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef conditionExpressionDef)
        //    {
        //        object[] items = conditionExpressionDef.Items;
        //        if (items != null)
        //        {
        //            complexConditionDataConstraint.Conditions = new DataModels.DataConstraint.ComplexCondition[items.Length];
        //            for (int k = 0; k < items.Length; k++)
        //            {
        //                complexConditionDataConstraint.Conditions[k] = new DataModels.DataConstraint.ComplexCondition();
        //                if (items[k] is Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef)
        //                {
        //                    complexConditionDataConstraint.Conditions[k] = new DataModels.DataConstraint.ComplexCondition();
        //                    var expr = items[k] as Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef;
        //                    if (expr != null)
        //                    {
        //                        if (expr.Items != null)
        //                        {
        //                            var conditions = ((DataModels.DataConstraint.ComplexCondition)complexConditionDataConstraint.Conditions[k]).Conditions;
        //                            conditions = new DataModels.DataConstraint.ConditionExpression[expr.Items.Length];
        //                            for (int z = 0; z < expr.Items.Length; z++)
        //                            {
        //                                conditions[z] = new DataModels.DataConstraint.ConditionExpression();
        //                                if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.TwoOperandsOperationDef)
        //                                {
        //                                    var condition = ((DataModels.DataConstraint.ConditionExpression)conditions[z]);
        //                                    condition.Operator = (DataModels.DataConstraint.ConditionOperator)Enum.Parse(typeof(DataModels.DataConstraint.ConditionOperator), expr.ItemsElementName[z].ToString());

        //                                    var twoOperans = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.TwoOperandsOperationDef);
        //                                    if (twoOperans != null)
        //                                    {
        //                                        if (twoOperans.Item is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
        //                                        {
        //                                            condition.LeftOperand = new DataModels.DataConstraint.ColumnOperand();
        //                                            string Name = (twoOperans.Item as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
        //                                            ((DataModels.DataConstraint.ColumnOperand)condition.LeftOperand).ColumnName = Name;
        //                                        }
        //                                        if (twoOperans.Item is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
        //                                        {
        //                                            condition.LeftOperand = new DataModels.DataConstraint.StringValueOperand();
        //                                            string Value = (twoOperans.Item as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
        //                                            ((DataModels.DataConstraint.StringValueOperand)(condition).LeftOperand).Value = Value;
        //                                        }

        //                                        if (twoOperans.Item1 is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
        //                                        {
        //                                            condition.RightOperand = new DataModels.DataConstraint.ColumnOperand();
        //                                            string Name = (twoOperans.Item1 as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
        //                                            ((DataModels.DataConstraint.ColumnOperand)condition.RightOperand).ColumnName = Name;

        //                                        }
        //                                        if (twoOperans.Item1 is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
        //                                        {
        //                                            condition.RightOperand = new DataModels.DataConstraint.StringValueOperand();
        //                                            string Value = (twoOperans.Item1 as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
        //                                            ((DataModels.DataConstraint.StringValueOperand)(condition).RightOperand).Value = Value;
        //                                        }
        //                                    }
        //                                }
        //                                else if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.OneOperandOperationDef)
        //                                {
        //                                    var condition = ((DataModels.DataConstraint.ConditionExpression)conditions[z]);
        //                                    condition.Operator = (DataModels.DataConstraint.ConditionOperator)Enum.Parse(typeof(DataModels.DataConstraint.ConditionOperator), expr.ItemsElementName[z].ToString());
        //                                    var oneOperand = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.OneOperandOperationDef);
        //                                    if (oneOperand != null)
        //                                    {

        //                                        if (oneOperand.Item is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
        //                                        {
        //                                            condition.LeftOperand = new DataModels.DataConstraint.ColumnOperand();
        //                                            string Name = (oneOperand.Item as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
        //                                            ((DataModels.DataConstraint.ColumnOperand)condition.LeftOperand).ColumnName = Name;

        //                                        }
        //                                        if (oneOperand.Item is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
        //                                        {
        //                                            condition.RightOperand = new DataModels.DataConstraint.StringValueOperand();
        //                                            string Value = (oneOperand.Item as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
        //                                            ((DataModels.DataConstraint.StringValueOperand)(condition).RightOperand).Value = Value;
        //                                        }
        //                                    }
        //                                }
        //                                else if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.InOperationDef)
        //                                {
        //                                    var condition = ((DataModels.DataConstraint.ConditionExpression)conditions[z]);
        //                                    condition.Operator = (DataModels.DataConstraint.ConditionOperator)Enum.Parse(typeof(DataModels.DataConstraint.ConditionOperator), expr.ItemsElementName[z].ToString());
        //                                    var oneOperand = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.InOperationDef);
        //                                    if (oneOperand != null)
        //                                    {
        //                                        if (oneOperand.Field is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
        //                                        {
        //                                            condition.LeftOperand = new DataModels.DataConstraint.ColumnOperand();
        //                                            string Name = (oneOperand.Field as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
        //                                            ((DataModels.DataConstraint.ColumnOperand)condition.LeftOperand).ColumnName = Name;
        //                                        }
        //                                        if (oneOperand.Values is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef[])
        //                                        {
        //                                            condition.RightOperand = new DataModels.DataConstraint.StringValuesOperand();
        //                                            List<string> Values = new List<string>();
        //                                            foreach (var cx in oneOperand.Values)
        //                                            {
        //                                                Values.Add(cx.Value);
        //                                            }
        //                                            ((DataModels.DataConstraint.StringValuesOperand)(condition).RightOperand).Values = Values.ToArray();
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    throw new NotImplementedException(Exceptions.NotSupportedOperation);
        //                                }
        //                            }
        //                            ((DataModels.DataConstraint.ComplexCondition)complexConditionDataConstraint.Conditions[k]).Conditions = conditions;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}



        /// <summary>
        /// Основной метод полученя методанных о сущности
        /// </summary>
        /// <param name="path">Путь к сущности. Может быть квалифицированным именем или относительным именем </param>
        /// <param name="relatedEntity">Сущность относительно которой идет запрос</param>
        /// <returns></returns>
        public IEntityMetadata GetEntityMetadata(string path, IEntityMetadata relatedEntity = null)
        {
            if (_entityMetadataCache.TryGetValue(path, out EntityMetadata entityMetadata))
            {
                return entityMetadata;
            }

            var entityPath = EntityPathDescriptor.EnsureEntityPath(_config, path, relatedEntity);

            if (_entityMetadataCache.TryGetValue(entityPath.QualifiedName, out entityMetadata))
            {
                return entityMetadata;
            }
            var filePath = entityPath.GetFilePath(_config);
            var entityMetadataDef = this.DeserializeEntityMetadata(filePath);
            entityMetadata = this.BuildEntityMetadataInstance(entityPath, entityMetadataDef);

            if (!_entityMetadataCache.TryAdd(entityMetadata.QualifiedName, entityMetadata))
            {
                if (!_entityMetadataCache.TryGetValue(entityPath.QualifiedName, out entityMetadata))
                {
                    throw new InvalidOperationException($"Failed to add the entity metadata information to cache with path '{entityPath}'");
                }
            }
            else
            {
                try
                {
                    // this is added me and we need to finish initialize
                    this.PadEntityMetada(entityMetadata, entityMetadataDef);
                }
                catch(Exception)
                {
                    
                    throw;
                }
            }

            // нужно доофрмить сущности - проинициализировать все ссылки на сущности во всех полях
            return entityMetadata;
        }

        private Metadata.EntityDef DeserializeEntityMetadata(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var serializer = new XmlSerializer(typeof(Metadata.EntityDef));
                if (!(serializer.Deserialize(reader) is Metadata.EntityDef entityDef))
                {
                    throw new InvalidOperationException($"Failed to deserialize entity metadata object from the file by path '{filePath}'");
                }
                return entityDef;
            }
        }

        private EntityMetadata BuildEntityMetadataInstance(EntityPathDescriptor entityPath, Metadata.EntityDef entityDef)
        {
            if (!entityPath.Name.Equals(entityDef.Name))
            {
                throw new InvalidOperationException($"The entity names do not match: expected name = '{entityPath.Name}', defined name = '{entityDef.Name}', file = '{entityPath.GetFilePath(_config)}'");
            }

            if (Metadata.EntityDefHelper.UsesBaseEntity(entityDef)
                && string.IsNullOrEmpty(entityDef.BaseEntity))
            {
                throw new InvalidOperationException($"The entity name (BaseEntity attribute) is not defined in the inheritance entity '{entityDef.Name}'");
            }

            var entityMetadata = new EntityMetadata
            {
                Name = entityPath.Name,
                QualifiedName = entityPath.QualifiedName,
                Namespace = (entityPath.Name.Length == entityPath.QualifiedName.Length) ? entityPath.QualifiedName : entityPath.QualifiedName.Substring(0, entityPath.QualifiedName.Length - entityPath.Name.Length - 1),
                Title = entityDef.Title,
                Desc = entityDef.Desc,
                Type = entityDef.Type.CopyTo<EntityType>()
            };

            entityMetadata.DataSource = this.BuildDataSourceMetadata(entityDef.DataSource);

            if (entityDef.Fields != null)
            {
                this.BuildFieldsMetadata(entityMetadata, entityDef);
            }
            

            entityMetadata.PrimaryKey = this.BuildPrimaryKeyMetadata(entityMetadata, entityDef);

            return entityMetadata;
        }

        private DataSourceMetadata BuildDataSourceMetadata(Metadata.DataSourceDef dataSourceDef)
        {
            if (dataSourceDef == null)
            {
                throw new ArgumentNullException(nameof(dataSourceDef));
            }

            var dataSourceMetadata = new DataSourceMetadata
            {
                Name = dataSourceDef.Name,
                Schema = dataSourceDef.Schema,
                Object = dataSourceDef.Object.CopyTo<DataSourceObject>(),
                Type = dataSourceDef.Type.CopyTo<DataSourceType>()
            };

            return dataSourceMetadata;
        }

        private PrimaryKeyMetadata BuildPrimaryKeyMetadata(EntityMetadata entityMetadata, Metadata.EntityDef entityDef)
        {
            if (entityDef.PrimaryKey == null)
            {
                return null;
            }

            if (entityMetadata.Type != EntityType.Abstruct && entityMetadata.UsesBaseEntityPrimaryKey())
            {
                throw new InvalidOperationException($"Defined primary key for the entity '{entityMetadata.Name}' with uses inheritance");
            }

            var primaryKeyDef = entityDef.PrimaryKey;

            var primaryKeyMetadata = new PrimaryKeyMetadata(entityMetadata)
            {
                Clustered = primaryKeyDef.Clustered
            };

            var fieldRefs = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
            for (int i = 0; i < primaryKeyDef.FieldRef.Length; i++)
            {
                var fieldRefDef = primaryKeyDef.FieldRef[i];
                if (string.IsNullOrEmpty(fieldRefDef.Name))
                {
                    throw new InvalidOperationException($"Undefined field name of the primary key. The field index is #{i}");
                }
                if (fieldRefs.ContainsKey(fieldRefDef.Name))
                {
                    throw new InvalidOperationException($"Duplicate primary key field with name '{fieldRefDef.Name}'");
                }
                if (!entityMetadata.Fields.TryGetValue(fieldRefDef.Name, out IFieldMetadata fieldMetadata))
                {
                    throw new InvalidOperationException($"Not found field metadata by name '{fieldRefDef.Name}' for assigning to the entity primary key");
                }
                if (fieldMetadata.SourceType != FieldSourceType.Column
                    && fieldMetadata.SourceType != FieldSourceType.Reference)
                {
                    throw new InvalidOperationException($"Cannot use field with source type '{fieldMetadata.SourceType}' for primary key. The field name is '{fieldMetadata.Name}'");
                }

                var fieldRefMetadata = new PrimaryKeyFieldRefMetadata
                {
                    Field = fieldMetadata,
                    SortOrder = fieldRefDef.SortOrder.CopyTo<SortDirection>()
                };

                fieldRefs.Add(fieldRefDef.Name, fieldRefMetadata);
            }

            if (fieldRefs.Count == 0)
            {
                throw new InvalidOperationException($"Primary key fields are not defined.");
            }
            primaryKeyMetadata.FieldRefs = fieldRefs;
            return primaryKeyMetadata;
        }

        private void BuildFieldsMetadata(EntityMetadata entityMetadata, Metadata.EntityDef entityDef)
        {
            var fields = new Dictionary<string, IFieldMetadata>();
            var fieldDefs = entityDef.Fields;

            for (int i = 0; i < fieldDefs.Length; i++)
            {
                var fieldDef = fieldDefs[i];

                if (fieldDef.Name == null)
                {
                    throw new InvalidOperationException($"Undefined a name in the field metadata with index #{i}");
                }

                if (fields.TryGetValue(fieldDef.Name, out IFieldMetadata field))
                {
                    throw new InvalidOperationException($"Field duplication with name '{fieldDef.Name}'");
                }

                if (fieldDef.SourceType == Metadata.FieldSourceType.Column)
                {
                    field = this.BuildFieldMetadataAsColumn(entityMetadata, fieldDef);
                }
                else if (fieldDef.SourceType == Metadata.FieldSourceType.Reference)
                {
                    field = BuildFieldMetadataAsReference(entityMetadata, fieldDef);
                }
                else if (fieldDef.SourceType == Metadata.FieldSourceType.Extension)
                {
                    field = BuildFieldMetadataAsExtension(entityMetadata, fieldDef);
                }
                else if (fieldDef.SourceType == Metadata.FieldSourceType.Relation)
                {
                    field = BuildFieldMetadataAsRelation(entityMetadata, fieldDef);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported the source type '{fieldDef.SourceType}'");
                }

                fields.Add(field.Name, field);
                entityMetadata.AppendField(field);
            }
        }

        private RelationFieldMetadata BuildFieldMetadataAsRelation(EntityMetadata entityMetadata, Metadata.FieldDef fieldDef)
        {
            if (fieldDef.SourceName == null)
            {
                throw new InvalidOperationException($"Undefined a source name in the field metadata '{fieldDef.Name}'");
            }
            if (fieldDef.DataType != null)
            {
                throw new InvalidOperationException($"Defined a data type '{fieldDef.DataType}' in the relation field metadata '{fieldDef.Name}'");
            }
            if (fieldDef.Unit != null)
            {
                throw new InvalidOperationException($"Defined a unit '{fieldDef.Unit}' in the relation field metadata '{fieldDef.Name}'");
            }
            if (fieldDef.RelationCondition == null)
            {
                throw new InvalidOperationException($"Undefined a relation condition in the field metadata '{fieldDef.Name}'");
            }

            var fieldMetadata = new RelationFieldMetadata(entityMetadata, fieldDef.Name)
            {
                Title = fieldDef.Title,
                SourceName = fieldDef.SourceName,
                Required = fieldDef.Required,
                Desc = fieldDef.Desc
            };

            return fieldMetadata;
        }

        private ExtensionFieldMetadata BuildFieldMetadataAsExtension(EntityMetadata entityMetadata, Metadata.FieldDef fieldDef)
        {
            if (fieldDef.SourceName == null)
            {
                throw new InvalidOperationException($"Undefined a source name in the field metadata '{fieldDef.Name}'");
            }
            if (fieldDef.DataType != null)
            {
                throw new InvalidOperationException($"Defined a data type '{fieldDef.DataType}' in the extension field metadata '{fieldDef.Name}'");
            }
            if (fieldDef.Unit != null)
            {
                throw new InvalidOperationException($"Defined a unit '{fieldDef.Unit}' in the extension field metadata '{fieldDef.Name}'");
            }

            var fieldMetadata = new ExtensionFieldMetadata(entityMetadata, fieldDef.Name)
            {
                Title = fieldDef.Title,
                SourceName = fieldDef.SourceName,
                Required = fieldDef.Required,
                Desc = fieldDef.Desc
            };

            return fieldMetadata;
        }

        private ReferenceFieldMetadata BuildFieldMetadataAsReference(EntityMetadata entityMetadata, Metadata.FieldDef fieldDef)
        {
            if (fieldDef.SourceName == null)
            {
                throw new InvalidOperationException($"Undefined a source name in the field metadata '{fieldDef.Name}'");
            }
            if (fieldDef.DataType != null)
            {
                throw new InvalidOperationException($"Defined a data type '{fieldDef.DataType}' in the reference field metadata '{fieldDef.Name}'");
            }
            if (fieldDef.Unit != null)
            {
                throw new InvalidOperationException($"Defined a unit '{fieldDef.Unit}' in the reference field metadata '{fieldDef.Name}'");
            }

            var fieldMetadata = new ReferenceFieldMetadata(entityMetadata, fieldDef.Name)
            {
                Title = fieldDef.Title,
                Desc = fieldDef.Desc,
                SourceName = fieldDef.SourceName,
                Required = fieldDef.Required
            };

            //CheckFieldsForReference(ref fieldMetadata, fieldDef, entityObject, ref primaryKeyMetadata, ref fields);

            return fieldMetadata;
        }

        private ColumnFieldMetadata BuildFieldMetadataAsColumn(EntityMetadata entityMetadata, Metadata.FieldDef fieldDef)
        {
            if (fieldDef.SourceName == null)
            {
                throw new InvalidOperationException($"Undefined a source name in the field metadata '{fieldDef.Name}'");
            }
            if (fieldDef.DataType == null)
            {
                throw new InvalidOperationException($"Undefined a data type in the field metadata '{fieldDef.Name}'");
            }

            var fieldMetadata = new ColumnFieldMetadata(entityMetadata, fieldDef.Name)
            {
                DataType = this.GetDataTypeMetadata(fieldDef.DataType, entityMetadata.DataSource.Type),
                Title = fieldDef.Title,
                Desc = fieldDef.Desc,
                SourceName = fieldDef.SourceName,
                Required = fieldDef.Required,
            };

            if (fieldDef.Unit != null)
            {
                fieldMetadata.Unit = this.GetUnitMetadata(fieldDef.Unit);
            }

            return fieldMetadata;
        }

        private void PadEntityMetada(EntityMetadata entityMetadata, Metadata.EntityDef entityDef)
        {
            if (entityMetadata.UsesInheritance())
            {
                entityMetadata.BaseEntity = this.GetEntityMetadata(entityDef.BaseEntity);
            }

            this.PadFieldsMetadata(entityMetadata, entityDef);
        }

        private void PadFieldsMetadata(EntityMetadata entityMetadata, Metadata.EntityDef entityDef)
        {
            //  Для всех полей ссылок определить сущности и допрелелить мапинг ключа и условия соединенния
            if (entityDef.Fields != null)
            {
                for (int i = 0; i < entityDef.Fields.Length; i++)
                {
                    var fieldDef = entityDef.Fields[i];
                    var field = entityMetadata.Fields[fieldDef.Name];
                    switch (field.SourceType)
                    {
                        case FieldSourceType.Reference:
                            this.PadFieldMetadataAsReference(field as ReferenceFieldMetadata, fieldDef);
                            break;
                        case FieldSourceType.Extension:
                            this.PadFieldMetadataAsExtension(field as ExtensionFieldMetadata, fieldDef);
                            break;
                        case FieldSourceType.Relation:
                            this.PadFieldMetadataAsRelation(field as RelationFieldMetadata, fieldDef);
                            break;
                        default:
                            break;
                    }
                }
            }
            

            // кипируем в свою структуру все поля базового класса в случаи типа Simple или Role
            if (entityMetadata.Type == EntityType.Simple 
                || entityMetadata.Type == EntityType.Role)
            {
                var baseFields = entityMetadata.BaseEntity.DefineFieldsWithInherited();
                for (int i = 0; i < baseFields.Length; i++)
                {
                    entityMetadata.CopyField(baseFields[i]); 
                }

                // первичный ключ также копиреум
                if (entityMetadata.Type == EntityType.Simple && entityMetadata.PrimaryKey == null)
                {
                    var basePrimaryKey = entityMetadata.BaseEntity.DefinePrimaryKey();
                    if (basePrimaryKey != null)
                    {
                        var pkFiledRefs = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                        foreach (var item in basePrimaryKey.FieldRefs.Values)
                        {
                            var localFieldRef = new PrimaryKeyFieldRefMetadata
                            {
                                Field = entityMetadata.Fields[item.Field.Name],
                                SortOrder = item.SortOrder
                            };
                            pkFiledRefs.Add(localFieldRef.Field.Name, localFieldRef);
                        }
                        var localPrimaryKey = new PrimaryKeyMetadata(entityMetadata)
                        {
                            Clustered = basePrimaryKey.Clustered,
                            FieldRefs = pkFiledRefs
                        };

                        entityMetadata.PrimaryKey = localPrimaryKey;
                    }
                }
            }

        }

        private void PadFieldMetadataAsExtension(ExtensionFieldMetadata field, Metadata.FieldDef fieldDef)
        {
            var extensionEntity = this.GetEntityMetadata(fieldDef.SourceName, field.Entity);
            field.ExtensionEntity = extensionEntity;
        }

        private void PadFieldMetadataAsReference(ReferenceFieldMetadata field, Metadata.FieldDef fieldDef)
        {
            var refEntity = this.GetEntityMetadata(fieldDef.SourceName, field.Entity);
            field.RefEntity = refEntity;

            var keyMappingDef = fieldDef.PrimaryKeyMapping;
            if (keyMappingDef != null && keyMappingDef.Length > 0)
            {
                var keyMappedfields = new Dictionary<string, IPrimaryKeyFieldMappedMetadata>();

                for (int i = 0; i < keyMappingDef.Length; i++)
                {
                    var keyMappedDef = keyMappingDef[i];
                    if (keyMappedfields.ContainsKey(keyMappedDef.KeyFieldName))
                    {
                        throw new InvalidOperationException($"Attempt to reuse primary key field with name '{keyMappedDef.KeyFieldName}' when describing mapping");
                    }
                    if (!field.RefEntity.TryGetPrimaryKeyField(keyMappedDef.KeyFieldName, out IFieldMetadata keyField))
                    {
                        throw new InvalidOperationException($"Not found field for PK-mapping by path '{keyMappedDef.KeyFieldName}' in '{field.RefEntity}'");
                    }
                    IPrimaryKeyFieldMappedMetadata keyMapped = null;
                    switch (keyMappedDef.MatchWith)
                    {
                        case Metadata.PrimaryKeyMappedMatchWith.Value:
                            keyMapped = new ValuePrimaryKeyFieldMappedMetadata(keyField)
                            {
                                KeyField = keyField,
                                Value = ValueOperand.Create(keyField.DataType.CodeVarType, keyMappedDef.Value),
                            };
                            break;
                        case Metadata.PrimaryKeyMappedMatchWith.Field:
                            if (!field.Entity.TryGetLocalField(keyMappedDef.Value, out IFieldMetadata localField))
                            {
                                throw new InvalidOperationException($"Not found local field for PK-mapping by path '{keyMappedDef.Value}'");
                            }
                            keyMapped = new FieldPrimaryKeyFieldMappedMetadata(keyField)
                            {

                                EntityField = localField
                            };
                            break;
                        case Metadata.PrimaryKeyMappedMatchWith.SourceName:
                            keyMapped = new SourceNamePrimaryKeyFieldMappedMetadata(keyField)
                            {
                                SourceName = keyMappedDef.Value
                            };
                            break;
                        default:
                            throw new InvalidOperationException($"Unsupported the primary Key mapped Match {keyMappedDef.MatchWith}");
                    }
                    keyMappedfields.Add(keyMapped.KeyField.Name, keyMapped);
                }
                field.Mapping = new PrimaryKeyMappingMetadata
                {
                    Fields = keyMappedfields
                };
            }
        }

        private void PadFieldMetadataAsRelation(RelationFieldMetadata field, Metadata.FieldDef fieldDef)
        {
            var relatedEntity = this.GetEntityMetadata(fieldDef.SourceName, field.Entity);
            field.RelatedEntity = relatedEntity;
            field.JoinType = fieldDef.RelationCondition.JoinType.CopyTo<RelationJoinType>();
            field.RelationCondition = this.BuildFieldRelationCondition(field, fieldDef.RelationCondition);

            //BuildConditionForRelation(ref fieldMetadata, fieldDef);
        }

        private Condition BuildFieldRelationCondition(RelationFieldMetadata field, Metadata.RelationConditionDef conditionDef)
        {
            if (conditionDef.Item == null)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }
            return this.ParseConditionExpression(field, conditionDef.Item, conditionDef.ItemElementName.CopyTo<Metadata.ItemsChoiceType>());
        }

        private Condition ParseConditionExpression(RelationFieldMetadata field, object expression, Metadata.ItemsChoiceType type)
        {
            var condition = default(Condition);

            switch (type)
            {
                case Metadata.ItemsChoiceType.And:
                    condition = this.ParseConditionExpressionAsComplex(field, expression as Metadata.ConditionExpressionDef, LogicalOperator.And);
                    break;
                case Metadata.ItemsChoiceType.Or:
                    condition = this.ParseConditionExpressionAsComplex(field, expression as Metadata.ConditionExpressionDef, LogicalOperator.Or);
                    break;
                case Metadata.ItemsChoiceType.BeginWith:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.BeginWith);
                    break;
                case Metadata.ItemsChoiceType.Between:
                    condition = this.ParseConditionExpressionAsBetweenOperation(field, expression as Metadata.BetweenOperationDef, false);
                    break;
                case Metadata.ItemsChoiceType.Contains:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.Contains);
                    break;
                case Metadata.ItemsChoiceType.EndWith:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.EndWith);
                    break;
                case Metadata.ItemsChoiceType.Equal:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.Equal);
                    break;
                case Metadata.ItemsChoiceType.GreaterEqual:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.GreaterEqual);
                    break;
                case Metadata.ItemsChoiceType.GreaterThan:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.GreaterThan);
                    break;
                case Metadata.ItemsChoiceType.In:
                    condition = this.ParseConditionExpressionAsInOperation(field, expression as Metadata.InOperationDef, false);
                    break;
                case Metadata.ItemsChoiceType.IsNotNull:
                    condition = this.ParseConditionExpressionAsOneOperand(field, expression as Metadata.OneOperandOperationDef, ConditionOperator.IsNotNull);
                    break;
                case Metadata.ItemsChoiceType.IsNull:
                    condition = this.ParseConditionExpressionAsOneOperand(field, expression as Metadata.OneOperandOperationDef, ConditionOperator.IsNull);
                    break;
                case Metadata.ItemsChoiceType.LessEqual:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.LessEqual);
                    break;
                case Metadata.ItemsChoiceType.LessThan:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.LessThan);
                    break;
                case Metadata.ItemsChoiceType.Like:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.Like);
                    break;
                case Metadata.ItemsChoiceType.NotBeginWith:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.NotBeginWith);
                    break;
                case Metadata.ItemsChoiceType.NotBetween:
                    condition = this.ParseConditionExpressionAsBetweenOperation(field, expression as Metadata.BetweenOperationDef, true);
                    break;
                case Metadata.ItemsChoiceType.NotContains:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.NotContains);
                    break;
                case Metadata.ItemsChoiceType.NotEndWith:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.NotEndWith);
                    break;
                case Metadata.ItemsChoiceType.NotEqual:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.NotEqual);
                    break;
                case Metadata.ItemsChoiceType.NotIn:
                    condition = this.ParseConditionExpressionAsInOperation(field, expression as Metadata.InOperationDef, true);
                    break;
                case Metadata.ItemsChoiceType.NotLike:
                    condition = this.ParseConditionExpressionAsTwoOperands(field, expression as Metadata.TwoOperandsOperationDef, ConditionOperator.NotLike);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported condition expression type with name '{type}'");
            }
            return condition;
        }

        private ConditionExpression ParseConditionExpressionAsInOperation(RelationFieldMetadata field, Metadata.InOperationDef conditionDef, bool isNot = false)
        {
            if (conditionDef == null)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }
            if (conditionDef.Field == null || conditionDef.Values == null || conditionDef.Values.Length == 0)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}: one or more of the In operands not define");
            }
            var condition = new ConditionExpression
            {
                Operator = !isNot ? ConditionOperator.In : ConditionOperator.NotIn,
                LeftOperand = this.ParseOperandAsField(field, conditionDef.Field),
                RightOperand = this.ParseStringValuesOperand(field, conditionDef.Values.Select(v => v.Value).ToArray())
            };

            this.CastTypeConditionExpression(field, condition);

            return condition;
        }

        private ConditionExpression ParseConditionExpressionAsBetweenOperation(RelationFieldMetadata field, Metadata.BetweenOperationDef conditionDef, bool isNot = false)
        {
            if (conditionDef == null)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }
            if (conditionDef.Item == null || conditionDef.Item1 == null || conditionDef.Item2 == null)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}: one or more of the between operands not define");
            }
            var condition = new ConditionExpression
            {
                Operator = !isNot ? ConditionOperator.Between : ConditionOperator.NotBetween,
                LeftOperand = this.ParseOperand(field, conditionDef.Item),
                RightOperand = this.ParseStringValuesOperand(field, new object[] { conditionDef.Item1, conditionDef.Item2 })
            };

            this.CastTypeConditionExpression(field, condition);

            return condition;
        }

        private ConditionExpression ParseConditionExpressionAsTwoOperands(RelationFieldMetadata field, Metadata.TwoOperandsOperationDef conditionDef, ConditionOperator conditionOperator)
        {
            if (conditionDef == null)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }
            if (conditionDef.Item == null || conditionDef.Item1 == null)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }

            var condition = new ConditionExpression
            {
                Operator = conditionOperator,
                LeftOperand = this.ParseOperand(field, conditionDef.Item),
                RightOperand = this.ParseOperand(field, conditionDef.Item1)
            };

            this.CastTypeConditionExpression(field, condition);

            return condition;
        }

        private void CastTypeConditionExpression(RelationFieldMetadata field, ConditionExpression expression)
        {
            var dataType = DataModels.DataType.Undefined;
            if (expression.LeftOperand.Type == OperandType.Column)
            {
                var column = expression.LeftOperand as ColumnOperand;
                if (field.Entity.TryGetEndFieldByPath(column.ColumnName, out IFieldMetadata endField))
                {
                    dataType = endField.DataType.CodeVarType;
                }
            }
            else if (expression.RightOperand.Type == OperandType.Column)
            {
                var column = expression.RightOperand as ColumnOperand;
                if (field.Entity.TryGetEndFieldByPath(column.ColumnName, out IFieldMetadata endField))
                {
                    dataType = endField.DataType.CodeVarType;
                }
            }

            if (dataType == DataModels.DataType.Undefined)
            {
                return;
            }

            if (expression.LeftOperand.Type == OperandType.Value)
            {
                expression.LeftOperand = this.CastTypeValueOperand(dataType, expression.LeftOperand as StringValueOperand);
            }
            else if (expression.LeftOperand.Type == OperandType.Values)
            {
                expression.LeftOperand = this.CastTypeValuesOperand(dataType, expression.LeftOperand as StringValuesOperand);
            }

            if (expression.RightOperand.Type == OperandType.Value)
            {
                expression.RightOperand = this.CastTypeValueOperand(dataType, expression.RightOperand as StringValueOperand);
            }
            else if (expression.RightOperand.Type == OperandType.Values)
            {
                expression.RightOperand = this.CastTypeValuesOperand(dataType, expression.RightOperand as StringValuesOperand);
            }
        }

        private ValueOperand CastTypeValueOperand(DataModels.DataType dataType, StringValueOperand from)
        {
            return ValueOperand.Create(dataType, from.Value);
        }

        private ValuesOperand CastTypeValuesOperand(DataModels.DataType dataType, StringValuesOperand from)
        {
            return ValuesOperand.Create(dataType, from.Values);
        }

        private ConditionExpression ParseConditionExpressionAsOneOperand(RelationFieldMetadata field, Metadata.OneOperandOperationDef conditionDef, ConditionOperator conditionOperator)
        {
            if (conditionDef == null)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }
            if (conditionDef.Item == null)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }

            var condition = new ConditionExpression
            {
                Operator = conditionOperator,
                LeftOperand = this.ParseOperand(field, conditionDef.Item)
            };

            this.CastTypeConditionExpression(field, condition);

            return condition;
        }

        private StringValuesOperand ParseStringValuesOperand(RelationFieldMetadata field, object[] operandDefs)
        {
            if (operandDefs == null)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }
            if (operandDefs.Length == 0)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }

            var operand = new StringValuesOperand
            {
                Values = new string[operandDefs.Length]
            };

            for (int i = 0; i < operandDefs.Length; i++)
            {
                var operandDef = operandDefs[i] as Metadata.ValueOperandDef;
                if (operandDef == null)
                {
                    throw new InvalidOperationException($"Incorrect relation condition {field.Name}: unsupported between operand type");
                }
                operand.Values[i] = operandDef.Value;
            }

            return operand;
        }

        private Operand ParseOperand(RelationFieldMetadata field, object operandDef)
        {
            if (operandDef is Metadata.FieldOperandDef operandAsFieldDef)
            {
                return ParseOperandAsField(field, operandAsFieldDef);
            }

            if (operandDef is Metadata.ValueOperandDef operandAsValueDef)
            {
                return ParseOperandAsStringValue(field, operandAsValueDef);
            }

            throw new InvalidOperationException($"Unsupported operand definition type '{operandDef.GetType().AssemblyQualifiedName}'");
        }

        private StringValueOperand ParseOperandAsStringValue(RelationFieldMetadata field, Metadata.ValueOperandDef operandAsValueDef)
        {
            var operand = new StringValueOperand
            {
                Value = operandAsValueDef.Value
            };

            return operand;
        }

        private Operand ParseOperandAsField(RelationFieldMetadata field, Metadata.FieldOperandDef operandAsFieldDef)
        {
            if (string.IsNullOrEmpty(operandAsFieldDef.Name))
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}: undefined field name");
            }
            if (!field.Entity.TryGetLocalFieldByPath(operandAsFieldDef.Name, out IFieldMetadata fieldMetadata))
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}: not found field with name '{operandAsFieldDef.Name}'");
            }
            var operand = new ColumnOperand
            {
                ColumnName = operandAsFieldDef.Name
            };

            return operand;
        }

        private ComplexCondition ParseConditionExpressionAsComplex(RelationFieldMetadata field, Metadata.ConditionExpressionDef conditionDef, LogicalOperator logicalOperator)
        {
            if (conditionDef == null)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }
            if (conditionDef.Items == null || conditionDef.Items.Length == 0)
            {
                throw new InvalidOperationException($"Incorrect relation condition {field.Name}");
            }

            var condition = new ComplexCondition
            {
                Operator = logicalOperator,
                Conditions = new Condition[conditionDef.Items.Length]
            };

            for (int i = 0; i < conditionDef.Items.Length; i++)
            {
                var item = conditionDef.Items[i];
                var itemType = conditionDef.ItemsElementName[i];
                condition.Conditions[i] = this.ParseConditionExpression(field, item, itemType);
            }

            return condition;
        }

        public object CreatePrimaryKeyInstance(IEntityMetadata entity)
        {
            var primaryKey = entity.DefinePrimaryKey();
            if (primaryKey == null)
            {
                return null;
            }
            //var pkTypeName = $"{_config.Namespace}.I{entity.Name}_PK, {_config.Assembly}";
            var pkTypeName = $"{entity.Namespace}.I{entity.Name}_PK, {_config.Assembly}";
            var pkType = Type.GetType(pkTypeName);
            if (pkType == null)
            {
                throw new InvalidOperationException($"Cann't found PrimaryKey Intreface by name '{pkTypeName}'");
            }

            if (!_primaryKeyTypes.TryGetValue(pkType, out Type proxyType))
            {
                proxyType = GenerateProxyType(pkType);
                if (!_primaryKeyTypes.TryAdd(pkType, proxyType))
                {
                    if (!_primaryKeyTypes.TryGetValue(pkType, out proxyType))
                    {
                        throw new InvalidOperationException($"Cann't append PrimaryKey Intreface Proxy to cache by name '{pkTypeName}'");
                    }
                }
            }

            var insatnce = Activator.CreateInstance(proxyType);
            return insatnce;
        }

		public Type GetPrimaryKeyInstanceType(IEntityMetadata entity)
		{
			var primaryKey = entity.DefinePrimaryKey();
			if (primaryKey == null)
			{
				return null;
			}
			//var pkTypeName = $"{_config.Namespace}.I{entity.Name}_PK, {_config.Assembly}";
			var pkTypeName = $"{entity.Namespace}.I{entity.Name}_PK, {_config.Assembly}";
			var pkType = Type.GetType(pkTypeName);
			if (pkType == null)
			{
				throw new InvalidOperationException($"Cann't found PrimaryKey Intreface by name '{pkTypeName}'");
			}

			return pkType;
		}

		static Type GenerateProxyType(Type baseInterface)
        {
            var ns = baseInterface.Namespace;
            var name = baseInterface.Name.Substring(1, baseInterface.Name.Length - 1);

            var an = baseInterface.Assembly.GetName();

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName($"{an.Name}.EntitiesProxy, Version=1.0.0.1"),
                RE.AssemblyBuilderAccess.RunAndSave);

            var dynaminModule = assemblyBuilder.DefineDynamicModule(
                $"{an.Name}.EntitiesProxy.Module",
                $"{an.Name}.EntitiesProxy.dll");

            var proxyTypeBuilder = dynaminModule.DefineType(
                $"{ns}.{name}_Proxy",
                TypeAttributes.BeforeFieldInit | TypeAttributes.Public, typeof(object), new Type[] { baseInterface });

            // генерируем переменные под свойства
            var propertiesInfo = baseInterface.GetPropertiesWithInherited();
            for (int i = 0; i < propertiesInfo.Length; i++)
            {
                var propertyInfo = propertiesInfo[i];
                PropertyEmitter(proxyTypeBuilder, propertyInfo.Name, propertyInfo.PropertyType);
            }

            // генерируем конструктор по умолчанию
            var constructor = proxyTypeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard, new Type[] { });

            var ctorIL = constructor.GetILGenerator();

            var objectType = typeof(object);
            var objectCtor = objectType.GetConstructor(new Type[0]);

            ctorIL.Emit(RE.OpCodes.Ldarg_0);
            ctorIL.Emit(RE.OpCodes.Call, objectCtor);
            ctorIL.Emit(RE.OpCodes.Ret);


            var proxyType = proxyTypeBuilder.CreateType();
            //assemblyBuilder.Save($"{an.Name}.EntitiesProxy.dll");

            return proxyType;
        }

        static void PropertyEmitter(RE.TypeBuilder typeBuilder, string name, Type propertyType)
        {
            var ci = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute).GetConstructor(new Type[] { });
            var attrBuilder = new RE.CustomAttributeBuilder(ci, new object[0]);

            var fieldBuilder = typeBuilder.DefineField(String.Format("<{0}>k__BackingField", name), propertyType, FieldAttributes.Private);
            fieldBuilder.SetCustomAttribute(attrBuilder);

            var methodAttrs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

            var getterBuilder = typeBuilder.DefineMethod(String.Format("get_{0}", name), methodAttrs, propertyType, Type.EmptyTypes);
            getterBuilder.SetCustomAttribute(attrBuilder);
            var getterIl = getterBuilder.GetILGenerator();
            getterIl.Emit(RE.OpCodes.Ldarg_0);
            getterIl.Emit(RE.OpCodes.Ldfld, fieldBuilder);
            getterIl.Emit(RE.OpCodes.Ret);

            var setterBuilder = typeBuilder.DefineMethod(String.Format("set_{0}", name), methodAttrs, typeof(void), new[] { propertyType });
            setterBuilder.SetCustomAttribute(attrBuilder);
            var setterIl = setterBuilder.GetILGenerator();
            setterIl.Emit(RE.OpCodes.Ldarg_0);
            setterIl.Emit(RE.OpCodes.Ldarg_1);
            setterIl.Emit(RE.OpCodes.Stfld, fieldBuilder);
            setterIl.Emit(RE.OpCodes.Ret);

            var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, CallingConventions.HasThis, propertyType, null);
            propertyBuilder.SetGetMethod(getterBuilder);
            propertyBuilder.SetSetMethod(setterBuilder);
        }
    }
}
