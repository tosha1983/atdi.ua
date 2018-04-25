using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the metadata to the column
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class ColumnMetadata
    {
        [DataMember]
        public CommonDataType Type;
        [DataMember]
        public string Title;
        [DataMember]
        public string Description;
        [DataMember]
        public uint Position;
        [DataMember]
        public uint Width;
        [DataMember]
        public uint Order;
        [DataMember]
        public uint Rank;
        [DataMember]
        public uint Show;
        [DataMember]
        public ColumnStyle Style;
        [DataMember]
        public string Format;
        [DataMember]
        public string JsonOptions;
    }
}
