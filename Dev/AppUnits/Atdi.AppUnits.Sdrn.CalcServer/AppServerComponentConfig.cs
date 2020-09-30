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
		/*
		 *
			<!-- Конфишурация шины системы событий -->

            <!-- Версия АПИ  -->
            <parameter name="EventSystem.ApiVersion" value="1.0" />
            <!-- Имя приложения  -->
            <parameter name="EventSystem.AppName" value="SDRN.Server" />
            <!-- Хост без порта  -->
            <parameter name="EventSystem.EventBus.Host" value="192.168.33.110" />
            <!-- Виртуальных хост размещения очередей -->
            <parameter name="EventSystem.EventBus.VirtualHost" value="Test.SDRN.SDRNSV-SBD12-A00-8591.EventSystem" />
            <!-- Порт, не обязательно, в этом случаи будет использован по умолчанию  -->
            <parameter name="EventSystem.EventBus.Port" value="" />
            <!-- Имя пользователя -->
            <parameter name="EventSystem.EventBus.User" value="andrey" />
            <!-- Зашифрованное значение пароля пользователя -->
            <parameter name="EventSystem.EventBus.Password" value="EAAAAF/F9XJFjjkBH2Ga08O6HRa3h9ZfpXMwtagtuwTSepnJ" />
            <parameter name="EventSystem.EventExchange" value="EX.SDRN.EventSystem" />
            <parameter name="EventSystem.EventQueueNamePart" value="Q.SDRN.EventSystem" />
            <parameter name="EventSystem.ErrorsQueueName" value="errors" />
            <parameter name="EventSystem.LogQueueName" value="log" />
            <!-- Признак шифрования передаваемого сообщения через шину -->
            <parameter name="EventSystem.UseEncryption" value="false" />
            <!-- Признак сжатия передаваемого сообщения через шину -->
            <parameter name="EventSystem.UseCompression" value="false" />		 
		 *
		 */

		//[ComponentConfigProperty("EventSystem.ApiVersion")]
		//public string EventSystemApiVersion { get; set; }

		//[ComponentConfigProperty("EventSystem.AppName")]
		//public string EventSystemAppName { get; set; }

		//[ComponentConfigProperty("EventSystem.EventBus.Host")]
		//public string EventSystemEventBusHost { get; set; }

		//[ComponentConfigProperty("EventSystem.EventBus.Port")]
		//public int? EventSystemEventBusPort { get; set; }

		//[ComponentConfigProperty("EventSystem.EventBus.VirtualHost")]
		//public string EventSystemEventBusVirtualHost { get; set; }

		//[ComponentConfigProperty("EventSystem.EventBus.User")]
		//public string EventSystemEventBusUser { get; set; }

		//[ComponentConfigProperty("EventSystem.EventBus.Password", SharedSecret = _sdatas)]
		//public string EventSystemEventBusPassword { get; set; }

		//[ComponentConfigProperty("EventSystem.EventExchange")]
		//public string EventSystemEventExchange { get; set; }

		//[ComponentConfigProperty("EventSystem.EventQueueNamePart")]
		//public string EventSystemEventQueueNamePart { get; set; }

		//[ComponentConfigProperty("EventSystem.ErrorsQueueName")]
		//public string EventSystemErrorsQueueName { get; set; }

		//[ComponentConfigProperty("EventSystem.LogQueueName")]
		//public string EventSystemLogQueueName { get; set; }

		//[ComponentConfigProperty("EventSystem.UseEncryption")]
		//public bool? EventSystemUseEncryption { get; set; }

		//[ComponentConfigProperty("EventSystem.UseCompression")]
		//public bool? EventSystemUseCompression { get; set; }



		[ComponentConfigProperty("ProcessJob.StartDelay")]
		public int? ProcessJobStartDelay { get; set; }

		[ComponentConfigProperty("ProcessJob.RepeatDelay")]
		public int? ProcessJobRepeatDelay { get; set; }

		[ComponentConfigProperty("Threshold.MaxSteps")]
		public int? ThresholdMaxSteps { get; set; }

		[ComponentConfigProperty("Maps.LocalStorage.Folder")]
		public string MapsLocalStorageFolder { get; set; }

		[ComponentConfigProperty("CommandJob.StartDelay")]
		public int? CommandJobStartDelay { get; set; }

		[ComponentConfigProperty("CommandJob.RepeatDelay")]
		public int? CommandJobRepeatDelay { get; set; }
	}
}
