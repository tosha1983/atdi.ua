using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents sensor for measurement. Includes administrative and technical data.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]

    public class Sensor
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public int? Id { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status { get; set; }

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
        public double? Azimuth { get; set; }

        /// <summary>
        /// Elevation, degree 
        /// </summary>
        [DataMember]
        public double? Elevation { get; set; }

        /// <summary>
        /// Altitude above ground level, m
        /// </summary>
        [DataMember]
        public double? AGL { get; set; }

        /// <summary>
        /// IdSysARGUS
        /// </summary>
        [DataMember]
        public string SysArgusId { get; set; }

        /// <summary>
        /// TypeSensor
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// StepMeasTime
        /// </summary>
        [DataMember]
        public double? StepMeasTime { get; set; }

        /// <summary>
        /// RxLoss, dB 
        /// </summary>
        [DataMember]
        public double? RxLoss { get; set; }

        /// <summary>
        /// Operation HH From
        /// </summary>
        [DataMember]
        public double? OpHHFr { get; set; }

        /// <summary>
        /// Operation HH To
        /// </summary>
        [DataMember]
        public double? OpHHTo { get; set; }

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
        public DateTime? Created { get; set; }

        /// <summary>
        /// CreatedBy
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Equipment of sensor
        /// </summary>
        [DataMember]
        public SensorEquipment Equipment { get; set; }

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
        public SensorPoligon Poligon { get; set; }
    }
}
