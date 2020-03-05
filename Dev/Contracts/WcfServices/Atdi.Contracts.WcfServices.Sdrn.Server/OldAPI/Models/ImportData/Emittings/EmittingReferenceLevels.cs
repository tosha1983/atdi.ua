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
    /// Emitting and ReferenceLevels
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class EmittingReferenceLevels
    {
        /// <summary>
        /// Emitting
        /// </summary>
        [DataMember]
        public Emitting Emitting;
        /// <summary>
        /// ReferenceLevels
        /// </summary>
        [DataMember]
        public ReferenceLevels  ReferenceLevels; 
       
    }
}
