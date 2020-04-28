using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
	class AppServerComponentConfig
	{
		[ComponentConfigProperty("Thresholds.Gis.ProfileData.ObjectPool.MinSize")]
		public int? ThresholdsGisProfileDataObjectPoolMinSize { get; set; }

		[ComponentConfigProperty("Thresholds.Gis.ProfileData.ObjectPool.MaxSize")]
		public int? ThresholdsGisProfileDataObjectPoolMaxSize { get; set; }


		[ComponentConfigProperty("Thresholds.Gis.ProfileData.ArrayLength")]
		public int? ThresholdsGisProfileDataArrayLength { get; set; }
	}
}
