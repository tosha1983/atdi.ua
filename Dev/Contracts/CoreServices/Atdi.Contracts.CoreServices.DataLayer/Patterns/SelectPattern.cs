using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer.Patterns
{
    public class SelectPattern : EngineQueryPattern
    {
        /// <summary>
        /// Последовательность выражений ввиде запросов на выборку связанных данных
        /// </summary>
        public SelectExpression[] Expressions { get; set; }
    }
}
