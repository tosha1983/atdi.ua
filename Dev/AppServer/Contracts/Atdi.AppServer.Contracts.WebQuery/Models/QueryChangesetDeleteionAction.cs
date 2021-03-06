﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    // <summary>
    /// Represents the action of delete record
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryChangesetDeleteionAction : QueryChangesetAction
    {
        [DataMember]
        public RecordReference RecordRef;
    }
}
