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

namespace Atdi.Test.Api.Sdrn.Client.Bus.Testing
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
            //while (true)
            {
                // Разкоментариваем нужную строку, в зависимости от того что мы хотим делать.
                // - Если нужно загрузить тестовый результат на основании JSON файлов из каталога - используем LoadJSONMeasResultfromFolder,
                //      при этом в самой этой функции внутри нужно указать правильный каталог
                // - Если тестовый результат описанный нами в классе TestMeasResult - используем LoadTestMeasResult
                
                LoadJSONMeasResultfromFolder();
                //LoadTestMeasResult();
            }
        }
        static void LoadJSONMeasResultfromFolder()
        {
            var gateFactory = BusGateFactory.Create();
            var gate = CreateGate(gateFactory);
            var publisher = gate.CreatePublisher("main");

            var res = LoadFromFiles(@"C:\Users\Administrator\Desktop\Upload");
            for (int i = 0; i < res.Length; i++)
            {
                var item = res[i];
                publisher.Send("SendMeasResults", item, $"MonitoringStations");
                Console.WriteLine($"TASK ID: {item.TaskId}");
            }
            Console.WriteLine($"Test finished ...");
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
        static void LoadTestMeasResult()
        {
            var gateFactory = BusGateFactory.Create();
            var gate = CreateGate(gateFactory);
            var publisher = gate.CreatePublisher("main");
            var measResultTest = new TestMeasResult();
            var measResult = measResultTest.BuildTestMeasResults();
            var count = 1; // тут можно указывать количество результатов который мы будем генерить за один запуск приложения
            for (int i = 0; i < count; i++)
            {
                publisher.Send("SendMeasResults", measResult, $"ID #{i}");
            }
            publisher.Dispose();
            Console.WriteLine($"Test finished ...");
            Console.ReadLine();
        }
        static IBusGate CreateGate(IBusGateFactory gateFactory)
        {
            var gateConfig = CreateConfig(gateFactory);
            var gate = gateFactory.CreateGate("MainGate", gateConfig);
            return gate;
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
    }
}
