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
    public static class GE06FillData
    {
        /// <summary>
        /// Заполнение объектов AffectedADMResult[] +  AllotmentOrAssignmentResult
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcResult"></param>
        public static void FillAllotmentOrAssignmentResult(BroadcastingContextBase broadcastingContextBase, BroadcastingTypeContext broadcastingTypeContext, ref Ge06CalcResult ge06CalcResult)
        {
            int countRecordsAllotmentOrAssignmentResult = 0;

            if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments != null) && (broadcastingContextBase.Assignments != null))
            {
                countRecordsAllotmentOrAssignmentResult = broadcastingContextBase.Assignments.Length + 1;
            }
            else if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments == null) && (broadcastingContextBase.Assignments != null))
            {
                countRecordsAllotmentOrAssignmentResult = broadcastingContextBase.Assignments.Length;
            }
            else if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments != null) && (broadcastingContextBase.Assignments == null))
            {
                countRecordsAllotmentOrAssignmentResult = 1;
            }

            var allotmentOrAssignmentResults = new AllotmentOrAssignmentResult[countRecordsAllotmentOrAssignmentResult];

            if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments != null) && (broadcastingContextBase.Assignments != null))
            {
                for (int i = 0; i < broadcastingContextBase.Assignments.Length; i++)
                {
                    var assignment = broadcastingContextBase.Assignments[i];
                    allotmentOrAssignmentResults[i] = new AllotmentOrAssignmentResult()
                    {
                        Adm = assignment.AdmData.Adm,
                        AdmRefId = assignment.AdmData.AdmRefId,
                        Polar = assignment.EmissionCharacteristics.Polar.ToString(),
                        Name = assignment.SiteParameters.Name,
                        Longitude_DEC = assignment.SiteParameters.Lon_Dec,
                        Latitude_DEC = assignment.SiteParameters.Lat_Dec,
                        Freq_MHz = assignment.EmissionCharacteristics.Freq_MHz,
                        TypeTable = "Assignment",
                        AntennaDirectional = assignment.AntennaCharacteristics.Direction.ToString(),
                        ErpH_dbW = assignment.EmissionCharacteristics.ErpH_dBW,
                        ErpV_dbW = assignment.EmissionCharacteristics.ErpV_dBW,
                        MaxEffHeight_m = assignment.AntennaCharacteristics.MaxEffHeight_m,
                        CountoursPoints = new CountoursPoint[0],
                        Source = broadcastingTypeContext.ToString()
                    };
                }

                allotmentOrAssignmentResults[allotmentOrAssignmentResults.Length - 1] = new AllotmentOrAssignmentResult()
                {
                    Adm = broadcastingContextBase.Allotments.AdminData.Adm,
                    AdmRefId = broadcastingContextBase.Allotments.AdminData.AdmRefId,
                    Polar = broadcastingContextBase.Allotments.EmissionCharacteristics.Polar.ToString(),
                    Name = broadcastingContextBase.Allotments.AllotmentParameters.Name,
                    //Longitude_DEC = broadcastingContextBase.Allotments.Target.Lon_Dec,
                    //Latitude_DEC = broadcastingContextBase.Allotments.Target.Lat_Dec,
                    Freq_MHz = broadcastingContextBase.Allotments.EmissionCharacteristics.Freq_MHz,
                    TypeTable = "Allotment",
                    CountoursPoints = ConvertAreaPointToCountoursPoint(broadcastingContextBase.Allotments.AllotmentParameters.Contur),
                    Source = broadcastingTypeContext.ToString()
                    //MaxEffHeight_m =  ?????????????????????????
                    //ErpV_dbW =  ?????????????????????????
                    //ErpH_dbW=  ?????????????????????????
                    //AntennaDirectional = calcForICSM.Allotments.Target.
                };

            }
            else if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments == null) && (broadcastingContextBase.Assignments != null))
            {
                for (int i = 0; i < broadcastingContextBase.Assignments.Length; i++)
                {
                    var assignment = broadcastingContextBase.Assignments[i];
                    allotmentOrAssignmentResults[i] = new AllotmentOrAssignmentResult()
                    {
                        Adm = assignment.AdmData.Adm,
                        AdmRefId = assignment.AdmData.AdmRefId,
                        Polar = assignment.EmissionCharacteristics.Polar.ToString(),
                        Name = assignment.SiteParameters.Name,
                        Longitude_DEC = assignment.SiteParameters.Lon_Dec,
                        Latitude_DEC = assignment.SiteParameters.Lat_Dec,
                        Freq_MHz = assignment.EmissionCharacteristics.Freq_MHz,
                        TypeTable = "Assignment",
                        AntennaDirectional = assignment.AntennaCharacteristics.Direction.ToString(),
                        ErpH_dbW = assignment.EmissionCharacteristics.ErpH_dBW,
                        ErpV_dbW = assignment.EmissionCharacteristics.ErpV_dBW,
                        MaxEffHeight_m = assignment.AntennaCharacteristics.MaxEffHeight_m,
                        CountoursPoints = new CountoursPoint[0],
                        Source = broadcastingTypeContext.ToString()
                    };
                }
            }
            else if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments != null) && (broadcastingContextBase.Assignments == null))
            {
                allotmentOrAssignmentResults[0] = new AllotmentOrAssignmentResult()
                {
                    Adm = broadcastingContextBase.Allotments.AdminData.Adm,
                    AdmRefId = broadcastingContextBase.Allotments.AdminData.AdmRefId,
                    Polar = broadcastingContextBase.Allotments.EmissionCharacteristics.Polar.ToString(),
                    Name = broadcastingContextBase.Allotments.AllotmentParameters.Name,
                    //Longitude_DEC = broadcastingContextBase.Allotments.Target.Lon_Dec,
                    //Latitude_DEC = broadcastingContextBase.Allotments.Target.Lat_Dec,
                    Freq_MHz = broadcastingContextBase.Allotments.EmissionCharacteristics.Freq_MHz,
                    TypeTable = "Allotment",
                    CountoursPoints = ConvertAreaPointToCountoursPoint(broadcastingContextBase.Allotments.AllotmentParameters.Contur),
                    Source = broadcastingTypeContext.ToString()
                    //MaxEffHeight_m =  ?????????????????????????
                    //ErpV_dbW =  ?????????????????????????
                    //ErpH_dbW=  ?????????????????????????
                    //AntennaDirectional = calcForICSM.Allotments.Target.
                };
            }

            ge06CalcResult.AllotmentOrAssignmentResult = allotmentOrAssignmentResults;
        }


        private static CountoursPoint[] ConvertAreaPointToCountoursPoint(AreaPoint[] areaPoints)
        {
            var countoursPoints = new List<CountoursPoint>();
            for (int i = 0; i < areaPoints.Length; i++)
            {
                countoursPoints.Add(new CountoursPoint()
                {
                    Lon_DEC = areaPoints[i].Lon_DEC,
                    Lat_DEC = areaPoints[i].Lat_DEC
                });
            }
            return countoursPoints.ToArray();
        }
    }
}
