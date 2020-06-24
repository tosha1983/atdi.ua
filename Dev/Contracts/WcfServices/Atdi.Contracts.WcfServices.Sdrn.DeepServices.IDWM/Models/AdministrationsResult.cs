using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.Contracts.WcfServices.Sdrn.DeepServices.IDWM
{
    [DataContract(Namespace = Specification.Namespace)]
    public class AdministrationsResult
    {
        /// <summary>
        /// Массив администраций
        /// </summary>
        [DataMember]
		public string Administration { get; set; }
        /// <summary>
        /// Азимут
        /// </summary>
        [DataMember]
        public float? Azimuth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Point Point { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? Distance { get; set; }

    }
}
