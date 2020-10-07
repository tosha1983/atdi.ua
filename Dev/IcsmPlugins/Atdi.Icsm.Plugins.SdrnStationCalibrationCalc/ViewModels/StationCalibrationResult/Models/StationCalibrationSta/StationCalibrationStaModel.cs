using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult
{
    public class StationCalibrationStaModel
    {
        public long Id { get; set; }
        public long StationMonitoringId { get; set; }
        public string ExternalSource { get; set; }
        public string ExternalCode { get; set; }
        public string LicenseGsid { get; set; }
        public string RealGsid { get; set; }
        public string ResultStationStatus { get; set; }
        public float MaxCorellation { get; set; }
        public int Old_Altitude_m { get; set; }
        public float Old_Tilt_deg { get; set; }
        public float Old_Azimuth_deg { get; set; }
        public string Old_Lat_dms_deg { get; set; }
        public string Old_Lon_dms_deg { get; set; }
        public double Old_Lat_dec_deg { get; set; }
        public double Old_Lon_dec_deg { get; set; }
        public float Old_Power_dB { get; set; }
        public float Old_Freq_MHz { get; set; }
        public int? New_Altitude_m { get; set; }
        public float? New_Tilt_deg { get; set; }
        public float? New_Azimuth_deg { get; set; }
        public string New_Lat_dms_deg { get; set; }
        public string New_Lon_dms_deg { get; set; }
        public double? New_Lat_dec_deg { get; set; }
        public double? New_Lon_dec_deg { get; set; }
        public float? New_Power_dB { get; set; }
        public long ResultId { get; set; }
        public double Freq_MHz { get; set; }
        public string Standard { get; set; }
        public double DeltaCorrelation_pc { get; set; }
        public int UsedPoints_pc { get; set; }
        public int CountPointsInDriveTest { get; set; }
    }
}
