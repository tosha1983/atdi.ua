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
    /// Represents task for measurements. 
    /// </summary>
    [Serializable]
    [DataContract(Namespace = Specification.Namespace)]
    [KnownType(typeof(StationDataForMeasurements))]
    [KnownType(typeof(SiteStationForMeas))]
    [KnownType(typeof(SectorStationForMeas))]
    [KnownType(typeof(PermissionForAssignment))]
    [KnownType(typeof(FrequencyForSectorFormICSM))]
    [KnownType(typeof(MaskElements))]
    public class MeasTask
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public MeasTaskIdentifier Id;
        
        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status;
        
        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name;
        /// <summary>
        /// Execution mode - A - automatic, M - manual
        /// </summary>
        [DataMember]
        public MeasTaskExecutionMode ExecutionMode;
      
        /// <summary>
        /// Prio 0 - maximum prioritet
        /// </summary>
        [DataMember]
        public int? Prio; // 0 - maximum prioritet
        /// <summary>
        /// ResultType - MR	- Measurement result; CMR	- Compressed measurement result; AMR	- Measurement result during an alarm; LOG	- Start and end of an alarm; AMR_CMR	- Measurement result during an alarm and compressed measurement result outside an alarm; 	MH	- MaxHold 
        /// </summary>
        //[DataMember]
        //public MeasTaskResultType ResultType;
        /// <summary>
        /// Type of measurements, SO - spectrum occupation; LV - Level; FO - Offset; FR - Frequency; FM - Freq. Modulation; AM - Ampl. Modulation; BW	- Bandwidth Meas; BE - Bearing; SA - Sub Audio Tone; PR	- Program; PI - PI Code  (Hex Code identifying radio program); SI - Sound ID; LO	- Location;
        /// </summary>
        [DataMember]
        public MeasurementType TypeMeasurements;
        /// <summary>
        /// DateCreated
        /// </summary>
        [DataMember]
        public DateTime? DateCreated;
        /// <summary>
        /// CreatedBy
        /// </summary>
        [DataMember]
        public string CreatedBy;
        /// <summary>
        /// MeasSubTasks
        /// </summary>
        [DataMember]
        public MeasSubTask[] MeasSubTasks;
        /// <summary>
        /// Sensors for measurements.
        /// </summary>
        [DataMember]
        public MeasSensor[] Sensors;
        /// <summary>
        /// Parameters of time measurements
        /// </summary>
        [DataMember]
        public MeasTimeParamList MeasTimeParamList;
    }
}
