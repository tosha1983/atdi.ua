using System;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Api.Sdrn.Device.BusController;
using Atdi.AppServer.Contracts.Sdrns;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Atdi.Modules.MonitoringProcess.SingleHound;


namespace Atdi.AppUnits.Sdrn.ControlA.Handlers
{
   
    public class BusManager
    {
        public static IBusGate _gate { get; set; }
        public static IMessageDispatcher _messageDispatcher { get; set; }
        public static IMessagePublisher  _messagePublisher { get; set; }
        public static Platform.Logging.ILogger _logger { get; set; }
        public static int Counter_Online { get; set; }
        public static SDRBB60C SDR { get; set; }
        public static Platform.AppComponent.IComponentConfig _configParameters { get; set; }

        public BusManager(Platform.Logging.ILogger logger, Platform.AppComponent.IComponentConfig configParameters)
        {
            SDR = new SDRBB60C();
            _configParameters = configParameters;
            _logger = logger;
            _gate = CreateGate(BusGateFactory.Create());
            _messageDispatcher = _gate.CreateDispatcher("main");
            _messagePublisher = _gate.CreatePublisher("main");
            _messageDispatcher.RegistryHandler(new SendMeasSdrTaskHandler(_gate));
            _messageDispatcher.RegistryHandler(new StopMeasSdrTaskHandler(_gate));
            _messageDispatcher.RegistryHandler(new SendCommandHandler(_gate));
            _messageDispatcher.Activate();

            Sensor sensorDeserialize = null;
            XmlSerializer sersens = new XmlSerializer(typeof(Sensor));
            var readersenss = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\sensor.xml");
            object obj = sersens.Deserialize(readersenss);
            if (obj != null)
            {
                sensorDeserialize = obj as Sensor;
            }
            if (sensorDeserialize != null)
            {
                string periodSendActivitySensor = _configParameters["PeriodSendActivitySensor"].ToString();
                int periodSendActivitySensorInt = Int32.Parse(periodSendActivitySensor);
                SensorActivity sensorActivity = new SensorActivity(periodSendActivitySensorInt);
                LoadDataMeasTask loadDataSensor = new LoadDataMeasTask();
                SensorDBExtension saveDataSensor = new SensorDBExtension();

                Task tskGPS = new Task(() =>
                {
                    GPS.Runner runGPS = new GPS.Runner();
                    runGPS.StartGPS();
                });
                tskGPS.Start();
                tskGPS.Wait();

                var loadSensor = saveDataSensor.LoadObjectSensor();
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



        static IBusGate CreateGate(IBusGateFactory gateFactory)
        {
            var gateConfig = CreateConfig(gateFactory);
            var gate = gateFactory.CreateGate("MainGate", gateConfig);
            return gate;
        }

        static IBusGateConfig CreateConfig(IBusGateFactory gateFactory)
        {
            var config = gateFactory.CreateConfig();
            config["License.FileName"] = _configParameters["License.FileName"];
            config["License.OwnerId"] = _configParameters["License.OwnerId"];
            config["License.ProductKey"] = _configParameters["License.ProductKey"];
            config["RabbitMQ.Host"] = _configParameters["RabbitMQ.Host"];
            config["RabbitMQ.User"] = _configParameters["RabbitMQ.User"]; 
            config["RabbitMQ.Password"] = _configParameters["RabbitMQ.Password"];
            config["RabbitMQ.Port"] = _configParameters["RabbitMQ.Port"];
            config["RabbitMQ.VirtualHost"] = _configParameters["RabbitMQ.VirtualHost"];
            config["SDRN.Device.SensorTechId"] = _configParameters["SDRN.Device.SensorTechId"]; 
            config["SDRN.ApiVersion"] = _configParameters["SDRN.ApiVersion"];
            config["SDRN.Server.Instance"] = _configParameters["SDRN.Server.Instance"]; 
            config["SDRN.Server.QueueNamePart"] = _configParameters["SDRN.Server.QueueNamePart"]; 
            config["SDRN.Device.Exchange"] = _configParameters["SDRN.Device.Exchange"]; 
            config["SDRN.Device.QueueNamePart"] = _configParameters["SDRN.Device.QueueNamePart"]; 
            config["SDRN.Device.MessagesBindings"] = _configParameters["SDRN.Device.MessagesBindings"];
            config["SDRN.MessageConvertor.UseEncryption"] = _configParameters["SDRN.Device.UseEncryption"];
            config["SDRN.MessageConvertor.UseСompression"] = _configParameters["SDRN.MessageConvertor.UseСompression"];
            return config;
        }
    }

   


}
