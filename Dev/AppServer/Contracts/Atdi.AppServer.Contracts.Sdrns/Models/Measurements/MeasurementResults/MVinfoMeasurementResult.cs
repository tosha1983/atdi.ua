using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Result of measurement the information about parameter of signal 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class MVinfoMeasurementResult : MeasurementResult
    {
        /// <summary>
        /// MdTxName
        /// </summary>
        [DataMember]
        public string MdTxName;
        /// <summary>
        /// MdTxService
        /// </summary>
        [DataMember]
        public string MdTxService;
        /// <summary>
        /// MdTxSignature
        /// </summary>
        [DataMember]
        public string MdTxSignature;
        /// <summary>
        /// MdTxCallSign
        /// </summary>
        [DataMember]
        public string MdTxCallSign;
        /// <summary>
        /// MdTxLicensee
        /// </summary>
        [DataMember]
        public string MdTxLicensee;
        /// <summary>
        /// BER
        /// </summary>
        [DataMember]
        public double BER;
        /// <summary>
        /// BERLim
        /// </summary>
        [DataMember]
        public double BERLim;
    }
}
