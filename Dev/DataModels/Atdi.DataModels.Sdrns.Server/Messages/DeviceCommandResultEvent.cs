using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{
    /// <summary>
    /// Represents the command result
    /// </summary>
    [Serializable]
    public class DeviceCommandResultEvent
    {

        public string CommandId { get; set; }

        /// <summary>
        /// Command status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Custom field, text
        /// </summary>
        public string CustTxt1 { get; set; }
        /// <summary>
        /// Custom field, datetime
        /// </summary>
        public DateTime? CustDate1 { get; set; }

        /// <summary>
        /// Custom field, number
        /// </summary>
        public double? CustNbr1 { get; set; }

        /// <summary>
        /// Sensor name
        /// </summary>
        public string SensorName { get; set; }

        /// <summary>
        /// Sensor tech Id
        /// </summary>
        public string SensorTechId { get; set; }
    }
}
