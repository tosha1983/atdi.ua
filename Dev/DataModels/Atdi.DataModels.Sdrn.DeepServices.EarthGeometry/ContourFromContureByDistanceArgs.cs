using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.EarthGeometry
{
    public struct ContourFromContureByDistanceArgs
    {
        /// <summary>
        /// координаты контура
        /// </summary>
        public PointEarthGeometric[]  ContourPoints;
        /// <summary>
        /// координаты барицентра
        /// </summary>
        public PointEarthGeometric PointBaryCenter;
        /// <summary>
        /// Шаг (градусы)
        /// </summary>
        public double Step_deg;
        /// <summary>
        /// Расстояние контура == s
        /// </summary>
        public double Distance_m;
    }
}

