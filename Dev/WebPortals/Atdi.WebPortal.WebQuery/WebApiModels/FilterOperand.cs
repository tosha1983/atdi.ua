using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    [DataContract]
    public class FilterOperand
    {
        [DataMember]
        public OperandType Type { get; set; }

        [DataMember]
        public string ColumnSource { get; set; }

        [DataMember]
        public string ColumnName { get; set; }

        [DataMember]
        public DataType DataType { get; set; }

        [DataMember]
        public object Value { get; set; }

        [DataMember]
        public object[] Values { get; set; }
    }
}
