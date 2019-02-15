﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// Represents frequency for measurement 
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class MeasFreq
    {
        /// <summary>
        /// frequency, MHz
        /// </summary>
        [DataMember]
        public double Freq;
    }
}
