using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.WcfServices.Sdrn.Device;
using System.ServiceModel;

namespace Atdi.Test.Api.Sdrn.Device.WcfService
{
    class Program
    {

        private const int SensorRegistrationTimeOut = 100;
        private const int SensorWorkSleepTime = 1000;

        static void Main(string[] args)
        {


            Console.WriteLine($"Press any key to start SDRN Device WCF Service Client test ...");
            Console.ReadLine();

            Run("NetTcpEndpoint", "SDRNSV-SBD12-A00-8591");
            //Run("BasicHttpEndpoint", "SDRNSV-SBD12-A00-8591");
            //Run("NetNamedPipeEndpoint", "SDRNSV-SBD12-A00-8591");

            Console.WriteLine($"Press any key to exit ...");
            Console.ReadKey();
        }

        static void Run(string endpointSuffix, string sdrnServer)
        {
            try
            {
                Console.WriteLine($"Running test: endpoint = '{endpointSuffix}', SDRN Server = '{sdrnServer}'");

                var measTasksBusServiceEndpointName = "MeasTasksBus" + endpointSuffix;

                for (int i = 0; i < 1000; i++)
                {
                    SendMeasResultsSimple(measTasksBusServiceEndpointName);
                }
                
                return;

                //    var sensor = CreateSensorData();

                //    var regInfo = RegisterSensor(measTasksBusServiceEndpointName, sensor, sdrnServer);

                //    if (regInfo != null)
                //    {
                //        Console.WriteLine($"Sensor '{regInfo.SensorName}' was registered: status = '{regInfo.Status}', Tech ID = '{regInfo.EquipmentTechId}'");
                //    }

                //    var updInfo = UpdateSensor(measTasksBusServiceEndpointName, sensor, sdrnServer);

                //    if (updInfo != null)
                //    {
                //        Console.WriteLine($"Sensor '{updInfo.SensorName}' was updated: status = '{updInfo.Status}', Tech ID = '{updInfo.EquipmentTechId}'");
                //    }

                //    var sensorDescriptor = new SensorDescriptor
                //    {
                //        SdrnServer = sdrnServer,
                //        SensorName = sensor.Name,
                //        EquipmentTechId = sensor.Equipment?.TechId
                //    };

                //    while (true)
                //    {
                //        var command = GetNextCommand(measTasksBusServiceEndpointName, sensorDescriptor);
                //        if (command != null)
                //        {
                //            HandleCommand(measTasksBusServiceEndpointName, sensorDescriptor, command);
                //        }

                //        var task = GetNextMeasTask(measTasksBusServiceEndpointName, sensorDescriptor);
                //        if (task != null)
                //        {
                //            HandleMeasTask(measTasksBusServiceEndpointName, sensorDescriptor, task);
                //        }

                //        var entity = GetNextEntity(measTasksBusServiceEndpointName, sensorDescriptor);
                //        if (entity != null)
                //        {
                //            HandleEntity(measTasksBusServiceEndpointName, sensorDescriptor, entity);
                //        }
                //        var entityPart = GetNextEntityPart(measTasksBusServiceEndpointName, sensorDescriptor);
                //        if (entityPart != null)
                //        {
                //            HandleEntityPart(measTasksBusServiceEndpointName, sensorDescriptor, entityPart);
                //        }

                //        System.Threading.Thread.Sleep(SensorWorkSleepTime);
                //    }
            }
            catch (Exception e)
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
            if (command != null)
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
            if (task != null)
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
            catch (Exception e)
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
                    EquipmentTechId = "Atdi.Sdrn.Device.WcfService", // идентификатор сенсора до 200 символов
                };


                var measResults = BuildTestMeasResults();

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


        static MeasResults BuildTestMeasResults()
        {
            var c = 10;//  50;
            var emottingsCount = c;
            var frequenciesCount = c;
            var frequencySamplesCount = c;
            var levelsCount = c;
            var routesCount = c;
            var stationResultsCount = c;
            var referenceLevels_LevelsCount = c;

            int bearingsCount = c;
            int bwMaskCount = c;
            int levelsSpectrumCount = c;
            int levelResultsCount = c;
            int infoBlocksCount = c;

            var result = new MeasResults
            {
                BandwidthResult = new BandwidthMeasResult
                {
                    Bandwidth_kHz = double.MaxValue,
                    MarkerIndex = int.MaxValue,
                    T1 = int.MaxValue,
                    T2 = int.MaxValue,
                    TraceCount = int.MaxValue,
                    СorrectnessEstimations = true
                },
                Emittings = BuildTestEmittings(emottingsCount),
                Frequencies = BuildTestFrequencies(frequenciesCount),
                FrequencySamples = BuildTestFrequencySamples(frequencySamplesCount),
                Levels_dBm = BuildTestLevelsdBm(levelsCount),
                Location = new Atdi.DataModels.Sdrns.GeoLocation
                {
                    AGL = double.MaxValue,
                    ASL = double.MinValue,
                    Lat = double.MaxValue,
                    Lon = double.MaxValue
                },
                Measured = DateTime.Now,
                Measurement = Atdi.DataModels.Sdrns.MeasurementType.Level,
                RefLevels = new ReferenceLevels
                {
                    levels = BuildTestReferenceLevels_Levels(referenceLevels_LevelsCount),
                    StartFrequency_Hz = double.MaxValue,
                    StepFrequency_Hz = double.MinValue
                },
                ResultId = Guid.NewGuid().ToString(),
                Routes = BuildTestRoutes(routesCount),
                ScansNumber = int.MaxValue,
                SensorId = int.MaxValue,
                StartTime = DateTime.Now,
                StationResults = BuildTestStationResults(stationResultsCount, bearingsCount, bwMaskCount, levelsSpectrumCount, levelResultsCount, infoBlocksCount),
                Status = Guid.NewGuid().ToString(),
                StopTime = DateTime.Now,
                SwNumber = int.MinValue,
                TaskId = Guid.NewGuid().ToString()
            };

            return result;
        }

