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
        private readonly IEntityOrmConfig _config;
        private readonly Dictionary<string, IEntityMetadata> _cache;
        private readonly List<string> _cashecontainerEntity;
        private readonly List<IEntityMetadata> _cashecontainerEntityList;


        public EntityOrm(IEntityOrmConfig config)
        {
            this._config = config;
            _cache = new Dictionary<string, IEntityMetadata>();
            _cashecontainerEntity = new List<string>();
            _cashecontainerEntityList = new List<IEntityMetadata>();
        }

        
        /// <summary>
        /// Извлечение данных о типах
        /// </summary>
        /// <param name="dataTypeName"></param>
        /// <param name="dataSourceType"></param>
        /// <returns></returns>
        public IDataTypeMetadata GetDataTypeMetadata(string dataTypeName, Atdi.Contracts.CoreServices.EntityOrm.Metadata.DataSourceType dataSourceType)
        {
            var dataTypeMetadata = new DataTypeMetadata();
            if (!string.IsNullOrEmpty(_config.DataTypesPath))
            {
                var serializer = new XmlSerializer(typeof(Atdi.CoreServices.EntityOrm.Metadata.DataTypeDef));
                var reader = new StreamReader(GetFullPathFile(dataTypeName, _config.DataTypesPath, dataSourceType, "xml"));
                object resDataTypeDef = serializer.Deserialize(reader);
                if (resDataTypeDef is Atdi.CoreServices.EntityOrm.Metadata.DataTypeDef)
                {
                    var dataTypeDef = resDataTypeDef as Atdi.CoreServices.EntityOrm.Metadata.DataTypeDef;
                    if (dataTypeDef != null)
                    {
                        var autonumMetadata = new AutonumMetadata();
                        dataTypeMetadata.Name = dataTypeDef.Name;
                        dataTypeMetadata.DataSourceType = (DataSourceType)Enum.Parse(typeof(DataSourceType), dataTypeDef.DataSourceType.ToString());
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
                        if (dataTypeDef.CodeVarType != null)
                        {
                            dataTypeMetadata.CodeVarType = (DataModels.DataType)Enum.Parse(typeof(DataModels.DataType), dataTypeDef.CodeVarType.Value.ToString());
                        }
                        dataTypeMetadata.DataSourceType = (DataSourceType)Enum.Parse(typeof(DataSourceType), dataTypeDef.DataSourceType.ToString());

                        if (dataTypeDef.Length != null)
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
                    }
                }
                reader.Close();
                reader.Dispose();
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
                var reader = new StreamReader(GetFullPathFile(unitName, _config.UnitsPath));
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
        private string GetFullPathFile(string Name, string Path,  string extension)
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
        /// <summary>
        /// Добавление в структуру сущности типа Extension сведений о первичных ключах из расширяемой сущности
        /// </summary>
        /// <param name="fieldMetadata"></param>
        /// <param name="fieldDef"></param>
        /// <param name="entityObject"></param>
        /// <param name="primaryKeyMetadata"></param>
        /// <param name="dictionaryFields"></param>
        private void CheckFieldsForReference(ref ReferenceFieldMetadata fieldMetadata, Metadata.FieldDef fieldDef, Metadata.EntityDef entityObject, ref PrimaryKeyMetadata primaryKeyMetadata, ref  Dictionary<string, IFieldMetadata> dictionaryFields)
        {
            if ((fieldMetadata.RefEntity != null) && (fieldDef.PrimaryKeyMapping != null))
            {
                var primaryKeyFieldMappingMetadata = new PrimaryKeyMappingMetadata();
                var dictionary = new Dictionary<string, IPrimaryKeyFieldMappedMetadata>();
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

                                                var primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                primaryKeyMetadataF.Clustered = entityMetadataOverride.BaseEntity.PrimaryKey.Clustered;
                                                if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
                                                {
                                                    dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, entityMetadataOverride.BaseEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                }
                                                primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                if (!dictionaryFieldsMissing.ContainsKey(fieldEntityMetadataBase.Name))
                                                {
                                                    dictionaryFieldsMissing.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
                                                }

                                                entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                primaryKeyMetadata = primaryKeyMetadataF;
                                                if (entityMetadataOverride != null)
                                                {
                                                    var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
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

                                                var primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                                if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
                                                {
                                                    dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                }
                                                primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
                                                {
                                                    dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                                }

                                                entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                primaryKeyMetadata = primaryKeyMetadataF;
                                                if (entityMetadataOverride != null)
                                                {
                                                    var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
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

                                            var primaryKeyMetadataF = new PrimaryKeyMetadata();
                                            primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                            if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
                                            {
                                                dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                            }
                                            primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                            if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
                                            {
                                                dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                            }

                                            entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                            primaryKeyMetadata = primaryKeyMetadataF;
                                            if (entityMetadataOverride != null)
                                            {
                                                var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
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
                                        var entityMetadataOverride = _cache[entityObject.Name] as EntityMetadata;
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

                                                var primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                primaryKeyMetadataF.Clustered = entityMetadataOverride.BaseEntity.PrimaryKey.Clustered;
                                                if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
                                                {
                                                    dictionaryPrimaryKeyFieldRefMetadata.Add(ch.Value, entityMetadataOverride.BaseEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.Value).Value);
                                                }
                                                primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                if (!dictionaryFieldsMissing.ContainsKey(fieldEntityMetadataBase.Name))
                                                {
                                                    dictionaryFieldsMissing.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
                                                }

                                                entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                primaryKeyMetadata = primaryKeyMetadataF;
                                                if (entityMetadataOverride != null)
                                                {
                                                    var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
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

                                                var primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                                if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
                                                {
                                                    dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                }
                                                primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
                                                {
                                                    dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                                }

                                                entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                primaryKeyMetadata = primaryKeyMetadataF;
                                                if (entityMetadataOverride != null)
                                                {
                                                    var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
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

                                            var primaryKeyMetadataF = new PrimaryKeyMetadata();
                                            primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                            if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
                                            {
                                                dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                            }
                                            primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                            if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
                                            {
                                                dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                            }

                                            entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                            primaryKeyMetadata = primaryKeyMetadataF;
                                            if (entityMetadataOverride != null)
                                            {
                                                var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
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
                                        var entityMetadataOverride = _cache[entityObject.Name] as EntityMetadata;
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

                                                var primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                primaryKeyMetadataF.Clustered = entityMetadataOverride.BaseEntity.PrimaryKey.Clustered;
                                                if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
                                                {
                                                    dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, entityMetadataOverride.BaseEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                }
                                                primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                if (!dictionaryFieldsMissing.ContainsKey(fieldEntityMetadataBase.Name))
                                                {
                                                    dictionaryFieldsMissing.Add(fieldEntityMetadataBase.Name, fieldEntityMetadataBase);
                                                }

                                                entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                primaryKeyMetadata = primaryKeyMetadataF;
                                                if (entityMetadataOverride != null)
                                                {
                                                    var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
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
                                                    var fieldMetadataMiss = fieldMetadataf as FieldMetadata;
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

                                                    var primaryKeyMetadataF = new PrimaryKeyMetadata();
                                                    primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                                    if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
                                                    {
                                                        dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                                    }
                                                    primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                                    if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
                                                    {
                                                        dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                                    }


                                                    entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                                    primaryKeyMetadata = primaryKeyMetadataF;
                                                    if (entityMetadataOverride != null)
                                                    {
                                                        var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
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

                                            var primaryKeyMetadataF = new PrimaryKeyMetadata();
                                            primaryKeyMetadataF.Clustered = fieldMetadata.RefEntity.PrimaryKey.Clustered;
                                            if (!dictionaryPrimaryKeyFieldRefMetadata.ContainsKey(ch.KeyFieldName))
                                            {
                                                dictionaryPrimaryKeyFieldRefMetadata.Add(ch.KeyFieldName, fieldMetadata.RefEntity.PrimaryKey.FieldRefs.ToList().Find(z => z.Key == ch.KeyFieldName).Value);
                                            }
                                            primaryKeyMetadataF.FieldRefs = dictionaryPrimaryKeyFieldRefMetadata;
                                            if (!dictionaryFieldsMissing.ContainsKey(fieldMetadataMiss.Name))
                                            {
                                                dictionaryFieldsMissing.Add(fieldMetadataMiss.Name, fieldMetadataMiss);
                                            }

                                            entityMetadataOverride.PrimaryKey = primaryKeyMetadataF;
                                            primaryKeyMetadata = primaryKeyMetadataF;
                                            if (entityMetadataOverride != null)
                                            {
                                                var entityMetadataOverridex = (entityMetadataOverride as EntityMetadata);
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
                            dictionary.Add(ch.Value, sourceprimaryKeyFieldMappedMetadata);
                        }
                    }
                }
                primaryKeyFieldMappingMetadata.Fields = dictionary;
                fieldMetadata.Mapping = primaryKeyFieldMappingMetadata;
            }
        }

        /// <summary>
        /// Преобразование данных из типа FieldDef в RelationFieldMetadata для сущности типа Relation
        /// </summary>
        /// <param name="fieldMetadata"></param>
        /// <param name="fieldDef"></param>
        private void BuildConditionForRelation(ref RelationFieldMetadata fieldMetadata, Metadata.FieldDef fieldDef)
        {
            fieldMetadata.RelationCondition = new DataModels.DataConstraint.ComplexCondition();
            var complexConditionDataConstraint = ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition));
            complexConditionDataConstraint.Operator = (DataModels.DataConstraint.LogicalOperator)Enum.Parse(typeof(DataModels.DataConstraint.LogicalOperator), fieldDef.RelationCondition.ItemElementName.ToString());
            if (fieldDef.RelationCondition.Item is Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef conditionExpressionDef)
            {
                object[] items = conditionExpressionDef.Items;
                if (items != null)
                {
                    complexConditionDataConstraint.Conditions = new DataModels.DataConstraint.ComplexCondition[items.Length];
                    for (int k = 0; k < items.Length; k++)
                    {
                        complexConditionDataConstraint.Conditions[k] = new DataModels.DataConstraint.ComplexCondition();
                        if (items[k] is Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef)
                        {
                            complexConditionDataConstraint.Conditions[k] = new DataModels.DataConstraint.ComplexCondition();
                            var expr = items[k] as Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef;
                            if (expr != null)
                            {
                                if (expr.Items != null)
                                {
                                    var conditions = ((DataModels.DataConstraint.ComplexCondition)complexConditionDataConstraint.Conditions[k]).Conditions;
                                    conditions = new DataModels.DataConstraint.ConditionExpression[expr.Items.Length];
                                    for (int z = 0; z < expr.Items.Length; z++)
                                    {
                                        conditions[z] = new DataModels.DataConstraint.ConditionExpression();
                                        if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.TwoOperandsOperationDef)
                                        {
                                            var condition = ((DataModels.DataConstraint.ConditionExpression)conditions[z]);
                                            condition.Operator = (DataModels.DataConstraint.ConditionOperator)Enum.Parse(typeof(DataModels.DataConstraint.ConditionOperator), expr.ItemsElementName[z].ToString());

                                            var twoOperans = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.TwoOperandsOperationDef);
                                            if (twoOperans != null)
                                            {
                                                if (twoOperans.Item is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                {
                                                    condition.LeftOperand = new DataModels.DataConstraint.ColumnOperand();
                                                    string Name = (twoOperans.Item as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                    ((DataModels.DataConstraint.ColumnOperand)condition.LeftOperand).ColumnName = Name;
                                                }
                                                if (twoOperans.Item is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
                                                {
                                                    condition.LeftOperand = new DataModels.DataConstraint.StringValueOperand();
                                                    string Value = (twoOperans.Item as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
                                                    ((DataModels.DataConstraint.StringValueOperand)(condition).LeftOperand).Value = Value;
                                                }

                                                if (twoOperans.Item1 is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                {
                                                    condition.RightOperand = new DataModels.DataConstraint.ColumnOperand();
                                                    string Name = (twoOperans.Item1 as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                    ((DataModels.DataConstraint.ColumnOperand)condition.RightOperand).ColumnName = Name;

                                                }
                                                if (twoOperans.Item1 is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
                                                {
                                                    condition.RightOperand = new DataModels.DataConstraint.StringValueOperand();
                                                    string Value = (twoOperans.Item1 as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
                                                    ((DataModels.DataConstraint.StringValueOperand)(condition).RightOperand).Value = Value;
                                                }
                                            }
                                        }
                                        else if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.OneOperandOperationDef)
                                        {
                                            var condition = ((DataModels.DataConstraint.ConditionExpression)conditions[z]);
                                            condition.Operator = (DataModels.DataConstraint.ConditionOperator)Enum.Parse(typeof(DataModels.DataConstraint.ConditionOperator), expr.ItemsElementName[z].ToString());
                                            var oneOperand = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.OneOperandOperationDef);
                                            if (oneOperand != null)
                                            {

                                                if (oneOperand.Item is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                {
                                                    condition.LeftOperand = new DataModels.DataConstraint.ColumnOperand();
                                                    string Name = (oneOperand.Item as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                    ((DataModels.DataConstraint.ColumnOperand)condition.LeftOperand).ColumnName = Name;

                                                }
                                                if (oneOperand.Item is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
                                                {
                                                    condition.RightOperand = new DataModels.DataConstraint.StringValueOperand();
                                                    string Value = (oneOperand.Item as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
                                                    ((DataModels.DataConstraint.StringValueOperand)(condition).RightOperand).Value = Value;
                                                }
                                            }
                                        }
                                        else if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.InOperationDef)
                                        {
                                            var condition = ((DataModels.DataConstraint.ConditionExpression)conditions[z]);
                                            condition.Operator = (DataModels.DataConstraint.ConditionOperator)Enum.Parse(typeof(DataModels.DataConstraint.ConditionOperator), expr.ItemsElementName[z].ToString());
                                            var oneOperand = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.InOperationDef);
                                            if (oneOperand != null)
                                            {
                                                if (oneOperand.Field is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                {
                                                    condition.LeftOperand = new DataModels.DataConstraint.ColumnOperand();
                                                    string Name = (oneOperand.Field as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                    ((DataModels.DataConstraint.ColumnOperand)condition.LeftOperand).ColumnName = Name;
                                                }
                                                if (oneOperand.Values is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef[])
                                                {
                                                    condition.RightOperand = new DataModels.DataConstraint.StringValuesOperand();
                                                    List<string> Values = new List<string>();
                                                    foreach (var cx in oneOperand.Values)
                                                    {
                                                        Values.Add(cx.Value);
                                                    }
                                                    ((DataModels.DataConstraint.StringValuesOperand)(condition).RightOperand).Values = Values.ToArray();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new NotImplementedException(Exceptions.NotSupportedOperation);
                                        }
                                    }
                                    ((DataModels.DataConstraint.ComplexCondition)complexConditionDataConstraint.Conditions[k]).Conditions = conditions;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Основной метод извлечения данных о сущности Entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public IEntityMetadata GetEntityMetadata(string entityName)
        {
            entityName = entityName.Replace(_config.RootPath.Replace("\\", ".") + ".", "").Replace(".", "\\");
            var entityMetadata = new EntityMetadata();
            if (_cache.ContainsKey(entityName))
            {
                return _cache[entityName];
            }

            bool isFinded = false;
            var serializer = new XmlSerializer(typeof(Atdi.CoreServices.EntityOrm.Metadata.EntityDef));
            var reader = new StreamReader(GetFullPathFile(entityName, _config.EntitiesPath, "xml"));
            object resEntity = serializer.Deserialize(reader);
            if (resEntity is Atdi.CoreServices.EntityOrm.Metadata.EntityDef)
            {
                var entityObject = resEntity as Atdi.CoreServices.EntityOrm.Metadata.EntityDef;
                if (entityObject != null)
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

                            if (fieldDef.Unit != null)
                            {
                                fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                            }
                            fieldMetadata.Title = fieldDef.Title;
                            fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                            fieldMetadata.SourceName = fieldDef.SourceName;
                            fieldMetadata.Required = fieldDef.Required;
                            fieldMetadata.Name = fieldDef.Name;
                            fieldMetadata.Desc = fieldDef.Desc;
                            if (!dictionaryFields.ContainsKey(fieldMetadata.Name))
                            {
                                dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);
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

                            CheckFieldsForReference(ref fieldMetadata, fieldDef, entityObject, ref primaryKeyMetadata, ref dictionaryFields);

                            if (fieldDef.Unit != null)
                            {
                                fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                            }
                            fieldMetadata.Title = fieldDef.Title;
                            fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                            fieldMetadata.SourceName = fieldDef.SourceName;
                            fieldMetadata.Required = fieldDef.Required;
                            fieldMetadata.Name = fieldDef.Name;
                            fieldMetadata.Desc = fieldDef.Desc;
                            if (!dictionaryFields.ContainsKey(fieldMetadata.Name))
                            {
                                dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);
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

                            if (fieldDef.Unit != null)
                            {
                                fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                            }
                            fieldMetadata.Title = fieldDef.Title;
                            fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                            fieldMetadata.SourceName = fieldDef.SourceName;
                            fieldMetadata.Required = fieldDef.Required;
                            fieldMetadata.Name = fieldDef.Name;
                            fieldMetadata.Desc = fieldDef.Desc;
                            if (!dictionaryFields.ContainsKey(fieldMetadata.Name))
                            {
                                dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);
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
                            BuildConditionForRelation(ref fieldMetadata, fieldDef);
                            fieldMetadata.RelationCondition.Type = DataModels.DataConstraint.ConditionType.Complex;
                            if (fieldDef.Unit != null)
                            {
                                fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                            }
                            fieldMetadata.Title = fieldDef.Title;
                            fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                            fieldMetadata.SourceName = fieldDef.SourceName;
                            fieldMetadata.Required = fieldDef.Required;
                            fieldMetadata.Name = fieldDef.Name;
                            fieldMetadata.Desc = fieldDef.Desc;
                            if (!dictionaryFields.ContainsKey(fieldMetadata.Name))
                            {
                                dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);
                            }
                        }
                        else if (fieldDef.SourceType == Metadata.FieldSourceType.Expression)
                        {
                            throw new NotImplementedException(Exceptions.HandlerTypeExpressionNotSupported);
                        }
                        else
                        {
                            throw new NotImplementedException(string.Format(Exceptions.UnknownDataSourceType, fieldDef.SourceType));
                        }
                    }
                    entityMetadata.Fields = dictionaryFields;
                    entityMetadata.Inheritance = (InheritanceType)Enum.Parse(typeof(InheritanceType), entityObject.Inheritance.ToString());
                    entityMetadata.Name = entityName.Replace("\\", ".");
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
            }
            _cashecontainerEntity.Clear();
            _cashecontainerEntityList.Clear();
            return entityMetadata;
        }
    }
}
