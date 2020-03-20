using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config
{
    [Serializable]
    public class AvtoRefLevel
    {
        [XmlElement]
        public int Start_dBm = 0;

        [XmlElement]
        public int Stop_dBm = 0;

        [XmlElement]
        public int Step_dB = 0;

        [XmlElement]
        public int NumberScan = 0;

        [XmlElement]
        public int PersentOverload = 0;
    }
}
