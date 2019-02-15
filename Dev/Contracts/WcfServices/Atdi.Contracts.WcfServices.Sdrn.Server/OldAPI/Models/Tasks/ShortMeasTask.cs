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
    /// Represents main parameters of task for measurement.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class ShortMeasTask
    {
        /// <summary>
        /// Id of Measurements Task 
        /// </summary>
        [DataMember]
        public MeasTaskIdentifier Id;
        /// <summary>
        /// OrderId
        /// </summary>
        [DataMember]
        public int OrderId;
        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status;
        /// <summary>
        /// Type
        /// </summary>
        [DataMember]
        public string Type;
        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name;
        /// <summary>
        /// Execution mode A - automatic, M - manual
        /// </summary>
        [DataMember]
        public MeasTaskExecutionMode ExecutionMode;
        /// <summary>
        /// Task; // FFM	 - Fixed Frequency Mode; SCAN	- Scan; DSCAN	-  Digiscan; FLSCAN	- Frequency List Scan; TLSCAN -	Transmitter List Scan; IMA	- Intermodulation Analysis; SWEEP -	Sweep
        /// </summary>
        [DataMember]
        public MeasTaskType Task;
        /// <summary>
        /// Prio 0 - maximum prioritet
        /// </summary>
        [DataMember]
        public  int? Prio; // 0 - maximum prioritet
        /// <summary>
        /// ResultType - MR	- Measurement result; CMR	- Compressed measurement result; AMR	- Measurement result during an alarm; LOG	- Start and end of an alarm; AMR_CMR	- Measurement result during an alarm and compressed measurement result outside an alarm; 	MH	- MaxHold 
        /// </summary>
        [DataMember]
        public MeasTaskResultType ResultType;
        /// <summary>
        /// TypeMeasurements  SO - spectrum occupation; LV - Level; FO - Offset; FR - Frequency; FM - Freq. Modulation; AM - Ampl. Modulation; BW	- Bandwidth Meas; BE - Bearing; SA - Sub Audio Tone; PR	- Program; PI - PI Code  (Hex Code identifying radio program); SI - Sound ID; LO	- Location;
        /// </summary>
        [DataMember]
        public MeasurementType TypeMeasurements;
        /// <summary>
        /// MaxTimeBs
        /// </summary>
        [DataMember]
        public int? MaxTimeBs;
        /// <summary>
        /// DateCreated;
        /// </summary>
        [DataMember]
        public DateTime? DateCreated;
        /// <summary>
        /// CreatedBy
        /// </summary>
        [DataMember]
        public string CreatedBy;
    }
}
