using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.AntennaPattern
{
    public class AntennaDiagrammArgs
    {
        public List<double> Angles;
        public List<double> Losses;
        public double Alpha;
        public string Type = string.Empty;
        public double Aff;
        public double MaximalGain;
    }
}

