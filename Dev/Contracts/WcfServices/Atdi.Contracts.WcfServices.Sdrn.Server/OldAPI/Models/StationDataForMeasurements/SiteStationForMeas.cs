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
    public class SiteStationForMeas // параметры сайта
    {
        [DataMember]
        public int Id;
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
