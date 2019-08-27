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
            // регистрация  обрабочика конвеера в DI-окружении
            this.Container.Register<MeasTasksOnAggServerPipelineHandler, MeasTasksOnAggServerPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<CommandsOnAggServerPipelineHandler, CommandsOnAggServerPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<RegistrationAggregationServer, RegistrationAggregationServer>(ServiceLifetime.Singleton);

            this.Container.Register<MeasTasksOnAggServerSendEventPipelineHandler, MeasTasksOnAggServerSendEventPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<ClientCommandsOnAggregationServerSendEventPipelineHandler, ClientCommandsOnAggregationServerSendEventPipelineHandler>(ServiceLifetime.Singleton);
        }

        protected override void OnActivateUnit()
        {
            var hostLoader = this.Resolver.Resolve<IServerHostLoader>();

            hostLoader.RegisterTrigger("Running the procedure registration AggregationServer", () =>
            {
                var messagesProcessing = this.Resolver.Resolve<RegistrationAggregationServer>();
                messagesProcessing.Run();
            });

            var pipelineSite = this.Resolver.Resolve<IPipelineSite>();

            // декларация конвейеров

            var tasksPipeline = pipelineSite.GetByName<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientMeasTasks);
            var commandsPipeline = pipelineSite.GetByName<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientCommands);

            // регистрация обработчика
            tasksPipeline.Register(typeof(MeasTasksOnAggServerPipelineHandler), PipelineHandlerRegistrationOptions.First);
            commandsPipeline.Register(typeof(CommandsOnAggServerPipelineHandler), PipelineHandlerRegistrationOptions.First);

            tasksPipeline.Register(typeof(MeasTasksOnAggServerSendEventPipelineHandler), PipelineHandlerRegistrationOptions.Last);
            commandsPipeline.Register(typeof(ClientCommandsOnAggregationServerSendEventPipelineHandler), PipelineHandlerRegistrationOptions.Last);
        }
    }
}
