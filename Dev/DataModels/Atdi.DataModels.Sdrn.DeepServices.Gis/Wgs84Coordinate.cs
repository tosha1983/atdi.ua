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
	public struct Wgs84Coordinate
	{
		public double Longitude;

		public double Latitude;

	}
}
