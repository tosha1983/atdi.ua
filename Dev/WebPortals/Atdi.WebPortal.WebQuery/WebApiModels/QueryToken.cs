using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the reference to record of web query
    /// </summary>
    [DataContract]
    public class QueryToken
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public byte[] Stamp { get; set; }
    }
}
