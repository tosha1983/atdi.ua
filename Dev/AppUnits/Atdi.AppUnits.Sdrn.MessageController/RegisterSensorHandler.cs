using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.CoreServices.EntityOrm;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;

namespace Atdi.AppUnits.Sdrn.MessageController
{
    public class RegisterSensorFromDeviceHandler : SdrnPrimaryHandlerBase<Atdi.DataModels.Sdrns.Device.Sensor>
    {
        private readonly IBusGate _busGate;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public RegisterSensorFromDeviceHandler(IBusGate busGate, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, IEventEmitter eventEmitter, ILogger logger) : base("RegisterSensor")
        {
            this._busGate = busGate;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }

        public void Handle()
        {
            /*
            EnitityOrmDataLayer enitityOrmDataLayer = new EnitityOrmDataLayer(this._dataLayer, this._logger);
            var query = enitityOrmDataLayer.GetBuilder<MD.ISensor>()
                .From()
                       .Where(c => c.Name, ConditionOperator.Equal, "1")
                       .Where(c => c.TechId, ConditionOperator.Equal, "2")
                       .OnTop(1);

            var  sensorExistsInDb = enitityOrmDataLayer.Executor<SdrnServerDataContext>()
                .Execute(query) == 0;
                */
        }

        public override void OnHandle(ISdrnReceivedMessage<Atdi.DataModels.Sdrns.Device.Sensor> message)
        {
            
        }
    }
}
