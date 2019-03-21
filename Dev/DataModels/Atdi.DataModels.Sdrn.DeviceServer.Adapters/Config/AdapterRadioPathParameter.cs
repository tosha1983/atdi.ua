using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config
{
    [Serializable]
    public class AdapterRadioPathParameter
    {
        /// <summary>
        /// In Hz
        /// </summary>
        [XmlElement]
        public decimal Freq = 0;

        /// <summary>
        /// In dBm
        /// </summary>
        [XmlElement]
        public double KTBF = 0;

        /// <summary>
        /// In dB
        /// </summary>
        [XmlElement]
        public double FeederLoss = 0;

        /// <summary>
        /// In dB
        /// </summary>
        [XmlElement]
        public double Gain = 0;

        [XmlElement]
        public string DiagA = "";

        [XmlElement]
        public string DiagH = "";

        [XmlElement]
        public string DiagV = "";
    }
}
