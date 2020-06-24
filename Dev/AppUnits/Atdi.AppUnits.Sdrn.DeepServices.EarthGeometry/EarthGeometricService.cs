using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using System.Linq;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace Atdi.AppUnits.Sdrn.DeepServices.EarthGeometry
{
    public class EarthGeometricService : IEarthGeometricService
    {


        public EarthGeometricService()
        {
          
        }



        private double CalcSquare(in GeometryArgs geometryArgs)
        {
            double square = 0;
            if ((geometryArgs.Points != null) && (geometryArgs.Points.Length > 0))
            {
                for (int i = 0; i < geometryArgs.Points.Length - 1; i++)
                {
                    var pointI = geometryArgs.Points[i];
                    var pointIPlus1 = geometryArgs.Points[i + 1];
                    square += (pointI.Longitude * pointIPlus1.Latitude - pointIPlus1.Longitude * pointI.Latitude);
                }
            }
            return square / 2;
        }

        private double CalcGxForPolygon(in GeometryArgs geometryArgs)
        {
            double Gx = 0;
            if ((geometryArgs.Points != null) && (geometryArgs.Points.Length > 0))
            {
                for (int i = 0; i < geometryArgs.Points.Length - 1; i++)
                {
                    var pointI = geometryArgs.Points[i];
                    var pointIPlus1 = geometryArgs.Points[i + 1];
                    Gx += (pointI.Longitude + pointIPlus1.Longitude)* (pointI.Longitude * pointIPlus1.Latitude - pointIPlus1.Longitude* pointI.Latitude);
                }
            }
            return Gx / (6 * CalcSquare(in geometryArgs));
        }

        private double CalcGyForPolygon(in GeometryArgs geometryArgs)
        {
            double Gy = 0;
            if ((geometryArgs.Points != null) && (geometryArgs.Points.Length > 0))
            {
                for (int i = 0; i < geometryArgs.Points.Length - 1; i++)
                {
                    var pointI = geometryArgs.Points[i];
                    var pointIPlus1 = geometryArgs.Points[i + 1];
                    Gy += (pointI.Latitude + pointIPlus1.Latitude) * (pointI.Longitude * pointIPlus1.Latitude - pointIPlus1.Longitude * pointI.Latitude);
                }
            }
            return Gy / (6 * CalcSquare(in geometryArgs));
        }

        private double CalcGxForPoints(in GeometryArgs geometryArgs)
        {
            double Gx = 0;
            if ((geometryArgs.Points != null) && (geometryArgs.Points.Length > 0))
            {
                for (int i = 0; i < geometryArgs.Points.Length; i++)
                {
                    var pointI = geometryArgs.Points[i];
                    Gx += pointI.Longitude;
                }
                Gx = Gx / geometryArgs.Points.Length;
            }
            return Gx;
        }

        private double CalcGyForPoints(in GeometryArgs geometryArgs)
        {
            double Gy = 0;
            if ((geometryArgs.Points != null) && (geometryArgs.Points.Length > 0))
            {
                for (int i = 0; i < geometryArgs.Points.Length; i++)
                {
                    var pointI = geometryArgs.Points[i];
                    Gy += pointI.Latitude;
                }
                Gy = Gy / geometryArgs.Points.Length;
            }
            return Gy;
        }

        public void CalcBarycenter(in GeometryArgs geometryArgs, ref PointEarthGeometric pointResult)
        {
            switch (geometryArgs.TypeGeometryObject)
            {
                case TypeGeometryObject.Points:
                    pointResult.Longitude = CalcGxForPoints(in geometryArgs);
                    pointResult.Latitude = CalcGyForPoints(in geometryArgs);
                    break;
                case TypeGeometryObject.Polygon:
                    pointResult.Longitude = CalcGxForPolygon(in geometryArgs);
                    pointResult.Latitude = CalcGyForPolygon(in geometryArgs);
                    break;
                default:
                    throw new Exception($"For type '{geometryArgs.TypeGeometryObject}'  is not implementation");
            }
        }
        /// <summary>
        /// Функция по определению ближайшей точки контура
        /// </summary>
        /// <param name="geometryArgs"></param>
        /// <param name="pointResult"></param>
        public void PutPointToContour(in PutPointToContourArgs geometryArgs, ref PointEarthGeometric pointResult)
        {
            var pointEarthGeometricCalc = geometryArgs.PointEarthGeometricCalc;
            var points = geometryArgs.Points;
            if ((points != null) && (points.Length > 0))
            {
                var curNearestPoint = points[0];
                var curNearestDistance = DistanceTo(pointEarthGeometricCalc, curNearestPoint);
                for (int i = 0; i < points.Length; i++)
                {
                    var point = points[i];
                    var distance = DistanceTo(pointEarthGeometricCalc, point);
                    if (distance < curNearestDistance)
                    {
                        curNearestDistance = distance;
                        curNearestPoint = point;
                    }
                }
                pointResult.Longitude = curNearestPoint.Longitude;
                pointResult.Latitude = curNearestPoint.Latitude;
            }
        }

        private double DistanceTo(PointEarthGeometric pointEarthGeometricFromArr, PointEarthGeometric point)
        {
            var distanceX = pointEarthGeometricFromArr.Longitude - point.Longitude;
            var distanceY = pointEarthGeometricFromArr.Latitude - point.Latitude;
            var distance = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            return distance;
        }

        /// <summary>
        /// Расчет координаты по заданной точке, азимуту и расстоянию
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="distance"></param>
        /// <param name="azimuth"></param>
        /// <returns></returns>
        public PointEarthGeometric CalculationCoordinateByLengthAndAzimuth(double longitude, double latitude, double distance, double azimuth, bool LargeCircleArc = true)
        {
            var R = 6371.0;
            var point = new PointEarthGeometric();
            if (LargeCircleArc)
            {
                double arcDist = distance/R;
                var newLat = Math.Sin(latitude * Math.PI / 180.0) * Math.Cos(arcDist) +
                    Math.Cos(latitude * Math.PI / 180.0) * Math.Sin(arcDist) * Math.Cos(azimuth * Math.PI / 180.00);
                point.Latitude = 180*Math.Asin(newLat)/Math.PI;
                var newLon = Math.Sin(arcDist) * Math.Sin(azimuth * Math.PI / 180.0) /
                    (Math.Cos(latitude * Math.PI / 180.0) * Math.Cos(arcDist) -
                    Math.Sin(latitude * Math.PI / 180.0) * Math.Sin(arcDist) * Math.Cos(azimuth * Math.PI / 180.0));
                point.Longitude = longitude + 180*Math.Atan(newLon)/Math.PI;
            }
            else
            {
                point.Longitude = longitude + distance * Math.Sin(azimuth * Math.PI / 180.0) / Math.Cos(latitude * Math.PI / 180.0) / (R * Math.PI / 180.0);
                point.Latitude = latitude + distance * Math.Cos(azimuth * Math.PI / 180.0) / (R * Math.PI / 180.0);
            }
            return point;

        }


        public void Dispose()
        {

        }

        private double CalcFieldStrength(PointEarthGeometric pointEarthGeometric1, PointEarthGeometric pointEarthGeometric2)
        {
            return -1;
        }

        public void CreateContourForStationByTriggerFieldStrengths(in ContourForStationByTriggerFieldStrengthsArgs contourForStationByTriggerFieldStrengthsArgs, ref PointEarthGeometric[] pointResult, out int sizeResultBuffer)
        {
            double d0_m = 500000.0;
            int index = 0;
            for (double azimuth = 0; azimuth < 360; azimuth = index * contourForStationByTriggerFieldStrengthsArgs.Step_deg)
            {
                var coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Longitude, contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Latitude, d0_m, azimuth);

                var calcFieldStrength = CalcFieldStrength(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);

                var d1_m = d0_m;
                while (true)
                {
                    if (calcFieldStrength > contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength)
                    {
                        d1_m += (d1_m / 2.0);
                        coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Longitude, contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Latitude, d1_m, azimuth);
                        calcFieldStrength = CalcFieldStrength(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                    }
                    else if (calcFieldStrength < contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength)
                    {
                        d1_m -= (d1_m / 2.0);
                        coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Longitude, contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Latitude, d1_m, azimuth);
                        calcFieldStrength = CalcFieldStrength(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                    }
                    else if ((calcFieldStrength == contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength) || (d1_m<50))
                    {
                        pointResult[index] = coordRecalc;
                        break;
                    }
                }
                
                index++;
            }
            sizeResultBuffer = index;
        }

        /// <summary>
        /// Расчет расстояния между точками
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns></returns>
        private double getDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; 
            var dLat = deg2rad(lat2 - lat1);  
            var dLon = deg2rad(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; 
            return d;
        }

        private double deg2rad(double deg)
        {
            return deg * (Math.PI / 180);
        }
        /// <summary>
        /// Вычисление точек контура по заданной точке, расстоянии от точки, набору азимутов и шаге между азимутами 
        /// </summary>
        /// <param name="contourFromPointByDistanceArgs"></param>
        /// <param name="pointResult"></param>
        /// <param name="sizeResultBuffer"></param>
        public void CreateContourFromPointByDistance(in ContourFromPointByDistanceArgs contourFromPointByDistanceArgs, ref PointEarthGeometric[] pointResult, out int sizeResultBuffer)
        {
            int index = 0;
            for (double azimuth = 0; azimuth < 360; azimuth= index * contourFromPointByDistanceArgs.Step_deg)
            {
                var coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourFromPointByDistanceArgs.PointEarthGeometricCalc.Longitude, contourFromPointByDistanceArgs.PointEarthGeometricCalc.Latitude, contourFromPointByDistanceArgs.Distance_m, azimuth);
                pointResult[index] = coordRecalc;
                index++;
            }
            sizeResultBuffer = index;
        }
    }
}
