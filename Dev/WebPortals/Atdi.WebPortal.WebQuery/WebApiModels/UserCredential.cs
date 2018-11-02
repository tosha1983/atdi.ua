using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the dictionary of other arguments of the operation
    /// </summary>
    [DataContract]

    public class UserCredential
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
