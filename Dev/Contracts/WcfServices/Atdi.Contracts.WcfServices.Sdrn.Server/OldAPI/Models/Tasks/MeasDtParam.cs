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
    /// Represents receiver (detector) setting (parameter) for measurements
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasDtParam
    {
        /// <summary>
        /// Type of measurements, SO - spectrum occupation; LV - Level; FO - Offset; FR - Frequency; FM - Freq. Modulation; AM - Ampl. Modulation; BW	- Bandwidth Meas; BE - Bearing; SA - Sub Audio Tone; PR	- Program; PI - PI Code  (Hex Code identifying radio program); SI - Sound ID; LO	- Location;
        /// </summary>
        [DataMember]
        public MeasurementType TypeMeasurements;
        /// <summary>
        /// RBW, kHz
        /// </summary>
        [DataMember]
        public double? RBW;//kHz
        /// <summary>
        /// VBW, kHz
        /// </summary>
        [DataMember]
        public double? VBW;//kHz
        /// <summary>
        /// Attenuation in radiofrequency, dB
        /// </summary>
        [DataMember]
        public double? RfAttenuation; //0, 10, 20, 30, ...,  dB 
        /// <summary>
        /// Attenuation in interim frequency, dB
        /// </summary>
        [DataMember]
        public double? IfAttenuation;
        /// <summary>
        /// Time singl measurement, sec
        /// </summary>
        [DataMember]
        public double? MeasTime;
        /// <summary>
        /// Type of detecting - Avarage; Peak; MaxPeak; MinPeak;
        /// </summary>
        [DataMember]
        public DetectingType DetectType;//Avarage; Peak; MaxPeak; MinPeak;
        /// <summary>
        /// Type of demodulation
        /// </summary>
        [DataMember]
        public string Demod;//AM; FM; ...
        /// <summary>
        /// Gain of preamplifier, dB
        /// </summary>
        [DataMember]
        public int? Preamplification;//0, 10, 20, ..., dB
        /// <summary>
        /// Mode of measurements
        /// </summary>
        [DataMember]
        public MeasurementMode Mode;
        /// <summary>
        /// Reference Level
        /// </summary>
        [DataMember]
        public double? ReferenceLevel { get; set; }
        /// <summary>
        /// Number total scan
        /// </summary>
        [DataMember]
        public int? NumberTotalScan { get; set; }
    }
}
