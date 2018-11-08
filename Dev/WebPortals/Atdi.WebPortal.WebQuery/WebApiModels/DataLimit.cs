using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Specifies a limit of the data to fetch
    /// </summary>
    [DataContract]
    public class DataLimit
    {
        [DataMember]
        public int Value { get; set; }
        [DataMember]
        public LimitValueType Type { get; set; }
    }
}
