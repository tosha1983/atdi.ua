using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the detailed file information
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class FileInfo
    {
        /// <summary>
        /// The ID file descriptor from ICSM
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The file name without extension
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The file extension 
        /// </summary>
        [DataMember]
        public string Extension { get; set; }

        /// <summary>
        /// The file size in bytes 
        /// </summary>
        [DataMember]
        public int Size { get; set; }
    }
}
