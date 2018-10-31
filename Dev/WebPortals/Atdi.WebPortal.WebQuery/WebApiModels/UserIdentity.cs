using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    public class UserIdentity
    {
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of user
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The token of current user
        /// </summary>
        [DataMember]
        public UserToken UserToken { get; set; }
    }
}
