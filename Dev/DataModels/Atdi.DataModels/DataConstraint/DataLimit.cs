using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.DataConstraint
{
    /// <summary>
    /// Specifies a limit of the data to fetch
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DataLimit
    {

        public int Value { get; set; }

        public LimitValueType Type { get; set; }
}
}
