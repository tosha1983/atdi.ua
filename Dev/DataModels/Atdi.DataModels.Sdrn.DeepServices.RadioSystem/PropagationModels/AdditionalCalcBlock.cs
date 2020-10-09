using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels
{
    [Serializable]
    public struct AdditionalCalcBlock
	{
		public AdditionalCalcBlockModelType ModelType;

		public bool Available;
	}
    [Serializable]
    public enum AdditionalCalcBlockModelType
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,


		/// <summary>
		/// ITU 1820 Model
		/// </summary>
		ITU1820 = 1,
	}
}
