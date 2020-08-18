using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using MP = Atdi.WpfControls.EntityOrm.Controls;
using Atdi.Platform.Logging;
using Atdi.DataModels.Api.EntityOrm.WebClient;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries
{
    public class DriveTestByResultIdsExecutor : IReadQueryExecutor<DriveTestByResultIds, long[]>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly ILogger _logger;

        public DriveTestByResultIdsExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _logger = logger;
        }
        public long[] Read(DriveTestByResultIds criterion)
        {
            try
            {
                var listIds = new List<long>();
                var query = _dataLayer.GetBuilder<CS_ES.IStationCalibrationDriveTestResult>()
                   .Read()
                   .Select(c => c.DriveTestId)
                   .Filter(f => f.CalibrationResultId, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.ResultId)
                   .Distinct();

                var reader = _dataLayer.Executor.ExecuteReader(query);
                while (reader.Read())
                {
                    listIds.Add(reader.GetValue(c => c.DriveTestId));
                }
                return listIds.ToArray();
            }
            catch (EntityOrmWebApiException e)
            {
                if (e.ServerException != null)
                {
                    _logger.Exception((EventContext)"IReadQueryExecutor", (EventCategory)"Execute", new Exception($"ExceptionMessage: {e.ServerException.ExceptionMessage}; ExceptionType: {e.ServerException.ExceptionType}; InnerException: {e.ServerException.InnerException}; Message: {e.ServerException.Message}!"));
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.Exception((EventContext)"IReadQueryExecutor", (EventCategory)"Execute", e);
                return null;
                throw;
            }
        }
    }
}
