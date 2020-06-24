using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.EarthGeometry
{
    public struct ContourFromPointByDistanceArgs
    {
        /// <summary>
        /// точка (координата в DEC)
        /// </summary>
        public PointEarthGeometric  PointEarthGeometricCalc;
        /// <summary>
        /// шаг (градусы)
        /// </summary>
        public double Step_deg;
        /// <summary>
        /// Расстояние от точки
        /// </summary>
        public double Distance_m;
    }
}

