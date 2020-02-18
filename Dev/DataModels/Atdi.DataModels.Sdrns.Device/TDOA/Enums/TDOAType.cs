using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.TDOA
{
    /// <summary>
    /// Describes the TDOA assessment logic. Optimal type depend from type of signal.
    /// </summary>
    public enum  TDOAType
    {
        Unknown,
        IQDirectCorrelation,
        PSK,
        KAM
    }
}
