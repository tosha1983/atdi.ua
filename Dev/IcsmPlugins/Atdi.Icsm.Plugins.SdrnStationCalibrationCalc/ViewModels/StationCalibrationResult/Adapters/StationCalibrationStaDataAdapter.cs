using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.Common;


namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Adapters
{
    public sealed class StationCalibrationStaDataAdapter : EntityDataAdapter<CS_ES.IStationCalibrationStaResult, StationCalibrationStaModel>
    {
        public StationCalibrationStaDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long resultId;

        protected override void PrepareQuery(IReadQuery<CS_ES.IStationCalibrationStaResult> query)
        {
            query.Select(
                c => c.Id,
                c => c.CalibrationResultId,
                c => c.MaxCorellation,
                c => c.ExternalCode,
                c => c.ExternalSource,
                c => c.LicenseGsid,
                c => c.ResultStationStatus,
                c => c.RealGsid,
                c => c.New_Altitude_m,
                c => c.New_Azimuth_deg,
                c => c.New_Lat_deg,
                c => c.New_Lon_deg,
                c => c.New_Power_dB,
                c => c.New_Tilt_deg,
                c => c.Old_Altitude_m,
                c => c.Old_Azimuth_deg,
                c => c.Old_Freq_MHz,
                c => c.Old_Lat_deg,
                c => c.Old_Lon_deg,
                c => c.Old_Power_dB,
                c => c.Old_Tilt_deg,
                c => c.StationMonitoringId
            )
            .Filter(f => f.CalibrationResultId, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, resultId);
        }
        protected override StationCalibrationStaModel ReadData(IDataReader<CS_ES.IStationCalibrationStaResult> reader, int index)
        {
            return new StationCalibrationStaModel
            {
                Id = reader.GetValue(c => c.Id),
                ResultId = reader.GetValue(c => c.CalibrationResultId),
                LicenseGsid = reader.GetValue(c => c.LicenseGsid),
                ExternalSource = reader.GetValue(c => c.ExternalSource),
                ExternalCode  = reader.GetValue(c => c.ExternalCode),
                MaxCorellation = (float)Math.Round(reader.GetValue(c => c.MaxCorellation),2),
                ResultStationStatus = reader.GetValue(c => c.ResultStationStatus),
                RealGsid = reader.GetValue(c => c.RealGsid),
                New_Altitude_m = reader.GetValue(c => c.New_Altitude_m),
                Old_Altitude_m = reader.GetValue(c => c.Old_Altitude_m),
                New_Tilt_deg = (float)Math.Round(reader.GetValue(c => c.New_Tilt_deg),1),
                New_Power_dB = (float)Math.Round(reader.GetValue(c => c.New_Power_dB),1),
                New_Lon_dms_deg = ConvertCoordinates.DecToDmsToString(ICSM.IMPosition.Dec2Dms(reader.GetValue(c => c.New_Lon_deg)), EnumCoordLine.Lon),
                New_Lat_dms_deg = ConvertCoordinates.DecToDmsToString(ICSM.IMPosition.Dec2Dms(reader.GetValue(c => c.New_Lat_deg)), EnumCoordLine.Lat),
                New_Lon_dec_deg = reader.GetValue(c => c.New_Lon_deg),
                New_Lat_dec_deg = reader.GetValue(c => c.New_Lat_deg),
                New_Azimuth_deg  = reader.GetValue(c => c.New_Azimuth_deg),
                Old_Azimuth_deg = reader.GetValue(c => c.Old_Azimuth_deg),
                Old_Freq_MHz = (float)reader.GetValue(c => c.Old_Freq_MHz),
                Old_Lat_dms_deg = ConvertCoordinates.DecToDmsToString(ICSM.IMPosition.Dec2Dms(reader.GetValue(c => c.Old_Lat_deg)), EnumCoordLine.Lat),
                Old_Lon_dms_deg = ConvertCoordinates.DecToDmsToString(ICSM.IMPosition.Dec2Dms(reader.GetValue(c => c.Old_Lon_deg)), EnumCoordLine.Lon),
                Old_Lon_dec_deg = reader.GetValue(c => c.Old_Lon_deg),
                Old_Lat_dec_deg = reader.GetValue(c => c.Old_Lat_deg),
                Old_Power_dB  = (float)Math.Round(reader.GetValue(c => c.Old_Power_dB),1),
                Old_Tilt_deg = (float)Math.Round(reader.GetValue(c => c.Old_Tilt_deg),1),
                StationMonitoringId = reader.GetValue(c => c.StationMonitoringId) // ccылка на сервер расчетов станция (CONTEXT_STATIONS)
            };
        }
    }

}
