using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Queries
{
    public class GetCalcTaskByIdExecutor : IReadQueryExecutor<GetCalcTaskById, CalcTaskModel>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetCalcTaskByIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public CalcTaskModel Read(GetCalcTaskById criterion)
        {
            var query = _dataLayer.GetBuilder<ICalcTask>()
                .Read()
                .Select(
                    c => c.Id,
                    c => c.MapName,
                    c => c.TypeCode)
                .Filter(c => c.Id, criterion.Id);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            return new CalcTaskModel()
            {
                Id = reader.GetValue(c => c.Id),
                MapName = reader.GetValue(c => c.MapName),
                TypeCode = reader.GetValue(c => c.TypeCode)
            };
        }
    }
}
