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
        /// The document name
        /// </summary>
        [DataMember]
        public string DocName { get; set; }

        /// <summary>
        /// The document path
        /// </summary>
        [DataMember]
        public string PathDocument { get; set; }

        /// <summary>
        /// The memo document
        /// </summary>
        [DataMember]
        public string DocMemo { get; set; }

        /// <summary>
        /// The file name without extension
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// The file extension 
        /// </summary>
        [DataMember]
        public string FileExtension { get; set; }

        /// <summary>
        /// The file size in bytes 
        /// </summary>
        [DataMember]
        public int Size { get; set; }

        /// <summary>
        /// Th date created document
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Th type of document
        /// </summary>
        [DataMember]
        public string TypeDocument { get; set; }

        /// <summary>
        /// Th created by 
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }
    }
}
