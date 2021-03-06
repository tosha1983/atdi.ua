﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// The static class represents the web query services specification
    /// </summary>
    public static class ServicesSpecification
    {
        /// <summary>
        /// The namespace of the web queries services operation and data contracts
        /// </summary>
        public const string Namespace = CommonServicesSpecification.Namespace + "/WebQueries";
    }
}
