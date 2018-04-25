using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts
{
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    public class DataConstraintValueOperand : DataConstraintOperand
    {
        public DataConstraintValueOperand()
        {
            this.Type = DataConstraintOperandType.Value;
        }

        [DataMember]
        public CommonDataType DataType;
        [DataMember]
        public string StringValue;
        [DataMember]
        public bool? BooleanValue;
        [DataMember]
        public DateTime? DateTimeValue;
        [DataMember]
        public int? IntegerValue;
        [DataMember]
        public double? DoubleValue;
        [DataMember]
        public byte[] BytesValue;
    }
}
