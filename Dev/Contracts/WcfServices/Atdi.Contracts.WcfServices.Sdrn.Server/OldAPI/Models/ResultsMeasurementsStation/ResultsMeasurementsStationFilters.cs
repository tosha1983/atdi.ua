using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    [Serializable]
    [DataContract(Namespace = Specification.Namespace)]
    public class ResultsMeasurementsStationFilters 
    {
        [DataMember]
        public string MeasGlobalSid { get; set; }

        [DataMember]
        public string Standard { get; set; }

        [DataMember]
        public double? FreqBg { get; set; }

        [DataMember]
        public double? FreqEd { get; set; }
    }
}

