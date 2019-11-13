using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the file content
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class FileContent : FileInfo
    {
        /// <summary>
        /// The file content
        /// </summary>
        [DataMember]
        public byte[] Body { get; set; }
    }
}
