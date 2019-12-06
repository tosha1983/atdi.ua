using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICSM.ICSControlClient.Models
{
    public class ResultsMeasurementsStationFilters
    {
        public string MeasGlobalSid { get; set; }
        public string Standard { get; set; }
        public string FreqBg { get; set; }
        public string FreqEd { get; set; }
    }
}
