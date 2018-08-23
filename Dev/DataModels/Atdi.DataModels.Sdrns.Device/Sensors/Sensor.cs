using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents the sensor for measurement. Includes administrative and technical data
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]

    public class Sensor
    {
        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Administration
        /// </summary>
        [DataMember]
        public string Administration { get; set; }

        /// <summary>
        /// Network identifier
        /// </summary>
        [DataMember]
        public string NetworkId { get; set; }

        /// <summary>
        /// Remarks
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
        /// Azimuth, degrees
        /// </summary>
        [DataMember]
        public double? Azimuth { get; set; }

        /// <summary>
        /// Elevation, degrees
        /// </summary>
        [DataMember]
        public double? Elevation { get; set; }

        /// <summary>
        /// Altitude above ground level, m
        /// </summary>
        [DataMember]
        public double? AGL { get; set; }

        /// <summary>
        /// ARGUS system identifier
        /// </summary>
        [DataMember]
        public string SysArgusId { get; set; }

        /// <summary>
        /// Sensor type
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// StepMeasTime
        /// </summary>
        [DataMember]
        public double? StepMeasTime { get; set; }

        /// <summary>
        /// Rx losses, dB 
        /// </summary>
        [DataMember]
        public double? RxLoss { get; set; }

        /// <summary>
        /// Operation hours: From
        /// </summary>
        [DataMember]
        public double? OpHHFr { get; set; }

        /// <summary>
        /// Operation hours: To
        /// </summary>
        [DataMember]
        public double? OpHHTo { get; set; }

        /// <summary>
        /// Operation days
        /// </summary>
        [DataMember]
        public string OpDays { get; set; }

        /// <summary>
        /// Custom data, text
        /// </summary>
        [DataMember]
        public string CustTxt1 { get; set; }
        /// <summary>
        /// Custom data, datetime
        /// </summary>
        [DataMember]
        public DateTime? CustDate1 { get; set; }

        /// <summary>
        /// Custom data, number (double)
        /// </summary>
        [DataMember]
        public double? CustNbr1 { get; set; }

        /// <summary>
        /// Date created
        /// </summary>
        [DataMember]
        public DateTime? Created { get; set; }

        /// <summary>
        /// Created by
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
        /// Sensor locations
        /// </summary>
        [DataMember]
        public SensorLocation[] Locations { get; set; }

        /// <summary>
        /// Points of sensor polygon
        /// </summary>
        [DataMember]
        public SensorPolygon Polygon { get; set; }
    }
}
