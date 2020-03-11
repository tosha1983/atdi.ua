using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels;

namespace Atdi.Modules.Sdrn.Calculation
{
    /// <summary>
    /// The static class represents the SDRNS Server services specification
    /// </summary>
    public static class Specification
    {
        /// <summary>
        /// The namespace of the SDRNS Device services operation and data contracts
        /// </summary>
        public const string Namespace = CommonSpecification.Namespace + "/Sdrns/Server";
    }
}
