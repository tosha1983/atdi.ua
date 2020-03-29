using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config
{
    [Serializable]
    public class AdapterAutoRefLevel
    {
        [XmlElement]
        public int Start_dBm = 0;

        [XmlElement]
        public int Stop_dBm = 0;

        [XmlElement]
        public uint Step_dB = 0;

        [XmlElement]
        public uint NumberScan = 0;

        [XmlElement]
        public uint PersentOverload = 0;
    }
}
