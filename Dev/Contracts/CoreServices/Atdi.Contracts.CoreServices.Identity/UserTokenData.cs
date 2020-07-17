using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.Identity
{ 
    [Serializable]
    public class UserTokenData
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserCode { get; set; }

        public DateTime AuthDate { get; set; }
    }

    [Serializable]
    public class ServiceTokenData
    {
	    public string Id { get; set; }

	    public string Name { get; set; }

	    public DateTime AuthDate { get; set; }
    }
}
