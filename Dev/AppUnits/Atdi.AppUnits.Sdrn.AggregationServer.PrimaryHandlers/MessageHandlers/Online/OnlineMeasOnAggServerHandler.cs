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
using DM = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Sdrn.Server;


namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.MessageHandlers
{
    public sealed class OnlineMeasOnAggServerHandler : IMessageHandler<OnlineMeasToAggregationServer, OnlineMeasurementPipebox>
    {
        private readonly IPublisher publisher;
        private readonly ILogger _logger;
        private readonly IPipelineSite _pipelineSite;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public OnlineMeasOnAggServerHandler(IPublisher publisher, IPipelineSite pipelineSite, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
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
        public void Handle(IIncomingEnvelope<OnlineMeasToAggregationServer, OnlineMeasurementPipebox> envelope, IHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnlineMeasOnAggServerHandler, this))
            {
                var deliveryObject = envelope.DeliveryObject;
                try
                {

                    var initOnlineMeasurementPipebox = new SdrnsServer.InitOnlineMeasurementPipebox();
                    initOnlineMeasurementPipebox.OnlineMeasMasterId = deliveryObject.OnlineMeasId;
                    initOnlineMeasurementPipebox.ServerToken = deliveryObject.ServerToken;
                    initOnlineMeasurementPipebox.Period = deliveryObject.Period;

                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        scope.BeginTran();
                        var sensorQuery = _dataLayer.GetBuilder<DM.ISensor>()
                              .From()
                              .Select(c => c.Id)
                              .Where(c => c.Name, ConditionOperator.Equal, deliveryObject.SensorName)
                              .Where(c => c.TechId, ConditionOperator.Equal, deliveryObject.SensorTechId);

                        var sensorExists = scope.Executor.ExecuteAndFetch(sensorQuery, reader =>
                        {
                            var exists = reader.Read();
                            if (exists)
                            {
                                initOnlineMeasurementPipebox.SensorId = reader.GetValue(c => c.Id);
                            }
                            return exists;
                        });
                        scope.Commit();
                    }

                    var site = this._pipelineSite.GetByName<InitOnlineMeasurementPipebox, SdrnsServer.InitOnlineMeasurementPipebox>(SdrnsServer.Pipelines.ClientInitOnlineMeasurement);
                    var resultCreateMeasTask = site.Execute(initOnlineMeasurementPipebox);

                }
                catch (Exception e)
                {
                    _logger.Exception(Contexts.ThisComponent, (EventCategory)"OnInitOnlineMeasurement", e, this);
                    throw;
                }
                result.Status = MessageHandlingStatus.Confirmed;
            }
        }
        
    }

  
}
