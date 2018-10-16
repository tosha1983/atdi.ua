using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.Sdrn.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers.Fake
{
    class FakeSdrnReceivedMessage<TObject> //: ISdrnReceivedMessage<TObject>
    {
        class FakeMessageToken : IMessageToken
        {
            public string Id { get; set; }

            public string Type { get; set; }
        }

        public FakeSdrnReceivedMessage(string id, string type)
        {
            this.Token = new FakeMessageToken()
            {
                Id = id,
                Type = type
            };
        }

        public string DeviceSensorName { get; set; }

        public string DeviceSensorTechId { get; set; }

        public IMessageToken Token { get; set; }

        public string CorrelationToken { get; set; }

        public DateTime Created { get; set; }

        public TObject Data { get; set; }

        public MessageHandlingResult Result { get; set; }

        public string ReasonFailure { get; set; }
    }
}
