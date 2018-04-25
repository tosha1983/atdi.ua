using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    ///
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class FSemples
    {
        [DataMember]
        public int Id;
        [DataMember]
        public float Freq; //MHz
        [DataMember]
        public float LeveldBm;
        [DataMember]
        public float LeveldBmkVm;
        [DataMember]
        public float LevelMindBm;
        [DataMember]
        public float LevelMaxdBm;
        [DataMember]
        public float OcupationPt; // в процентах степень занятости канала
    }
}
