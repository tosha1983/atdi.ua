using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class ReceivedMessage<TObject> : IReceivedMessage<TObject>, IMessageResult
    {
        public ReceivedMessage(IMessageToken token, TObject data, DateTime created, string correlationToken)
        {
            this.Token = token;
            this.Data = data;
            this.Created = created;
            this.CorrelationToken = correlationToken;
            this.Result = MessageHandlingResult.Received;
        }

        public IMessageToken Token { get; }

        public string CorrelationToken { get; }

        public DateTime Created { get; }

        public TObject Data { get; }

        public MessageHandlingResult Result { get; set; }

        public string ReasonFailure { get; set; }
    }
}
