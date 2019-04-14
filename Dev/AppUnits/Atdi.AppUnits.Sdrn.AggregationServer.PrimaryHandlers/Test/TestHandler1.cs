using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.DataModels.Sdrns.Server.Test;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers
{
    public sealed class TestHandler1_1 : IMessageHandler<TestMessage1, TestDeliveryObject1>
    {
        private readonly IPublisher publisher;
        private readonly ILogger logger;

        public TestHandler1_1(IPublisher publisher, ILogger logger)
        {
            this.publisher = publisher;
            this.logger = logger;
        }

        public void Handle(IIncomingEnvelope<TestMessage1, TestDeliveryObject1> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Test of TestHandler1_1 with TestMessage1 and TestDeliveryObject1";
            result.Detail = $"Input delivery object: Index =  '{envelope.DeliveryObject.Index}', Data = '{envelope.DeliveryObject.Data}'";

            var retEnvelope = publisher.CreateEnvelope<TestMessage1, TestDeliveryObject2>();
            retEnvelope.To = "MasterServer";
            retEnvelope.DeliveryObject = new TestDeliveryObject2
            {
                Index = envelope.DeliveryObject.Index + 1,
                Data = envelope.DeliveryObject.Data + " -> " + "AggregationServer.TestHandler1_1"
            };
            publisher.Send(retEnvelope);
        }
    }

    public sealed class TestHandler1_2 : IMessageHandler<TestMessage1, TestDeliveryObject2>
    {
        private readonly IPublisher publisher;
        private readonly ILogger logger;

        public TestHandler1_2(IPublisher publisher, ILogger logger)
        {
            this.publisher = publisher;
            this.logger = logger;
        }

        public void Handle(IIncomingEnvelope<TestMessage1, TestDeliveryObject2> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Test of TestHandler1_2 with TestMessage1 and TestDeliveryObject2";
            result.Detail = $"Input delivery object: Index =  '{envelope.DeliveryObject.Index}', Data = '{envelope.DeliveryObject.Data}'";

            var retEnvelope = publisher.CreateEnvelope<TestMessage2, TestDeliveryObject3>();
            retEnvelope.To = "MasterServer";
            retEnvelope.DeliveryObject = new TestDeliveryObject3
            {
                Index = envelope.DeliveryObject.Index + 1,
                Data = envelope.DeliveryObject.Data + " -> " + "AggregationServer.TestHandler1_2"
            };
            publisher.Send(retEnvelope);
        }
    }

    public sealed class TestHandler2_1 : IMessageHandler<TestMessage2, TestDeliveryObject3>
    {
        private readonly IPublisher publisher;
        private readonly ILogger logger;

        public TestHandler2_1(IPublisher publisher, ILogger logger)
        {
            this.publisher = publisher;
            this.logger = logger;
        }

        public void Handle(IIncomingEnvelope<TestMessage2, TestDeliveryObject3> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Test of TestHandler2_1 with TestMessage2 and TestDeliveryObject3";
            result.Detail = $"Input delivery object: Index =  '{envelope.DeliveryObject.Index}', Data = '{envelope.DeliveryObject.Data}'";

            var retEnvelope = publisher.CreateEnvelope<TestMessage2, TestDeliveryObject4>();
            retEnvelope.To = "MasterServer";
            retEnvelope.DeliveryObject = new TestDeliveryObject4
            {
                Index = envelope.DeliveryObject.Index + 1,
                Data = envelope.DeliveryObject.Data + " -> " + "AggregationServer.TestHandler2_1"
            };
            publisher.Send(retEnvelope);
        }
    }

    public sealed class TestHandler2_2 : IMessageHandler<TestMessage2, TestDeliveryObject4>
    {
        private readonly IPublisher publisher;
        private readonly ILogger logger;

        public TestHandler2_2(IPublisher publisher, ILogger logger)
        {
            this.publisher = publisher;
            this.logger = logger;
        }
        public void Handle(IIncomingEnvelope<TestMessage2, TestDeliveryObject4> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Test of TestHandler2_2 with TestMessage2 and TestDeliveryObject4";
            result.Detail = $"Input delivery object: Index =  '{envelope.DeliveryObject.Index}', Data = '{envelope.DeliveryObject.Data}'";

            var retEnvelope = publisher.CreateEnvelope<TestMessage3, TestDeliveryObject5>();
            retEnvelope.To = "MasterServer";
            retEnvelope.DeliveryObject = new TestDeliveryObject5
            {
                Index = envelope.DeliveryObject.Index + 1,
                Data = envelope.DeliveryObject.Data + " -> " + "AggregationServer.TestHandler2_2"
            };
            publisher.Send(retEnvelope);
        }
    }

    public sealed class TestHandler3_1 : IMessageHandler<TestMessage3, TestDeliveryObject5>
    {
        private readonly IPublisher publisher;
        private readonly ILogger logger;

        public TestHandler3_1(IPublisher publisher, ILogger logger)
        {
            this.publisher = publisher;
            this.logger = logger;
        }
        public void Handle(IIncomingEnvelope<TestMessage3, TestDeliveryObject5> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Test of TestHandler3_1 with TestMessage3 and TestDeliveryObject5";
            result.Detail = $"Input delivery object: Index =  '{envelope.DeliveryObject.Index}', Data = '{envelope.DeliveryObject.Data}'";

            var retEnvelope = publisher.CreateEnvelope<TestMessage4, TestDeliveryObject6>();
            retEnvelope.To = "MasterServer";
            retEnvelope.DeliveryObject = new TestDeliveryObject6
            {
                Index = envelope.DeliveryObject.Index + 1,
                Data = envelope.DeliveryObject.Data + " -> " + "AggregationServer.TestHandler3_1"
            };
            publisher.Send(retEnvelope);
        }
    }

    public sealed class TestHandler4_1 : IMessageHandler<TestMessage4, TestDeliveryObject6>
    {
        private readonly IPublisher publisher;
        private readonly ILogger logger;

        public TestHandler4_1(IPublisher publisher, ILogger logger)
        {
            this.publisher = publisher;
            this.logger = logger;
        }
        public void Handle(IIncomingEnvelope<TestMessage4, TestDeliveryObject6> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Test of TestHandler4_1 with TestMessage4 and TestDeliveryObject6";
            result.Detail = $"Input delivery object: Index =  '{envelope.DeliveryObject.Index}', Data (Finished.TestHandler4_1) = '{envelope.DeliveryObject.Data}'";
        }
    }
}
