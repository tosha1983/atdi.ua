using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IFieldDefaultMetadata
    {

    }
    public interface IFieldMetadata
    {
        /// <summary>
        /// Имя поля или отношения
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Отображаемое название поля или отношения
        /// </summary>
        string Title { get; }
        /// <summary>
        /// 
        /// </summary>
        string Desc { get; }
        /// <summary>
        /// признак обязательности поля
        /// </summary>
        bool Required { get;  }

        /// <summary>
        /// Тип источника определения значения поля
        /// </summary>
        FieldSourceType SourceType { get; }
        
        /// <summary>
        /// Описание типа поля
        /// </summary>
        IDataTypeMetadata DataType { get; }

        /// <summary>
        /// Единица измерения значения поля
        /// </summary>
        IUnitMetadata Unit { get; }

        /// <summary>
        /// Наименование поля в БД
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// Сущность владелец поля
        /// </summary>
        IEntityMetadata Entity { get; }

        /// <summary>
        /// Базовая сущность, с которой поле было наследовано
        /// </summary>
        IEntityMetadata BaseEntity { get; }

        IFieldDefaultMetadata Default { get; }
    }

    public static class FieldMetadataExtensions
    {
        /// <summary>
        /// Поле как ссылка
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool IsLookup(this IFieldMetadata field)
        {
            return field.SourceType == FieldSourceType.Extension
                || field.SourceType == FieldSourceType.Reference
                || field.SourceType == FieldSourceType.Relation;
        }

        /// <summary>
        /// Признак того что поле является полем входящим в состав первичного ключа. 
        /// Природа первичного ключа не учитывается - локальный набор или наследуемый или доставшийся при расширении
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool IsPrimaryKey(this IFieldMetadata field)
        {
            if (field.SourceType != FieldSourceType.Column)
            {
                return false;
            }

            var primaryKey = field.Entity.DefinePrimaryKey();
            if (primaryKey == null || primaryKey.FieldRefs == null)
            {
                return false;
            }
            return primaryKey.FieldRefs.ContainsKey(field.Name);
        }

        public static IReferenceFieldMetadata AsReference(this IFieldMetadata field)
        {
            if (field.SourceType == FieldSourceType.Reference)
            {
                return field as IReferenceFieldMetadata;
            }

            throw new InvalidCastException($"Field is not reference. Field is {field.SourceType}");
        }

        public static IExtensionFieldMetadata AsExtension(this IFieldMetadata field)
        {
            if (field.SourceType == FieldSourceType.Extension)
            {
                return field as IExtensionFieldMetadata;
            }

            throw new InvalidCastException($"Field is not extention. Field is {field.SourceType}");
        }

        public static IEntityMetadata GetRefEntity(this IFieldMetadata field)
        {
            if (field.SourceType == FieldSourceType.Reference)
            {
                return ((IReferenceFieldMetadata)field).RefEntity;
            }
            if (field.SourceType == FieldSourceType.Extension)
            {
                return ((IExtensionFieldMetadata)field).ExtensionEntity;
            }
            if (field.SourceType == FieldSourceType.Relation)
            {
                return ((IRelationFieldMetadata)field).RelatedEntity;
            }
            return null;
        }

        public static bool BelongsEntity(this IFieldMetadata field, IEntityMetadata entity)
        {
            // четкое совпадение
            if (field.Entity.QualifiedName == entity.QualifiedName)
            {
                return true;
            }

            // это момент когда идет копирование полей, при котором описательно поле принадлежит базовому объекту
            // но физичесик входит в состав сущности 
            // такая ситуация при наследовании с типом Simple или Role (при Role роля копируются и хранятся в двух сущностях одновременно в базовом и наследнике)
            if (entity.Fields.ContainsKey(field.Name))
            {
                return true;
            }
            return false;
        }


    }
}
