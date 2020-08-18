using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;


namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries
{
    public class CalcTaskByResultIdExecutor : IReadQueryExecutor<CalcTaskByResultId, CalcTaskModel>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public CalcTaskByResultIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public CalcTaskModel Read(CalcTaskByResultId criterion)
        {
            var query = _dataLayer.GetBuilder<IStationCalibrationResult>()
                .Read()
                .Select(c => c.RESULT.TASK.CONTEXT.Id)
                .Select(c => c.RESULT.TASK.MapName)
                .Select(c => c.RESULT.StatusCode)
                .Filter(c => c.Id, criterion.ResultId);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            return new CalcTaskModel() { ContextId = reader.GetValue(c => c.RESULT.TASK.CONTEXT.Id), MapName = reader.GetValue(c => c.RESULT.TASK.MapName) };
        }
    }
}
