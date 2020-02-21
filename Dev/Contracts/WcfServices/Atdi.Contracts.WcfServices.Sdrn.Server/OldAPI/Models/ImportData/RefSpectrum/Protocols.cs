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
        /// Идентификатор записи в таблице StationExtended
        /// </summary>
        [DataMember]
        public long? StationExtendedId { get; set; }

        /// <summary>
        /// Идентификатор записи в таблице SynchroProcess
        /// </summary>
        [DataMember]
        public long? SynchroProcessId { get; set; }

        /// <summary>
        /// сведения о сенсоре
        /// </summary>
        [DataMember]
        public Sensor Sensor { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DataRefSpectrum  DataRefSpectrum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public StationExtended StationExtended  { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public ProtocolsWithEmittings ProtocolsLinkedWithEmittings { get; set; }
    }
}
