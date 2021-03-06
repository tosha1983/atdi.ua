﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    public class PermissionForAssignment// данные дозвола
    {
        [DataMember]
        public int? Id; // из ICSM
        [DataMember]
        public DateTime? StartDate;
        [DataMember]
        public DateTime? EndDate;
        [DataMember]
        public DateTime? CloseDate; // дата закртытия 
        [DataMember]
        public string DozvilName;
    }

}
