using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels;
using Atdi.DataModels.WebQuery;

namespace Atdi.AppServices.WebQuery
{
        /// Класс для хранения сведений об ограничениях на запрос
        /// </summary>
        internal sealed class XWebConstraint
        {
            /// <summary>
            /// Идентификатор ограничения в таблице
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// Ccылка на запрос 
            /// </summary>
            public int WebQueryId { get; set; }
            /// <summary>
            /// Наименование ограничения
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Наименование поля, на которое накладывается ограничение
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// диапазон чисел от (Min)
            /// </summary>
            public double Min { get; set; }
            /// <summary>
            /// диапазон чисел до (Max)
            /// </summary>
            public double Max { get; set; }
            /// <summary>
            /// Строка 
            /// </summary>
            public string StrValue { get; set; }
            /// <summary>
            /// диапазон дат от (Date_Value_Min)
            /// </summary>
            public DateTime DateValueMin { get; set; }
            /// <summary>
            /// диапазон дат до (Date_Value_Max)
            /// </summary>
            public DateTime DateValueMax { get; set; }
            /// <summary>
            /// призак прямого или инверсного включения
            /// т.е. если  Include = true - это эквивалентно  (column_name BETWEEN value1 AND value2)
            /// если  Include = false - это эквивалентно  (column_name NOT BETWEEN value1 AND value2)
            /// </summary>
            public int Include { get; set; }

        }

    }
