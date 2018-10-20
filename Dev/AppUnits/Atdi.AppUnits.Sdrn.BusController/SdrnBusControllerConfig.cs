using Atdi.Contracts.Sdrn.Server;
using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnBusControllerConfig
    {
        private readonly static string _sdatas = "Atdi.AppServer.AppService.SdrnsController";
        private readonly ISdrnServerEnvironment _environment;

        public SdrnBusControllerConfig(IComponentConfig config, ISdrnServerEnvironment environment)
        {
            this._environment = environment;

            this.ApiVersion = config.GetParameterAsString("ApiVersion");

            this.BusHost = config.GetParameterAsString("MessageBus.Host");
            this.BusVirtualHost = config.GetParameterAsString("MessageBus.VirtualHost");
            this.BusPort = config.GetParameterAsInteger("MessageBus.Port");
            this.BusUser = config.GetParameterAsString("MessageBus.User");
            this.BusPassword = config.GetParameterAsDecodeString("MessageBus.Password", _sdatas);

            this.ServerExchange = config.GetParameterAsString("Server.Exchange");
            this.DeviceExchange = config.GetParameterAsString("Device.Exchange");
            this.ServerQueueNamePart = config.GetParameterAsString("Server.QueueNamePart");
            this.DeviceQueueNamePart = config.GetParameterAsString("Device.QueueNamePart");

            var serverQueuesParam = config.GetParameterAsString("Server.Queues");
            var serverQueues = new Dictionary<string, ServerQueueDecriptor>();

            if (!string.IsNullOrEmpty(serverQueuesParam))
            {
                var serverQueuesParts = serverQueuesParam.Split(new string[] { "; ", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (serverQueuesParts.Length > 0)
                {
                    serverQueues = serverQueuesParts.Select(es => new ServerQueueDecriptor(es)).ToDictionary(k=> k.RoutingKey, v => v);
                }
            }
            this.ServerQueueus = serverQueues;

            this.UseEncryption = config.GetParameterAsBoolean("MessageConvertor.UseEncryption");
            this.UseCompression = config.GetParameterAsBoolean("MessageConvertor.UseCompression");

            
            

        }

        public ISdrnServerEnvironment Environment => _environment;

        public string BuildServerQueueName(string routingKey)
        {
            return $"{this.ServerQueueNamePart}.[{this._environment.ServerInstance}].[{routingKey}].[v{this.ApiVersion}]";
        }

        public string BuildServerQueueName(string routingKey, string internalPart)
        {
            return $"{this.ServerQueueNamePart}.[{this._environment.ServerInstance}].[{routingKey}].[{internalPart}].[v{this.ApiVersion}]";
        }

        public string BuildServerUnprocessedQueueName(string routingKey)
        {
            return BuildServerQueueName(routingKey, "unprocessed");
        }
        public string BuildServerRejectedQueueName(string routingKey)
        {
            return BuildServerQueueName(routingKey, "rejected");
        }

        internal string GetServerInnerExchangeName()
        {
            return $"{this.ServerExchange}.[inner].[v{this.ApiVersion}]";
        }

        public string BuildServerErrorsQueueName(string routingKey)
        {
            return BuildServerQueueName(routingKey, "errors");
        }
        public string BuildServerTrashQueueName(string routingKey)
        {
            return BuildServerQueueName(routingKey, "trash");
        }

        public string BuildServerQueueRoute(string routingKey)
        {
            return $"[{this._environment.ServerInstance}].[{routingKey}]";
        }
        public string BuildServerQueueRoute(string routingKey, string routingSubKey)
        {
            return $"[{this._environment.ServerInstance}].[{routingKey}].[{routingSubKey}]";
        }

        public string BuildServerUnprocessedQueueRoute(string routingKey)
        {
            return BuildServerQueueRoute(routingKey, "unprocessed");
        }
        public string BuildServerRejectedQueueRoute(string routingKey)
        {
            return BuildServerQueueRoute(routingKey, "rejected");
        }
        public string BuildServerErrorQueueRoute(string routingKey)
        {
            return BuildServerQueueRoute(routingKey, "errors");
        }
        public string BuildServerTrashQueueRoute(string routingKey)
        {
            return BuildServerQueueRoute(routingKey, "trash");
        }

        public string GetDeviceExchangeName ()
        {
            return $"{this.DeviceExchange}.[v{this.ApiVersion}]";
        }

        public string GetServerExchangeName ()
        {
            return $"{this.ServerExchange}.[v{this.ApiVersion}]";
        }

        public string BusHost { get; set; }

        public string BusVirtualHost { get; set; }

        public int? BusPort { get; set; }

        public string BusUser { get; set; }

        public string BusPassword { get; set; }

        public string ApiVersion { get; set; }

        public string DeviceExchange { get; set; }
        public string ServerExchange { get; set; }

        public string ServerQueueNamePart { get; set; }

        public string DeviceQueueNamePart { get; set; }

        public bool UseEncryption { get; set; }

        public bool UseCompression { get; set; }

        public IDictionary<string, ServerQueueDecriptor> ServerQueueus { get; set; }

    }
}
