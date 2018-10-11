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


namespace Atdi.CoreServices.EntityOrm
{
    public class EntityOrm : IEntityOrm
    {
        private readonly IEntityOrmConfig _config;
        private readonly Dictionary<string, IEntityMetadata> _cache;
        public EntityOrm(IEntityOrmConfig config)
        {
            this._config = config;
            _cache = new Dictionary<string, IEntityMetadata>();
        }

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
                                    entityMetadata.BaseEntity = GetEntityMetadata(entityObject.BaseEntity);
                                }
                                if (!string.IsNullOrEmpty(entityObject.ExtendEntity))
                                {
                                    entityMetadata.ExtendEntity = GetEntityMetadata(entityObject.ExtendEntity);
                                }
                                var dataSourceMetadata = new DataSourceMetadata();
                                dataSourceMetadata.Name = entityObject.DataSource.Name;
                                dataSourceMetadata.Schema = entityObject.DataSource.Schema;
                                dataSourceMetadata.Object = (DataSourceObject)Enum.Parse(typeof(DataSourceObject), entityObject.DataSource.Object.ToString());
                                dataSourceMetadata.Type = (DataSourceType)Enum.Parse(typeof(DataSourceType), entityObject.DataSource.Type.ToString());
                                entityMetadata.DataSource = dataSourceMetadata;
                                var primaryKeyMetadata = new PrimaryKeyMetadata();
                                //Внедрить также IRelationFieldMetadata и остальные типы, наследуемые от IFieldMetadata
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
                                    }
                                    else if (fieldDef.SourceType == Metadata.FieldSourceType.Reference)
                                    {
                                        var fieldMetadata = new ReferenceFieldMetadata();
                                        if (fieldDef.DataType != null)
                                        {
                                            fieldMetadata.DataType = GetDataTypeMetadata(fieldDef.DataType, dataSourceMetadata.Type);
                                        }
                                        var primaryKeyFieldMappedMetadata = new PrimaryKeyFieldMappedMetadata();
                                        //fieldMetadata.Mapping
                                        //fieldMetadata.RefEntity  
                                        fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                                        fieldMetadata.Title = fieldDef.Title;
                                        fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                                        fieldMetadata.SourceName = fieldDef.SourceName;
                                        fieldMetadata.Required = fieldDef.Required;
                                        fieldMetadata.Name = fieldDef.Name;
                                        fieldMetadata.Desc = fieldDef.Desc;
                                        dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);
                                    }
                                    else if (fieldDef.SourceType == Metadata.FieldSourceType.Extension)
                                    {
                                        var fieldMetadata = new ExtensionFieldMetadata();
                                        if (fieldDef.DataType != null)
                                        {
                                            fieldMetadata.DataType = GetDataTypeMetadata(fieldDef.DataType, dataSourceMetadata.Type);
                                        }
                                        var primaryKeyFieldMappedMetadata = new PrimaryKeyFieldMappedMetadata();
                                        //fieldMetadata.ExtensionEntity
                                        fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                                        fieldMetadata.Title = fieldDef.Title;
                                        fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                                        fieldMetadata.SourceName = fieldDef.SourceName;
                                        fieldMetadata.Required = fieldDef.Required;
                                        fieldMetadata.Name = fieldDef.Name;
                                        fieldMetadata.Desc = fieldDef.Desc;
                                        dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);
                                    }
                                    else if (fieldDef.SourceType == Metadata.FieldSourceType.Relation)
                                    {
                                        var fieldMetadata = new RelationFieldMetadata();
                                        if (fieldDef.DataType != null)
                                        {
                                            fieldMetadata.DataType = GetDataTypeMetadata(fieldDef.DataType, dataSourceMetadata.Type);
                                        }
                                        var primaryKeyFieldMappedMetadata = new PrimaryKeyFieldMappedMetadata();

                                        DataModels.DataConstraint.ComplexCondition complexCondition = new DataModels.DataConstraint.ComplexCondition();
                                        fieldMetadata.RelationCondition = new DataModels.DataConstraint.ComplexCondition();
                                        ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Operator = (DataModels.DataConstraint.LogicalOperator)Enum.Parse(typeof(DataModels.DataConstraint.LogicalOperator), fieldDef.RelationCondition.ItemElementName.ToString());
                                        
                                        if (fieldDef.RelationCondition.Item is Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef)
                                        {
                                            object[] items = (fieldDef.RelationCondition.Item as Atdi.CoreServices.EntityOrm.Metadata.ConditionExpressionDef).Items;
                                            if (items!=null)
                                            {
                                                ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions = new DataModels.DataConstraint.ComplexCondition[items.Length];
                                                for (int k=0; k< items.Length; k++)
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
                                                               
                                                                ((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Operator = (DataModels.DataConstraint.LogicalOperator)Enum.Parse(typeof(DataModels.DataConstraint.LogicalOperator), expr.ItemsElementName[k].ToString());
                                                                ((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions = new DataModels.DataConstraint.ConditionExpression[expr.Items.Length];
                                                                for (int z = 0; z < expr.Items.Length; z++)
                                                                {
                                                                    ((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z] = new DataModels.DataConstraint.ConditionExpression();
                                                                    if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.TwoOperandsOperationDef)
                                                                    {
                                                                        var twoOperans = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.TwoOperandsOperationDef);
                                                                        if (twoOperans != null)
                                                                        {

                                                                            if (twoOperans.Item is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand = new DataModels.DataConstraint.ColumnOperand();
                                                                                string Name = (twoOperans.Item as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                                                ((DataModels.DataConstraint.ColumnOperand)((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand).ColumnName = Name;

                                                                            }
                                                                            else if (twoOperans.Item is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand = new DataModels.DataConstraint.StringValueOperand();
                                                                                string Value = (twoOperans.Item as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
                                                                                ((DataModels.DataConstraint.StringValueOperand)(((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z])).LeftOperand).Value = Value;
                                                                            }

                                                                            if (twoOperans.Item1 is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand = new DataModels.DataConstraint.ColumnOperand();
                                                                                string Name = (twoOperans.Item1 as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                                                ((DataModels.DataConstraint.ColumnOperand)((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand).ColumnName = Name;

                                                                            }
                                                                            else if (twoOperans.Item1 is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand = new DataModels.DataConstraint.StringValueOperand();
                                                                                string Value = (twoOperans.Item1 as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
                                                                                ((DataModels.DataConstraint.StringValueOperand)(((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z])).LeftOperand).Value = Value;
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (expr.Items[z] is Atdi.CoreServices.EntityOrm.Metadata.OneOperandOperationDef)
                                                                    {
                                                                        var oneOperand = (expr.Items[z] as Atdi.CoreServices.EntityOrm.Metadata.OneOperandOperationDef);
                                                                        if (oneOperand != null)
                                                                        {

                                                                            if (oneOperand.Item is Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand = new DataModels.DataConstraint.ColumnOperand();
                                                                                string Name = (oneOperand.Item as Atdi.CoreServices.EntityOrm.Metadata.FieldOperandDef).Name;
                                                                                ((DataModels.DataConstraint.ColumnOperand)((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand).ColumnName = Name;

                                                                            }
                                                                            else if (oneOperand.Item is Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef)
                                                                            {
                                                                                ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z]).LeftOperand = new DataModels.DataConstraint.StringValueOperand();
                                                                                string Value = (oneOperand.Item as Atdi.CoreServices.EntityOrm.Metadata.ValueOperandDef).Value;
                                                                                ((DataModels.DataConstraint.StringValueOperand)(((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[k]).Conditions[z])).LeftOperand).Value = Value;
                                                                            }

                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                       
                                        /*
                                        DataModels.DataConstraint.ComplexCondition complexCondition = new DataModels.DataConstraint.ComplexCondition();
                                        fieldMetadata.RelationCondition = new DataModels.DataConstraint.ComplexCondition();
                                        ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Operator = (DataModels.DataConstraint.LogicalOperator)Enum.Parse(typeof(DataModels.DataConstraint.LogicalOperator), fieldDef.RelationCondition.ItemElementName.ToString());
                                       
                                        ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions = new DataModels.DataConstraint.ConditionExpression[fieldDef.RelationCondition.Item.Items.Length];
                                        for (int j=0; j< fieldDef.RelationCondition.Item.Items.Length;j++)
                                        {
                                            ((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[j] = new DataModels.DataConstraint.ConditionExpression();
                                            ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[j]).LeftOperand = new DataModels.DataConstraint.ColumnOperand();
                                            ((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[j]).LeftOperand = new DataModels.DataConstraint.ValueOperand();

                                            

                                            ((DataModels.DataConstraint.ColumnOperand)(((DataModels.DataConstraint.ConditionExpression)((DataModels.DataConstraint.ComplexCondition)(fieldMetadata.RelationCondition)).Conditions[j]).LeftOperand)).ColumnName = fieldDef.RelationCondition.Item.ItemsElementName[j].ToString();
                                               // = fieldDef.RelationCondition.Item.Items[j]
                                        }
                                        */

                                        fieldMetadata.RelationCondition.Type =  DataModels.DataConstraint.ConditionType.Complex;
                                        fieldMetadata.Unit = GetUnitMetadata(fieldDef.Unit);
                                        fieldMetadata.Title = fieldDef.Title;
                                        fieldMetadata.SourceType = (FieldSourceType)Enum.Parse(typeof(FieldSourceType), fieldDef.SourceType.ToString());
                                        fieldMetadata.SourceName = fieldDef.SourceName;
                                        fieldMetadata.Required = fieldDef.Required;
                                        fieldMetadata.Name = fieldDef.Name;
                                        fieldMetadata.Desc = fieldDef.Desc;
                                        dictionaryFields.Add(fieldMetadata.Name, fieldMetadata);
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
                                if (entityObject.PrimaryKey != null)
                                {
                                    primaryKeyMetadata.Clustered = entityObject.PrimaryKey.Clustered;
                                    var dicIPrimaryKeyFieldRefMetadata = new Dictionary<string, IPrimaryKeyFieldRefMetadata>();
                                    foreach (var fld in entityObject.PrimaryKey.FieldRef)
                                    {
                                        var primaryKeyFieldRefMetadata = new PrimaryKeyFieldRefMetadata();
                                        primaryKeyFieldRefMetadata.SortOrder = (DataModels.DataConstraint.SortDirection)Enum.Parse(typeof(DataModels.DataConstraint.SortDirection), fld.SortOrder.ToString());
                                        // здесь вопрос: что нужно присвоить в Field?
                                        primaryKeyFieldRefMetadata.Field = primaryKeyFieldRefMetadata;
                                        dicIPrimaryKeyFieldRefMetadata.Add(fld.Name, primaryKeyFieldRefMetadata);
                                    }
                                    primaryKeyMetadata.FieldRefs = dicIPrimaryKeyFieldRefMetadata;
                                    entityMetadata.PrimaryKey = primaryKeyMetadata;
                                }
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
                        _cache.Add(entityName, entityMetadata);
                        break;
                    }
                }
            }
            return entityMetadata;
        }

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
    }
}
