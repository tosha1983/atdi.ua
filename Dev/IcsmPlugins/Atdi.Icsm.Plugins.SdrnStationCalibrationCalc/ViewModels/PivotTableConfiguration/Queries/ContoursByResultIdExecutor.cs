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
    public class ContoursByResultIdExecutor : IReadQueryExecutor<ContoursByResultId, string[]>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly ILogger _logger;

        public ContoursByResultIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _logger = logger;
        }
        public string[] Read(ContoursByResultId criterion)
        {
            try
            {
                var query = _dataLayer.GetBuilder<CS_ES.IStationCalibrationResult>()
                   .Read()
                   .Select(c => c.RESULT.TASK.Id)
                   .Filter(f => f.Id, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.ResultId)
                   .OrderByDesc(o => o.Id);

                var reader = _dataLayer.Executor.ExecuteReader(query);
                if (!reader.Read())
                {
                    return null;
                }

                var taskId = reader.GetValue(c => c.RESULT.TASK.Id);

                var queryCt = _dataLayer.GetBuilder<CS_ES.IStationCalibrationArgs>()
                   .Read()
                   .Select(c => c.Contours)
                   .Filter(f => f.TASK.Id, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, taskId);

                var readerCt = _dataLayer.Executor.ExecuteReader(queryCt);
                if (!readerCt.Read())
                {
                    return null;
                }
                return readerCt.GetValue(c => c.Contours);
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
