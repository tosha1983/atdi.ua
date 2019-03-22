using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config
{
    [Serializable]
    public class AdapterEquipmentInfo
    {
        /// <summary>
        /// EquipmentInfo.AntennaCode
        /// </summary>
        [XmlElement]
        public string AntennaSN = "";

        /// <summary>
        /// EquipmentInfo.AntennaManufacturer
        /// </summary>
        [XmlElement]
        public string AntennaManufacturer = "";

        /// <summary>
        /// EquipmentInfo.AntennaName
        /// </summary>
        [XmlElement]
        public string AntennaName = "";
    }
}
