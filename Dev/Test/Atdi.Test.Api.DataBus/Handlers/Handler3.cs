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
    class Handler3_1 : IMessageHandler<Messages.SpecificMessageType1, DeliveryObjects.Address3DeliveryObject>
    {
        public void Handle(IIncomingEnvelope<SpecificMessageType1, Address3DeliveryObject> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Testing Data Bus: Handler3_1, SpecificMessage1";
            result.Detail = "Detail Handler3_1";
        }
    }

    class Handler3_2 : IMessageHandler<Messages.SpecificMessageType2, DeliveryObjects.Address3DeliveryObject>
    {
        public void Handle(IIncomingEnvelope<SpecificMessageType2, Address3DeliveryObject> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Testing Data Bus: Handler3_2, SpecificMessage2";
            result.Detail = "Detail Handler3_2";
        }
    }

    class Handler3_3 : IMessageHandler<Messages.SpecificMessageType3, DeliveryObjects.Address3DeliveryObject>
    {
        public void Handle(IIncomingEnvelope<SpecificMessageType3, Address3DeliveryObject> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Testing Data Bus: Handler3_3, SpecificMessage3";
            result.Detail = "Detail Handler3_3";
        }
    }
}
