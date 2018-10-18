using Atdi.Contracts.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    internal class EventSystemConfig
    {
        public EventSystemConfig(IEventSiteConfig config)
        {
            this.ApiVersion = config.GetValue<string>(EventSiteConfig.ApiVersion);
            this.AppName = config.GetValue<string>(EventSiteConfig.AppName);
            this.EventBusHost = config.GetValue<string>(EventSiteConfig.EventBusHost);
            this.EventBusVirtualHost = config.GetValue<string>(EventSiteConfig.EventBusVirtualHost);

            this.EventBusPort = config.GetValue<int?>(EventSiteConfig.EventBusPort);
            this.EventBusUser = config.GetValue<string>(EventSiteConfig.EventBusUser);
            this.EventBusPassword = config.GetValue<string>(EventSiteConfig.EventBusPassword);
            this.EventExchange = config.GetValue<string>(EventSiteConfig.EventExchange);
            this.EventQueueNamePart = config.GetValue<string>(EventSiteConfig.EventQueueNamePart);
            this.ErrorsQueueName = config.GetValue<string>(EventSiteConfig.ErrorsQueueName);
            this.LogQueueName = config.GetValue<string>(EventSiteConfig.LogQueueName);
            this.UseEncryption = config.GetValue<bool>(EventSiteConfig.UseEncryption);
            this.UseCompression = config.GetValue<bool>(EventSiteConfig.UseCompression);
        }

        internal string ApiVersion { get; set; }
        internal string AppName { get; set; }
        internal string EventBusHost { get; set; }
        internal string EventBusVirtualHost { get; set; }
        internal int? EventBusPort { get; set; }
        internal string EventBusUser { get; set; }
        internal string EventBusPassword { get; set; }
        internal string EventExchange { get; set; }
        internal string EventQueueNamePart { get; set; }
        internal string ErrorsQueueName { get; set; }
        internal string LogQueueName { get; set; }
        internal bool UseEncryption { get; set; }
        internal bool UseCompression { get; set; }

        public string BuildEventExchangeName()
        {
            return $"{this.EventExchange}.[v{this.ApiVersion}]";
        }

 
        public string BuildCommonLogQueueName()
        {
            return $"{this.EventQueueNamePart}.[@_all].[#_all]._log_.[v{this.ApiVersion}]";
        }

        public string BuildEventQueueName(string eventName, string subscriberName)
        {
            return $"{this.EventQueueNamePart}.[@{eventName}].[#{subscriberName}]._work_.[v{this.ApiVersion}]";
        }

        public string BuildEventErrorsQueueName(string eventName, string subscriberName)
        {
            return $"{this.EventQueueNamePart}.[@{eventName}].[#{subscriberName}]._errors_.[v{this.ApiVersion}]";
        }

        public string BuildEventLogQueueName(string eventName)
        {
            return $"{this.EventQueueNamePart}.[@{eventName}].[#_all]._log_.[v{this.ApiVersion}]";
        }

    }
}
