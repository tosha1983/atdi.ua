using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.Gis
{
    /// <summary>
    /// Координаты WGS84 в DEC
    /// </summary>
    [Serializable]
    public struct Wgs84Site
	{
		public double Longitude;

		public double Latitude;

		public double Altitude;

		public static implicit operator Wgs84Coordinate(Wgs84Site source) => new Wgs84Coordinate
		{
			Longitude = source.Longitude,
			Latitude = source.Latitude
		};
	}
}
