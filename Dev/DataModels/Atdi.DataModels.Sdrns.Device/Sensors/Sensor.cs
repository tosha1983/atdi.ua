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
        /// Defines the Sensor name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Defines the Sensor status
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Defines the Sensor administration
        /// </summary>
        [DataMember]
        public string Administration { get; set; }

        /// <summary>
        /// Network identifier
        /// </summary>
        [DataMember]
        public string NetworkId { get; set; }

        /// <summary>
        /// User’s remarks
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
        //[DataMember]
        //public double? Azimuth { get; set; }

        ///// <summary>
        ///// Elevation, degrees
        ///// </summary>
        //[DataMember]
        //public double? Elevation { get; set; }

        ///// <summary>
        ///// Altitude above ground level, m
        ///// </summary>
        //[DataMember]
        //public double? AGL { get; set; }

        ///// <summary>
        ///// ARGUS system identifier
        ///// </summary>
        //[DataMember]
        //public string SysArgusId { get; set; }

        /// <summary>
        /// Sensor type
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Defines the time step of measurements
        /// </summary>
        [DataMember]
        public double? StepMeasTime { get; set; }

        /// <summary>
        /// Defines the Sensor Rx losses, dB 
        /// </summary>
        [DataMember]
        public double? RxLoss { get; set; }

        ///// <summary>
        ///// Operation hours: From
        ///// </summary>
        //[DataMember]
        //public double? OpHHFr { get; set; }

        ///// <summary>
        ///// Operation hours: To
        ///// </summary>
        //[DataMember]
        //public double? OpHHTo { get; set; }

        ///// <summary>
        ///// Operation days
        ///// </summary>
        //[DataMember]
        //public string OpDays { get; set; }

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
        /// Creator name
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Defines the Sensor equipment
        /// </summary>
        [DataMember]
        public SensorEquipment Equipment { get; set; }
        [DataMember]
        public SensorEquipment[] OtherEquipments { get; set; }
        /// <summary>
        /// Defines the Sensor antenna
        /// </summary>
        [DataMember]
        public SensorAntenna Antenna { get; set; }
        [DataMember]
        public SensorAntenna[] OtherAntennas { get; set; }
        ///// <summary>
        ///// Sensor locations
        ///// </summary>
        [DataMember]
        public SensorLocation[] Locations { get; set; }

        /// <summary>
        /// Defines the points of assigned Sensor polygon
        /// </summary>
        [DataMember]
        public SensorPolygon Polygon { get; set; }
    }
}
