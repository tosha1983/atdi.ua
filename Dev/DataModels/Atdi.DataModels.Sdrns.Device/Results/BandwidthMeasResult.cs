using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    public class BandwidthMeasResult
    {
        /// <summary>
        /// индекс Т1 надо отображать на спектрограмме
        /// </summary>
        [DataMember]
        public int? T1 { get; set; }

        /// <summary>
        /// индекс Т2 надо отображать на спектрограмме 
        /// </summary>
        [DataMember]
        public int? T2 { get; set; }

        /// <summary>
        /// индекс для M1 надо отображать
        /// </summary>
        [DataMember]
        public int? MarkerIndex { get; set; }

        /// <summary>
        /// ширина спектра в килогерцах
        /// </summary>
        [DataMember]
        public double? BandwidthkHz { get; set; }

        /// <summary>
        /// коректность проведеннного измерения
        /// </summary>
        [DataMember]
        public bool? СorrectnessEstimations { get; set; }
    }
}
