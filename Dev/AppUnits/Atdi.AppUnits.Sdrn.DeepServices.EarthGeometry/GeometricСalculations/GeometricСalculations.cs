using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;


namespace Atdi.AppUnits.Sdrn.DeepServices.EarthGeometry
{
    public static class GeometricСalculations
    {
        public const double re_km = 6371.0;
        public static double GetDistance_km(double x1, double y1, double x2, double y2, CoordinateUnits coordinateUnits = CoordinateUnits.m)
        { //Надо проверить - дистанция и преобразование координат совпадают с телекомом
            double d = 0;
            if (coordinateUnits == CoordinateUnits.m)
            {
                d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) / 1000.0;
            }
            else
            {
                double dlon = x2 - x1;
                double r = Math.Sin(y1 * Math.PI / 180) * Math.Sin(y2 * Math.PI / 180) + Math.Cos(y1 * Math.PI / 180) * Math.Cos(y2 * Math.PI / 180) * Math.Cos(dlon * Math.PI / 180);
                double angle = Math.Acos(r);
                d = angle * re_km;
            }
            return d;
        }
        public static double GetAzimut(double xpoint, double ypoint, double xtarget, double ytarget, CoordinateUnits coordinateUnits = CoordinateUnits.m)
        { //Надо проверить - азимут совпадает с телекомом
            double az = 0;
            if (coordinateUnits == CoordinateUnits.m)
            {
                double dy = ytarget - ypoint;
                double dx = xtarget - xpoint;
                if (dx == 0) { if (dy >= 0) { az = 0; } else { az = 180; } }
                else { az = 90 - 180 * Math.Atan(dy / dx) / Math.PI; }
                if (dx < 0) { az = az + 180; }
            }
            else
            {
                //double lon1 = xtarget * Math.PI / 180;
                //double lat1 = ytarget * Math.PI / 180;
                //double lon2 = xpoint * Math.PI / 180;
                //double lat2 = ypoint * Math.PI / 180;
                //double cl1 = Math.Cos(lat1);
                //double cl2 = Math.Cos(lat2);
                //double sl1 = Math.Sin(lat1);
                //double sl2 = Math.Sin(lat2);
                //double delta = lon2 - lon1;
                //double cdelta = Math.Cos(delta);
                //double sdelta = Math.Sin(delta);
                //double x = (cl1 * sl2) - (sl1 * cl2 * cdelta);
                //double y = sdelta * cl2;
                //double z = Math.Atan(-y/x)* 180 / Math.PI;
                //if (x < 0) { z = z + 180;}
                //az = z;

                double dlon = xtarget - xpoint;
                double r = Math.Sin(ypoint * Math.PI / 180) * Math.Sin(ytarget * Math.PI / 180) + Math.Cos(ypoint * Math.PI / 180) * Math.Cos(ytarget * Math.PI / 180) * Math.Cos(dlon * Math.PI / 180);
                double x1 = Math.Sin(ytarget * Math.PI / 180) - Math.Sin(ypoint * Math.PI / 180) * r;
                double y1 = Math.Cos(ypoint * Math.PI / 180) * Math.Cos(ytarget * Math.PI / 180) * Math.Sin(dlon * Math.PI / 180);
                if ((Math.Abs(x1) < 0.000000001) && (Math.Abs(y1) < 0.000000001)) { az = xtarget; }
                else { az = Math.Atan2(y1, x1) * 180 / Math.PI; }
                if (az < 0) { az = az + 360; }
            }
            return az;
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
        /// Дает ответ есть ли пересечение между отрезком лучем на сфере
        /// </summary>
        /// <param name="StartPoint">Центр луча</param>
        /// <param name="Azimut">Азимут луча</param>
        /// <param name="SegmentPoint1">Первая точка отрезка</param>
        /// <param name="SegmentPoint2">Вторая точка отрезка</param>
        /// <param name="InterseptionPoint">точка пересечения если пересечение есть</param>
        /// <returns>Факт пересечения</returns>
        public static bool GetInterseptionOnSphere(PointEarthGeometric StartPoint, double Azimut, PointEarthGeometric SegmentPoint1, PointEarthGeometric SegmentPoint2, out PointEarthGeometric InterseptionPoint)
        { // Тестированно
          //проверка на совпадение координат
            InterseptionPoint = new PointEarthGeometric();
            if ((StartPoint.Longitude == SegmentPoint1.Longitude)&&(StartPoint.Latitude == SegmentPoint1.Latitude))
            {InterseptionPoint = StartPoint; return true;}
            if ((StartPoint.Longitude == SegmentPoint2.Longitude) && (StartPoint.Latitude == SegmentPoint2.Latitude))
            { InterseptionPoint = StartPoint; return true; }

            // определение азимутов на конци отрезков
            double Az1 = GetAzimut(StartPoint.Longitude, StartPoint.Latitude, SegmentPoint1.Longitude, SegmentPoint1.Latitude, CoordinateUnits.deg);
            double Az2 = GetAzimut(StartPoint.Longitude, StartPoint.Latitude, SegmentPoint2.Longitude, SegmentPoint2.Latitude, CoordinateUnits.deg);
            //Проверка попадаем ли мы между азимутами
            if (Math.Abs(Az2 - Az1) > 180)
            {// есть переход через 0
                if (Az1 > Az2)
                {
                    if ((Azimut < Az1) && (Azimut > Az2)) { return false; }
                }
                else
                {
                    if ((Azimut < Az2) && (Azimut > Az1)) { return false; }
                }
            }
            else
            {
                if (Az1 > Az2)
                {
                    if ((Azimut > Az1) || (Azimut < Az2)) { return false; }
                }
                else
                {
                    if ((Azimut > Az2) || (Azimut < Az1)) { return false; }
                }
            }
            // Проверка попадаем ли мы на какую либо точку отрезков
            if (Azimut == Az1) { InterseptionPoint = SegmentPoint1; return true; }
            if (Azimut == Az2) { InterseptionPoint = SegmentPoint2; return true; }
            double Az1_; PointEarthGeometric Point1; PointEarthGeometric Point2;
            if (Math.Abs(Azimut - Az1) > Math.Abs(Azimut - Az2))
            {
                Az1_ = Az1;
                Point1 = SegmentPoint1;
                Point2 = SegmentPoint2;
            }
            else
            {
                Az1_ = Az2;
                Point1 = SegmentPoint2;
                Point2 = SegmentPoint1;
            }
            double C = GetAngle(Azimut, Az1_); 
            double AzTo2 = GetAzimut(Point1.Longitude, Point1.Latitude, Point2.Longitude, Point2.Latitude, CoordinateUnits.deg);
            double AzToStart = GetAzimut(Point1.Longitude, Point1.Latitude, StartPoint.Longitude, StartPoint.Latitude, CoordinateUnits.deg);
            double B = GetAngle(AzTo2, AzToStart);
            double a = GetDistance_km(StartPoint.Longitude, StartPoint.Latitude, Point1.Longitude, Point1.Latitude, CoordinateUnits.deg);

            double Brad = B * Math.PI / 180;
            double Crad = C * Math.PI / 180;
            double arad = a / re_km;
            double cosa = Math.Cos(arad);
            double cosC = Math.Cos(Crad);
            double cosB = Math.Cos(Brad);
            double sinB = Math.Sin(Brad);
            double sinC= Math.Sin(Crad);
            double cosA = sinB * sinC * cosa - cosB * cosC;
            double sinA = Math.Sqrt(1-cosA*cosA);
            double cosb = (cosB + cosA * cosC) / (sinA * sinC);
            double b = re_km * Math.Abs(Math.Acos(cosb));
            InterseptionPoint = CalculationCoordinateByLengthAndAzimuth(StartPoint, b, Azimut);
            return true;
        }
        /// <summary>
        /// Функция по расчету минимального растояния к отрезку
        /// </summary>
        /// <param name="StartPoint">начальная точка отрезка</param>
        /// <param name="StopPoint">конечная точка отрезка</param>
        /// <param name="Point">точка для которой будем считать</param>
        /// <returns>Дистанция в км</returns>
        public static double GetCrossTrackDistanceOnSphere_km(PointEarthGeometric StartPoint, PointEarthGeometric StopPoint, PointEarthGeometric Point)
        {
            double DistToStart = GetDistance_km(StartPoint.Longitude, StartPoint.Latitude, Point.Longitude, Point.Latitude, CoordinateUnits.deg);
            double DistToStop = GetDistance_km(StopPoint.Longitude, StopPoint.Latitude, Point.Longitude, Point.Latitude, CoordinateUnits.deg);
            double DistBetw = GetDistance_km(StopPoint.Longitude, StopPoint.Latitude, StartPoint.Longitude, StartPoint.Latitude, CoordinateUnits.deg);
            if (DistToStart == 0) { return 0;}
            if (DistToStop == 0) { return 0; }
            if (DistBetw == 0) { return DistToStart; }
            PointEarthGeometric Start;
            PointEarthGeometric Stop;
            double c; double DistMin;
            if (DistToStart > DistToStop)
            {Start = StartPoint;Stop = StopPoint; c = DistToStart; DistMin = DistToStop; }
            else {Stop = StartPoint; Start = StopPoint; c = DistToStop; DistMin = DistToStart; }
            double AzToPoint = GetAzimut(Start.Longitude, Start.Latitude, Point.Longitude, Point.Latitude, CoordinateUnits.deg);
            double AzToStop = GetAzimut(Start.Longitude, Start.Latitude, Stop.Longitude, Stop.Latitude, CoordinateUnits.deg);
            double B = Math.Abs(AzToPoint - AzToStop);
            double Brad = B * Math.PI/180;
            double crad = c / re_km;
            double sinB = Math.Sin(Brad);
            double cosB = Math.Cos(Brad);
            double sinc = Math.Sin(crad);
            double tgc = Math.Tan(crad);
            double sinb = sinc * sinB;
            double brad = Math.Abs(Math.Asin(sinb));
            double tga = tgc * cosB;
            double arad = Math.Abs(Math.Atan(tga));
            double a = arad * re_km;
            if (DistBetw < a) { return DistMin; }
            else { return brad * re_km;}
        }
        private static double GetAngle(double Azimuth1, double Azimuth2)
        {
            double diff = Math.Abs(Azimuth1 - Azimuth2);
            if (diff > 180) { diff = 360 - diff; }
            return diff;
        }
    }

}
