using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Atdi.Contracts.WcfServices.Sdrn.Device;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;



namespace Atdi.WcfServices.Sdrn.Device
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MeasTasksBus : WcfServiceBase<IMeasTasksBus>, IMeasTasksBus
    {
        private readonly MessagesBus _bus;
        private readonly ILogger _logger;

        public MeasTasksBus(MessagesBus bus, ILogger logger)
        {
            this._bus = bus;
            this._logger = logger;
        }

        public Result AckCommand(SensorDescriptor sensorDescriptor, byte[] token)
        {
            throw new NotImplementedException();
        }

        public Result AckEntity(SensorDescriptor sensorDescriptor, byte[] token)
        {
            throw new NotImplementedException();
        }

        public Result AckEntityPart(SensorDescriptor sensorDescriptor, byte[] token)
        {
            throw new NotImplementedException();
        }

        public Result AckMeasTask(SensorDescriptor sensorDescriptor, byte[] token)
        {
            throw new NotImplementedException();
        }

        public BusResult<DeviceCommand> GetCommand(SensorDescriptor sensorDescriptor)
        {
            throw new NotImplementedException();
        }

        public BusResult<Entity> GetEntity(SensorDescriptor sensorDescriptor)
        {
            throw new NotImplementedException();
        }

        public BusResult<EntityPart> GetEntityPart(SensorDescriptor sensorDescriptor)
        {
            throw new NotImplementedException();
        }

        public BusResult<MeasTask> GetMeasTask(SensorDescriptor sensorDescriptor)
        {
            throw new NotImplementedException();
        }

        public Result<SensorRegistrationResult> RegisterSensor(Sensor sensor, string sdrnServer)
        {
            try
            {
                var descriptor = new SensorDescriptor
                {
                    SdrnServer = sdrnServer,
                    SensorName = sensor.Name,
                    EquipmentTechId = sensor.Equipment.TechId
                };

                var correlationId = Guid.NewGuid().ToString();
                var messageId = this._bus.SendObject(descriptor, "RegisterSensor", sensor, correlationId);

                var data = this._bus.WaiteObject<SensorRegistrationResult>(descriptor, "SendRegistrationResult", correlationId);

                var result = new Result<SensorRegistrationResult>
                {
                    State = OperationState.Success,
                    Data = data
                };

                return result;
            }
            catch(Exception e)
            {
                return new Result<SensorRegistrationResult>
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result SendCommandResult(SensorDescriptor sensorDescriptor, DeviceCommandResult commandResult)
        {
            throw new NotImplementedException();
        }

        public Result SendEntity(SensorDescriptor sensorDescriptor, Entity entity)
        {
            throw new NotImplementedException();
        }

        public Result SendEntityPart(SensorDescriptor sensorDescriptor, EntityPart entityPart)
        {
            throw new NotImplementedException();
        }

        public Result SendMeasResults(SensorDescriptor sensorDescriptor, MeasResults results)
        {
            throw new NotImplementedException();
        }
    }
}
