using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class SemplFreq
    {
        public float Freq { get; set; }
        public float LeveldBm { get; set; }
        public float LeveldBmkVm { get; set; }
        public float LevelMindBm { get; set; }
        public float LevelMaxdBm { get; set; }
        public float OcupationPt { get; set; }
    }
}
