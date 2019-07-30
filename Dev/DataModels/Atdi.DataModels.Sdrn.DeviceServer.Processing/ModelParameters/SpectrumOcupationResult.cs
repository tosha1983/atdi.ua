using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    [Serializable]
    public class SpectrumOcupationResult 
    {
        public SemplFreq[] fSemplesResult { get; set; } // результат со всеми отсчетами
        public int NN { get; set; } // Количество вычислений
    }
}
