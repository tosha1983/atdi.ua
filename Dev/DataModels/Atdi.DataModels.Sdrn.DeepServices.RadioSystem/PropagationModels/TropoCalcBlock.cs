using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels
{
	public struct TropoCalcBlock
	{
		public TropoCalcBlockModelType ModelType;

		public bool Available;
	}

	public enum TropoCalcBlockModelType
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// ITU 617 Model
		/// </summary>
		ITU617 = 1,

	}
}
