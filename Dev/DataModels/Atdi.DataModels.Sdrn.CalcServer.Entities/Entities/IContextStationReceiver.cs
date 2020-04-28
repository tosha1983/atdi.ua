using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IContextStationReceiver_PK
	{
		long StationId { get; set; }
	}

	[Entity]
	public interface IContextStationReceiver : IContextStationReceiver_PK
	{

		double Freq_MHz { get; set; }

		double BW_kHz { get; set; }

		float Loss_dB { get; set; }

		float KTBF_dBm { get; set; }

		float Threshold_dBm { get; set; }

		byte PolarizationCode { get; set; }

		string PolarizationName { get; set; }

	}

	public enum PolarizationCode
	{
		/// <summary>
		/// Unknown
		/// </summary>
		U = 0,

		/// <summary>
		/// Vertical
		/// </summary>
		V = 1,

		/// <summary>
		/// Horizontal
		/// </summary>
		H = 2,

		/// <summary>
		/// CL
		/// </summary>
		CL = 3,

		/// <summary>
		/// RL
		/// </summary>
		RL = 4,

		/// <summary>
		/// M
		/// </summary>
		M = 5
	}

}
