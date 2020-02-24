using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Sdrn.CalcServer
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


		[ComponentConfigProperty("EventSystem.ApiVersion")]
		public string EventSystemApiVersion { get; set; }

		[ComponentConfigProperty("EventSystem.AppName")]
		public string EventSystemAppName { get; set; }

		[ComponentConfigProperty("EventSystem.EventBus.Host")]
		public string EventSystemEventBusHost { get; set; }

		[ComponentConfigProperty("EventSystem.EventBus.Port")]
		public int? EventSystemEventBusPort { get; set; }

		[ComponentConfigProperty("EventSystem.EventBus.VirtualHost")]
		public string EventSystemEventBusVirtualHost { get; set; }

		[ComponentConfigProperty("EventSystem.EventBus.User")]
		public string EventSystemEventBusUser { get; set; }

		[ComponentConfigProperty("EventSystem.EventBus.Password", SharedSecret = _sdatas)]
		public string EventSystemEventBusPassword { get; set; }

		[ComponentConfigProperty("EventSystem.EventExchange")]
		public string EventSystemEventExchange { get; set; }

		[ComponentConfigProperty("EventSystem.EventQueueNamePart")]
		public string EventSystemEventQueueNamePart { get; set; }

		[ComponentConfigProperty("EventSystem.ErrorsQueueName")]
		public string EventSystemErrorsQueueName { get; set; }

		[ComponentConfigProperty("EventSystem.LogQueueName")]
		public string EventSystemLogQueueName { get; set; }

		[ComponentConfigProperty("EventSystem.UseEncryption")]
		public bool? EventSystemUseEncryption { get; set; }

		[ComponentConfigProperty("EventSystem.UseCompression")]
		public bool? EventSystemUseCompression { get; set; }



		[ComponentConfigProperty("ProcessJob.StartDelay")]
		public int? ProcessJobStartDelay { get; set; }

		[ComponentConfigProperty("ProcessJob.RepeatDelay")]
		public int? ProcessJobRepeatDelay { get; set; }

		[ComponentConfigProperty("Threshold.MaxSteps")]
		public int? ThresholdMaxSteps { get; set; }

	}
}
