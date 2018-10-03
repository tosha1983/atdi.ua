using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class FrequencyForSectorFormICSM // перечень частот
    {
        [DataMember]
        public int? Id; // Идентификатор частоты
        [DataMember]
        public int? IdPlan; // Идетификатор частотного плана
        [DataMember]
        public int? ChannalNumber;
        [DataMember]
        public decimal? Frequency; //МГц;
    }
}
