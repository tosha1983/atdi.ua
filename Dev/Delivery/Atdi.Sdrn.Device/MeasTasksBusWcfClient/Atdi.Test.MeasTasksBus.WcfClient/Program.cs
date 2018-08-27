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
                    var commands = GetNextCommands(measTasksBusServiceEndpointName, sensor);
                    for (int i = 0; i < commands.Length; i++)
                    {
                        HandleCommand(measTasksBusServiceEndpointName, commands[i], sensor);
                    }

                    var tasks = GetNextMeasTasks(measTasksBusServiceEndpointName, sensor);
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        HandleMeasTask(measTasksBusServiceEndpointName, tasks[i], sensor);
                    }

                    System.Threading.Thread.Sleep(SensorWorkSleepTime);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }

            
        }

        static DeviceCommand[] GetNextCommands(string endpointName, Sensor sensor)
        {
            var sensorDescriptor = new SensorDescriptor
            {
                SensorName = sensor.Name,
                EquipmentTechId = sensor.Equipment?.TechId
            };

            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var getCommandsResult = busService.GetCommands(sensorDescriptor);
            if (getCommandsResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(getCommandsResult.FaultCause);
            }

            var commands = getCommandsResult.Data;
            if (commands != null && commands.Length > 0)
            {
                Console.WriteLine($"New commands have been received: count = '{commands.Length}'");
            }

            return commands;
        }

        static MeasTask[] GetNextMeasTasks(string endpointName, Sensor sensor)
        {
            var sensorDescriptor = new SensorDescriptor
            {
                SensorName = sensor.Name,
                EquipmentTechId = sensor.Equipment?.TechId
            };

            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var getMeasTasksResult = busService.GetMeasTasks(sensorDescriptor);
            if (getMeasTasksResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(getMeasTasksResult.FaultCause);
            }

            var tasks = getMeasTasksResult.Data;
            if (tasks != null && tasks.Length > 0)
            {
                Console.WriteLine($"New measurment tasks have been received: count = '{tasks.Length}'");
            }

            return tasks;
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

            var result = new DeviceCommandResult
            {
                SdrnServer = deviceCommand.SdrnServer,
                SensorName = sensor.Name,
                EquipmentTechId = sensor.Equipment.TechId,
                CommandId = deviceCommand.CommandId,
                Status = "Success"
            };

            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var sendCommandResultsResult = busService.SendCommandResults(new DeviceCommandResult[] { result });
            if (sendCommandResultsResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                throw new InvalidOperationException(sendCommandResultsResult.FaultCause);
            }

            Console.WriteLine($"Command '#{deviceCommand.CommandId}: {deviceCommand.Command}' has been handled");
        }

        static void HandleMeasTask(string endpointName, MeasTask measTask, Sensor sensor)
        {
            var result1 = new MeasResults
            {
                SdrnServer = measTask.SdrnServer,
                SensorName = sensor.Name,
                EquipmentTechId = sensor.Equipment.TechId,
                TaskId = measTask.TaskId
            };
            var result2 = new MeasResults
            {
                SdrnServer = measTask.SdrnServer,
                SensorName = sensor.Name,
                EquipmentTechId = sensor.Equipment.TechId,
                TaskId = measTask.TaskId
            };
            var result3 = new MeasResults
            {
                SdrnServer = measTask.SdrnServer,
                SensorName = sensor.Name,
                EquipmentTechId = sensor.Equipment.TechId,
                TaskId = measTask.TaskId
            };

            var measResults = new MeasResults[]
            {
                result1, result2, result3
            };

            var busService = GetMeasTasksBusServicByEndpoint(endpointName);
            var sendMeasResultsResult = busService.SendMeasResults(measResults);
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
                AGL = 0.111,
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
                SensorRegistrationResult sensorRegistrationResult = null;

                var busService = GetMeasTasksBusServicByEndpoint(endpointName);

                var tryRegisterResult = busService.TryRegister(sensor, sdrnServer);

                if (tryRegisterResult.State == DataModels.CommonOperation.OperationState.Fault)
                {
                    throw new InvalidOperationException(tryRegisterResult.FaultCause);
                }

                var sensorDescriptor = new SensorDescriptor
                {
                    SensorName = sensor.Name,
                    EquipmentTechId = sensor.Equipment?.TechId
                };

                var attempt = 100;
                while(--attempt >= 0)
                {
                    var getRegistrationResultsResult = busService.GetRegistrationResults(sensorDescriptor);
                    if (getRegistrationResultsResult.State == DataModels.CommonOperation.OperationState.Fault)
                    {
                        throw new InvalidOperationException(getRegistrationResultsResult.FaultCause);
                    }

                    foreach (var result in getRegistrationResultsResult.Data)
                    {
                        if (result != null && result.SdrnServer == sdrnServer)
                        {
                            sensorRegistrationResult = result;
                            break;
                        }
                    }
                    System.Threading.Thread.Sleep(SensorRegistrationTimeOut);
                }

                if (sensorRegistrationResult == null)
                {
                    throw new InvalidOperationException("Time out is over");
                }

                return sensorRegistrationResult;
            }
            catch(Exception e)
            {
                throw new InvalidOperationException("The sensor was not registered", e);
            }
        }

    }
}
