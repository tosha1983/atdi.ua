using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers
{
    public class UpdateSensorHandler : SdrnPrimaryHandlerBase<Sensor>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;

        public UpdateSensorHandler(ILogger logger) 
            : base(DeviceBusMessages.UpdateSensorMessage.Name)
        {
            this._logger = logger;
        }

        public override void OnHandle(IReceivedMessage<Sensor> message)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.MessageProcessing, this))
            {
                try
                {

                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
                }
            }
        }
    }
}
