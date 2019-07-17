using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = Atdi.DataModels.EntityOrm.Api;

namespace Atdi.WebApiServices.EntityOrm.Controllers.DTO
{
    public class EntityFieldMetadata : API.IEntityFieldMetadata
    {
        public EntityFieldMetadata(IFieldMetadata fieldMetadata)
        {
            if (fieldMetadata == null)
            {
                throw new ArgumentNullException(nameof(fieldMetadata));
            }

            this.Name = fieldMetadata.Name;
            this.Title = fieldMetadata.Title;
            this.Desc = fieldMetadata.Desc;
            this.Required = fieldMetadata.Required;
            this.SourceTypeCode  = (int)fieldMetadata.SourceType;
            this.SourceTypeName = fieldMetadata.SourceType.ToString();
            if (fieldMetadata.Entity != null)
            {
                this.Entity = fieldMetadata.Entity.QualifiedName;
            }
            if (fieldMetadata.BaseEntity != null)
            {
                this.BaseEntity = fieldMetadata.BaseEntity.QualifiedName;
            }
        }

        /// <summary>
        /// Имя поля или отношения
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Отображаемое название поля или отношения
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// 
        /// </summary>
        public string Desc { get; }
        /// <summary>
        /// признак обязательности поля
        /// </summary>
        public bool Required { get; }

        /// <summary>
        /// Тип источника определения значения поля
        /// </summary>
        public int SourceTypeCode { get; }


        public string SourceTypeName { get; }

        /// <summary>
        /// Сущность владелец поля
        /// </summary>
        public string Entity { get; }

        /// <summary>
        /// Базовая сущность, с которой поле было наследовано
        /// </summary>
        public string BaseEntity { get; }
    }

    public class ColumnEntityFieldMetadata : EntityFieldMetadata, API.IColumnEntityFieldMetadata
    {
        public ColumnEntityFieldMetadata(IColumnFieldMetadata fieldMetadata) : base(fieldMetadata)
        {
            if (fieldMetadata.DataType != null)
            {
                this.DataType = new DataTypeMetadata(fieldMetadata.DataType);
            }
            if (fieldMetadata.Unit != null)
            {
                this.Unit = new UnitMetadata(fieldMetadata.Unit);
            }
        }

        /// <summary>
        /// Описание типа поля
        /// </summary>
        public API.IDataTypeMetadata DataType { get; }

        /// <summary>
        /// Единица измерения значения поля
        /// </summary>
        public API.IUnitMetadata Unit { get; }
    }

    public class ReferenceEntityFieldMetadata : EntityFieldMetadata, API.IReferenceEntityFieldMetadata
    {
        public ReferenceEntityFieldMetadata(IReferenceFieldMetadata fieldMetadata) 
            : base(fieldMetadata)
        {
            this.RefEntity = fieldMetadata.RefEntity.QualifiedName;
        }

        /// <summary>
        /// Сущность расширение
        /// </summary>
        public string RefEntity { get; }
    }

    public class ExtensionEntityFieldMetadata : EntityFieldMetadata, API.IExtensionEntityFieldMetadata
    {
        public ExtensionEntityFieldMetadata(IExtensionFieldMetadata fieldMetadata)
            : base(fieldMetadata)
        {
            this.ExtensionEntity = fieldMetadata.ExtensionEntity.QualifiedName;
        }

        /// <summary>
        /// Сущность расширение
        /// </summary>
        public string ExtensionEntity { get; }
    }

    public class RelationEntityFieldMetadata : EntityFieldMetadata, API.IRelationEntityFieldMetadata
    {
        public RelationEntityFieldMetadata(IRelationFieldMetadata fieldMetadata)
            : base(fieldMetadata)
        {
            this.RelatedEntity = fieldMetadata.RelatedEntity.QualifiedName;
        }

        /// <summary>
        /// Сущность расширение
        /// </summary>
        public string RelatedEntity { get; }
    }
}
