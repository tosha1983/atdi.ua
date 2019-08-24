using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Contracts.Api.DataBus;
using Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers;
using Atdi.DataModels.Sdrns.Server;



namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers
{
    public class RegistrationAggregationServer : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IPublisher _publisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private Thread _processingThread;

        public RegistrationAggregationServer(IPublisher publisher, IDataLayer<EntityDataOrm> dataLayer, ILogger logger, ISdrnServerEnvironment environment)
        {
            this._logger = logger;
            this._environment = environment;
            this._publisher = publisher;
            this._dataLayer = dataLayer;
        }

        public void Run()
        {
            this._processingThread = new Thread(this.Process)
            {
                Name = $"RegistrationAggregationServerOnMasterServer"
            };

            this._processingThread.Start();
        }

        private void Process()
        {
            try
            {
                using (this._logger.StartTrace(Contexts.ThisComponent, Categories.RegistrationAggregationServer, this))
                {
                    var loadSensor = new LoadSensor(this._dataLayer, this._logger);
                    var saveSensor = new SaveSensor(this._dataLayer, this._logger);
                    var sensors = loadSensor.LoadAllSensors();
                    if (sensors != null)
                    {
                        for (int i = 0; i < sensors.Length; i++)
                        {
                            saveSensor.UpdateFieldAggregationServerInstanceInSensor(sensors[i].Name, sensors[i].Equipment.TechId, this._environment.ServerInstance);
                            var retEnvelope = this._publisher.CreateEnvelope<SendSensorFromAggregationToMasterServer, Sensor>();
                            retEnvelope.To = this._environment.MasterServerInstance;
                            retEnvelope.DeliveryObject = sensors[i];
                            retEnvelope.DeliveryObject.AggregationServerInstance = this._environment.ServerInstance;
                            this._publisher.Send(retEnvelope);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.StartTrace(Contexts.ThisComponent, Categories.RegistrationAggregationServer, e);
            }
        }

      
       

        public void Dispose()
        {

        }
    }
}
