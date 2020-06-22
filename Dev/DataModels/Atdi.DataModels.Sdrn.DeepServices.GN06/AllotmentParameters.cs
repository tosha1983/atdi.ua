using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.GN06
{
	public class AllotmentParameters
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name; //M

		/// <summary>
		/// 
		/// </summary>
		public int ContourId;//M
        /// <summary>
        /// Контур с точками
        /// </summary>
        public AreaPoint[] Сontur;//M
	}
}
