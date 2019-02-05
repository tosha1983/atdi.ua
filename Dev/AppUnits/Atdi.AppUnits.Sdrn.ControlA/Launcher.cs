using System;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Api.Sdrn.Device.BusController;
using Atdi.AppServer.Contracts.Sdrns;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Atdi.Modules.MonitoringProcess.SingleHound;
using Atdi.AppUnits.Sdrn.ControlA.Process;
using Atdi.AppUnits.Sdrn.ControlA.Handlers;
using Atdi.AppUnits.Sdrn.ControlA.ManageDB;
using Atdi.Modules.Licensing;

namespace Atdi.AppUnits.Sdrn.ControlA.Bus
{
   
    public class Launcher
    {
        public static IBusGate _gate { get; set; }
        public static IMessageDispatcher _messageDispatcher { get; set; }
        public static IMessagePublisher  _messagePublisher { get; set; }
        public static Platform.Logging.ILogger _logger { get; set; }
        public static int _counterOnline { get; set; }
        public static SDRBB60C _sdr { get; set; }

        public Launcher(Platform.Logging.ILogger logger, Platform.AppComponent.IComponentConfig configParameters)
        {
            try
            {
                _sdr = new SDRBB60C();
                _logger = logger;
                BusConfig busConfig = new BusConfig();
                IBusGateFactory gateFactory = BusGateFactory.Create();
                var gateConfig = busConfig.GetBusConfig(gateFactory, configParameters);
                _gate = gateFactory.CreateGate("MainGate", gateConfig);
                _messageDispatcher = _gate.CreateDispatcher("main");
                _messagePublisher = _gate.CreatePublisher("main");
                _messageDispatcher.RegistryHandler(new SendMeasSdrTaskHandler(_gate));
                _messageDispatcher.RegistryHandler(new StopMeasSdrTaskHandler(_gate));
                _messageDispatcher.RegistryHandler(new UpdateSensorLocationResultHandler(_gate));
                _messageDispatcher.Activate();

                SensorDb sensorDb = new SensorDb();
                Sensor sensorDeserialize = sensorDb.GetSensorFromDefaultConfigFile(); 
                if (sensorDeserialize != null)
                {
                    var licenseDeviceFileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\" + configParameters["License.FileName"].ToString();
                    if (System.IO.File.Exists(licenseDeviceFileName))
                    {
                        var productKey = configParameters["License.ProductKey"].ToString();
                        var ownerId = configParameters["License.OwnerId"].ToString();
                        var verificationData = new VerificationData
                        {
                            OwnerId = ownerId,
                            ProductName = "ICS Control Device",
                            ProductKey = productKey,
                            LicenseType = "DeviceLicense",
                            Date = DateTime.Now
                        };
                        var licenseBody = System.IO.File.ReadAllBytes(licenseDeviceFileName);
                        var verResult = LicenseVerifier.Verify(verificationData, licenseBody);
                        if (verResult != null)
                        {
                            if (!string.IsNullOrEmpty(verResult.Instance))
                            {
                                ArchiveResults archiveResults = new ArchiveResults();
                                SensorActivity sensorActivity = new SensorActivity();
                                LoadDataMeasTask loadDataSensor = new LoadDataMeasTask();
                                SensorDb saveDataSensor = new SensorDb();
                                Task tskGPS = new Task(() =>
                                {
                                    GPS.Runner runGPS = new GPS.Runner();
                                    runGPS.StartGPS();
                                });
                                tskGPS.Start();
                                tskGPS.Wait();
                                var loadSensor = saveDataSensor.LoadObjectSensor();
                                sensorDeserialize.Name = verResult.Instance;
                                sensorDeserialize.Equipment.TechId = configParameters["SDRN.Device.SensorTechId"].ToString();
                                if (loadSensor.Find(u => u.Name == sensorDeserialize.Name && u.Equipment.TechId == sensorDeserialize.Equipment.TechId) == null)
                                {
                                    saveDataSensor.CreateNewObjectSensor(sensorDeserialize);
                                    saveDataSensor.UpdateStatus();
                                    _messagePublisher.Send("RegisterSensor", sensorDeserialize);
                                }
                                _messagePublisher.Send("SendActivitySensor", sensorDeserialize);
                                Task tsk = new Task(() =>
                                {
                                    System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
                                    loadDataSensor.ProcessBB60C();
                                });
                                tsk.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Exception(Contexts.ThisComponent, Categories.BusManagerInit, Events.BusManagerInit, ex, null, null);
            }
        }
    }


}
