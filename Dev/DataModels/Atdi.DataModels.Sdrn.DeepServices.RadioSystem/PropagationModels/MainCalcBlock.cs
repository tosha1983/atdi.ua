using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels
{
	public struct MainCalcBlock
	{
		public MainCalcBlockModelType ModelType;

	}

	public enum MainCalcBlockModelType
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// ITU 525 Model
		/// </summary>
		ITU525 = 1,

		/// <summary>
		/// ITU 1546 Model
		/// </summary>
		ITU1546 = 2,
        

        /// <summary>
        /// ITU 2001 Model
        /// </summary>
        ITU2001 = 3,

            /// <summary>
            /// ITU 1546 Model
            /// </summary>
        ITU1546_4 = 4,
    }
}
