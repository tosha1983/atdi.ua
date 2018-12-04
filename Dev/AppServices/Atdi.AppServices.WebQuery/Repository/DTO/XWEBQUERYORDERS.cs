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
    public sealed class XWEBQUERYORDERS
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
        /// Наименование поля, на которое накладывается ограничение
        /// </summary>
        public string PATH { get; set; }
        /// <summary>
        /// 1 - ASC, 2 - DESC
        /// </summary>
        public int ORDER { get; set; }
      
    }
    
}
