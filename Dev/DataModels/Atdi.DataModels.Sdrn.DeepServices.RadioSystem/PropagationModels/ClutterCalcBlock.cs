using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels
{
    [Serializable]
    public struct ClutterCalcBlock
	{
		public ClutterCalcBlockModelType ModelType;

		public bool Available;
	}
    [Serializable]
    public enum ClutterCalcBlockModelType
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// ITU 2109 Model
		/// </summary>
		ITU2109 = 1,

        /// <summary>
        /// Flat Model
        /// </summary>
        Flat = 2
    }
}
