using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class BusConfig : IBusConfig
    {
        public BusConfig()
        {
            this.Buffer = new BusBufferConfig();
        }

        public string Address { get; set; }

        public string ApiVersion { get; set; }

        public string Name { get; set; }

        public string Host { get; set; }

        public int? Port { get; set; }

        public string VirtualHost { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public bool? UseEncryption { get; set; }

        public bool? UseCompression { get; set; }

        public IBusBufferConfig Buffer { get; }

        public ContentType ContentType { get; set; }
    }
}
