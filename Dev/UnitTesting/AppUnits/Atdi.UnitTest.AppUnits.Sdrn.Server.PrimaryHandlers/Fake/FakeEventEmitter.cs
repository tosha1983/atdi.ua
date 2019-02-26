using Atdi.Contracts.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers.Fake
{
    class FakeEventEmitter : IEventEmitter
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Guid Emit(IEvent @event, EventEmittingOptions options)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (string.IsNullOrEmpty(@event.Name))
            {
                throw new ArgumentNullException("@event.Name");
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return @event.Id;
        }

        void IEventEmitter.Emit(IEvent @event, EventEmittingOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
