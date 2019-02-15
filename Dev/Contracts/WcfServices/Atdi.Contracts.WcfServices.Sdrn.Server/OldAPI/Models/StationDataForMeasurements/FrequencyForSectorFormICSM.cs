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
    public class FrequencyForSectorFormICSM // перечень частот
    {
        [DataMember]
        public int? Id; // Идентификатор частоты
        [DataMember]
        public int? IdPlan; // Идетификатор частотного плана
        [DataMember]
        public int? ChannalNumber;
        [DataMember]
        public decimal? Frequency; //МГц;
    }
}
