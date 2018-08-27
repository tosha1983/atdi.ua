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
            Console.ReadKey();

            Run("NetTcpEndpoint", "SdrnServer1");
            Run("BasicHttpEndpoint", "SdrnServer2");
            Run("NetNamedPipeEndpoint", "SdrnServer2");

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

                Console.WriteLine($"Sensor '{regInfo.SensorName}' was registered: status = '{regInfo.SensorId}', id = '{regInfo.SensorId}'");

                while (true)
                {
                    var command = GetNextCommand(measTasksBusServiceEndpointName, sensor);
                    if (command != null)
                    {
                        HandleCommand(measTasksBusServiceEndpointName, command, sensor);
                    }

                    var task = GetNextMeasTask(measTasksBusServiceEndpointName, sensor);
                    if (task != null)
                    { 
                        HandleMeasTask(measTasksBusServiceEndpointName, task, sensor);
                    }

                    System.Threading.Thread.Sleep(SensorWorkSleepTime);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }

            
        }

        static DeviceCommand GetNextCommand(string endpointName, Sensor sensor)
        {
            var sensorDescriptor = new SensorDescriptor
            {
                SensorName = sensor.Name,
                EquipmentTechId = sensor.Equipment?.TechId
            };

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

            return command;
        }

        static MeasTask GetNextMeasTask(string endpointName, Sensor sensor)
        {
            var sensorDescriptor = new SensorDescriptor
            {
                SensorName = sensor.Name,
                EquipmentTechId = sensor.Equipment?.TechId
            };

            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var getMeasTaskResult = busService.GetMeasTask(sensorDescriptor);
            if (getMeasTaskResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(getMeasTaskResult.FaultCause);
            }

            var task = getMeasTaskResult.Data;
            if (task != null )
            {
                Console.WriteLine($"New measurment tasks have been received: ID = '{task.TaskId}'");
            }

            return task;
        }

        static void HandleCommand(string endpointName, DeviceCommand deviceCommand, Sensor sensor)
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

            var descriptor = new SensorDescriptor
            {
                SdrnServer = deviceCommand.SdrnServer,
                SensorName = sensor.Name,
                EquipmentTechId = sensor.Equipment.TechId
            };
            var result = new DeviceCommandResult
            {
                
                CommandId = deviceCommand.CommandId,
                Status = "Success"
            };

            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var sendCommandResultsResult = busService.SendCommandResult(descriptor, result);
            if (sendCommandResultsResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(sendCommandResultsResult.FaultCause);
            }

            Console.WriteLine($"Command '#{deviceCommand.CommandId}: {deviceCommand.Command}' has been handled");
        }

        static void HandleMeasTask(string endpointName, MeasTask measTask, Sensor sensor)
        {
            var result = new MeasResults
            {
                //SdrnServer = measTask.SdrnServer,
                //SensorName = sensor.Name,
                //EquipmentTechId = sensor.Equipment.TechId,
                TaskId = measTask.TaskId
            };
            var descriptor = new SensorDescriptor
            {
                SdrnServer = measTask.SdrnServer,
                SensorName = sensor.Name,
                EquipmentTechId = sensor.Equipment.TechId
            };

            

            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var sendMeasResultsResult = busService.SendMeasResults(descriptor, result);
            if (sendMeasResultsResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(sendMeasResultsResult.FaultCause);
            }

            Console.WriteLine($"Meas task '#{measTask.TaskId}' has been handled");
        }

        static Sensor CreateSensorData()
        {
            var sensor = new Sensor
            {
                Administration = "Administration",
                Antenna = new SensorAntenna
                {
                    AddLoss = 1,
                    Category = "SensorAntenna.Category",
                    Class = "SensorAntenna.Class",
                },
                Equipment = new SensorEquipment
                {
                    TechId = "SN:0010023747364"
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

                var busService = GetMeasTasksBusServicByEndpoint(endpointName);

                var tryRegisterResult = busService.RegisterSensor(sensor, sdrnServer);

                if (tryRegisterResult.State == DataModels.CommonOperation.OperationState.Fault)
                {
                    throw new InvalidOperationException(tryRegisterResult.FaultCause);
                }

                return tryRegisterResult.Data;

            }
            catch(Exception e)
            {
                throw new InvalidOperationException("The sensor was not registered", e);
            }
        }

    }
}
