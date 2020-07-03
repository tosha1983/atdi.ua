using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06TaskResult
{
    public class AllotmentOrAssignmentModel
    {
        public long Id { get; set; }
        public long Gn06ResultId { get; set; }
        public string Adm { get; set; }
        public string TypeTable { get; set; }
        public string Name { get; set; }
        public double? Freq_MHz { get; set; }
        public double? Longitude_DEC { get; set; }
        public double? Latitude_DEC { get; set; }
        public int? MaxEffHeight_m { get; set; }
        public string Polar { get; set; }
        public float? ErpH_dbW { get; set; }
        public float? ErpV_dbW { get; set; }
        public string AntennaDirectional { get; set; }
        public string AdmRefId { get; set; }
    }
}
