using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer.Patterns
{
    public class EngineCommandPattern : EngineQueryPattern
    {
        public EngineCommand Command { get; set; }
    }
}
