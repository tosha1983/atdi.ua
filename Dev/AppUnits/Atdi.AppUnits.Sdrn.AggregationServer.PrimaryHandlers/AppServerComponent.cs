﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;
using Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.PipelineHandlers;
using Atdi.Platform.Workflows;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.AppServer;
using Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers;
using Atdi.Platform.AppComponent;
using Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Subscribes;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers
{
    public sealed class AppServerComponent : AppUnitComponent
    {
        public AppServerComponent()
            : base("SdrnAggregationServerPrimaryHandlersAppUnit")
        {

        }

        protected override void OnInstallUnit()
        {
            var serverConfig = this.Config.Extract<AppComponentConfig>();
            // регистрация  обрабочика конвеера в DI-окружении
            this.Container.Register<RegistrationAggregationServer, RegistrationAggregationServer>(ServiceLifetime.Singleton);
            this.Container.Register<RegisterSensorPipelineHandler, RegisterSensorPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<OnDeviceCommandPipelineHandler, OnDeviceCommandPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<OnlineMeasOnAggServerPipelineHandler, OnlineMeasOnAggServerPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<MeasTasksOnAggServerSendEventPipelineHandler, MeasTasksOnAggServerSendEventPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<ClientCommandsOnAggregationServerSendEventPipelineHandler, ClientCommandsOnAggregationServerSendEventPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<MeasResultWorker, MeasResultWorker>(ServiceLifetime.Singleton);
            this.Container.RegisterInstance(serverConfig, ServiceLifetime.Singleton);
        }

        protected override void OnActivateUnit()
        {
            var hostLoader = this.Resolver.Resolve<IServerHostLoader>();

            hostLoader.RegisterTrigger("Running the procedure registration AggregationServer", () =>
            {
                var messagesProcessing = this.Resolver.Resolve<RegistrationAggregationServer>();
                messagesProcessing.Run();
            });

            hostLoader.RegisterTrigger("Running the procedure aggergation MeasResult", () =>
            {
                var measResultProcessing = this.Resolver.Resolve<MeasResultWorker>();
                measResultProcessing.Run();
            });

            var pipelineSite = this.Resolver.Resolve<IPipelineSite>();

            // декларация конвейеров

            var tasksPipeline = pipelineSite.GetByName<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientMeasTasks);
            var commandsPipeline = pipelineSite.GetByName<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientCommands);
            var InitOnlineMeasurementPipeline = pipelineSite.Declare<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox>(Pipelines.ClientInitOnlineMeasurement);
            var tasksSendEventsPipeline = pipelineSite.Declare<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientMeasTaskSendEvents);
            var tasksCommandEventsPipeline = pipelineSite.Declare<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientCommandsSendEvents);
            var registerAggregationServerPipeline = pipelineSite.Declare<RegisterSensorSendEvent, RegisterSensorSendEvent>(Pipelines.ClientRegisterAggregationServer);
            var deviceCommandAggregationServerPipeline = pipelineSite.Declare<DeviceCommandResultEvent, DeviceCommandResultEvent>(Pipelines.ClientDeviceCommandAggregationServer);


            tasksSendEventsPipeline.Register(typeof(MeasTasksOnAggServerSendEventPipelineHandler), PipelineHandlerRegistrationOptions.First);
            tasksCommandEventsPipeline.Register(typeof(ClientCommandsOnAggregationServerSendEventPipelineHandler), PipelineHandlerRegistrationOptions.First);

            InitOnlineMeasurementPipeline.Register(typeof(OnlineMeasOnAggServerPipelineHandler), PipelineHandlerRegistrationOptions.First);
            registerAggregationServerPipeline.Register(typeof(RegisterSensorPipelineHandler), PipelineHandlerRegistrationOptions.First);
            deviceCommandAggregationServerPipeline.Register(typeof(OnDeviceCommandPipelineHandler), PipelineHandlerRegistrationOptions.First);
        }
    }
}
