using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels
{
    [Serializable]
    public struct SubPathDiffractionCalcBlock
	{
		public SubPathDiffractionCalcBlockModelType ModelType;

		public bool Available;
	}
    [Serializable]
    public enum SubPathDiffractionCalcBlockModelType
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Sub Deygout 91 Model
		/// </summary>
		SubDeygout91 = 1
	}
}
