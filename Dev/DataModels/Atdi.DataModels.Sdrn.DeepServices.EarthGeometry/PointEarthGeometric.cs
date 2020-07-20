using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.EarthGeometry
{
    public struct PointEarthGeometric
    {
        public double Longitude;
        public double Latitude;
        public CoordinateUnits CoordinateUnits;

        public PointEarthGeometric(double Longitude, double Latitude, CoordinateUnits CoordinateUnits) : this()
        {
            this.Longitude = Longitude;
            this.Latitude = Latitude;
            this.CoordinateUnits = CoordinateUnits;

        }
    }
}

