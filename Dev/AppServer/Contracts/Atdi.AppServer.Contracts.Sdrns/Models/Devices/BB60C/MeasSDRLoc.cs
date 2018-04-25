using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    ///
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class MeasSdrLoc
    {
        [DataMember]
        public double Lon; // Longitude, DEC
        [DataMember]
        public double Lat; // Latitude,  DEC
        [DataMember]
        public double ASL; // Altitude above sea level, m
    }

}
