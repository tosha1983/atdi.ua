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
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class FillAffectedADMResult
    {
        public static AffectedADMResult[] Fill(ContoursResult[] contoursResults, string affectedServices)
        {
            AffectedADMResult[] affectedADMResult = null;
            var distinctAdm = contoursResults.Select(c => c.AffectedADM).Distinct();
            var arrDistinctAdmByAdm = distinctAdm.ToArray();
            if ((arrDistinctAdmByAdm != null) && (arrDistinctAdmByAdm.Length > 0))
            {
                affectedADMResult = new AffectedADMResult[arrDistinctAdmByAdm.Length];
                if (affectedServices != null)
                {
                    for (int k = 0; k < arrDistinctAdmByAdm.Length; k++)
                    {
                        var isAffected = contoursResults.ToList().Find(c => c.ContourType == ContourType.Affected && c.AffectedADM== arrDistinctAdmByAdm[k]);
                        affectedADMResult[k] = new AffectedADMResult();
                        affectedADMResult[k].ADM = arrDistinctAdmByAdm[k];
                        affectedADMResult[k].AffectedServices = string.Join(",", affectedServices);
                        affectedADMResult[k].TypeAffected = (isAffected != null) ? "Affected" : "Not Affected";
                    }
                }
            }
            return affectedADMResult;
        }
    }
}
