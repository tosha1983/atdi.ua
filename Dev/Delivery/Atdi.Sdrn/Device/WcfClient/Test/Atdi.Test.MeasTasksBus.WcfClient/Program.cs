using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.WcfServices.Sdrn.Device;
using System.ServiceModel;

namespace Atdi.Test.MeasTasksBus.WcfClient
{


    class Program
    {

        private const int SensorRegistrationTimeOut = 100;
        private const int SensorWorkSleepTime = 1000;

        static void Main(string[] args)
        {
    

            Console.WriteLine($"Press any key to start test ...");
            Console.ReadLine();

            Run("NetTcpEndpoint", "SDRNSV-SBD12-A00-8591");
            Run("BasicHttpEndpoint", "SDRNSV-SBD12-A00-8591");
            Run("NetNamedPipeEndpoint", "SDRNSV-SBD12-A00-8591");

            Console.WriteLine($"Press any key to exit ...");
            Console.ReadKey();
        }

        static void Run(string endpointSuffix, string sdrnServer)
        {
            try
            {
                Console.WriteLine($"Running test: endpoint = '{endpointSuffix}', SDRN Server = '{sdrnServer}'");

                var measTasksBusServiceEndpointName = "MeasTasksBus" + endpointSuffix;
                var sensor = CreateSensorData();

                var regInfo = RegisterSensor(measTasksBusServiceEndpointName, sensor, sdrnServer);

                if (regInfo != null)
                {
                    Console.WriteLine($"Sensor '{regInfo.SensorName}' was registered: status = '{regInfo.Status}', Tech ID = '{regInfo.EquipmentTechId}'");
                }

                var updInfo = UpdateSensor(measTasksBusServiceEndpointName, sensor, sdrnServer);

                if (updInfo != null)
                {
                    Console.WriteLine($"Sensor '{updInfo.SensorName}' was updated: status = '{updInfo.Status}', Tech ID = '{updInfo.EquipmentTechId}'");
                }

                var sensorDescriptor = new SensorDescriptor
                {
                    SdrnServer = sdrnServer,
                    SensorName = sensor.Name,
                    EquipmentTechId = sensor.Equipment?.TechId
                };

                while (true)
                {
                    var command = GetNextCommand(measTasksBusServiceEndpointName, sensorDescriptor);
                    if (command != null)
                    {
                        HandleCommand(measTasksBusServiceEndpointName, sensorDescriptor, command);
                    }

                    var task = GetNextMeasTask(measTasksBusServiceEndpointName, sensorDescriptor);
                    if (task != null)
                    { 
                        HandleMeasTask(measTasksBusServiceEndpointName, sensorDescriptor, task);
                    }

                    var entity = GetNextEntity(measTasksBusServiceEndpointName, sensorDescriptor);
                    if (entity != null)
                    {
                        HandleEntity(measTasksBusServiceEndpointName, sensorDescriptor, entity);
                    }
                    var entityPart = GetNextEntityPart(measTasksBusServiceEndpointName, sensorDescriptor);
                    if (entityPart != null)
                    {
                        HandleEntityPart(measTasksBusServiceEndpointName, sensorDescriptor, entityPart);
                    }

                    System.Threading.Thread.Sleep(SensorWorkSleepTime);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }

            
        }

        static DeviceCommand GetNextCommand(string endpointName, SensorDescriptor sensorDescriptor)
        {
            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var getCommandResult = busService.GetCommand(sensorDescriptor);
            if (getCommandResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(getCommandResult.FaultCause);
            }

            var command = getCommandResult.Data;
            if (command != null )
            {
                Console.WriteLine($"New commands have been received: Name = '{command.Command}'");
            }

            if (getCommandResult.Token != null)
            {
             //   busService.AckCommand(sensorDescriptor, getCommandResult.Token);
            }
            
            return command;
        }

        static MeasTask GetNextMeasTask(string endpointName, SensorDescriptor sensorDescriptor)
        {

            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var getMeasTaskResult = busService.GetMeasTask(sensorDescriptor);
            if (getMeasTaskResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(getMeasTaskResult.FaultCause);
            }

            busService.AckMeasTask(sensorDescriptor, getMeasTaskResult.Token);

            var task = getMeasTaskResult.Data;
            if (task != null )
            {
                Console.WriteLine($"New measurment tasks have been received: ID = '{task.TaskId}'");
            }

            if (getMeasTaskResult.Token != null)
            {
              //  busService.AckMeasTask(sensorDescriptor, getMeasTaskResult.Token);
            }

            return task;
        }

        static Entity GetNextEntity(string endpointName, SensorDescriptor sensorDescriptor)
        {
            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var getEntityResult = busService.GetEntity(sensorDescriptor);
            if (getEntityResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(getEntityResult.FaultCause);
            }

            var entity = getEntityResult.Data;
            if (entity != null)
            {
                Console.WriteLine($"New entity have been received: Name = '{entity.Name}'");
            }

            if (getEntityResult.Token != null)
            {
              //  busService.AckEntity(sensorDescriptor, getEntityResult.Token);
            }
            
            return entity;
        }

        static EntityPart GetNextEntityPart(string endpointName, SensorDescriptor sensorDescriptor)
        {
            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var getEntityPartResult = busService.GetEntityPart(sensorDescriptor);
            if (getEntityPartResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(getEntityPartResult.FaultCause);
            }

            var entityPart = getEntityPartResult.Data;
            if (entityPart != null)
            {
                Console.WriteLine($"New entity part have been received: Entity ID = '{entityPart.EntityId}', Part index = #{entityPart.PartIndex}");
            }

            if (getEntityPartResult.Token != null)
            {
              //  busService.AckEntityPart(sensorDescriptor, getEntityPartResult.Token);
            }
            
            return entityPart;
        }

        static void HandleCommand(string endpointName, SensorDescriptor sensorDescriptor, DeviceCommand deviceCommand)
        {
            switch (deviceCommand.Command)
            {
                case "CheckSensorActivity":
                    break;
                case "StopMeasTask":
                    break;
                default:
                    break;
            }

            var result = new DeviceCommandResult
            {
                
                CommandId = deviceCommand.CommandId,
                Status = "Success"
            };

            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var sendCommandResultsResult = busService.SendCommandResult(sensorDescriptor, result);
            if (sendCommandResultsResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(sendCommandResultsResult.FaultCause);
            }

            Console.WriteLine($"Command '#{deviceCommand.CommandId}: {deviceCommand.Command}' has been handled");
        }

        static void HandleMeasTask(string endpointName, SensorDescriptor sensorDescriptor, MeasTask measTask)
        {
            var result = new MeasResults
            {
                TaskId = measTask.TaskId
            };

            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var sendMeasResultsResult = busService.SendMeasResults(sensorDescriptor, result);
            if (sendMeasResultsResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(sendMeasResultsResult.FaultCause);
            }

            Console.WriteLine($"Meas task '#{measTask.TaskId}' has been handled");
        }

        static void HandleEntity(string endpointName, SensorDescriptor sensorDescriptor, Entity entity)
        {
            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var sendEntityResult = busService.SendEntity(sensorDescriptor, entity);
            if (sendEntityResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(sendEntityResult.FaultCause);
            }

            Console.WriteLine($"New entity have been handled: Name = '{entity.Name}'");
        }

        static void HandleEntityPart(string endpointName, SensorDescriptor sensorDescriptor, EntityPart entityPart)
        {
            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var sendEntityPartResult = busService.SendEntityPart(sensorDescriptor, entityPart);
            if (sendEntityPartResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(sendEntityPartResult.FaultCause);
            }

            Console.WriteLine($"New entity part have been handled: Entity ID = '{entityPart.EntityId}', Part index = #{entityPart.PartIndex}");
        }

        static Sensor CreateSensorData()
        {
            var sensor = new Sensor
            {
                Name = "SENSOR-DBD12-A00-1280",
                Administration = "Administration",
                Antenna = new SensorAntenna
                {
                    AddLoss = 1,
                    Category = "SensorAntenna.Category",
                    Class = "SensorAntenna.Class",
                },
                Equipment = new SensorEquipment
                {
                    TechId = "#0010023747364"
                }
            };

            return sensor;
        }
        static IMeasTasksBus GetMeasTasksBusServicByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<IMeasTasksBus>(endpointName);
            return f.CreateChannel();
        }

