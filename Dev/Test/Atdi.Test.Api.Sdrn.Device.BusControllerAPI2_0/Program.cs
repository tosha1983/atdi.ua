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
        
        static void Test1()
        {
            Console.WriteLine($"Press [Enter] to start testing ...");
            Console.ReadLine();

            var gateFactory = BusGateFactory.Create();
            var gate = CreateGate(gateFactory);

            var dispatcher = gate.CreateDispatcher("main");
            dispatcher.RegistryHandler(new SendMeasTaskHandler(gate));
            dispatcher.RegistryHandler(new SendCommandHandler(gate));
            dispatcher.RegistryHandler(new SendRegistrationResultHandler(gate));
            dispatcher.RegistryHandler(new SendSensorUpdatingResultHandler(gate));
            dispatcher.Activate();



            var publisher = gate.CreatePublisher("main");
            //Пример регистрации сенсора
            // в параметре Name нужно указать Instance лицензии (т.е он должен совпадать с Instance лицензии)
            // в параметре TechId - произвольное значение
            var sensor = new DM.Sensor
            {
                Antenna = new DM.SensorAntenna
                {
                    AddLoss = 0
                },
                Equipment = new DM.SensorEquipment
                {
                    TechId = "SomeSensor2"
                },
                Name = "SENSOR-DBD12-A00-1280"

            };
            var msgTokenRegisterSensor = publisher.Send("RegisterSensor", sensor);


            //Пример обновления сенсора
            // в параметре Name нужно указать Instance лицензии (т.е он должен совпадать с Instance лицензии)
            // в параметре TechId - произвольное значение 
            //
            var updatesensor = new DM.Sensor
            {
                Antenna = new DM.SensorAntenna
                {
                    AddLoss = 10,
                    Category = "Cat"
                },
                Equipment = new DM.SensorEquipment
                {
                    TechId = "SomeSensor2"
                },
                Name = "SENSOR-DBD12-A00-1280"

            };
           
            var msgTokenUpdateSensor = publisher.Send("UpdateSensor", updatesensor);


            //Пример отправки результатов
            var measResult = new DM.MeasResults
            {
                TaskId = "SomeTask"
            };
            var msgTokenSendMeasResults = publisher.Send("SendMeasResults", measResult);


            //Пример отправки объекта Entity
            var entity = new DM.Entity
            {
                Content = new byte[2000],   // содержимое сообщения
                ContentType = "ContentType",//тип содержимого    
                Description = "Descr",      //вспомогательное описание
                Encoding = "UTF-8",         //кодировка
                EntityId = "IDENT01",       // идентификатор Entity
                EOF = false,                // признак последней части сообщения
                HashAlgorithm = "RSA",      // алгоритм шифрования
                HashCode = "Code",          // некоторый HashCode
                Name = "Name",              // Наименование объекта
                ParentId = "ParentId",      // id группы сообщений (для отслеживания все частей)
                ParentType = "ParentType",  // тип группы сообщений (для отслеживания все частей)
                PartIndex = 1 //номер части сообщения 
            };
            var msgTokenSendentity = publisher.Send("SendEntity", measResult);

            //Пример отправки объекта EntityPart
            var pertEntity = new DM.EntityPart
            {
                Content = new byte[2000],  // содержимое сообщения
                EntityId = "EntityId",     // идентификатор Entity
                PartIndex = 2,             //номер части сообщения 
                EOF = false                // признак последней части сообщения
            };
           var msgTokenSendEntityPart = publisher.Send("SendEntityPart", pertEntity);

            publisher.Dispose();

            //********** ВАЖНО ****************
            // деактивацию и закрытие диспетчера (прослушивателя сообщений на шине) следует выполнять только при завершении работы приложения иначе сообщения из очереди не извлекаются
            //т.е. фрагмент кода, который представлен ниже работать не будет если по ходу выполнения программы будет:
            //dispatcher.RegistryHandler(new SendMeasTaskHandler(gate));
            //dispatcher.RegistryHandler(new SendCommandHandler(gate));
            //dispatcher.RegistryHandler(new SendRegistrationResultHandler(gate));
            //dispatcher.RegistryHandler(new SendSensorUpdatingResultHandler(gate));
            //dispatcher.Deactivate();
            //dispatcher.Dispose();
            //********** ВАЖНО ****************
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

            config["License.FileName"] = "LIC-DBD12-A00-187.SENSOR-DBD12-A00-1280.lic";  // наименование файла лицензии
            config["License.OwnerId"] = "OID-BD12-A00-N00";//  OwnerId лицензии
            config["License.ProductKey"] = "0ZB0-DVZR-ATI1-WIHB-NC1B";// ProductKey лицензии
            //config["RabbitMQ.Host"] = "109.237.91.29";//  IP RabbitMQ
            config["RabbitMQ.Host"] = "10.1.2.129";//  IP RabbitMQ
            config["RabbitMQ.User"] = "SDR_Client";//  User Name RabbitMQ
            config["RabbitMQ.Password"] = "32Xr567";//  Password RabbitMQ
            config["SDRN.Device.SensorTechId"] = "SomeSensor"; // здесь произвольное наименование  SensorTechId


            config["SDRN.ApiVersion"] = "2.0"; // версия АПИ - не менять
            config["SDRN.Server.Instance"] = "ServerSDRN01"; // здесь имя сервера SDRN (т.е. серверов может быть много) (не менять)
            config["SDRN.Server.QueueNamePart"] = "Q.SDRN.Server";   //- это часть наименования конечной очереди в которую будут приходить сообщения от сенсора сервису SDRN (путь следования такой  EX.SDRN.Device.[v2.0] ->Q.SDRN.Server.[ServerSDRN01].[#01].[v2.0])  - не изменять
            config["SDRN.Device.Exchange"] = "EX.SDRN.Device"; // здесь часть имени точки обмена EX.SDRN.Device.[v2.0] - не изменять
            config["SDRN.Device.QueueNamePart"] = "Q.SDRN.Device"; // здесь часть имени конечной очереди в которую вервис SDRN отправляет данные сенсору (путь следования такой EX.SDRN.Server.[v2.0] ->  Q.SDRN.Device.[SENSOR-DBD12-A00-1280].[SomeSensor].[v2.0]) - не изменять
            config["SDRN.Device.MessagesBindings"] = "{messageType=RegisterSensor, routingKey=#01};{messageType=SendCommandResult, routingKey=#02};{messageType=SendMeasResults, routingKey=#03};{messageType=SendEntity, routingKey=#04};{messageType=SendEntityPart, routingKey=#05};{messageType=UpdateSensor, routingKey=#06}"; // здесь наименования подочередей для обработки сообщений конкретного типа (фактически создаются очереди Q.SDRN.Server.[ServerSDRN01].[#01].[v2.0], Q.SDRN.Server.[ServerSDRN01].[#02].[v2.0], Q.SDRN.Server.[ServerSDRN01].[#03].[v2.0], Q.SDRN.Server.[ServerSDRN01].[#04].[v2.0], Q.SDRN.Server.[ServerSDRN01].[#05].[v2.0], Q.SDRN.Server.[ServerSDRN01].[#06].[v2.0]) - не изменять

            // Шифрование и компресия сообщений
            config["SDRN.MessageConvertor.UseEncryption"] = "true"; //признак шифрования сообщения (на данный момент пока не внедрено в сервис, но в конфигурации должно быть)
            config["SDRN.MessageConvertor.UseСompression"] = "true"; //признак упаковки сообщения (на данный момент пока не внедрено в сервис, но в конфигурации должно быть)

            return config;
        }
    }
}
