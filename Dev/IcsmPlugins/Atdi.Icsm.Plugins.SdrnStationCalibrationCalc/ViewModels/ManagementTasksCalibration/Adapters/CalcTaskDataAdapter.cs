using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Adapters
{
    public sealed class CalcTaskDataAdapter : EntityDataAdapter<CS_ES.ICalcResult, CalcTaskModel>
    {
        private readonly CalcServerDataLayer _dataLayer;
        public CalcTaskDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
            _dataLayer = dataLayer;
        }
        public long ContextId;
        protected override void PrepareQuery(IReadQuery<CS_ES.ICalcResult> query)
        {
            query.Select(
                c => c.Id,
                c => c.TASK.Id,
                c => c.TASK.TypeName,
                c => c.TASK.StatusCode,
                c => c.TASK.StatusName,
                c => c.TASK.StatusNote,
                c => c.TASK.MapName,
                c => c.TASK.CreatedDate,
                c => c.TASK.OwnerInstance,
                c => c.TASK.Note,
                c => c.StartTime,
                c => c.FinishTime,
                c => c.StatusCode,
                c => c.StatusName,
                c => c.StatusNote
                )
            .Filter(f => f.TASK.CONTEXT.Id, ContextId)
            .OrderByDesc(o => o.Id);
        }
        protected override CalcTaskModel ReadData(IDataReader<CS_ES.ICalcResult> reader, int index)
        {
            var taskId = reader.GetValue(c => c.TASK.Id);
            var task = new CalcTaskModel
            {
                Id = taskId,
                TypeName = reader.GetValue(c => c.TASK.TypeName),
                StatusCode = reader.GetValue(c => c.TASK.StatusCode),
                StatusName = reader.GetValue(c => c.TASK.StatusName),
                StatusNote = reader.GetValue(c => c.TASK.StatusNote),
                MapName = reader.GetValue(c => c.TASK.MapName),
                CreatedDate = reader.GetValue(c => c.TASK.CreatedDate),
                OwnerInstance = reader.GetValue(c => c.TASK.OwnerInstance),
                Note = reader.GetValue(c => c.TASK.Note),
                StartTime = reader.GetValue(c => c.StartTime),
                FinishTime = reader.GetValue(c => c.FinishTime),
                ResultId = reader.GetValue(c => c.Id),
                ResultStatusCode = reader.GetValue(c => c.StatusCode),
                ResultStatusName = reader.GetValue(c => c.StatusName),
                ResultStatusNote = reader.GetValue(c => c.StatusNote)
            };

            var queryArgs = _dataLayer.GetBuilder<CS_ES.Tasks.IStationCalibrationArgs>()
                .Read()
                .Select(c => c.Standard, c => c.Method)
                .Filter(c => c.TASK.Id, taskId);

            var readerArgs = _dataLayer.Executor.ExecuteReader(queryArgs);
            if (readerArgs.Read())
            {
                task.Standard = readerArgs.GetValue(c => c.Standard);
                var mt = readerArgs.GetValue(c => c.Method);
                if (mt.HasValue)
                    task.Method = mt.Value == 0 ? "Exhaustive research" : "Quick descent";
            }

            int count = 0;
            var queryResults = _dataLayer.GetBuilder<CS_ES.Tasks.IStationCalibrationResult>()
               .Read()
               .Select(c => c.PercentComplete, c => c.NumberStation)
               .Filter(f => f.RESULT.TASK.Id, taskId)
               .OrderByAsc(c => c.Id);

            var readerResult = _dataLayer.Executor.ExecuteReader(queryResults);
            while (readerResult.Read())
            {
                task.NumberStation = readerResult.GetValue(c => c.NumberStation);
                task.NumberTimeStarted = ++count;
            }

            return task;
        }
    }
}
