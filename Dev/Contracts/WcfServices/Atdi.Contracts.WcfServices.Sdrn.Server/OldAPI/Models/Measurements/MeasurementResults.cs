﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// Represents measurements results
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [KnownType(typeof(AmplitudeModulationMeasurementResult))]
    [KnownType(typeof(BandwidthMeasurementResult))]
    [KnownType(typeof(FrequencyModulationMeasurementResult))]
    [KnownType(typeof(FrequencyOffsetMeasurementResult))]
    [KnownType(typeof(IntermodulationMeasurementResult))]
    [KnownType(typeof(LevelMeasurementResult))]
    [KnownType(typeof(LocationMeasurementResult))]
    [KnownType(typeof(MVinfoMeasurementResult))]
    [KnownType(typeof(PhaseModulationMeasurementResult))]
    [KnownType(typeof(SpectrumOccupationMeasurementResult))]
    [KnownType(typeof(SubAudioToneMeasurementResult))]
    [KnownType(typeof(TextrMeasurementResult))]
    [KnownType(typeof(LevelMeasurementOnlineResult))]
    [KnownType(typeof(ResultsMeasurementsStation))]
    [KnownType(typeof(LevelMeasurementsCar))]
    [KnownType(typeof(MeasurementsParameterGeneral))]
    [KnownType(typeof(StationMeasurements))]
    [KnownType(typeof(LocationSensorMeasurement))]
    [KnownType(typeof(FrequencyMeasurement))]
    [KnownType(typeof(MeasurementResult))]
    [KnownType(typeof(MeasTaskIdentifier))]


    public class MeasurementResults
    {
        /// <summary>
        /// Identifier of measurements results
        /// </summary>
        [DataMember]
        public MeasurementResultsIdentifier Id;

        /// <summary>
        /// Value of Antenna, dB
        /// </summary>
        [DataMember]
        public double? AntVal;
        /// <summary>
        /// Time of measurements
        /// </summary>
        [DataMember]
        public DateTime TimeMeas;

        ///// <summary>
        ///// Time start (SpectrumOccupation, Level, BandwidthMeas)
        ///// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }
        ///// <summary>
        ///// Time stop  (SpectrumOccupation, Level, BandwidthMeas)
        ///// </summary>
        [DataMember]
        public DateTime StopTime { get; set; }
        /// <summary>
        /// Rank of data
        /// </summary>
        [DataMember]
        public int? DataRank;
        /// <summary>
        /// measurement number
        /// </summary>
        [DataMember]
        public int? N;
        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status;
        /// <summary>
        /// Type of measurements, SO - spectrum occupation; LV - Level; FO - Offset; FR - Frequency; FM - Freq. Modulation; AM - Ampl. Modulation; BW	- Bandwidth Meas; BE - Bearing; SA - Sub Audio Tone; PR	- Program; PI - PI Code  (Hex Code identifying radio program); SI - Sound ID; LO	- Location;
        /// </summary>
        [DataMember]
        public MeasurementType TypeMeasurements;
        /// <summary>
        /// Represents station of measurement
        /// </summary>
        [DataMember]
        public StationMeasurements StationMeasurements;
        /// <summary>
        /// Location Station (sensor) for measurement 
        /// </summary>
        [DataMember]
        public LocationSensorMeasurement[] LocationSensorMeasurement;
        /// <summary>
        /// Represents frequencies of measurements 
        /// </summary>
        [DataMember]
        public FrequencyMeasurement[] FrequenciesMeasurements;
        /// <summary>
        /// Results of measurements
        /// </summary>
        [DataMember]
        public MeasurementResult[] MeasurementsResults;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public ResultsMeasurementsStation[] ResultsMeasStation;
        /// <summary>
        /// Reference levels
        /// </summary>
        [DataMember]
        public ReferenceLevels RefLevels { get; set; }
        /// <summary>
        /// Emittings
        /// </summary>
        [DataMember]
        public Emitting[] Emittings { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Route[] Routes;
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
        /// <summary>
        /// Scans Number
        /// </summary>
        [DataMember]
        public int? ScansNumber;
        /// <summary>
        /// LowFreq
        /// </summary>
        [DataMember]
        public double? LowFreq;
        /// <summary>
        /// UpFreq
        /// </summary>
        [DataMember]
        public double? UpFreq;
        /// <summary>
        /// Sensor title
        /// </summary>
        [DataMember]
        public string SensorTitle;
    }
}
