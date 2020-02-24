using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.TDOA
{
    /// <summary>
    /// Сontains parameters for controlling the localization  TDOA
    /// </summary>
    class TDOAParameters
    {
        TDOAType TDOAType;
        DateTime[] StartReceiveTimes;
        double[] ReceiveDurations_s;
        bool MandatorySignal;
        double MinSignalLevel_dBm; // -1 = avto

    }
}
