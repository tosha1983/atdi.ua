using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[Entity]
	public interface IContextStationTransmitter : IContextStation_PK
	{

		double Freq_MHz { get; set; }

		double BW_kHz { get; set; }

		double Loss_dB { get; set; }

		double MaxPower_dBm { get; set; }

		byte PolarizingCode { get; set; }

		string PolarizingName { get; set; }

	}

}
