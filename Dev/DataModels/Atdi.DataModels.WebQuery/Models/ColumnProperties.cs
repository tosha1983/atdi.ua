using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.DataConstraint;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the metadata to the column
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class ColumnProperties
    {
        [DataMember]
        public string FieldJoinFrom { get; set; }
        [DataMember]
        public string FieldJoinTo { get; set; }
        [DataMember]
        public string NameTableFrom { get; set; }
        [DataMember]
        public string NameTableTo { get; set; }
        [DataMember]
        public string NameField { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Type TypeColumn { get; set; }
        [DataMember]
        public int Precision { get; set; }
        [DataMember]
        public string DefaultValueFrom { get; set; }
        [DataMember]
        public string DefaultValueTo { get; set; }
        [DataMember]
        public string DefaultValue { get; set; }
        [DataMember]
        public int PrecisionFieldJoinTo { get; set; }
        [DataMember]
        public int PrecisionFieldJoinFrom { get; set; }

    }
}
