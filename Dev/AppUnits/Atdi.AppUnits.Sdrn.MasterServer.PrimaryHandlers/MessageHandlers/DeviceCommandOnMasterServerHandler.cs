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
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Contracts.Sdrn.Server;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;
using System.Globalization;
using Atdi.Common;


namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.MessageHandlers
{
    public sealed class DeviceCommandOnMasterServerHandler : IMessageHandler<SendDeviceCommandFromAggregationToMasterServer, DeviceCommandResultEvent>
    {
        private readonly IPublisher publisher;
        private readonly ILogger _logger;
        private readonly IPipelineSite _pipelineSite;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;


        public DeviceCommandOnMasterServerHandler(IPublisher publisher, IPipelineSite pipelineSite, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this.publisher = publisher;
            this._logger = logger;
            this._pipelineSite = pipelineSite;
            this._dataLayer = dataLayer;
        }

        /// <summary>
        /// Получение сообщения со стороны AggregationServer о необходимости запуска процедуры регистрации/обновления сенсора в БД MasterServer
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="result"></param>
        public void Handle(IIncomingEnvelope<SendDeviceCommandFromAggregationToMasterServer, DeviceCommandResultEvent> envelope, IHandlingResult result)
        {
            using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
            {
                scope.BeginTran();
                var deliveryObject = envelope.DeliveryObject;

                switch (deliveryObject.CommandId)
                {
                    case "SendActivitySensor":
                        var query = this._dataLayer.GetBuilder<MD.ISensor>()
                   .From()
                   .Select(c => c.Name)
                   .Select(c => c.Id)
                   .Where(c => c.Name, ConditionOperator.Equal, deliveryObject.SensorName)
                   .Where(c => c.TechId, ConditionOperator.Equal, deliveryObject.SensorTechId)
                   .OrderByAsc(c => c.Id);
                        var sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
                         .Execute(query) == 1;

                        if (sensorExistsInDb == true)
                        {
                            var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensor>().Update();
                            builderUpdateSensor.Where(c => c.TechId, ConditionOperator.Equal, deliveryObject.SensorTechId);
                            builderUpdateSensor.Where(c => c.Name, ConditionOperator.Equal, deliveryObject.SensorName);
                            builderUpdateSensor.SetValue(c => c.Status, "A");
                            builderUpdateSensor.SetValue(c => c.LastActivity, DateTime.Now);
                            var cnt = scope.Executor
                             .Execute(builderUpdateSensor);
                            if (cnt == 1)
                            {
                                scope.Commit();
                            }
                        }
                        break;
                    case "UpdateStatusMeasTask":
                        long subTaskStationId = -1;
                        long taskIds = -1;
                        if (deliveryObject.CustTxt1 != null)
                        {
                            var subTaskId = deliveryObject.CustTxt1.ConvertStringToDouble();
                            if (subTaskId != null)
                            {
                                subTaskStationId = (long)subTaskId.Value;
                                if (subTaskStationId > 0)
                                {
                                    var querySubTaskSensor = this._dataLayer.GetBuilder<MD.ISubTaskSensor>()
                                        .Update()
                                        .Where(c => c.Id, ConditionOperator.Equal, subTaskStationId)
                                        .SetValue(c => c.Status, deliveryObject.Status);
                                    var updated = scope.Executor
                                    .Execute(querySubTaskSensor) == 1;


                                    var queryMeasTaskSelect = this._dataLayer.GetBuilder<MD.ISubTaskSensor>()
                                  .From()
                                  .Select(c => c.Id)
                                  .Select(c => c.Status)
                                  .Select(c => c.SUBTASK.MEAS_TASK.Id)
                                  .Where(c => c.Id, ConditionOperator.Equal, subTaskStationId);
                                    scope.Executor
                                   .Fetch(queryMeasTaskSelect, reader =>
                                   {
                                       while (reader.Read())
                                       {
                                           taskIds = reader.GetValue(c => c.SUBTASK.MEAS_TASK.Id);
                                           break;
                                       }
                                       return true;
                                   });


                                    int cntCompleteTask = 0;
                                    int cntAllTask = 0;
                                    var querySubTaskSensorSelect = this._dataLayer.GetBuilder<MD.ISubTaskSensor>()
                                   .From()
                                   .Select(c => c.Id)
                                   .Select(c => c.Status)
                                   .Select(c => c.SUBTASK.MEAS_TASK.Id)
                                   .Where(c => c.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, taskIds);
                                    scope.Executor
                                   .Fetch(querySubTaskSensorSelect, reader =>
                                   {
                                       while (reader.Read())
                                       {
                                           if (reader.GetValue(c => c.Status) == "C")
                                           {
                                               cntCompleteTask++;
                                           }
                                           cntAllTask++;
                                       }
                                       return true;
                                   });

                                    if (cntAllTask == cntCompleteTask)
                                    {
                                        var queryMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>()
                                        .Update()
                                        .Where(c => c.Id, ConditionOperator.Equal, taskIds)
                                        .SetValue(c => c.Status, deliveryObject.Status);
                                        var updatedMeasTask = scope.Executor
                                        .Execute(queryMeasTask) == 1;
                                    }
                                    scope.Commit();
                                }
                            }
                        }
                        break;
                    case "UpdateSensorLocation":
                        long? Id = -1;
                        var queryUpdateSensorLocation = this._dataLayer.GetBuilder<MD.ISensor>()
                        .From()
                        .Select(c => c.Name)
                        .Select(c => c.Id)
                        .Where(c => c.Name, ConditionOperator.Equal, deliveryObject.SensorName)
                        .Where(c => c.TechId, ConditionOperator.Equal, deliveryObject.SensorTechId)
                        .OrderByAsc(c => c.Id);
                        scope.Executor
                       .Fetch(queryUpdateSensorLocation, reader =>
                       {
                           var res = reader.Read();
                           if (res)
                           {
                               Id = reader.GetValue(c => c.Id);
                           }
                           return res;
                       });


                        if (Id > 0)
                        {
                            var values = deliveryObject.CustTxt1.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            if ((values != null) && (values.Length > 0))
                            {
                                if (values.Length == 3)
                                {

                                    var lon = values[0].ConvertStringToDouble();
                                    var lat = values[1].ConvertStringToDouble();
                                    var asl = values[2].ConvertStringToDouble();

                                    if ((lon != null) && (lat != null) && (asl != null))
                                    {
                                        var queryCheck = this._dataLayer.GetBuilder<MD.ISensorLocation>()
                                       .From()
                                       .Select(c => c.Id)
                                       .Where(c => c.Lon, ConditionOperator.Equal, lon)
                                       .Where(c => c.Lat, ConditionOperator.Equal, lat)
                                       .Where(c => c.Asl, ConditionOperator.Equal, asl);
                                        var cnt = scope.Executor.Execute(queryCheck);
                                        if (cnt == 0)
                                        {
                                            var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensorLocation>().Update();
                                            builderUpdateSensor.Where(c => c.SENSOR.Id, ConditionOperator.Equal, Id);
                                            builderUpdateSensor.Where(c => c.Status, ConditionOperator.NotEqual, "Z");
                                            builderUpdateSensor.SetValue(c => c.Status, "Z");
                                            scope.Executor
                                             .Execute(builderUpdateSensor);

                                            var builderInsertSensor = this._dataLayer.GetBuilder<MD.ISensorLocation>().Insert();
                                            builderInsertSensor.SetValue(c => c.SENSOR.Id, Id);
                                            builderInsertSensor.SetValue(c => c.Lon, lon);
                                            builderInsertSensor.SetValue(c => c.Lat, lat);
                                            builderInsertSensor.SetValue(c => c.Asl, asl);
                                            builderInsertSensor.SetValue(c => c.DateCreated, DateTime.Now);
                                            builderInsertSensor.SetValue(c => c.DateFrom, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1));
                                            builderInsertSensor.SetValue(c => c.DateTo, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));
                                            builderInsertSensor.SetValue(c => c.Status, "A");

                                            scope.Executor
                                            .Execute(builderInsertSensor);

                                        }
                                    }
                                }
                            }
                            scope.Commit();
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Handle for CommandId {deliveryObject.CommandId} not implemented");
                }
            }
            result.Status = MessageHandlingStatus.Confirmed;
        }
    }
  
}
