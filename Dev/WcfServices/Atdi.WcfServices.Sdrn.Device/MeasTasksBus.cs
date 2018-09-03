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
            try
            {
                this._bus.AckMessage(sensorDescriptor, token);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result AckEntity(SensorDescriptor sensorDescriptor, byte[] token)
        {
            try
            {
                this._bus.AckMessage(sensorDescriptor, token);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result AckEntityPart(SensorDescriptor sensorDescriptor, byte[] token)
        {
            try
            {
                this._bus.AckMessage(sensorDescriptor, token);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result AckMeasTask(SensorDescriptor sensorDescriptor, byte[] token)
        {
            try
            {
                this._bus.AckMessage(sensorDescriptor, token);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public BusResult<DeviceCommand> GetCommand(SensorDescriptor sensorDescriptor)
        {
            try
            {
                var token = string.Empty;
                var data = this._bus.TryGetObject<DeviceCommand>(sensorDescriptor, "SendCommand", out token);

                var result = new BusResult<DeviceCommand>
                {
                    State = OperationState.Success,
                    Data = data,
                    Token = Encoding.UTF8.GetBytes(token)
                };

                return result;
            }
            catch (Exception e)
            {
                return new BusResult<DeviceCommand>
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public BusResult<Entity> GetEntity(SensorDescriptor sensorDescriptor)
        {
            try
            {
                var token = string.Empty;
                var data = this._bus.TryGetObject<Entity>(sensorDescriptor, "SendEntity", out token);

                var result = new BusResult<Entity>
                {
                    State = OperationState.Success,
                    Data = data,
                    Token = Encoding.UTF8.GetBytes(token)
                };

                return result;
            }
            catch (Exception e)
            {
                return new BusResult<Entity>
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public BusResult<EntityPart> GetEntityPart(SensorDescriptor sensorDescriptor)
        {
            try
            {
                var token = string.Empty;
                var data = this._bus.TryGetObject<EntityPart>(sensorDescriptor, "SendEntityPart", out token);

                var result = new BusResult<EntityPart>
                {
                    State = OperationState.Success,
                    Data = data,
                    Token = Encoding.UTF8.GetBytes(token)
                };

                return result;
            }
            catch (Exception e)
            {
                return new BusResult<EntityPart>
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public BusResult<MeasTask> GetMeasTask(SensorDescriptor sensorDescriptor)
        {
            try
            {
                var token = string.Empty;
                var data = this._bus.TryGetObject<MeasTask>(sensorDescriptor, "SendMeasTask", out token);

                var result = new BusResult<MeasTask>
                {
                    State = OperationState.Success,
                    Data = data,
                    Token = Encoding.UTF8.GetBytes(token)
                };

                return result;
            }
            catch (Exception e)
            {
                return new BusResult<MeasTask>
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
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

                var data = this._bus.WaitObject<SensorRegistrationResult>(descriptor, "SendRegistrationResult", correlationId);

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
            try
            {
                this._bus.SendObject(sensorDescriptor, "SendCommandResult", commandResult);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result SendEntity(SensorDescriptor sensorDescriptor, Entity entity)
        {
            try
            {
                this._bus.SendObject(sensorDescriptor, "SendEntity", entity);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result SendEntityPart(SensorDescriptor sensorDescriptor, EntityPart entityPart)
        {
            try
            {
                this._bus.SendObject(sensorDescriptor, "SendEntityPart", entityPart);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result SendMeasResults(SensorDescriptor sensorDescriptor, MeasResults results)
        {
            try
            {
                this._bus.SendObject(sensorDescriptor, "SendMeasResults", results);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result<SensorUpdatingResult> UpdateSensor(Sensor sensor, string sdrnServer)
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
                var messageId = this._bus.SendObject(descriptor, "UpdateSensor", sensor, correlationId);

                var data = this._bus.WaitObject<SensorUpdatingResult>(descriptor, "SendSensorUpdatingResult", correlationId);

                var result = new Result<SensorUpdatingResult>
                {
                    State = OperationState.Success,
                    Data = data
                };

                return result;
            }
            catch (Exception e)
            {
                return new Result<SensorUpdatingResult>
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }
    }
}
