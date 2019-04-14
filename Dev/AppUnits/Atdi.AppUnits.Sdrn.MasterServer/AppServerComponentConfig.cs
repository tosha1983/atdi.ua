using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.MasterServer
{
    class AppServerComponentConfig
    {
        private const string _sdatas = "Atdi.AppServer.AppService.SdrnsController";

        [ComponentConfigProperty("License.FileName")]
        public string LicenseFileName { get; set; }

        [ComponentConfigProperty("License.OwnerId", SharedSecret = _sdatas)]
        public string LicenseOwnerId { get; set; }

        [ComponentConfigProperty("License.ProductKey", SharedSecret = _sdatas)]
        public string LicenseProductKey { get; set; }

        [ComponentConfigProperty("DataBus.ApiVersion")]
        public string DataBusApiVersion { get; set; }
        [ComponentConfigProperty("DataBus.Name")]
        public string DataBusName { get; set; }
        [ComponentConfigProperty("DataBus.Host")]
        public string DataBusHost { get; set; }
        [ComponentConfigProperty("DataBus.Port")]
        public int? DataBusPort { get; set; }
        [ComponentConfigProperty("DataBus.VirtualHost")]
        public string DataBusVirtualHost { get; set; }
        [ComponentConfigProperty("DataBus.User")]
        public string DataBusUser { get; set; }

        [ComponentConfigProperty("DataBus.Password", SharedSecret = _sdatas)]
        public string DataBusPassword { get; set; }
        [ComponentConfigProperty("DataBus.UseEncryption")]
        public bool? DataBusUseEncryption { get; set; }
        [ComponentConfigProperty("DataBus.UseCompression")]
        public bool? DataBusUseCompression { get; set; }
        [ComponentConfigProperty("DataBus.ContentType")]
        public string DataBusContentType { get; set; }
        [ComponentConfigProperty("DataBus.UseBuffer")]
        public string DataBusUseBuffer { get; set; }
        [ComponentConfigProperty("DataBus.Buffer.OutboxFolder")]
        public string DataBusBufferOutboxFolder { get; set; }
        [ComponentConfigProperty("DataBus.Buffer.ConnectionStringConfig")]
        public string DataBusBufferConnectionStringConfig { get; set; }
        [ComponentConfigProperty("DataBus.Buffer.ContentType")]
        public string DataBusBufferContentType { get; set; }

    }
}
