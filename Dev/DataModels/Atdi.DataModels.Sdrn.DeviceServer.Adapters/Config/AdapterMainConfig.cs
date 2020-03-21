using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config
{
    [Serializable]
    public class AdapterMainConfig
    {
        /// <summary>
        /// MesureIQStreamDeviceProperties.AvailabilityPPS
        /// </summary>
        [XmlElement]
        public bool AvailabilityPPS = false;

        /// <summary>
        /// MesureIQStreamDeviceProperties.BitRateMax_MBs
        /// </summary>
        [XmlElement]
        public double IQBitRateMax = 0;

        [XmlElement]
        public AdapterEquipmentInfo AdapterEquipmentInfo = new AdapterEquipmentInfo();

        [XmlElement]
        public AdapterAutoRefLevel AutoRefLevel = new AdapterAutoRefLevel();

        [XmlArray]
        public AdapterRadioPathParameter[] AdapterRadioPathParameters = new AdapterRadioPathParameter[] { };

        [XmlArray]
        public AdapterResultPool[] AdapterTraceResultPools = new AdapterResultPool[] { };

        

        public void Serialize(string FilePath)
        {
            XmlSerializer ser = new XmlSerializer(this.GetType());
            TextWriter writer = new StreamWriter(FilePath, false);
            ser.Serialize(writer, this);
            writer.Close();
        }

    }
}
