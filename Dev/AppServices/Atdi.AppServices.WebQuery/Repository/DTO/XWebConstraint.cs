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
        public sealed class XWEBCONSTRAINT
        {
            /// <summary>
            /// Идентификатор ограничения в таблице
            /// </summary>
            public int ID { get; set; }
            /// <summary>
            /// Ccылка на запрос 
            /// </summary>
            public int WEBQUERYID { get; set; }
            /// <summary>
            /// Наименование ограничения
            /// </summary>
            public string NAME { get; set; }
            /// <summary>
            /// Наименование поля, на которое накладывается ограничение
            /// </summary>
            public string PATH { get; set; }
            /// <summary>
            /// диапазон чисел от (Min)
            /// </summary>
            public double? MIN { get; set; }
            /// <summary>
            /// диапазон чисел до (Max)
            /// </summary>
            public double? MAX { get; set; }
            /// <summary>
            /// Строка 
            /// </summary>
            public string STRVALUE { get; set; }
            /// <summary>
            /// диапазон дат от (Date_Value_Min)
            /// </summary>
            public DateTime? DATEVALUEMIN { get; set; }
            /// <summary>
            /// диапазон дат до (Date_Value_Max)
            /// </summary>
            public DateTime? DATEVALUEMAX { get; set; }
            /// <summary>
            /// призак прямого или инверсного включения
            /// т.е. если  Include = true - это эквивалентно  (column_name BETWEEN value1 AND value2)
            /// если  Include = false - это эквивалентно  (column_name NOT BETWEEN value1 AND value2)
            /// </summary>
            public int INCLUDE { get; set; }


    }

    }
