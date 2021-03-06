﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels
{
    // <summary>
    /// Represents the action of create record
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class TypedRowCreationAction : CreationAction
    {
        public TypedRowCreationAction()
        {
            this.Type = ActionType.Create;
            this.RowType = DataRowType.TypedCell;
        }

        [DataMember]
        public TypedDataRow Row { get; set; }
    }
}
