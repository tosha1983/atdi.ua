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
                Configuration conf = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                InitConnectionString.oraDbString = ConfigurationManager.ConnectionStrings["ORACLE_DB_ICSM_ConnectionString"].ConnectionString;
                _oracleDataAccess.OpenConnection(InitConnectionString.oraDbString);
                DateTime? CurrDate = _oracleDataAccess.GetSystemDate();
                var licenseFileName = ConfigurationManager.AppSettings["License.FileName"].ToString();
                if (System.IO.File.Exists(licenseFileName))
                {
                    var productKey = Atdi.Platform.Cryptography.Encryptor.DecryptStringAES(ConfigurationManager.AppSettings["License.ProductKey"].ToString(), "Atdi.AppServer.AppService.SdrnsController");
                    var ownerId = Atdi.Platform.Cryptography.Encryptor.DecryptStringAES(ConfigurationManager.AppSettings["License.OwnerId"], "Atdi.AppServer.AppService.SdrnsController");
                    var verificationData = new VerificationData
                    {
                        OwnerId = ownerId,
                        ProductName = "ICS Control Server",
                        ProductKey = productKey,
                        LicenseType = "ServerLicense",
                        Date = CurrDate.Value
                    };

                    var licenseBody = System.IO.File.ReadAllBytes(licenseFileName);
                    var verResult = LicenseVerifier.Verify(verificationData, licenseBody);
                    if (verResult != null)
                    {
                        if (!string.IsNullOrEmpty(verResult.Instance))
                        {
                            _configurationRabbitOptions.CreateChannelsAndQueues(_classDBGetSensor.LoadObjectAllSensor());
                            BaseXMLConfiguration xml_conf = new BaseXMLConfiguration();
                            GlobalInit.Initialization();
                            Sc_Up_Meas_SDR = new ShedulerUpMeasSDRResults(_logger);
                            Sc_Up_Meas_SDR.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._TimeUpdateMeasResult);
                            CheckActivitySensor = new ShedulerCheckActivitySensor(_logger);
                            CheckActivitySensor.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._RescanActivitySensor);
                            getMeasTask = new ShedulerGetMeasTask(this._logger); getMeasTask.ShedulerRepeatStart(20);
                            Quartz = new ShedulerCheckStart(this._logger);
                            Quartz.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._ReloadStart);
                        }
                        else
                        {
                            _logger.Error("Error validation license");
                        }
                    }
                }
                else
                {
                    _logger.Error("Not found SDRN.Server.v2.0.lic file");
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
            Sc_Up_Meas_SDR.Dispose();
            ShedulerUpMeasSDRResults.DisposeSheduler();
            ShedulerCheckActivitySensor.DisposeSheduler();
            ShedulerGetMeasTask.DisposeSheduler();
            ShedulerCheckStart.DisposeSheduler();
            CheckActivitySensor.Dispose();
            getMeasTask.Dispose();
            Quartz.Dispose();
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
