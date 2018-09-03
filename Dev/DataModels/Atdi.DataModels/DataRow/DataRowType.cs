﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public enum DataRowType
    {
        [EnumMember]
        TypedCell,

        [EnumMember]
        StringCell,

        [EnumMember]
        ObjectCell,

    }
}
