using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Queries
{
    public class StationCalibrationStaModelByResultIdExecutor : IReadQueryExecutor<StationCalibrationStaModelByResultId, StationCalibrationStaModel[]>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public StationCalibrationStaModelByResultIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public StationCalibrationStaModel[] Read(StationCalibrationStaModelByResultId criterion)
        {
            var listStationCalibrationStaModel = new List<StationCalibrationStaModel>();

            var query = _dataLayer.GetBuilder<CS_ES.IStationCalibrationStaResult>()
               .Read()
               .Select(
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
                 c => c.New_Freq_MHz,
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
             ).Filter(f => f.CalibrationResultId, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.ResultId);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            while (reader.Read())
            {
                listStationCalibrationStaModel.Add(new StationCalibrationStaModel()
                {
                    Id = reader.GetValue(c => c.Id),
                    ResultId = reader.GetValue(c => c.CalibrationResultId),
                    LicenseGsid = reader.GetValue(c => c.LicenseGsid),
                    ExternalSource = reader.GetValue(c => c.ExternalSource),
                    ExternalCode = reader.GetValue(c => c.ExternalCode),
                    MaxCorellation = reader.GetValue(c => c.MaxCorellation),
                    ResultStationStatus = reader.GetValue(c => c.ResultStationStatus),
                    RealGsid = reader.GetValue(c => c.RealGsid),
                    New_Altitude_m = reader.GetValue(c => c.New_Altitude_m),
                    Old_Altitude_m = reader.GetValue(c => c.Old_Altitude_m),
                    New_Tilt_deg = reader.GetValue(c => c.New_Tilt_deg),
                    New_Power_dB = reader.GetValue(c => c.New_Power_dB),
                    New_Lon_deg = reader.GetValue(c => c.New_Lon_deg),
                    New_Lat_deg = reader.GetValue(c => c.New_Lat_deg),
                    New_Freq_MHz = reader.GetValue(c => c.New_Freq_MHz),
                    New_Azimuth_deg = reader.GetValue(c => c.New_Azimuth_deg),
                    Old_Azimuth_deg = reader.GetValue(c => c.Old_Azimuth_deg),
                    Old_Freq_MHz = reader.GetValue(c => c.Old_Freq_MHz),
                    Old_Lat_deg = reader.GetValue(c => c.Old_Lat_deg),
                    Old_Lon_deg = reader.GetValue(c => c.Old_Lon_deg),
                    Old_Power_dB = reader.GetValue(c => c.Old_Power_dB),
                    Old_Tilt_deg = reader.GetValue(c => c.Old_Tilt_deg),
                    StationMonitoringId = reader.GetValue(c => c.StationMonitoringId) // ccылка на сервер расчетов станция (CONTEXT_STATIONS)
                });
            }
            return listStationCalibrationStaModel.ToArray();
        }
    }
}
