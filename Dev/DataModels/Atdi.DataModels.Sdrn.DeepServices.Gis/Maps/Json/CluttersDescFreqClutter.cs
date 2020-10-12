using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.Gis.Maps.Json
{
    [Serializable]
    public class CluttersDescFreqClutter
	{
		public byte Code;

		public float LinearLoss_dBkm;

		public float FlatLoss_dB;

		public float Reflection;

		public string Note;
	}
}
