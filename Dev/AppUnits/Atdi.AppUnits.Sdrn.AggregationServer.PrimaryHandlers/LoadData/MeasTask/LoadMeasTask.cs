using Atdi.Contracts.Api.DataBus;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Sdrn.Server;
using MD = Atdi.DataModels.Sdrns.Server.Entities;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers
{
    /// <summary>
    /// Загрузка из БД AggregationServer сведений о задачах
    /// </summary>
    public sealed class LoadMeasTask
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public LoadMeasTask(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public long GetAggregationMeasTaskId(long masterSubTaskId)
        {
            long aggregationTaskId = -1;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var builderAggregationSensor = this._dataLayer.GetBuilder<MD.ILinkSubTaskSensorMasterId>().From();
            builderAggregationSensor.Select(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
            builderAggregationSensor.Select(c => c.Id);
            builderAggregationSensor.Where(c => c.SubtaskSensorMasterId, ConditionOperator.Equal, masterSubTaskId);
            queryExecuter.Fetch(builderAggregationSensor, readerAggregationSensor =>
            {
                while (readerAggregationSensor.Read())
                {
                    aggregationTaskId = readerAggregationSensor.GetValue(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                    break;
                }
                return true;
            });
            return aggregationTaskId;
        }
    }
}