        // use case #1
        static SensorRegistrationResult RegisterSensor(string endpointName, Sensor sensor, string sdrnServer)
        {
            try
            {
                SensorRegistrationResult result = null;

                var busService = GetMeasTasksBusServicByEndpoint(endpointName);

                var registerSensorResult = busService.RegisterSensor(sensor, sdrnServer);

                if (registerSensorResult.State == DataModels.CommonOperation.OperationState.Fault)
                {
                    throw new InvalidOperationException(registerSensorResult.FaultCause);
                }

                result = registerSensorResult.Data;

                if (result == null)
                {
                    throw new InvalidOperationException("Test RegisterSensor is failed: result is null ");
                }
                if (result.SdrnServer != sdrnServer)
                {
                    throw new InvalidOperationException("Test RegisterSensor is failed: SdrnServer ");
                }
                if (result.SensorName != sensor.Name)
                {
                    throw new InvalidOperationException("Test RegisterSensor is failed: SdrnServer ");
                }
                if (result.EquipmentTechId != sensor.Equipment.TechId)
                {
                    throw new InvalidOperationException("Test RegisterSensor is failed: EquipmentTechId ");
                }

                Console.WriteLine($"Test PASS: the sensor '{sensor.Name}' has been registered [Status='{result.Status}']");

                return result;
            }
            catch(Exception e)
            {
                throw new InvalidOperationException("The sensor was not registered", e);
            }
        }

