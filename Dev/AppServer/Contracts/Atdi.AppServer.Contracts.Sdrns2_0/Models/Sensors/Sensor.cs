using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Represents sensor for measurement. Includes administrative and technical data.
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
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
        public SensorIdentifier Id;
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
        /// Administration
        /// </summary>
        [DataMember]
        public string Administration;
        /// <summary>
        /// NetworkId
        /// </summary>
        [DataMember]
        public string NetworkId;
        /// <summary>
        /// Remark
        /// </summary>
        [DataMember]
        public string Remark;
        /// <summary>
        /// Bring into use date 
        /// </summary>
        [DataMember]
        public DateTime? BiuseDate; 
        /// <summary>
        /// End of use date
        /// </summary>
        [DataMember]
        public DateTime? EouseDate; 
        /// <summary>
        /// Azimuth, degree
        /// </summary>
        [DataMember]
        public Double? Azimuth; //degree
        /// <summary>
        /// Elevation, degree 
        /// </summary>
        [DataMember]
        public Double? Elevation; //degree 
        /// <summary>
        /// Altitude above ground level, m
        /// </summary>
        [DataMember]
        public Double? AGL; 
        /// <summary>
        /// IdSysARGUS
        /// </summary>
        [DataMember]
        public string IdSysARGUS;
        /// <summary>
        /// TypeSensor
        /// </summary>
        [DataMember]
        public string TypeSensor;
        /// <summary>
        /// StepMeasTime
        /// </summary>
        [DataMember]
        public Double? StepMeasTime;
        /// <summary>
        /// RxLoss, dB 
        /// </summary>
        [DataMember]
        public Double? RxLoss; 
        /// <summary>
        /// Operation HH From
        /// </summary>
        [DataMember]
        public Double? OpHHFr;
        /// <summary>
        /// Operation HH To
        /// </summary>
        [DataMember]
        public Double? OpHHTo;
        /// <summary>
        /// Operation Days
        /// </summary>
        [DataMember]
        public string OpDays;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CustTxt1;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? CustData1;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Double? CustNbr1;
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
        /// Equipment of sensor
        /// </summary>
        [DataMember]
        public SensorEquip Equipment;
        /// <summary>
        /// Antenna of sensor
        /// </summary>
        [DataMember]
        public SensorAntenna Antenna;
        /// <summary>
        /// Locations of sensor
        /// </summary>
        [DataMember]
        public SensorLocation[] Locations;
        /// <summary>
        /// Point of poligon of sensor
        /// </summary>
        [DataMember]
        public SensorPoligonPoint[] Poligon;
    }
}
