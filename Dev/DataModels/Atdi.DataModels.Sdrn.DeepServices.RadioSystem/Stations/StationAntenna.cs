using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations
{
	public class StationAntenna
	{
		public float Gain_dB;

		public float Tilt_deg;

		public float Azimuth_deg;

		public float XPD_dB;

		public AntennaItuPattern ItuPattern;

		public StationAntennaPattern HhPattern;

		public StationAntennaPattern HvPattern;

		public StationAntennaPattern VhPattern;

		public StationAntennaPattern VvPattern;
	}

	public enum AntennaItuPattern
	{
		None = 0,
		ITU465 = 1,
		ITU580 = 2,
		ITU699 = 3,
		ITU1213 = 4,
		ITU1245 = 5,
		ITU1428 = 6
	}
}
