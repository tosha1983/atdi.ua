using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.IDWM
{
	/// <summary>
	/// 
	/// </summary>
	public struct AdministrationsResult
    {
        /// <summary>
        /// Массив администраций
        /// </summary>
		public string Administration;
        /// <summary>
        /// Азимут
        /// </summary>
        public float? Azimuth;
        /// <summary>
        /// 
        /// </summary>
        public Point Point;
        /// <summary>
        /// 
        /// </summary>
        public double? Distance;

    }
}
