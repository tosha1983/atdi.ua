using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using System.Linq;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;

namespace Atdi.AppUnits.Sdrn.DeepServices.EarthGeometry
{
    public class EarthGeometricService : IEarthGeometricService
    {


        public EarthGeometricService()
        {

        }

        private const double re_km = 6371.0;

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
            var prevPointGeom = new PointEarthGeometric();
            var nextPointGeom = new PointEarthGeometric();
            var pointEarthGeometricCalc = geometryArgs.PointEarthGeometricCalc;
            var points = geometryArgs.Points;
            if ((points != null) && (points.Length > 0))
            {
                // берем первую точку контура
                var curNearestPoint = points[0];
                // вычисляем расстояние от заданной точки до первой точки контура
                var curNearestDistance = GetDistance_km(in pointEarthGeometricCalc, in curNearestPoint);
                for (int i = 0; i < points.Length; i++)
                {
                    if (i > 0)
                    {
                        // извлекаем предыдущую точку контура
                        prevPointGeom = points[i - 1];
                    }
                    if (i < points.Length - 1)
                    {
                        // извлекаем следующую точку контура
                        nextPointGeom = points[i + 1];
                    }
                    // получем очередную точку контура
                    var point = points[i];
                    // если дистанция от следующей точки point до заданной точки pointEarthGeometricCalc меньше чем рассточние от предыдущей точки  point до заданной точки pointEarthGeometricCalc, 
                    var distance = GetDistance_km(pointEarthGeometricCalc, point);
                    if (distance < curNearestDistance)
                    {
                        curNearestDistance = distance;
                        //тогда в переменнуюcurNearestPoint копируем точку point 
                        curNearestPoint = point;

                        // для точек curNearestPoint и prevPointGeom выполняем поиск координат конца перпендикуляра xPrev,  yPrev (к отрезку из точек curNearestPoint и prevPointGeom)
                        CreateContourFromContureByDistanceСalculations.Ortho(prevPointGeom.Longitude, prevPointGeom.Latitude,
                                           curNearestPoint.Longitude, curNearestPoint.Latitude,
                                          pointEarthGeometricCalc.Longitude, pointEarthGeometricCalc.Latitude,
                                          out double xPrev, out double yPrev);

                        // для точек curNearestPoint и nextPointGeom выполняем поиск координат конца перпендикуляра xNext,  yNext (к отрезку из точек curNearestPoint и nextPointGeom)
                        CreateContourFromContureByDistanceСalculations.Ortho(nextPointGeom.Longitude, nextPointGeom.Latitude,
                                                    curNearestPoint.Longitude, curNearestPoint.Latitude,
                                                   pointEarthGeometricCalc.Longitude, pointEarthGeometricCalc.Latitude,
                                                   out double xNext, out double yNext);


                        // поиск расстояния и
                        var distance_prev = GetDistance_km(pointEarthGeometricCalc, new PointEarthGeometric() { Longitude = xPrev, Latitude = yPrev, CoordinateUnits = CoordinateUnits.deg });
                        var distance_next = GetDistance_km(pointEarthGeometricCalc, new PointEarthGeometric() { Longitude = xNext, Latitude = yNext, CoordinateUnits = CoordinateUnits.deg });



                        if ((distance_next > distance_prev) && (double.IsNaN(distance_next) == false))
                        {
                            pointResult.Longitude = xPrev;
                            pointResult.Latitude = yPrev;
                        }
                        else if ((distance_next < distance_prev) && (double.IsNaN(distance_prev) == false))
                        {
                            pointResult.Longitude = xNext;
                            pointResult.Latitude = yNext;
                        }
                        else
                        {
                            if (double.IsNaN(distance_prev) == false)
                            {
                                pointResult.Longitude = xPrev;
                                pointResult.Latitude = yPrev;
                            }
                            if (double.IsNaN(distance_next) == false)
                            {
                                pointResult.Longitude = xNext;
                                pointResult.Latitude = yNext;
                            }
                        }

                        pointResult.CoordinateUnits = CoordinateUnits.deg;
                    }
                }
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
        public  PointEarthGeometric CalculationCoordinateByLengthAndAzimuth(in PointEarthGeometric PointStart, double distance_km, double azimuth, bool LargeCircleArc = true)
        {
            return CreateContourFromContureByDistanceСalculations.CalculationCoordinateByLengthAndAzimuth(in PointStart, distance_km, azimuth, LargeCircleArc);
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

                var coordRecalc = CalculationCoordinateByLengthAndAzimuth(in contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, d0_m, azimuth);
                var calcFieldStrength = calcFieldStrengths(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                var d1_m = d0_m;
                while (true)
                {
                    if (calcFieldStrength < contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength)
                    {
                        d1_m += (d1_m / 2.0);
                        coordRecalc = CalculationCoordinateByLengthAndAzimuth(in contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, d1_m, azimuth);
                        calcFieldStrength = calcFieldStrengths(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                    }
                    else if (calcFieldStrength > contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength)
                    {
                        d1_m -= (d1_m / 2.0);
                        coordRecalc = CalculationCoordinateByLengthAndAzimuth(in contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, d1_m, azimuth);
                        calcFieldStrength = calcFieldStrengths(contourForStationByTriggerFieldStrengthsArgs.PointEarthGeometricCalc, coordRecalc);
                    }
                    if ((Math.Round(calcFieldStrength, 2) == contourForStationByTriggerFieldStrengthsArgs.TriggerFieldStrength) || (d1_m < mindistance_m))
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

        /// <summary>
        /// Вычисление точек контура по заданной точке, расстоянии от точки, набору азимутов и шаге между азимутами 
        /// </summary>
        /// <param name="contourFromPointByDistanceArgs"></param>
        /// <param name="pointResult"></param>
        /// <param name="sizeResultBuffer"></param>
        public void CreateContourFromPointByDistance(in ContourFromPointByDistanceArgs contourFromPointByDistanceArgs, ref PointEarthGeometric[] pointResult, out int sizeResultBuffer)
        {
            int index = 0;
            for (double azimuth = 0; azimuth < 360; azimuth = index * contourFromPointByDistanceArgs.Step_deg)
            {
                var coordRecalc = CalculationCoordinateByLengthAndAzimuth(in contourFromPointByDistanceArgs.PointEarthGeometricCalc, contourFromPointByDistanceArgs.Distance_km, azimuth);
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

        /// <summary>
        /// Проверка попадания заданной точки в контур
        /// </summary>
        /// <param name="checkHittingArgs"></param>
        /// <returns></returns>
        public bool CheckHitting(in CheckHittingArgs checkHittingArgs)
        {
            if (checkHittingArgs.Poligon == null || checkHittingArgs.Poligon.Length == 0)
                return false;


            bool hit = false; // количество пересечений луча слева в право четное = false, нечетное = true;
            for (int i = 0; i < checkHittingArgs.Poligon.Length - 1; i++)
            {
                if (((checkHittingArgs.Poligon[i].Latitude <= checkHittingArgs.Point.Latitude) && ((checkHittingArgs.Poligon[i + 1].Latitude > checkHittingArgs.Point.Latitude))) || ((checkHittingArgs.Poligon[i].Latitude > checkHittingArgs.Point.Latitude) && ((checkHittingArgs.Poligon[i + 1].Latitude <= checkHittingArgs.Point.Latitude))))
                {
                    if ((checkHittingArgs.Poligon[i].Longitude > checkHittingArgs.Point.Longitude) && (checkHittingArgs.Poligon[i + 1].Longitude > checkHittingArgs.Point.Longitude))
                    {
                        hit = !hit;
                    }
                    else if (!((checkHittingArgs.Poligon[i].Longitude < checkHittingArgs.Point.Longitude) && (checkHittingArgs.Poligon[i + 1].Longitude < checkHittingArgs.Point.Longitude)))
                    {
                        if (checkHittingArgs.Point.Longitude < checkHittingArgs.Poligon[i + 1].Longitude - (checkHittingArgs.Point.Latitude - checkHittingArgs.Poligon[i + 1].Latitude) * (checkHittingArgs.Poligon[i + 1].Longitude - checkHittingArgs.Poligon[i].Longitude) / (checkHittingArgs.Poligon[i].Latitude - checkHittingArgs.Poligon[i + 1].Latitude))
                        {
                            hit = !hit;
                        }
                    }
                }
            }
            int i_ = checkHittingArgs.Poligon.Length - 1;
            if (((checkHittingArgs.Poligon[i_].Latitude <= checkHittingArgs.Point.Latitude) && ((checkHittingArgs.Poligon[0].Latitude > checkHittingArgs.Point.Latitude))) || ((checkHittingArgs.Poligon[i_].Latitude > checkHittingArgs.Point.Latitude) && ((checkHittingArgs.Poligon[0].Latitude <= checkHittingArgs.Point.Latitude))))
            {
                if ((checkHittingArgs.Poligon[i_].Longitude > checkHittingArgs.Point.Longitude) && (checkHittingArgs.Poligon[0].Longitude > checkHittingArgs.Point.Longitude))
                {
                    hit = !hit;
                }
                else if (!((checkHittingArgs.Poligon[i_].Longitude < checkHittingArgs.Point.Longitude) && (checkHittingArgs.Poligon[0].Longitude < checkHittingArgs.Point.Longitude)))
                {
                    if (checkHittingArgs.Point.Longitude < checkHittingArgs.Poligon[0].Longitude - (checkHittingArgs.Point.Latitude - checkHittingArgs.Poligon[0].Latitude) * (checkHittingArgs.Poligon[0].Longitude - checkHittingArgs.Poligon[i_].Longitude) / (checkHittingArgs.Poligon[i_].Latitude - checkHittingArgs.Poligon[0].Latitude))
                    {
                        hit = !hit;
                    }
                }
            }

            return hit;
        }



        /// <summary>
        /// Функция по формированию контура от контура
        /// </summary>
        /// <param name="contourFromContureByDistanceArgs"></param>
        /// <param name="pointEarthGeometricWithAzimuth"></param>
        /// <param name="sizeResultBuffer"></param>
        public void CreateContourFromContureByDistance(in ContourFromContureByDistanceArgs contourFromContureByDistanceArgs, ref PointEarthGeometricWithAzimuth[] pointEarthGeometricWithAzimuth, out int sizeResultBuffer)
        {
            CreateContourFromContureByDistanceСalculations.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricWithAzimuth, out sizeResultBuffer);
        }


    }
}
