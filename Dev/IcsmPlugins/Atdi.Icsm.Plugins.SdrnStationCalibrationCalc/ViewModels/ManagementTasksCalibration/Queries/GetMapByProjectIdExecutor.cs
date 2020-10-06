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
    public class GetMapByProjectIdExecutor : IReadQueryExecutor<GetMapByProjectId, string>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly ILogger _logger;

        public GetMapByProjectIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _logger = logger;
        }
        public string Read(GetMapByProjectId criterion)
        {
            try
            {
                var listMaps = new List<string>();
                var query = _dataLayer.GetBuilder<IProjectMap>()
                   .Read()
                   .Select(c => c.MapName)
                   .Filter(c => c.PROJECT.Id, criterion.Id)
                   .OnTop(1)
                   .OrderByAsc(c => c.Id);

                var reader = _dataLayer.Executor.ExecuteReader(query);
                if (!reader.Read())
                {
                    return "";
                }

                return reader.GetValue(c => c.MapName);
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
