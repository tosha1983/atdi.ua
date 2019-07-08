using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer.Patterns
{
    public class UpdatePattern : EngineQueryPattern
    {
        /// <summary>
        /// Последовательность выражений ввиде запросов на обновление чтения и обновления данных
        /// </summary>
        public QueryExpression[] Expressions { get; set; }
    }
}
