using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis.MapService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using System.Runtime.InteropServices.WindowsRuntime;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.FieldStrength;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class GE06CheckEffectiveHeight
    {

        /// <summary>
        /// Если есть признак не учитывать эффективные высоты, то обнуляем их
        /// </summary>
        /// <param name="assignments"></param>
        /// <returns></returns>
        public static void CheckEffectiveHeightForAssignment(ref BroadcastingAssignment[] assignments, bool isEnableEffectiveHeight)
        {
            if (isEnableEffectiveHeight == false)
            {
                if ((assignments != null) && (assignments.Length > 0))
                {
                    for (int i = 0; i < assignments.Length; i++)
                    {
                        if (assignments[i]!=null)
                        {
                            if (assignments[i].AntennaCharacteristics!=null)
                            {
                                assignments[i].AntennaCharacteristics.EffHeight_m = null;
                            }
                        }
                    }
                }
            }
        }
    }
}
