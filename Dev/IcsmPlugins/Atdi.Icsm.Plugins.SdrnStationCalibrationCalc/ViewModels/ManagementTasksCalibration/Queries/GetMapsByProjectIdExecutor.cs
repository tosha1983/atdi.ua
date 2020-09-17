using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Queries
{
    public class GetMapsByProjectIdExecutor : IReadQueryExecutor<GetMapsByProjectId, string[]>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly ILogger _logger;

        public GetMapsByProjectIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _logger = logger;
        }
        public string[] Read(GetMapsByProjectId criterion)
        {
            try
            {
                var listMaps = new List<string>();
                var query = _dataLayer.GetBuilder<IProjectMap>()
                   .Read()
                   .Select(c => c.MapName)
                   .Filter(c => c.PROJECT.Id, criterion.Id)
                   .Distinct();

                var reader = _dataLayer.Executor.ExecuteReader(query);
                while (reader.Read())
                {
                    listMaps.Add(reader.GetValue(c => c.MapName));
                }
                return listMaps.ToArray();
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
