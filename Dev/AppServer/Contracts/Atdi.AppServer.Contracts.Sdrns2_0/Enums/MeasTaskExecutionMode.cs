using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Mode of measurement task execution
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public enum MeasTaskExecutionMode
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Automatic,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Manual
    }
}
