﻿using Atdi.Api.DataBus;
using Atdi.Contracts.Api.DataBus;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Api.DataBus;
using Atdi.DataModels.Sdrns.Server.Test;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.AppServer;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.MasterServer
{
    public sealed class AppServerComponent : AppUnitComponent
    {
        

        public AppServerComponent()
            : base("SdrnMasterServerAppUnit")
        {

        }

        protected override void OnInstallUnit()
        {
            var environmentModifier = this.Resolver.Resolve<ISdrnServerEnvironmentModifier>();
            environmentModifier.AddServerRole(ServerRole.MasterServer);
            var environment = this.Resolver.Resolve<ISdrnServerEnvironment>();

            var serverConfig = this.Config.Extract<AppServerComponentConfig>();
            var dataBusConfig = BusConnector.GateFactory.CreateConfig();

            //dataBusConfig.Address = "MasterServer";
            dataBusConfig.Address = environment.ServerInstance;
            dataBusConfig.ApiVersion = serverConfig.DataBusApiVersion;
            if (!string.IsNullOrEmpty(serverConfig.DataBusUseBuffer))
            {
                if (BufferType.Filesystem.ToString().Equals(serverConfig.DataBusUseBuffer, StringComparison.OrdinalIgnoreCase))
                {
                    dataBusConfig.Buffer.Type = BufferType.Filesystem;
                    dataBusConfig.Buffer.ContentType = (ContentType)Enum.Parse(typeof(ContentType), serverConfig.DataBusBufferContentType);
                    dataBusConfig.Buffer.OutboxFolder = serverConfig.DataBusBufferOutboxFolder;
                }
                else if (BufferType.Database.ToString().Equals(serverConfig.DataBusUseBuffer, StringComparison.OrdinalIgnoreCase))
                {
                    dataBusConfig.Buffer.Type = BufferType.Database;
                    dataBusConfig.Buffer.ContentType = (ContentType)Enum.Parse(typeof(ContentType), serverConfig.DataBusBufferContentType);
                    dataBusConfig.Buffer.ConnectionString = serverConfig.DataBusBufferConnectionStringConfig;
                }

            }
            else
            {
                dataBusConfig.Buffer.Type = BufferType.None;
            }

            dataBusConfig.ContentType = (ContentType)Enum.Parse(typeof(ContentType), serverConfig.DataBusContentType);
            dataBusConfig.Host = serverConfig.DataBusHost;
            dataBusConfig.Name = serverConfig.DataBusName;
            dataBusConfig.Password = serverConfig.DataBusPassword;
            dataBusConfig.Port = serverConfig.DataBusPort;
            dataBusConfig.UseCompression = serverConfig.DataBusUseCompression;
            dataBusConfig.UseEncryption = serverConfig.DataBusUseEncryption;
            dataBusConfig.User = serverConfig.DataBusUser;
            dataBusConfig.VirtualHost = serverConfig.DataBusVirtualHost;

            this.Container.Register<IBusEventObserver, DataBusEventObserver>(ServiceLifetime.Singleton);
            this.Container.Register<IMessageHandlerResolver, DataBusHandlerResolver>(ServiceLifetime.Singleton);

            var dataBusEventObserver = this.Resolver.Resolve<IBusEventObserver>();
            var dataBusHandlerResolver = this.Resolver.Resolve<IMessageHandlerResolver>();

            var dataBusGate = BusConnector.GateFactory.CreateGate(dataBusConfig, dataBusEventObserver);

            var dataBusPublisher = dataBusGate.CreatePublisher();
            var dataBusDispatcher = dataBusGate.CreateDispatcher(dataBusHandlerResolver);

            this.Container.RegisterInstance<IBusGate>(dataBusGate);
            this.Container.RegisterInstance<IPublisher>(dataBusPublisher);
            this.Container.RegisterInstance<IDispatcher>(dataBusDispatcher);

        }

        protected override void OnActivateUnit()
        {
            var typeResolver = this.Resolver.Resolve<ITypeResolver>();

            var dataBusDispatcher = this.Resolver.Resolve<IDispatcher>();
            var handlerTypes = typeResolver.GetTypesByInterface(typeof(IMessageHandler<,>));
            foreach (var handlerType in handlerTypes)
            {
                this.Container.Register(handlerType, handlerType, ServiceLifetime.PerThread);
                dataBusDispatcher.RegistryHandler(handlerType);
            }

            Logger.Info(Contexts.ThisComponent, Categories.Initilazing, Events.HandlerTypesWereRegistered);
            var hostLoader = this.Resolver.Resolve<IServerHostLoader>();
            hostLoader.RegisterTrigger("DataBus Dispatcher Activating", () =>
            {
                dataBusDispatcher.Activate();
                Logger.Info(Contexts.ThisComponent, Categories.Activating, Events.DispatcherWasActivated);

                ////some test
                //var publisher = this.Resolver.Resolve<IPublisher>();
                //var envelope = publisher.CreateEnvelope<TestMessage1, TestDeliveryObject1>();
                //envelope.DeliveryObject = new TestDeliveryObject1
                //{
                //    Index = 1,
                //    Data = "MasterServer.Start"
                //};
                //envelope.To = "MasterServer";
                //envelope.Options |= SendingOptions.UseCompression | SendingOptions.UseEncryption;
                //envelope.ContentType = ContentType.Binary
                //for (int i = 0; i < 10000; i++)
                //{
                //    publisher.Send(envelope);
                //}

            });

        }
    }
}
