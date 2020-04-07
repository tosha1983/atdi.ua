using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.PropagationCalc
{
    public static class CalcSubDiffraction
    {
        public static float SubDeygout91 (float d_km)
        {
            return 10.0f + 0.04f * d_km;
        }
    }
}
