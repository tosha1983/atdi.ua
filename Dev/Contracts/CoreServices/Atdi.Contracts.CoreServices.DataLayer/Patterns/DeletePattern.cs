using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer.Patterns
{
    public class DeletePattern : EngineQueryPattern
    {
        /// <summary>
        /// Последовательность выражений ввиде запросов на чтения и удаления данных
        /// </summary>
        public QueryExpression[] Expressions { get; set; }
    }
}
