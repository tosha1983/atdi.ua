﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IStationCalibrationStaResult_PK
    {
		long Id { get; set; }
	}

	[Entity]
	public interface IStationCalibrationStaResult : IStationCalibrationStaResult_PK
    {
        IStationCalibrationResult CALIBRATION_RESULT { get; set; }
        string TableName { get; set; }
        long? StationId { get; set; }
        string GSIDByICSM { get; set; }
        string GSIDByMeasurement { get; set; }
        string ResultStationStatus { get; set; }
        float MaxCorellation { get; set; }
        int Old_Altitude_m { get; set; }
        float Old_Tilt_deg { get; set; }
        float Old_Azimuth_deg { get; set; }
        double Old_Lat_deg { get; set; }
        double Old_Lon_deg { get; set; }
        float Old_Power_dB { get; set; }
        double Old_Freq_MHz { get; set; }
        int New_Altitude_m { get; set; }
        float New_Tilt_deg { get; set; }
        float New_Azimuth_deg { get; set; }
        double New_Lat_deg { get; set; }
        double New_Lon_deg { get; set; }
        float New_Power_dB { get; set; }
        double New_Freq_MHz { get; set; }
    }

}