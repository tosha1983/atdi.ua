using Atdi.DataModels.Api.EventSystem;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendMeasResultsDeviceBusEvent", SubscriberName = "SendMeasResultsSubscriber")]
    public class SendMeasResultsSubscriber : SubscriberBase<DM.MeasResults>
    {

        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IEventEmitter _eventEmitter;
        public SendMeasResultsSubscriber(IEventEmitter eventEmitter, ISdrnMessagePublisher messagePublisher, IMessagesSite messagesSite, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger) : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
        }


        public void GetMeasTaskSDRIdentifier(string ResultIds, string TaskId, string SensorName, string SensorTechId, out long SubTaskId, out long SubTaskStationId, out long SensorId, out long ResultId, out long TaskIdOut)
        {
            TaskIdOut = -1;
            SubTaskId = -1;
            SubTaskStationId = -1;
            SensorId = -1;
            ResultId = -1;
            long SensorIdTemp = -1;

            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

            if (TaskId != null)
            {
                TaskId = TaskId.Replace("||", "|");
                string[] word = TaskId.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if ((word != null) && (word.Length == 4))
                {
                    TaskIdOut = int.Parse(word[0]);
                    SubTaskId = int.Parse(word[1]);
                    SubTaskStationId = int.Parse(word[2]);
                    SensorId = int.Parse(word[3]);
                }
            }

            if (ResultIds != null)
            {
                ResultIds = ResultIds.Replace("||", "|");
                string[] word = ResultIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if ((word != null) && (word.Length == 5))
                {
                    SubTaskId = int.Parse(word[1]);
                    SubTaskStationId = int.Parse(word[2]);
                    SensorId = int.Parse(word[3]);
                    ResultId = int.Parse(word[4]);
                }
                else
                {
                    long SubTaskIdTemp = -1;
                    long SubTaskStationIdTemp = -1;
                    long taskId = -1;
                    if (long.TryParse(TaskId, out taskId))
                    {
                        var builderFromIMeasSubTaskSta = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>().From();
                        builderFromIMeasSubTaskSta.Select(c => c.Id, c => c.MEASSUBTASK.Id, c => c.SensorId, c => c.MEASSUBTASK.TimeStart);
                        builderFromIMeasSubTaskSta.Where(c => c.MEASSUBTASK.MEASTASK.Id, ConditionOperator.Equal, taskId);
                        builderFromIMeasSubTaskSta.OrderByDesc(c => c.MEASSUBTASK.TimeStart);
                        queryExecuter.Fetch(builderFromIMeasSubTaskSta, reader =>
                        {
                            while (reader.Read())
                            {
                                SubTaskIdTemp = reader.GetValue(c => c.MEASSUBTASK.Id);
                                SubTaskStationIdTemp = reader.GetValue(c => c.Id);
                                if (reader.GetValue(c => c.SensorId).HasValue)
                                {
                                    SensorIdTemp = reader.GetValue(c => c.SensorId).Value;
                                }
                                break;
                            }
                            return true;
                        });

                        SubTaskId = SubTaskIdTemp;
                        SubTaskStationId = SubTaskStationIdTemp;
                        SensorId = SensorIdTemp;
                    }
                }
            }

            if (SensorId == -1)
            {
                var builderISensor = this._dataLayer.GetBuilder<MD.ISensor>().From();
                builderISensor.Select(c => c.Id, c => c.Name, c => c.TechId);
                builderISensor.Where(c => c.Name, ConditionOperator.Equal, SensorName);
                builderISensor.Where(c => c.TechId, ConditionOperator.Equal, SensorTechId);
                queryExecuter.Fetch(builderISensor, reader =>
                {
                    while (reader.Read())
                    {
                        SensorIdTemp = reader.GetValue(c => c.Id);
                    }
                    return true;
                });
                SensorId = SensorIdTemp;
            }
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.MeasResults deliveryObject)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                var  status = SdrnMessageHandlingStatus.Unprocessed;
                bool isSuccessProcessed=false;
                var reasonFailure = "";
                try
                {
                    // здесь код с обработчика OnReceivedNewSOResult
                    isSuccessProcessed = true;
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    status = SdrnMessageHandlingStatus.Error;
                    reasonFailure = e.StackTrace;
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    var deviceCommandResult = new DeviceCommand
                    {
                        EquipmentTechId = sensorTechId,
                        SensorName = sensorName,
                        SdrnServer = this._environment.ServerInstance,
                        Command = "SendMeasResultsConfirmed",
                        CommandId = "SendCommand",
                        CustTxt1 = "Success"
                    };

                    if (status == SdrnMessageHandlingStatus.Error)
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"" + reasonFailure + "\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    else if (isSuccessProcessed)
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Success\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    else
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"" + reasonFailure + "\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DeviceCommand>();
                    envelop.SensorName = sensorName;
                    envelop.SensorTechId = sensorTechId;
                    envelop.DeliveryObject = deviceCommandResult;
                    _messagePublisher.Send(envelop);
                }

            }
        }
    }
}
