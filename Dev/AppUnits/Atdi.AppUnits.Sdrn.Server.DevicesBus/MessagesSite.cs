using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.DevicesBus
{
    public class MessagesSite : IMessagesSite
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IQueryBuilder<IAmqpMessage> _amqpMessageQueryBuilder;
        private readonly IQueryBuilder<IAmqpEvent> _amqpEventQueryBuilder;


        public MessagesSite(IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._logger = logger;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();
            this._amqpMessageQueryBuilder = this._dataLayer.GetBuilder<IAmqpMessage>();
            this._amqpEventQueryBuilder = this._dataLayer.GetBuilder<IAmqpEvent>();
        }

        public IMessageProcessingScope<TDeliveryObject> StartProcessing<TDeliveryObject>(long messageId)
        {
            return new MessageProcessingScope<TDeliveryObject>(messageId, _dataLayer, _environment, _logger);
        }

        public void ChangeStatus(long messageId, byte oldCode, byte newCode, string statusNote)
        {
            try
            {
                var updateQuery = this._amqpMessageQueryBuilder
                .Update()
                .SetValue(c => c.StatusCode, newCode)
                .SetValue(c => c.StatusName, ((MessageProcessingStatus)newCode).ToString())
                .SetValue(c => c.StatusNote, statusNote)
                .Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, messageId)
                // важно: следующее услови это блокиратор от ситуации когда 
                // евент дошел и начал обрабатывать на прикладном уровне сообщение
                .Where(c => c.StatusCode, DataModels.DataConstraint.ConditionOperator.Equal, oldCode);

                this._queryExecutor.Execute(updateQuery);

                // для статуса 0 нужно подчистить собітия
                if (oldCode == (byte)MessageProcessingStatus.Created)
                {
                    var deleteQuery = this._amqpEventQueryBuilder
                        .Delete()
                        .Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, messageId);

                    this._queryExecutor.Execute(deleteQuery);
                }
            }
            catch (Exception e)
            {
                var error = $"An error occurred while changing status from {oldCode} to {newCode} for the message with id #{messageId}";
                _logger.Exception(Contexts.ThisComponent, Categories.Processing, error, e, (object)this);
                throw new InvalidOperationException(error, e);
            }
        }

        public ValueTuple<long, string>[] GetMessagesForNotification()
        {
            try
            {
                var query = this._amqpEventQueryBuilder
                .From()
                .Select(
                    c => c.Id,
                    c => c.PropType
                    )
                .OrderByAsc(c => c.Id);

                var result = new List<ValueTuple<long, string>>();

                this._queryExecutor.Fetch(query, reader =>
                {
                    while(reader.Read())
                    {
                        var record = new ValueTuple<long, string>(reader.GetValue(c => c.Id), reader.GetValue(c => c.PropType));
                        result.Add(record);
                    }
                    return true;
                });

                return result.ToArray();
            }
            catch (Exception e)
            {
                var error = $"An error occurred while retrieving database records of AMQP Messages for notificstion";
                _logger.Exception(Contexts.ThisComponent, Categories.Processing, error, e, (object)this);
                throw new InvalidOperationException(error, e);
            }
        }
    }
}
