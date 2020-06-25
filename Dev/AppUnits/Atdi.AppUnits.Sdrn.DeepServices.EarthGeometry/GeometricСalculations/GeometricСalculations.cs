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
                d = angle * re;
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
                double dlon = xtarget - xpoint;
                double r = Math.Sin(ypoint * Math.PI / 180) * Math.Sin(ytarget * Math.PI / 180) + Math.Cos(ypoint * Math.PI / 180) * Math.Cos(ytarget * Math.PI / 180) * Math.Cos(dlon * Math.PI / 180);
                double x1 = Math.Sin(ytarget * Math.PI / 180) - Math.Sin(ypoint * Math.PI / 180) * r;
                double y1 = Math.Cos(ypoint * Math.PI / 180) * Math.Cos(ytarget * Math.PI / 180) * Math.Cos(dlon * Math.PI / 180);
                if ((Math.Abs(x1) < 0.000000001) && (Math.Abs(y1) < 0.000000001)) { az = xtarget; }
                else { az = Math.Atan2(y1, x1) * 180 / Math.PI; }
                if (az < 0) { az = az + 360; }
            }
            return az;
        }
    }
}
