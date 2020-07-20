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
    public static class FillContoursResultOnDistance
    {
        public static void Fill(int[] distances, CountoursPointExtended[] countoursPointExtended, BroadcastingTypeContext broadcastingTypeContext, ref ContoursResult[] contoursResults, out int sizeBufferContoursResult)
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
                            for (int i = 0; i < distances.Length; i++)
                            {
                                var distinctByDistance = distinctByPointType.FindAll(c => c.Distance == distances[i]);
                                if (distinctByDistance != null)
                                {
                                    var distinctAdmByAdm = distinctByDistance.Select(c => c.administration).Distinct();
                                    if (distinctAdmByAdm != null)
                                    {
                                        var arrDistinctAdmByDistance = distinctAdmByAdm.ToArray();
                                        for (int k = 0; k < arrDistinctAdmByDistance.Length; k++)
                                        {
                                            var listContourPoints = lstCountoursPoints.FindAll(c => c.Distance == distances[i] && c.PointType == arrPointType[n] && c.administration == arrDistinctAdmByDistance[k]);
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

                                                var arrCountoursPoint = new CountoursPoint[listContourPoints.Count];
                                                for (int x=0; x< listContourPoints.Count; x++ )
                                                {
                                                    arrCountoursPoint[x] = new CountoursPoint()
                                                    {
                                                        Distance = listContourPoints[x].Distance,
                                                        FS = listContourPoints[x].FS,
                                                        Height = listContourPoints[x].Height,
                                                        Lat_DEC = listContourPoints[x].Lat_DEC,
                                                        Lon_DEC = listContourPoints[x].Lon_DEC,
                                                        PointType = listContourPoints[x].PointType
                                                    };
                                                }

                                                contoursResults[index] = new ContoursResult()
                                                {
                                                    AffectedADM = arrDistinctAdmByDistance[k],
                                                    ContourType = contourType,
                                                    CountoursPoints = arrCountoursPoint,
                                                    Distance = distances[i],
                                                    PointsCount = arrCountoursPoint.Length
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
