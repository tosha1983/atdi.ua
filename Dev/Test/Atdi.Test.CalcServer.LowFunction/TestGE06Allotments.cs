using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.DeepServices.RadioSystem;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using Atdi.AppUnits.Sdrn.DeepServices.GN06;
using GE = Atdi.DataModels.Sdrn.DeepServices.GN06;
using WPF = Atdi.Test.DeepServices.Client.WPF;

namespace Atdi.Test.CalcServer.LowFunction
{
    public class TestGE06
    {
        public void Test()
        {
            //START DATA
            GE.RefNetworkType RefNetwork = GE.RefNetworkType.RN1;
            GE.RefNetworkConfigType RefNetworkConfig = GE.RefNetworkConfigType.RPC3;
            double PointLon = 30;
            double PointLat = 51;
            double AllotmentPointLon = 31;
            double AllotmentPointLat = 50;
            //

            GE.BroadcastingAllotment broadcastingAllotment = new GE.BroadcastingAllotment();
            broadcastingAllotment.EmissionCharacteristics = new GE.BroadcastingAllotmentEmissionCharacteristics()
            { RefNetwork = RefNetwork, RefNetworkConfig = RefNetworkConfig };
            GE.AreaPoint Point = new GE.AreaPoint() { Lat_DEC = PointLat, Lon_DEC = PointLon };
            GE.AreaPoint AllotmentPoint = new GE.AreaPoint() { Lat_DEC = AllotmentPointLat, Lon_DEC = AllotmentPointLon };
            GE.PointWithAzimuth[] points = new GE.PointWithAzimuth[7];
            EstimationAssignmentsService estimationAssignmentsService = new EstimationAssignmentsService();
            estimationAssignmentsService.EstimationAssignmentsPointsForEtalonNetwork(in broadcastingAllotment, in AllotmentPoint, in Point, ref points, out int i);
            // на карту 
            WPF.Location[] InputData = new WPF.Location[2] { new WPF.Location(Point.Lon_DEC, Point.Lat_DEC), new WPF.Location(AllotmentPoint.Lon_DEC, AllotmentPoint.Lat_DEC) };
            WPF.Location[] OutputData = new WPF.Location[i];
            for (int j = 0; i > j; j++)
            { OutputData[j] = new WPF.Location(points[j].AreaPoint.Lon_DEC, points[j].AreaPoint.Lat_DEC); }
            WPF.RunApp.Start(WPF.TypeObject.Points, InputData, WPF.TypeObject.Points, OutputData);
        }
    }
}
