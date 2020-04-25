using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis
{
	public class CluttersDescFreq
	{
		public long Id;

		public double Freq_MHz;

		public CluttersDescFreqClutter[] Clutters;
	}
}