        static SensorUpdatingResult UpdateSensor(string endpointName, Sensor sensor, string sdrnServer)
        {
            try
            {
                SensorUpdatingResult result = null;
                var busService = GetMeasTasksBusServicByEndpoint(endpointName);

                var updateSensorResult = busService.UpdateSensor(sensor, sdrnServer);
                if (updateSensorResult.State == DataModels.CommonOperation.OperationState.Fault)
                {
                    throw new InvalidOperationException(updateSensorResult.FaultCause);
                }

                result = updateSensorResult.Data;

                if (result == null)
                {
                    throw new InvalidOperationException("Test UpdateSensor is failed: result is null ");
                }
                if (result.SdrnServer != sdrnServer)
                {
                    throw new InvalidOperationException("Test UpdateSensor is failed: SdrnServer ");
                }
                if (result.SensorName != sensor.Name)
                {
                    throw new InvalidOperationException("Test UpdateSensor is failed: SdrnServer ");
                }
                if (result.EquipmentTechId != sensor.Equipment.TechId)
                {
                    throw new InvalidOperationException("Test UpdateSensor is failed: EquipmentTechId ");
                }

                Console.WriteLine($"Test PASS: the sensor '{sensor.Name}' has been updated [Status='{result.Status}']");

                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The sensor was not update", e);
            }
        }

        static SensorRegistrationResult RegisterSensorSimple(string endpointName)
        {
            try
            {
                var sdrnServer = "SDRNSV-SBD12-A00-8591"; // Имя SDRN Server (зависит от доступной на сервере лицензии)
                var sensor = new Sensor()
                {
                   Name = "SENSOR-DBD12-A00-1280", //Важно: Имя сенсора из лицензии
                   Equipment = new SensorEquipment
                   {
                       TechId = "SomeSensor SN:0923382737273", // идентификатор сенсора до 200 символов
                   },
                   Antenna = new SensorAntenna
                   {
                       TechId = "SomeTechId"
                   }
                   // определение прочих реквизитов сенсора
                };

                SensorRegistrationResult result = null;

                var busService = GetMeasTasksBusServicByEndpoint(endpointName);

                var registerSensorResult = busService.RegisterSensor(sensor, sdrnServer);
                if (registerSensorResult.State == DataModels.CommonOperation.OperationState.Fault)
                {
                    throw new InvalidOperationException(registerSensorResult.FaultCause);
                }

                result = registerSensorResult.Data;

                Console.WriteLine($"The sensor '{sensor.Name}' has been registered [Status='{result.Status}']");

                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The sensor was not registered", e);
            }
        }

        static void SendMeasResultsSimple(string endpointName)
        {
            try
            {
                var sensorDescriptor = new SensorDescriptor()
                {
                    SdrnServer = "SDRNSV-SBD12-A00-8591", // Имя SDRN Server (зависит от доступной на сервере лицензии)
                    SensorName = "SENSOR-DBD12-A00-1280", //Важно: Имя сенсора из лицензии WCF сервиса
                    EquipmentTechId = "SomeSensor SN:0923382737273", // идентификатор сенсора до 200 символов
                };


                var measResults = new MeasResults
                {
                    TaskId = "Some task ID",
                    ResultId = "Some result ID",
                    StationResults = new StationMeasResult[]
                    {
                        new StationMeasResult
                        {
                            SectorId = "Some sector ID"
                        }
                    },

                    // определение прочих данныых об измерении
                };

                var busService = GetMeasTasksBusServicByEndpoint(endpointName);

                var sendMeasResultsResult = busService.SendMeasResults(sensorDescriptor, measResults);
                if (sendMeasResultsResult.State == DataModels.CommonOperation.OperationState.Fault)
                {
                    throw new InvalidOperationException(sendMeasResultsResult.FaultCause);
                }


                Console.WriteLine($"The sensor '{sensorDescriptor.SensorName}' has sent measurment result '{measResults.Measured}'");

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The meas results was not sent to server", e);
            }
        }
    }
}
