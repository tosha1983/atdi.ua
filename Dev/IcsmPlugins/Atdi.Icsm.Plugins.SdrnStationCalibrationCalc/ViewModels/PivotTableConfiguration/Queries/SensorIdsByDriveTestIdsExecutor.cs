using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using CS_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using MP = Atdi.WpfControls.EntityOrm.Controls;
using Atdi.Platform.Logging;
using Atdi.DataModels.Api.EntityOrm.WebClient;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries
{
    public class SensorIdsByDriveTestIdsExecutor : IReadQueryExecutor<SensorIdsByDriveTestIds, long[]>
    {
        private readonly AppComponentConfig _config;
        private readonly InfocenterDataLayer _dataLayer;
        private readonly ILogger _logger;

        public SensorIdsByDriveTestIdsExecutor(AppComponentConfig config, InfocenterDataLayer dataLayer, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _logger = logger;
        }
        public long[] Read(SensorIdsByDriveTestIds criterion)
        {
            try
            {
                var listIds = new List<long>();
                var query = _dataLayer.GetBuilder<CS_ES.IDriveTest>()
                   .Read()
                   .Select(c => c.RESULT.SENSOR.Id)
                   .Filter(f => f.Id, DataModels.Api.EntityOrm.WebClient.FilterOperator.In, criterion.DriveTestIds)
                   .Distinct();

                var reader = _dataLayer.Executor.ExecuteReader(query);
                while (reader.Read())
                {
                    listIds.Add(reader.GetValue(c => c.RESULT.SENSOR.Id));
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
