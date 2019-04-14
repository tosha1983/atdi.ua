using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.Test.Api.DataBus.DeliveryObjects;
using Atdi.Test.Api.DataBus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.DataBus.Handlers
{
    class Handler2_1 : IMessageHandler<Messages.PrivateMessageType1, DeliveryObjects.Address2DeliveryObject>
    {
        public void Handle(IIncomingEnvelope<PrivateMessageType1, Address2DeliveryObject> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Testing Data Bus: Handler2_1, PrivateMessage1";
            result.Detail = "Detail Handler2_1";
        }
    }

    class Handler2_2 : IMessageHandler<Messages.PrivateMessageType2, DeliveryObjects.Address2DeliveryObject>
    {
        public void Handle(IIncomingEnvelope<PrivateMessageType2, Address2DeliveryObject> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Testing Data Bus: Handler2_2, PrivateMessage2";
            result.Detail = "Detail Handler2_2";
        }
    }

    class Handler2_3 : IMessageHandler<Messages.PrivateMessageType3, DeliveryObjects.Address2DeliveryObject>
    {
        public void Handle(IIncomingEnvelope<PrivateMessageType3, Address2DeliveryObject> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Testing Data Bus: Handler2_3, PrivateMessage3";
            result.Detail = "Detail Handler2_3";
        }
    }
}
