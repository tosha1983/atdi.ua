using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class PermissionForAssignment// данные дозвола
    {
        [DataMember]
        public int? Id; // из ICSM
        [DataMember]
        public DateTime? StartDate;
        [DataMember]
        public DateTime? EndDate;
        [DataMember]
        public DateTime? CloseDate; // дата закртытия 
        [DataMember]
        public string DozvilName;
    }

}
