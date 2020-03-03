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
    /// 
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class RadioControlParams
    {
        /// <summary>
        /// RadioControlMeasFreq_MHz
        /// </summary>
        [DataMember]
        public double? RadioControlMeasFreq_MHz;

        /// <summary>
        /// RadioControlBandWidth
        /// </summary>
        [DataMember]
        public double? RadioControlBandWidth;
     
    }
}