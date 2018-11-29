using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
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
