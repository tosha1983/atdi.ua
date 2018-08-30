//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;

//namespace Atdi.DataModels.Sdrns.Device
//{
//    /// <summary>
//    /// Class presents parameters of emission above predefined level. Illegal emission.
//    /// </summary>
//    [DataContract(Namespace = Specification.Namespace)]
//    public class SignalingSample
//    {
//        /// <summary>
//        /// Received level, dBm
//        /// </summary>
//        [DataMember]
//        public float Receivedevel_dBm { get; set; }
//        /// <summary>
//        /// Predefined level, dBm
//        /// </summary>
//        [DataMember]
//        public float PredefinedLevel_dBm { get; set; }
//        /// <summary>
//        /// Frequency, MHz
//        /// </summary>
//        [DataMember]
//        public double Frequency_MHz { get; set; }
//        /// <summary>
//        /// Emission Bandwidth, kHz
//        /// </summary>
//        [DataMember]
//        public double Bandwidth_kHz { get; set; }
//        /// <summary>
//        /// Times of events of emitting
//        /// </summary>
//        [DataMember]
//        public DateTime[] EventsTimetimes { get; set; }
//        /// <summary>
//        /// Durations of events of emitting
//        /// </summary>
//        [DataMember]
//        public TimeSpan[] EventsDurations { get; set; }
//    }
//}
