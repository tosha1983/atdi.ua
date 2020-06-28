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
                var curNearestPoint = points[0];
                var curNearestDistance = GetDistance_km(in pointEarthGeometricCalc, in curNearestPoint);
                for (int i = 0; i < points.Length; i++)
                {
                    if (i > 0)
                    {
                        prevPointGeom = points[i - 1];
                    }
                    if (i < points.Length - 1)
                    {
                        nextPointGeom = points[i + 1];
                    }
                    var point = points[i];
                    var distance = GetDistance_km(pointEarthGeometricCalc, point);
                    if (distance < curNearestDistance)
                    {
                        curNearestDistance = distance;
                        curNearestPoint = point;
                    }
                }


                Ortho(prevPointGeom.Longitude, prevPointGeom.Latitude,
                                            curNearestPoint.Longitude, curNearestPoint.Latitude,
                                           pointEarthGeometricCalc.Longitude, pointEarthGeometricCalc.Latitude,
                                           out double xPrev, out double yPrev);

                Ortho(nextPointGeom.Longitude, nextPointGeom.Latitude,
                                            curNearestPoint.Longitude, curNearestPoint.Latitude,
                                           pointEarthGeometricCalc.Longitude, pointEarthGeometricCalc.Latitude,
                                           out double xNext, out double yNext);

                var distance_prev = GetDistance_km(pointEarthGeometricCalc, new PointEarthGeometric() { Longitude = xPrev, Latitude = yPrev, CoordinateUnits = CoordinateUnits.deg });
                var distance_next = GetDistance_km(pointEarthGeometricCalc, new PointEarthGeometric() { Longitude = xNext, Latitude = yNext, CoordinateUnits = CoordinateUnits.deg });

                

                if ((distance_next > distance_prev) && (double.IsNaN(distance_next)==false))
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
                    if (double.IsNaN(distance_prev)==false)
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

        public void Ortho(double x1, double y1,
                                   double x2, double y2,
                                   double x3, double y3,
                                   out double x, out double y)
        {

            var x1y1 = new PointEarthGeometric(x1, y1, CoordinateUnits.deg);
            var x2y2 = new PointEarthGeometric(x2, y2, CoordinateUnits.deg);
            var x3y3 = new PointEarthGeometric(x3, y3, CoordinateUnits.deg);

            var c = GetDistance_km(x1y1, x2y2);
            var b = GetDistance_km(x2y2, x3y3);
            var a = GetDistance_km(x3y3, x1y1);


            var p = (a + b + c) / 2;

            var h = 2 * Math.Sqrt(p * (p - a) * (p - b) * (p - c)) / c;

            if (a / c > b / c)
            {
                var c1 = Math.Sqrt(a * a - h * h);
                var k = c1 / c;
                x = x1 + k * (x2 - x1);
                y = y1 + k * (y2 - y1);

            }
            else
            {
                var c2 = Math.Sqrt(b * b - h * h);
                var k = c2 / c;
                x = x2 + k * (x1 - x2);
                y = y2 + k * (y1 - y2);

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
            var point = new PointEarthGeometric();

            if (PointStart.CoordinateUnits == CoordinateUnits.deg)
            {
                var latitude = PointStart.Latitude;
                var longitude = PointStart.Longitude;
                if (LargeCircleArc)
                {
                    double arcDist = distance_km / re_km;
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
                    point.Longitude = longitude + distance_km * Math.Sin(azimuth * Math.PI / 180.0) / Math.Cos(latitude * Math.PI / 180.0) / (re_km * Math.PI / 180.0);
                    point.Latitude = latitude + distance_km * Math.Cos(azimuth * Math.PI / 180.0) / (re_km * Math.PI / 180.0);
                    point.CoordinateUnits = CoordinateUnits.deg;
                }
                return point;
            }
            else
            {//Не проверенно
                point.Longitude = PointStart.Longitude + (distance_km / 1000.0) * Math.Sin(azimuth * Math.PI / 180.0);
                point.Latitude = PointStart.Latitude + (distance_km / 1000.0) * Math.Cos(azimuth * Math.PI / 180.0); ;
                point.CoordinateUnits = CoordinateUnits.m;
                return point;
            }
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
        /// Функция по формированию контура от контура
        /// </summary>
        /// <param name="contourFromContureByDistanceArgs"></param>
        /// <param name="pointEarthGeometricWithAzimuth"></param>
        /// <param name="sizeResultBuffer"></param>
        public void CreateContourFromContureByDistance(in ContourFromContureByDistanceArgs contourFromContureByDistanceArgs, ref PointEarthGeometricWithAzimuth[] pointEarthGeometricWithAzimuth, out int sizeResultBuffer)
        {
            int iterNum = 0;
            int index = 0;
            double s = contourFromContureByDistanceArgs.Distance_km;
            var baryCenter = contourFromContureByDistanceArgs.PointBaryCenter;
            for (double azimuth = 0; azimuth < 360; azimuth = index * contourFromContureByDistanceArgs.Step_deg)
            {
                var coordInterSect = CheckInterSectPoint(baryCenter, contourFromContureByDistanceArgs.ContourPoints, s, azimuth);
                var pointEarthGeometric = new PointEarthGeometric();
                if ((coordInterSect != null) && (coordInterSect.Length > 0))
                {
                    var pointResult = new PointEarthGeometric() { Longitude = coordInterSect[0].PointEarthGeometric.Longitude, Latitude = coordInterSect[0].PointEarthGeometric.Latitude, CoordinateUnits = CoordinateUnits.deg };
                    var distance = GetDistance_km(in pointResult, in baryCenter);
                    for (int h = 0; h < coordInterSect.Length; h++)
                    {
                        pointResult = new PointEarthGeometric() { Longitude = coordInterSect[h].PointEarthGeometric.Longitude, Latitude = coordInterSect[h].PointEarthGeometric.Latitude, CoordinateUnits = CoordinateUnits.deg };
                        if  (GetDistance_km(in pointResult, in baryCenter) >= distance)
                        {
                            distance = GetDistance_km(in pointResult, in baryCenter);
                            pointEarthGeometric = pointResult;
                        }
                    }
                }
                var pointEarthGeometricRecalc = CalculationCoordinateByLengthAndAzimuth(in pointEarthGeometric, s, azimuth);
                var dist = GetDistance_km(in pointEarthGeometricRecalc, in pointEarthGeometric);
                System.Diagnostics.Debug.WriteLine($" distance - {dist};  ");
                pointEarthGeometricWithAzimuth[iterNum] = new PointEarthGeometricWithAzimuth() { Azimuth_deg = azimuth, PointEarthGeometric = pointEarthGeometricRecalc };
                iterNum++;
                index++;
            }
            sizeResultBuffer = iterNum;
        }

        /// <summary>
        /// По заданному расстоянию 'distance' от точки барицентра 'pointEarthGeometricBaryCentr' и заданному азимуту 'azimuth' выполняется
        /// последовательная проверка на пересечение с каким либо отрезком контура 'contourPoints'
        /// </summary>
        /// <param name="pointEarthGeometricBaryCentr"></param>
        /// <param name="contourPoints"></param>
        /// <param name="distance"></param>
        /// <param name="azimuth"></param>
        /// <returns>Точка пересечения</returns>
        public PointEarthGeometricWithAzimuth[] CheckInterSectPoint(PointEarthGeometric pointEarthGeometricBaryCentr, PointEarthGeometric[] contourPoints, double distance, double azimuth)
        {
            var maxDistance_Km = 40000;
            var pointEarthGeometricWithAzimuth = new List<PointEarthGeometricWithAzimuth>();
            var pointEarthGeometric = new PointEarthGeometric(pointEarthGeometricBaryCentr.Longitude, pointEarthGeometricBaryCentr.Latitude, CoordinateUnits.deg);
            var coordRecalcNew = CalculationCoordinateByLengthAndAzimuth(in pointEarthGeometric, maxDistance_Km, azimuth, false);
            var lineNew = new LineEarthGeometric()
            {
                PointEarthGeometric1 = pointEarthGeometricBaryCentr,
                PointEarthGeometric2 = coordRecalcNew
            };

            var lineEarthGeometric = new LineEarthGeometric()
            {
                PointEarthGeometric1 = contourPoints[contourPoints.Length - 1],
                PointEarthGeometric2 = contourPoints[0]
            };

            bool linesIntersect, segmentsIntersect;
            PointEarthGeometric poi;
            CreateContourFromContureByDistanceСalculations.FindIntersection(lineEarthGeometric.PointEarthGeometric1, lineEarthGeometric.PointEarthGeometric2, lineNew.PointEarthGeometric1, lineNew.PointEarthGeometric2, out linesIntersect, out segmentsIntersect, out poi);
            if (segmentsIntersect)
            {
                pointEarthGeometricWithAzimuth.Add(new PointEarthGeometricWithAzimuth()
                {
                    PointEarthGeometric = new PointEarthGeometric() { Longitude = poi.Longitude, Latitude = poi.Latitude, CoordinateUnits = CoordinateUnits.deg },
                    Azimuth_deg = azimuth
                });
            }

            for (int j = 0; j < contourPoints.Count() - 1; j++)
            {
                var line = new LineEarthGeometric()
                {
                    PointEarthGeometric1 = contourPoints[j],
                    PointEarthGeometric2 = contourPoints[j + 1]
                };

                linesIntersect = false; segmentsIntersect = false;
                poi = new PointEarthGeometric();

                CreateContourFromContureByDistanceСalculations.FindIntersection(line.PointEarthGeometric1, line.PointEarthGeometric2, lineNew.PointEarthGeometric1, lineNew.PointEarthGeometric2, out linesIntersect, out segmentsIntersect, out poi);
                if (segmentsIntersect)
                {
                    pointEarthGeometricWithAzimuth.Add(new PointEarthGeometricWithAzimuth()
                    {
                        PointEarthGeometric = new PointEarthGeometric() { Longitude = poi.Longitude, Latitude = poi.Latitude, CoordinateUnits = CoordinateUnits.deg },
                        Azimuth_deg = azimuth
                    });
                }
            }
            return pointEarthGeometricWithAzimuth.ToArray();
        }
    }
}
