using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    [DataContract(Namespace = Specification.Namespace)]
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
