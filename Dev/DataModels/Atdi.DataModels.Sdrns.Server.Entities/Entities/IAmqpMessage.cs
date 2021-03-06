﻿using Atdi.DataModels.EntityOrm;
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

        string StatusName { get; set; }

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

        string HeaderApiVersion { get; set; }

        string HeaderProtocol { get; set; }

        string HeaderBodyAQName { get; set; }

    }

    public enum AmqpMessageStatusCode
	{
	    /// <summary>
	    /// Message was created in DB
	    /// </summary>
	    Created = 0,

	    /// <summary>
	    /// Sent event to  EventSystems
	    /// </summary>
	    SentEvent = 1,

	    /// <summary>
	    /// Message is processing
	    /// </summary>
	    Processing = 2,

	    /// <summary>
	    /// Message was processed
	    /// </summary>
	    Processed = 3,

		/// <summary>
		/// Abort by some exception
		/// </summary>
		Failure = 4
    }
}
