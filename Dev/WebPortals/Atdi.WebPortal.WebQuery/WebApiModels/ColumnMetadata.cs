using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the metadata to the column
    /// </summary>
    [DataContract]
    public class ColumnMetadata
    {
        [DataMember]
        public DataType Type { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public OrderType Order { get; set; }
        
        [DataMember]
        public string Format { get; set; }

       
    }
}
