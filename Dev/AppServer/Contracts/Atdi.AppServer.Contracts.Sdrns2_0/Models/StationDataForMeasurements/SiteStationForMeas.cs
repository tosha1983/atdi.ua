using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class SiteStationForMeas // параметры сайта
    {
        [DataMember]
        public double? Lon; //DEC
        [DataMember]
        public double? Lat; //DEC
        [DataMember]
        public string Adress;
        [DataMember]
        public string Region; // Район (часть области)
    }
}
