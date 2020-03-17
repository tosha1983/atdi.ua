using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    /// Represents sensor for measurement. Includes administrative and technical data.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [KnownType(typeof(SensorEquip))]
    [KnownType(typeof(SensorLocation))]
    [KnownType(typeof(SensorPoligonPoint))]
    [KnownType(typeof(SensorAntenna))]
    [KnownType(typeof(SensorIdentifier))]
    public class Sensor
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public SensorIdentifier Id { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status { get; set; }
        /// <summary>
        /// Title
        /// </summary>
        [DataMember]
        public string Title { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Administration
        /// </summary>
        [DataMember]
        public string Administration { get; set; }
        /// <summary>
        /// NetworkId
        /// </summary>
        [DataMember]
        public string NetworkId { get; set; }
        /// <summary>
        /// Remark
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
        /// <summary>
        /// Bring into use date 
        /// </summary>
        [DataMember]
        public DateTime? BiuseDate { get; set; }
        /// <summary>
        /// End of use date
        /// </summary>
        [DataMember]
        public DateTime? EouseDate { get; set; }
        /// <summary>
        /// Azimuth, degree
        /// </summary>
        [DataMember]
        public Double? Azimuth { get; set; } //degree
        /// <summary>
        /// Elevation, degree 
        /// </summary>
        [DataMember]
        public Double? Elevation { get; set; } //degree 
        /// <summary>
        /// Altitude above ground level, m
        /// </summary>
        [DataMember]
        public Double? AGL { get; set; }
        /// <summary>
        /// IdSysARGUS
        /// </summary>
        [DataMember]
        public string IdSysARGUS { get; set; }
        /// <summary>
        /// TypeSensor
        /// </summary>
        [DataMember]
        public string TypeSensor { get; set; }
        /// <summary>
        /// StepMeasTime
        /// </summary>
        [DataMember]
        public Double? StepMeasTime { get; set; }
        /// <summary>
        /// RxLoss, dB 
        /// </summary>
        [DataMember]
        public Double? RxLoss { get; set; }
        /// <summary>
        /// Operation HH From
        /// </summary>
        [DataMember]
        public Double? OpHHFr { get; set; }
        /// <summary>
        /// Operation HH To
        /// </summary>
        [DataMember]
        public Double? OpHHTo { get; set; }
        /// <summary>
        /// Operation Days
        /// </summary>
        [DataMember]
        public string OpDays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CustTxt1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? CustData1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Double? CustNbr1 { get; set; }
        /// <summary>
        /// DateCreated
        /// </summary>
        [DataMember]
        public DateTime? DateCreated { get; set; }
        /// <summary>
        /// CreatedBy
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Equipment of sensor
        /// </summary>
        [DataMember]
        public SensorEquip Equipment { get; set; }
        /// <summary>
        /// Antenna of sensor
        /// </summary>
        [DataMember]
        public SensorAntenna Antenna { get; set; }
        /// <summary>
        /// Locations of sensor
        /// </summary>
        [DataMember]
        public SensorLocation[] Locations { get; set; }
        /// <summary>
        /// Point of poligon of sensor
        /// </summary>
        [DataMember]
        public SensorPoligonPoint[] Poligon { get; set; }
    }
}
