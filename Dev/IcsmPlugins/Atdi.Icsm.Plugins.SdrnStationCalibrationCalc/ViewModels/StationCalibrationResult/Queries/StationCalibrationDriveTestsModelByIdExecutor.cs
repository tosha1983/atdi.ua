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
    public class StationCalibrationDriveTestsModelByIdExecutor : IReadQueryExecutor<StationCalibrationDriveTestsModelById, StationCalibrationDriveTestsModel[]>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public StationCalibrationDriveTestsModelByIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public StationCalibrationDriveTestsModel[] Read(StationCalibrationDriveTestsModelById criterion)
        {
            var listDriveTestsModel = new List<StationCalibrationDriveTestsModel>();

            var query = _dataLayer.GetBuilder<CS_ES.IStationCalibrationDriveTestResult>()
               .Read()
               .Select(
                 c => c.CalibrationResultId,
                c => c.CountPointsInDriveTest,
                c => c.ExternalCode,
                c => c.ExternalSource,
                c => c.MeasGcid,
                c => c.MaxPercentCorellation,
                c => c.StationGcid,
                c => c.ResultDriveTestStatus,
                c => c.DriveTestId,
                c => c.LinkToStationMonitoringId

             ).Filter(f => f.CalibrationResultId, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.ResultId);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            while (reader.Read())
            {
                listDriveTestsModel.Add(
                     new StationCalibrationDriveTestsModel
                    {
                        CountPointsInDriveTest = reader.GetValue(c => c.CountPointsInDriveTest),
                        ResultDriveTestStatus = reader.GetValue(c => c.ResultDriveTestStatus),
                        MeasGcid = reader.GetValue(c => c.MeasGcid),
                        MaxPercentCorellation = (float)Math.Round(reader.GetValue(c => c.MaxPercentCorellation),2),
                        StationGcid = reader.GetValue(c => c.StationGcid),
                        ExternalSource = reader.GetValue(c => c.ExternalSource),
                        ExternalCode = reader.GetValue(c => c.ExternalCode),
                        DriveTestId = reader.GetValue(c => c.DriveTestId), // ссылка  на инфоцентр (SM_MEAS_RESULTS)
                        LinkToStationMonitoringId = reader.GetValue(c => c.LinkToStationMonitoringId) // ссылка  на сервер расчетов станция (CONTEXT_STATIONS)
                    }
            );
            }
            return listDriveTestsModel.ToArray();
        }
    }
}
