using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
	public class AppServerComponentConfig
	{
		[ComponentConfigProperty("Thresholds.Gis.ProfileData.ObjectPool.MinSize")]
		public int? ThresholdsGisProfileDataObjectPoolMinSize { get; set; }

		[ComponentConfigProperty("Thresholds.Gis.ProfileData.ObjectPool.MaxSize")]
		public int? ThresholdsGisProfileDataObjectPoolMaxSize { get; set; }


		[ComponentConfigProperty("Thresholds.Gis.ProfileData.ArrayLength")]
		public int? ThresholdsGisProfileDataArrayLength { get; set; }


        [ComponentConfigProperty("Thresholds.StationCalibration.ObjectPool.MinSize")]
        public int? ThresholdsStationCalibrationObjectPoolMinSize { get; set; }

        [ComponentConfigProperty("Thresholds.StationCalibration.ObjectPool.MaxSize")]
        public int? ThresholdsStationCalibrationObjectPoolMaxSize { get; set; }


        [ComponentConfigProperty("Thresholds.StationCalibration.ArrayLength")]
        public int? ThresholdsStationCalibrationArrayLength { get; set; }

        public int? MaxCountPointInDriveTest { get; set; }

        public int? MaxCountStationsByOneStandard { get; set; }

        public int? MaxCountDriveTestsByOneStandard { get; set; }

        public int? MinDistanceBetweenDriveTestAndStation_GSM { get; set; }

    }
}
