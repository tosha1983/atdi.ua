using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.GN06
{
	public class AntennaCharacteristics
	{
		/// <summary>
		/// ITU Name: "ant_dir".
		/// </summary>
		public AntennaDirectionType Direction; //M

		/// <summary>
		/// ITU Name: "hgt_agl_m".
		/// </summary>
		public short AglHeight_m; // M min = 0 max = 800

		/// <summary>
		/// ITU Name: "eff_hgtmax_m".
		/// </summary>
		public int MaxEffHeight_m; // M max from ArrEff_hgtAz_m

		/// <summary>
		/// ITU Name: "ArrEff_hgtAz_m".
		/// </summary>
		public short[] EffHeight_m; // в масиве 36 первый соответсвует нулевому азимуту елементов min = -3000 max = 3000

		/// <summary>
		/// ITU Name: "ArrAttnAnt_Diagr_H".
		/// </summary>
		public float[] DiagrH;  //M for polar H, M В масиве 36 елементов первый соответсвует нулевому азимуту min = 0 max = 40 

		/// <summary>
		/// ITU Name: "ArrAttnAnt_Diagr_V".
		/// </summary>
		public float[] DiagrV;  //M for polar V, M В масиве 36 елементов первый соответсвует нулевому азимуту min = 0 max = 40
	}
}
