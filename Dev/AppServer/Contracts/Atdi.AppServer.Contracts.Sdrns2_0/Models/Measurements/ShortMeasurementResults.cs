using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Represents main measurements results 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class ShortMeasurementResults
    {
        /// <summary>
        /// Identifier of measurements results
        /// </summary>
        [DataMember]
        public MeasurementResultsIdentifier Id;

        /// <summary>
        /// TimeMeas
        /// </summary>
        [DataMember]
        public DateTime? TimeMeas;
        /// <summary>
        /// DataRank;
        /// </summary>
        [DataMember]
        public int? DataRank;
        /// <summary>
        /// Number of results
        /// </summary>
        [DataMember]
        public int Number;
        /// <summary>
        /// Status;
        /// </summary>
        [DataMember]
        public string Status;
        /// <summary>
        /// TypeMeasurements SO - spectrum occupation; LV - Level; FO - Offset; FR - Frequency; FM - Freq. Modulation; AM - Ampl. Modulation; BW	- Bandwidth Meas; BE - Bearing; SA - Sub Audio Tone; PR	- Program; PI - PI Code  (Hex Code identifying radio program); SI - Sound ID; LO	- Location;
        /// </summary>
        [DataMember]
        public MeasurementType TypeMeasurements;
        /// <summary>
        /// Longitude, DEC 
        /// </summary>
        [DataMember]
        public double? CurrentLon;
        /// <summary>
        /// Latitude, DEC
        /// </summary>
        [DataMember]
        public double? CurrentLat;
        /// <summary>
        /// Sensor name
        /// </summary>
        [DataMember]
        public string SensorName;
        /// <summary>
        /// Sensor TechId
        /// </summary>
        [DataMember]
        public string SensorTechId;
        /// <summary>
        /// Count measurements
        /// </summary>
        [DataMember]
        public int? CountStationMeasurements;
        /// <summary>
        /// Count Unknown Station Measurements
        /// </summary>
        [DataMember]
        public int? CountUnknownStationMeasurements;

    }
}
