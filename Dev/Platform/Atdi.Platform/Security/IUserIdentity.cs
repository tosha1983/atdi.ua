using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace Atdi.Platform
{
    public interface IUserIdentity : IIdentity
    {
        string Id { get;  }
    }
}
