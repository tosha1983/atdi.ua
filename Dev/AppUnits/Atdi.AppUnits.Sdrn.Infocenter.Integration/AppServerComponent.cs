using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.Infocenter.Integration.CalcServer;
using Atdi.AppUnits.Sdrn.Infocenter.Integration.FilesImport;
using Atdi.AppUnits.Sdrn.Infocenter.Integration.SdrnServer;
using Atdi.AppUnits.Sdrn.Infocenter.Integration.Stations;
using Atdi.Contracts.Sdrn.Infocenter;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.AppServer;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration
{
	public class AppServerComponent : AppUnitComponent
	{
		public AppServerComponent()
			: base("SdrnInfocenterIntegrationAppUnit")
		{

		}

		protected override void OnInstallUnit()
		{
			// Configuration ...
			var componentConfig = this.Config.Extract<AppServerComponentConfig>();
			this.Container.RegisterInstance(componentConfig, ServiceLifetime.Singleton);

			// Jobs ...
			this.Container.Register<SdrnServerSyncJob>(ServiceLifetime.Singleton);
			this.Container.Register<FilesAutoImportJob>(ServiceLifetime.Singleton);

			// Pipelines ...
			this.Container.Register<GlobalIdentityPipelineHandler>(ServiceLifetime.Singleton);
		}

		protected override void OnActivateUnit()
		{
			var typeResolver = this.Resolver.Resolve<ITypeResolver>();
			var appConfig = this.Resolver.Resolve<AppServerComponentConfig>();

			var pipelineSite = this.Resolver.Resolve<IPipelineSite>();

			var filesImportPipeline = pipelineSite.Declare<ImportFileInfo, ImportFileResult>(Pipelines.FilesImport, new ImportFileResult
			{
				Status = ImportFileResultStatus.NotProcessed
			});

			filesImportPipeline.Register(typeof(Stations.GlobalIdentityPipelineHandler));

			var hostLoader = this.Resolver.Resolve<IServerHostLoader>();

			var jobBroker = this.Resolver.Resolve<IJobBroker>();
			hostLoader.RegisterTrigger("Running Jobs ...", () =>
			{
				var startDelaySeconds = appConfig.AutoImportSdrnServerStartDelay;
				if (!startDelaySeconds.HasValue)
				{
					startDelaySeconds = 0;
				}
				var repeatDelaySeconds = appConfig.AutoImportSdrnServerRepeatDelay;
				if (!repeatDelaySeconds.HasValue)
				{
					repeatDelaySeconds = 0;
				}
				var sdrnServerJobDef = new JobDefinition<SdrnServerSyncJob>()
				{
					Name = "SDRN Server Synchronization Job",
					Recoverable = true,
					Repeatable = true,
					StartDelay = new TimeSpan(TimeSpan.TicksPerSecond * startDelaySeconds.Value),
					RepeatDelay = new TimeSpan(TimeSpan.TicksPerSecond * repeatDelaySeconds.Value)
				};

				jobBroker.Run(sdrnServerJobDef);

				startDelaySeconds = appConfig.AutoImportCalcServerStartDelay;
				if (!startDelaySeconds.HasValue)
				{
					startDelaySeconds = 0;
				}
				repeatDelaySeconds = appConfig.AutoImportCalcServerRepeatDelay;
				if (!repeatDelaySeconds.HasValue)
				{
					repeatDelaySeconds = 0;
				}
				var calcServerJobDef = new JobDefinition<CalcServerSyncJob>()
				{
					Name = "Calc Server Synchronization Job",
					Recoverable = true,
					Repeatable = true,
					StartDelay = new TimeSpan(TimeSpan.TicksPerSecond * startDelaySeconds.Value),
					RepeatDelay = new TimeSpan(TimeSpan.TicksPerSecond * repeatDelaySeconds.Value)
				};

				jobBroker.Run(calcServerJobDef);

				startDelaySeconds = appConfig.AutoImportFilesStartDelay;
				if (!startDelaySeconds.HasValue)
				{
					startDelaySeconds = 0;
				}
				repeatDelaySeconds = appConfig.AutoImportFilesRepeatDelay;
				if (!repeatDelaySeconds.HasValue)
				{
					repeatDelaySeconds = 0;
				}
				var filesJobDef = new JobDefinition<FilesAutoImportJob>()
				{
					Name = "Files Auto Import Job",
					Recoverable = true,
					Repeatable = true,
					StartDelay = new TimeSpan(TimeSpan.TicksPerSecond * startDelaySeconds.Value),
					RepeatDelay = new TimeSpan(TimeSpan.TicksPerSecond * repeatDelaySeconds.Value)
				};

				jobBroker.Run(filesJobDef);
			});
		}
	}
}
