using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using System.Windows;


namespace Atdi.AppUnits.Sdrn.DeepServices.EarthGeometry
{
    public static class CreateContourFromContureByDistanceСalculations
    {

        private const double re_km = 6371.0;
        /// <summary>
        /// Поиск координат пересечения отрезков
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="lines_intersect"></param>
        /// <param name="segments_intersect"></param>
        /// <param name="intersection"></param>
        public static void FindIntersection(
             PointEarthGeometric p1, PointEarthGeometric p2, PointEarthGeometric p3, PointEarthGeometric p4,
             out bool lines_intersect, out bool segments_intersect,
             out PointEarthGeometric intersection)
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

        }


        /// <summary>
        /// Перпендикуляр из точки pointEarthGeometricCalc на прямую, проходящую через точки (prevPointGeom или nextPointGeom) и curNearestPoint.
        /// (x1,y1), (x2,y2) - основание треугольника
        /// (x3,y3) - это наш перпендикуляр
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Ortho(double x1, double y1, // координаты точки prevPointGeom или nextPointGeom
                                   double x2, double y2, // координаты точки curNearestPoint
                                   double x3, double y3, // координаты точки pointEarthGeometricCalc
                                   out double x, out double y)
        {

            //В основе решения лежат свойства треугольника и уравнение прямой:
            //x = x1 + t * (x2 - x1);
            //y = y1 + t * (y2 - y1);

            var x1y1 = new PointEarthGeometric(x1, y1, CoordinateUnits.deg); // координаты точки prevPointGeom или nextPointGeom
            var x2y2 = new PointEarthGeometric(x2, y2, CoordinateUnits.deg); // координаты точки curNearestPoint
            var x3y3 = new PointEarthGeometric(x3, y3, CoordinateUnits.deg); // координаты точки pointEarthGeometricCalc

            // Длины отрезков

            var c = GeometricСalculations.GetDistance_km(x1y1.Longitude, x1y1.Latitude, x2y2.Longitude, x2y2.Latitude, x1y1.CoordinateUnits);  // расстояние между точкой (prevPointGeom или nextPointGeom) и точкой curNearestPoint
            var b = GeometricСalculations.GetDistance_km(x2y2.Longitude, x2y2.Latitude, x3y3.Longitude, x3y3.Latitude, x2y2.CoordinateUnits);  // расстояние между точкой curNearestPoint и точкой pointEarthGeometricCalc
            var a = GeometricСalculations.GetDistance_km(x3y3.Longitude, x3y3.Latitude, x1y1.Longitude, x1y1.Latitude, x3y3.CoordinateUnits); // расстояние между точкой pointEarthGeometricCalc и точкой (prevPointGeom или nextPointGeom)

            // Полупериметр треугольника
            var p = (a + b + c) / 2;

            // Высота (длина перпендикуляра)
            var h = 2 * Math.Sqrt(p * (p - a) * (p - b) * (p - c)) / c;

            // вычисление координат пересечения  перпендикуляра, опущенного с точки  pointEarthGeometricCalc на отрезок  из точек (prevPointGeom или nextPointGeom) и curNearestPoint
            // Рассмотрим два прямоугольных треугольника со сторонами a, h, c1 и b, h, c2
            //По формуле Пифагора - рассчитаем c1, c2.
            // Отношение с1/ c и будет являться t в уравнении прямой, но тут важно учесть частный случай треугольника с тупым углом
            //Т.к.тупой угол в треугольнике может быть только один, то мы выбираем более острый угол у основания, сравнивая отношения a / c и a/ b
            //Если a/ c > a / b то используем с1 / с, иначе c2/ c
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
        /// По заданному расстоянию 'distance' от точки барицентра 'pointEarthGeometricBaryCentr' и заданному азимуту 'azimuth' выполняется
        /// последовательная проверка на пересечение с каким либо отрезком контура 'contourPoints'
        /// </summary>
        /// <param name="pointEarthGeometricBaryCentr"></param>
        /// <param name="contourPoints"></param>
        /// <param name="distance"></param>
        /// <param name="azimuth"></param>
        /// <returns>Точка пересечения</returns>
        public static PointEarthGeometricWithAzimuth[] CheckInterSectPoint(PointEarthGeometric pointEarthGeometricBaryCentr, PointEarthGeometric[] contourPoints, double distance, double azimuth)
        {
            //формируем очень длинную линию для поиска всех внутренних пересечений отрезков контура
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
                    PointStart = contourPoints[contourPoints.Length - 1],
                    PointStop = contourPoints[0],
                    Azimuth_deg = azimuth
                });
            }

            for (int j = 0; j < contourPoints.Length - 1; j++)
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
                        PointStart = contourPoints[j],
                        PointStop = contourPoints[j + 1],
                        Azimuth_deg = azimuth
                    });
                }
            }
            return pointEarthGeometricWithAzimuth.ToArray();
        }

        public static PointEarthGeometric CalculationCoordinateByLengthAndAzimuth(in PointEarthGeometric PointStart, double distance_km, double azimuth, bool LargeCircleArc = true)
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

        /// <summary>
        ///  Функция по формированию контура от контура
        /// </summary>
        /// <param name="contourFromContureByDistanceArgs"></param>
        /// <param name="pointEarthGeometricWithAzimuth"></param>
        /// <param name="sizeResultBuffer"></param>
        public static void CreateContourFromContureByDistance(in ContourFromContureByDistanceArgs contourFromContureByDistanceArgs, ref PointEarthGeometricWithAzimuth[] pointEarthGeometricWithAzimuth, out int sizeResultBuffer)
        {
            int iterNum = 0;
            int index = 0;
            // расстояние на которое должны быть удален точки искомого контура
            double s = contourFromContureByDistanceArgs.Distance_km;
            // координаты барицентра
            var baryCenter = contourFromContureByDistanceArgs.PointBaryCenter;
            // цикл по азимутам для нахождения координат перечения
            for (double azimuth = 0; azimuth < 360; azimuth = index * contourFromContureByDistanceArgs.Step_deg)
            {
                // полученный массив точек пересечений
                var coordInterSect = CreateContourFromContureByDistanceСalculations.CheckInterSectPoint(baryCenter, contourFromContureByDistanceArgs.ContourPoints, s, azimuth);
                // искомая точка пересечения с контуром, которая расположена на максимальном удалении от барицентра "промежуточные точки пересечения отбрасываются"
                var pointEarthGeometric = new PointEarthGeometric();
                // если есть хотя бы одно пересечение
                if ((coordInterSect != null) && (coordInterSect.Length > 0))
                {
                    // поиск точки пересечения с максимальным удалением от барицентра
                    var pointResult = new PointEarthGeometric() { Longitude = coordInterSect[0].PointEarthGeometric.Longitude, Latitude = coordInterSect[0].PointEarthGeometric.Latitude, CoordinateUnits = CoordinateUnits.deg };


                    var distance = GeometricСalculations.GetDistance_km(pointResult.Longitude, pointResult.Latitude, baryCenter.Longitude, baryCenter.Latitude, pointResult.CoordinateUnits);
                    for (int h = 0; h < coordInterSect.Length; h++)
                    {
                        pointResult = new PointEarthGeometric() { Longitude = coordInterSect[h].PointEarthGeometric.Longitude, Latitude = coordInterSect[h].PointEarthGeometric.Latitude, CoordinateUnits = CoordinateUnits.deg };

                        if (GeometricСalculations.GetDistance_km(pointResult.Longitude, pointResult.Latitude, baryCenter.Longitude, baryCenter.Latitude, pointResult.CoordinateUnits) >= distance)
                        {
                            distance = GeometricСalculations.GetDistance_km(pointResult.Longitude, pointResult.Latitude, baryCenter.Longitude, baryCenter.Latitude, pointResult.CoordinateUnits);
                            pointEarthGeometric = pointResult;
                        }
                    }
                }
                // увеличиваем расстояние от точки пересечения pointEarthGeometric на s по значению азимута azimuth
                var pointEarthGeometricRecalc = CalculationCoordinateByLengthAndAzimuth(in pointEarthGeometric, s, azimuth);
                // копируем полученное значение координаты в итоговый массив pointEarthGeometricWithAzimuth
                pointEarthGeometricWithAzimuth[iterNum] = new PointEarthGeometricWithAzimuth() { Azimuth_deg = azimuth, PointEarthGeometric = pointEarthGeometricRecalc };
                iterNum++;
                index++;
            }
            sizeResultBuffer = iterNum;
        }
    }
}
