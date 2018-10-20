using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    public class ConnectionConfig
    {

        public string ConnectionName { get; set; }
        
        public string HostName { get; set; }

        public int? Port { get; set; }

        public string VirtualHost { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool? AutoRecovery { get; set; }
    }
}
