using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels
{
	public struct DiffractionCalcBlock
	{
		public DiffractionCalcBlockModelType ModelType;

		public bool Available;
	}

	public enum DiffractionCalcBlockModelType
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Deygout 66 Model
		/// </summary>
		Deygout66 = 1,

		/// <summary>
		/// Deygout 91 Model
		/// </summary>
		Deygout91 = 2,

		/// <summary>
		/// ITU 526(15) Model
		/// </summary>
		ITU526_15 = 3,

		/// <summary>
		/// Bullington Model
		/// </summary>
		Bullington = 4
	}
}
