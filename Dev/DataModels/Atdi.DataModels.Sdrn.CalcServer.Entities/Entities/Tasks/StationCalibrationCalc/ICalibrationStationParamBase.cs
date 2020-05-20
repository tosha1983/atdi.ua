using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{

	[Entity]
	public interface ICalibrationStationParamBase 
    {
        ICalibrationStationResult CALIBRATION_STATION_RESULT { get; set; }

        int Altitude_m { get; set; }

        float Tilt_Deg { get; set; }

        float Azimuth_deg { get; set; }

        double Lat_deg { get; set; }

        double Lon_deg { get; set; }

        float Power_dB { get; set; }

        double Freq_MHz { get; set; }
    }

}