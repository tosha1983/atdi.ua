﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.DataConstraint;

namespace Atdi.DataModels
{
    // <summary>
    /// Represents the action of update record
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    [KnownType(typeof(TypedRowUpdationAction))]
    [KnownType(typeof(StringRowUpdationAction))]
    [KnownType(typeof(ObjectRowUpdationAction))]
    public class UpdationAction : Action
    {
        [DataMember]
        public DataSetColumn[] Columns { get; set; }

        [DataMember]
        public Condition Condition { get; set; }

        [DataMember]
        public DataRowType RowType { get; set; }
    }
}
