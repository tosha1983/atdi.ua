using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Queries
{
    public class GetCalcTaskStatusByIdExecutor : IReadQueryExecutor<GetCalcTaskStatusById, byte>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetCalcTaskStatusByIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public byte Read(GetCalcTaskStatusById criterion)
        {
            var query = _dataLayer.GetBuilder<ICalcTask>()
                .Read()
                .Select(
                    c => c.Id,
                    c => c.StatusCode)
                .Filter(c => c.Id, criterion.TaskId);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return 0;
            }

            return reader.GetValue(c => c.StatusCode);
        }
    }
}
