using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Represents frequency for measurement 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class GetSOformMeasResultStationValue
    {
        [DataMember]
        public List<double> Frequencies_MHz;
        [DataMember]
        public double BW_kHz;
        [DataMember]
        public List<int> MeasResultID;
        [DataMember]
        public double LonMax;
        [DataMember]
        public double LonMin;
        [DataMember]
        public double LatMax;
        [DataMember]
        public double LatMin;
        [DataMember]
        public double TrLevel_dBm;
    }
}

