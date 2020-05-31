using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Queries
{
    public class GetCalcResultByIdExecutor : IReadQueryExecutor<GetCalcResultById, CalcResultModel>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetCalcResultByIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public CalcResultModel Read(GetCalcResultById criterion)
        {
            var query = _dataLayer.GetBuilder<ICalcResult>()
                .Read()
                .Select(
                    c => c.Id,
                    c => c.TASK.Id
                    )
                .Filter(c => c.TASK.Id, criterion.Id);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            return new CalcResultModel()
            {
                Id = reader.GetValue(c => c.Id)
            };
        }
    }
}
