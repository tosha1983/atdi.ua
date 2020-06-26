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

                if (distance_next > distance_prev)
                {
                    pointResult.Longitude = xPrev;
                    pointResult.Latitude = yPrev;
                }
                else
                {
                    pointResult.Longitude = xNext;
                    pointResult.Latitude = yNext;
                }
                pointResult.CoordinateUnits = CoordinateUnits.deg;
            }
        }

        public  void Ortho(double x1, double y1,
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
        public PointEarthGeometric CalculationCoordinateByLengthAndAzimuth(in PointEarthGeometric PointStart, double distance_km, double azimuth, bool LargeCircleArc = true)
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
                point.Longitude = PointStart.Longitude + (distance_km/1000.0)*Math.Sin(azimuth * Math.PI / 180.0);
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
        /// По заданному расстоянию 'distance' от точки барицентра 'pointEarthGeometricBaryCentr' и заданному азимуту 'azimuth' выполняется
        /// последовательная проверка на пересечение с каким либо отрезком контура 'contourPoints'
        /// </summary>
        /// <param name="pointEarthGeometricBaryCentr"></param>
        /// <param name="contourPoints"></param>
        /// <param name="distance"></param>
        /// <param name="azimuth"></param>
        /// <returns>Точка пересечения</returns>
        private PointEarthGeometricWithAzimuth[] CheckInterSectPoint(PointEarthGeometric pointEarthGeometricBaryCentr, PointEarthGeometric[] contourPoints, double distance, double azimuth)
        {
            var pointEarthGeometricWithAzimuth = new List<PointEarthGeometricWithAzimuth>();
            var pointEarthGeometric = new PointEarthGeometric(pointEarthGeometricBaryCentr.Longitude, pointEarthGeometricBaryCentr.Latitude, CoordinateUnits.deg);
            var coordRecalcNew = CalculationCoordinateByLengthAndAzimuth(in pointEarthGeometric, 30000, azimuth, false);
            var lineNew = new LineEarthGeometric()
            {
                PointEarthGeometric1 = pointEarthGeometricBaryCentr,
                PointEarthGeometric2 = coordRecalcNew
            };

            for (int j = 0; j < contourPoints.Length - 1; j++)
            {
                var lineR = new LineEarthGeometric()
                {
                    PointEarthGeometric1 = contourPoints[j],
                    PointEarthGeometric2 = contourPoints[j + 1]
                };

                bool lines_intersect_, segments_intersect_;
                PointEarthGeometric poi_, close1_, close2_;
                FindIntersection(lineR.PointEarthGeometric1, lineR.PointEarthGeometric2, lineNew.PointEarthGeometric1, lineNew.PointEarthGeometric2, out lines_intersect_, out segments_intersect_, out poi_, out close1_, out close2_);
                if (segments_intersect_)
                {
                    pointEarthGeometricWithAzimuth.Add(new PointEarthGeometricWithAzimuth()
                    {
                        PointEarthGeometric = new PointEarthGeometric() { Longitude = poi_.Longitude, Latitude = poi_.Latitude, CoordinateUnits = CoordinateUnits.deg },
                        Azimuth_deg = azimuth
                    });
                }
            }

            var lineR_ = new LineEarthGeometric()
            {
                PointEarthGeometric1 = contourPoints[contourPoints.Length - 1],
                PointEarthGeometric2 = contourPoints[0]
            };

            bool lines_intersect, segments_intersect;
            PointEarthGeometric poi, close1, close2;
            FindIntersection(lineR_.PointEarthGeometric1, lineR_.PointEarthGeometric2, lineNew.PointEarthGeometric1, lineNew.PointEarthGeometric2, out lines_intersect, out segments_intersect, out poi, out close1, out close2);
            if (segments_intersect)
            {
                pointEarthGeometricWithAzimuth.Add(new PointEarthGeometricWithAzimuth()
                {
                    PointEarthGeometric = new PointEarthGeometric() { Longitude = poi.Longitude, Latitude = poi.Latitude, CoordinateUnits = CoordinateUnits.deg },
                    Azimuth_deg = azimuth
                });
            }

            return pointEarthGeometricWithAzimuth.ToArray();
        }



        private void FindIntersection(
            PointEarthGeometric p1, PointEarthGeometric p2, PointEarthGeometric p3, PointEarthGeometric p4,
            out bool lines_intersect, out bool segments_intersect,
            out PointEarthGeometric intersection,
            out PointEarthGeometric close_p1, out PointEarthGeometric close_p2)
        {

            double dx12 = p2.Longitude - p1.Longitude;
            double dy12 = p2.Latitude - p1.Latitude;
            double dx34 = p4.Longitude - p3.Longitude;
            double dy34 = p4.Latitude - p3.Latitude;


            double denominator = (dy12 * dx34 - dx12 * dy34);

            double t1 =
                ((p1.Longitude - p3.Longitude) * dy34 + (p3.Latitude - p1.Latitude) * dx34)
                    / denominator;
            if (double.IsInfinity(t1))
            {

                lines_intersect = false;
                segments_intersect = false;
                intersection = new PointEarthGeometric(double.NaN, double.NaN,
                        CoordinateUnits.deg);
                close_p1 = new PointEarthGeometric(double.NaN, double.NaN,
                        CoordinateUnits.deg);
                close_p2 = new PointEarthGeometric(double.NaN, double.NaN,
                        CoordinateUnits.deg);
                return;
            }
            lines_intersect = true;

            double t2 =
                ((p3.Longitude - p1.Longitude) * dy12 + (p1.Latitude - p3.Latitude) * dx12)
                    / -denominator;


            intersection = new PointEarthGeometric(p1.Longitude + dx12 * t1, p1.Latitude + dy12 * t1,
                        CoordinateUnits.deg);


            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));


            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new PointEarthGeometric(p1.Longitude + dx12 * t1, p1.Latitude + dy12 * t1,
                        CoordinateUnits.deg);
            close_p2 = new PointEarthGeometric(p3.Longitude + dx34 * t2, p3.Latitude + dy34 * t2,
                        CoordinateUnits.deg);
        }


        public void CreateContourFromContureByDistance(in ContourFromContureByDistanceArgs contourFromContureByDistanceArgs, ref PointEarthGeometricWithAzimuth[] pointEarthGeometricWithAzimuth, out int sizeResultBuffer)
        {
            //int maxCountIteration = 20;
            int iterNum = 0;
            int index = 0;

            double s = contourFromContureByDistanceArgs.Distance_km;


            var lstPointEarthGeometricWithAzimuth = new List<PointEarthGeometric>();
            for (double azimuth = 0; azimuth < 360; azimuth = index * contourFromContureByDistanceArgs.Step_deg)
            {
                var pt = contourFromContureByDistanceArgs.PointBaryCenter;
                var coordInterSect = CheckInterSectPoint(pt, contourFromContureByDistanceArgs.ContourPoints, s, azimuth);

                for (int i = 0; i < coordInterSect.Length; i++)
                {
                    lstPointEarthGeometricWithAzimuth.Add(new PointEarthGeometric()
                    {
                        Longitude = coordInterSect[i].PointEarthGeometric.Longitude,
                        Latitude = coordInterSect[i].PointEarthGeometric.Latitude,
                        CoordinateUnits = CoordinateUnits.deg
                    });

                    pointEarthGeometricWithAzimuth[iterNum] = new PointEarthGeometricWithAzimuth()
                    {
                        PointEarthGeometric = new PointEarthGeometric()
                        {
                            Longitude = coordInterSect[i].PointEarthGeometric.Longitude,
                            Latitude = coordInterSect[i].PointEarthGeometric.Latitude
                        },
                        Azimuth_deg = azimuth
                    };
                    iterNum++;
                }
                index++;
            }
            sizeResultBuffer = iterNum + 1;
        }
    }
}
