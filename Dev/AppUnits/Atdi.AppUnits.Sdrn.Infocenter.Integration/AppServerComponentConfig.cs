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

		[ComponentConfigProperty("AutoImport.SDRN.DriveTest.StartDelay")]
		public int? AutoImportSdrnDriveTestStartDelay { get; set; }

		[ComponentConfigProperty("AutoImport.SDRN.DriveTest.RepeatDelay")]
		public int? AutoImportSdrnDriveTestRepeatDelay { get; set; }

	}
}
