using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using Atdi.Icsm.Plugins.SdrnCalcServerClient;
using Atdi.Platform;
using Atdi.Platform.AppServer;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace XICSM.SdrnCalcServerClient
{
    /// <summary>
    /// The class of the Control Client Plugin
    /// </summary>
    public class Plugin : IPlugin
    {
	    private readonly IServerHost _platformHost;
	    private readonly ILogger _logger;
	    private readonly PluginMenuCommands _menuCommands;

        public double SchemaVersion => PluginMetadata.SchemaVersion;

        public string Description => PluginMetadata.Title;

        public string Ident => PluginMetadata.Ident;

        public Plugin()
        {
	        this._platformHost = PlatformConfigurator.GetSingleHost();
	        var resolver = this._platformHost.Container.GetResolver<IServicesResolver>();
	        _logger = resolver.Resolve<ILogger>();
	        _menuCommands = resolver.Resolve<PluginMenuCommands>();

			_logger.Info("CalcServerClient", "Init", "Plugin loaded");
        }

		public void GetMainMenu(IMMainMenu mainMenu)
        {
            mainMenu.SetLocation();
            mainMenu.InsertItem(PluginMetadata.Menu.MainTool, PluginMetadata.Menu.Tools.RunProjectManagerCommand, _menuCommands.OnRunProjectManagerCommand);
            mainMenu.InsertItem(PluginMetadata.Menu.MainTool, PluginMetadata.Menu.Tools.About, _menuCommands.OnAboutCommand);
        }

        public bool OtherMessage(string message, object inParam, ref object outParam)
        {
            return false;
        }
        public void RegisterBoard(IMBoard b)
        {
            //b.RegisterQueryMenuBuilder(MD.Tours.TableName, ToursContextMenuBuilder.Build);
            //b.RegisterQueryMenuBuilder(MD.Allotments.TableName, AllotmentsContextMenuBuilder.Build);
            //b.RegisterQueryMenuBuilder(MD.Inspection.TableName, InspectionsContextMenuBuilder.Build);
            //b.RegisterQueryMenuBuilder(MD.MobStations.TableName, OtherTerrestrialStationsContextMenuBuilder.Build);
            //b.RegisterQueryMenuBuilder(MD.MobStations2.TableName, YetOtherTerrestrialStationsContextMenuBuilder.Build);
        }

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
