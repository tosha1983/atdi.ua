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
            this.Container.Register<ClientMeasTasksPipelineHandler, ClientMeasTasksPipelineHandler>(ServiceLifetime.Singleton);
        }

        protected override void OnActivateUnit()
        {
            var pipelineSite = this.Resolver.Resolve<IPipelineSite>();
            var tasksPipeline = pipelineSite.GetByName<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientMeasTasks);
            // регистрация обработчика
            tasksPipeline.Register(typeof(ClientMeasTasksPipelineHandler), PipelineHandlerRegistrationOptions.First);
        }
    }
}
