using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels;
using Atdi.DataModels.WebQuery;

namespace Atdi.AppServices.WebQuery
{
        /// Класс для хранения свойств запроса
        /// </summary>
        internal sealed class XWebQuery
        {
            /// <summary>
            /// ID запроса
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// Наименование запроса
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Категория запроса - стандартная или пользовательская
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// Код запроса (в виде структури IRP или чистый SQL)
            /// </summary>
            public byte[] Query { get; set; }
            /// <summary>
            /// Комментарии к запросу
            /// </summary>
            public string Comments { get; set; }
            /// <summary>
            /// Идентификатор поля, которое содержит номер ID пользователя (для фильтрации данных конкретного пользователя)
            /// </summary>
            public string IdentUser { get; set; }
            /// <summary>
            /// Основная таблица запроса
            /// </summary>
            public string TaskForceGroup { get; set; }
        }
    
}
