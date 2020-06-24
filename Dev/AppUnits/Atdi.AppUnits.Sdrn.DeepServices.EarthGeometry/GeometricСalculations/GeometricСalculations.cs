using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;


namespace Atdi.AppUnits.Sdrn.DeepServices.EarthGeometry
{
    internal static class GeometricСalculations
    {
        public const double re = 6371;
        public static double GetDistance_km(in PointEarthGeometricArgs sourcePointAgs, in PointEarthGeometricArgs targetPointArgs, CoordinateUnits coordinateUnits = CoordinateUnits.m)
        { //Надо проверить - дистанция и преобразование координат совпадают с телекомом
            double d = 0;
            if (coordinateUnits == CoordinateUnits.m)
            {
                d = Math.Sqrt((sourcePointAgs.Longitude - targetPointArgs.Longitude) * (sourcePointAgs.Longitude - targetPointArgs.Longitude) + (sourcePointAgs.Latitude - targetPointArgs.Latitude) * (sourcePointAgs.Latitude - targetPointArgs.Latitude)) /1000.0;
            }
            else
            {
                double dlon = targetPointArgs.Longitude - sourcePointAgs.Longitude;
                double r = Math.Sin(sourcePointAgs.Latitude * Math.PI / 180) * Math.Sin(targetPointArgs.Latitude * Math.PI / 180) + Math.Cos(sourcePointAgs.Latitude * Math.PI / 180) * Math.Cos(targetPointArgs.Latitude * Math.PI / 180) * Math.Cos(dlon * Math.PI / 180);
                double angle = 180 * Math.Acos(r) / Math.PI;
                d = angle * re;
                //var dLat = Deg2Rad(targetPointArgs.Latitude - sourcePointAgs.Latitude);
                //var dLon = Deg2Rad(targetPointArgs.Longitude - sourcePointAgs.Longitude);
                //var a =  Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +  Math.Cos(Deg2Rad(sourcePointAgs.Latitude)) * Math.Cos(Deg2Rad(targetPointArgs.Latitude)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                //var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                //d = re * c;
            }
            return d;
        }

        private static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }


        public static double GetAzimut(in PointEarthGeometricArgs sourcePointAgs, in PointEarthGeometricArgs targetPointArgs, CoordinateUnits coordinateUnits = CoordinateUnits.m)
        { //Надо проверить - азимут совпадает с телекомом
            double az = 0;
            if (coordinateUnits == CoordinateUnits.m)
            {
                double dy = targetPointArgs.Latitude - sourcePointAgs.Latitude;
                double dx = targetPointArgs.Longitude - sourcePointAgs.Longitude;
                if (dx == 0) { if (dy >= 0) { az = 0; } else { az = 180; } }
                else { az = 90 -  180 * Math.Atan(dy / dx) / Math.PI; }
                if (dx < 0) { az = az + 180;}
            }
            else
            {
                double dlon = targetPointArgs.Longitude - sourcePointAgs.Longitude;
                double r = Math.Sin(sourcePointAgs.Latitude * Math.PI / 180) * Math.Sin(targetPointArgs.Latitude * Math.PI / 180) + Math.Cos(sourcePointAgs.Latitude * Math.PI / 180) * Math.Cos(targetPointArgs.Latitude * Math.PI / 180) * Math.Cos(dlon * Math.PI / 180);
                double x1 = Math.Sin(targetPointArgs.Latitude * Math.PI / 180) - Math.Sin(sourcePointAgs.Latitude * Math.PI / 180)*r;
                double y1 = Math.Cos(sourcePointAgs.Latitude * Math.PI / 180) * Math.Cos(targetPointArgs.Latitude * Math.PI / 180) * Math.Cos(dlon * Math.PI / 180);
                if ((Math.Abs(x1)<0.000000001)&&(Math.Abs(y1) < 0.000000001)){ az = targetPointArgs.Longitude; }
                else { az = Math.Atan2(y1, x1)*180/Math.PI;}
                if (az < 0) { az = az + 360;}
            }
            return az;
        }
    }
}
