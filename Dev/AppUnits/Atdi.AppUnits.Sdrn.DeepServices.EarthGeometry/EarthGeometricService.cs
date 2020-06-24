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
                    square += (pointI.Longitude_dec * pointIPlus1.Latitude_dec - pointIPlus1.Longitude_dec * pointI.Latitude_dec);
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
                    Gx += (pointI.Longitude_dec + pointIPlus1.Longitude_dec) * (pointI.Longitude_dec * pointIPlus1.Latitude_dec - pointIPlus1.Longitude_dec * pointI.Latitude_dec);
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
                    Gy += (pointI.Latitude_dec + pointIPlus1.Latitude_dec) * (pointI.Longitude_dec * pointIPlus1.Latitude_dec - pointIPlus1.Longitude_dec * pointI.Latitude_dec);
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
                    Gx += pointI.Longitude_dec;
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
                    Gy += pointI.Latitude_dec;
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
                    pointResult.Longitude_dec = CalcGxForPoints(in geometryArgs);
                    pointResult.Latitude_dec = CalcGyForPoints(in geometryArgs);
                    break;
                case TypeGeometryObject.Polygon:
                    pointResult.Longitude_dec = CalcGxForPolygon(in geometryArgs);
                    pointResult.Latitude_dec = CalcGyForPolygon(in geometryArgs);
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
                pointResult.Longitude_dec = curNearestPoint.Longitude_dec;
                pointResult.Latitude_dec = curNearestPoint.Latitude_dec;
            }
        }

        private double DistanceTo(PointEarthGeometric pointEarthGeometricFromArr, PointEarthGeometric point)
        {
            var distanceX = pointEarthGeometricFromArr.Longitude_dec - point.Longitude_dec;
            var distanceY = pointEarthGeometricFromArr.Latitude_dec - point.Latitude_dec;
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
        private PointEarthGeometric CalculationCoordinateByLengthAndAzimuth(double longitude, double latitude, double distance, double azimuth)
        {
            var R = 6371000.0;
            var point = new PointEarthGeometric();
            point.Longitude_dec = longitude + distance * Math.Sin(azimuth * Math.PI / 180.0) / Math.Cos(latitude * Math.PI / 180.0) / (R * Math.PI / 180.0);
            point.Latitude_dec = latitude + distance * Math.Cos(azimuth * Math.PI / 180.0) / (R * Math.PI / 180.0);
            return point;
        }


        public void Dispose()
        {

        }

        public void CreateContourForStationByTriggerFieldStrengths(Func<PointEarthGeometric, PointEarthGeometric, double> calcFieldStrengths, in ContourForStationByTriggerFieldStrengthsArgs contourForStationByTriggerFieldStrengthsArgs, ref PointEarthGeometric[] pointResult, out int sizeResultBuffer)
        {
            double d0_m = 500000.0;
            double mindistance_m = 50.0;
            int index = 0;
            for (double azimuth = 0; azimuth < 360; azimuth = index * contourForStationByTriggerFieldStrengthsArgs.Step_deg)
            {
                
                var coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Longitude_dec, contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Latitude_dec, d0_m, azimuth);
                var calcFieldStrength = calcFieldStrengths(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                var d1_m = d0_m;
                while (true)
                {
                    if (calcFieldStrength < contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength)
                    {
                        d1_m += (d1_m / 2.0);
                        coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Longitude_dec, contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Latitude_dec, d1_m, azimuth);
                        calcFieldStrength = calcFieldStrengths(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                    }
                    else if (calcFieldStrength > contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength)
                    {
                        d1_m -= (d1_m / 2.0);
                        coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Longitude_dec, contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc.Latitude_dec, d1_m, azimuth);
                        calcFieldStrength = calcFieldStrengths(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                    }
                    if ((Math.Round(calcFieldStrength,2) == contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength) || (d1_m < mindistance_m))
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
                var coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourFromPointByDistanceArgs.PointEarthGeometricCalc.Longitude_dec, contourFromPointByDistanceArgs.PointEarthGeometricCalc.Latitude_dec, contourFromPointByDistanceArgs.Distance_m, azimuth);
                pointResult[index] = coordRecalc;
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
        public double GetDistance_km(in PointEarthGeometricArgs sourcePointAgs, in PointEarthGeometricArgs targetPointArgs, CoordinateUnits coordinateUnits = CoordinateUnits.m)
        {
            return GeometricСalculations.GetDistance_km(in sourcePointAgs, in targetPointArgs, coordinateUnits);
        }

        /// <summary>
        /// Определение азимута
        /// </summary>
        /// <param name="sourcePointAgs"></param>
        /// <param name="targetPointArgs"></param>
        /// <param name="coordinateUnits"></param>
        /// <returns></returns>
        public double GetAzimut(in PointEarthGeometricArgs sourcePointAgs, in PointEarthGeometricArgs targetPointArgs, CoordinateUnits coordinateUnits = CoordinateUnits.m)
        {
            return GeometricСalculations.GetAzimut(in sourcePointAgs, in targetPointArgs, coordinateUnits);
        }

        public void CreateContourFromContureByDistance(in ContourFromContureByDistanceArgs contourFromContureByDistanceArgs, ref PointEarthGeometricWithAzimuth[] pointEarthGeometricWithAzimuth, out int sizeResultBuffer)
        {
            int index = 0;
            for (double azimuth = 0; azimuth < 360; azimuth = index * contourFromContureByDistanceArgs.Step_deg)
            {
                var coordRecalc = CalculationCoordinateByLengthAndAzimuth(contourFromContureByDistanceArgs.PointBaryCenter.Longitude_dec, contourFromContureByDistanceArgs.PointBaryCenter.Latitude_dec, contourFromContureByDistanceArgs.Distance_m, azimuth);


                var line2 = new LineEarthGeometric()
                {
                    PointEarthGeometric1 = contourFromContureByDistanceArgs.PointBaryCenter,
                    PointEarthGeometric2 = coordRecalc
                };

                for (int i = 0; i < contourFromContureByDistanceArgs.ContourPoints.Length - 1; i++)
                {
                    var line = new LineEarthGeometric()
                    {
                        PointEarthGeometric1 = contourFromContureByDistanceArgs.ContourPoints[i],
                        PointEarthGeometric2 = contourFromContureByDistanceArgs.ContourPoints[i + 1]
                    };

                    var x = intersect(line, line2);
                }

                index++;
            }
            sizeResultBuffer = 0;
        }



        public bool intersect(LineEarthGeometric p1, LineEarthGeometric p2)
        {

            //если первый отрезок вертикальный
            if (p1.PointEarthGeometric1.Longitude_dec - p1.PointEarthGeometric2.Longitude_dec == 0)
            {

                //найдём Xa, Ya - точки пересечения двух прямых
                double Xa = p1.PointEarthGeometric1.Longitude_dec;
                double A2 = (p2.PointEarthGeometric1.Latitude_dec - p2.PointEarthGeometric2.Latitude_dec) / (p2.PointEarthGeometric1.Longitude_dec - p2.PointEarthGeometric2.Longitude_dec);
                double b2 = p2.PointEarthGeometric1.Latitude_dec - A2 * p2.PointEarthGeometric1.Longitude_dec;
                double Ya = A2 * Xa + b2;

                if (p2.PointEarthGeometric1.Longitude_dec <= Xa && p2.PointEarthGeometric2.Longitude_dec >= Xa && Math.Min(p1.PointEarthGeometric1.Latitude_dec, p1.PointEarthGeometric2.Latitude_dec) <= Ya &&
                        Math.Max(p1.PointEarthGeometric1.Latitude_dec, p1.PointEarthGeometric2.Latitude_dec) >= Ya)
                {

                    return true;
                }

                return false;
            }
            //если второй отрезок вертикальный
            else if (p2.PointEarthGeometric1.Longitude_dec - p2.PointEarthGeometric2.Longitude_dec == 0)
            {

                //найдём Xa, Ya - точки пересечения двух прямых
                double Xa = p2.PointEarthGeometric1.Longitude_dec;
                double A2 = (p1.PointEarthGeometric1.Latitude_dec - p1.PointEarthGeometric2.Latitude_dec) / (p1.PointEarthGeometric1.Longitude_dec - p1.PointEarthGeometric2.Longitude_dec);
                double b2 = p1.PointEarthGeometric1.Latitude_dec - A2 * p1.PointEarthGeometric1.Longitude_dec;
                double Ya = A2 * Xa + b2;

                if (p1.PointEarthGeometric1.Longitude_dec <= Xa && p1.PointEarthGeometric2.Longitude_dec >= Xa && Math.Min(p2.PointEarthGeometric1.Latitude_dec, p2.PointEarthGeometric2.Latitude_dec) <= Ya &&
                        Math.Max(p2.PointEarthGeometric1.Latitude_dec, p2.PointEarthGeometric2.Latitude_dec) >= Ya)
                {

                    return true;
                }

                return false;
            }
            else
            {



                double A1 = (p1.PointEarthGeometric1.Latitude_dec - p1.PointEarthGeometric2.Latitude_dec) / (p1.PointEarthGeometric1.Longitude_dec - p1.PointEarthGeometric2.Longitude_dec);
                double A2 = (p2.PointEarthGeometric1.Latitude_dec - p2.PointEarthGeometric2.Latitude_dec) / (p2.PointEarthGeometric1.Longitude_dec - p2.PointEarthGeometric2.Longitude_dec);
                double b1 = p1.PointEarthGeometric1.Latitude_dec - A1 * p1.PointEarthGeometric1.Longitude_dec;
                double b2 = p2.PointEarthGeometric1.Latitude_dec - A2 * p2.PointEarthGeometric1.Longitude_dec;

                if (A1 == A2)
                {
                    return false; //отрезки параллельны
                }

                double Xa = (b2 - b1) / (A1 - A2);

                if ((Xa < Math.Max(p1.PointEarthGeometric1.Longitude_dec, p2.PointEarthGeometric1.Longitude_dec)) || (Xa > Math.Min(p1.PointEarthGeometric2.Longitude_dec, p2.PointEarthGeometric2.Longitude_dec)))
                {
                    return false; //точка Xa находится вне пересечения проекций отрезков на ось X 
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
