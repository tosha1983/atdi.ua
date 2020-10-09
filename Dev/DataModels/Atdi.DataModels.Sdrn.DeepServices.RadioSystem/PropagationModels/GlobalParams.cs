using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels
{
    [Serializable]
    public struct GlobalParams
	{
		public const float TimeMin = 00.0001F;
		public const float TimeMax = 99.9999F;
		public const float TimeDefault = 50F;

		public const float LocationMin = 00.0001F;
		public const float LocationMax = 99.9999F;
		public const float LocationDefault = 50F;
		public const float EarthRadiusDefault = 8500F;

		public float Time_pc;

		public float Location_pc;

		public float EarthRadius_km;
	}
}
