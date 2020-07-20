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
	public class SiteParameters
	{
		/// <summary>
		/// 
		/// </summary>
		public double Lon_Dec; //M

		/// <summary>
		/// 
		/// </summary>
		public double Lat_Dec; //M

		/// <summary>
		/// 
		/// </summary>
		public short Alt_m; // M min = -1000 max = 8850 

		/// <summary>
		/// 
		/// </summary>
		public string Name;//M
	}
}
