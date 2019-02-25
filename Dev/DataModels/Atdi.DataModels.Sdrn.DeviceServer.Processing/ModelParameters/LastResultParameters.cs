using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class LastResultParameters
    {
        public int NN { get; set; }
        public SemplFreq[] FSemples { get; set; }
        public int APIversion { get; set; }
    }
}
