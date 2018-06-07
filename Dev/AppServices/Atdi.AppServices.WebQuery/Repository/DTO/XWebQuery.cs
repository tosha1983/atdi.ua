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
        public sealed class XWebQuery
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
            /// Идентификатор запроса
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// Код запроса 
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
            /// Группа TaskForce
            /// </summary>
            public string TaskForceGroup { get; set; }
        }
    
}
