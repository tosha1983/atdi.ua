using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer
{
    public class AppServerContext : LoggedObject, IAppServerContext
    {
        public AppServerContext(ILogger logger) : base(logger)
        {
        }

        public ISecurityContext SecurityContext  { get; set; }
        //=>  throw new NotImplementedException();

        public string HostName { get; set; }
        //throw new NotImplementedException();
    }
}
