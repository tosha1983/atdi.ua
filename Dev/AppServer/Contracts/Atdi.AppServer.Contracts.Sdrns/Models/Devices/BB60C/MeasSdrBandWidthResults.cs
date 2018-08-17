using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    [DataContract(Namespace = ServicesSpecification.Namespace)]

    public class MeasSdrBandwidthResults
    {
        [DataMember]
        public int? T1; // индекс Т1 надо отображать на спектрограмме
        [DataMember]
        public int? T2; // индекс Т2 надо отображать на спектрограмме 
        [DataMember]
        public int? MarkerIndex; // индекс для M1 надо отображать
        [DataMember]
        public double? BandwidthkHz; //ширина спектра в килогерцах
        [DataMember]
        public bool? СorrectnessEstimations; //коректность проведеннного измерения
    }
}
