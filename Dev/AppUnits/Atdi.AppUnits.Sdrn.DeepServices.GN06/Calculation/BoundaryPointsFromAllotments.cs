using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeepServices.GN06
{
    public static class BoundaryPointsFromAllotments
    {

        public static void Calc(IEarthGeometricService earthGeometricService, in BroadcastingAllotmentWithStep broadcastingAllotment, ref Points pointsResult)
        {
            double step_km = broadcastingAllotment.step_km == null ? 5 : broadcastingAllotment.step_km.Value;
            if ((broadcastingAllotment.BroadcastingAllotment.AllotmentParameters is null) || (broadcastingAllotment.BroadcastingAllotment.AllotmentParameters.Сontur is null) || (broadcastingAllotment.BroadcastingAllotment.AllotmentParameters.Сontur.Length < 3))
            { pointsResult.SizeResultBuffer = 0; return; }
            var conture = broadcastingAllotment.BroadcastingAllotment.AllotmentParameters.Сontur;
            PointEarthGeometric firstPoint = new PointEarthGeometric()
            { Latitude = conture[0].Lat_DEC, Longitude = conture[0].Lon_DEC, CoordinateUnits = CoordinateUnits.deg };
            var oldPoint = firstPoint;
            pointsResult.PointEarthGeometrics[0] = oldPoint;
            pointsResult.SizeResultBuffer = 1;
            int i = 1;
            do
            {
                PointEarthGeometric newPoint = new PointEarthGeometric()
                {Latitude = conture[i].Lat_DEC, Longitude = conture[i].Lon_DEC, CoordinateUnits = CoordinateUnits.deg};
                FillBetwenPoints(earthGeometricService, ref pointsResult, oldPoint, newPoint, step_km);
                oldPoint = newPoint;
                i++;
            }
            while (conture.Length > i);
            FillBetwenPoints(earthGeometricService, ref pointsResult, oldPoint, firstPoint, step_km);
            pointsResult.SizeResultBuffer = pointsResult.SizeResultBuffer - 1;
        }
        private static void FillBetwenPoints(IEarthGeometricService earthGeometricService, ref Points pointsResult, PointEarthGeometric oldPoint, PointEarthGeometric newPoint, double step_km)
        {
            double minDistanse = 99999;
            do
            {
                var distanse = earthGeometricService.GetDistance_km(oldPoint, newPoint);
                if (distanse <= 1.1*step_km)
                {
                    pointsResult.PointEarthGeometrics[pointsResult.SizeResultBuffer] = newPoint;
                    pointsResult.SizeResultBuffer++; return;
                }
                else
                {
                    if (distanse < minDistanse)
                    {
                        minDistanse = distanse;
                        double Az = earthGeometricService.GetAzimut(oldPoint, newPoint);
                        var Point = earthGeometricService.CalculationCoordinateByLengthAndAzimuth(in oldPoint, step_km, Az);
                        pointsResult.PointEarthGeometrics[pointsResult.SizeResultBuffer] = Point;
                        pointsResult.SizeResultBuffer++;
                        oldPoint = Point;
                    }
                    else
                    {
                        pointsResult.PointEarthGeometrics[pointsResult.SizeResultBuffer] = newPoint;
                        pointsResult.SizeResultBuffer++; return;
                    }
                }
            }
            while (true);

        }
    }
}
