using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.DataModels.CoverageCalculation
{
    public class EwxData
    {
        public Header  Header { get; set; }
        public Station[] Stations { get; set; }
    }
}
