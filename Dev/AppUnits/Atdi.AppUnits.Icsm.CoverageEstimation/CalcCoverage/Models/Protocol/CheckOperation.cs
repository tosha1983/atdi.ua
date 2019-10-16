using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    [Serializable]
    public class CurrentOperation
    {
        public string CurrICSTelecomProjectDir { get; set; }
        public string NameProvince { get; set; }
        public string Standard { get; set; }
        public bool Status { get; set; }
        public Operation  Operation { get; set; }
    }
}
