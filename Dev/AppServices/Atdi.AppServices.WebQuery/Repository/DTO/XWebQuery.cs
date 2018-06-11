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
        public sealed class XWEBQUERY
        {
            /// <summary>
            /// ID запроса
            /// </summary>
            public int ID { get; set; }
            /// <summary>
            /// Наименование запроса
            /// </summary>
            public string NAME { get; set; }
            /// <summary>
            /// Идентификатор запроса
            /// </summary>
            public string CODE { get; set; }
            /// <summary>
            /// Код запроса 
            /// </summary>
            public byte[] QUERY { get; set; }
            /// <summary>
            /// Комментарии к запросу
            /// </summary>
            public string COMMENTS { get; set; }
            /// <summary>
            /// Идентификатор поля, которое содержит номер ID пользователя (для фильтрации данных конкретного пользователя)
            /// </summary>
            public string IDENTUSER { get; set; }
            /// <summary>
            /// Группа TaskForce
            /// </summary>
           public string TASKFORCEGROUP { get; set; }

    }
    
}
