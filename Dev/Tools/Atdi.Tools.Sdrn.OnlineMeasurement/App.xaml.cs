using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Atdi.Platform.AppServer;
using Atdi.Platform;
using Atdi.Platform.DependencyInjection;

namespace Atdi.Tools.Sdrn.OnlineMeasurement
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private IServerHost _platformHost;
		private ILogger _logger;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			this._platformHost = PlatformConfigurator.GetSingleHost();
			this._platformHost.Container.Register<MainWindow>(ServiceLifetime.Transient);

			var resolver = this._platformHost.Container.GetResolver<IServicesResolver>();
			_logger = resolver.Resolve<ILogger>();

			_logger.Info("App", "Init", "Application loaded");


			var mainWindow = resolver.Resolve<MainWindow>();
			this.MainWindow = mainWindow;
			this.MainWindow?.Show();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			_logger?.Info("App", "Exit", "Application will be closed");
			_logger = null;
			this._platformHost.Stop();
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			_logger?.Exception((EventContext)"App", (EventCategory)"Dispatcher", e.Exception);
		}

		private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			_logger?.Info("App", "LoadCompleted", "Application has been loaded");
		}
	}
}
