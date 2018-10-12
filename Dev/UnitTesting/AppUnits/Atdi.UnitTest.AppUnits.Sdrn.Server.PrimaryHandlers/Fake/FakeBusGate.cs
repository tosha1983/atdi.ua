using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers.Fake
{
    class FakeBusGate : IBusGate
    {
        class FakeMessagePublisher : IMessagePublisher
        {
            class FakeMessageToken : IMessageToken
            {
                public FakeMessageToken(string id, string type)
                {
                    this.Id = id;
                    this.Type = type;
                }

                public string Id { get; }

                public string Type { get; }
            }

            public FakeMessagePublisher(string tag)
            {
                this.Tag = tag;
            }

            public string Tag { get;  }

            public void Dispose()
            {
            }

            public IMessageToken Send<TObject>(string messageType, TObject messageObject, string correlationToken = null)
            {
                return new FakeMessageToken(Guid.NewGuid().ToString(), messageType);
            }
        }

        class FakeMessageDispatcher : IMessageDispatcher
        {
            public FakeMessageDispatcher(string tag)
            {
                this.Tag = tag;
                this.State = MessageDispatcherState.Deactivated;
            }

            public MessageDispatcherState State { get; private set; }

            public string Tag { get; }

            public void Activate()
            {
                this.State = MessageDispatcherState.Activated;
            }

            public void Deactivate()
            {
                this.State = MessageDispatcherState.Deactivated;
            }

            public void Dispose()
            {
            }

            public void RegistryHandler<TObject>(string messageType, Action<IReceivedMessage<TObject>> handler)
            {
            }

            public void RegistryHandler<TObject>(IMessageHandler<TObject> handler)
            {
            }
        }

        public string Tag => "Fake gate";

        public IBusGateConfig Config { get; }

        public IMessageDispatcher CreateDispatcher(string dispatcherTag, IBusEventObserver eventObserver = null)
        {
            return new FakeMessageDispatcher(dispatcherTag);
        }

        public IMessagePublisher CreatePublisher(string publisherTag, IBusEventObserver eventObserver = null)
        {
           return new FakeMessagePublisher(publisherTag);
        }

        public void Dispose()
        {
            
        }
    }
}
