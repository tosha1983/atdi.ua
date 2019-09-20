using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using System;
using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers;


namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.PipelineHandlers
{
    public class CommandsOnAggServerPipelineHandler : IPipelineHandler<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public CommandsOnAggServerPipelineHandler(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        /// <summary>
        /// Конвеерный обработчик команд, выполняющий замену идентификаторов, полученных со сотороны MasterServer на идентификаторы
        /// соответсвующие идентификаторам AggregationServer
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ClientMeasTaskPiperesult Handle(ClientMeasTaskPipebox data, IPipelineContext<ClientMeasTaskPipebox, ClientMeasTaskPiperesult> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.CommandsOnAggServerPipelineHandler, this))
            {
                var dic = new Dictionary<long?, long?>();
                var loadSensor = new LoadSensor(this._dataLayer, this._logger);
                var loadMeasTask = new LoadMeasTask(this._dataLayer, this._logger);
                var listNotFoundSensors = new List<long>();
                var listNotFoundSensorsTxt = new List<string>();
                for (int i = 0; i < data.MeasTaskPipeBox.Sensors.Length; i++)
                {
                    var val = data.MeasTaskPipeBox.Sensors[i];
                    var aggregationSensor = loadSensor.LoadObjectSensor(val.SensorName, val.SensorTechId);
                    if (aggregationSensor != null)
                    {
                        dic.Add(val.SensorId.Value, aggregationSensor.Id.Value);
                        val.SensorId.Value = aggregationSensor.Id.Value;
                    }
                    else
                    {
                        listNotFoundSensors.Add(val.SensorId.Value);
                        listNotFoundSensorsTxt.Add($"Name: {val.SensorName}  TechId: {val.SensorTechId}");
                    }
                }

                var allListSensors = data.MeasTaskPipeBox.Sensors.ToList();
                for (int i = 0; i < listNotFoundSensors.Count; i++)
                {
                    allListSensors.RemoveAll(x => x.SensorId.Value == listNotFoundSensors[i]);
                }
                data.MeasTaskPipeBox.Sensors = allListSensors.ToArray();

                if (listNotFoundSensors.Count > 0)
                {
                    var measSubTasksForDel = data.MeasTaskPipeBox.MeasSubTasks;
                    if ((measSubTasksForDel != null) && (measSubTasksForDel.Length > 0))
                    {
                        for (int i = 0; i < measSubTasksForDel.Length; i++)
                        {
                            var listMeasSubTasks = measSubTasksForDel[i];
                            if ((listMeasSubTasks.MeasSubTaskSensors != null) && (listMeasSubTasks.MeasSubTaskSensors.Length > 0))
                            {
                                var listMeasSubTaskSensorsForDel = listMeasSubTasks.MeasSubTaskSensors.ToList();
                                for (int j = 0; j < listNotFoundSensors.Count; j++)
                                {
                                    listMeasSubTaskSensorsForDel.RemoveAll(x => x.SensorId == listNotFoundSensors[i]);
                                }
                                listMeasSubTasks.MeasSubTaskSensors = listMeasSubTaskSensorsForDel.ToArray();
                            }
                        }
                    }
                }


                var measSubTasks = data.MeasTaskPipeBox.MeasSubTasks;
                if ((measSubTasks != null) && (measSubTasks.Length > 0))
                {
                    for (int i = 0; i < measSubTasks.Length; i++)
                    {
                        var measSubTask = measSubTasks[i];
                        if ((measSubTask.MeasSubTaskSensors != null) && (measSubTask.MeasSubTaskSensors.Length > 0))
                        {
                            for (int j = 0; j < measSubTask.MeasSubTaskSensors.Length; j++)
                            {
                                var MeasSubTaskSensor = measSubTask.MeasSubTaskSensors[j];
                                var dicSensors = dic.ToList();
                                var findaggregationSensorId = dicSensors.Find(x => x.Key == MeasSubTaskSensor.SensorId);
                                if (findaggregationSensorId.Value != null)
                                {
                                    MeasSubTaskSensor.SensorId = findaggregationSensorId.Value.Value;
                                }
                                if (MeasSubTaskSensor.MasterId != null)
                                {
                                    var arrgregationTaskId = loadMeasTask.GetAggregationMeasTaskId(MeasSubTaskSensor.MasterId.Value);
                                    if (arrgregationTaskId != -1)
                                    {
                                        data.MeasTaskPipeBox.Id.Value = arrgregationTaskId;
                                    }
                                }
                            }
                        }
                    }
                }

                if (data.MeasTaskPipeBox is MeasTaskSignaling)
                {
                    var measTaskSignaling = data.MeasTaskPipeBox as MeasTaskSignaling;
                    if (measTaskSignaling != null)
                    {
                        if ((measTaskSignaling.RefSituation != null) && (measTaskSignaling.RefSituation.Length > 0))
                        {
                            for (int i = 0; i < measTaskSignaling.RefSituation.Length; i++)
                            {
                                var dicSensors = dic.ToList();
                                var findaggregationSensorId = dicSensors.Find(x => x.Key == measTaskSignaling.RefSituation[i].SensorId);
                                if (findaggregationSensorId.Value != null)
                                {
                                    measTaskSignaling.RefSituation[i].SensorId = findaggregationSensorId.Value.Value;
                                }
                            }
                        }
                    }
                }


                if (listNotFoundSensorsTxt.Count > 0)
                {
                    string allSensorNotFoundInAggregationServer = string.Join(";", listNotFoundSensorsTxt);
                    this._logger.Warning(Contexts.ThisComponent, Categories.ClientMeasTasksPipelineHandler, $"Sensor's '{listNotFoundSensorsTxt}' not found in AggregationServer");
                }
                if (allListSensors.Count == 0)
                {
                    this._logger.Exception(Contexts.ThisComponent, Categories.ClientMeasTasksPipelineHandler, new Exception(Events.TaskNotContainSensors.ToString()));
                    throw new Exception(Events.TaskNotContainSensors.ToString());
                }
                return context.GoAhead(data);
            }
        }
    }
}
