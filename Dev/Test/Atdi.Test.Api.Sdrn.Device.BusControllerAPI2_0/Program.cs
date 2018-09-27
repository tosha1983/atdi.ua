using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Api.Sdrn.Device.BusController;

using DM = Atdi.DataModels.Sdrns.Device;


namespace Atdi.Test.Api.Sdrn.Device.BusControllerAPI2_0
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
        }

        static void Test()
        {

            Console.WriteLine($"Press [Enter] to start testing ...");
            Console.ReadLine();

            // 1. Создаем фабрику шлюзов к шине обмена сообщениями
            var busGateFactory = BusGateFactory.Create();

            // Из фабрики запрашиваем новый объект для описания конфигурации
            var config = busGateFactory.CreateConfig();

            // все параметрі уже указіваются в расшифрованном виде
            config["License.FileName"] = "LIC-DBD12-A00-187.SENSOR-DBD12-A00-1280.lic";
            config["License.OwnerId"] = "OID-BD12-A00-N00";
            config["License.ProductKey"] = "0ZB0-DVZR-ATI1-WIHB-NC1B";

            config["RabbitMQ.Host"] = "10.1.2.129";
            config["RabbitMQ.User"] = "SDR_Client";
            config["RabbitMQ.Password"] = "32Xr567";

            config["SDRN.ApiVersion"] = "2.0";

            config["SDRN.Server.Instance"] = "ServerSDRN01";
            config["SDRN.Server.QueueNamePart"] = "Q.SDRN.Server";

            // будет взят из лицензии - изменить будет нельзя
            //config["SDRN.Device.SensorName"] = "my SENSOR-DBD12-A00-1280";
            config["SDRN.Device.SensorTechId"] = "SomeSensor";

            config["SDRN.Device.Exchange"] = "EX.SDRN.Device";
            config["SDRN.Device.QueueNamePart"] = "Q.SDRN.Device";
            config["SDRN.Device.MessagesBindings"] = "{messageType=RegisterSensor, routingKey=#01};{messageType=SendCommandResult, routingKey=#02};{messageType=SendMeasResults, routingKey=#03};{messageType=SendEntity, routingKey=#04};{messageType=SendEntityPart, routingKey=#05};{messageType=UpdateSensor, routingKey=#06}";


            // Шифрование и компресия сообщений
            config["SDRN.MessageConvertor.UseEncryption"] = "true";
            config["SDRN.MessageConvertor.UseСompression"] = "true";

            // создание гейта к шине
            // Важно гейт = один сенсор

            // если нужно отследить события работы гейта, следует подготовить объект реализующий интерфейс
            IBusEventObserver busObserver = new MyEventObserver(); // инстанцирование своего объекта
            var busGate = busGateFactory.CreateGate("someTag", config, busObserver);
            var publisher = busGate.CreatePublisher("someTag", busObserver);
            var messageDispatcher = busGate.CreateDispatcher("Main", busObserver);
            /*
            // некий код подготовки результатов
            var measResult = new DM.MeasResults
            {
                TaskId = "SomeTask"
            };

            var msgToken = publisher.Send("SendMeasResults", measResult);
            */
            /*
            var sensor = new DM.Sensor
            {
                Antenna = new DM.SensorAntenna
                {
                    AddLoss = 0
                },
                Equipment = new DM.SensorEquipment
                {
                    TechId = config["SDRN.Device.SensorTechId"].ToString()
                },
                Name = "SENSOR-DBD12-A00-1280"

            };
            var msgToken = publisher.Send("RegisterSensor", sensor);

    */
            //messageDispatcher.RegistryHandler(new SendRegistrationResultHandler(busGate));
            
            
            messageDispatcher.RegistryHandler<DM.SensorRegistrationResult>("SendRegistrationResult", m =>
            {
                // тут код обработки сообщения с типом "SendCommand" и входящем объекте типа "DeviceCommand"
                try
                {
                    if (m.Data.Status == "Success")
                    {
                        // подтверждаем обработку
                        m.Result = MessageHandlingResult.Confirmed;
                    }
                    else
                    {
                        m.Result = MessageHandlingResult.Reject;
                    }
                }
                catch (Exception e)
                {
                    // в даннмо случаи от ряда факторов мы можем переправить сообщение в очередь содержаших ошиюочные сообщения
                    m.Result = MessageHandlingResult.Error;
                    m.ReasonFailure = e.Message;
                }

            });
            messageDispatcher.Activate();


            publisher.Dispose();
            messageDispatcher.Dispose();
            busGate.Dispose();
        }

        static void Test1()
        {
            Console.WriteLine($"Press [Enter] to start testing ...");
            Console.ReadLine();

            var gateFactory = BusGateFactory.Create();
            var gate = CreateGate(gateFactory);

            var dispatcher = gate.CreateDispatcher("main");
            //dispatcher.RegistryHandler(new SendMeasTaskHandler(gate));
            //dispatcher.RegistryHandler(new SendCommandHandler(gate));
            dispatcher.RegistryHandler(new SendRegistrationResultHandler(gate));
            //dispatcher.RegistryHandler(new SendSensorUpdatingResultHandler(gate));
            dispatcher.Activate();


            var publisher = gate.CreatePublisher("main");

            //var sensor = new DM.Sensor
            //{
                //Name = "SENSOR-DBD12-A00-1280", 
                //Equipment = new DM.SensorEquipment
                //{
                    //TechId = "SomeSensor 2.3 SN:00009093"
                //}
            //};

            //for (int i = 0; i < 10000; i++)
            //{
                //publisher.Send("RegisterSensor", sensor, "Some Correlation ID #" + i.ToString());
            //}

            // обязательно все почистить
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

            config["License.FileName"] = "LIC-DBD12-A00-187.SENSOR-DBD12-A00-1280.lic";
            config["License.OwnerId"] = "OID-BD12-A00-N00";
            config["License.ProductKey"] = "0ZB0-DVZR-ATI1-WIHB-NC1B";

            config["RabbitMQ.Host"] = "10.1.2.129";
            config["RabbitMQ.User"] = "SDR_Client";
            config["RabbitMQ.Password"] = "32Xr567";

            config["SDRN.ApiVersion"] = "2.0";

            config["SDRN.Server.Instance"] = "ServerSDRN01";
            config["SDRN.Server.QueueNamePart"] = "Q.SDRN.Server";

            // будет взят из лицензии - изменить будет нельзя
            //config["SDRN.Device.SensorName"] = "my SENSOR-DBD12-A00-1280";
            config["SDRN.Device.SensorTechId"] = "SomeSensor";

            config["SDRN.Device.Exchange"] = "EX.SDRN.Device";
            config["SDRN.Device.QueueNamePart"] = "Q.SDRN.Device";
            config["SDRN.Device.MessagesBindings"] = "{messageType=RegisterSensor, routingKey=#01};{messageType=SendCommandResult, routingKey=#02};{messageType=SendMeasResults, routingKey=#03};{messageType=SendEntity, routingKey=#04};{messageType=SendEntityPart, routingKey=#05};{messageType=UpdateSensor, routingKey=#06}";


            // Шифрование и компресия сообщений
            config["SDRN.MessageConvertor.UseEncryption"] = "true";
            config["SDRN.MessageConvertor.UseСompression"] = "true";

            return config;
        }
    }
}
