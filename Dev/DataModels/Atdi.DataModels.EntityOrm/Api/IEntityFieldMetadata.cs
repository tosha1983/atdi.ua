using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IEntityFieldMetadata
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
        bool Required { get; }

        /// <summary>
        /// Тип источника определения значения поля
        /// </summary>
        int SourceTypeCode { get; }


        string SourceTypeName { get; }

        /// <summary>
        /// Сущность владелец поля
        /// </summary>
        string Entity { get; }

        /// <summary>
        /// Базовая сущность, с которой поле было наследовано
        /// </summary>
        string BaseEntity { get; }
    }

    public interface IColumnEntityFieldMetadata : IEntityFieldMetadata
    {
        /// <summary>
        /// Описание типа поля
        /// </summary>
        IDataTypeMetadata DataType { get; }

        /// <summary>
        /// Единица измерения значения поля
        /// </summary>
        IUnitMetadata Unit { get; }
    }

    public interface IReferenceEntityFieldMetadata : IEntityFieldMetadata
    {
        /// <summary>
        /// Сущность расширение
        /// </summary>
        string RefEntity { get; }
    }

    public interface IExtensionEntityFieldMetadata : IEntityFieldMetadata
    {
        /// <summary>
        /// Сущность расширение
        /// </summary>
        string ExtensionEntity { get; }
    }

    public interface IRelationEntityFieldMetadata : IEntityFieldMetadata
    {
        /// <summary>
        /// Сущность расширение
        /// </summary>
        string RelatedEntity { get; }
    }
}
