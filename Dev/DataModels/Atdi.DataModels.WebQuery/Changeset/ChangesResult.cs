using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class ChangesResult
    {
        [DataMember]
        public ActionResult[] Actions { get; set; }
    }
}
