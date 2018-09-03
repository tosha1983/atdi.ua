//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;

//namespace Atdi.DataModels.Sdrns.Device
//{
//    /// <summary>
//    /// Represents frequencies for  measurements
//    /// </summary>
//    [DataContract(Namespace = Specification.Namespace)]
//    public class MeasuredFrequencies
//    {
//        /// <summary>
//        /// Frequency mode
//        /// </summary>
//        [DataMember]
//        public FrequencyMode Mode { get; set; }

//        /// <summary>
//        /// Lower frequency of the range, MHz
//        /// </summary>
//        [DataMember]
//        public double? RgL_MHz { get; set; }

//        /// <summary>
//        /// Upper frequency of the range, MHz
//        /// </summary>
//        [DataMember]
//        public double? RgU_MHz { get; set; }

//        /// <summary>
//        /// Step, kHz
//        /// </summary>
//        [DataMember]
//        public double? Step_kHz { get; set; }

//        /// <summary>
//        /// Array of the frequencies, MHz
//        /// </summary>
//        [DataMember]
//        public double[] Values_MHz { get; set; }
//    }
//}
