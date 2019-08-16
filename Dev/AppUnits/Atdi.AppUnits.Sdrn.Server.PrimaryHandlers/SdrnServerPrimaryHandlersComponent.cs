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
            this.Container.Register<ClientMeasTasksPipelineHandler, ClientMeasTasksPipelineHandler>(ServiceLifetime.Singleton);
        }

        protected override void OnActivateUnit()
        {
            var pipelineSite = this.Resolver.Resolve<IPipelineSite>();

            // декларация конвейера
            var tasksPipeline = pipelineSite.Declare<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientMeasTasks);
            var commandsPipeline = pipelineSite.Declare<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientCommands);
            
            // регистрация обработчика
            tasksPipeline.Register(typeof(ClientMeasTasksPipelineHandler), PipelineHandlerRegistrationOptions.Last);
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
