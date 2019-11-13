using Atdi.Contracts.Sdrn.Server;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using Atdi.DataModels.Sdrns.Server;
using Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers
{
    public class SdrnServerPrimaryHandlersComponent : AppUnitComponent
    {

        public SdrnServerPrimaryHandlersComponent() 
            : base("SdrnServerPrimaryHandlersAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            var exampleConfig = this.Config.Extract<Configs>();
            this.Container.RegisterInstance(exampleConfig, ServiceLifetime.Singleton);

            // регистрация  обрабочика конвеера в DI-окружении
            this.Container.Register<MeasTasksPipelineHandler, MeasTasksPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<CommandsPipelineHandler, CommandsPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<MeasTasksSendPipelineHandler, MeasTasksSendPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<CommandsSendEventPipelineHandler, CommandsSendEventPipelineHandler>(ServiceLifetime.Singleton);

            //Online
            this.Container.Register<InitOnlineMeasurementPipelineHandler, InitOnlineMeasurementPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<SendEventOnlineMeasurementPipelineHandler, SendEventOnlineMeasurementPipelineHandler>(ServiceLifetime.Singleton);

            
        }

        protected override void OnActivateUnit()
        {
            var pipelineSite = this.Resolver.Resolve<IPipelineSite>();

            // декларация конвейеров
            var tasksPipeline = pipelineSite.Declare<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientMeasTasks);
            var commandsPipeline = pipelineSite.Declare<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientCommands);
            var InitOnlineMeasurementPipeline = pipelineSite.Declare<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox>(Pipelines.ClientInitOnlineMeasurement);
            var SendEventOnlineMeasurementPipeline = pipelineSite.Declare<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox>(Pipelines.ClientSendEventOnlineMeasurement);
            var tasksSendEventsPipeline = pipelineSite.Declare<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientMeasTaskSendEvents);
            var tasksCommandEventsPipeline = pipelineSite.Declare<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientCommandsSendEvents);


            // регистрация обработчиков
            tasksPipeline.Register(typeof(MeasTasksPipelineHandler), PipelineHandlerRegistrationOptions.First);
            commandsPipeline.Register(typeof(CommandsPipelineHandler), PipelineHandlerRegistrationOptions.First);
            tasksSendEventsPipeline.Register(typeof(MeasTasksSendPipelineHandler), PipelineHandlerRegistrationOptions.Last);
            tasksCommandEventsPipeline.Register(typeof(CommandsSendEventPipelineHandler), PipelineHandlerRegistrationOptions.Last);


            //Online
            InitOnlineMeasurementPipeline.Register(typeof(InitOnlineMeasurementPipelineHandler), PipelineHandlerRegistrationOptions.Last);
            SendEventOnlineMeasurementPipeline.Register(typeof(SendEventOnlineMeasurementPipelineHandler), PipelineHandlerRegistrationOptions.Last);
          

        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
