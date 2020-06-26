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

        public static void Calc(IEarthGeometricService earthGeometricService, BroadcastingAllotment broadcastingAllotment, ref Points pointsResult, double step_km)
        {
            if ((broadcastingAllotment.AllotmentParameters is null) || (broadcastingAllotment.AllotmentParameters.Сontur is null) || (broadcastingAllotment.AllotmentParameters.Сontur.Length < 3))
            { pointsResult.SizeResultBuffer = 0; return; }
            var conture = broadcastingAllotment.AllotmentParameters.Сontur;
            PointEarthGeometric FirstPoint = new PointEarthGeometric()
            { Latitude = conture[0].Lat_DEC, Longitude = conture[0].Lon_DEC, CoordinateUnits = CoordinateUnits.deg };
            var oldPoint = FirstPoint;
            pointsResult.PointEarthGeometrics[0] = oldPoint;
            pointsResult.SizeResultBuffer = 1;
            int i = 1;
            do
            {
                PointEarthGeometric newPoint = new PointEarthGeometric()
                {Latitude = conture[i].Lat_DEC, Longitude = conture[i].Lon_DEC, CoordinateUnits = CoordinateUnits.deg};
                FillBetwenPoints(earthGeometricService, ref pointsResult, oldPoint, newPoint, step_km);
                oldPoint = newPoint;
            }
            while (conture.Length >= i);
            FillBetwenPoints(earthGeometricService, ref pointsResult, oldPoint, FirstPoint, step_km);
            pointsResult.SizeResultBuffer = pointsResult.SizeResultBuffer - 1;
        }
        private static void FillBetwenPoints(IEarthGeometricService earthGeometricService, ref Points pointsResult, PointEarthGeometric oldPoint, PointEarthGeometric newPoint, double step_km)
        {
            double MinDistanse = 99999;
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
                    if (distanse < MinDistanse)
                    {
                        MinDistanse = distanse;
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
