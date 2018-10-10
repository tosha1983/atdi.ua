using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Platform.AppComponent;
using System.Xml.Linq;

namespace Atdi.CoreServices.EntityOrm
{
    public class EntityOrm : IEntityOrm
    {
        private readonly IEntityOrmConfig _config;
        private readonly Dictionary<string, IEntityMetadata> _cache;
        public EntityOrm(IEntityOrmConfig config)
        {
            this._config = config;
        }

        public IDataTypeMetadata GetDataTypeMetadata(string dataTypeName)
        {
            IDataTypeMetadata dataTypeMetadata = null;
            if (!string.IsNullOrEmpty(_config.DataTypesPath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(_config.DataTypesPath);
                System.IO.FileInfo[] list = di.GetFiles();
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].Extension.ToLower() == ".xml")
                    {
                        XDocument _dataTypeMetadata = XDocument.Load(list[i].FullName);
                        if (_dataTypeMetadata!=null)
                        {

                            XAttribute attributeName = _dataTypeMetadata.Element("DataType").FirstAttribute;
                            XAttribute attributeDataSourceType = _dataTypeMetadata.Element("DataType").LastAttribute;
                            XElement elementAutonum = _dataTypeMetadata.Element("DataType").Element("Autonum");
                            XAttribute attributeStart = _dataTypeMetadata.Element("Autonum").FirstAttribute;
                            XAttribute attributeStep = _dataTypeMetadata.Element("Autonum").LastAttribute;
                            XElement elementCodeVarType = _dataTypeMetadata.Element("DataType").Element("CodeVarType");
                            XElement elementSourceVarType = _dataTypeMetadata.Element("DataType").Element("SourceVarType");
                            XElement elementLength = _dataTypeMetadata.Element("DataType").Element("Length");
                            XElement elementPrecision = _dataTypeMetadata.Element("DataType").Element("Precision");
                            XElement elementScale = _dataTypeMetadata.Element("DataType").Element("Scale");
                             
                            if (attributeName != null)
                            {
                                if (attributeName.Value == dataTypeName)
                                {
                                    dataTypeMetadata.Name = attributeName.Value;
                                    if (attributeStart != null)
                                    {
                                        if (attributeStart.Value != null)
                                        {
                                            dataTypeMetadata.Autonum.Start = Convert.ToInt32(attributeStart.Value);
                                        }
                                    }
                                    if (attributeStep != null)
                                    {
                                        if (attributeStep.Value != null)
                                        {
                                            dataTypeMetadata.Autonum.Step = Convert.ToInt32(attributeStep.Value);
                                        }
                                    }
                                    if (elementCodeVarType != null)
                                    {
                                        DataModels.DataType dataType;
                                        if (Enum.TryParse(elementCodeVarType.Value, out dataType))
                                        {
                                            dataTypeMetadata.CodeVarType = dataType;
                                        }
                                    }
                                    if (attributeDataSourceType != null)
                                    {
                                        DataSourceType dataSourceType;
                                        if (Enum.TryParse(attributeDataSourceType.Value, out dataSourceType))
                                        {
                                            dataTypeMetadata.DataSourceType = dataSourceType;
                                        }
                                    }
                                    if (elementLength != null)
                                    {
                                        dataTypeMetadata.Length = (elementLength.Value!=null ? Convert.ToInt32(elementLength.Value) as int? : null);
                                    }
                                    if (elementPrecision != null)
                                    {
                                        dataTypeMetadata.Precision = (elementPrecision.Value != null ? Convert.ToInt32(elementPrecision.Value) as int? : null);
                                    }
                                    if (elementScale != null)
                                    {
                                        dataTypeMetadata.Scale = (elementScale.Value != null ? Convert.ToInt32(elementScale.Value) as int? : null);
                                    }
                                    if (elementSourceVarType != null)
                                    {
                                        dataTypeMetadata.SourceVarType = elementSourceVarType.Value;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return dataTypeMetadata;
        }

        public IEntityMetadata GetEntityMetadata(string entityName)
        {
            IEntityMetadata entityMetadata = null;
            if (_cache.ContainsKey(entityName))
            {
                return _cache[entityName];
            }
            if (!string.IsNullOrEmpty(_config.EntitiesPath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(_config.EntitiesPath);
                System.IO.FileInfo[] list = di.GetFiles();
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].Extension.ToLower() == ".xml")
                    {
                        XDocument _dataTypeMetadata = XDocument.Load(list[i].FullName);
                        if (_dataTypeMetadata != null)
                        {
                            IEnumerable<XAttribute> attributes = _dataTypeMetadata.Element("Entity").Attributes();
                            if (attributes != null)
                            {
                                XAttribute attributeNameEntity = attributes.ToList().Find(x => x.Name == "Name");
                                if (attributeNameEntity != null)
                                {
                                    if (attributeNameEntity.Value == entityName)
                                    {
                                        entityMetadata.Name = attributeNameEntity.Value;
                                        XAttribute attributeTitle = attributes.ToList().Find(x => x.Name == "Title");
                                        if (attributeTitle!=null)
                                        {
                                            entityMetadata.Title = attributeTitle.Value;
                                        }
                                        XAttribute attributeDesc = attributes.ToList().Find(x => x.Name == "Desc");
                                        if (attributeDesc!=null)
                                        {
                                            entityMetadata.Desc = attributeDesc.Value;
                                        }
                                        XAttribute attributeType = attributes.ToList().Find(x => x.Name == "Type");
                                        if (attributeType!=null)
                                        {
                                            EntityType entityType;
                                            if (Enum.TryParse(attributeType.Value, out entityType))
                                            {
                                                entityMetadata.Type = entityType;
                                            }
                                        }

                                        IDataSourceMetadata dataSourceMetadata = null;
                                        IEnumerable<XAttribute> attributesDataSource = _dataTypeMetadata.Element("Entity").Element("DataSource").Attributes();
                                        if (attributesDataSource != null)
                                        {
                                            
                                            XAttribute attributeDataSourceType = attributesDataSource.ToList().Find(x => x.Name == "Type");

                                            if (attributeDataSourceType != null)
                                            {
                                                DataSourceType dataSourceType;
                                                if (Enum.TryParse(attributeDataSourceType.Value, out dataSourceType))
                                                {
                                                    dataSourceMetadata.Type = dataSourceType;
                                                }
                                            }

                                            XAttribute attributeDataSourceObject = attributesDataSource.ToList().Find(x => x.Name == "Object");
                                            if (attributeDataSourceObject!=null)
                                            {
                                                DataSourceObject dataSourceObject;
                                                if (Enum.TryParse(attributeDataSourceType.Value, out dataSourceObject))
                                                {
                                                    dataSourceMetadata.Object = dataSourceObject;
                                                }
                                            }


                                        }
                                        XElement xElementEntityDataSourceName = _dataTypeMetadata.Element("Entity").Element("DataSource").Element("Name");
                                        if (xElementEntityDataSourceName!=null)
                                        {
                                            dataSourceMetadata.Name = xElementEntityDataSourceName.Value;
                                        }

                                        XElement xElementEntityDataSourceSchema = _dataTypeMetadata.Element("Entity").Element("DataSource").Element("Schema");
                                        if (xElementEntityDataSourceSchema!=null)
                                        {
                                            dataSourceMetadata.Schema = xElementEntityDataSourceSchema.Value;
                                        }

                                        entityMetadata.DataSource = dataSourceMetadata;

                                        Dictionary<string, IFieldMetadata> dic = new Dictionary<string, IFieldMetadata>();
                                        XElement xElementEntityFields = _dataTypeMetadata.Element("Entity").Element("Fields");
                                        IEnumerable<XElement> xElementEntityFieldValues = xElementEntityFields.Elements("Field");
                                        foreach (XElement elm in xElementEntityFieldValues)
                                        {
                                            IFieldMetadata fieldMetadata = null;
                                            IColumnFieldMetadata columnFieldMetadata = null;
                                            IRelationFieldMetadata relationFieldMetadata = null;
                                            IExtensionFieldMetadata extensionFieldMetadata = null;
                                            IReferenceFieldMetadata referenceFieldMetadata = null;
                                            if (elm != null)
                                            {
                                                IEnumerable<XAttribute> attributeEntityFields = elm.Attributes();
                                                XAttribute attributeEntityFieldSourceType = attributeEntityFields.ToList().Find(x => x.Name == "SourceType");
                                                if (attributeEntityFieldSourceType != null)
                                                {
                                                    XAttribute attributeEntityFieldName = attributeEntityFields.ToList().Find(x => x.Name == "Name");
                                                    if (attributeEntityFieldName!=null)
                                                    {
                                                        fieldMetadata.Name = attributeEntityFieldName.Value;
                                                    }
                                                    XAttribute attributeEntityFieldSourceName = attributeEntityFields.ToList().Find(x => x.Name == "SourceName");
                                                    if (attributeEntityFieldSourceName!=null)
                                                    {
                                                        fieldMetadata.SourceName = attributeEntityFieldSourceName.Value;
                                                    }
                                                    XAttribute attributeEntityFieldDataType = attributeEntityFields.ToList().Find(x => x.Name == "DataType");
                                                    if (attributeEntityFieldDataType!=null)
                                                    {
                                                        fieldMetadata.DataType = GetDataTypeMetadata(attributeEntityFieldDataType.Value);
                                                    }
                                                    XAttribute attributeEntityFieldTitle = attributeEntityFields.ToList().Find(x => x.Name == "Title");
                                                    if (attributeEntityFieldTitle!=null)
                                                    {
                                                        fieldMetadata.Title = attributeEntityFieldTitle.Value;
                                                    }
                                                    XAttribute attributeEntityFieldDesc = attributeEntityFields.ToList().Find(x => x.Name == "Desc");
                                                    if (attributeEntityFieldDesc!=null)
                                                    {
                                                        fieldMetadata.Desc = attributeEntityFieldDesc.Value;
                                                    }
                                                    XAttribute attributeEntityFieldRequired = attributeEntityFields.ToList().Find(x => x.Name == "Required");
                                                    if (attributeEntityFieldRequired!=null)
                                                    {
                                                        fieldMetadata.Required =  attributeEntityFieldRequired.Value.ToLower()=="true" ? true : false;

                                                    }
                                                    XAttribute attributeEntityFieldUnit = attributeEntityFields.ToList().Find(x => x.Name == "Unit");
                                                    if (attributeEntityFieldUnit!=null)
                                                    {
                                                        fieldMetadata.Unit= GetUnitMetadata(attributeEntityFieldUnit.Value);
                                                    }
                                                    
                                                    if (attributeEntityFieldSourceType.Value == FieldSourceType.Column.ToString())
                                                    {
                                                        columnFieldMetadata = (IColumnFieldMetadata)fieldMetadata;
                                                    }
                                                    else if (attributeEntityFieldSourceType.Value == FieldSourceType.Extension.ToString())
                                                    {
                                                        extensionFieldMetadata = (IExtensionFieldMetadata)fieldMetadata;
                                                    }
                                                    else if (attributeEntityFieldSourceType.Value == FieldSourceType.Reference.ToString())
                                                    {
                                                        referenceFieldMetadata = (IReferenceFieldMetadata)fieldMetadata;
                                                    }
                                                    else if (attributeEntityFieldSourceType.Value == FieldSourceType.Relation.ToString())
                                                    {
                                                        relationFieldMetadata = (IRelationFieldMetadata)fieldMetadata;
                                                    }
                                                    else if (attributeEntityFieldSourceType.Value == FieldSourceType.Expression.ToString())
                                                    {

                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Unknown SourceType");
                                                    }
                                                }
                                              


                                                XElement xElementEntityFieldPrimaryKeyMapping = elm.Element("PrimaryKeyMapping");
                                                if (xElementEntityFieldPrimaryKeyMapping != null)
                                                {
                                                    IEnumerable<XElement> xElementEntityFieldPrimaryKeyMappingMapped = xElementEntityFieldPrimaryKeyMapping.Elements("Mapped");
                                                    foreach (XElement map in xElementEntityFieldPrimaryKeyMappingMapped)
                                                    {
                                                        if (map != null)
                                                        {
                                                            IEnumerable<XAttribute> attributesmap = map.Attributes();
                                                            if (attributesmap != null)
                                                            {
                                                                XAttribute attributeKeyFieldName = attributesmap.ToList().Find(x => x.Name == "KeyFieldName");
                                                                XAttribute attributeMatchWith = attributesmap.ToList().Find(x => x.Name == "MatchWith");
                                                            }
                                                        }
                                                    }
                                                }

                                                XElement xElementEntityFieldRelationCondition = elm.Element("RelationCondition");
                                                if (xElementEntityFieldRelationCondition != null)
                                                {


                                                }
                                            }
                                         
                                        }

                                        XElement xElementEntityPrimaryKey = _dataTypeMetadata.Element("Entity").Element("PrimaryKey");
                                        IEnumerable<XAttribute> xattrEntityPrimaryKeyClustered = xElementEntityPrimaryKey.Attributes();
                                        if (xattrEntityPrimaryKeyClustered != null)
                                        {
                                            XAttribute attrEntityPrimaryKeyClustered = xattrEntityPrimaryKeyClustered.ToList().Find(x => x.Name == "Clustered");
                                        }

                                         XElement xElementEntityPrimaryKeyFieldRef = _dataTypeMetadata.Element("Entity").Element("PrimaryKey").Element("FieldRef");
                                        IEnumerable<XAttribute> xattrEntityPrimaryKeyFieldRef = xElementEntityPrimaryKeyFieldRef.Attributes();
                                        if (xattrEntityPrimaryKeyFieldRef != null)
                                        {
                                            XAttribute attrEntityPrimaryKeyFieldRefName = xattrEntityPrimaryKeyFieldRef.ToList().Find(x => x.Name == "Name");
                                            XAttribute attrEntityPrimaryKeyFieldRefSortOrder = xattrEntityPrimaryKeyFieldRef.ToList().Find(x => x.Name == "SortOrder");
                                        }
                                   }
                                }
                            }
                        }
                    }
                }
                
            }
            return entityMetadata;
        }

        public IUnitMetadata GetUnitMetadata(string unitName)
        {
            IUnitMetadata unitMetadata = null;
            if (!string.IsNullOrEmpty(_config.UnitsPath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(_config.UnitsPath);
                System.IO.FileInfo[] list = di.GetFiles();
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].Extension.ToLower() == ".xml")
                    {
                        XDocument _dataTypeMetadata = XDocument.Load(list[i].FullName);
                        if (_dataTypeMetadata != null)
                        {
                            XAttribute attributeName = _dataTypeMetadata.Element("Unit").FirstAttribute;
                            XElement elementDimension = _dataTypeMetadata.Element("Unit").Element("Dimension");
                            XElement elementCategory = _dataTypeMetadata.Element("Unit").Element("Category");

                            if (attributeName != null)
                            {
                                if (attributeName.Value == unitName)
                                {
                                    unitMetadata.Name = attributeName.Value;
                                   
                                    if (elementDimension != null)
                                    {
                                        unitMetadata.Dimension = elementDimension.Value;
                                    }

                                    if (elementCategory != null)
                                    {
                                        unitMetadata.Category = elementCategory.Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return unitMetadata;
        }
    }
}
