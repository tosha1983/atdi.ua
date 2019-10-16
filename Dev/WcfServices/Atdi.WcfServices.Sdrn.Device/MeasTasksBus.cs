using System;
using System.ServiceModel;
using Atdi.Api.Sdrn.Device.BusController;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.WcfServices.Sdrn.Device;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;



namespace Atdi.WcfServices.Sdrn.Device
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MeasTasksBus : WcfServiceBase<IMeasTasksBus>, IMeasTasksBus
    {
        private readonly IMessageDispatcher _dispatcher;
        private readonly IMessagePublisher _publisher;
        private readonly IServiceEnvironment _environment;
        private readonly SdrnServerDescriptor _descriptor;
        private readonly ILogger _logger;
        
        public MeasTasksBus(IMessageDispatcher dispatcher, IMessagePublisher publisher, IServiceEnvironment environment, SdrnServerDescriptor descriptor, ILogger logger)
        {
            this._dispatcher = dispatcher;
            this._publisher = publisher;
            this._environment = environment;
            _descriptor = descriptor;
            this._logger = logger;
            _logger.Verbouse(Contexts.ThisComponent, Categories.Creating, "The meas task service was created");
        }

        public void CheckSensorName(string name)
        {
            if (!_environment.AllowedSensors.ContainsKey(name))
            {
                throw new InvalidOperationException("Attempting to use an unlicensed device");
            }
        }

        public void CheckSensorTechId(string name)
        {
            if (!_descriptor.SensorTechId.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Attempting to use an unlicensed device");
            }
        }

        private void Verify(SensorDescriptor sensorDescriptor)
        {
            if (sensorDescriptor == null)
            {
                throw new ArgumentNullException(nameof(sensorDescriptor));
            }

            if (string.IsNullOrEmpty(sensorDescriptor.SdrnServer))
            {
                throw new ArgumentOutOfRangeException(nameof(sensorDescriptor.SdrnServer));
            }

            if (string.IsNullOrEmpty(sensorDescriptor.SensorName))
            {
                throw new ArgumentOutOfRangeException(nameof(sensorDescriptor.SensorName));
            }

            if (string.IsNullOrEmpty(sensorDescriptor.EquipmentTechId))
            {
                throw new ArgumentOutOfRangeException(nameof(sensorDescriptor.EquipmentTechId));
            }

            this.CheckSensorName(sensorDescriptor.SensorName);
            this.CheckSensorTechId(sensorDescriptor.EquipmentTechId);

            //this._busConsumers.DeclareSensor(sensorDescriptor.SensorName, sensorDescriptor.EquipmentTechId);

        }
        public Result AckCommand(SensorDescriptor sensorDescriptor, byte[] token)
        {
            try
            {
                this.Verify(sensorDescriptor);
                
                //this._bus.AckMessage(sensorDescriptor, token);
                this._dispatcher.TryAckMessage(MessageToken.FromBytes(token));

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);

                //this._bus.AckMessage(sensorDescriptor, token);
                this._dispatcher.TryAckMessage(MessageToken.FromBytes(token));

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);

                //this._bus.AckMessage(sensorDescriptor, token);
                this._dispatcher.TryAckMessage(MessageToken.FromBytes(token));

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);

                //this._bus.AckMessage(sensorDescriptor, token);
                this._dispatcher.TryAckMessage(MessageToken.FromBytes(token));

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);

                var data = this._dispatcher.TryGetObject<DeviceCommand>("SendCommand");

                var result = new BusResult<DeviceCommand>
                {
                    State = OperationState.Success,
                    Data = data?.Data,
                    Token = MessageToken.ToBytes(data?.Token)
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);

                var data = this._dispatcher.TryGetObject<Entity>("SendEntity");

                var result = new BusResult<Entity>
                {
                    State = OperationState.Success,
                    Data = data?.Data,
                    Token = MessageToken.ToBytes(data?.Token)
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);

                var data = this._dispatcher.TryGetObject<EntityPart>("SendEntityPart");

                var result = new BusResult<EntityPart>
                {
                    State = OperationState.Success,
                    Data = data?.Data,
                    Token = MessageToken.ToBytes(data?.Token)
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);
                
                var data = this._dispatcher.TryGetObject<MeasTask>("SendMeasTask");

                var result = new BusResult<MeasTask>
                {
                    State = OperationState.Success,
                    Data = data?.Data,
                    Token = MessageToken.ToBytes(data?.Token)
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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

                this.Verify(descriptor);

                var messageToken = this._publisher.Send("RegisterSensor", sensor);

                var data = this._dispatcher.WaitObject<SensorRegistrationResult>("SendRegistrationResult");

                var result = new Result<SensorRegistrationResult>
                {
                    State = OperationState.Success,
                    Data = data.Data
                };

                return result;
            }
            catch(Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);

                this._publisher.Send("SendCommandResult", commandResult);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);

                this._publisher.Send("SendEntity", entity);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);

                this._publisher.Send("SendEntityPart", entityPart);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                this.Verify(sensorDescriptor);

                this._publisher.Send("SendMeasResults", results);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
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
                var sensorDescriptor = new SensorDescriptor
                {
                    SdrnServer = sdrnServer,
                    SensorName = sensor.Name,
                    EquipmentTechId = sensor.Equipment.TechId
                };

                this.Verify(sensorDescriptor);

                var messageToken = this._publisher.Send("UpdateSensor", sensor);

                var data = this._dispatcher.WaitObject<SensorUpdatingResult>("SendSensorUpdatingResult");

                var result = new Result<SensorUpdatingResult>
                {
                    State = OperationState.Success,
                    Data = data.Data
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
                return new Result<SensorUpdatingResult>
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result SendSensorRegistration(Sensor sensor, string sdrnServer)
        {
            try
            {
                var descriptor = new SensorDescriptor
                {
                    SdrnServer = sdrnServer,
                    SensorName = sensor.Name,
                    EquipmentTechId = sensor.Equipment.TechId
                };

                this.Verify(descriptor);

                this._publisher.Send("RegisterSensor", sensor);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result<SensorRegistrationResult> GetSensorRegistrationResult(SensorDescriptor sensorDescriptor)
        {
            try
            {
                this.Verify(sensorDescriptor);

                var data = this._dispatcher.TryGetObject<SensorRegistrationResult>("SendRegistrationResult");

                var result = new BusResult<SensorRegistrationResult>
                {
                    State = OperationState.Success,
                    Data = data?.Data,
                    Token = MessageToken.ToBytes(data?.Token)
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
                return new BusResult<SensorRegistrationResult>
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result AckSensorRegistrationResult(SensorDescriptor sensorDescriptor, byte[] token)
        {
            try
            {
                this.Verify(sensorDescriptor);

                this._dispatcher.TryAckMessage(MessageToken.FromBytes(token));

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result SendSensorUpdating(Sensor sensor, string sdrnServer)
        {
            try
            {
                var sensorDescriptor = new SensorDescriptor
                {
                    SdrnServer = sdrnServer,
                    SensorName = sensor.Name,
                    EquipmentTechId = sensor.Equipment.TechId
                };

                this.Verify(sensorDescriptor);

               this._publisher.Send("UpdateSensor", sensor);

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result<SensorUpdatingResult> GetSensorUpdatingResult(SensorDescriptor sensorDescriptor)
        {
            try
            {
                this.Verify(sensorDescriptor);

                var data = this._dispatcher.TryGetObject<SensorUpdatingResult>("SendSensorUpdatingResult");

                var result = new BusResult<SensorUpdatingResult>
                {
                    State = OperationState.Success,
                    Data = data?.Data,
                    Token = MessageToken.ToBytes(data?.Token)
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
                return new BusResult<SensorUpdatingResult>
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }

        public Result AckSensorUpdatingResult(SensorDescriptor sensorDescriptor, byte[] token)
        {
            try
            {
                this.Verify(sensorDescriptor);

                this._dispatcher.TryAckMessage(MessageToken.FromBytes(token));

                var result = new Result
                {
                    State = OperationState.Success
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
                return new Result
                {
                    FaultCause = e.Message,
                    State = OperationState.Fault
                };
            }
        }
    }
}
