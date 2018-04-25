using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts
{
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    public class DataConstraintValuesOperand : DataConstraintOperand
    {
        public DataConstraintValuesOperand()
        {
            this.Type = DataConstraintOperandType.Values;
        }

        [DataMember]
        public CommonDataType DataType;
        [DataMember]
        public string[] StringValues;
        [DataMember]
        public bool?[] BooleanValues;
        [DataMember]
        public DateTime?[] DateTimeValues;
        [DataMember]
        public int?[] IntegerValues;
        [DataMember]
        public double?[] DoubleValues;
        [DataMember]
        public byte[][] BytesValues;
    }
}
