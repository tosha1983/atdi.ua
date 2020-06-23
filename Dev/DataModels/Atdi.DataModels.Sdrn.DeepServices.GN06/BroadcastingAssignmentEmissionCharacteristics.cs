using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.GN06
{
	public class BroadcastingAssignmentEmissionCharacteristics
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
		/// ITU Name: "Erp_h_dBW".
		/// </summary>
		public float ErpH_dBW; // max 53

		/// <summary>
		/// ITU Name: "Erp_v_dBW".
		/// </summary>
		public float ErpV_dBW; // max 53

		/// <summary>
		/// ITU Name: "Ref_plan_cfg" of (Sys_var and Rx_mode) is M
		/// </summary>
		public RefNetworkConfigType RefNetworkConfig;

		/// <summary>
		/// ITU Name: "Sys_var" as Sys_varType.
		/// </summary>
		public SystemVariationType SystemVariation; // 

		/// <summary>
		/// ITU Name: "Rx_mode".
		/// </summary>
		public RxModeType RxMode; 

		/// <summary>
		/// ITU Name: "Spect_mask".
		/// </summary>
		public SpectrumMaskType SpectrumMask; //M
	}
}
