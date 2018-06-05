using System;
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
    public class ChangesResult
    {
        /// <summary>
        /// The ID of the changeset
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public ActionResult[] Actions { get; set; }
    }
}
