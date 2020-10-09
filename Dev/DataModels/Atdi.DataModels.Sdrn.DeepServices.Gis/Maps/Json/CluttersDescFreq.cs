using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.Gis.Maps.Json
{
    [Serializable]
    public class CluttersDescFreq
	{
		public double Freq_MHz;

		public string Note;

		public CluttersDescFreqClutter[] Clutters;
	}
}
