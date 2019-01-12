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
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

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
               

 
                GlobalInit.Initialization();
                Configuration conf = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                InitConnectionString.oraDbString = ConfigurationManager.ConnectionStrings["ORACLE_DB_ICSM_ConnectionString"].ConnectionString;
                BaseXMLConfiguration xml_con = new BaseXMLConfiguration();
                BusManager<Atdi.DataModels.Sdrns.Device.MeasResults> ressd = new BusManager<Atdi.DataModels.Sdrns.Device.MeasResults>();
                //ressd.SendDataToQueue(System.IO.File.ReadAllText("D:\\TEMP\\1629590000.json"), "Q.SDRN.Server.[ServerSDRN01].[#03].[v2.0]");
                ressd.SendDataToQueue(System.IO.File.ReadAllText("D:\\TEMP\\1931181000.json"), "Q.SDRN.Server.[ServerSDRN01].[#03].[v2.0]");
                ressd.SendDataToQueue(System.IO.File.ReadAllText("D:\\TEMP\\1931127000.json"), "Q.SDRN.Server.[ServerSDRN01].[#03].[v2.0]");
                //ressd.SendDataToQueue(System.IO.File.ReadAllText("D:\\TEMP\\1931308000.json"), "Q.SDRN.Server.[ServerSDRN01].[#03].[v2.0]");
                //ressd.SendDataToQueue(System.IO.File.ReadAllText("D:\\TEMP\\1931495000.json"), "Q.SDRN.Server.[ServerSDRN01].[#03].[v2.0]");
                // var T = ressd.GetDataObject<Atdi.DataModels.Sdrns.Device.MeasResults>("Q.SDRN.Server.[ServerSDRN01].[#03].[v2.0]");


                //BusManager<List<MeasSdrResults>> ressd = new BusManager<List<MeasSdrResults>>();
                //List<Atdi.AppServer.Contracts.Sdrns.MeasSdrResults> dyn = (List<Atdi.AppServer.Contracts.Sdrns.MeasSdrResults>)JsonConvert.DeserializeObject(System.IO.File.ReadAllText("C:\\TEMP\\_rabbit_queue_message_2018-Oct-12_1716306813.json"), typeof(List<Atdi.AppServer.Contracts.Sdrns.MeasSdrResults>));
                //ressd.SendDataObject(dyn, "MEAS_SDR_RESULTS_Main_List_APPServer_INS-DV-2018-TESTMMS-02");
  
  */ 
                Configuration conf = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                InitConnectionString.oraDbString = ConfigurationManager.ConnectionStrings["ORACLE_DB_ICSM_ConnectionString"].ConnectionString;

                _oracleDataAccess.OpenConnection(InitConnectionString.oraDbString);
                Atdi.Oracle.DataAccess.OracleDataAccess oracleDataAccess = new Atdi.Oracle.DataAccess.OracleDataAccess();
                oracleDataAccess.OpenConnection(InitConnectionString.oraDbString);

                DateTime? CurrDate = _oracleDataAccess.GetSystemDate();
                var licenseServerFileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+ @"\License\" +ConfigurationManager.AppSettings["LicenseServer.FileName"].ToString();
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

                            BaseXMLConfiguration xml_conf = new BaseXMLConfiguration();
                            GlobalInit.Initialization();
                            _configurationRabbitOptions.CreateChannelsAndQueues(_classDBGetSensor.LoadObjectAllSensorAPI2_0());
                            ClassDBGetSensor gsd = new ClassDBGetSensor(_logger);
                            ConfigurationRabbitOptions.listAllSensors = gsd.LoadObjectAllSensor();
                            gsd.Dispose();
                            Activity activity = new Activity(_logger);
                            //Sc_Up_Meas_SDR = new ShedulerUpMeasSDRResults(_logger);
                            //Sc_Up_Meas_SDR.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._TimeUpdateMeasResult);
                            //CheckActivitySensor = new ShedulerCheckActivitySensor(_logger);
                            //CheckActivitySensor.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._RescanActivitySensor);
                            //getMeasTask = new ShedulerGetMeasTask(this._logger); getMeasTask.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._ScanMeasTasks);
                            //Quartz = new ShedulerCheckStart(this._logger);
                            //Quartz.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._ReloadStart);

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