        static float[] BuildTestReferenceLevels_Levels(int count)
        {
            var r = new Random();
            var result = new float[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = (float)r.NextDouble();
            }
            return result;
        }

        static StationMeasResult[] BuildTestStationResults(int count, int bearingsCount, int bwMaskCount, int levelsSpectrumCount, int levelResultsCount, int infoBlocksCount)
        {

            var result = new StationMeasResult[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new StationMeasResult
                {
                    Bearings = BuildTestBearings(bearingsCount),
                    GeneralResult = new GeneralMeasResult
                    {
                        BandwidthResult = new BandwidthMeasResult
                        {
                            Bandwidth_kHz = double.MinValue,
                            MarkerIndex = int.MinValue,
                            T1 = int.MaxValue,
                            T2 = int.MaxValue,
                            TraceCount = int.MaxValue,
                            СorrectnessEstimations = true
                        },
                        BWMask = BuildTestBWMask(bwMaskCount),
                        CentralFrequencyMeas_MHz = double.MaxValue,
                        LevelsSpectrum_dBm = BuildTestLevelsSpectrum(levelsSpectrumCount),
                        CentralFrequency_MHz = double.MaxValue,
                        MeasDuration_sec = double.MaxValue,
                        MeasFinishTime = DateTime.Now,
                        MeasStartTime = DateTime.Now,
                        OffsetFrequency_mk = double.MaxValue,
                        RBW_kHz = double.MinValue,
                        SpectrumStartFreq_MHz = decimal.MinValue,
                        SpectrumSteps_kHz = decimal.MaxValue,
                        StationSysInfo = new StationSysInfo
                        {
                            BandWidth = double.MaxValue,
                            BaseID = int.MaxValue,
                            BSIC = int.MinValue,
                            ChannelNumber = int.MaxValue,
                            CID = int.MinValue,
                            Code = double.MaxValue,
                            CtoI = double.MaxValue,
                            ECI = int.MinValue,
                            eNodeBId = int.MaxValue,
                            Freq = double.MaxValue,
                            IcIo = double.MaxValue,
                            INBAND_POWER = double.MaxValue,
                            InfoBlocks = BuildTestInfoBlocks(infoBlocksCount),
                            ISCP = double.MaxValue,
                            LAC = int.MaxValue,
                            Location = new Atdi.DataModels.Sdrns.GeoLocation
                            {
                                AGL = double.MaxValue,
                                ASL = double.MinValue,
                                Lat = double.MaxValue,
                                Lon = double.MaxValue
                            },
                            MCC = int.MaxValue,
                            MNC = int.MaxValue,
                            NID = int.MaxValue,
                            PCI = int.MaxValue,
                            PN = int.MinValue,
                            Power = double.MaxValue,
                            Ptotal = double.MaxValue,
                            RNC = int.MaxValue,
                            RSCP = double.MaxValue,
                            RSRP = double.MaxValue,
                            RSRQ = double.MaxValue,
                            SC = int.MaxValue,
                            SID = int.MaxValue,
                            TAC = int.MaxValue,
                            TypeCDMAEVDO = Guid.NewGuid().ToString(),
                            UCID = int.MaxValue
                        },
                        VBW_kHz = double.MaxValue
                    },
                    LevelResults = BuildTestLevelMeasResult(levelResultsCount),
                    RealGlobalSid = Guid.NewGuid().ToString(),
                    SectorId = Guid.NewGuid().ToString(),
                    Standard = Guid.NewGuid().ToString(),
                    StationId = Guid.NewGuid().ToString(),
                    Status = Guid.NewGuid().ToString(),
                    TaskGlobalSid = Guid.NewGuid().ToString()
                };
            }
            return result;
        }

        static StationSysInfoBlock[] BuildTestInfoBlocks(int count)
        {
            var result = new StationSysInfoBlock[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new StationSysInfoBlock
                {
                    Data = Guid.NewGuid().ToString(),
                    Type = Guid.NewGuid().ToString()
                };
            }
            return result;
        }

