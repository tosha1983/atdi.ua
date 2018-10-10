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

    }
}
