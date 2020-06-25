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
                    Gx += (pointI.Longitude + pointIPlus1.Longitude) * (pointI.Longitude * pointIPlus1.Latitude - pointIPlus1.Longitude * pointI.Latitude);
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
                    pointResult.CoordinateUnits = CoordinateUnits.deg;
                    break;
                case TypeGeometryObject.Polygon:
                    pointResult.Longitude = CalcGxForPolygon(in geometryArgs);
                    pointResult.Latitude = CalcGyForPolygon(in geometryArgs);
                    pointResult.CoordinateUnits = CoordinateUnits.deg;
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
                var curNearestDistance = GetDistance_km(in pointEarthGeometricCalc, in curNearestPoint);
                for (int i = 0; i < points.Length; i++)
                {
                    var point = points[i];
                    var distance = GetDistance_km(pointEarthGeometricCalc, point);
                    if (distance < curNearestDistance)
                    {
                        curNearestDistance = distance;
                        curNearestPoint = point;
                    }
                }
                pointResult.Longitude = curNearestPoint.Longitude;
                pointResult.Latitude = curNearestPoint.Latitude;
                pointResult.CoordinateUnits = CoordinateUnits.deg;
            }
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
                double arcDist = distance / R;
                var newLat = Math.Sin(latitude * Math.PI / 180.0) * Math.Cos(arcDist) +
                    Math.Cos(latitude * Math.PI / 180.0) * Math.Sin(arcDist) * Math.Cos(azimuth * Math.PI / 180.00);
                point.Latitude = 180 * Math.Asin(newLat) / Math.PI;
                var newLon = Math.Sin(arcDist) * Math.Sin(azimuth * Math.PI / 180.0) /
                    (Math.Cos(latitude * Math.PI / 180.0) * Math.Cos(arcDist) -
                    Math.Sin(latitude * Math.PI / 180.0) * Math.Sin(arcDist) * Math.Cos(azimuth * Math.PI / 180.0));
                point.Longitude = longitude + 180 * Math.Atan(newLon) / Math.PI;
                point.CoordinateUnits = CoordinateUnits.deg;
            }
            else
            {
                point.Longitude = longitude + distance * Math.Sin(azimuth * Math.PI / 180.0) / Math.Cos(latitude * Math.PI / 180.0) / (R * Math.PI / 180.0);
                point.Latitude = latitude + distance * Math.Cos(azimuth * Math.PI / 180.0) / (R * Math.PI / 180.0);
                point.CoordinateUnits = CoordinateUnits.deg;
            }
            return point;
        }


        public void Dispose()
        {

        }

        public void CreateContourForStationByTriggerFieldStrengths(Func<PointEarthGeometric, PointEarthGeometric, double> calcFieldStrengths, in ContourForStationByTriggerFieldStrengthsArgs contourForStationByTriggerFieldStrengthsArgs, ref PointEarthGeometric[] pointResult, out int sizeResultBuffer)
        {
            double d0_m = 500.0;
            double mindistance_m = 0.05;
            int index = 0;
            for (double azimuth = 0; azimuth < 360; azimuth = index * contourForStationByTriggerFieldStrengthsArgs.Step_deg)
            {
                
                var coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Longitude, contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Latitude, d0_m, azimuth);
                var calcFieldStrength = calcFieldStrengths(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                var d1_m = d0_m;
                while (true)
                {
                    if (calcFieldStrength < contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength)
                    {
                        d1_m += (d1_m / 2.0);
                        coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Longitude, contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Latitude, d1_m, azimuth);
                        calcFieldStrength = calcFieldStrengths(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                    }
                    else if (calcFieldStrength > contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength)
                    {
                        d1_m -= (d1_m / 2.0);
                        coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Longitude, contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Latitude, d1_m, azimuth);
                        calcFieldStrength = calcFieldStrengths(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                    }
                    if ((Math.Round(calcFieldStrength,2) == contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength) || (d1_m < mindistance_m))
                    {
                        pointResult[index] = coordRecalc;
                        pointResult[index].CoordinateUnits = CoordinateUnits.deg;
                        break;
                    }
                }
                index++;
            }
            sizeResultBuffer = index;
        }

        public bool CheckHitting(PointEarthGeometric[] poligon, PointEarthGeometric point)
        {
            if (poligon == null || poligon.Length == 0)
                return false;


            bool hit = false; // количество пересечений луча слева в право четное = false, нечетное = true;
            for (int i = 0; i < poligon.Length - 1; i++)
            {
                if (((poligon[i].Latitude <= point.Latitude) && ((poligon[i + 1].Latitude > point.Latitude))) || ((poligon[i].Latitude > point.Latitude) && ((poligon[i + 1].Latitude <= point.Latitude))))
                {
                    if ((poligon[i].Longitude > point.Longitude) && (poligon[i + 1].Longitude > point.Longitude))
                    {
                        hit = !hit;
                    }
                    else if (!((poligon[i].Longitude < point.Longitude) && (poligon[i + 1].Longitude < point.Longitude)))
                    {
                        if (point.Longitude < poligon[i + 1].Longitude - (point.Latitude - poligon[i + 1].Latitude) * (poligon[i + 1].Longitude - poligon[i].Longitude) / (poligon[i].Latitude - poligon[i + 1].Latitude))
                        {
                            hit = !hit;
                        }
                    }
                }
            }
            int i_ = poligon.Length - 1;
            if (((poligon[i_].Latitude <= point.Latitude) && ((poligon[0].Latitude > point.Latitude))) || ((poligon[i_].Latitude > point.Latitude) && ((poligon[0].Latitude <= point.Latitude))))
            {
                if ((poligon[i_].Longitude > point.Longitude) && (poligon[0].Longitude > point.Longitude))
                {
                    hit = !hit;
                }
                else if (!((poligon[i_].Longitude < point.Longitude) && (poligon[0].Longitude < point.Longitude)))
                {
                    if (point.Longitude < poligon[0].Longitude - (point.Latitude - poligon[0].Latitude) * (poligon[0].Longitude - poligon[i_].Longitude) / (poligon[i_].Latitude - poligon[0].Latitude))
                    {
                        hit = !hit;
                    }
                }
            }

            return hit;
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
                var coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourFromPointByDistanceArgs.PointEarthGeometricCalc.Longitude, contourFromPointByDistanceArgs.PointEarthGeometricCalc.Latitude, contourFromPointByDistanceArgs.Distance_km, azimuth);
                pointResult[index] = coordRecalc;
                pointResult[index].CoordinateUnits = CoordinateUnits.deg;
                index++;
            }
            sizeResultBuffer = index;
        }
        /// <summary>
        /// Определение расстояния между двумя заданными точками
        /// </summary>
        /// <param name="sourcePointAgs"></param>
        /// <param name="targetPointArgs"></param>
        /// <param name="coordinateUnits"></param>
        /// <returns></returns>
        public double GetDistance_km(in PointEarthGeometric sourcePointAgs, in PointEarthGeometric targetPointArgs)
        {
            if (sourcePointAgs.CoordinateUnits != targetPointArgs.CoordinateUnits)
            {
                throw new Exception("Сoordinate types do not match!");
            }
            else
            {
                return GeometricСalculations.GetDistance_km(sourcePointAgs.Longitude, sourcePointAgs.Latitude, targetPointArgs.Longitude, targetPointArgs.Latitude, sourcePointAgs.CoordinateUnits);
            }
        }

        /// <summary>
        /// Определение азимута
        /// </summary>
        /// <param name="sourcePointAgs"></param>
        /// <param name="targetPointArgs"></param>
        /// <param name="coordinateUnits"></param>
        /// <returns></returns>
        public double GetAzimut(in PointEarthGeometric sourcePointAgs, in PointEarthGeometric targetPointArgs)
        {
            if (sourcePointAgs.CoordinateUnits != targetPointArgs.CoordinateUnits)
            {
                throw new Exception("Сoordinate types do not match!");
            }
            else
            {
                return GeometricСalculations.GetAzimut(sourcePointAgs.Longitude, sourcePointAgs.Latitude, targetPointArgs.Longitude, targetPointArgs.Latitude, sourcePointAgs.CoordinateUnits);
            }
        }

        public void CreateContourFromContureByDistance(in ContourFromContureByDistanceArgs contourFromContureByDistanceArgs, ref PointEarthGeometricWithAzimuth[] pointEarthGeometricWithAzimuth, out int sizeResultBuffer)
        {
          
            sizeResultBuffer = 0;
        }

    }
}
