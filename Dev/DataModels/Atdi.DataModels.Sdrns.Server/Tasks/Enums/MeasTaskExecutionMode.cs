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
    /// Mode of measurement task execution
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
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
