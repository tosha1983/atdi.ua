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


namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Adapters
{
    public sealed class StationCalibrationDriveTestsDataAdapter : EntityDataAdapter<CS_ES.IStationCalibrationDriveTestResult, StationCalibrationDriveTestsModel>
    {
        public StationCalibrationDriveTestsDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long resultId;

        protected override void PrepareQuery(IReadQuery<CS_ES.IStationCalibrationDriveTestResult> query)
        {
            query.Select(
                c => c.Id,
                c => c.CalibrationResultId,
                c => c.CountPointsInDriveTest,
                c => c.ExternalCode,
                c => c.ExternalSource,
                c => c.LicenseGsid,
                c => c.MaxPercentCorellation,
                c => c.RealGsid,
                c => c.ResultDriveTestStatus,
                c => c.DriveTestId,
                c => c.LinkToStationMonitoringId

            ).Filter(f => f.CalibrationResultId, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, resultId);
        }
        protected override StationCalibrationDriveTestsModel ReadData(IDataReader<CS_ES.IStationCalibrationDriveTestResult> reader, int index)
        {
            return new StationCalibrationDriveTestsModel
            {
                Id = reader.GetValue(c => c.Id),
                CountPointsInDriveTest  = reader.GetValue(c => c.CountPointsInDriveTest),
                ResultDriveTestStatus = reader.GetValue(c => c.ResultDriveTestStatus),
                RealGsid = reader.GetValue(c => c.RealGsid),
                MaxPercentCorellation = reader.GetValue(c => c.MaxPercentCorellation),
                LicenseGsid = reader.GetValue(c => c.LicenseGsid),
                ExternalSource = reader.GetValue(c => c.ExternalSource),
                ExternalCode  = reader.GetValue(c => c.ExternalCode),
                DriveTestId = reader.GetValue(c => c.DriveTestId), // ссылка  на инфоцентр (SM_MEAS_RESULTS)
                LinkToStationMonitoringId = reader.GetValue(c => c.LinkToStationMonitoringId) // ссылка  на сервер расчетов станция (CONTEXT_STATIONS)
            };
        }
    }

}
