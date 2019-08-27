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
    /// SpectrumOccupation
    /// </summary>
    [Serializable]
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasTaskSpectrumOccupation : MeasTask
    {
        /// <summary>
        /// Parameters for measurements useful for SDR and Spectrum Occupation
        /// </summary>
        [DataMember]
        public SpectrumOccupationParameters SpectrumOccupationParameters;
       
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
