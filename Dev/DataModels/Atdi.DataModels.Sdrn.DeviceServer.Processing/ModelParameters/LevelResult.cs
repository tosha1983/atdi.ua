using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    [Serializable]
    public class LevelResult
    {
        public float[] Level;
        public double[] Freq_Hz;
        public DeviceParameterState DeviceParameterState;
    }
}
