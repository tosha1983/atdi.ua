using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries
{
    public class GetResultEventsByIdAndResultIdExecutor : IReadQueryExecutor<GetResultEventsByEventIdAndResultId, CalcResultEventsModel[]>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetResultEventsByIdAndResultIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public CalcResultEventsModel[] Read(GetResultEventsByEventIdAndResultId criterion)
        {
            var listEnents = new List<CalcResultEventsModel>();
            var query = _dataLayer.GetBuilder<ICalcResultEvent>()
                .Read()
                .Select(c => c.Id)
                .Select(c => c.CreatedDate)
                .Select(c => c.LevelCode)
                .Select(c => c.LevelName)
                .Select(c => c.Message)
                .Select(c => c.DataJson)
                .Filter(c => c.RESULT.Id, criterion.ResultId)
                .Filter(c => c.Id, FilterOperator.GreaterThan, criterion.EventId)
                .OrderByAsc(c => c.Id);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            while (reader.Read())
            {
                listEnents.Add(new CalcResultEventsModel()
                {
                    Id = reader.GetValue(c => c.Id),
                    CreatedDate = reader.GetValue(c => c.CreatedDate),
                    LevelCode = reader.GetValue(c => c.LevelCode),
                    LevelName = reader.GetValue(c => c.LevelName),
                    Message = reader.GetValue(c => c.Message),
                    State = string.IsNullOrEmpty(reader.GetValue(c => c.DataJson)) ? null : reader.GetValueAs<CurrentProgress>(c => c.DataJson)
                });
            }
            return listEnents.ToArray();
        }
    }
}
