using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Type of result of measurement task 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public enum MeasTaskResultType
    {
        /// <summary>
        /// Measurement result
        /// </summary>
        [EnumMember]
        MeasurementResult,
        /// <summary>
        /// Compressed measurement result
        /// </summary>
        [EnumMember]
        CompressedMeasurementResult,
        /// <summary>
        /// Measurement result during an alarm
        /// </summary>
        [EnumMember]
        MeasurementResultDuringAlarm,
        /// <summary>
        /// Start and end of an alarm
        /// </summary>
        [EnumMember]
        StartAndEndOfAlarm,
        /// <summary>
        /// Measurement result during an alarm and compressed measurement result outside an alarm
        /// </summary>
        [EnumMember]
        MeasurementResultDuringAlarmAndCompressedOutsideAlarm,
        /// <summary>
        /// MaxHold
        /// </summary>
        [EnumMember]
        MaxHold
    }
}