        static LevelMeasResult[] BuildTestLevelMeasResult(int count)
        {
            var r = new Random();
            var result = new LevelMeasResult[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new LevelMeasResult
                {
                    DifferenceTimeStamp_ns = r.NextDouble(),
                    Level_dBm = r.NextDouble(),
                    Level_dBmkVm = r.NextDouble(),
                    Location = new Atdi.DataModels.Sdrns.GeoLocation
                    {
                        AGL = r.NextDouble(),
                        ASL = r.NextDouble(),
                        Lat = r.NextDouble(),
                        Lon = r.NextDouble(),
                    },
                    MeasurementTime = DateTime.Now
                };
            }
            return result;
        }

        static float[] BuildTestLevelsSpectrum(int count)
        {
            var r = new Random();
            var result = new float[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = (float)r.NextDouble();
            }
            return result;
        }

        static ElementsMask[] BuildTestBWMask(int count)
        {
            var r = new Random();
            var result = new ElementsMask[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new ElementsMask
                {
                    BW_kHz = r.NextDouble(),
                    Level_dB = r.NextDouble()
                };
            }
            return result;
        }

        static DirectionFindingData[] BuildTestBearings(int count)
        {
            var r = new Random();
            var result = new DirectionFindingData[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new DirectionFindingData
                {
                    AntennaAzimut = r.NextDouble(),
                    Bandwidth_kHz = r.NextDouble(),
                    Bearing = r.NextDouble(),
                    CentralFrequency_MHz = r.NextDouble(),
                    Level_dBm = r.NextDouble(),
                    Level_dBmkVm = r.NextDouble(),
                    Location = new Atdi.DataModels.Sdrns.GeoLocation
                    {
                        AGL = r.NextDouble(),
                        ASL = r.NextDouble(),
                        Lat = r.NextDouble(),
                        Lon = r.NextDouble(),
                    },
                    MeasurementTime = DateTime.MinValue,
                    Quality = r.NextDouble(),
                };
            }
            return result;
        }

        static Route[] BuildTestRoutes(int count)
        {
            var r = new Random();
            var result = new Route[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new Route
                {
                    RouteId = Guid.NewGuid().ToString(),
                    RoutePoints = new RoutePoint[]
                    {
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                        }
                    }
                };
            }
            return result;
        }

        static Emitting[] BuildTestEmittings(int count)
        {
            var r = new Random();
            var result = new Emitting[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new Emitting
                {
                    CurentPower_dBm = r.NextDouble(),
                    EmittingParameters = new EmittingParameters
                    {
                        RollOffFactor = r.NextDouble(),
                        StandardBW = r.NextDouble(),
                    },
                    LastDetaileMeas = DateTime.Now,
                    LevelsDistribution = new LevelsDistribution
                    {
                        Count = new int[200],
                        Levels = new int[200]
                    },
                    MeanDeviationFromReference = r.NextDouble(),
                    ReferenceLevel_dBm = r.NextDouble(),
                    SensorId = r.Next(),
                    SignalMask = new SignalMask
                    {
                        Freq_kHz = new double[200],
                        Loss_dB = new float[200]
                    },
                    Spectrum = new Spectrum
                    {
                        Bandwidth_kHz = r.NextDouble(),
                        Contravention = true,
                        Levels_dBm = new float[200],
                        MarkerIndex = r.Next(),
                        SignalLevel_dBm = r.Next(),
                        SpectrumStartFreq_MHz = r.NextDouble(),
                        SpectrumSteps_kHz = r.NextDouble(),
                        T1 = r.Next(),
                        T2 = r.Next(),
                        TraceCount = r.Next(),
                        СorrectnessEstimations = true
                    },
                    SpectrumIsDetailed = true,
                    StartFrequency_MHz = r.NextDouble(),
                    StopFrequency_MHz = r.NextDouble(),
                    TriggerDeviationFromReference = r.NextDouble(),
                    WorkTimes = new WorkTime[]
                    {
                        new WorkTime{HitCount = 1},
                        new WorkTime{HitCount = 1},
                        new WorkTime{HitCount = 1}
                    }
                };
            }
            return result;
        }

        static float[] BuildTestFrequencies(int count)
        {
            var r = new Random();
            var result = new float[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = (float)r.NextDouble();
            }
            return result;
        }

        static FrequencySample[] BuildTestFrequencySamples(int count)
        {
            var r = new Random();
            var result = new FrequencySample[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new FrequencySample
                {
                    Freq_MHz = (float)r.NextDouble(),
                    Id = r.Next(),
                    LevelMax_dBm = (float)r.NextDouble(),
                    LevelMin_dBm = (float)r.NextDouble(),
                    Level_dBm = (float)r.NextDouble(),
                    Level_dBmkVm = (float)r.NextDouble(),
                    Occupation_Pt = (float)r.NextDouble(),
                };
            }
            return result;
        }

        static float[] BuildTestLevelsdBm(int count)
        {
            var r = new Random();
            var result = new float[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = (float)r.NextDouble();
            }
            return result;
        }
    }
}
