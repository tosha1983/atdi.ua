using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{

    /// <summary>
    /// Signaling
    /// </summary>
    [Serializable]
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasTaskSignaling : MeasTask
    {
        ///// <summary>
        ///// Reference situation
        ///// </summary>
        [DataMember]
        public ReferenceSituation[] RefSituation { get; set; }

        /// <summary>
        /// Set Signaling MeasTask Parameters
        /// </summary>
        [DataMember]
        public SignalingMeasTaskParameters SignalingMeasTaskParameters { get; set; }


        /// <summary>
        /// receiver (detector) setting (parameter) for measurements
        /// </summary>
        [DataMember]
        public MeasDtParam MeasDtParam;

        /// <summary>
        /// Frequencies for measurements
        /// </summary>
        [DataMember]
        public MeasFreqParam MeasFreqParam;
    }
}
