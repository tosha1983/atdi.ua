using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration
{
	class AppServerComponentConfig
	{

		[ComponentConfigProperty("AutoImport.SdrnServer.StartDelay")]
		public int? AutoImportSdrnServerStartDelay { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.RepeatDelay")]
		public int? AutoImportSdrnServerRepeatDelay { get; set; }


		[ComponentConfigProperty("AutoImport.Files.Folder")]
		public string AutoImportFilesFolder { get; set; }

		[ComponentConfigProperty("AutoImport.Files.StartDelay")]
		public int? AutoImportFilesStartDelay { get; set; }

		[ComponentConfigProperty("AutoImport.Files.RepeatDelay")]
		public int? AutoImportFilesRepeatDelay { get; set; }

	}
}
