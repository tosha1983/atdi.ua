using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Workflows;
using SdrnsServer = Atdi.DataModels.Sdrns.Server;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers;
using Atdi.DataModels.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;


namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.MessageHandlers
{
    public sealed class MeasTaskCreateOnAggServerHandler : IMessageHandler<MeasTaskToAggregationServer, CreateMeasTaskPipebox>
    {
        private readonly IPublisher publisher;
        private readonly ILogger _logger;
        private readonly IPipelineSite _pipelineSite;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public MeasTaskCreateOnAggServerHandler(IDataLayer<EntityDataOrm> dataLayer, IPublisher publisher, IPipelineSite pipelineSite, ILogger logger)
        {
            this.publisher = publisher;
            this._logger = logger;
            this._pipelineSite = pipelineSite;
            this._dataLayer = dataLayer;
        }

        /// <summary>
        ///  Класс, выполняющий "прослушивание" и обработку сообщений  типа MeasTaskToAggregationServer,
        ///  которые отправляются со стороны MasterServer и несут информацию о таске, предназначенном для сохранения на данном AggregationServer (создание новой задачи)
        ///  Запуск конвеерной обработки полученных данных
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="result"></param>
        public void Handle(IIncomingEnvelope<MeasTaskToAggregationServer, CreateMeasTaskPipebox> envelope, IHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MeasTaskCreateOnAggServerHandler, this))
            {
                var data = envelope.DeliveryObject;
                var dic = new Dictionary<long?, long?>();
                var loadSensor = new LoadSensor(this._dataLayer, this._logger);
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
                var dicSensors = dic.ToList();

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
                                var findaggregationSensorId = dicSensors.Find(x => x.Key == MeasSubTaskSensor.SensorId);
                                if (findaggregationSensorId.Value != null)
                                {
                                    MeasSubTaskSensor.SensorId = findaggregationSensorId.Value.Value;
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



                var site = this._pipelineSite.GetByName<SdrnsServer.ClientMeasTaskPipebox, SdrnsServer.ClientMeasTaskPiperesult>(SdrnsServer.Pipelines.ClientMeasTasks);
                var resultCreateMeasTask = site.Execute(new SdrnsServer.ClientMeasTaskPipebox()
                {
                    MeasTaskPipeBox = data.MeasTaskPipeBox,
                    MeasTaskModePipeBox = data.MeasTaskModePipeBox
                });
                result.Status = MessageHandlingStatus.Confirmed;
            }
        }
    }

  
}
