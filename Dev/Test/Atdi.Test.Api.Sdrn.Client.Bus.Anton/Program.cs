using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Api.Sdrn.Device.BusController;

using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrns.Device;
using Newtonsoft.Json;

namespace Atdi.Test.Api.Sdrn.Device.BusController
{
    class JsonData
    {
        public string Type;
        public string JsonBody;
    }
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Test();
            }

        }
        static void Test()
        {

            var gateFactory = BusGateFactory.Create();
            var gate = CreateGate(gateFactory);
            var publisher = gate.CreatePublisher("main");

            var res = LoadFromFiles(@"C:\Users\Administrator\Desktop\UploadMS");
            for (int i = 0; i < res.Length; i++)
            {
                var item = res[i];
                publisher.Send("SendMeasResults", item, $"MonitoringStations");
                Console.WriteLine($"TASK ID: {item.TaskId}");
            }

            Console.WriteLine($"Test finished ...");

            var measResult = BuildTestMeasResultsSignaling();
            //var measResult = BuildTestMeasResultsMonitoringStations()
            var commandResult = new DeviceCommandResult()
            {
                CommandId = Guid.NewGuid().ToString()
            };

            var entity = new Entity
            {
                EntityId = Guid.NewGuid().ToString(),
                ContentType = "xml",
                Description = "The some data",
                Encoding = "",
                Name = "Test object",
                PartIndex = 1,
                EOF = true,
                Content = new byte[250]
            };

            var entityPart = new EntityPart()
            {
                EOF = true,
                EntityId = entity.EntityId,
                Content = new byte[250]
            };
            var count = 1;
            for (int i = 0; i < count; i++)
            {
                //publisher.Send("RegisterSensor", sensor, $"ID #{i}");
                //Console.ReadLine();
                //publisher.Send("UpdateSensor", sensor, $"ID #{i}");
                //Console.ReadLine();
                //publisher.Send("SendCommandResult", commandResult, $"ID #{i}");
                //Console.ReadLine();

                publisher.Send("SendMeasResults", measResult, $"ID #{i}");
                //Console.ReadLine();


                //publisher.Send("SendEntity", entity, $"#{i}");
                //publisher.Send("SendEntityPart", entityPart, $"ID #{i}");

                //Console.WriteLine(i);
            }


            publisher.Dispose();
            //dispatcher.Deactivate();
            //dispatcher.Dispose();

            Console.ReadLine();
        }

        static IBusGate CreateGate(IBusGateFactory gateFactory)
        {
            var gateConfig = CreateConfig(gateFactory);
            var gate = gateFactory.CreateGate("MainGate", gateConfig);
            return gate;
        }
        static IBusGateConfig CreateConfig(IBusGateFactory gateFactory)
        {
            var config = gateFactory.CreateConfig();

            config["License.FileName"] = "LIC-DBD12-A00-878.SENSOR-DBD12-A00-8918.lic";
            config["License.OwnerId"] = "OID-BD12-A00-N00";
            config["License.ProductKey"] = "0VE1-OCOL-S4S0-C1D1-SEXB";

            config["RabbitMQ.Host"] = "localhost";
            config["RabbitMQ.VirtualHost"] = "Test.SDRN.Control";
            config["RabbitMQ.User"] = "guest";
            config["RabbitMQ.Password"] = "guest";

            config["SDRN.ApiVersion"] = "2.0";

            config["SDRN.Server.Instance"] = "SDRNSV-SBD12-A00-5244";
            config["SDRN.Server.QueueNamePart"] = "Q.SDRN.Server";

            config["SDRN.Device.SensorTechId"] = "Atdi.Sdrn.Device.Client.API";
            config["SDRN.Device.Exchange"] = "EX.SDRN.Device";
            config["SDRN.Device.QueueNamePart"] = "Q.SDRN.Device";
            config["SDRN.Device.MessagesBindings"] = "{messageType=RegisterSensor, routingKey=#01};{messageType=SendCommandResult, routingKey=#02};{messageType=SendMeasResults, routingKey=#03};{messageType=SendEntity, routingKey=#04};{messageType=SendEntityPart, routingKey=#05};{messageType=UpdateSensor, routingKey=#06}";
            config["SDRN.MessageConvertor.UseEncryption"] = "false";
            config["SDRN.MessageConvertor.UseCompression"] = "false";

            return config;
        }

        static MeasResults[] LoadFromFiles(string folder)
        {
            var files = System.IO.Directory.GetFiles(folder, "*.json");
            var result = new MeasResults[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var json = System.IO.File.ReadAllText(file);

                var body = JsonConvert.DeserializeObject<JsonData>(json);
                result[i] = JsonConvert.DeserializeObject<MeasResults>(body.JsonBody);
            }
            return result;
        }
        #region Monitoring Station
        static MeasResults BuildTestMeasResultsMonitoringStations()
        {
            var result = new MeasResults
            {
                TaskId = "",
                Measured = DateTime.Now,
                Measurement = DataModels.Sdrns.MeasurementType.MonitoringStations,
                ResultId = $"Client result ID: {Guid.NewGuid()}",
                StartTime = DateTime.Today,
                StopTime = DateTime.Today,
                Status = "A",
                ScansNumber = 1000,
                SwNumber = 9000,
                StationResults = BuildStationResults(10)
            };
            return result;
        }
        static StationMeasResult[] BuildStationResults(int count)
        {
            var result = new StationMeasResult[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = BuildStationResult(i);
            }
            return result;
        }
        static StationMeasResult BuildStationResult(int index)
        {
            return new StationMeasResult
            {
                Status = "A",
                Standard = "Standard",
                RealGlobalSid = $"RGSID-{index:D4}",
                TaskGlobalSid = $"TGSID-{index:D4}",
                SectorId = index.ToString(),
                StationId = index.ToString(),
                LevelResults = BuildLevelResults(400),
                Bearings = BuildBearings(400),
                GeneralResult = new GeneralMeasResult
                {
                    CentralFrequency_MHz = 5000,
                    CentralFrequencyMeas_MHz = 6000,
                    LevelsSpectrum_dBm = new float[300],
                    BandwidthResult = new BandwidthMeasResult
                    {
                        Bandwidth_kHz = 120,
                        MarkerIndex = 25,
                        T1 = 1,
                        T2 = 2,
                        TraceCount = 123,
                        СorrectnessEstimations = false
                    },
                    MeasDuration_sec = 100,
                    MeasFinishTime = DateTime.Now,
                    MeasStartTime = DateTime.Today,
                    OffsetFrequency_mk = 455,
                    RBW_kHz = 65665,
                    SpectrumStartFreq_MHz = 5454,
                    SpectrumSteps_kHz = 25,
                    VBW_kHz = 55,

                }
            };
        }
        #endregion
        static LevelMeasResult[] BuildLevelResults(int count)
        {
            var result = new LevelMeasResult[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new LevelMeasResult
                {
                    DifferenceTimeStamp_ns = 1000,
                    Level_dBm = 45,
                    Level_dBmkVm = 12,
                    MeasurementTime = DateTime.Now
                };
            }
            return result;
        }
        static DirectionFindingData[] BuildBearings(int count)
        {
            var result = new DirectionFindingData[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new DirectionFindingData
                {
                    AntennaAzimut = 100,
                    Bandwidth_kHz = 200,
                    Bearing = 150,
                    CentralFrequency_MHz = 350,
                    Quality = 12,
                    Level_dBm = 45,
                    Level_dBmkVm = 12,
                    MeasurementTime = DateTime.Now
                };
            }
            return result;
        }

        static MeasResults BuildTestMeasResultsSignaling()
        {
            var c = 10; // 50;
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
                Measurement = Atdi.DataModels.Sdrns.MeasurementType.Signaling,
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
