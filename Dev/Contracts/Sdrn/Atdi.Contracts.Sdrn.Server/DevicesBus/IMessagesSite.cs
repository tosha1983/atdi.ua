using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server.DevicesBus
{
    public interface IMessagesSite
    {
        IMessageProcessingScope<TDeliveryObject> StartProcessing<TDeliveryObject>(long messageId);

        void ChangeStatus(long messageId, byte oldCode, byte newCode, string statusNote);

        ValueTuple<long, string>[] GetMessagesForNotification();
    }
}
