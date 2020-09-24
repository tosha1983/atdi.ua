using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.Infocenter.Entities;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Queries
{
    public class GetProjectionsExecutor : IReadQueryExecutor<GetProjections, string[]>
    {
        private readonly AppComponentConfig _config;
        private readonly InfocenterDataLayer _dataLayer;
        private readonly ILogger _logger;

        public GetProjectionsExecutor(AppComponentConfig config, InfocenterDataLayer dataLayer, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _logger = logger;
        }
        public string[] Read(GetProjections criterion)
        {
            try
            {
                var listPrj = new List<string>();
                var query = _dataLayer.GetBuilder<IMap>()
                   .Read()
                   .Select(c => c.Projection)
                   .Distinct();

                var reader = _dataLayer.Executor.ExecuteReader(query);
                while (reader.Read())
                {
                    listPrj.Add(reader.GetValue(c => c.Projection));
                }
                return listPrj.ToArray();
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
