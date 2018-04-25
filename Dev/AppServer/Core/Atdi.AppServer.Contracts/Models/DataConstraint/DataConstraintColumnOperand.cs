using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts
{
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    public class DataConstraintColumnOperand : DataConstraintOperand
    {
        public DataConstraintColumnOperand()
        {
            this.Type = DataConstraintOperandType.Column;
        }

        [DataMember]
        public string Alias;
        [DataMember]
        public string ColumnName;
    }
}
