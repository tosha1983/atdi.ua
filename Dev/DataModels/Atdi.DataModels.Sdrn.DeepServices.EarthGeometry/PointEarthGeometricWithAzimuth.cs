using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.EarthGeometry
{
    public struct PointEarthGeometricWithAzimuth
    {
        public PointEarthGeometric PointEarthGeometric;
        public PointEarthGeometric PointStart;
        public PointEarthGeometric PointStop;
        public double Azimuth_deg;
    }
}

