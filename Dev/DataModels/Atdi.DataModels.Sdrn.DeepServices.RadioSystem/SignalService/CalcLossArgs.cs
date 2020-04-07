using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService
{
	public struct CalcLossArgs
	{
		public PropagationModel Model;

		public short[] ReliefProfile;

		public byte[] ClutterProfile;

		public byte[] BuildingProfile;

		public short[] HeightProfile;

		public int ProfileLength;

		public int ReliefStartIndex;
		public int ClutterStartIndex;
		public int BuildingStartIndex;
		public int HeightStartIndex;
	}
}
