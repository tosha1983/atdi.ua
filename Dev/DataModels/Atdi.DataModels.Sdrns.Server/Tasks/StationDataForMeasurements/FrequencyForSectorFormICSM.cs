using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{
    [Serializable]
    [DataContract(Namespace = Specification.Namespace)]
    public class FrequencyForSectorFormICSM // перечень частот
    {
        [DataMember]
        public long? Id; // Идентификатор частоты
        [DataMember]
        public long? IdPlan; // Идетификатор частотного плана
        [DataMember]
        public long? ChannalNumber;
        [DataMember]
        public decimal? Frequency; //МГц;
    }
}
