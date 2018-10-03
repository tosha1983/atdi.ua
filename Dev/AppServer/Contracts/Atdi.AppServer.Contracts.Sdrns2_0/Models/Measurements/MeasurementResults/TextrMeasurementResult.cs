using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Result of measurement the information about signal 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class TextrMeasurementResult : MeasurementResult
    {
        /// <summary>
        /// PiCode
        /// </summary>
        [DataMember]
        public string PiCode;
        /// <summary>
        /// Program
        /// </summary>
        [DataMember]
        public string Program;
        /// <summary>
        /// SoundId
        /// </summary>
        [DataMember]
        public string SoundId;
        /// <summary>
        /// TxtRes
        /// </summary>
        [DataMember]
        public string TxtRes;
        /// <summary>
        /// PPiCode
        /// </summary>
        [DataMember]
        public string PPiCode;
        /// <summary>
        /// PProgram
        /// </summary>
        [DataMember]
        public string PProgram;
    }
}
