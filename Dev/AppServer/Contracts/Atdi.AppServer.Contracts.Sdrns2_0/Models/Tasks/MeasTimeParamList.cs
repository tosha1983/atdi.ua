using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Represents parameters of time measurements
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class MeasTimeParamList
    {
        /// <summary>
        /// PerStart
        /// </summary>
        [DataMember]
        public DateTime PerStart;
        /// <summary>
        /// PerStop
        /// </summary>
        [DataMember]
        public DateTime PerStop;
        /// <summary>
        /// TimeStart
        /// </summary>
        [DataMember]
        public DateTime? TimeStart;
        /// <summary>
        /// TimeStop
        /// </summary>
        [DataMember]
        public DateTime? TimeStop;
        /// <summary>
        /// Days
        /// </summary>
        [DataMember]
        public string Days;
        /// <summary>
        /// PerInterval
        /// </summary>
        [DataMember]
        public double? PerInterval;//sec
    }
}
