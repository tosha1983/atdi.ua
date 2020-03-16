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
    /// 
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class GroupSensors
    {
        /// <summary>
        /// Идентификатор сенсора 
        /// </summary>
        [DataMember]
        public long SensorId;


        /// <summary>
        /// Частота
        /// </summary>
        [DataMember]
        public double Freq_MHz;

    }
}
