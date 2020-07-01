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
        private static PointEarthGeometricWithAzimuth[] CheckInterSectPoint(PointEarthGeometric pointEarthGeometricBaryCentr, PointEarthGeometric[] contourPoints, double distance, double azimuth)
        {
            //формируем очень длинную линию для поиска всех внутренних пересечений отрезков контура
            var maxDistance_Km = 40000;
            var pointEarthGeometricWithAzimuth = new List<PointEarthGeometricWithAzimuth>();
            var pointEarthGeometric = new PointEarthGeometric(pointEarthGeometricBaryCentr.Longitude, pointEarthGeometricBaryCentr.Latitude, CoordinateUnits.deg);
            var coordRecalcNew = GeometricСalculations.CalculationCoordinateByLengthAndAzimuth(in pointEarthGeometric, maxDistance_Km, azimuth, false);
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
        private static PointEarthGeometric GetPointInterseption(PointEarthGeometric BaryCentr, in PointEarthGeometric[] contourPoints,  double Azimut, out double dist_km, bool FarthestPoint = true)
        {//не тестированно
            double FirstAngle = GeometricСalculations.GetAzimut(BaryCentr.Longitude, BaryCentr.Latitude, contourPoints[0].Longitude, contourPoints[0].Longitude, CoordinateUnits.deg);
            PointEarthGeometric point = new PointEarthGeometric(); double dist = 99999;
            bool FlagInter = false; double SecondAngle; bool inter;
            for (int i = 1; contourPoints.Length < i; i++)
            {
                SecondAngle = GeometricСalculations.GetAzimut(BaryCentr.Longitude, BaryCentr.Latitude, contourPoints[i].Longitude, contourPoints[i].Longitude, CoordinateUnits.deg);
                // проверка попадания угла 
                inter = ChackInterAngle(FirstAngle, SecondAngle, Azimut);
                if (inter)
                {
                    UpdatePoint(BaryCentr, in contourPoints, i-1, i, Azimut, FarthestPoint, ref dist, ref FlagInter, ref point);
                }
                FirstAngle = SecondAngle;
            }
            SecondAngle = GeometricСalculations.GetAzimut(BaryCentr.Longitude, BaryCentr.Latitude, contourPoints[0].Longitude, contourPoints[0].Longitude, CoordinateUnits.deg);
            // проверка попадания угла 
            inter = ChackInterAngle(FirstAngle, SecondAngle, Azimut);
            if (inter)
            {
                UpdatePoint(BaryCentr, in contourPoints, contourPoints.Length-1, 0, Azimut, FarthestPoint, ref dist, ref FlagInter, ref point);
            }
            dist_km = dist;
            return point;
        }
        private static void UpdatePoint(PointEarthGeometric BaryCentr, in PointEarthGeometric[] contourPoints, int i, int j, double Azimut, bool FarthestPoint, ref double dist, ref bool FlagInter, ref PointEarthGeometric point)
        {
            bool inter = GeometricСalculations.GetInterseptionOnSphere(BaryCentr, Azimut, contourPoints[i], contourPoints[j], out PointEarthGeometric IntersaptionPoint);
            if (FlagInter)
            {
                double newdist = GeometricСalculations.GetDistance_km(IntersaptionPoint.Longitude, IntersaptionPoint.Latitude, BaryCentr.Longitude, BaryCentr.Latitude, CoordinateUnits.deg);
                if (FarthestPoint)
                {
                    if (newdist > dist)
                    {
                        dist = newdist;
                        point = IntersaptionPoint;
                    }
                }
                else
                {
                    if (newdist < dist)
                    {
                        dist = newdist;
                        point = IntersaptionPoint;
                    }
                }
            }
            else
            {
                point = IntersaptionPoint;
                dist = GeometricСalculations.GetDistance_km(point.Longitude, point.Latitude, BaryCentr.Longitude, BaryCentr.Latitude, CoordinateUnits.deg);
                FlagInter = true;
            }
        }
        private static bool ChackInterAngle(double FirstAngle, double SecondAngle, double Azimut)
        {
            bool inter = true;
            if (Math.Abs(SecondAngle - FirstAngle) > 180)
            {// есть переход через 0
                if (FirstAngle > SecondAngle)
                {
                    if ((Azimut < FirstAngle) && (Azimut > SecondAngle)) { inter = false; }
                }
                else
                {
                    if ((Azimut < SecondAngle) && (Azimut > FirstAngle)) { inter = false; }
                }
            }
            else
            {
                if (FirstAngle > SecondAngle)
                {
                    if ((Azimut > FirstAngle) || (Azimut < SecondAngle)) { inter = false; }
                }
                else
                {
                    if ((Azimut > SecondAngle) || (Azimut < FirstAngle)) { inter = false; }
                }
            }
            return inter;
        }
        private static double GetMinDistanceToConture(in PointEarthGeometric[] contourPoints, PointEarthGeometric point)
        {// пока тупа перебераем все точки, но это пиздец тупа, может оптимизирую
            double Min_Dist = 999999; double newdist;
            for (int i = 1;i<contourPoints.Length ; i++)
            {
                newdist = GeometricСalculations.GetCrossTrackDistanceOnSphere_km(contourPoints[i - 1], contourPoints[i], point);
                if (newdist < Min_Dist) { Min_Dist = newdist;}
            }
            newdist = GeometricСalculations.GetCrossTrackDistanceOnSphere_km(contourPoints[0], contourPoints[contourPoints.Length-1], point);
            if (newdist < Min_Dist) { Min_Dist = newdist; }
            return Min_Dist;
        }
        /// <summary>
        ///  Функция по формированию контура от контура
        /// </summary>
        /// <param name="contourFromContureByDistanceArgs"></param>
        /// <param name="pointEarthGeometricWithAzimuth"></param>
        /// <param name="sizeResultBuffer"></param>
        public static void CreateContourFromContureByDistance(in ContourFromContureByDistanceArgs contourFromContureByDistanceArgs, ref PointEarthGeometric[] ResultPoint, out int sizeResultBuffer)
        {
            int MaxIterNum = 30;
            double DistanseError_km = 0.05;
            int index=0;

            // расстояние на которое должны быть удален точки искомого контура
            double s = contourFromContureByDistanceArgs.Distance_km;
            // координаты барицентра
            var baryCenter = contourFromContureByDistanceArgs.PointBaryCenter;
            // цикл по азимутам для нахождения координат перечения

            for (double azimuth = 0; azimuth < 360; azimuth = index * contourFromContureByDistanceArgs.Step_deg)
            {
                // нахождение точки пересечения по данному азимуту
                var PointX = GetPointInterseption(baryCenter, in contourFromContureByDistanceArgs.ContourPoints, azimuth, out double dist_toX);
                // по данному азимуту находим точку на удалении 2s от точки x
                // далее находим точку которая удалена более чем на s от контура
                PointEarthGeometric CurrentPoint = new PointEarthGeometric();
                double DistToPoligon;
                double CurentDistOnAz = dist_toX + s;
                do
                {
                    CurentDistOnAz = CurentDistOnAz + s;
                    CurrentPoint = GeometricСalculations.CalculationCoordinateByLengthAndAzimuth(baryCenter, CurentDistOnAz, azimuth);
                    DistToPoligon = GetMinDistanceToConture(in contourFromContureByDistanceArgs.ContourPoints, CurrentPoint);
                }
                while (DistToPoligon < s);
                int iterNum = 0;
                do
                {
                    iterNum++;
                    double diff = DistToPoligon - s;
                    CurentDistOnAz = CurentDistOnAz - diff;
                    CurrentPoint = GeometricСalculations.CalculationCoordinateByLengthAndAzimuth(baryCenter, CurentDistOnAz, azimuth);
                    DistToPoligon = GetMinDistanceToConture(in contourFromContureByDistanceArgs.ContourPoints, CurrentPoint);
                }
                while ((Math.Abs(DistToPoligon - s)< DistanseError_km)&&(iterNum < MaxIterNum));
                // копируем полученное значение координаты в итоговый массив pointEarthGeometricWithAzimuth
                ResultPoint[index] = CurrentPoint;
                index++;
            }
            sizeResultBuffer = index++;
        }
    }
}
