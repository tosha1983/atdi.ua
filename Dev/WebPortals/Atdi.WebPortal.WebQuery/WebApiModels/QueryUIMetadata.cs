using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the metadata to the web query
    /// </summary>
    [DataContract]
    public class QueryUIMetadata
    {
        [DataMember]
        public string[] ViewFormColumns { get; set; }

        [DataMember]
        public string[] AddFormColumns { get; set; }

        [DataMember]
        public string[] EditFormColumns { get; set; }

        [DataMember]
        public string[] TableColumns { get; set; }

    }
}
