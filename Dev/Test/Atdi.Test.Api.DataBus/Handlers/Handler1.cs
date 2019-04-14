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
    class Handler1_1 : IMessageHandler<Messages.CommonMessageType1, DeliveryObjects.Address1DeliveryObject>
    {
        public void Handle(IIncomingEnvelope<CommonMessageType1, Address1DeliveryObject> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Testing Data Bus: Handler1_1, CommonMessage1";
            result.Detail = "Detail Handler1_1";
        }
    }

    class Handler1_2 : IMessageHandler<Messages.CommonMessageType2, DeliveryObjects.Address1DeliveryObject>
    {
        public void Handle(IIncomingEnvelope<CommonMessageType2, Address1DeliveryObject> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Testing Data Bus: Handler1_2, CommonMessage2";
            result.Detail = "Detail Handler1_2";
        }
    }

    class Handler1_3 : IMessageHandler<Messages.CommonMessageType3, DeliveryObjects.Address1DeliveryObject>
    {
        public void Handle(IIncomingEnvelope<CommonMessageType3, Address1DeliveryObject> envelope, IHandlingResult result)
        {
            result.Status = MessageHandlingStatus.Rejected;
            result.Reason = "Testing Data Bus: Handler1_3, CommonMessage3";
            result.Detail = "Detail Handler1_3";
        }
    }
}
