using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using System.Windows;


namespace Atdi.AppUnits.Sdrn.DeepServices.EarthGeometry
{
    public static  class CreateContourFromContureByDistanceСalculations
    {
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
    }
}
