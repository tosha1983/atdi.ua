using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class OwnerData 
    {
        [DataMember]
        public int Id;
        [DataMember]
        public string OwnerName;
        [DataMember]
        public string OKPO;
        [DataMember]
        public string Zip;
        [DataMember]
        public string Code;
        [DataMember]
        public string Addres;
    }
}
