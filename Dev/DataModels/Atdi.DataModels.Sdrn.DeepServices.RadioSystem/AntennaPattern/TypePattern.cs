using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.AntennaPattern
{
    [Serializable]
    public enum TypePattern
    {
        UNKNOWN,
        FOREIGN,
        POINTS,
        WIEN,
        VECTOR
    }
}

