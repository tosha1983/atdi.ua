using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.GN06
{
	/// <summary>
	/// 
	/// </summary>
	public class BroadcastingAllotmentEmissionCharacteristics
	{
		/// <summary>
		/// ITU Name: "Freq_assgn_MHz".
		/// </summary>
		public double Freq_MHz; //M есть жесткое ограничение данного поля. смотри соглашение женева 06 Максим

		/// <summary>
		/// 
		/// </summary>
		public PolarType Polar; //M

		/// <summary>
		/// ITU Name: "Ref_plan_cfg" of (Sys_var and Rx_mode) is M
		/// </summary>
		public RefNetworkConfigType RefNetworkConfig;

		/// <summary>
		/// ITU Name: "Spect_mask".
		/// </summary>
		public SpectrumMaskType SpectrumMask; //M

		/// <summary>
		/// ITU Name: "Typ_Ref_Netwk".
		/// </summary>
		public RefNetworkType RefNetwork; //M

	}
}
