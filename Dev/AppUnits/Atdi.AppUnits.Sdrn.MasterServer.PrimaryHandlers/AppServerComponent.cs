using Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.PipelineHandlers;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers
{
    public sealed class AppServerComponent : AppUnitComponent
    {
        public AppServerComponent()
            : base("SdrnMasterServerPrimaryHandlersAppUnit")
        {

        }

        protected override void OnInstallUnit()
        {
            // регистрация  обрабочика конвеера в DI-окружении
            this.Container.Register<MeasTasksMasterServerSendEventPipelineHandler, MeasTasksMasterServerSendEventPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<CommandsMasterServerSendEventPipelineHandler, CommandsMasterServerSendEventPipelineHandler>(ServiceLifetime.Singleton);
            this.Container.Register<SendEventOnlineMeasurementPipelineHandler, SendEventOnlineMeasurementPipelineHandler>(ServiceLifetime.Singleton);
        }


        protected override void OnActivateUnit()
        {
            var pipelineSite = this.Resolver.Resolve<IPipelineSite>();
            var tasksPipeline = pipelineSite.GetByName<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientMeasTasks);
            var commandsPipeline = pipelineSite.GetByName<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientCommands);
            var InitOnlineMeasurementPipeline = pipelineSite.Declare<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox>(Pipelines.ClientInitOnlineMeasurement);
            // регистрация обработчика
            tasksPipeline.Register(typeof(MeasTasksMasterServerSendEventPipelineHandler), PipelineHandlerRegistrationOptions.Last);
            commandsPipeline.Register(typeof(CommandsMasterServerSendEventPipelineHandler), PipelineHandlerRegistrationOptions.Last);
            InitOnlineMeasurementPipeline.Register(typeof(SendEventOnlineMeasurementPipelineHandler), PipelineHandlerRegistrationOptions.Last);
        }
    }
}
