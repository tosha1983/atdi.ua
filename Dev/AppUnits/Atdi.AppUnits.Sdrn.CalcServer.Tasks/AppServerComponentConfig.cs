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

        [ComponentConfigProperty("Threshold.DriveTest.Points.MaxFetchRows")]
        public int? MaxCountPointInDriveTest { get; set; }

        [ComponentConfigProperty("Threshold.Stations.Standard.MaxRows")]
        public int? MaxCountStationsByOneStandard { get; set; }

        [ComponentConfigProperty("Threshold.DriveTest.Standard.MaxRows")]
        public int? MaxCountDriveTestsByOneStandard { get; set; }

        [ComponentConfigProperty("Threshold.DriveTestAndStation.GSM.MinDistance")]
        public int? MinDistanceBetweenDriveTestAndStation_GSM { get; set; }

        [ComponentConfigProperty("Threshold.DriveTestAndStation.UMTS.MinDistance")]
        public int? MinDistanceBetweenDriveTestAndStation_UMTS { get; set; }

        [ComponentConfigProperty("Threshold.DriveTestAndStation.LTE.MinDistance")]
        public int? MinDistanceBetweenDriveTestAndStation_LTE { get; set; }

        [ComponentConfigProperty("Threshold.DriveTestAndStation.CDMA.MinDistance")]
        public int? MinDistanceBetweenDriveTestAndStation_CDMA { get; set; }

        [ComponentConfigProperty("Thresholds.GE06PointEarthGeometric.ObjectPool.MinSize")]
        public int? ThresholdsGE06PointEarthGeometricObjectPoolMinSize { get; set; }

        [ComponentConfigProperty("Thresholds.GE06PointEarthGeometric.ObjectPool.MaxSize")]
        public int? ThresholdsGE06PointEarthGeometricObjectPoolMaxSize { get; set; }

        [ComponentConfigProperty("Thresholds.GE06PointEarthGeometric.ArrayLength")]
        public int? ThresholdsGE06PointEarthGeometricArrayLength { get; set; }


        [ComponentConfigProperty("Thresholds.CountoursPointExtendedPool.ObjectPool.MinSize")]
        public int? ThresholdsCountoursPointExtendedPoolMinSize { get; set; }

        [ComponentConfigProperty("Thresholds.CountoursPointExtendedPool.ObjectPool.MaxSize")]
        public int? ThresholdsCountoursPointExtendedPoolMaxSize { get; set; }

        [ComponentConfigProperty("Thresholds.CountoursPointExtendedPool.ArrayLength")]
        public int? ThresholdsCountoursPointExtendedPoolArrayLength { get; set; }


        [ComponentConfigProperty("Thresholds.CountoursResultPool.ObjectPool.MinSize")]
        public int? ThresholdsCountoursResultPoolMinSize { get; set; }

        [ComponentConfigProperty("Thresholds.CountoursResultPool.ObjectPool.MaxSize")]
        public int? ThresholdsCountoursResultPoolMaxSize { get; set; }

        [ComponentConfigProperty("Thresholds.CountoursResultPool.ArrayLength")]
        public int? ThresholdsCountoursResultPoolArrayLength { get; set; }



        [ComponentConfigProperty("Brific.DBSource")]
        public string BrificDBSource { get; set; }

        [ComponentConfigProperty("Thresholds.RefractivityGradient")]
        public int? ThresholdsRefractivityGradient { get; set; }

    }
}
