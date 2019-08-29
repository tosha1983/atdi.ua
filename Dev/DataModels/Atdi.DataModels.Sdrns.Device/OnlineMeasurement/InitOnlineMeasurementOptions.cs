using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class InitOnlineMeasurementOptions
    {
        /// <summary>
        /// The measurement record ID
        /// </summary>
        [DataMember]
        public long OnlineMeasId { get; set; }


    }

    

}
