using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[Entity]
	public interface IContextStationReceiver : IContextStation_PK
	{

		double Freq_MHz { get; set; }

		double BW_kHz { get; set; }

		float Loss_dB { get; set; }

		float KTBF_dBm { get; set; }

		float Threshold_dBm { get; set; }

		byte PolarizingCode { get; set; }

		string PolarizingName { get; set; }

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
