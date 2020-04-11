using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal
{
    internal static class ITU525
    {
        public static float Calc(double ha_m, double hb_m, double Freq_MHz, double d_km)
        {// Надо протестить.
            var d = Math.Sqrt((hb_m - ha_m) * (hb_m - ha_m)*0.000001 + d_km * d_km);
            var Lbf_dB = 32.4 + 20 * Math.Log10(Freq_MHz) + 20 * Math.Log10(d);
            return (float)Lbf_dB;
        }
    }
}
