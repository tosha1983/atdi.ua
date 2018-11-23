using System;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using System.Configuration;
using Atdi.SDNRS.AppServer.BusManager;
using XMLLibrary;
using Atdi.SDNRS.AppServer.Sheduler;
using Atdi.Modules.Licensing;
using Atdi.Platform.Cryptography;
using Atdi.Platform.AppComponent;
using Atdi.AppServer.Contracts.Sdrns;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Atdi.AppServer.ConfigurationSdrnController
{
    public class ConfigurationSdrnController : IAppServerComponent 
    {
        private readonly string _name;
        private ILogger _logger;
        private ConfigurationRabbitOptions _configurationRabbitOptions;
        private Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor _classDBGetSensor;
        private Atdi.AppServer.AppService.SdrnsControllerv2_0.OracleDataAccess _oracleDataAccess;
        private ShedulerUpMeasSDRResults Sc_Up_Meas_SDR;
        private ShedulerCheckActivitySensor CheckActivitySensor;
        private ShedulerGetMeasTask getMeasTask;
        private ShedulerCheckStart Quartz;
        public static List<SensorActivity> listSensorActivity = new List<SensorActivity>();

        public ConfigurationSdrnController()
        {
            this._name = "ConfigurationSdrnControllerComponent";
        }

        AppServerComponentType IAppServerComponent.Type => AppServerComponentType.AppService;

        string IAppServerComponent.Name => this._name;


        void IAppServerComponent.Activate()
        {
            try
            {

             /*
               GlobalInit.Initialization();
               Configuration conf = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
               InitConnectionString.oraDbString = ConfigurationManager.ConnectionStrings["ORACLE_DB_ICSM_ConnectionString"].ConnectionString;
               BaseXMLConfiguration xml_con = new BaseXMLConfiguration();
               List<Atdi.AppServer.Contracts.Sdrns.MeasSdrResults> dyn2 = new List<MeasSdrResults>();
               List<Atdi.AppServer.Contracts.Sdrns.MeasSdrResults> dyn = (List<Atdi.AppServer.Contracts.Sdrns.MeasSdrResults>)JsonConvert.DeserializeObject(System.IO.File.ReadAllText("C:\\Projects\\_rabbit_queue_message_2018-Oct-24_1648278085.json"), typeof(List<Atdi.AppServer.Contracts.Sdrns.MeasSdrResults>));
               BusManager<List<Atdi.AppServer.Contracts.Sdrns.MeasSdrResults>> ressd = new BusManager<List<Atdi.AppServer.Contracts.Sdrns.MeasSdrResults>>();
               for (int i = 0; i < 1; i++)
               {
                   dyn2.AddRange(dyn);
               }
                
               if (ressd.SendDataObject(dyn2, "TESTMMSRSR-021"))
               {
                    dyn2.Clear();
                    dyn.Clear();
               }
               */

                
                Configuration conf = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                InitConnectionString.oraDbString = ConfigurationManager.ConnectionStrings["ORACLE_DB_ICSM_ConnectionString"].ConnectionString;
                _oracleDataAccess.OpenConnection(InitConnectionString.oraDbString);
                Atdi.Oracle.DataAccess.OracleDataAccess oracleDataAccess = new Atdi.Oracle.DataAccess.OracleDataAccess();
                oracleDataAccess.OpenConnection(InitConnectionString.oraDbString);

                DateTime? CurrDate = _oracleDataAccess.GetSystemDate();
                var licenseServerFileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+ @"\License\" +ConfigurationManager.AppSettings["LicenseServer.FileName"].ToString();
                var licenseDeviceFileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\License\" + ConfigurationManager.AppSettings["LicenseDevice.FileName"].ToString();

                if (System.IO.File.Exists(licenseServerFileName))
                {
                    var productKey = Atdi.Platform.Cryptography.Encryptor.DecryptStringAES(ConfigurationManager.AppSettings["LicenseServer.ProductKey"].ToString(), "Atdi.AppServer.AppService.SdrnsController");
                    var ownerId = Atdi.Platform.Cryptography.Encryptor.DecryptStringAES(ConfigurationManager.AppSettings["LicenseServer.OwnerId"], "Atdi.AppServer.AppService.SdrnsController");
                    var verificationData = new VerificationData
                    {
                        OwnerId = ownerId,
                        ProductName = "ICS Control Server",
                        ProductKey = productKey,
                        LicenseType = "ServerLicense",
                        Date = CurrDate.Value
                    };

                    var licenseBody = System.IO.File.ReadAllBytes(licenseServerFileName);
                    var verResult = LicenseVerifier.Verify(verificationData, licenseBody);
                    if (verResult != null)
                    {
                        if (!string.IsNullOrEmpty(verResult.Instance))
                        {
                            var lstAllSensors = _classDBGetSensor.LoadObjectAllSensorAPI1_0();
                            foreach (var c in lstAllSensors)
                            {
                                listSensorActivity.Add(new SensorActivity(c));
                            }
                            _configurationRabbitOptions.CreateChannelsAndQueues(_classDBGetSensor.LoadObjectAllSensorAPI2_0());
                            BaseXMLConfiguration xml_conf = new BaseXMLConfiguration();
                            GlobalInit.Initialization();
                            var productK = Atdi.Platform.Cryptography.Encryptor.DecryptStringAES(ConfigurationManager.AppSettings["LicenseDevice.ProductKey"].ToString(),"Atdi.WcfServices.Sdrn.Device");
                            var ownerI = Atdi.Platform.Cryptography.Encryptor.DecryptStringAES(ConfigurationManager.AppSettings["LicenseDevice.OwnerId"].ToString(), "Atdi.WcfServices.Sdrn.Device");
                            var licenseFile = licenseDeviceFileName;
                            // зашит в код
                            var verificationD = new VerificationData
                            {
                                OwnerId = ownerI,
                                ProductName = "ICS Control Device",
                                ProductKey = productK,
                                LicenseType = "DeviceLicense",
                                Date = DateTime.Now
                            };

                            if (System.IO.File.Exists(licenseFile))
                            {
                                var licenseB = System.IO.File.ReadAllBytes(licenseFile);
                                var verRes = LicenseVerifier.Verify(verificationD, licenseB);
                                if (verRes != null)
                                {
                                    if (!string.IsNullOrEmpty(verRes.Instance))
                                    {
                                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\License");
                                        System.IO.FileInfo[] list = di.GetFiles();
                                        for (int i = 0; i < list.Length; i++)
                                        {
                                            if (list[i].Extension.ToLower() == ".xml")
                                            {
                                                XmlReaderStruct structXml = XMLReader.GetXmlSettings(list[i].FullName);
                                                if ((structXml._OwnerId == ownerI) && (structXml._ProductKey == productK))
                                                {
                                                    BusManager<Atdi.AppServer.Contracts.Sdrns.Sensor> sens = new BusManager<Contracts.Sdrns.Sensor>();
                                                    if (sens.SendDataObject(new Contracts.Sdrns.Sensor { Name = verRes.Instance, Administration = "UKR", Antenna = new Contracts.Sdrns.SensorAntenna(), Status = "N", DateCreated = CurrDate.Value, Equipment = new Contracts.Sdrns.SensorEquip() { TechId = structXml._SensorEquipmentTechId } }, structXml._SensorQueue))
                                                    {
                                                        if (System.IO.File.Exists(list[i].FullName))
                                                        {
                                                            System.IO.File.Delete(list[i].FullName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        _logger.Error("Error sending sensor data from registration temp file to queue 'Sensors_List' ");
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _logger.Error("Error validation license: " + licenseFile);
                                    }
                                }
                                else
                                {
                                    _logger.Error("Error validation license: "+ licenseFile);
                                }
                            }
                            else
                            {
                                _logger.Error("Not found file: "+ licenseFile);
                            }

                            
                            Sc_Up_Meas_SDR = new ShedulerUpMeasSDRResults(_logger);
                            Sc_Up_Meas_SDR.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._TimeUpdateMeasResult);
                            CheckActivitySensor = new ShedulerCheckActivitySensor(_logger);
                            CheckActivitySensor.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._RescanActivitySensor);
                            getMeasTask = new ShedulerGetMeasTask(this._logger); getMeasTask.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._ScanMeasTasks);
                            Quartz = new ShedulerCheckStart(this._logger);
                            Quartz.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._ReloadStart);

                        }
                        else
                        {
                            _logger.Error("Error validation license" + licenseServerFileName);
                        }
                    }
                    else
                    {
                        _logger.Error("Error validation license: " + licenseServerFileName);
                    }

                }
                else
                {
                    _logger.Error(string.Format("Not found {0} file", licenseServerFileName));
                }
            
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        void IAppServerComponent.Deactivate()
        {
            _configurationRabbitOptions.UnBindConcumers();
            ShedulerUpMeasSDRResults.DisposeSheduler();
            ShedulerCheckActivitySensor.DisposeSheduler();
            ShedulerGetMeasTask.DisposeSheduler();
            ShedulerCheckStart.DisposeSheduler();
            _oracleDataAccess.CloseConnection();
        }



        void IAppServerComponent.Install(IWindsorContainer container, IAppServerContext serverContext)
        {
            this._logger = container.Resolve<ILogger>();
            container.Register(
                   Component.For<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor>()
                       .ImplementedBy<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor>()
                       .LifeStyle.Pooled
               );
            container.Register(
                  Component.For<Atdi.AppServer.AppService.SdrnsControllerv2_0.OracleDataAccess>()
                      .ImplementedBy<Atdi.AppServer.AppService.SdrnsControllerv2_0.OracleDataAccess>()
                      .LifeStyle.Pooled
              );
            container.Register(
                 Component.For<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassesDBGetResult>()
                     .ImplementedBy<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassesDBGetResult>()
                     .LifeStyle.Pooled
             );

            container.Register(
                 Component.For<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassesDBGetTasks>()
                     .ImplementedBy<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassesDBGetTasks>()
                     .LifeStyle.Pooled
             );

            container.Register(
                Component.For<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassConvertTasks>()
                    .ImplementedBy<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassConvertTasks>()
                    .LifeStyle.Pooled
            );

            container.Register(
             Component.For<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassConvertToSDRResults>()
                 .ImplementedBy<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassConvertToSDRResults>()
                 .LifeStyle.Pooled
            );

             container.Register(
             Component.For<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassSDRResults>()
                 .ImplementedBy<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassSDRResults>()
                 .LifeStyle.Pooled
            );

            container.Register(
            Component.For<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBEntity>()
                .ImplementedBy<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBEntity>()
                .LifeStyle.Pooled
           );
           this._oracleDataAccess = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.OracleDataAccess>();
           this._classDBGetSensor = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor>();
           this._configurationRabbitOptions = new ConfigurationRabbitOptions(container, this._logger);
        }

        void IAppServerComponent.Uninstall(IWindsorContainer container, IAppServerContext serverContext)
        {
            _logger.Trace("Component RunServiceComponent: Uninstalled");
            this._configurationRabbitOptions.Dispose(true);
        }

    }
}
