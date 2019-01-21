using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dev = Atdi.DataModels.Sdrns.Device;

namespace ControlU.DB
{
    class ClassTest
    {
        public Atdi.DataModels.Sdrns.Device.MeasTask res()
        {
            //Atdi.DataModels.Sdrns.Device.Sensor
            #region
            Atdi.DataModels.Sdrns.Device.MeasResults r = new Atdi.DataModels.Sdrns.Device.MeasResults()
            {
                ResultId = "",//непонятяно
                Status = "", //??
                SwNumber = 0, //
                TaskId = "",//ОО стринг??
                Measured = DateTime.Now,//время отправления
                Routes = new Atdi.DataModels.Sdrns.Device.Route[]
                 {
                    new Atdi.DataModels.Sdrns.Device.Route()
                    {
                        RouteId = "",///включил изм / выкл изм плюсовать одын
                        RoutePoints = new Atdi.DataModels.Sdrns.Device.RoutePoint[]
                        {
                            new Atdi.DataModels.Sdrns.Device.RoutePoint()
                            {
                                AGL = 0, //высота
                                ASL = 0, //непонятяно
                                Lat = 0, //пнятно
                                Lon = 0, //пнятно
                                PointStayType = Atdi.DataModels.Sdrns.PointStayType.InMove, //смыл понятен, зачем и как определить нет
                                StartTime = DateTime.Now, //время  
                                FinishTime = DateTime.Now, //время чегото
                            }
                        }
                    }
                 },
                StationResults = new Atdi.DataModels.Sdrns.Device.StationMeasResult[]
                 {
                    new Atdi.DataModels.Sdrns.Device.StationMeasResult()
                    {
                        #region
                        StationId = "",//ОО стринг??
                        SectorId = "",//ОО стринг??
                        TaskGlobalSid = "", // по идее из бд
                        RealGlobalSid = "", //from radio
                        Standard = "", // надо определить названия
                        Status = "",//тоже определить все случаю
                        GeneralResult = new Atdi.DataModels.Sdrns.Device.GeneralMeasResult()
                        {
                            #region
                            CentralFrequencyMeas_MHz = 0,
                            CentralFrequency_MHz = 0,
                            LevelsSpectrum_dBm = new float[] { },
                            MeasDuration_sec = 0,//чего не в BandwidthResult
                            MeasStartTime = DateTime.Now,
                            MeasFinishTime = DateTime.Now,

                            OffsetFrequency_mk = 0,
                            RBW_kHz = 0,
                            VBW_kHz = 0,
                            SpectrumStartFreq_MHz = 0,
                            SpectrumSteps_kHz = 0,
                            BandwidthResult = new Atdi.DataModels.Sdrns.Device.BandwidthMeasResult()
                            {
                                Bandwidth_kHz = 0,//пнятно
                                MarkerIndex = 0,//пнятно
                                T1 = 0,//пнятно
                                T2 = 0,//пнятно
                                TraceCount = 0,//количевство учтенных трейсов
                                СorrectnessEstimations = false,//пнятно смог померить по -30 тру, не смог аля CDMA false
                            },
                            BWMask = new Atdi.DataModels.Sdrns.Device.ElementsMask[]
                            {
                                new Atdi.DataModels.Sdrns.Device.ElementsMask()
                                {
                                    BW_kHz = 0,
                                    Level_dB = 0
                                }
                            },
                            StationSysInfo = new Atdi.DataModels.Sdrns.Device.StationSysInfo()
                            {
                                BandWidth = 0,//а BandwidthResult зачем
                                BaseID = 0,//
                                //ааа ага, зашибись конечно но чето придумаю
                                Location = new Atdi.DataModels.Sdrns.GeoLocation(),
                                Freq = 0,
                            }
                            #endregion
                        },
                        LevelResults = new Atdi.DataModels.Sdrns.Device.LevelMeasResult[]
                        {
                            #region
                            new Atdi.DataModels.Sdrns.Device.LevelMeasResult()
                            {
                                DifferenceTimeStamp_ns = 0,
                                Level_dBm = 0,
                                Level_dBmkVm = 0,
                                Location = new Atdi.DataModels.Sdrns.GeoLocation()
                                {
                                    AGL = 0,
                                    ASL = 0,
                                    Lat = 0,
                                    Lon = 0,
                                },
                                MeasurementTime = DateTime.Now,
                            }
                            #endregion
                        },
                        #endregion
                    }
                },
            };
            #endregion

            #region
            Atdi.DataModels.Sdrns.Device.MeasTask t = new Atdi.DataModels.Sdrns.Device.MeasTask()
            {
                TaskId = "12",
                EquipmentTechId = "13254523",
                MobEqipmentMeasurements = new Atdi.DataModels.Sdrns.MeasurementType[]
                {
                    Atdi.DataModels.Sdrns.MeasurementType.MonitoringStations
                },
                Priority = 1,//??
                ScanPerTaskNumber = 400, // в ScanParameters
                SdrnServer = "dfbhs", //??
                SensorName = "fgjhn",//??
                StartTime = DateTime.Now,
                StopTime = DateTime.Now,
                Status = "sgn",//??
                ScanParameters = new Atdi.DataModels.Sdrns.Device.StandardScanParameter[]//где привязка к Stations?? id может
                {
                    new Atdi.DataModels.Sdrns.Device.StandardScanParameter()
                    {
                        DetectionLevel_dBm = -100,
                        MaxFrequencyRelativeOffset_mk = 20,
                        MaxPermissionBW_kHz = 4850,
                        Standard = "UMTS",
                        XdBLevel_dB = 30,
                        DeviceParam = new Atdi.DataModels.Sdrns.Device.DeviceMeasParam()
                        {
                            DetectType = Atdi.DataModels.Sdrns.DetectingType.MaxPeak,
                            MeasTime_sec = 100,
                            Preamplification_dB = 0,
                            RBW_kHz = 10,
                            VBW_kHz = 10,
                            RefLevel_dBm = 0,
                            RfAttenuation_dB = 0,
                            ScanBW_kHz = 5000//10000,// т.к. есть LTE и ШПС 
                        },
                    }
                },
                Stations = new Atdi.DataModels.Sdrns.Device.MeasuredStation[]
                {
                    new Atdi.DataModels.Sdrns.Device.MeasuredStation()
                    {
                        GlobalSid = "255 5 00000 12345", //GCID
                        OwnerGlobalSid = "255 5 13245 12345",//неясно
                        Standard = "UMTS",
                        StationId = "dfasdfasf",//Int??
                        Status = "1",
                        License = new Atdi.DataModels.Sdrns.Device.StationLicenseInfo()
                        {
                            Name = "sgvfvasdf",
                            StartDate = DateTime.Now,
                            CloseDate = DateTime.Now,
                            EndDate = DateTime.Now,
                            IcsmId = 1,
                        },
                        Owner = new Atdi.DataModels.Sdrns.Device.StationOwner()
                        {
                            Address = "fgvas",
                            Code = "ergvb",
                            Id = 0,//у харькова натыкался что гдето в этих полях вместо цифр написан УДЦР прописью кирилицей, какая то релейка вроде
                            OKPO = "uyuyuykyujmn",
                            OwnerName = "djnjdf",
                            Zip = "5464"
                        },
                        Sectors = new Atdi.DataModels.Sdrns.Device.StationSector[]
                        {
                            new Atdi.DataModels.Sdrns.Device.StationSector()
                            {
                                AGL = 20,
                                Azimuth = 200,
                                BW_kHz = 5000,
                                ClassEmission = "dfbsfb",
                                EIRP_dBm = 100,
                                SectorId = "fbnd xfb ",
                                Frequencies = new Atdi.DataModels.Sdrns.Device.SectorFrequency[]
                                {
                                    new Atdi.DataModels.Sdrns.Device.SectorFrequency()
                                    {
                                        ChannelNumber = 200,
                                        Frequency_MHz = 2142.4m,
                                        Id = 441,
                                        PlanId = 41,
                                    }
                                },
                                BWMask = new Atdi.DataModels.Sdrns.Device.ElementsMask[]
                                {
                                    new Atdi.DataModels.Sdrns.Device.ElementsMask()
                                    {
                                        BW_kHz = 0,
                                        Level_dB = 0
                                    }
                                },
                            }
                        },
                        Site = new Atdi.DataModels.Sdrns.Device.StationSite()
                        {
                            Adress = "xvbasdb",
                            Lat = 10.67,
                            Lon = 20.432,
                            Region = "zd\b"
                        }
                    }
                }
            };
            #endregion

            var sensor = new dev.Sensor
            {
                Administration = "User?",
                BiuseDate = DateTime.Now, //хз
                Created = DateTime.Now, //хз а смысл
                CreatedBy = "Выбранным юзером",//хз
                CustDate1 = DateTime.Now, //хз
                CustNbr1 = 0, //хз
                CustTxt1 = "хз", //хз
                EouseDate = DateTime.Now,//зчем конечная дата если есть лицензия для этого, которая с обоих сторо
                NetworkId = "", //хз тайные знания
                Polygon = new dev.SensorPolygon(),//хз тайные знания
                Remark = "",// какието коментарии, и чего там писать то юзеру
                RxLoss = 0,// нууууууууууууууууу
                Status = "",//хз тайные знания
                StepMeasTime = 0,//хз тайные знания
                Type = "",//теоретически понятно, но что писать хз
                

                Antenna = new dev.SensorAntenna
                {
                    AddLoss = 0,
                    

                },
                Equipment = new dev.SensorEquipment
                {
                    TechId = ControlU.MainWindow.db_v2.ATDIConnectionData_Selsected.sensor_equipment_tech_id,
                    
                },
                //Name = NameSensor

            };
            return t;
        }
    }
}
