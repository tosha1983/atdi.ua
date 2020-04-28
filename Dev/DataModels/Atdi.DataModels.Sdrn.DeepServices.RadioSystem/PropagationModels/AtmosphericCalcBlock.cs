using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels
{
	public struct AtmosphericCalcBlock
	{
		public AtmosphericCalcBlockModelType ModelType;

		public bool Available;
	}

	public enum AtmosphericCalcBlockModelType
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Deygout 66 Model
		/// </summary>
		ITU838_530 = 1,

		/// <summary>
		/// Deygout 91 Model
		/// </summary>
		ITU678 = 2,

		/// <summary>
		/// ITU 526(15) Model
		/// </summary>
		ITU1820 = 3
	}
}
