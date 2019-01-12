using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class SiteStationForMeasExtend: SiteStationForMeas
    {
        [DataMember]
        public int Id;
        [DataMember]
        public double? Lon; 
        [DataMember]
        public double? Lat; 
        [DataMember]
        public string Adress;
        [DataMember]
        public string Region; 
    }
}
