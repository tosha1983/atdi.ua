using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IAmqpMessage_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IAmqpMessage: IAmqpMessage_PK
    {

        byte StatusCode { get; set; }

        string StatusNote { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        int ThreadId { get; set; }

        DateTimeOffset ProcessedStartDate { get; set; }

        DateTimeOffset ProcessedFinishDate { get; set; }

        string PropExchange { get; set; }

        string PropRoutingKey { get; set; }

        string PropAppId { get; set; }

        string PropType { get; set; }

        long? PropTimestamp { get; set; }

        string PropMessageId { get; set; }

        string PropCorrelationId { get; set; }

        byte? PropDeliveryMode { get; set; }

        string PropDeliveryTag { get; set; }

        string PropConsumerTag { get; set; }

        string PropContentEncoding { get; set; }

        string PropContentType { get; set; }

        string HeaderCreated { get; set; }

        string HeaderSdrnServer { get; set; }

        string HeaderSensorName { get; set; }

        string HeaderSensorTechId { get; set; }

        string BodyContentType { get; set; }

        string BodyContentEncoding { get; set; }

        byte[] BodyContent { get; set; }
    }
}
