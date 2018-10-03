using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{

    /// <summary>
    /// Represents task for measurements. 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
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
        /// OrderId
        /// </summary>
        [DataMember]
        public int? OrderId;
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
        /// Execution mode - A - automatic, M - manual
        /// </summary>
        [DataMember]
        public MeasTaskExecutionMode ExecutionMode;
        /// <summary>
        /// Task - FFM	 - Fixed Frequency Mode; SCAN	- Scan; DSCAN	-  Digiscan; FLSCAN	- Frequency List Scan; TLSCAN -	Transmitter List Scan; IMA	- Intermodulation Analysis; SWEEP -	Sweep
        /// </summary>
        [DataMember]
        public MeasTaskType Task;
        /// <summary>
        /// Prio 0 - maximum prioritet
        /// </summary>
        [DataMember]
        public int? Prio; // 0 - maximum prioritet
        /// <summary>
        /// ResultType - MR	- Measurement result; CMR	- Compressed measurement result; AMR	- Measurement result during an alarm; LOG	- Start and end of an alarm; AMR_CMR	- Measurement result during an alarm and compressed measurement result outside an alarm; 	MH	- MaxHold 
        /// </summary>
        [DataMember]
        public MeasTaskResultType ResultType;
        /// <summary>
        /// MaxTimeBs
        /// </summary>
        [DataMember]
        public int? MaxTimeBs;
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
        /// Parameters of locations for measurements.
        /// </summary>
        [DataMember]
        public MeasLocParam[] MeasLocParams;
        /// <summary>
        /// Stations for measurements.
        /// </summary>
        [DataMember]
        public MeasStation[] Stations;
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
        /// <summary>
        /// Parameters for measurements useful for SDR and Spectrum Occupation
        /// </summary>
        [DataMember]
        public MeasOther MeasOther;
        /// <summary>
        /// Parameters of time measurements
        /// </summary>
        [DataMember]
        public MeasTimeParamList MeasTimeParamList;

        [DataMember]
        public StationDataForMeasurements[] StationsForMeasurements; 
    }
}
