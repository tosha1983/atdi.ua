﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.DataConstraint
{
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ColumnOperand : Operand
    {
        public ColumnOperand()
        {
            this.Type = OperandType.Column;
        }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string ColumnName { get; set; }

        public override string ToString()
        {
            return $"Type = '{Type}', Column = '{ColumnName}'";
        }
    }
}
