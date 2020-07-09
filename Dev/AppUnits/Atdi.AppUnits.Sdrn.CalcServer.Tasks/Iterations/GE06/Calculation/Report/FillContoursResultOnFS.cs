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
    public static class FillContoursResultOnFS
    {
        public static void Fill(int[] fieldStrength, CountoursPointExtended[] countoursPointExtended, BroadcastingTypeContext broadcastingTypeContext, ref ContoursResult[] contoursResults, out int sizeBufferContoursResult)
        {
            sizeBufferContoursResult = 0;
            int index = 0;
            if ((countoursPointExtended!=null) && (countoursPointExtended.Length > 0))
            {
                var lstCountoursPoints = countoursPointExtended.ToList();
                var arrPointType = new PointType[4] { PointType.Etalon, PointType.Unknown, PointType.Affected, PointType.Correct };
                if (lstCountoursPoints != null)
                {
                    for (int n = 0; n < arrPointType.Length; n++)
                    {
                        var distinctByPointType = lstCountoursPoints.FindAll(c => c.PointType == arrPointType[n]);
                        if (distinctByPointType != null)
                        {
                            for (int i = 0; i < fieldStrength.Length; i++)
                            {
                                var distinctByFieldStrength = distinctByPointType.FindAll(c => c.FS == fieldStrength[i]);
                                if (distinctByFieldStrength != null)
                                {
                                    var distinctAdmByAdm = distinctByFieldStrength.Select(c => c.administration).Distinct();
                                    if (distinctAdmByAdm != null)
                                    {
                                        var arrDistinctAdmByFieldStrength = distinctAdmByAdm.ToArray();
                                        for (int k = 0; k < arrDistinctAdmByFieldStrength.Length; k++)
                                        {
                                            var listContourPoints = lstCountoursPoints.FindAll(c => c.FS == fieldStrength[i] && c.PointType == arrPointType[n] && c.administration == arrDistinctAdmByFieldStrength[k]);
                                            if (listContourPoints != null)
                                            {

                                                var contourType = ContourType.Unknown;
                                                if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                                                {
                                                    contourType = ContourType.Etalon;
                                                }
                                                if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                                                {

                                                    if (listContourPoints.Find(x => x.PointType == PointType.Affected) != null)
                                                    {
                                                        contourType = ContourType.Affected;
                                                    }
                                                    else
                                                    {
                                                        contourType = ContourType.Correct;
                                                    }
                                                }

                                                var allPoints = listContourPoints.ToArray();

                                                contoursResults[index] = new ContoursResult()
                                                {
                                                    AffectedADM = arrDistinctAdmByFieldStrength[k],
                                                    ContourType = contourType,
                                                    CountoursPoints = allPoints,
                                                    FS = fieldStrength[i],
                                                    PointsCount = allPoints.Length
                                                };

                                                index++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            sizeBufferContoursResult = index;
        }
    }
}



