using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Api.Sdrn.Device.BusController;

using DM = Atdi.DataModels.Sdrns.Device;


namespace Atdi.Test.Api.Sdrn.Device.BusController
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //Test1();
        }
        /*
        static void Test()
        {
            //var mSettiongs = new MessageConvertSettings
            //{
            //    UseСompression = true,
            //    UseEncryption = false
            //};

            //var cn = new MessageConverter(mSettiongs, new MessageObjectTypeResolver());

            //var message = new Message
            //{

            //};

            //var messageObject = new MessageObject
            //{
            //    Type = typeof(DM.DeviceCommand),
            //    Object = new DM.DeviceCommand { Command = "Test", CommandId = "123" }
            //};

            //cn.Serialize(messageObject, message);


            //var mo = cn.Deserialize(message);

            //var tt = typeof(ReceivedMessage<>);
            //var aa = new Type[] { typeof(DM.MeasTask) };
            //var gt = tt.MakeGenericType(aa);

            //var obj = Activator.CreateInstance(gt, 
            //    new object[] {
            //        null,
            //        new DM.MeasTask(),
            //        DateTime.Now,
            //        "correlationToken"
            //});

            //object o = new MyTaskHandler();
            //var gt = o.GetType().BaseType.GetGenericArguments()[0];
            //var t = o.GetType();
            //var mt = t.GetMethod("OnHandle");
            //mt.Invoke(o, new object[] { obj });

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

            config["RabbitMQ.Host"] = "192.168.33.110";
            config["RabbitMQ.User"] = "andrey";
            config["RabbitMQ.Password"] = "P@ssw0rd";

            config["SDRN.ApiVersion"] = "2.0";

            config["SDRN.Server.Instance"] = "SDRNSV-SBD12-A00-8591";
            config["SDRN.Server.QueueNamePart"] = "Q.SDRN.Server";

            // будет взят из лицензии - изменить будет нельзя
            //config["SDRN.Device.SensorName"] = "my SENSOR-DBD12-A00-1280";
            config["SDRN.Device.SensorTechId"] = "SomeSensor 2.3 SN:00009093";

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

            // При создании гейта происходит вся полня инициализация Rabbit MQ с учетом указанной конфигурации.
            // экземпляр busGate можно использовать в разных потоках, и даже порой нужно
            // т.е. фактически достатчоно его одного на все приложение, 
            // можно обернуть в синглтон на свое усмотрение, главное его потом не забыть освободить так как он может держать конекшены к Rabbit MQ



            // Для получения сообщений необходимо с гейта запросить дисптчера сообщений.
            // Диспетчер ассоциируется содной очередью Rabbit MQ, 
            // он является ее потребителем поступаемых сообщений и по приходу каждого из них диспатчерезирует 
            // передачу сообщения нужному приклданому обработчику. Все обработчик подлежат регистрации
            // Обработчики с точки зрения реализаци могут быть двух видо - делаг или класс реализхующий специальны интерфейс IMessageHandler



            var messageDispatcher = busGate.CreateDispatcher("Main", busObserver);

            // messageDispatcher - этот объект можно ассоцииролвать свыполняемым потоком так как он будет вызван в рамках созданного потака Rabbit MQ
            // это важно учитывать что всвязи с этим все обрпаботчки нге зависимо от типа будут выполнятся  в рамках этого потока
            //  С целью мастобировани янагрузки допускается создание несколко диспетчеров  врамках одного гейта, в этмо случаи они все будут обраюатывать одну и 
            // туже очередь по порядку но в разных потоках. Балансировко йнагрузки иа также кого именно вызвать в каком потоке занимается Rabbit MQ

            // Пример подключения обработчика, ввиде делегата
            messageDispatcher.RegistryHandler<DM.DeviceCommand>("SendCommand", m =>
            {
                // тут код обработки сообщения с типом "SendCommand" и входящем объекте типа "DeviceCommand"
                try
                {
                    if (m.Data.Command == "DropTask")
                    {
                        // подтверждаем обработку
                        m.Result = MessageHandlingResult.Confirmed;
                    }
                    else
                    {
                        // это может быть неизвестная для нас комманда, удаляем такое сообщение
                        m.Result = MessageHandlingResult.Reject;
                        // или можем переместить сообщение в муссорную очередь для учет анестандартных ситуаций
                        m.Result = MessageHandlingResult.Trash;
                        // есть третий вариант, проигнорировать его, тогда сообщение поступить в обработку на другой диспетчер
                        m.Result = MessageHandlingResult.Ignore;
                        // четвертый вариант проигнорировать обработку не меняя стаутс тогда в рамках одного диспетчера сообщение будет пердано следующему обработчику
                        
                    }
                    
                }
                catch(Exception e)
                {
                    // в даннмо случаи от ряда факторов мы можем переправить сообщение в очередь содержаших ошиюочные сообщения
                    m.Result = MessageHandlingResult.Error;
                    // или просто удалить как неудачную обработку - все зависит от логики клиента
                    m.Result = MessageHandlingResult.Reject;
                    // или даже пометить как обработано
                    m.Result = MessageHandlingResult.Confirmed;

                    m.ReasonFailure = e.Message;
                }
                
            });


            // пример подключения обработчка ввиде отдельного класса, 
            messageDispatcher.RegistryHandler(new MyTaskHandler());


            // отправка сообщение осуществляется через публикатор
            // каждый экземпляр публикатора ассоциирован с одним каналом (и соотв. соединене) c Rabbit MQ
            // публикатор также порождается и должен жыть в рамках одного потока
            // публикатор контроллирует соединение и всегда его восттанавливает при отправке сообщения после длительного простоя - Rabbit MQ переодически сбрасывает пассивные соединения
            // т.е. клиенту не нужно явно контроллировать соединения с Rabbit MQ только получаемые от него сообщения и выводить кудато в лог для пользователей

            // пример отправки сообщения сервре: результаты измерения
            var publisher = busGate.CreatePublisher("someTag", busObserver);

            // некий код подготовки результатов
            var measResult = new DM.MeasResults
            {
                TaskId = "SomeTask"
            };

            var msgToken = publisher.Send("SendMeasResults", measResult);

            var sensor = new DM.Sensor
            {
                Antenna = new DM.SensorAntenna
                {
                    AddLoss = 0
                }
            };

            msgToken = publisher.Send("RegisterSensor", sensor);

            // когда паблишер не нужен, нужно его явно закрыть так ка кон держит конекшен с Rabbit MQ
            // можно вызывать несколько раз
            publisher.Dispose();



            // при выгрузки приложения, обязательно явно закрыть дистптчер - он постоянно держит коннект с Rabbitom для получения сообщений
            // он сам отслеживает его состояние,восстанавливает его если нужно и конечно в случаи неудачи извещает обэтом соотвевующим евентом
            // можно вызывать несколько раз, главное вызывать в том потоке в котром он создан был на базе гейта
            messageDispatcher.Dispose();

            

            // при выгрузки приложения, явно закрыть гейт
            // можно вызывать несколько раз
            busGate.Dispose();
        }

        static void Test1()
        {
            Console.WriteLine($"Press [Enter] to start testing ...");
            Console.ReadLine();

            var gateFactory = BusGateFactory.Create();
            var gate = CreateGate(gateFactory);

            var dispatcher = gate.CreateDispatcher("main");
            dispatcher.RegistryHandler(new Handlers.SendMeasTaskHandler(gate));
            dispatcher.RegistryHandler(new Handlers.SendCommandHandler(gate));
            dispatcher.RegistryHandler(new Handlers.SendRegistrationResultHandler(gate));
            dispatcher.RegistryHandler(new Handlers.SendSensorUpdatingResultHandler(gate));
            dispatcher.Activate();


            var publisher = gate.CreatePublisher("main");

            var sensor = new DM.Sensor
            {
                Name = "SENSOR-DBD12-A00-1280", 
                Equipment = new DM.SensorEquipment
                {
                    TechId = "SomeSensor 2.3 SN:00009093"
                }
            };

            for (int i = 0; i < 10000; i++)
            {
                publisher.Send("RegisterSensor", sensor, "Some Correlation ID #" + i.ToString());
            }

            // обязательно все почистить
            publisher.Dispose();
            dispatcher.Deactivate();
            dispatcher.Dispose();

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

            config["RabbitMQ.Host"] = "192.168.33.110";
            config["RabbitMQ.User"] = "andrey";
            config["RabbitMQ.Password"] = "P@ssw0rd";

            config["SDRN.ApiVersion"] = "2.0";

            config["SDRN.Server.Instance"] = "SDRNSV-SBD12-A00-8591";
            config["SDRN.Server.QueueNamePart"] = "Q.SDRN.Server";

            config["SDRN.Device.SensorTechId"] = "SomeSensor 2.3 SN:00009093";
            config["SDRN.Device.Exchange"] = "EX.SDRN.Device";
            config["SDRN.Device.QueueNamePart"] = "Q.SDRN.Device";
            config["SDRN.Device.MessagesBindings"] = "{messageType=RegisterSensor, routingKey=#01};{messageType=SendCommandResult, routingKey=#02};{messageType=SendMeasResults, routingKey=#03};{messageType=SendEntity, routingKey=#04};{messageType=SendEntityPart, routingKey=#05};{messageType=UpdateSensor, routingKey=#06}";
            config["SDRN.MessageConvertor.UseEncryption"] = "true";
            config["SDRN.MessageConvertor.UseСompression"] = "true";

            return config;
        }
        */
    }
}
