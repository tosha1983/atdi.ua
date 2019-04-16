using Atdi.Api.DataBus;
using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.Test.Api.DataBus.DeliveryObjects;
using Atdi.Test.Api.DataBus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Test.Api.DataBus
{
    class Program
    {
        static int _counter = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("Pree any key to start testing ...");
            Console.ReadLine();

            var factory = BusConnector.GateFactory;
            var address1 = "address_1";
            var address2 = "address_2";
            var address3 = "address_3";

            var config1 = BuildConfig(address1, address1, factory);
            var config2 = BuildConfig(address2, address2, factory);
            var config3 = BuildConfig(address3, address3, factory);

            var busEventObserver = new BusEventObserver();

            var gate1 = factory.CreateGate(config1, busEventObserver);
            var gate2 = factory.CreateGate(config2, busEventObserver);
            var gate3 = factory.CreateGate(config3, busEventObserver);


            var publisher1 = gate1.CreatePublisher();
            var publisher2 = gate2.CreatePublisher();
            var publisher3 = gate3.CreatePublisher();

            var dispatcher1 = gate1.CreateDispatcher(new HandlerResolver());
            var dispatcher2 = gate2.CreateDispatcher(new HandlerResolver());
            var dispatcher3 = gate3.CreateDispatcher(new HandlerResolver());

            //Thread.Sleep(5000);
            //Console.WriteLine("Pree any key to next step 2 ...");
            //Console.ReadLine();

            dispatcher1.RegistryHandler(typeof(Handlers.Handler1_1));
            dispatcher1.RegistryHandler(typeof(Handlers.Handler1_2));
            dispatcher1.RegistryHandler(typeof(Handlers.Handler1_3));
            dispatcher1.RegistryHandler(typeof(Handlers.Handler2_1));
            dispatcher1.RegistryHandler(typeof(Handlers.Handler2_2));
            dispatcher1.RegistryHandler(typeof(Handlers.Handler2_3));
            dispatcher1.RegistryHandler(typeof(Handlers.Handler3_1));
            dispatcher1.RegistryHandler(typeof(Handlers.Handler3_2));
            dispatcher1.RegistryHandler(typeof(Handlers.Handler3_3));
            dispatcher1.Activate();

            //Thread.Sleep(60000);
            //Console.WriteLine("Pree any key to next step 3 ...");
            //Console.ReadLine();

            dispatcher2.RegistryHandler(typeof(Handlers.Handler1_1));
            dispatcher2.RegistryHandler(typeof(Handlers.Handler1_2));
            dispatcher2.RegistryHandler(typeof(Handlers.Handler1_3));
            dispatcher2.RegistryHandler(typeof(Handlers.Handler2_1));
            dispatcher2.RegistryHandler(typeof(Handlers.Handler2_2));
            dispatcher2.RegistryHandler(typeof(Handlers.Handler2_3));
            dispatcher2.RegistryHandler(typeof(Handlers.Handler3_1));
            dispatcher2.RegistryHandler(typeof(Handlers.Handler3_2));
            dispatcher2.RegistryHandler(typeof(Handlers.Handler3_3));
            dispatcher2.Activate();

            //Thread.Sleep(60000);
            //Console.WriteLine("Pree any key to next step 4 ...");
            //Console.ReadLine();

            dispatcher3.RegistryHandler(typeof(Handlers.Handler1_1));
            dispatcher3.RegistryHandler(typeof(Handlers.Handler1_2));
            dispatcher3.RegistryHandler(typeof(Handlers.Handler1_3));
            dispatcher3.RegistryHandler(typeof(Handlers.Handler2_1));
            dispatcher3.RegistryHandler(typeof(Handlers.Handler2_2));
            dispatcher3.RegistryHandler(typeof(Handlers.Handler2_3));
            dispatcher3.RegistryHandler(typeof(Handlers.Handler3_1));
            dispatcher3.RegistryHandler(typeof(Handlers.Handler3_2));
            dispatcher3.RegistryHandler(typeof(Handlers.Handler3_3));
            dispatcher3.Activate();

            //Thread.Sleep(60000);
            //Console.WriteLine("Pree any key to start generation some messages");
            //Console.ReadLine();


            var d = new int[3];
            var r = new Random();
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = r.Next();
            }

            // Console.ReadLine();
            var count = 100;

            var tasks = new Task[]
            {
                Task.Run(() =>
                {
                    Test2(address2, publisher1, d, count);
                }),

                Task.Run(() =>
                {
                    Test2(address3, publisher2, d, count);
                }),

                Task.Run(() =>
                {
                    Test2(address1, publisher3, d, count);
                })
            };

            Task.WaitAll(tasks);

            Thread.Sleep(60000);

            Console.WriteLine($"Send messages: {_counter}");

            Console.WriteLine("Pree any key to dispose all objects");
            Console.ReadLine();

            dispatcher3.Deactivate();
            dispatcher3.Dispose();
            dispatcher2.Deactivate();
            dispatcher2.Dispose();
            dispatcher1.Deactivate();
            dispatcher1.Dispose();

            publisher3.Dispose();
            publisher2.Dispose();
            publisher1.Dispose();

            Console.ReadLine();

        }

        private static void Test2(string address, IPublisher publisher, int[] d, int count)
        {
            var deliveryObject1 = new DeliveryObjects.Address1DeliveryObject
            {
                Data = d
            };
            Test1<CommonMessageType1, Address1DeliveryObject>(count, publisher, address, deliveryObject1);
            Test1<CommonMessageType2, Address1DeliveryObject>(count, publisher, address, deliveryObject1);
            Test1<CommonMessageType3, Address1DeliveryObject>(count, publisher, address, deliveryObject1);

            var deliveryObject2 = new DeliveryObjects.Address2DeliveryObject
            {
                Data = d
            };
            Test1<PrivateMessageType1, Address2DeliveryObject>(count, publisher, address, deliveryObject2);
            Test1<PrivateMessageType2, Address2DeliveryObject>(count, publisher, address, deliveryObject2);
            Test1<PrivateMessageType3, Address2DeliveryObject>(count, publisher, address, deliveryObject2);

            var deliveryObject3 = new DeliveryObjects.Address3DeliveryObject
            {
                Data = d
            };
            Test1<SpecificMessageType1, Address3DeliveryObject>(count, publisher, address, deliveryObject3);
            Test1<SpecificMessageType2, Address3DeliveryObject>(count, publisher, address, deliveryObject3);
            Test1<SpecificMessageType3, Address3DeliveryObject>(count, publisher, address, deliveryObject3);
        }

        private static void Test1<TMessageType, TDeliveryObject>(int count, IPublisher publisher, string to, TDeliveryObject deliveryObject)
            where TMessageType : IMessageType, new()
        {
            for (int i = 0; i < count; i++)
            {
                var envelope = publisher.CreateEnvelope<TMessageType, TDeliveryObject>();
                envelope.To = to;

                envelope.DeliveryObject = default(TDeliveryObject);

                envelope.Options = SendingOptions.UseBuffer;
                SendProcessing(publisher, envelope);

                envelope.Options = SendingOptions.UseBuffer | SendingOptions.UseCompression;
                SendProcessing(publisher, envelope);

                envelope.Options = SendingOptions.UseBuffer | SendingOptions.UseEncryption;
                SendProcessing(publisher, envelope);

                envelope.Options = SendingOptions.UseBuffer | SendingOptions.UseCompression | SendingOptions.UseEncryption;
                SendProcessing(publisher, envelope);

                envelope.DeliveryObject = deliveryObject;

                envelope.Options = SendingOptions.UseBuffer;
                SendProcessing(publisher, envelope);

                envelope.Options = SendingOptions.UseBuffer | SendingOptions.UseCompression;
                SendProcessing(publisher, envelope);

                envelope.Options = SendingOptions.UseBuffer | SendingOptions.UseEncryption;
                SendProcessing(publisher, envelope);

                envelope.Options = SendingOptions.UseBuffer | SendingOptions.UseCompression | SendingOptions.UseEncryption;
                SendProcessing(publisher, envelope);
            }
        }

        private static void SendProcessing<TMessageType, TDeliveryObject>(IPublisher publisher, IOutgoingEnvelope<TMessageType, TDeliveryObject> envelope) where TMessageType : IMessageType, new()
        {
            envelope.ContentType = ContentType.Binary;
            publisher.Send(envelope);
            Interlocked.Increment(ref _counter);
            envelope.ContentType = ContentType.Xml;
            publisher.Send(envelope);
            Interlocked.Increment(ref _counter);
            envelope.ContentType = ContentType.Json;
            publisher.Send(envelope);
            Interlocked.Increment(ref _counter);
        }

        static IBusConfig BuildConfig(string address, string folder, IBusGateFactory gateFactory)
        {
            var config = gateFactory.CreateConfig();

            config.Address = address;
            config.ApiVersion = "1.0";
            config.Name = "SDRN";
            config.Host = "192.168.33.110";
            config.VirtualHost = "DataBusApiTest";
            config.User = "andrey";
            config.Password = "P@ssw0rd";
            config.Buffer.Type = BufferType.Filesystem;
            config.Buffer.OutboxFolder = @"C:\Temp\DataBusOutBox\" + folder;
            config.Buffer.ContentType = ContentType.Binary;

            return config;
        }
    }
}
