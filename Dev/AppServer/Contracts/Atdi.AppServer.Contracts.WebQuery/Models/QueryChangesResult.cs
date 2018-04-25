using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryChangesResult
    {
        [DataMember]
        public QueryReference QueryRef;
        [DataMember]
        public QueryChangesetActionResult[] Actions;
    }
}
