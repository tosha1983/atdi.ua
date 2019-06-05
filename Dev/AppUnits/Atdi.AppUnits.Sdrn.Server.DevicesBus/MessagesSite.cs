using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.DevicesBus
{
    class MessagesSite : IMessagesSite
    {
        private readonly IDataLayer<EntityDataOrm> dataLayer;
        private readonly ILogger logger;

        public MessagesSite(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this.dataLayer = dataLayer;
            this.logger = logger;
        }

        public IMessageProcessingScope<TDeliveryObject> StartProcessing<TDeliveryObject>(long messageId)
        {
            return new MessageProcessingScope<TDeliveryObject>(messageId, dataLayer, logger);
        }
    }
}
