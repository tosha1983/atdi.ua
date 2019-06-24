using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
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
        /// Сущность, которой принадлежит поле
        /// </summary>
        IEntityMetadata Entity { get; }
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

        
    }
}
