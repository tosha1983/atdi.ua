using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis
{
    /// <summary>
    /// Параметры клатеров на некой частоте Freq_MHz
    /// </summary>
    [Serializable]
    public class CluttersDescFreq
	{
		public long Id;

		public double Freq_MHz;
        /// <summary>
        /// Массив с параметрами клетеров, индекс массива соответсвует коду клатера
        /// </summary>
		public CluttersDescFreqClutter[] Clutters; 
	}
}
