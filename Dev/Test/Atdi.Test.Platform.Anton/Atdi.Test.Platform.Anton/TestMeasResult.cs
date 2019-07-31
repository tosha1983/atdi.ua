using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Platform
{
    class TestMeasResult
    {
        public MeasResults BuildTestMeasResults()
        {
            var c = 20;
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
                Measurement = Atdi.DataModels.Sdrns.MeasurementType.MonitoringStations,
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
                TaskId = "SDRN.SubTaskSensorId.1"//Guid.NewGuid().ToString()
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
                        CentralFrequencyMeas_MHz = 2222,
                        LevelsSpectrum_dBm = BuildTestLevelsSpectrum(levelsSpectrumCount),
                        CentralFrequency_MHz = 1111,
                        MeasDuration_sec = double.MaxValue,
                        MeasFinishTime = DateTime.Now,
                        MeasStartTime = DateTime.Now,
                        OffsetFrequency_mk = double.MaxValue,
                        RBW_kHz = double.MinValue,
                        SpectrumStartFreq_MHz = 3333,
                        SpectrumSteps_kHz = 4444,
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
                    MeasurementTime = DateTime.Now,
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
                            StartTime = DateTime.Now,
                            FinishTime = DateTime.Now,
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                            StartTime = DateTime.Now,
                            FinishTime = DateTime.Now,
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                            StartTime = DateTime.Now,
                            FinishTime = DateTime.Now,
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                            StartTime = DateTime.Now,
                            FinishTime = DateTime.Now,
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                            StartTime = DateTime.Now,
                            FinishTime = DateTime.Now,
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                            StartTime = DateTime.Now,
                            FinishTime = DateTime.Now,
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                            StartTime = DateTime.Now,
                            FinishTime = DateTime.Now,
                        },
                        new RoutePoint
                        {
                            AGL = r.NextDouble(),
                            ASL = r.NextDouble(),
                            Lat = r.NextDouble(),
                            Lon = r.NextDouble(),
                            StartTime = DateTime.Now,
                            FinishTime = DateTime.Now,
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
                        new WorkTime{HitCount = 1, StartEmitting = DateTime.Now, StopEmitting = DateTime.Now },
                        new WorkTime{HitCount = 1, StartEmitting = DateTime.Now, StopEmitting = DateTime.Now },
                        new WorkTime{HitCount = 1, StartEmitting = DateTime.Now, StopEmitting = DateTime.Now }
                    },
                    SysInfos = BuildTestSignalingSysInfo(count)
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
        static SignalingSysInfo[] BuildTestSignalingSysInfo(int count)
        {
            var r = new Random();
            var result = new SignalingSysInfo[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new SignalingSysInfo
                {
                    BandWidth_Hz = r.NextDouble(),
                    BSIC = r.Next(),
                    ChannelNumber = r.Next(),
                    CID = r.Next(),
                    CtoI = r.NextDouble(),
                    Freq_Hz = (decimal)r.NextDouble(),
                    LAC = r.Next(),
                    Level_dBm = r.NextDouble(),
                    MCC = r.Next(),
                    MNC = r.Next(),
                    Power = r.NextDouble(),
                    RNC = r.Next(),
                    Standard = "111",
                    WorkTimes = new WorkTime[]
                    {
                        new WorkTime{HitCount = 1, StartEmitting = DateTime.Now, StopEmitting = DateTime.Now },
                        new WorkTime{HitCount = 1, StartEmitting = DateTime.Now, StopEmitting = DateTime.Now },
                        new WorkTime{HitCount = 1, StartEmitting = DateTime.Now, StopEmitting = DateTime.Now }
                    }
                };
            }
            return result;
        }
    }
}
