using Atdi.Platform;
using Atdi.Platform.AppServer;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using ICSM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.Core
{

	public abstract class PluginBase<TMenuCommands> : IPlugin
	{
		private readonly IServerHost _platformHost;
		private readonly ILogger _logger;
		protected readonly TMenuCommands _menuCommands;

		protected PluginBase(string ident, double schemaVersion, string description)
		{
			this.Ident = ident;
			this.SchemaVersion = schemaVersion;
			this.Description = description;

			this._platformHost = PlatformConfigurator.GetSingleHost();
			var resolver = this._platformHost.Container.GetResolver<IServicesResolver>();
			_logger = resolver.Resolve<ILogger>();
			_menuCommands = resolver.Resolve<TMenuCommands>();

			

			_logger.Info(ident, "Init", "Plugin loaded");
		}

		public double SchemaVersion { get; }

		public string Description { get; }

		public string Ident { get; }

		public void GetMainMenu(IMMainMenu mainMenu)
		{
			mainMenu.SetLocation();
			this.ConnectToMenu(mainMenu);
		}

		protected abstract void ConnectToMenu(IMMainMenu mainMenu);
		

		public bool OtherMessage(string message, object inParam, ref object outParam)
		{
			return false;
		}

		public void RegisterBoard(IMBoard b)
		{
            this.AddBoard(b);
		}
        protected abstract void AddBoard(IMBoard b);

        public void RegisterSchema(IMSchema s)
		{
		}

		public bool UpgradeDatabase(IMSchema s, double dbCurVersion)
		{
			if (dbCurVersion < this.SchemaVersion)
			{
				s.SetDatabaseVersion(this.SchemaVersion);
			}
			return true;
		}
	}
}
