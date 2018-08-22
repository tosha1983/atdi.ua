using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Блок системной информации
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationSysInfoBlock
    {
        /// <summary>
        /// Тип блока системной информации
        /// </summary>
        [DataMember]
        public string Type { get; set; }
        /// <summary>
        /// Данные блока системной информации
        /// </summary>
        [DataMember]
        public string Data { get; set; }
    }
}
