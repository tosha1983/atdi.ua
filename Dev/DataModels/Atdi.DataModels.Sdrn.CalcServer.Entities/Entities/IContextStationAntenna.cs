using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[Entity]
	public interface IContextStationAntenna : IContextStation_PK
	{

		double Gain_dB { get; set; }

		double Tilt_deg { get; set; }

		double Azimuth_deg { get; set; }

		double XPD_dB { get; set; }

		byte ItuPatternCode { get; set; }

		string ItuPatternName { get; set; }

		IContextStationPattern HH_PATTERN { get; set; }

		IContextStationPattern HV_PATTERN { get; set; }

		IContextStationPattern VH_PATTERN { get; set; }

		IContextStationPattern VV_PATTERN { get; set; }

	}

	public enum ItuPattern
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
