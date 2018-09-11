﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents the device command
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class DeviceCommand
    {
        /// <summary>
        /// The instance name of the SDRN Server that puts the task on measurement
        /// </summary>
        [DataMember]
        public string SdrnServer { get; set; }

        /// <summary>
        /// Sensor name
        /// </summary>
        [DataMember]
        public string SensorName { get; set; }

        /// <summary>
        /// Equipment technical ID
        /// </summary>
        [DataMember]
        public string EquipmentTechId { get; set; }

        /// <summary>
        /// Command ID
        /// </summary>
        [DataMember]
        public string CommandId { get; set; }

        /// <summary>
        /// Command name
        /// </summary>
        [DataMember]
        public string Command { get; set; }

        /// <summary>
        /// Custom field, text
        /// </summary>
        [DataMember]
        public string CustTxt1 { get; set; }
        /// <summary>
        /// Custom field, datetime
        /// </summary>
        [DataMember]
        public DateTime? CustDate1 { get; set; }

        /// <summary>
        /// Custom field, number
        /// </summary>
        [DataMember]
        public double? CustNbr1 { get; set; }
    }
}