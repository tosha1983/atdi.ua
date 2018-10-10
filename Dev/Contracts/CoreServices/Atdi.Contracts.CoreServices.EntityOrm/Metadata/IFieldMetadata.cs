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
        string Name { get; set; }
        /// <summary>
        /// Отображаемое название поля или отношения
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string Desc { get; set; }
        /// <summary>
        /// признак обязательности поля
        /// </summary>
        bool Required { get; set; }
        /// <summary>
        /// Тип источника определения значения поля
        /// </summary>
        FieldSourceType SourceType { get; set; }
        
        /// <summary>
        /// Описание типа поля
        /// </summary>
        IDataTypeMetadata DataType { get; set; }
        /// <summary>
        /// Единица измерения значения поля
        /// </summary>
        IUnitMetadata Unit { get; set; }
        /// <summary>
        /// Наименование поля в БД
        /// </summary>
        string SourceName { get; set; }

    }
}
