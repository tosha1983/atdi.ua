using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts
{
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    [KnownType(typeof(DataConstraintValueOperand))]
    [KnownType(typeof(DataConstraintColumnOperand))]
    [KnownType(typeof(DataConstraintValuesOperand))]
    public class DataConstraintOperand
    {
        [DataMember]
        public DataConstraintOperandType Type;
    }
}
