using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class Protocols 
    {
        /// <summary>
        /// Info by Sensor
        /// </summary>
        [DataMember]
        public Sensor Sensor { get; set; }

        /// <summary>
        /// Data RefSpectrum
        /// </summary>
        [DataMember]
        public DataRefSpectrum  DataRefSpectrum { get; set; }

        /// <summary>
        /// StationExtended
        /// </summary>
        [DataMember]
        public StationExtended StationExtended  { get; set; }

        /// <summary>
        /// Protocols with emittings
        /// </summary>
        [DataMember]
        public ProtocolsWithEmittings ProtocolsLinkedWithEmittings { get; set; }

        /// <summary>
        /// Synchronization process
        /// </summary>
        [DataMember]
        public DataSynchronizationProcess DataSynchronizationProcess { get; set; }

        /// <summary>
        /// Radio Control Params
        /// </summary>
        [DataMember]
        public RadioControlParams RadioControlParams { get; set; }


        
    }
}
