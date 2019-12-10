using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config
{
    [Serializable]
    public class AdapterResultPool
    {
        [XmlElement]
        public string KeyName = "";

        [XmlElement]
        public int MinSize = 0;

        [XmlElement]
        public int MaxSize = 0;

        [XmlElement]
        public int Size = 0;
    }
}
