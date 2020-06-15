using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IContextStationTransmitter_PK
	{
		long StationId { get; set; }
	}

	[Entity]
	public interface IContextStationTransmitter : IContextStationTransmitter_PK
	{
        double[] Freqs_MHz { get; set; }

        float Freq_MHz { get; set; }

		double BW_kHz { get; set; }

		float Loss_dB { get; set; }

		float MaxPower_dBm { get; set; }

		byte PolarizationCode { get; set; }

		string PolarizationName { get; set; }

	}

}
